using System.Data;
using System.IO;

namespace DotHass.Tools.ExcelExport.Abstractions
{
    public abstract class AbstractionExport
    {
        protected ExportOptions options;

        protected string outDir { get; set; }
        protected abstract string outExtension { get; }

        protected virtual string content
        { get; set; }

        protected DataRow commentRow
        {
            get
            {
                return options.Schema.Rows[0];
            }
        }

        protected DataRow typeRow
        {
            get
            {
                return options.Schema.Rows[1];
            }
        }

        protected DataRow headRow
        {
            get
            {
                return options.Schema.Rows[2];
            }
        }

        public AbstractionExport(string OutDir, ExportOptions options)
        {
            this.outDir = OutDir;
            this.options = options;
        }

        public abstract void Parse();

        public virtual string OutputPath
        {
            get
            {
                return Path.Combine(this.outDir, options.Name + "." + this.outExtension);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void SaveToFile()
        {
            if (Directory.Exists(this.outDir) == false)
            {
                Directory.CreateDirectory(this.outDir);
            }
            //-- 保存文件
            using (FileStream file = new FileStream(OutputPath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter writer = new StreamWriter(file, options.Encode))
                    writer.Write(content);
            }
        }
    }
}