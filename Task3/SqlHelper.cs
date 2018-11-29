using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using LitJson;
using System.Diagnostics;

namespace Task3
{
    class SqlHelper
    {
       
       public Queue<string> Sqls = new Queue<string>();
        List<string> tablename = new List<string>();
        List<string> paramsname = new List<string>();
        List<string> type = new List<string>();
        List<string> values = new List<string>();
        List<string> givenparamsname = new List<string>();
        List<string> givenvalues = new List<string>();
        List<string> operation = new List<string>();
        public void Init()
        {
            operation.Clear();
            Sqls.Clear();
            paramsname.Clear();
            type.Clear();
            tablename.Clear();
            values.Clear();
            givenparamsname.Clear();
            givenvalues.Clear();
        }
        public SqlHelper()
        {
            if (!Manager.isLogin)
            {
                Login();
                Manager.isLogin = true;
            }
            else
            Console.Write("CSSql-->");

        }
        public void Login()
        {
            if (!File.Exists("Manager.json"))
            {
                
                Console.WriteLine("第一次登陆请设置数据库用户名密码:");
                Console.Write("用户名:");
                string username = Console.ReadLine();
                Console.Write("密码:");
                string password = Console.ReadLine();
                JsonData jd = new JsonData();
                File.Create("Manager.json").Close();
                JsonData jd2 = new JsonData();
               jd2["UserName"] = username;
                jd2["Password"] = password;
                jd2["select"] = true;
                jd2["update"] = true;
                jd2["delete"] = true;
                jd2["insert"] = true;
                jd2["create"] = true;
                jd2["drop"] = true;
                jd2["alter"] = true;
                jd.Add(jd2);
                Manager.UserData = jd2;
                FileHelper.SetFileContenct("Manager.json", FileType.Default, jd.ToJson());
                Console.WriteLine("设置成功！正在启动数据库......");
                Console.WriteLine("数据库启动成功!!");
                Console.WriteLine();
                Console.WriteLine();
                Console.Write("CSSql-->");
            }
            else
            {
                Console.Write("请输入用户名:");
                string username = Console.ReadLine();

                string password;
                 
                JsonData jd = JsonMapper.ToObject(FileHelper.GetFileContenct("Manager.json", FileType.Default));

                for (int i = 0; i < jd.Count; i++)
                {
                    if (username == jd[i]["UserName"].ToString())
                    {
                        Console.Write("请输入密码:");
                        password = Console.ReadLine();
                        if (password != jd[i]["Password"].ToString())
                        {
                            Console.WriteLine("密码输入错误！");
                            Login();
                            return;
                        }
                        Manager.UserData = jd[i];
                        Console.WriteLine("数据库启动成功!!");
                        Console.WriteLine();
                        Console.WriteLine("你的权限: "+"Create:"+ (bool)Manager.UserData["create"]+" Insert:"+ (bool)Manager.UserData["insert"]+" Drop:"+ (bool)Manager.UserData["drop"]+" Update:"+ (bool)Manager.UserData["update"]+" Select:"+ (bool)Manager.UserData["select"]+" Delete:"+ (bool)Manager.UserData["delete"]+" Alter:"+ (bool)Manager.UserData["alter"]);
                        Console.WriteLine();
                        Console.Write("CSSql-->");
                        return;
                    }        
               }
                Console.WriteLine("用户名错误!");
                Login();
            }
        }
        public bool  SqlParser(string sql)
        {
            if (!sql.Contains("(") && sql.Contains(")"))
            {
                return false;
            }
            if (sql.Contains("(") && !sql.Contains(")"))
            {
                return false;
            }
            for (int i = 0; i < Tool.GetLegalString(sql).Split(' ').Length; i++)
            {
       // Console.WriteLine(Tool.GetLegalString(sql).Split(' ')[i]);
  Sqls.Enqueue(Tool.GetLegalString(sql).Split(' ')[i]);
   
            }
             
            return Commond();
        }
        bool ChangeUser()
        {
            if (!QueueEquals("user"))
                return false;
            if (!QueueEquals("username"))
                return false;
            if (!QueueEquals("="))
                return false;
            if (Sqls.Count == 0)
                return false;
            string username = Name();
            if (!QueueEquals("password"))
                return false;
            if (!QueueEquals("="))
                return false;
            if (Sqls.Count == 0)
                return false;
            string password = Name();

            JsonData jd = JsonMapper.ToObject(FileHelper.GetFileContenct("Manager.json", FileType.Default));
            for (int i = 0; i < jd.Count; i++)
            {
                if(jd[i]["UserName"].ToString()==username)
                {
                    if (jd[i]["Password"].ToString() == password)
                    {
                        Manager.UserData = jd[i];
                        Manager.DatabaseName = "";
                        Console.WriteLine("切换成功!");
                        Console.WriteLine("你的权限: " + "Create:" + (bool)Manager.UserData["create"] + " Insert:" + (bool)Manager.UserData["insert"] + " Drop:" + (bool)Manager.UserData["drop"] + " Update:" + (bool)Manager.UserData["update"] + " Select:" + (bool)Manager.UserData["select"] + " Delete:" + (bool)Manager.UserData["delete"] + " Alter:" + (bool)Manager.UserData["alter"]);

                        return true;
                    }
                }
            }
            return false;
        }
        bool Commond()
        {
        
            if (Sqls.Count == 0)
                return false;
            string s = Sqls.Dequeue();
            switch (s)
            {
                case "exit":
                    Environment.Exit(0);
                    break;
                case "change":
                     
                        return ChangeUser();
                     
                    //Manager.isLogin = false;
                    //SqlHelper sq = new SqlHelper();
                    //string sql = Console.ReadLine();
                    //string[] sqls = sql.Split(';');
                    //for (int i = 0; i < sqls.Length; i++)
                    //{
                    //    sq.Init();
                    //    Console.WriteLine(sq.SqlParser(sqls[i]));
                    //}
                    //return true;

                case "create":
                if ((bool)Manager.UserData["create"])
                return (Create());
                else
                        Console.WriteLine("你没有Create权限！");
                break;
                case "use":
                    return (Use());
                case "insert":
                    if ((bool)Manager.UserData["insert"])
                        return (Insert());
                    Console.WriteLine("你没有Insert权限！");
                    break;
                case "drop":
                    if ((bool)Manager.UserData["drop"])
                        return (Drop());
                    else
                        Console.WriteLine("你没有Drop权限！");
                    break;
                case "update":
                    if ((bool)Manager.UserData["update"])
                        return (Update());
                    Console.WriteLine("你没有Update权限！");
                    break;
                case "select":
                 if ((bool)Manager.UserData["select"])
                    return (Select());
                 else
                    Console.WriteLine("你没有Select权限！");
                    break;
                case "delete":
                    if ((bool)Manager.UserData["delete"])
                        return (Delete());
                    else
                        Console.WriteLine("你没有Delete权限！");
                    break;
                case "alter":
                    if ((bool)Manager.UserData["alter"])
                        return (AlterTable());
                    else
                        Console.WriteLine("你没有Alter权限！");
                    break;
                case "show":
                    return (Show());
            }
            return false;
        }
        bool Show()
        {
            if (Sqls.Count == 0)
                return false;
            string temp =Name();
            if (Sqls.Count != 0)
                return false;
            if (temp == "table")
            {
                return (ShowTable());
            }
            else if(temp =="database")
            {
                return (ShowDatabase());
            }
            return false;
        }
        bool ShowTable()
        {
            if (Manager.DatabaseName == "")
                return false;
            Console.WriteLine();
            if(FileHelper.isExists(Manager.DatabaseName,FileType.DataBase))
            {
                JsonData jd = JsonMapper.ToObject(FileHelper.GetFileContenct(Manager.DatabaseName, FileType.DataBase));
                if(!jd.IsArray)
                {
                    return false;
                }
                for (int i = 0; i < jd.Count; i++)
                {
                    Console.WriteLine(jd[i]);
                }
                return true;
            }
            return false;
        }
        bool ShowDatabase()
        {
           
            if (!Manager.UserData.Keys.Contains("DataBase"))
                return false;

            for (int i = 0; i < Manager.UserData["DataBase"].Count; i++)
            {
                Console.WriteLine(Manager.UserData["DataBase"][i].ToString());
            }

            return true;
        }
        bool Update()
        {
            if (Sqls.Count == 0)
                return false;
            if (Sqls.Peek() == "user")
                return SetUser();
           if(Sqls.Contains("where"))
            {
                return UpdateGivenValue(Name());
            }else
            {
                return UpdateAllValue(Name());
            }
        }
        bool Insert()
        {
 
            if (Sqls.ToList().FindAll((t) => { return t == "("; }).Count >= 2)
            {
                return InsertGivenValue();
            }
            else
                return InsertValue();
        }
        bool Drop()
        {
            if (Sqls.Count == 0)
                return false;
            string s = Sqls.Dequeue();
            switch(s)
            {
                case "database":
                    return DropDatabase(Name());
                case "table":
                    return DropTable(Name());
                case "index":
                    return DropIndex(Name());
                case "user":
                    return DropUser();
            }
            return false;
        }
        bool DropUser()
        {
            if (!FileHelper.isExists("Manager.json", FileType.Default))
                return false;
            if (!QueueEquals("username"))
                return false;
            if (!QueueEquals("="))
                return false;
            if (Sqls.Count == 0)
                return false;

            string username = Sqls.Dequeue();
            if (username == Manager.UserData["UserName"].ToString())
                return false;
            
            JsonData jd = JsonMapper.ToObject(FileHelper.GetFileContenct("Manager.json", FileType.Default));
            for (int i = 0; i < jd.Count; i++)
            {
                if(jd[i]["UserName"].ToString()==username)
                {
                    for (int j= 0; j < jd[i]["DataBase"].Count; j++)
                    {
                        DropDatabase(jd[i]["DataBase"][j].ToString());
                    }
                    jd.Remove(jd[i]);
                }
            }
            jd = jd;
            FileHelper.SetFileContenct("Manager.json", FileType.Default, jd.ToJson(), FileMode.Truncate);
            return true;
        }
        bool DropDatabase(string name)
        {
            JsonData jd=JsonMapper.ToObject(FileHelper.GetFileContenct(name, FileType.DataBase));
            for (int i = 0; i < jd.Count; i++)
            {
                JsonData temp = JsonMapper.ToObject(FileHelper.GetFileContenct(jd[i].ToString(),FileType.Table));
                if (temp.Keys.Contains("Params"))
                {
                    for (int j = 0; j < temp["Params"].Count; j++)
                    {
                        if (temp["Params"][j].Keys.Contains("Index"))
                        {
                            FileHelper.Delete(temp["Params"][j]["Index"].ToString() + ".index", FileType.Default);
                        }
                    }
                }
                FileHelper.Delete(jd[i].ToString(), FileType.Table);
            }
            return FileHelper.Delete(name, FileType.DataBase);
        }
        bool DropTable(string name)
        {
            if (!FileHelper.isExists(name, FileType.Table))
            {
                return false;
            }
                JsonData jd = JsonMapper.ToObject(FileHelper.GetFileContenct(Manager.DatabaseName, FileType.DataBase));
             
                JsonData jd2 = JsonMapper.ToObject(FileHelper.GetFileContenct(name, FileType.Table));
            if (jd2.Keys.Contains("Params"))
            {
                for (int i = 0; i < jd2["Params"].Count; i++)
                {
                    if (jd2["Params"][i].Keys.Contains("Index"))
                    {
                        FileHelper.Delete(jd2["Params"][i]["Index"].ToString() + ".index", FileType.Default);
                    }
                }
            }
            
            jd.Remove(name);
            FileHelper.SetFileContenct(Manager.DatabaseName, FileType.DataBase, jd.ToJson(), FileMode.Truncate);
            return FileHelper.Delete(name, FileType.Table);
        }
        bool Use()
        {
            if (Sqls.Count == 0)
                return false;
            string name = Sqls.Dequeue();
            if (FileHelper.isExists(name, FileType.DataBase))
            {
                Manager.DatabaseName = name;
                return true;
            }
            return false;
        }
        string Name()
        {
            if (Sqls.Count == 0)
                return null;
            string name = Sqls.Dequeue();
            return name;
        }
         bool Create()
        {
            if (Sqls.Count == 0)
                return false;
            string s = Sqls.Dequeue();
            switch(s)
            {
                case "table":
                    return (CreateTable(Name()));
                case "index":
                    return (CreateIndex(Name()));
                case "database":
                    return (CreateDataBase(Name()));
                case "user":
                    return (CreateUser());
                   
            }
            return false;
        }
        bool CheckType(string s)
        {
            switch(s)
            {
                case "insert":
                    return true;
                case "create":
                    return true;
                case "delete":
                    return true;
                case "drop":
                    return true;
                case "select":
                    return true;
                case "alter":
                    return true;
                case "update":
                    return true;
              
            }
            return false;
        }
        bool SetUser()
        {
            string username = "";
            if (!QueueEquals("user"))
                return false;
            if (!QueueEquals("username"))
                return false;
            if (!QueueEquals("="))
                return false;
            if (Sqls.Count == 0)
                return false;
            username = Name();
            List<string> name = new List<string>();
            List<string> result = new List<string>();
            while (Sqls.Count > 0)
            {
                string type = Sqls.Dequeue();
                if (!CheckType(type))
                    return false;
                name.Add(type);
                if (!QueueEquals("="))
                    return false;
                if (Sqls.Count == 0)
                    return false;
                string s = Sqls.Dequeue();
                if(s!="true"&&s!="false")
                {
                    return false;
                }
                result.Add(s);
               

                if (Sqls.Count != 0)
                {
                    if (!QueueEquals(","))
                        return false;
                }
            }
            JsonData jd = JsonMapper.ToObject(FileHelper.GetFileContenct("Manager.json", FileType.Default));
            for (int i = 0; i < jd.Count; i++)
            {
                if(jd[i]["UserName"].ToString()==username)
                {
                    for (int j = 0; j < name.Count; j++)
                    {
                     
                        jd[i][name[j]] =bool.Parse( result[j]);
                    }
                }
            }
            FileHelper.SetFileContenct("Manager.json",FileType.Default, jd.ToJson(), FileMode.Truncate);
            return true;
        }
        bool CreateUser()
        {
            if (!QueueEquals("username"))
                return false;
            if (!QueueEquals("="))
                return false;
            if (Sqls.Count == 0)
                return false;

            string username = Sqls.Dequeue();
            if (!QueueEquals(","))
                return false;
            if (!QueueEquals("password"))
                return false;
            if (!QueueEquals("="))
                return false;
            if (Sqls.Count == 0)
                return false;
            string password = Sqls.Dequeue();
            if (Sqls.Count != 0)
            {
                if (!QueueEquals("set"))
                    return false;
            }
            List<string> name = new List<string>();
            List<string> result = new List<string>();
            while (Sqls.Count > 0)
            {
                string temp = Sqls.Dequeue();
                if (!CheckType(temp))
                    return false;
                name.Add(temp);
                if (!QueueEquals("="))
                    return false;
                if (Sqls.Count == 0)
                    return false;
                string boo = Sqls.Dequeue();
                result.Add(boo);
                if (boo != "true" && boo != "false")
                    return false;
                if (Sqls.Count != 0)
                {
                    if (!QueueEquals(","))
                        return false;
                }

            }
            if(FileHelper.isExists("Manager.json",FileType.Default))
            {
                JsonData jd = JsonMapper.ToObject(FileHelper.GetFileContenct("Manager.json", FileType.Default));
                JsonData jd2 = new JsonData();
                jd2["UserName"] = username;
                jd2["Password"] = password;
                jd2["select"] = true;
                jd2["update"] = true;
                jd2["delete"] = true;
                jd2["insert"] = true;
                jd2["create"] = true;
                jd2["drop"] = true;
                jd2["alter"] = true;
                for (int i = 0; i < name.Count; i++)
                {
                    jd2[name[i]] =bool.Parse( result[i]);
                }
                jd.Add(jd2);
                jd = jd;
                FileHelper.SetFileContenct("Manager.json", FileType.Default, jd.ToJson(), FileMode.Truncate);
                return true;
            }
            return false;
        }
          bool CreateDataBase(string name) //创建数据库
        {
            bool result = FileHelper.CreateFile(name, FileType.DataBase);
            if (result)
            {
                if (FileHelper.isExists("Manager.json", FileType.Default))
                {
                    int temp=-1;
                    JsonData jd = JsonMapper.ToObject(FileHelper.GetFileContenct("Manager.json", FileType.Default));
                    for (int i = 0; i < jd.Count; i++)
                    {
                        if (jd[i].ToJson()==Manager.UserData.ToJson())
                        {
                            temp = i;
                        }
                    }
                    if (!Manager.UserData.Keys.Contains("DataBase"))
                    {
                        Manager.UserData["DataBase"] = new JsonData();
                    }
                    Manager.UserData["DataBase"].Add(name);
                    jd[temp] = Manager.UserData;
                    FileHelper.SetFileContenct("Manager.json", FileType.Default, jd.ToJson(), FileMode.Truncate);
                }
            }
            
        return   result;   
        }
        bool LegalType(string type)
        {
            if (type == "int" || type == "string")
                return true;

            return false;

        }
        bool GetCreateTypeParamsNameList()
        {
            if (Sqls.Count < 3)
                return false;
           while(Sqls.Count>=3)
            {
                string a = Sqls.Dequeue();
               string b = Sqls.Dequeue();
               if (LegalType(a))
                {
                    return false;
                }
               else
                {
                    
                    paramsname.Add(a);
                }
                if (!LegalType(b))
                {
                    return false;
                }
                else
                {
                  
                    type.Add(b);
                }
                string temp = Sqls.Dequeue();
                if (temp != ")" && temp != ",")
                    return false;
            }

            if (Sqls.Count != 0)
                return false;

            return true;
        }
        bool QueueEquals(string type)
        {
            return Sqls.Count > 0 && Sqls.Dequeue() == type;
        }

         bool CreateTable(string tablename)//创建表
        {
          
            if (!QueueEquals("("))
                return false;
           
            if (!GetCreateTypeParamsNameList())
                return false;
            
            if (Manager.DatabaseName != "")
            {
                bool result = FileHelper.CreateFile(tablename, FileType.Table);
                if (result)
                {
                    JsonData jd = JsonMapper.ToObject(FileHelper.GetFileContenct(Manager.DatabaseName,FileType.DataBase));
                    if (jd.ToJson() == "")
                    {
                        jd = new JsonData();
                        jd.Add(tablename);
                    }
                    else
                    {
                        jd.Add(tablename);

                    }
                    FileHelper.SetFileContenct(Manager.DatabaseName ,FileType.DataBase, jd.ToJson());
                    JsonData jd2 = new JsonData();
                    jd2["Params"] = new JsonData();
                    for (int i = 0; i < paramsname.Count; i++)
                    {
                        JsonData temp = new JsonData();
                        temp["ParamName"] = paramsname[i];
                        temp["Type"] = type[i];
                        jd2["Params"].Add(temp);
                    }

                    FileHelper.SetFileContenct(tablename ,FileType.Table, jd2.ToJson());

                }
                return result;
            }
            
            return false;
           
        }


        bool GetInsertValueList(string tablename)
        {
         
                if (!QueueEquals("values"))
                return false;
            if (!QueueEquals("("))
                return false;
           
            JsonData jd = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename,FileType.Table));
            int Count = jd["Params"].Count;
            while(Sqls.Count>0)
            {
                if (Sqls.Count >= 2)
                {
                    string temp = Sqls.Dequeue();
                    values.Add(temp);

                    string s = Sqls.Dequeue();
                    if (s != ")" && s != ",")
                        return false;
                }
                
            }
            if (Sqls.Count != 0)
                return false;
            if (Count != values.Count)
                return false;

            return true;
        }
        bool GetUpdateValueAndParamName(List<string> paramsnames)
        {
            if (Sqls.Count == 0)
                return false;
            while (Sqls.Count >0 )
            {
                string a = Sqls.Dequeue();

                if (!paramsnames.Contains(a))
                    return false;

                paramsname.Add(a);
                if (!QueueEquals("="))
                    return false;

                if (Sqls.Count ==0)
                    return false;

                string b = Sqls.Dequeue();
                values.Add(b);

                if(Sqls.Count==0)
                {

                }
                else
                {
                       string temp = Sqls.Dequeue();
                        if (temp==",")
                       {

                       }
                        else if(temp=="where")
                        {
                               while(Sqls.Count>0)
                        {
                            string m = Sqls.Dequeue();
                            if (!paramsnames.Contains(m))
                                return false;
                            givenparamsname.Add(m);
                            if (!QueueEquals("="))
                                return false;
                            if (Sqls.Count == 0)
                                return false;
                            givenvalues.Add(Sqls.Dequeue());

                            if(Sqls.Count==0)
                            {

                            }
                            else
                            {
                                if (!QueueEquals("and"))
                                    return false;
                            }
                        }
                        }
                        else
                        return false;
                }

            }
            return true;
        }
        bool UpdateGivenValue(string tablename)
        {
            if (FileHelper.isExists(tablename, FileType.Table))
            {
                if (!QueueEquals("set"))
                    return false;
                List<string> temp = new List<string>();
                JsonData table = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename, FileType.Table));
                for (int i = 0; i < table["Params"].Count; i++)
                {
                    temp.Add(table["Params"][i]["ParamName"].ToString());
                }
                if (!GetUpdateValueAndParamName(temp))
                    return false;

            
                    for (int j = 0; j < table["Values"].Count; j++)
                {
                    bool isEqual = true;
                    for (int i = 0; i < givenparamsname.Count; i++)
                    {
                        if (table["Values"][j][givenparamsname[i]].ToString() != givenvalues[i])
                        {
                            isEqual = false;
 
                        } 
                    }
                 
                    if (isEqual)
                    {
                        for (int i = 0; i < paramsname.Count; i++)
                        {
                            for (int m = 0; m < table["Params"].Count ; m++)
                            {
                                if(table["Params"][m]["ParamName"].ToString()==paramsname[i])
                                {
                                    if(table["Params"][m].Keys.Contains("Index"))
                                    {
                                        JsonData json = JsonMapper.ToObject(FileHelper.GetFileContenct(table["Params"][m]["Index"].ToString() + ".index", FileType.Default));
                                        int count = json.Count;
                                        bool isChange = false;
                                        JsonData RemoveJson = null;
                                        for (int n = 0; n < count; n++)
                                        {
                                            
                                            if (json[n]["Key"].ToString() == givenvalues[i])
                                            {

                                                json[n]["Values"].Remove(j.ToString());
                                                
                                            
                                                    if (json[n]["Values"].Count == 0)
                                                        RemoveJson = json[n];

                                            }
                                            if (json[n]["Key"].ToString()==values[i])
                                            {
                                                isChange = true;
                                                json[n]["Values"].Add(j.ToString());
                                                json[n] = json[n];
                                            }
                                            
                                             
                                        }
                                        if (RemoveJson != null)
                                            json.Remove(RemoveJson);
                                        if (!isChange)
                                        {
                                            JsonData jtemp = new JsonData();
                                            jtemp["Key"] = values[i];
                                            jtemp["Values"] = new JsonData();
                                            jtemp["Values"].Add(j.ToString());
                                            json.Add(jtemp);
                                        }
                                        FileHelper.SetFileContenct(table["Params"][m]["Index"].ToString() + ".index", FileType.Default, json.ToJson(),FileMode.Truncate);
                                    }
                                }
                            }
                            table["Values"][j][paramsname[i]] = values[i];
                        }
                        isEqual = false;
                    }
                }

               
                FileHelper.SetFileContenct(tablename, FileType.Table, table.ToJson());
                return true;
            }

            return false;
        }
         
        bool UpdateAllValue(string tablename)
        {
            if (FileHelper.isExists(tablename,FileType.Table))
                {
                    if (!QueueEquals("set"))
                        return false;
                
                JsonData table = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename, FileType.Table));
               
                if (!GetUpdateValueAndParamName(GetTableParams(tablename)))
                    return false;

                for (int i = 0; i < paramsname.Count; i++)
                {
                    for (int j = 0; j < table["Values"].Count; j++)
                    {
                        for (int m = 0; m < table["Params"].Count; m++)
                        {
                            if (table["Params"][m]["ParamName"].ToString() == paramsname[i])
                            {
                            if (table["Params"][m].Keys.Contains("Index"))
                            {
                                JsonData json = JsonMapper.ToObject(FileHelper.GetFileContenct(table["Params"][m]["Index"].ToString() + ".index", FileType.Default));
                                int count = json.Count;
                                bool isChange = false;
                                    JsonData RemoveJson=null;
                                for (int n = 0; n < count; n++)
                                {
                                        //Console.WriteLine(json.Count);
                                    if (json[n]["Key"].ToString() == table["Values"][j][paramsname[i]].ToString())
                                    {
                                            if (values[i] != json[n]["Key"].ToString())
                                            {
                                                json[n]["Values"].Remove(j.ToString());


                                                if (json[n]["Values"].Count == 0)
                                                    RemoveJson = json[n];
                                            }
                                           
                                           
                                     }
                                   // Console.WriteLine(json[n]["Key"].ToString()+": "+values[i]);
                                    if (json[n]["Key"].ToString() == values[i])       
                                    {
                                        isChange = true;
                                            bool isExist = false;
                                            for (int l = 0; l < json[n]["Values"].Count; l++)
                                            {
                                                  if(json[n]["Values"][l].ToString()==j.ToString())
                                                {
                                                    isExist = true;
                                                    break;
                                                }
                                            }
                                            if(!isExist)
                                        json[n]["Values"].Add(j.ToString());
                                        json[n] = json[n];
                                    }
                                   
                                }
                                    if (RemoveJson!= null)
                                        json.Remove(RemoveJson);

                                    if (!isChange)
                                    {
                                        JsonData jtemp = new JsonData();
                                        jtemp["Key"] = values[i];
                                        jtemp["Values"] = new JsonData();
                                        jtemp["Values"].Add(j.ToString());
                                        json.Add(jtemp);
                                    }
                                    FileHelper.SetFileContenct(table["Params"][m]["Index"].ToString() + ".index", FileType.Default, json.ToJson(), FileMode.Truncate);
                                }
                            }
                        }
                      

                        table["Values"][j][paramsname[i]] = values[i];
                    }
                     
                }
                FileHelper.SetFileContenct(tablename,FileType.Table, table.ToJson());
                return true;
            }
           
            return false;
        }
        bool GetGivenInsertValue(List<string> paramslist)
        {
            if (!QueueEquals("("))
                return false;
            while(Sqls.Count>0)
            {
                string s = Sqls.Dequeue();
                if (!paramslist.Contains(s))
                    return false;

                paramsname.Add(s);
                if (Sqls.Count == 0)
                    return false;
                string t = Sqls.Dequeue();
                if (t == ",")
                {

                }
                else if (t == ")")
                {
                    if(!QueueEquals("values"))
                    {
                        return false;
                    }
                    if (!QueueEquals("("))
                    {
                        return false;
                    }
                    while(Sqls.Count>0)
                    {
                        values.Add(Sqls.Dequeue());
                        if (Sqls.Count == 0)
                            return false;
                        string t2 = Sqls.Dequeue();
                        if(t2==",")
                        {
                            if (Sqls.Count == 0)
                                return false;
                        }
                        else if(t2==")")
                        {
                            if (Sqls.Count != 0)
                                return false;
                        }
                    }
                }
                else
                    return false;
            }
            return true;
        }
       
        bool InsertGivenValue()
        {
            if (!QueueEquals("into"))
                return false;

            string tablename = Name();

            if (!GetGivenInsertValue(GetTableParams(tablename)))
                return false;

            if (FileHelper.isExists(tablename, FileType.Table))
            {
                JsonData table = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename, FileType.Table));
                JsonData jd = new JsonData();

                for (int i = 0; i < paramsname.Count; i++)
                {
                    for (int j = 0; j < table["Params"].Count; j++)
                    {
                     
                        if (table["Params"][j]["ParamName"].ToString() == paramsname[i])
                        {
                          
                                if (table["Params"][j].Keys.Contains("Index"))
                                {
                                bool change = false;
                                JsonData jd2 = JsonMapper.ToObject(FileHelper.GetFileContenct(table["Params"][j]["Index"] + ".index", FileType.Default));
                                if (jd2.ToJson() == "")
                                    jd2.SetJsonType(JsonType.Array);
                                  
                                for (int m = 0; m < jd2.Count; m++)
                                {
                                    if(jd2[m]["Key"].ToString()==values[i])
                                    {
                                        change = true;
                                        jd2[m]["Values"].Add(table["Values"].Count.ToString());
                                        jd2[m] = jd2[m];
                                    }
                                }
                                if (!change)
                                {
                                    JsonData jd3 = new JsonData();
                                    jd3["Key"] = values[i];
                                    jd3["Values"] = new JsonData();
                                    if(table.Keys.Contains("Values"))
                                    jd3["Values"].Add(table["Values"].Count.ToString() );
                                    else
                                    jd3["Values"].Add("0");
                                    jd2.Add(jd3);
                                }
                                FileHelper.SetFileContenct(table["Params"][j]["Index"] + ".index", FileType.Default, jd2.ToJson(), FileMode.Truncate);
                                }
                            
                            if (table["Params"][j]["Type"].ToString() == "string")
                                jd[paramsname[i]] = values[i];
                            else
                                jd[paramsname[i]] = int.Parse(values[i]);
                        }

                    }
                }

                for (int i = 0; i < table["Params"].Count; i++)
                {
                    if (!paramsname.Contains(table["Params"][i]["ParamName"].ToString()))
                    {
                        jd[table["Params"][i]["ParamName"].ToString()] = "";
                    }
                }
                if (!table.Keys.Contains("Values"))
                {
                    table["Values"] = new JsonData();
                }
                table["Values"].Add(jd);
                FileHelper.SetFileContenct(tablename, FileType.Table, table.ToJson(), FileMode.Truncate);
                return true;
            }
            return false;
        }
        bool InsertValue()
        {
           
            if (!QueueEquals("into"))
                return false;

           string tablename = Name();
          
            if (!GetInsertValueList(tablename))
                return false;
           
            if (FileHelper.isExists(tablename, FileType.Table))
            {
                

               JsonData table = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename,FileType.Table));

                if (!table.Keys.Contains("Values"))
                {
                    table["Values"] = new JsonData();
                }
               
                JsonData temp = new JsonData();

                for (int i = 0; i < values.Count; i++)
                {
                    if(table["Params"][i].Keys.Contains("Index"))
                    {
 
                            bool change = false;
                            JsonData jd2 = JsonMapper.ToObject(FileHelper.GetFileContenct(table["Params"][i]["Index"] + ".index", FileType.Default));
                            if (jd2.ToJson() == "")
                                jd2.SetJsonType(JsonType.Array);
                      
                            for (int m = 0; m < jd2.Count; m++)
                            {
                                if (jd2[m]["Key"].ToString() == values[i])
                                {
                                    change = true;
                             
                                jd2[m]["Values"].Add(table["Values"].Count.ToString());           
                                jd2[m] = jd2[m];
                            }
                            }

                            if (!change)
                            {
                                JsonData jd3 = new JsonData();
                                jd3["Key"] = values[i];
                                jd3["Values"] = new JsonData();
                         
                                if (table.Keys.Contains("Values"))
                                    jd3["Values"].Add(table["Values"].Count.ToString());
                                else
                                    jd3["Values"].Add("0");

                                jd2.Add(jd3);
                            }
                        
                        FileHelper.SetFileContenct(table["Params"][i]["Index"] + ".index", FileType.Default, jd2.ToJson(), FileMode.Truncate);
 
                    }
                   if(table["Params"][i]["Type"].ToString()=="string")
                    temp[table["Params"][i]["ParamName"].ToString()]  = values[i];
                   else
                    temp[table["Params"][i]["ParamName"].ToString()] =int.Parse(values[i]);              
                }

                table["Values"].Add(temp);
             
                FileHelper.SetFileContenct(tablename ,FileType.Table, table.ToJson());

                //JsonData test = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename + ".table"));
                //Console.WriteLine(test[2].ToJson());
                return true;
            }

            return false;
        
        }
        bool Delete()
        {
          
            if (!QueueEquals("from"))
                return false;
            if (Sqls.Contains("where"))
            {
                return DeleteGivenValue(Name());
            }
            else
            {
                return DeleteAllValue(Name());
            }
        }
        List<string> GetTableParams(string tablename)//获得表的所有属性名
        {
            if (FileHelper.isExists(tablename, FileType.Table))
            {
                List<string> temp = new List<string>();
                JsonData table = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename, FileType.Table));
                for (int i = 0; i < table["Params"].Count; i++)
                {
                    temp.Add(table["Params"][i]["ParamName"].ToString());
                }
                return temp;
            }
            return null;
         }
        bool GetDeleteGivenList(List<string> Param)
        {
            if (Sqls.Count == 0)
                return false;
            if (!QueueEquals("where"))
                return false;
           
               while (Sqls.Count > 0)
              {
                         string m = Sqls.Dequeue();
                            if (!Param.Contains(m))
                                return false;
                            givenparamsname.Add(m);
                            if (!QueueEquals("="))
                                return false;
                            if (Sqls.Count == 0)
                                return false;
                            givenvalues.Add(Sqls.Dequeue());

                            if (Sqls.Count == 0)
                            {

                            }
                            else
                            {
               
                                if (QueueEquals("and"))
                                    return false;
                              if (Sqls.Count==0)
                                 return false;
                }
               }
           
 
            return true;
        }
        //bool CreateSparseIndex(string indexname)//稀疏索引
        //{

        //}
        //bool CreateDenseIndex(string indexname)//稠密索引
        //{

        //}
        bool Select()
        {
            if (Sqls.Count == 0)
                return false;
 
                return (SelectGivenValue());
         
        }
        bool GetSelectParamList()
        {
            if (Sqls.Count == 0)
                return false;

            while (Sqls.Count > 0)
            {

                string temp = Sqls.Dequeue();
                paramsname.Add(temp);
              
                if (Sqls.Count == 0)
                    return false;
                string a = Sqls.Dequeue();
                if (a == ",")
                {
                    if (Sqls.Count == 0)
                        return false;
                }
                else if (a == "from")
                {
                    if (Sqls.Count == 0)
                        return false;

                    int time = 1;
                    while (Sqls.Count > 0)
                    {
                      
                        string tablaname = Name();
                       
                        for (int i = 0; i < Manager.UserData["DataBase"].Count; i++)
                        {
                           
                            JsonData jd2 = JsonMapper.ToObject(FileHelper.GetFileContenct(Manager.UserData["DataBase"][i].ToString(), FileType.DataBase));
                            
                            for (int j = 0; j < jd2.Count; j++)
                            {
                               
                                if (jd2[j].ToString() == tablaname)
                                {
                                    tablename.Add(tablaname);
                                }
                            }
                            
                        }

                     
                        if (tablename.Count != time)
                            return false;

                        if (Sqls.Count == 0)
                        {
                            List<string> AllParams = new List<string>();
                            for (int i = 0; i < tablename.Count; i++)
                            {

                                JsonData jd2 = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename[i], FileType.Table));
                                for (int j = 0; j < jd2["Params"].Count; j++)
                                {
                                    AllParams.Add(jd2["Params"][j]["ParamName"].ToString());
                                }

                            }

                            for (int i = 0; i < paramsname.Count; i++)
                            {

                                if (!AllParams.Contains(paramsname[i]) && paramsname[i] != "*")
                                    return false;
                            }

                            return true;
                        }
                        
                        string b = Sqls.Dequeue();
                        if (b == ",")
                        {
                            if (Sqls.Count == 0)
                                return false;
                            time++;
                        }
                        
                        else if (b == "where")
                        {
                       
                            List<string> AllParams = new List<string>();
                            for (int i = 0; i < tablename.Count; i++)
                            {
                               
                              JsonData jd2 = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename[i], FileType.Table));
                                for (int j = 0; j < jd2["Params"].Count; j++)
                                {
                                    AllParams.Add(jd2["Params"][j]["ParamName"].ToString());
                                }
                                 
                            }
                            
                            for (int i = 0; i < paramsname.Count; i++)
                            {
                              
                                if (!AllParams.Contains(paramsname[i])&& paramsname[i]!="*")
                                    return false;
                            }
                            
                            if (Sqls.Count == 0)
                                return false;
                           
                            while (Sqls.Count > 0)
                            {

                                string c = Sqls.Dequeue();
                              
                                if (!AllParams.Contains(c))
                                    return false;
                              
                                givenparamsname.Add(c);

                                if (Sqls.Count == 0)
                                    return false;

                                string d = Sqls.Dequeue();
                                
                                if (d == "=")
                                {
                                    operation.Add(d);
                                }
                                else if (d == "<")
                                {
                                    operation.Add(d);
                                }
                                else if (d == ">")
                                {
                                    operation.Add(d);
                                }
                                else
                                 return false;

                                if (Sqls.Count == 0)
                                    return false;

                                string e = Sqls.Dequeue();
                              
                                 givenvalues.Add(e);
                             
                               if(Sqls.Count==0)
                                {
                                    return true;
                                }
                               else
                                {
                                  
                                    if (!QueueEquals("and"))
                                        return false;
                                  
                                }


                            }


                        }
                        
                        else
                         return false;
                    }
                }
                else
                return false;
            }
            return false;
        }
        bool DropIndex(string indexname)
        {
            if (!QueueEquals("on"))
                return false;

            string tablename = Name();

            if(FileHelper.isExists(tablename,FileType.Table))
            {
              
                JsonData table = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename,FileType.Table));
                for (int i = 0; i < table["Params"].Count; i++)
                {
                   if( table["Params"][i].Keys.Contains("Index"))
                    {
                       
                      if(  table["Params"][i]["Index"].ToString()==indexname)
                        {

                          table["Params"][i].Remove("Index");
                            FileHelper.SetFileContenct(tablename, FileType.Table, table.ToJson(),FileMode.Truncate);
                           return FileHelper.Delete(indexname + ".index", FileType.Default);
                        }
                    }
                }
            }
            
            return false;
        }
        bool CreateIndex(string indexname)
        {
            string tablename = "";
            if (!GetIndexParamList(ref tablename))
                return false;
          
            
            if(FileHelper.isExists(tablename,FileType.Table))
            {
                JsonData jd = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename, FileType.Table));
                for (int i = 0; i < paramsname.Count; i++)
                {
                   
                    for (int j = 0; j < jd["Params"].Count; j++)
                    {
                        if(jd["Params"][j]["ParamName"].ToString()==paramsname[i])
                        {
                          

                            if (jd["Params"][j].Keys.Contains("Index"))
                                return false;

                            if (FileHelper.CreateFile(indexname + ".index", FileType.Default))
                            {
                                B_Tree Tree = new B_Tree(5);
                                JsonData tree = new JsonData();

                                if (jd.Keys.Contains("Values"))
                                {
                                    for (int k = 0; k < jd["Values"].Count; k++)
                                    {


                                        LeafNode temp = new LeafNode(5);
                                        if (!Tree.Find(jd["Values"][k][paramsname[i]].ToString(), out temp))
                                        {
                                            JsonData tmptree = new JsonData();
                                            tmptree["Key"] = jd["Values"][k][paramsname[i]].ToString();
                                            tmptree["Values"] = new JsonData();
                                            tmptree["Values"].Add(k.ToString());
                                            tree.Add(tmptree);
                                            Tree.Insert(new Record(jd["Values"][k][paramsname[i]].ToString(), new List<string>() { k.ToString() }));
                                        }
                                        else
                                        {
                                            for (int l = 0; l < tree.Count; l++)
                                            {
                                                if (tree[l]["Key"].ToString() == jd["Values"][k][paramsname[i]].ToString())
                                                {
                                                    tree[l]["Values"].Add(k.ToString());
                                                }
                                            }
                                            temp.Records.Find(t => { return t.Key == jd["Values"][k][paramsname[i]].ToString(); }).Datas.Add(k.ToString());
                                        }

                                    }
                                }
                                 
                              
                                FileHelper.SetFileContenct(indexname + ".index", FileType.Default, tree.ToJson());

                            }
                            else { return false; }

                            jd["Params"][j]["Index"] =indexname;
                            FileHelper.SetFileContenct(tablename, FileType.Table, jd.ToJson(), FileMode.Truncate);
                            break;
                        }
                    }
                }
            }
            return true; 
        }
        bool GetIndexParamList(ref string tablename)
        {
            if(!QueueEquals("on"))
            {
                return false;
            }
          
            if (Sqls.Count == 0)
                return false;

              tablename = Name();
           
            if (!FileHelper.isExists(tablename, FileType.Table))
                return false;
            
            if (!QueueEquals("("))
                return false;

            
            if (Sqls.Count == 0)
                return false;
          
            while (Sqls.Count > 0)
            {
                string a = Sqls.Dequeue();
                if (GetTableParams(tablename).Contains(a))
                    paramsname.Add(a);

                if (Sqls.Count == 0)
                    return false;

                string b = Sqls.Dequeue();
                if (b == ",")
                {
                    if (Sqls.Count == 0)
                        return false;


                }
                else if (b == ")")
                {
                 
                    if (Sqls.Count != 0)
                        return false;

                   
                }
            }
            return true;
             
        }
        
        List<JsonData> GetConnectTableValueList(List<JsonData> tables, string param,string operation, string value)
        {
           
            List<JsonData> Values = new List<JsonData>();
            JsonData First = tables[0];
            tables.RemoveAt(0);
            JsonData Second = tables[0];
            tables.RemoveAt(0);
            string type = "";
            for (int n = 0; n < First["Params"].Count; n++)
            {
                if (First["Params"][n]["ParamName"].ToString() == param)
                {
                    type = First["Params"][n]["Type"].ToString();
                    break;
                }
            }

            for (int i = 0; i < First["Values"].Count; i++)
                {
                  
                    for (int j  = 0; j < Second["Values"].Count; j++)
                    {
                        
                        JsonData json = new JsonData();
                        

                        if (Compare(First["Values"][i][param].ToString(), Second["Values"][j][value].ToString(), operation, type == "string" ? true : false))
                        {
                            for (int l = 0; l < First["Params"].Count; l++)
                            {

                                json[First["Params"][l]["ParamName"].ToString()] = First["Values"][i][First["Params"][l]["ParamName"].ToString()];
                            }
                            for (int m = 0; m < Second["Params"].Count; m++)
                            {

                                json[Second["Params"][m]["ParamName"].ToString()] = Second["Values"][j][Second["Params"][m]["ParamName"].ToString()];
                            }
                           
                                Values.Add(json);
                        }
                    }
                }
            return Values;
        }
        List<JsonData> GetConnectTableValueList(List<JsonData> tables)
        {
            List<JsonData> Values = new List<JsonData>();
            JsonData First = tables[0];
            tables.RemoveAt(0);
            int Count = tables.Count;
            while (Count > 0)
            {
                JsonData Second = tables[0];
                tables.RemoveAt(0);
                JsonData temp = new JsonData();
                temp["Params"] = new JsonData();
                temp["Values"] = new JsonData();
                for (int l = 0; l < First["Params"].Count; l++)
                {
                    temp["Params"].Add(First["Params"][l]);
                }
                for (int m = 0; m < Second["Params"].Count; m++)
                {
                    temp["Params"].Add(Second["Params"][m]);
                }
                for (int i = 0; i < First["Values"].Count; i++)
                {

                    for (int j = 0; j < Second["Values"].Count; j++)
                    {

                        JsonData json = new JsonData();
                        for (int l = 0; l < First["Params"].Count; l++)
                        {

                            json[First["Params"][l]["ParamName"].ToString()] = First["Values"][i][First["Params"][l]["ParamName"].ToString()];
                        }
                        for (int m = 0; m < Second["Params"].Count; m++)
                        {

                            json[Second["Params"][m]["ParamName"].ToString()] = Second["Values"][j][Second["Params"][m]["ParamName"].ToString()];
                        }
                        temp["Values"].Add(json);
                        if (tables.Count == 0)
                            Values.Add(json);
                    }
                }
                First = temp;

                Count--;
            }
            return Values;
        }
        JsonData ConnectTable(List<JsonData> tables,string param,string operation,string value)
        {
            JsonData temp = new JsonData();
            temp["Params"] = new JsonData();
            temp["Values"] = new JsonData();
            List<JsonData> Values = GetConnectTableValueList(new List<JsonData>(tables),param,operation,value);  
            for (int i = 0; i < tables.Count; i++)
            {
                for (int j = 0; j < tables[i]["Params"].Count; j++)
                {
                    temp["Params"].Add(tables[i]["Params"][j]);
                   
                }
            }
            for (int i = 0; i < Values.Count; i++)
            {
                temp["Values"].Add(Values[i]);
            }
            return temp;
        }
        bool Compare(string a,string b,string operation,bool isstring)
        {
            
            int i=-2;
            if (isstring)
            {
                 i = a.CompareTo(b);
            }
            else
            {

                
                i = int.Parse(a).CompareTo(int.Parse(b));
            }
            switch (operation)
            {
                case "=":
                    if (i==0)
                        return true;
                    break;
                case ">":
                    if (i==1)
                        return true;
                    break;
                case "<":
                    if (i==-1)
                        return true;
                    break;
            }
            return false;
        }
        JsonData ChooseTable(string givenparam,string operation,string givenvalue,JsonData table)
        {
          
           
            Stopwatch sw = new Stopwatch();
            sw.Start();

            //耗时巨大的代码  

           
            //耗时巨大的代码  
            JsonData result = new JsonData();
            result["Params"] = new JsonData();
            result["Values"] = new JsonData();
            List<string> temp = new List<string>();
            for (int i = 0; i < table["Params"].Count; i++)
            {
                result["Params"].Add(table["Params"][i]);
                temp.Add(table["Params"][i]["ParamName"].ToString());          
            }
         
                if (!temp.Contains(givenparam))
                 return null;
            

            //for (int l = 0;l < givenvalue.Count;l++)
            //{
            //    for (int k = 0; k < table["Params"].Count; k++)
            //    {
            //        int count = 0;
            //        if (table["Params"][k]["ParamName"].ToString() == givenparam[l])
            //        {
            //            if (table["Params"][k].Keys.Contains("Index"))
            //            {
            //                List<string> rows = SelectByIndex(table["Params"][k]["Index"].ToString(), givenvalue[l]);
            //            }
            //            else
            //            {
            //                for (int j = 0; j < table["Values"].Count; j++)
            //                {
            //                    for (int i = 0; i < givenvalue.Count; i++)
            //                    {
            //                        if (!temp.Contains(givenvalue[i]))
            //                        {
            //                            string type = "";
            //                            for (int m = 0; m < table["Params"].Count; m++)
            //                            {
            //                                if (table["Params"][m]["ParamName"].ToString() == givenparam[i])
            //                                {
            //                                    type = table["Params"][m]["Type"].ToString();
            //                                    break;
            //                                }
            //                            }
            //                            if (Compare(table["Values"][j][givenparam[i]].ToString(), givenvalue[i], operation[i], type == "string" ? true : false))
            //                            {
            //                                count++;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            isconnected = true;
            //                            string type = "";
            //                            for (int m = 0; m < table["Params"].Count; m++)
            //                            {
            //                                if (table["Params"][m]["ParamName"].ToString() == givenparam[i])
            //                                {
            //                                    type = table["Params"][m]["Type"].ToString();
            //                                    break;
            //                                }
            //                            }
            //                            if (Compare(table["Values"][j][givenparam[i]].ToString(), table["Values"][j][givenvalue[i]].ToString(), operation[i], type == "string" ? true : false))
            //                            {

            //                                count++;

            //                            }
            //                        }
            //                    }


            //                    if (count == givenvalue.Count)
            //                    {
            //                        result["Values"].Add(table["Values"][j]);
            //                    }


            //                }
            //            }


            //        }
            //    }
            //}

            for (int j = 0; j < table["Values"].Count; j++)
            {

                        string type = "";
                        for (int m = 0; m < table["Params"].Count; m++)
                        {
                            if (table["Params"][m]["ParamName"].ToString() == givenparam)
                            {
                                type = table["Params"][m]["Type"].ToString();
                                break;
                            }
                        }
                        if (Compare(table["Values"][j][givenparam].ToString(), givenvalue, operation, type == "string" ? true : false))
                        {
                    result["Values"].Add(table["Values"][j]);
                        }
            }

            sw.Stop();
            TimeSpan ts2 = sw.Elapsed;
            Console.WriteLine("选择总共花费{0}ms.", ts2.TotalMilliseconds);
            return result;
        }
        JsonData TableCartesianProduct(List<JsonData> tables)
        {
            if (tables.Count == 1)
                return tables[0];

            JsonData temp = new JsonData();
            temp["Params"] = new JsonData();
            temp["Values"] = new JsonData();
         
            List<JsonData> Values = GetConnectTableValueList(new List<JsonData>(tables));
            for (int i = 0; i < tables.Count; i++)
            {
                for (int j = 0; j < tables[i]["Params"].Count; j++)
                {
                    temp["Params"].Add(tables[i]["Params"][j]);

                }
            }
            for (int i = 0; i < Values.Count; i++)
            {
                temp["Values"].Add(Values[i]);
            }
            return temp;
        }
        JsonData SelectDataFromTable(List<string> paramname,JsonData table)
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < table["Params"].Count; i++)
            {
                temp.Add(table["Params"][i]["ParamName"].ToString());
            }
            for (int i = 0; i < paramname.Count; i++)
            {
                if (!temp.Contains(paramname[i])&&paramname[i]!="*")
                    return null;
            }
            List<int> Rows = new List<int>();
            JsonData Result = new JsonData();
            Result["Params"] = new JsonData();
            Result["Values"] = new JsonData();
            for (int i = 0; i < paramname.Count; i++)
            {
                for (int j = 0; j < table["Params"].Count; j++)
                {
                    if (paramname[i] != "*")
                    {
                        if (table["Params"][j]["ParamName"].ToString() == paramname[i])
                        {
                            Result["Params"].Add(table["Params"][j]);
                        }
                    }
                    else
                    {
                        Result["Params"].Add(table["Params"][j]);
                    }
                }
                
            }
            for (int j = 0; j < table["Values"].Count; j++)
            {
                JsonData tempjson = new JsonData();
                for (int i = 0; i < paramname.Count; i++)
                {
                    if (paramname[i] != "*")
                    {
                        tempjson[paramname[i]] = table["Values"][j][paramname[i]];
                    }
                    else
                    {
                            for (int k = 0; k < table["Params"].Count; k++)
                            {
                                tempjson[table["Params"][k]["ParamName"].ToString()] = table["Values"][j][table["Params"][k]["ParamName"].ToString()];
                            }
                    }
                }
                Result["Values"].Add(tempjson);
            }
            return Result;
        }
        List<string> SelectByIndex(string indexname,string key)
        {
            DateTime beforDT = System.DateTime.Now;

            //耗时巨大的代码  

           
            Record record = null;
            B_Tree tree = new B_Tree(5);
            JsonData index = JsonMapper.ToObject(FileHelper.GetFileContenct(indexname + ".index", FileType.Default));

            if (index == null)
                return null;
            for (int i = 0; i < index.Count; i++)
            {
                List<string> temp = new List<string>();
                for (int j = 0; j < index[i]["Values"].Count; j++)
                {
                    temp.Add(index[i]["Values"][j].ToString());
                }
                tree.Insert(new Record(index[i]["Key"].ToString(), temp));
            }
            tree.Find(key, out record);
            if (record == null)
                return null;

            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforDT);
            Console.WriteLine("索引查找总共花费{0}ms.", ts.TotalMilliseconds);
            return record.Datas;
        }
        JsonData GetTableByParam(List<JsonData> table, string param)
        {
            for (int i = 0; i < table.Count; i++)
            {
                List<string> tableparams = new List<string>();
             
                for (int j = 0; j < table[i]["Params"].Count; j++)
                {
                    tableparams.Add(table[i]["Params"][j]["ParamName"].ToString());
                }
                if (tableparams.Contains(param))
                    return table[i];
            }
            return null;
        }
        bool SelectGivenValue()
        {
            //耗时巨大的代码  
            if (!GetSelectParamList())
                return false;
            List<string> tableparams = new List<string>();
            List<JsonData> tables = new List<JsonData>();
            for (int i = 0; i < tablename.Count; i++)
            {
                JsonData table = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename[i], FileType.Table));
                tables.Add(table);
                for (int j = 0; j < table["Params"].Count; j++)
                {
                    tableparams.Add(table["Params"][j]["ParamName"].ToString());
                }       
            }
            if (givenvalues.Count > 0)
            {
                List<JsonData> NowTable = new List<JsonData>();
                for (int i = 0; i < givenvalues.Count; i++)
                {
                    if (!tableparams.Contains(givenvalues[i]))
                    {
                        JsonData json = GetTableByParam(tables, givenparamsname[i]);
                        JsonData ChooseResult = ChooseTable(givenparamsname[i], operation[i], givenvalues[i], json);
                        tables[tables.FindIndex((t) => { return t.ToJson() == json.ToJson(); })] = ChooseResult;
                        NowTable.Add(ChooseResult);
                    }

                }
              
                for (int i = 0; i < givenvalues.Count; i++)
                {
                    if (tableparams.Contains(givenvalues[i]))
                    {
                        JsonData first = GetTableByParam(tables, givenparamsname[i]);
                        JsonData second = GetTableByParam(tables, givenvalues[i]);
                        NowTable.Remove(first);
                        NowTable.Remove(second);
                        tables.Remove(first);
                        tables.Remove(second);
                        JsonData ConnectResult = ConnectTable(new List<JsonData>() {first,second }, givenparamsname[i], operation[i], givenvalues[i]);
                        NowTable.Add(ConnectResult);
                        tables.Add(ConnectResult);
                    }
                }

                JsonData result = SelectDataFromTable(paramsname, TableCartesianProduct(NowTable));
                ShowTable(result);

            }
            else
            {
                JsonData result = SelectDataFromTable(paramsname, TableCartesianProduct(tables));
                ShowTable(result);
            }
            return true;
        }
        bool AlterTable()
        {
            if (!QueueEquals("table"))
                return false;

            if (Sqls.Count == 0)
                return false;

            string tablename = Name();

            if (Sqls.Count == 0)
                return false;

            string next = Sqls.Dequeue();

            if(next=="add")
            {
                return (AlterTableAdd(tablename));
            }
            if(next=="drop")
            {
                return (AlterTableDrop(tablename));
            }
            return false;
        }
        bool AlterTableAdd(string tablename)
        {
            if (FileHelper.isExists(tablename, FileType.Table))
            {
                JsonData table = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename, FileType.Table));

                if (Sqls.Count == 0)
                    return false;

                string paramname = Sqls.Dequeue();

                if (GetTableParams(tablename).Contains(paramname))
                    return false;

                if (Sqls.Count == 0)
                    return false;
                string paramtype = Sqls.Dequeue();
                if (!LegalType(paramtype))
                    return false;

                JsonData jd = new JsonData();
                jd["ParamName"] = paramname;
                jd["Type"] = paramtype;
                table["Params"].Add(jd);
                for (int i = 0; i < table["Values"].Count; i++)
                {

                    table["Values"][i][paramname] = "";
                }
                FileHelper.SetFileContenct(tablename, FileType.Table, table.ToJson(), FileMode.Truncate);
                return true;
            }
            return false;
        }
        bool AlterTableDrop(string tablename)
        {
            if (!QueueEquals("column"))
                return false;
            if (FileHelper.isExists(tablename, FileType.Table))
            {
                JsonData table = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename, FileType.Table));

                if (Sqls.Count == 0)
                    return false;

                string paramname = Sqls.Dequeue();
                for (int i = 0; i < table["Params"].Count; i++)
                {
                    if (table["Params"][i]["ParamName"].ToString()== paramname)
                    {
                        if(table["Params"][i].Keys.Contains("Index"))
                        {
                            FileHelper.Delete(table["Params"][i]["Index"].ToString() + ".index", FileType.Default);
                        }
                        table["Params"].Remove(table["Params"][i]);
                    }
                }
                for (int i = 0; i < table["Values"].Count; i++)
                {
                    table["Values"][i].Remove(paramname);
                }
                FileHelper.SetFileContenct(tablename, FileType.Table, table.ToJson(), FileMode.Truncate);
                return true;
            }
            return false;
        }
        int[] GetMaxLength(JsonData table)
        {
            int[] MaxLength = null;
           
                MaxLength = new int[table["Params"].Count];
                if (!table.Keys.Contains("Values"))
                {
                    return MaxLength;
                }
                for (int i = 0; i < table["Params"].Count; i++)
                {
                    for (int j = 0; j < table["Values"].Count; j++)
                    {
                           if(System.Text.Encoding.Default.GetBytes(table["Params"][i]["ParamName"].ToString()).Length > MaxLength[i] - 2)
                    {
                        MaxLength[i] = System.Text.Encoding.Default.GetBytes(table["Params"][i]["ParamName"].ToString()).Length + 2;
                    }
                    if (System.Text.Encoding.Default.GetBytes(table["Values"][j][table["Params"][i]["ParamName"].ToString()].ToString()).Length>MaxLength[i]-2)
                        {
                            MaxLength[i] = System.Text.Encoding.Default.GetBytes(table["Values"][j][table["Params"][i]["ParamName"].ToString()].ToString()).Length+2;
                        }
                    }
                }
               
            
            return MaxLength;
        }
        void ShowTable(JsonData table)
        {
           // JsonData table = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename, FileType.Table));
            int[] Length = GetMaxLength(table);
         
                
                for (int j = 0; j < table["Params"].Count; j++)
                {
                  
                    
                    Console.Write(table["Params"][j]["ParamName"].ToString());
                    for (int m = 0; m < Length[j] - System.Text.Encoding.Default.GetBytes(table["Params"][j]["ParamName"].ToString()).Length; m++)
                    {
                        Console.Write(" ");
                    }
                 
                }
            
            Console.WriteLine();
            if (table.Keys.Contains("Values"))
            {

                for (int i = 0; i < table["Values"].Count; i++)
                {

                    for (int j = 0; j < table["Params"].Count; j++)
                    {
                        Console.Write(table["Values"][i][table["Params"][j]["ParamName"].ToString()].ToString());
                        for (int m = 0; m < Length[j] - System.Text.Encoding.Default.GetBytes(table["Values"][i][table["Params"][j]["ParamName"].ToString()].ToString()).Length; m++)
                        {
                            Console.Write(" ");
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
       
            
        bool DeleteGivenValue(string tablename)
        {

            if (FileHelper.isExists(tablename, FileType.Table))
            {
 
                JsonData table = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename, FileType.Table));
               
                if (!GetDeleteGivenList(GetTableParams(tablename)))
                    return false;

                List<JsonData> removedata = new List<JsonData>();
                for (int j = 0; j < table["Values"].Count; j++)
                {
                    bool isEqual = true;
                    for (int i = 0; i < givenparamsname.Count; i++)
                    {
                        if (table["Values"][j][givenparamsname[i]].ToString() != givenvalues[i])
                        {
                            isEqual = false;

                        }
                    }
             
                    if (isEqual)
                    {
                        for (int i = 0; i < table["Params"].Count; i++)
                        {
                            for (int m = 0; m < givenparamsname.Count; m++)
                            {
                                if (table["Params"][i].Keys.Contains("Index"))
                                {
                                    if (table["Params"][i]["ParamName"].ToString() == givenparamsname[m])
                                    {
                                        JsonData temp = JsonMapper.ToObject(FileHelper.GetFileContenct(table["Params"][i]["Index"]+".index",FileType.Default));
                                        JsonData removejson = null;
                                        for (int l = 0; l < temp.Count; l++)
                                        {
                                            if(temp[i]["Key"].ToString()==givenvalues[m])
                                            {
                                                temp[i]["Values"].Remove(j.ToString());

                                                if (temp[i]["Values"].Count == 0)
                                                    removejson = temp[i];

                                                temp[i] = temp[i];

                                            }
                                        }
                                        if (removejson != null)
                                            temp.Remove(removejson);
                                        FileHelper.SetFileContenct(table["Params"][i]["Index"] + ".index", FileType.Default, temp.ToJson(), FileMode.Truncate);
                                    }
                                }
                            }
                        }
                        removedata.Add(table["Values"][j]);
                      
                        
                        isEqual = false;
                    }
                }
                for (int i = 0; i < removedata.Count; i++)
                {
                    table["Values"].Remove(removedata[i]);
                }
            
                FileHelper.SetFileContenct(tablename, FileType.Table, table.ToJson(),FileMode.Truncate);
                return true;
            }

            return false;
        }
        bool DeleteAllValue(string tablename)
        {
            if (FileHelper.isExists(tablename, FileType.Table))
            {
                JsonData table = JsonMapper.ToObject(FileHelper.GetFileContenct(tablename, FileType.Table));
                for (int i = 0; i < table["Params"].Count; i++)
                {
                    if(table["Params"][i].Keys.Contains("Index"))
                    {
                        JsonData temp = JsonMapper.ToObject(FileHelper.GetFileContenct(table["Params"][i]["Index"] + ".index", FileType.Default));
                        temp.Clear();
                        FileHelper.SetFileContenct(table["Params"][i]["Index"] + ".index", FileType.Default, temp.ToJson(), FileMode.Truncate);
                    }
                }
                table.Remove("Values");
               
                FileHelper.SetFileContenct(tablename, FileType.Table, table.ToJson(),FileMode.Truncate);
                return true;
            }   
            return false;
        }

        
    }
}
