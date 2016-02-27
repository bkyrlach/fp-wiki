using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;
using Anorm.Net;
using fp_wiki.Models;
using FunctionalProgramming.Basics;
using FunctionalProgramming.Monad;
using FunctionalProgramming.Streaming;

namespace fp_wiki.Controllers
{
    public class SearchResult
    {
        public int HelpId { get; set; }
        public string HelpBlurb { get; set; }
        public string MethodName { get; set; }
        public string TypeSignature { get; set; }
    }

    public class SearchController : ApiController
    {        
        public IEnumerable<SearchResult> Get(Search search)
        {
            var methodMapper =
                from id in Mapper.Int("Id")
                from name in Mapper.Str("Name")
                from blurb in Mapper.Str("Blurb")
                select new SearchResult
                {
                    HelpId = id,
                    MethodName = name,
                    HelpBlurb = blurb
                };

            var queryText = @"SELECT hc.Id as Id, m.Name as Name, hc.Blurb as Blurb
FROM HelpContent hc
INNER JOIN Method m on hc.MethodId = m.Id";

            var query = new Query(queryText);
            var results = Process.Resource(
                create: () => new OdbcConnection("Driver={SQL Server};Server=APNSQL-DEV;Database=fp_wiki;Trusted_Connection=Yes;"),
                initialize: conn => conn.Open(),
                release: conn => conn.Dispose(),
                use: conn => Database.ExecuteQuery(query, methodMapper, conn)).RunLog().KeepRights();

            return results;
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