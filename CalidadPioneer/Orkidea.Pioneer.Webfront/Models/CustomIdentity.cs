﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Orkidea.Pioneer.Webfront.Models
{
    public class CustomIdentity : IIdentity
    {
        public CustomIdentity(string name, string id)
        {
            IsAuthenticated = true;
            Name = name;
            string[] info = id.Split('|');
            Id = int.Parse(info[0]);
            IsAdmin = int.Parse(info[1]);
            AuthenticationType = "Forms";
        }

        public string AuthenticationType { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public string Name { get; private set; }
        public int Id { get; private set; }
        public int IsAdmin { get; private set; }
    }
}