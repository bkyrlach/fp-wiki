using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using fp_wiki.DataContext;

namespace fp_wiki.Controllers
{
    public class SearchController : ApiController
    {
        private FpWikiDataContext _dataContext = new FpWikiDataContext();

        public IHttpActionResult Get(Search search)
        {
            return Ok(_dataContext.Set<MethodDescriptor>().ToList());
        }

        public IHttpActionResult GetById(int id)
        {
            return Ok(_dataContext.Set<MethodDescriptor>().Where(md => md.Id == id));
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