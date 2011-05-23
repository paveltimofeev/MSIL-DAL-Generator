using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DPocessor
{
    public class EntityType
    {
        public string Name { get; set; }
        public string SqlTypeName { get; set; }
        public string TypeName { get; set; }

        private bool isKey = false;

        public bool IsKey
        {
            get { return isKey; }
            set { isKey = value; }
        }

        public void SetType(string SqlType)
        {
            string sql = SqlType.ToUpper();

            if (sql == "NVARCHAR" | sql == "NCHAR" | sql == "CHAR")
            {
                TypeName = "String";
            }
            else if (sql == "INT" | sql == "BIT" | sql == "SMALLINT")
                TypeName = "Int32";
            else if (sql == "BOOLEAN")
                TypeName = "Boolean";
            else if (sql == "FLOAT" | sql == "DECIMAL" | sql == "MONEY")
                TypeName = "Decimal";
            else
                TypeName = "String";
        }

        public void SetSqlDbType(string SqlType)
        {
            string sql = SqlType.ToUpper();

            if (sql == "NVARCHAR")
                SqlTypeName = "NVarChar";
            else if (sql == "NCHAR")
                SqlTypeName = "NChar";
            else if (sql == "CHAR")
                SqlTypeName = "Char";
            else if (sql == "INT")
                SqlTypeName = "Int";
            else if (sql == "BIT")
                SqlTypeName = "Bit";
            else if (sql == "SMALLINT")
                SqlTypeName = "SmallInt";
            else if (sql == "BOOLEAN")
                SqlTypeName = "Bit";
            else if (sql == "FLOAT")
                SqlTypeName = "Float";
            else if (sql == "DECIMAL")
                SqlTypeName = "Decimal";
            else if (sql == "MONEY")
                SqlTypeName = "Money";
            else if (sql == "DATETIME")
                SqlTypeName = "DateTime";
            else
                SqlTypeName = "NVarChar";

        }

        public override string ToString()
        {
            return this.Name;
        }

        public string Format(string format)
        {
            return format.Replace("<%F%>", Name).Replace("<%FT%>", SqlTypeName).Replace("<%FTT%>", TypeName);
        }
    }
}
