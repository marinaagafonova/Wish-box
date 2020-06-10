using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wish_Box.Contracts
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        //public const string Version = "v1";
        //public const string Base = {Root}";
        public static class Following
        {
            public const string Get = Root +"/followings";
            public const string Post = Root + "/followings/add/{id}";
            //public static readonly string Delete = "";
        }
    }
}
