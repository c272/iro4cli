using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iro4cli
{
    class Program
    {
        static void Main(string[] args)
        {
            //Load the first arg as a file for testing purposes.
            string text = File.ReadAllText(args[0]);
            Emulator.Run(text);
        }
    }
}
