using System.Data;
using System.Text;

namespace DotHass.Tools.ExcelExport.Abstractions
{
    public class ExportOptions
    {
        public string Name { get; set; }

        public Encoding Encode { get; set; } = Encoding.UTF8;

        public DataTable Data { get; set; }
        public DataTable Schema { get; set; }
    }
}