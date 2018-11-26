using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Task3
{
   static class FileHelper
    {
        public static bool CreateFile(string name,FileType FileType)
        {
            if (!File.Exists(name + (FileType == FileType.DataBase ? ".database" : FileType == FileType.Table ? ".table" : "")))
            {
                File.Create(name + (FileType == FileType.DataBase ? ".database" : FileType == FileType.Table ? ".table" : "")).Close();
                return true;
            }
            return false;
        }
        public static bool Delete(string name,FileType FileType)
        {
            if (isExists(name, FileType))
            {
                File.Delete(name + (FileType == FileType.DataBase ? ".database" : FileType == FileType.Table ? ".table" : ""));
                return true;
            }
            return false;
        }
        public static bool isExists(string name,FileType FileType)
        {

            return File.Exists(name + (FileType == FileType.DataBase ? ".database" : FileType == FileType.Table ? ".table" : ""));
        }
        public static bool SetFileContenct(string name,FileType FileType, string contenct,FileMode mode=FileMode.OpenOrCreate)
        {
            using (FileStream fi = new FileStream(name + (FileType == FileType.DataBase ? ".database" : FileType==FileType.Table ? ".table" : ""), mode))
            {
                   
                using (StreamWriter sw = new StreamWriter(fi))
                {
                    sw.WriteLine(contenct);
                    sw.Flush();
                    sw.Close();
                }    
            }
            return true;
        }
        public static string GetFileContenct(string name,FileType FileType)
        {
            if (!isExists(name, FileType))
                return "";
            using (StreamReader sr=new StreamReader(name + (FileType == FileType.DataBase ? ".database" : FileType == FileType.Table ? ".table" : "")))
            {  
                return sr.ReadToEnd();
            }
        }
    }
}
