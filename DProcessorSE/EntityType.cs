using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DProcessorSE
{
    public class EntityType
    {
        public string Name { get; set; }
        public SqlDbType SqlTypeName { get; set; }
        public Type TypeName { get; set; }

        private bool isKey = false;

        public bool IsKey
        {
            get { return isKey; }
            set { isKey = value; }
        }
    }
}
