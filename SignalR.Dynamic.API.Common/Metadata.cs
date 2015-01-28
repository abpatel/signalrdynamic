﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API.Common
{
    public class Metadata
    {
        public string Topic { get; set; }
        public string[] AuthorizationRoles { get; set; }
    }
}
