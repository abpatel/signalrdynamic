using Microsoft.AspNet.SignalR;
using SignalR.Dynamic.API.Interfaces;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API
{
    public class HubCodeGenerator : IHubCodeGenerator
    {
        private Lazy<CodeDomProvider> lazyProvider = null;

        public HubCodeGenerator()
        {
            lazyProvider = new Lazy<CodeDomProvider>(() => CodeDomProvider.CreateProvider("CSharp"),
            System.Threading.LazyThreadSafetyMode.PublicationOnly);
        }

        private CodeMemberMethod CreateGetHubAssemblyMethodBody()
        {
            
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            method.Name = "GetHubAssembly";
            method.ReturnType = new CodeTypeReference(typeof(Assembly));
            CodeSnippetExpression body = new CodeSnippetExpression("return this.GetType().Assembly");
            method.Statements.Add(body);
            return method;
        }

        private CodeMemberMethod CreateBroadcastMessageMethodBody()
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            method.Name = "BroadcastMessage";
            var param = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "message");
            param.Direction = FieldDirection.In;
            method.Parameters.Add(param);
            CodeSnippetExpression body = new CodeSnippetExpression("Clients.All.broadcastMessage(message)");
            method.Statements.Add(body);
            return method;
        }

        private CodeMemberMethod CreatInvokeMethodBody()
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            method.Name = "Invoke";
            method.PrivateImplementationType = new CodeTypeReference(typeof(IHub));
            var param = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "message");            
            param.Direction = FieldDirection.In;
            method.Parameters.Add(param);
            method.Statements.Add(new CodeSnippetExpression("SendMessage(message)"));
            return method;
        }

        private CodeMemberMethod CreateStaticSendMessageMethodBody(string hubName)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            method.Name = "SendMessage";
            var param = new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "message");
            param.Direction = FieldDirection.In;
            method.Parameters.Add(param);
#if DEBUG
            method.Statements.Add(new CodeSnippetExpression("System.Diagnostics.Debug.WriteLine(message)"));
#endif
            method.Statements.Add(new CodeSnippetExpression(string.Format("var context = GlobalHost.ConnectionManager.GetHubContext<{0}>()", hubName)));
            method.Statements.Add(new CodeSnippetExpression("context.Clients.All.broadcastMessage(message)"));
            return method;
        }

        public CodeCompileUnit GenerateCodeDOM(string hubName, params string[] authorizationRoles)
        {
            if(string.IsNullOrWhiteSpace(hubName))
            {
                throw new ArgumentException("hubName must be specified");
            }
            CodeCompileUnit targetUnit = new CodeCompileUnit();
            string nameSpaceName = "SignalR.Dynamic.API.Hubs";

            CodeNamespace nameSpace = new CodeNamespace(nameSpaceName);
            nameSpace.Imports.Add(new CodeNamespaceImport("Microsoft.AspNet.SignalR"));
            nameSpace.Imports.Add(new CodeNamespaceImport("Microsoft.AspNet.SignalR.Hubs"));            
            nameSpace.Imports.Add(new CodeNamespaceImport("SignalR.Dynamic.API.Interfaces"));
            nameSpace.Imports.Add(new CodeNamespaceImport("System.Reflection"));

            CodeTypeDeclaration classType = new CodeTypeDeclaration(hubName);
            classType.IsClass = true;
            classType.TypeAttributes = TypeAttributes.Public;
            classType.BaseTypes.Add(typeof(Hub));
            classType.BaseTypes.Add(typeof(IHub));
            var hubNameAttribute = new CodeAttributeDeclaration("HubName"
                ,new CodeAttributeArgument(new CodePrimitiveExpression(hubName)));
            classType.CustomAttributes.Add(hubNameAttribute);

            if (authorizationRoles != null && authorizationRoles.Length > 0)
            {
                string roles = string.Join(",", authorizationRoles);
                if (!string.IsNullOrEmpty(roles))
                {
                    var authAttribute = new CodeAttributeDeclaration("Authorize",
                        new CodeAttributeArgument(
                            "Roles",
                            new CodePrimitiveExpression(roles)
                            ));
                    classType.CustomAttributes.Add(authAttribute);
                }
            }
            classType.Members.Add(CreateBroadcastMessageMethodBody());
            classType.Members.Add(CreatInvokeMethodBody());
            classType.Members.Add(CreateStaticSendMessageMethodBody(hubName));
            classType.Members.Add(CreateGetHubAssemblyMethodBody());
            nameSpace.Types.Add(classType);
            targetUnit.Namespaces.Add(nameSpace);
            return targetUnit;
        }
    }
}
