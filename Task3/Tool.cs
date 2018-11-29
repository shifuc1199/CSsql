using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using LitJson;
using System.Reflection;
namespace Task3
{
    static class Tool
    {
        public static string SaveTree(JsonData jsondata, ChildNode cn)
        {
            JsonData jd = new JsonData();
            jd["Keys"] = new JsonData();
            jd["NextNodes"] = new JsonData();
            for (int i = 0; i < cn.Keys.Count; i++)
            {
                jd["Keys"].Add(cn.Keys[i]);
            }
            if (jsondata.Keys.Contains("NextNodes"))
            {
                jsondata["NextNodes"].Add(jd);
            }
            else
            {
                jsondata = new JsonData();
                jsondata.Add(jd);
            }
            for (int i = 0; i < cn.Nodes.Count; i++)
            {
                if (cn.Nodes[i] is ChildNode)
                    SaveTree(jd, (cn.Nodes[i] as ChildNode));
                else
                {
                    JsonData jd2 = new JsonData();
                    jd2["Keys"] = new JsonData();
                    jd2["NextNodes"] = new JsonData();
                    for (int j = 0; j < (cn.Nodes[i] as LeafNode).Records.Count; j++)
                    {
                        jd2["Keys"].Add((cn.Nodes[i] as LeafNode).Records[j].Key);

                    }
                    jd["NextNodes"].Add(jd2);
                }
            }
            return jsondata.ToJson();
        }
        public static B_Tree LoadTree(string json)//读取树
        {
            B_Tree tree = new B_Tree(5);
            JsonData jd = JsonMapper.ToObject(json);
            return tree;
        }
        public static bool Remove(this JsonData t,object obj)
        {
            Type type = typeof(JsonData);
            FieldInfo fi_inst_object = type.GetField("inst_object", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fi_object_list = type.GetField("object_list", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fi_inst_array = type.GetField("inst_array", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fi_json = type.GetField("json", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo ToJsonData = type.GetMethod("ToJsonData", BindingFlags.NonPublic | BindingFlags.Instance);
            //json = null;
            fi_json.SetValue(t, null);
            if (t.IsObject)
            {
                JsonData value = null;
                
                if (((IDictionary<string, JsonData>)fi_inst_object.GetValue(t)).TryGetValue((string)obj, out value))
                    return ((IDictionary<string, JsonData>)fi_inst_object.GetValue(t)).Remove((string)obj) && ((IList<KeyValuePair<string, JsonData>>)fi_object_list.GetValue(t)).Remove(new KeyValuePair<string, JsonData>((string)obj, value));
                else
                    throw new KeyNotFoundException("The specified key was not found in the JsonData object.");
            }
            if (t.IsArray)
            {
                return ((IList<JsonData>)fi_inst_array.GetValue(t)).Remove(ToJsonData.Invoke(t,new object[] { obj }) as JsonData);
            }
            throw new InvalidOperationException(
                    "Instance of JsonData is not an object or a list.");
        }
       
        public static string GetLegalString(string sql)
        {
           
            string result = "";
            sql = new Regex(@"[(]+").Replace(sql, "(");
            sql = new Regex(@"[)]+").Replace(sql, ")");
            for (int i = 0; i < sql.Length; i++)
            {
                result += sql[i];
                if (i < sql.Length - 1)
                {
                    if (sql[i + 1] == ';')
                    {
                        result += ' ';
                    }
                    if (sql[i] == ';')
                    {
                        result += ' ';
                    }
                    if (sql[i + 1] == '>')
                    {
                        result += ' ';
                    }
                    if (sql[i] == '>')
                    {
                        result += ' ';
                    }
                    if (sql[i + 1] == '<')
                    {
                        result += ' ';
                    }
                    if (sql[i] == '<')
                    {
                        result += ' ';
                    }
                    if (sql[i + 1] == '=')
                    {
                        result += ' ';
                    }
                    if (sql[i] == '=')
                    {
                        result += ' ';
                    }
                    if (sql[i + 1] == ',')
                    {
                        result += ' ';
                    }
                    if (sql[i] == ',')
                    {
                        result += ' ';
                    }
                    if (sql[i + 1] == '(')
                    {
                        result += ' ';
                    }
                    if (sql[i + 1] == ')')
                    {
                        result += ' ';
                    }
                    if (sql[i] == '(')
                    {
                        result += ' ';
                    }
                }

            }
            result = result.Replace("\'"," ");


            result = result.ToLower();
            
            result = new Regex("[\\s+]").Replace(result, " ");
            return result;
        }
      
    }
}
