using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Anorm.Net;
using FunctionalProgramming.Basics;
using FunctionalProgramming.Monad;
using FunctionalProgramming.Streaming;
using Process = FunctionalProgramming.Streaming.Process;

namespace ConsoleApplication1
{
    class Program
    {
        private static readonly Query GetLastMethodId = new Query("SELECT MAX(Id) as LastId FROM Method");
        private static readonly Query InsertMethodQuery = new Query("INSERT INTO Method (Id, Name, ReturnType, DeclaringType, TypeSignature, Declaration) VALUES ({id}, {name}, {returnType}, {declaringType}, {typeSignature}, {declaration})");
        private static readonly Query InsertParameterQuery = new Query("INSERT INTO Parameter (Name, Type, MethodId) VALUES ({name}, {type}, {methodId})");
        private static readonly Query InsertHelpContentQuery = new Query("INSERT INTO HelpContent (MethodId, Blurb, HelpContent) VALUES ({methodId}, {blurb}, {content})");

        static string ProcessType(Type t)
        {
            var name = t.Name;
            if (t.GenericTypeArguments.Length > 0)
            {
                var genericTypeArgs = t.GenericTypeArguments.Select(ProcessType).ToList();
                var genericString = string.Join(",", genericTypeArgs);
                name = $"{name.Substring(0, name.IndexOf('`'))}<{genericString}>";                
            }
            return name;
        }

        static Tuple<string, bool> ProcessTypeAsAlgebra(Type t)
        {
            var isMultiTerm = false;
            string retval;
            var name = t.Name;
            var generics = t.GetGenericArguments();
            if (generics.Length > 0)
            {
                isMultiTerm = true;
                name = name.Substring(0, name.IndexOf('`'));
                var genericTypeArgs = generics.Select(ProcessTypeAsAlgebra).ToList();
                if (name == "Func")
                {
                    string inputString;
                    var inputs = genericTypeArgs.Take(genericTypeArgs.Count - 1).Select(pair => pair.Item2 ? $"({pair.Item1})" : pair.Item1).ToList();
                    if (inputs.Count == 0)
                    {
                        inputString = "()";
                    }
                    else if (inputs.Count == 1)
                    {
                        inputString = inputs[0];
                    }
                    else
                    {
                        inputString = string.Join(" -> ", inputs);
                    }
                    var output = genericTypeArgs.Last();
                    retval = $"({inputString} -> {output.Item1})";
                }
                else
                {
                    var parametricTypes = string.Join(" ", genericTypeArgs.Select(pair => pair.Item2 ? $"({pair.Item1})" : pair.Item1));
                    retval = $"{name} {parametricTypes}";
                }
            }
            else
            {
                if (t.IsGenericParameter)
                {
                    retval = name.ToLower();
                }
                else
                {
                    retval = name;
                }                
            }
            return Tuple.Create(retval, isMultiTerm);
        }

        static Process<IEither<Failure, int>> InsertParameter(DbConnection conn, ParameterInfo p, int methodId)
        {            
            return Database.ExecuteStatement(conn,
                InsertParameterQuery.On("methodId", methodId)
                    .On("name", p.Name)
                    .On("type", ProcessType(p.ParameterType)));
        }

        static string GetDeclaration(MethodInfo m)
        {
            var returnType = ProcessType(m.ReturnType);
            var declaringType = ProcessType(m.DeclaringType);
            var methodName = m.Name;
            var parameters = m.GetParameters().Select(p => Tuple.Create(p.Name, ProcessType(p.ParameterType))).ToList();
            var parameterString = string.Join(", ", parameters.Select(pair => $"{pair.Item2} {pair.Item1}"));
            return $"{returnType} {declaringType}.{methodName}({parameterString})";
        }

        static string GetTypeSignature(MethodInfo m)
        {
            var returnType = ProcessTypeAsAlgebra(m.ReturnType).Item1;
            var inputs = m.GetParameters().Select(p => ProcessTypeAsAlgebra(p.ParameterType)).ToList();
            string inputString;
            if (inputs.Count == 0)
            {
                inputString = "()";
            }
            else if (inputs.Count == 1)
            {
                inputString = inputs[0].Item1;
            }
            else
            {
                inputString = $"({string.Join(", ", inputs.Select(pair => pair.Item1))})";
            }
            return $"{inputString} -> {returnType}";
        }

        static Process<IEither<Failure, int>> InsertMethod(DbConnection conn, MethodInfo m, int id)
        {
            bool badActor = m.Name.Contains("Where") && m.DeclaringType == typeof (Maybe);
            return Database.ExecuteStatement(conn, InsertMethodQuery.On("id", id).On("name", m.Name).On("returnType", ProcessType(m.ReturnType)).On("declaringType", ProcessType(m.DeclaringType)).On("typeSignature", GetTypeSignature(m)).On("declaration", GetDeclaration(m)))
                .SelectMany(either => either.Match(
                    left: failure => new Halt<IEither<Failure, int>>(new Exception(failure.ToString())), 
                    right: n => Process.Apply(m.GetParameters()).SelectMany(p => InsertParameter(conn, p, id))))
                .Concat(() =>
                {
                    if(badActor)
                        Console.WriteLine("Ruh-roh");
                    return InsertDefaultHelpContent(conn, m.Name, id);
                });
        }

        private static Process<IEither<Failure, int>> InsertDefaultHelpContent(DbConnection conn, string name, int id)
        {
            return Database.ExecuteStatement(conn,
                InsertHelpContentQuery.On("methodId", id)
                    .On("blurb", $"Please add a short blurb describing '{name}'")
                    .On("content", $"No help content exists yet for '{name}'"));
        }

        static Process<IEither<Failure, int>> ProcessMethod(DbConnection conn, MethodInfo m)
        {
            return
                Database.ExecuteQuery(GetLastMethodId, Mapper.Int("LastId"), conn)
                    .SelectMany(either => either.Match(
                        left: failure => InsertMethod(conn, m, 1),
                        right: id => InsertMethod(conn, m, id + 1)));
        }

        static Process<IEither<Failure, int>> ProcessMethodsForType(DbConnection conn, Type t)
        {
            return Process.Apply(t.GetMethods().Where(MethodFilter).ToArray()).SelectMany(m => ProcessMethod(conn, m));
        }        

        static bool MethodFilter(MethodInfo m)
        {
            return
                m.IsPublic &&
                !m.Name.Contains("get_") &&
                !m.Name.Contains("set_") &&
                m.Name != "ToString" &&
                m.Name != "GetHashCode" &&
                m.Name != "Equals" &&
                m.Name != "GetType" &&
                m.Name != "GetBaseException" &&
                m.Name != "GetBaseObjectData" &&
                m.Name != "GetObjectData";
        }

        static void Main(string[] args)
        {
            //typeof (Maybe).Assembly.ExportedTypes.SelectMany(t => t.GetMethods()).Where(MethodFilter).Select(m => $"{m.Name} :: {GetTypeSignature(m)}").Take(50).ToList().ForEach(Console.WriteLine);

            Process.Resource(
                create: () => new OdbcConnection("Driver={SQL Server};Server=APNSQL-DEV;Database=fp_wiki;Trusted_Connection=Yes;"),
                initialize: conn => conn.Open(),
                release: conn => conn.Dispose(),
                use: conn => Process.Apply(typeof(Maybe).Assembly.ExportedTypes.ToArray()).SelectMany(t => ProcessMethodsForType(conn, t))).Run();
        }
    }

}
