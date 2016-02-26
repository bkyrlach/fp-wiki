using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {

        static void Main(string[] args)
        {
            string sample = @"
public static void Main(string[] args, string blah)
{
    Console.writeLine(""Hello World"");
}

//check out my dope comment
/* 
    this is an example
    of a multiline comment yo
*/
public static int Foo()
{
    var dude = 3;
    return dude * dude;
}";
            
            
        }

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
}
