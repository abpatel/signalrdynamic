using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API.Interfaces
{
   
    public interface ICodeDOMCompiler
    {
        Assembly Compile(CodeCompileUnit targetUnit);
    }
}
