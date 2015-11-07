using System;
using System.Reflection;
using System.IO;
using DemoInfo;
using System.Collections.Generic;
using System.Linq;


namespace steam_api_test
{
    class Program
    {
        static String getAPIKey()
        {
            StreamReader parse = File.OpenText("apikey.xml");
            return parse.ReadLine();
        }

        static void Main(string[] args)
        {
            System.Console.WriteLine(getAPIKey());
            System.Console.ReadKey();
        }
    }
}
