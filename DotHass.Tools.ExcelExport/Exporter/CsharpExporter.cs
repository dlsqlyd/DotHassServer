using DotHass.Tools.ExcelExport.Abstractions;
using System;
using System.Data;
using System.IO;
using System.Text;

namespace DotHass.Tools.ExcelExport.Exporter
{
    public class CsharpExporter : AbstractionExport
    {
        private string templateFile;

        private FieldSchema IDField = null;

        protected override string outExtension { get { return "cs"; } }

        public CsharpExporter(string templatefile, string outdir, ExportOptions options) : base(outdir, options)
        {
            this.templateFile = templatefile;
        }

        public override void Parse()
        {
            if (string.IsNullOrEmpty(this.templateFile))
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine();
                sb.AppendLine($"// Generate From {options.Name}.xlsx");
                sb.AppendLine();

                sb.AppendLine($"public class {options.Name}\r\n{{");
                sb.AppendLine();

                sb.Append(this.GenerateFileds());

                sb.Append('}');
                sb.AppendLine();
                sb.AppendLine("// End of Auto Generated Code");
                content = sb.ToString();
            }
            else
            {
                using (var streamReader = new StreamReader(this.templateFile, Encoding.UTF8))
                {
                    content = streamReader.ReadToEnd();
                    content = content.Replace("{CLASSNAME}", options.Name);
                    content = content.Replace("{FILEDS}", this.GenerateFileds(2).ToString());
                    content = content.Replace("{IDField_NAME}", IDField.name + ".ToString()");
                    content = content.Replace("{IDField_TYPE}", IDField.type);
                }
            }
        }

        public StringBuilder GenerateFileds(int tabnum = 1)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataColumn column in options.Schema.Columns)
            {
                FieldSchema field = new FieldSchema()
                {
                    name = headRow[column].ToString(),
                    type = typeRow[column].ToString(),
                    comment = commentRow[column].ToString()
                };

                if (IDField == null)
                {
                    IDField = field;
                }
                var tab = new String('\t', tabnum);
                sb.AppendLine($"{tab}/// <summary>");
                sb.AppendLine($"{tab}/// {field.comment}");
                sb.AppendLine($"{tab}/// </summary>");
                sb.AppendLine($"{tab}public {field.type} {field.name} {{ get; set; }}");
                sb.AppendLine();
            }
            return sb;
        }
    }
}