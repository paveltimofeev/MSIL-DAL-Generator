using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace DPocessor
{
    class Program
    {
        static void Main(string[] args)
        {
            if ((args.Length == 1 && args[0].Equals("/?"))|args.Length == 0)
            {
                Console.WriteLine("Sintax:");
                Console.WriteLine("\tDProcessor [CONNECTION STRING] [LIBRARY/FILE NAME] /f");
                Console.WriteLine("\t/f - write code to file;");
                Console.WriteLine("");
                Console.WriteLine("Эта программа создаёт сборку с классами соответствующими таблицам справочкиков в базе данных. Каждый класс соответствует таблице имеющей имеющей префикс Book_, и содержит набор методов для работы с данным справочником.");
                Console.WriteLine("Для генерации сборки необходимо передать программе строку подключения и имя сборки которая будет создана, если указать ключ /f то компиляция выполнена не будет вместо этого будет сгенерирован файл с кодом классов сборки.");
                Console.WriteLine("Для корректной компиляции кода необходимо добавить переменные среды PATH или воспользоваться командной строкой VisualStudio.");
                Console.ReadLine();
            }
            if (args.Length > 1)
            {

                EntityData[] entities = EntityFactory.GetEntities(args[0]);

                string template = File.ReadAllText("Template.txt");
                StringBuilder gc = new StringBuilder(string.Format("//© dalCore, {0}\r\n\r\n", DateTime.Now.Year));
                gc.AppendLine("using System;");
                gc.AppendLine("using System.Collections.Generic;");
                gc.AppendLine("using System.Linq;");
                gc.AppendLine("using System.Text;");
                gc.AppendLine("using System.Data.SqlClient;");
                gc.AppendLine("using System.Data;");

                for (int i = 0; i < entities.Length; i++)
                {
                    gc.Append(DalFactory.CreateEntityClass(entities[i], template));
                    gc.AppendLine();
                }

                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                if (args.Length >= 3 && args[2] == "/f")
                    File.WriteAllText(Path.Combine(dir, args[1]), gc.ToString());
                else
                    Console.WriteLine(DalFactory.CompileAssembly(gc.ToString(), Path.Combine(dir, args[1])));
            }
        }
    }
}
