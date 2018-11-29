using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Task3
{
    class Program
    {
         
        
        static void Main(string[] args)
        {


            //B_Tree tree = new B_Tree(5);

            //tree.Insert(new Record("13", new List<string>() { "1" }));
            //tree.Insert(new Record("8", new List<string>() { "1" }));
            //tree.Insert(new Record("10", new List<string>() { "1" }));
            //tree.Insert(new Record("15", new List<string>() { "1" }));
            //tree.Insert(new Record("2", new List<string>() { "1" }));
            //tree.Insert(new Record("14", new List<string>() { "1" }));
            //tree.Insert(new Record("16", new List<string>() { "1" }));
            //tree.Insert(new Record("17", new List<string>() { "1" }));
            //tree.Insert(new Record("18", new List<string>() { "1" }));
            //tree.Insert(new Record("19", new List<string>() { "1" }));
            //tree.Insert(new Record("20", new List<string>() { "1" }));
            //tree.Insert(new Record("21", new List<string>() { "1" }));
            //tree.Insert(new Record("22", new List<string>() { "1" }));
            //JsonData jd = new JsonData();
            //tree.Delete("13", ref tree.RootNode);
            //tree.Show();
            //tree.ShowRoot();
            while (true)
            {



                SqlHelper s = new SqlHelper();
                string sql = Console.ReadLine();
                string[] sqls = sql.Split(';');
                for (int i = 0; i < sqls.Length; i++)
                {
                    s.Init();
                    Console.WriteLine(s.SqlParser(sqls[i]));
                }


            }
            //  SqlHelper.SqlParser(sql);
            //  Console.WriteLine(Manager.GetLegalString("inert into student values ('123','234')"));

            Console.Read();
        }
    }
}
