using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dalCore;
using System.Data;

namespace DCore
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLConfigurator.InitializeConnection(@"Integrated Security=True;Initial Catalog=MISCReportSingle;Data Source=DEVSYSTEM\SQLEXPRESS");
            Console.WriteLine("ready");

            Console.ReadLine();
        }
    }
}
