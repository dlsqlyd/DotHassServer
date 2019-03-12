using DotHass.Tools.ExcelExport.Abstractions;
using System.Data;
using System.Text;

namespace DotHass.Tools.ExcelExport.Exporter
{
    internal class SQLExporter : AbstractionExport
    {
        private string mStructSQL;
        private string mContentSQL;
        private bool createTable;

        protected override string content { get { return mStructSQL + "\n" + mContentSQL; } }

        protected override string outExtension { get { return "sql"; } }

        public SQLExporter(bool createTable, string outdir, ExportOptions options) : base(outdir, options)
        {
            this.createTable = createTable;
        }

        /// <summary>
        /// 初始化内部数据
        /// </summary>
        /// <param name="sheet">Excel读取的一个表单</param>
        /// <param name="headerRows">表头有几行</param>
        public override void Parse()
        {
            //-- 转换成SQL语句
            mStructSQL = GetTabelStructSQL();
            mContentSQL = GetTableContentSQL();
        }

        /// <summary>
        /// 将表单内容转换成INSERT语句
        /// </summary>
        private string GetTableContentSQL()
        {
            StringBuilder sbContent = new StringBuilder();
            StringBuilder sbNames = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();

            sbContent.AppendFormat("TRUNCATE TABLE `{0}`;\n", options.Name);
            sbContent.AppendFormat("LOCK TABLES `{0}` WRITE;\n", options.Name);

            var sheet = options.Data;
            //-- 字段名称列表
            foreach (DataColumn column in sheet.Columns)
            {
                if (sbNames.Length > 0)
                    sbNames.Append(", ");
                sbNames.Append("`" + column.ToString() + "`");
            }

            //-- 逐行转换数据
            for (int i = 0; i < sheet.Rows.Count; i++)
            {
                DataRow row = sheet.Rows[i];

                if (i != 0)
                {
                    sbValues.Append(", \n");
                }

                sbValues.Append("(");

                for (int j = 0; j < sheet.Columns.Count; j++)
                {
                    var column = sheet.Columns[j];

                    if (j != 0)
                    {
                        sbValues.Append(", ");
                    }

                    var value = ExportManager.GetColumnValue(row[column], typeRow[column.Ordinal].ToString());

                    if (value.GetType() == typeof(string))
                    {
                        sbValues.AppendFormat("'{0}'", value);
                    }
                    else
                    {
                        sbValues.AppendFormat("{0}", value);
                    }
                }
                sbValues.Append(")");
            }

            sbContent.AppendFormat("INSERT DELAYED INTO `{0}` ({1}) VALUES \n{2};\n", options.Name, sbNames.ToString(), sbValues.ToString());

            sbContent.AppendLine("UNLOCK TABLES;\n");

            return sbContent.ToString();
        }

        /// <summary>
        /// 根据表头构造CREATE TABLE语句
        /// </summary>
        private string GetTabelStructSQL()
        {
            if (options.Schema == null || this.createTable == false)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DROP TABLE IF EXISTS `{0}`;\n", options.Name);
            sb.AppendFormat("CREATE TABLE `{0}` (\n", options.Name);

            foreach (DataColumn column in options.Schema.Columns)
            {
                string filedName = headRow[column].ToString();
                string filedType = typeRow[column].ToString();

                if (filedType == "varchar")
                    sb.AppendFormat("`{0}` {1}(64),", filedName, filedType);
                else if (filedType == "text")
                    sb.AppendFormat("`{0}` {1}(256),", filedName, filedType);
                else
                    sb.AppendFormat("`{0}` {1},", filedName, filedType);
            }

            sb.AppendFormat("PRIMARY KEY (`{0}`) ", headRow[0].ToString());
            sb.AppendLine("\n) DEFAULT CHARSET=utf8;");
            sb.AppendLine();
            return sb.ToString();
        }
    }
}