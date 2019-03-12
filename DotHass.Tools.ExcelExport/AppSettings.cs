using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotHass.Tools.ExcelExport
{
    public enum JsonMode
    {
        Dictonary,
        Array,
        Detailed
    }

    public class AppSettings
    {
        public bool ShowInfo { get; set; } = true;

        public string Root { get; set; }

        /// <summary>
        /// 排除的文件
        /// </summary>
        public List<string> Excludes { get; set; } = new List<string>();

        public string ExcelDir { get; set; }

        /// <summary>
        ///  输入的Excel文件路径.
        /// </summary>
        public string ExcelPath
        {
            get
            {
                return GetPath(ExcelDir);
            }
        }

        public string JsonDir { get; set; }

        /// <summary>
        /// 指定输出的json文件路径.
        /// </summary>
        public string JsonPath
        {
            get
            {
                return GetPath(JsonDir);
            }
        }

        public string SQLDir { get; set; }

        /// <summary>
        /// 指定输出的SQL文件路径.
        /// </summary>
        public string SQLPath
        {
            get
            {
                return GetPath(SQLDir);
            }
        }

        public string CSharpDir { get; set; } = "";

        /// <summary>
        /// 指定输出的C#数据定义代码文件路径.
        /// </summary>
        public string CSharpPath
        {
            get
            {
                return GetPath(CSharpDir);
            }
        }

        /// <summary>
        /// 表格中有几行是表头.
        /// </summary>
        public int HeaderRows { get; set; } = 3;

        /// <summary>
        ///  保存到文件的编码的名称
        /// </summary>
        public string EncodeName { get; set; } = "utf8-nobom";

        private Encoding _encode { get; set; }

        public Encoding Encode
        {
            get
            {
                if (_encode == null)
                {
                    Encoding cd = Encoding.UTF8;
                    if (EncodeName != "utf8-nobom")
                    {
                        foreach (EncodingInfo ei in Encoding.GetEncodings())
                        {
                            Encoding e = ei.GetEncoding();
                            if (e.HeaderName == EncodeName)
                            {
                                cd = e;
                                break;
                            }
                        }
                    }
                    this._encode = cd;
                }
                return _encode;
            }
        }

        #region csharp

        public string CSharpTemplate { get; set; }

        public string CSharpTemplatePath
        {
            get
            {
                return GetPath(CSharpTemplate);
            }
        }

        #endregion csharp

        #region json

        /// <summary>
        /// 自动把字段名称转换成小写格式.
        /// </summary>
        public bool Lowcase { get; set; } = false;

        public JsonMode JsonMode { get; set; } = JsonMode.Detailed;

        #endregion json

        public string GetPath(string dir)
        {
            if (string.IsNullOrEmpty(dir))
            {
                return "";
            }

            //如果是个根目录或者文件
            if (Path.IsPathRooted(dir))
            {
                return dir;
            }

            var root = Path.IsPathRooted(Root) ? Root : Path.Combine(Path.GetFullPath(AppContext.BaseDirectory), Root);

            return Path.Combine(root, dir);
        }

        /// <summary>
        /// 是否过滤空行
        /// </summary>
        public bool FilterRow { get; set; } = true;

        /// <summary>
        /// 是否过滤空列
        /// </summary>
        public bool FilterCol { get; set; } = true;

        /// <summary>
        /// 是否过滤空列
        /// </summary>
        public bool SQLCreateTable { get; set; } = false;
    }
}