using DotHass.Tools.ExcelExport.Abstractions;
using DotHass.Tools.ExcelExport.Exporter;
using ExcelDataReader;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace DotHass.Tools.ExcelExport
{
    public class ExportManager
    {
        private AppSettings options;
        private PhysicalFileProvider FileProvider;

        public List<Func<ExportOptions, AbstractionExport>> ExportFactory = new List<Func<ExportOptions, AbstractionExport>>();

        public ExportManager(AppSettings options)
        {
            this.options = options;

            if (string.IsNullOrEmpty(options.JsonPath) == false)
            {
                ExportFactory.Add((exportOptions) =>
                {
                    return new JsonExporter(options.Lowcase, options.JsonMode, options.JsonPath, exportOptions);
                });
            }

            if (string.IsNullOrEmpty(options.CSharpPath) == false)
            {
                ExportFactory.Add((exportOptions) =>
                {
                    return new CsharpExporter((string)options.CSharpTemplatePath, options.CSharpPath, exportOptions);
                });
            }

            if (string.IsNullOrEmpty(options.SQLPath) == false)
            {
                ExportFactory.Add((exportOptions) =>
                {
                    return new SQLExporter(options.SQLCreateTable, options.SQLPath, exportOptions);
                });
            }
        }

        public void Export()
        {
            var excelPath = options.ExcelPath;

            if (File.Exists(excelPath))
            {
                this.ExportFile(excelPath);
            }
            else
            {
                FileProvider = new PhysicalFileProvider(excelPath);
                this.Export("");
            }
        }

        public void Export(string subpath)
        {
            var contents = FileProvider.GetDirectoryContents(subpath);

            if (contents.Exists == true)
            {
                foreach (var file in contents)
                {
                    if (file.IsDirectory)
                    {
                        //排除存在的目录
                        if (options.Excludes.Contains(file.PhysicalPath))
                        {
                            options.Excludes.Remove(file.PhysicalPath);
                            continue;
                        }
                        this.Export(Path.GetRelativePath(FileProvider.Root, file.PhysicalPath));
                    }
                    else
                    {
                        this.ExportFile(file.PhysicalPath);
                    }
                }
            }
            else
            {
                if (FileProvider.GetFileInfo(subpath).Exists == true)
                {
                    this.ExportFile(subpath);
                }
            }
        }

        /// <summary>
        /// 加载Excel文件
        /// </summary>
        /// <param name="options">导入设置</param>
        public void ExportFile(string excelPath)
        {
            string ext = Path.GetExtension(excelPath);
            if (ext.Contains("xlsx") == false)
            {
                return;
            }

            string excelName = Path.GetFileNameWithoutExtension(excelPath);
            try
            {
                // 加载Excel文件
                using (FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx)
                    using (var reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
                    {
                        // Gets or sets the encoding to use when the input XLS lacks a CodePage
                        // record. Default: cp1252. (XLS BIFF2-5 only)
                        FallbackEncoding = Encoding.GetEncoding(1252),

                        // Gets or sets the password used to open password protected workbooks.
                        //Password = "password"
                    }))
                    {
                        if (reader == null)
                            return;

                        var exportOptions = new ExportOptions
                        {
                            Data = this.GetTableData(reader, excelPath),
                            Schema = this.GetTableDefine(reader, excelPath),
                            Encode = options.Encode,
                            Name = excelName
                        };

                        foreach (var item in ExportFactory)
                        {
                            var exporter = item(exportOptions);
                            exporter.Parse();
                            exporter.SaveToFile();
                        }
                    }
                }

                Console.WriteLine($"export {excelName} success");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"export {excelName} fail" + ex);
            }
        }

        private DataTable GetTableData(IExcelDataReader reader, string excelPath)
        {
            var book = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                // Gets or sets a value indicating whether to set the DataColumn.DataType
                // property in a second pass.
                UseColumnDataType = true,

                // Gets or sets a callback to obtain configuration options for a DataTable.
                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                {
                    // Gets or sets a value indicating the prefix of generated column names.
                    EmptyColumnNamePrefix = "Column",

                    // Gets or sets a value indicating whether to use a row from the
                    // data as column names.
                    UseHeaderRow = true,

                    // Gets or sets a callback to determine which row is the header row.
                    // Only called when UseHeaderRow = true.
                    ReadHeaderRow = (rowReader) =>
                    {
                        // F.ex skip the first row and use the 2nd row as column headers:
                        for (int i = 0; i < options.HeaderRows - 1; i++)
                        {
                            rowReader.Read();
                        }
                    },

                    // Gets or sets a callback to determine whether to include the
                    // current row in the DataTable.
                    FilterRow = (IExcelDataReader rowReader) =>
                    {
                        if (options.FilterRow == false)
                        {
                            return true;
                        }
                        //如果第一行是空或者包含注释“//”,代表这行数据无效则跳过
                        return filterTable(rowReader, 0);
                    },

                    FilterColumn = (IExcelDataReader rowReader, int index) =>
                    {
                        if (options.FilterCol == false)
                        {
                            return true;
                        }
                        //如果头是空,或者包含注释,代表这列数据无效
                        return filterTable(rowReader, index);
                    },
                }
            });

            if (book.Tables.Count < 1)
            {
                throw new Exception("Excel file is empty: " + excelPath);
            }
            DataTable sheet = book.Tables[0];
            return sheet;
        }

        private bool filterTable(IExcelDataReader rowReader, int index)
        {
            if (rowReader.GetFieldType(index) == null)
            {
                return false;
            }
            else if (rowReader.GetFieldType(index) == typeof(string))
            {
                var value = rowReader.GetString(index);
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) || value.Contains("//") == true)
                {
                    return false;
                }
            }
            return true;
        }

        private DataTable GetTableDefine(IExcelDataReader reader, string excelPath)
        {
            var book = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                // Gets or sets a value indicating whether to set the DataColumn.DataType
                // property in a second pass.
                UseColumnDataType = true,

                // Gets or sets a callback to obtain configuration options for a DataTable.
                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                {
                    // Gets or sets a value indicating the prefix of generated column names.
                    EmptyColumnNamePrefix = "Column",

                    // Gets or sets a value indicating whether to use a row from the
                    // data as column names.
                    UseHeaderRow = false,

                    // Gets or sets a callback to determine which row is the header row.
                    // Only called when UseHeaderRow = true.
                    ReadHeaderRow = (rowReader) =>
                    {
                    },

                    // Gets or sets a callback to determine whether to include the
                    // current row in the DataTable.
                    FilterRow = (rowReader) =>
                    {
                        if (rowReader.Depth >= options.HeaderRows)
                        {
                            return false;
                        }
                        return true;
                    },

                    FilterColumn = (IExcelDataReader rowReader, int index) =>
                    {
                        if (options.FilterCol == false)
                        {
                            return true;
                        }
                        //如果头是空,或者包含注释,代表这列数据无效
                        return filterTable(rowReader, index);
                    },
                }
            });

            if (book.Tables.Count < 1)
            {
                throw new Exception("Excel file is empty: " + excelPath);
            }
            DataTable sheet = book.Tables[0];
            if (sheet.Rows.Count <= 0)
            {
                return null;
            }

            return sheet;
        }

        /// <summary>
        /// 对于表格中的空值，找到一列中的非空值，并构造一个同类型的默认值
        /// 所以第一行的数据最好是填满,不要填空数据
        /// </summary>
        public static object GetColumnValue(object value, string type)
        {
            if (value.GetType() == typeof(System.DBNull) || string.IsNullOrEmpty(value.ToString()) || string.IsNullOrWhiteSpace(value.ToString()))
            {
                switch (type)
                {
                    case "string":
                        value = "";
                        break;

                    case "int":
                        value = 0;
                        break;

                    case "double":
                        value = 0;
                        break;

                    case "float":
                        value = 0;
                        break;

                    default:
                        value = "";
                        break;
                }
            }
            else if (value.GetType() == typeof(double))
            { // 去掉数值字段的“.0”
                double num = (double)value;
                if ((int)num == num)
                    value = (int)num;
            }

            return value;
        }
    }
}