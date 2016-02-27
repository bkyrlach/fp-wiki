using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIClient
{
    class FileContents
    {
       public string sha { get; set; } 
       public string size { get; set; }
       public string url { get; set; }
       public string content { get; set; }
       public string encoding { get; set; }
    }
}
