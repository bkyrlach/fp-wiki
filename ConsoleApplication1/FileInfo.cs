using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Model
{
    class FileInfo
    {
        public string sha { get; set; }
        public string url { get; set; }
        public File[] tree { get; set; }
    }
}
