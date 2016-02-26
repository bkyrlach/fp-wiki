using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using fp_wiki.DataContext;

namespace fp_wiki.Controllers
{
    public class SearchController : ApiController
    {
        private readonly FpWikiDataContext _dataContext = new FpWikiDataContext();

        public IHttpActionResult Get(Search search)
        {
            return Ok(_dataContext.Set<MethodDescriptor>().Include(md => md.Parameters).Include(md => md.HelpContent).ToList());
        }

        public IHttpActionResult GetById(int id)
        {
            return Ok(_dataContext.Set<MethodDescriptor>().Where(md => md.Id == id));
        }

        private static bool IsNameSearch(string query)
        {
            return query.Split(' ').Count() == 1;
        }

        private static Regex _list = new Regex("\\[([^[])+\\]");

        private static List<List<string>> MapHaskellToCSharp(string query)
        {
            var results = new List<List<string>>();
            var queryParts = query.Split(' ');
            foreach (var queryPart in queryParts)
            {
                var innerResults = new List<string>();
                if (_list.IsMatch(queryPart))
                {                    
                    innerResults.Add("IEnumerable");
                    innerResults.Add("IQueryable");
                    innerResults.Add("IConsList");
                    innerResults.Add("List");
                    innerResults.Add("IList");
                }
                else if (queryPart == "Int")
                {
                    innerResults.Add("int");
                }
                results.Add(innerResults);
            }

            return results;
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