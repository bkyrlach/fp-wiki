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
        public string DeclaringType { get; set; }
        public string MethodName { get; set; }
        public string MethodSignature { get; set; }

    }

    public class SearchController : ApiController
    {        
        public IEnumerable<SearchResult> Get(string search)
        {
            if (search == null || string.IsNullOrWhiteSpace(search))
            {
                return Enumerable.Empty<SearchResult>();
            }
            var methodMapper =
                from id in Mapper.Int("Id")
                from name in Mapper.Str("Name")
                from blurb in Mapper.Str("Blurb")
                from declaring in Mapper.Str("DeclaringType")
                from methodSignature in Mapper.Str("MethodSignature")
                select new SearchResult
                {
                    HelpId = id,
                    MethodName = name,
                    HelpBlurb = blurb,
                    DeclaringType = declaring,
                    MethodSignature = methodSignature
                };

            var queryText = @"
SELECT 
    hc.Id as Id, 
    m.Name as Name, 
    hc.Blurb as Blurb,
    m.DeclaringType as DeclaringType,
    m.Declaration as MethodSignature
FROM 
    HelpContent hc 
    INNER JOIN Method m 
        ON hc.MethodId = m.Id
WHERE
    m.Name LIKE {searchString}
    OR m.DeclaringType LIKE {searchString}
";

            var query = new Query(queryText).On("searchString", $"%{search}");
            var results = Process.Resource(
                create: () => new OdbcConnection("Driver={SQL Server};Server=APNSQL-DEV;Database=fp_wiki;Trusted_Connection=Yes;"),
                initialize: conn => conn.Open(),
                release: conn => conn.Dispose(),
                use: conn => Database.ExecuteQuery(query, methodMapper, conn)).RunLog().KeepRights();

            return results;
        }

    }
}