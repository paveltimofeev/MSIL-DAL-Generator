using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace DPocessor
{
    public class EntityFactory
    {
        internal static EntityData[] GetEntities(string connStr)
        {
            SqlConnection conn = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand("select TABLE_CATALOG, TABLE_NAME from INFORMATION_SCHEMA.TABLES; select TABLE_CATALOG, TABLE_NAME, COLUMN_NAME, DATA_TYPE from INFORMATION_SCHEMA.COLUMNS", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            Dictionary<string, EntityData> entities = new Dictionary<string, EntityData>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string className = dr["TABLE_NAME"].ToString();
                className = className.Replace("Book_", "");
                entities.Add(className, new EntityData(className));
            }

            foreach (DataRow dr in ds.Tables[1].Rows)
            {
                EntityType t = new EntityType();
                t.Name = dr["COLUMN_NAME"].ToString();
                t.SetSqlDbType(dr["DATA_TYPE"].ToString());
                t.SetType(dr["DATA_TYPE"].ToString());
                t.IsKey = (t.Name.ToUpper() == "ID") ? true : false;
                entities[dr["TABLE_NAME"].ToString().Replace("Book_", "")].F.Add(t);
            }

            EntityData[] result = new EntityData[entities.Count];
            entities.Values.CopyTo(result, 0);

            return result;
        }
    }
}
