using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security;
using System.Xml;
using APIClient;
using ConsoleApplication1.Model;
using Newtonsoft.Json;
using FunctionalProgramming.Basics;
using FunctionalProgramming.Monad;
using FunctionalProgramming.Monad.Parsing;
using FunctionalProgramming.Monad.Transformer;

using StringParser = FunctionalProgramming.Monad.Transformer.StateEither<FunctionalProgramming.Monad.Parsing.ParserState<char>, string, string>;
using CharParser = FunctionalProgramming.Monad.Transformer.StateEither<FunctionalProgramming.Monad.Parsing.ParserState<char>, string, char>;
using UnitParser = FunctionalProgramming.Monad.Transformer.StateEither<FunctionalProgramming.Monad.Parsing.ParserState<char>, string, FunctionalProgramming.Basics.Unit>;
using MethodParser = FunctionalProgramming.Monad.Transformer.StateEither<FunctionalProgramming.Monad.Parsing.ParserState<char>, string, ConsoleApplication1.MethodDescriptor>;

namespace ConsoleApplication1
{
    class Program
    {

            private const string URL = "https://api.github.com/repos/RacoWireless/FunctionalProgramming/git/trees/3b1b516d02494c295d46012f14bd2818ad022f2b";
            private static string urlParameters = "?recursive=1";
             private static string urlParameter = "recursive=1";

        public static CharParser Comma = Parser.Elem(',');
        public static CharParser Space = Parser.Elem(' ');
        public static CharParser RightParen = Parser.Elem(')');
        public static CharParser LeftParen = Parser.Elem('(');
        public static UnitParser Any = Parser.ElemWhere<char>(c => true, ".").Select(c => Unit.Only);
        public static StringParser NotNewline = Parser.ElemWhere<char>(c => c != '\n', "[\n]").Many1().Select(chars => chars.MkString());
        public static CharParser Newline = Parser.Elem('\n');

        public static StringParser NotSpace = Parser.ElemWhere<char>(c => c != ' ', "[^ ]").Many1().Select(chars => chars.MkString());
        public static StringParser NotLeftParen = Parser.ElemWhere<char>(c => c != '(', "[^(]").Many1().Select(chars => chars.MkString());
        public static StringParser NotSpaceOrParen = Parser.ElemWhere<char>(c => c != ' ' && c != ')', "[^ )]").Many1().Select(chars => chars.MkString());
        public static StringParser NotCommaOrParen = Parser.ElemWhere<char>(c => c != ',' && c != ')', "[^,)]").Many1().Select(chars => chars.MkString());

        public static StringParser Public = ParseWord("public");
        public static StringParser Private = ParseWord("private");
        public static StringParser Protected = ParseWord("protected");
        public static StringParser Static = ParseWord("static");
        public static StringParser AccessModifier = Public.Or(Private).Or(Protected).CombineTakeLeft(Space);

        public static UnitParser ParamSeparator = from _1 in Comma
                                                  from _2 in Space.MakeOptional()
                                                  select Unit.Only;

        /*public static MethodParser Method =
            from modifier in AccessModifier
            from isStatic in Static.MakeOptional().Select(m => !m.IsEmpty)
            from _1 in Space
            from @return in NotSpace
            from _2 in Space
            from methodName in NotLeftParen
            from _3 in LeftParen
            from parameters in Params()
            select new MethodDescriptor
            {
                MethodName = methodName,
                Parameters = parameters.Select(pair => new ParameterDescriptor
                {
                    Type = pair.Item1,
                    Name = pair.Item2
                }),
                AccessModifier = modifier,
                ReturnType = @return
            };*/

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

            /*            string sample = @"
            public static void Main(string[] args, string blah)
            {
                Console.writeLine(""Hello World"");
            }

            //check out my dope comment
            /* 
                this is an example
                of a multiline comment yo
            #1#
            public static int Foo()
            {
                var dude = 3;
                return dude * dude;
            }";*/
            
          //  Console.WriteLine(Parser.Parse(ParseMethods(), sample));
        }

          
            // or automatically deserialize result
            // return content type is sniffed but can be explicitly set via RestClient.AddHandler();
            //RestResponse<Person> response2 = client.Execute<Person>(request);
            // var name = response2.Data.Name;
        static StringParser ParseWord(string word)
        {
            return word.Traverse(Parser.Elem).Select(chars => chars.MkString());
        }

        static StateEither<ParserState<char>, string, IEnumerable<Tuple<string, string>>>  Params()
        {
            return
                from type in NotSpaceOrParen
                from _1 in Space
                from name in NotCommaOrParen.Select(chars => chars.MkString())
                from rest in RightParen.Select(c => Enumerable.Empty<Tuple<string, string>>()).Or(ParamSeparator.CombineTakeRight(Params()))
                select Tuple.Create(type, name).LiftEnumerable().Concat(rest);
        }

    /*    static StateEither<ParserState<char>, string, IEnumerable<MethodDescriptor>> ParseMethods()
        {
            //return
            //    from head in Parser.EoF<char>().Select(u => Enumerable.Empty<MethodDescriptor>()).Or(Method.Select(m => m.LiftEnumerable()).Or(NotNewline.CombineTakeLeft(Newline).Select(u => Enumerable.Empty<MethodDescriptor>())))
            //    from rest in ParseMethods()
            //    select head.Concat(rest);
        }*/
    }

    public class MethodDescriptor
    {
        public string MethodName { get; set; }
        public IEnumerable<ParameterDescriptor> Parameters { get; set; }
        public string AccessModifier { get; set; }
        public string ReturnType { get; set; }
        public string Comments { get; set; }
    }

    public class ParameterDescriptor
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }

    public static class Helpers
    {
        public static StateEither<ParserState<TInput>, string, Unit> Rest<TInput>()
        {
            return
                from _1 in ParserState<TInput>.MoveNext()
                from isEOF in ParserState<TInput>.IsEoF()
                from next in isEOF ? Parser.Pure<TInput, Unit>(Unit.Only) : Rest<TInput>()
                select Unit.Only;

            // easy async support
            /* client.ExecuteAsync(request, response => {
                 Console.WriteLine(response.Content);
             });*/
        }
    }
}
