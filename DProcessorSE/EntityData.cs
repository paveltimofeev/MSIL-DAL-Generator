using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DProcessorSE
{
    public class EntityData
    {
        public EntityData()
            : this("", "")
        {
            ;
        }

        public EntityData(string className)
            : this("", className)
        {
            ;
        }

        public EntityData(string classNamespace, string className)
        {
            Namespace = classNamespace;
            ClassName = className;
            Fields = new EntityTypes();
            IsStoredProcedure = false;
        }

        public bool IsStoredProcedure { get; set; }
        public string Namespace { get; set; }
        public string ClassName { get; set; }
        public EntityTypes Fields { get; set; }
        public string DatabaseTableName { get; set; }
    }
}
