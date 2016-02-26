using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using APIClient;
using ConsoleApplication1.Model;
using Newtonsoft.Json;

namespace ConsoleApplication1
{
    class Program
    {

            private const string URL = "https://api.github.com/repos/RacoWireless/FunctionalProgramming/git/trees/3b1b516d02494c295d46012f14bd2818ad022f2b";
            private static string urlParameters = "?recursive=1";
             private static string urlParameter = "recursive=1";

        static void Main(string[] args)
        {
            var client = new RestClient("https://api.github.com");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var shaRequest =
                new RestRequest(
                    "repos/RacoWireless/FunctionalProgramming/commits/master",
                    Method.GET);
            IRestResponse shaResponse = client.Execute(shaRequest);
            var shaContent = JsonConvert.DeserializeObject<Commits>(shaResponse.Content); // raw content as string



            var treeRequest =
                new RestRequest(
                    "repos/RacoWireless/FunctionalProgramming/git/trees/{sha}",
                    Method.GET);
            treeRequest.AddUrlSegment("sha", shaContent.sha);
            treeRequest.AddParameter("recursive", "1"); // adds to POST or URL querystring based on Method
            // request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

            // easily add HTTP Headers
            //request.AddHeader("header", "value");


            // execute the request
            IRestResponse response2 = client.Execute(treeRequest);
            var content = response2.Content; // raw content as string
            FileInfo fileInfo = JsonConvert.DeserializeObject<FileInfo>(content);
            foreach (var file in fileInfo.tree)
            {
                if (file.path.EndsWith(".cs"))
                {
                    var fileRequest = new RestRequest(
                       string.Format("repos/RacoWireless/FunctionalProgramming/git/blobs/{0}", file.sha),
                    Method.GET);
                    IRestResponse fileResponse = client.Execute(fileRequest);
                    var fileContent = JsonConvert.DeserializeObject<FileContents>(fileResponse.Content);
                    var decodedContent = fileContent.content.Base64Decode();
                    Console.WriteLine(decodedContent.Split(null)[0]);
                }
               
                // raw content as string

            }

            // or automatically deserialize result
            // return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            //RestResponse<Person> response2 = client.Execute<Person>(request);
            // var name = response2.Data.Name;

            // easy async support
            /* client.ExecuteAsync(request, response => {
                 Console.WriteLine(response.Content);
             });*/
        }
    }
}
