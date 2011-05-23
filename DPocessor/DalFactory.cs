using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.CodeDom.Compiler;
using System.Reflection.Emit;
using System.Diagnostics;
using System.Reflection;

namespace DPocessor
{
    public class DalFactory
    {
        public static string CreateEntityClass(EntityData entity, string template)
        {
            StringBuilder resultCode = new StringBuilder("///©pavel timofeev");
            Regex r = new Regex("<%CN%>");
            resultCode.Append(r.Replace(template, entity.CN));

            resultCode = resultCode.Replace("<%FF%>", entity.F.GetFF());
            resultCode = resultCode.Replace("<%Ff%>", entity.F.GetFf());
            resultCode = resultCode.Replace("<%FP%>", entity.F.GetFP());
            resultCode = resultCode.Replace("<%FS%>", entity.F.GetFS());

            StringReader s = new StringReader(resultCode.ToString());
            StringWriter w = new StringWriter();
            string stroke = s.ReadLine();
            while (stroke != null)
            {
                if (!stroke.StartsWith("//"))
                    w.WriteLine(ReplaceTypes(stroke, entity));
                stroke = s.ReadLine();
            }

            string str = w.ToString();

            return str;
        }

        private static string ReplaceTypes(string stroke, EntityData entity)
        {
            if (stroke == null)
                return null;

            string result = stroke.TrimStart(' ');
            if (result.StartsWith(">>"))
            {
                result = result.Remove(0, 2);

                string temp = "";

                for (int i = 0; i < entity.F.Count; i++)
                {
                    if (!entity.F[i].IsKey)
                        temp += entity.F[i].Format(result) + "\r\n";
                }
                result = temp;

            }
            return result;
        }

        public static string CompileAssembly(string classesCode, string assemblyName)
        {
            string result = "ok.";
            string codeFile = @"d:\code.cs";
            string coreDll = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "dalCore.dll");
            
            if (!File.Exists(coreDll))
                return "Ошибка компиляции: Не найден dalCore.dll";

            codeFile = Path.ChangeExtension(codeFile, ".cs");
            File.WriteAllText(codeFile, classesCode);
            
            try
            {
                string args = string.Format("/target:library /reference:\"{2}\" /out:\"{0}\" \"{1}\"", assemblyName, codeFile, coreDll);
                Process.Start(@"C:\WINDOWS\Microsoft.NET\Framework\v3.5\csc.exe", args);
            }
            catch (Exception ex)
            {
                result ="Ошибка компиляции: " + ex.Message +"\r\n"+ assemblyName + "\r\n" + codeFile;
            }
            
            return result;
        }
    }
}
