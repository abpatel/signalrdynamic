using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API
{
    public static  class CodeCompileUnitExtensions
    {
        public static string Print(this CodeCompileUnit unit)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            using (StringWriter writer = new StringWriter())
            {
                provider.GenerateCodeFromCompileUnit(unit, writer, options);
                return writer.ToString();
            }
        }
    }
}
