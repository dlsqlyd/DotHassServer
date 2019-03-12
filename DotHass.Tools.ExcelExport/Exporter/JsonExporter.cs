using DotHass.Tools.ExcelExport.Abstractions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace DotHass.Tools.ExcelExport.Exporter
{
    /// <summary>
    /// 将DataTable对象，转换成JSON string，并保存到文件中
    /// </summary>
    internal class JsonExporter : AbstractionExport
    {
        protected override string outExtension { get { return "json"; } }

        protected bool Lowcase { get; private set; }
        protected JsonMode Mode { get; private set; }

        public JsonExporter(bool lowcase, JsonMode mode, string outdir, ExportOptions options) : base(outdir, options)
        {
            this.Lowcase = lowcase;
            this.Mode = mode;
        }

        /// <summary>
        /// 构造函数：完成内部数据创建
        /// </summary>
        /// <param name="sheet">ExcelReader创建的一个表单</param>
        /// <param name="headerRows">表单中的那几行是表头</param>
        public override void Parse()
        {
            if (options.Data.Columns.Count <= 0)
                return;

            object value = null;
            switch (Mode)
            {
                case JsonMode.Dictonary:
                    value = ConvertDictonary();
                    break;

                case JsonMode.Array:
                    value = ConvertArray();
                    break;

                case JsonMode.Detailed:
                    value = new Dictionary<string, object> {
                        {options.Name,new Dictionary<string, object>
                        {
                            {"Local",ConvertDictonary() }
                        }}
                    };
                    break;
            }
            //-- convert to json string
            content = JsonConvert.SerializeObject(value, Formatting.Indented);
        }

        private object ConvertArray()
        {
            List<object> values = new List<object>();

            foreach (DataRow row in options.Data.Rows)
            {
                values.Add(ConvertRowData(row));
            }
            return values;
        }

        /// <summary>
        /// 以第一列为ID，转换成ID->Object的字典对象
        /// </summary>
        private object ConvertDictonary()
        {
            Dictionary<string, object> importData = new Dictionary<string, object>();
            DataColumn IDColumn = options.Data.Columns[0];
            foreach (DataRow row in options.Data.Rows)
            {
                importData.Add(row[IDColumn].ToString(), ConvertRowData(row));
            }
            return importData;
        }

        /// <summary>
        /// 把一行数据转换成一个对象，每一列是一个属性
        /// </summary>
        private object ConvertRowData(DataRow row)
        {
            var rowData = new Dictionary<string, object>();

            foreach (DataColumn column in options.Data.Columns)
            {
                var data = row[column];
                var type = typeRow[column.Ordinal].ToString();

                rowData[GetColumnName(column)] = ExportManager.GetColumnValue(data, type);
            }

            return rowData;
        }

        public string GetColumnName(DataColumn column)
        {
            string fieldName = column.ToString();
            // 表头自动转换成小写
            if (Lowcase)
                fieldName = fieldName.ToLower();
            return fieldName;
        }
    }
}