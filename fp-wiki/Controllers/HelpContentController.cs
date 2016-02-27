using System;
using System.Data.Odbc;
using System.Web.Http;
using Anorm.Net;
using fp_wiki.Models;
using FunctionalProgramming.Basics;
using FunctionalProgramming.Streaming;

namespace fp_wiki.Controllers
{
    public class HelpContentController : ApiController
    {
        public class HelpResult
        {
            public int HelpId { get; set; }
            public string MethodName { get; set; }
            public string Blurb { get; set; }
            public string Content { get; set; }
        }

        public HelpResult Get(int id)
        {
            var mapper =
                from resultId in Mapper.Int("Id")
                from methodName in Mapper.Str("Name")
                from blurb in Mapper.Str("Blurb")
                from content in Mapper.Str("HelpContent")
                select new HelpResult
                {
                    HelpId = resultId,
                    MethodName = methodName,
                    Blurb = blurb,
                    Content = content
                };
            var query = new Query("SELECT hc.Id as Id, m.Name as Name, hc.Blurb as Blurb, hc.HelpContent as HelpContent FROM HelpContent hc INNER JOIN Method m ON m.Id = hc.MethodId WHERE hc.Id = {id}").On("id", new IntValue(id));
            return Process.Resource(
                create: () => new OdbcConnection("Driver={SQL Server};Server=APNSQL-DEV;Database=fp_wiki;Trusted_Connection=Yes;"),
                initialize: conn => conn.Open(),
                release: conn => conn.Dispose(),
                use: conn => Database.ExecuteQuery(query, mapper, conn)).Run().Match(
                    left: failure => new HelpResult { Content = failure.ToString() },
                    right: BasicFunctions.Identity);
        }

        public void Post(HelpContentHolder updatedContent)
        {
            var conn = new OdbcConnection("Driver={SQL Server};Server=APNSQL-DEV;Database=fp_wiki;Trusted_Connection=Yes;");
            var statement = new Query("UPDATE HelpContent SET Blurb = {blurb}, HelpContent = {content} WHERE Id = {id}")
                .On("id", new IntValue(updatedContent.Id))
                .On("blurb", new StringValue(updatedContent.Blurb))
                .On("content", new StringValue(updatedContent.HelpContent));
            var result = Database.ExecuteStatement(conn, statement).UnsafePerformIo();
            Console.WriteLine(result);
        }
    }
}