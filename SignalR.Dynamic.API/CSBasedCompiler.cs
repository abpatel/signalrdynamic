using Microsoft.CSharp;
using SignalR.Dynamic.API.Interfaces;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API
{
    public class CSBasedCompiler : ICodeDOMCompiler
    {
        public Assembly Compile(CodeCompileUnit targetUnit,string assemblyName = null)
        {
            //Ref:http://stackoverflow.com/questons/929349/is-there-a-qay-to-build-a-new-type-during-runtime
            string path = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
            var compilerParameters = new CompilerParameters
            {                 
                GenerateInMemory = true,
                IncludeDebugInformation = true,
                TreatWarningsAsErrors = true,
                WarningLevel = 4,
            };
            if(! string.IsNullOrWhiteSpace(assemblyName))
            {
                string fileName = string.Format("{0}.dll", assemblyName);
                compilerParameters.OutputAssembly = Path.Combine(path, fileName);
                compilerParameters.GenerateInMemory = false;
            }
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            compilerParameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            compilerParameters.ReferencedAssemblies.Add(Path.Combine(path,"Microsoft.AspNet.SignalR.Core.dll"));
            compilerParameters.ReferencedAssemblies.Add(Path.Combine(path,"SignalR.Dynamic.API.Common.dll"));
            compilerParameters.ReferencedAssemblies.Add(Path.Combine(path,"SignalR.Dynamic.API.Interfaces.dll"));
            compilerParameters.ReferencedAssemblies.Add(Path.Combine(path,"SignalR.Dynamic.API.dll"));

            var compilerResults = new CSharpCodeProvider()
            .CompileAssemblyFromDom(compilerParameters, targetUnit);
            if (compilerResults == null)
            {
                throw new InvalidOperationException("ClassCompiler did not return results.");
            }
            if(compilerResults.Errors.HasErrors)
            {
                var errors = string.Empty;
                foreach (CompilerError compilerError in compilerResults.Errors)
                {
                    errors += compilerError.ErrorText + "\n";
                }
                Debug.Fail(errors);
                throw new InvalidOperationException("Errors while compiling the dynamic classes:\n" + errors);
            }
            var dynamicAssembly = compilerResults.CompiledAssembly;
            return dynamicAssembly;

        }
    }
}
