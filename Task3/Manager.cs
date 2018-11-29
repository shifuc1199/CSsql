using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using LitJson;
namespace Task3
{
    static class Manager
    {
        public static string DatabaseName="";
        public static bool isLogin = false;
        public static JsonData UserData = null;
        public static Dictionary<string,B_Tree> Indexs = new Dictionary<string,B_Tree>();
     
    }
}
