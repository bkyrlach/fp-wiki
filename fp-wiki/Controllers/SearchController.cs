using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace fp_wiki.Controllers
{
    public class SearchController : ApiController
    {
        public List<string> Get(Search search)
        {
            Console.WriteLine(search);
            return new List<string>
            {
                "Foo",
                "Bar",
                "Baz",
                "FooBar"
            };
        } 
    }

    public class Search
    {
        public string Query { get; set; }

        public override string ToString()
        {
            return $"Search({Query})";
        }
    }
}