﻿using SignalR.Dynamic.API.Common;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API.Interfaces
{
    public interface IHubCodeGenerator
    {
        CodeCompileUnit GenerateCodeDOM(string hubName, params string[] authorizationRoles);
    }
}
