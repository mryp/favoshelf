using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace favoshelf.Data
{
    public class ImageNavigateParameter
    {
        public enum DataType
        {
            ImageFile,
            Folder,
            Archive,
        }

        public string Path { get; set; }
        public DataType Type { get; set; }

        public ImageNavigateParameter(DataType type, string path)
        {
            this.Type = type;
            this.Path = path;
        }
    }
}
