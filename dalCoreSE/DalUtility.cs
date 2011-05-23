using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace dalCoreSE
{
    public class DalUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetList<T>()
            where T : SQLBookSEBase
        {
            T[] result = new T[] { };
            Type t = typeof(T);

            if (Attribute.IsDefined(t, typeof(DalClassAttribute)))
            {
                DalClassAttribute at = null;

                object[] attr = t.GetCustomAttributes(typeof(DalClassAttribute), true);

                if (attr != null && attr.Length > 0)
                {
                    at = attr[0] as DalClassAttribute;
                    if (at != null)
                    {
                        string tableName = at.DatabaseTableName;
                        if (tableName != null && tableName != string.Empty && tableName != "")
                        {
                            SqlCommand cmd = new SqlCommand(string.Format("SELECT * FROM {0}", tableName), SQLConfigurator.GetConnection());
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            if (dt != null && dt.Rows.Count > 0)
                            {
                                result = new T[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    result[i] = (T)typeof(T).GetConstructor(Type.EmptyTypes).Invoke(null);
                                    result[i].Postload(dt, i);
                                }
                            }
                            else
                            {//return null data
                                result = null;
                            }
                        }
                        else
                        {//a name of the table is null or not specified
                            result = null;
                        }
                    }
                    else
                    {//attribute is not a DalClassAttribute
                        result = null;
                    }
                }
                else
                {//class havn't DalClassAttribute
                    result = null;
                }
            }

            return result;
        }
    }
}
