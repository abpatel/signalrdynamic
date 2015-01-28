using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            SignalRDynamicHostBootStrapper bootStrapper = new SignalRDynamicHostBootStrapper();
            bootStrapper.Start();
        }
    }
}
