using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace dalCoreSE
{
    public class DalTypeConverter
    {
        private static Dictionary<SqlDbType, Type> sqt_types = new Dictionary<SqlDbType, Type>();
        private static Dictionary<SqlDbType, string> sql_sql = new Dictionary<SqlDbType, string>();
        private static Dictionary<string, SqlDbType> str_sql = new Dictionary<string, SqlDbType>();

        static DalTypeConverter()
        {
            sqt_types.Add(SqlDbType.BigInt, typeof(Int64));
            sqt_types.Add(SqlDbType.Int, typeof(Int32));
            sqt_types.Add(SqlDbType.SmallInt, typeof(Int16));
            sqt_types.Add(SqlDbType.Binary, typeof(byte[]));
            sqt_types.Add(SqlDbType.Bit, typeof(byte));
            sqt_types.Add(SqlDbType.Char, typeof(String));
            sqt_types.Add(SqlDbType.DateTime, typeof(DateTime));
            sqt_types.Add(SqlDbType.Date, typeof(DateTime));
            sqt_types.Add(SqlDbType.Decimal, typeof(Decimal));
            sqt_types.Add(SqlDbType.Float, typeof(float));
            sqt_types.Add(SqlDbType.Image, typeof(byte[]));
            sqt_types.Add(SqlDbType.Money, typeof(Decimal));
            sqt_types.Add(SqlDbType.NChar, typeof(String));
            sqt_types.Add(SqlDbType.NText, typeof(String));
            sqt_types.Add(SqlDbType.NVarChar, typeof(String));
            sqt_types.Add(SqlDbType.Real, typeof(Single));
            sqt_types.Add(SqlDbType.SmallDateTime, typeof(DateTime));
            sqt_types.Add(SqlDbType.SmallMoney, typeof(Decimal));
            sqt_types.Add(SqlDbType.Structured, typeof(object));
            sqt_types.Add(SqlDbType.Text, typeof(String));
            sqt_types.Add(SqlDbType.Time, typeof(DateTime));
            sqt_types.Add(SqlDbType.Timestamp, typeof(byte[]));
            sqt_types.Add(SqlDbType.TinyInt, typeof(byte));
            sqt_types.Add(SqlDbType.UniqueIdentifier, typeof(Guid));
            sqt_types.Add(SqlDbType.VarBinary, typeof(byte[]));
            sqt_types.Add(SqlDbType.VarChar, typeof(String));
            sqt_types.Add(SqlDbType.Variant, typeof(Object));
            sqt_types.Add(SqlDbType.Xml, typeof(String));

            sql_sql.Add(SqlDbType.BigInt, "BigInt");
            sql_sql.Add(SqlDbType.Binary, "binary");
            sql_sql.Add(SqlDbType.Bit, "bit");
            sql_sql.Add(SqlDbType.Char, "Char");
            sql_sql.Add(SqlDbType.Date, "datetime");
            sql_sql.Add(SqlDbType.DateTime, "datetime");
            sql_sql.Add(SqlDbType.DateTime2, "datetime");
            sql_sql.Add(SqlDbType.DateTimeOffset, "");
            sql_sql.Add(SqlDbType.Decimal, "decimal");
            sql_sql.Add(SqlDbType.Float, "float");
            sql_sql.Add(SqlDbType.Image, "image");
            sql_sql.Add(SqlDbType.Int, "int");
            sql_sql.Add(SqlDbType.Money, "money");
            sql_sql.Add(SqlDbType.NChar, "nchar");
            sql_sql.Add(SqlDbType.NText, "ntext");
            sql_sql.Add(SqlDbType.NVarChar, "nvarchar");
            sql_sql.Add(SqlDbType.Real, "real");
            sql_sql.Add(SqlDbType.SmallDateTime, "smalldatetime");
            sql_sql.Add(SqlDbType.SmallInt, "smallint");
            sql_sql.Add(SqlDbType.SmallMoney, "smallmoney");
            sql_sql.Add(SqlDbType.Structured, "");
            sql_sql.Add(SqlDbType.Text, "text");
            sql_sql.Add(SqlDbType.Time, "time");
            sql_sql.Add(SqlDbType.Timestamp, "timestamp");
            sql_sql.Add(SqlDbType.TinyInt, "TinyInt");
            sql_sql.Add(SqlDbType.Udt, "");
            sql_sql.Add(SqlDbType.UniqueIdentifier, "UniqueIdentifier");
            sql_sql.Add(SqlDbType.VarBinary, "VarBinary");
            sql_sql.Add(SqlDbType.VarChar, "VarChar");
            sql_sql.Add(SqlDbType.Variant, "VarChar");
            sql_sql.Add(SqlDbType.Xml, "Xml");

            str_sql.Add("bigint", SqlDbType.BigInt);
            str_sql.Add("binary", SqlDbType.Binary);
            str_sql.Add("bit", SqlDbType.Bit);
            str_sql.Add("char", SqlDbType.Char);
            str_sql.Add("date", SqlDbType.Date);
            str_sql.Add("datetime", SqlDbType.DateTime);
            str_sql.Add("decimal", SqlDbType.Decimal);
            str_sql.Add("float", SqlDbType.Float);
            str_sql.Add("image", SqlDbType.Image);
            str_sql.Add("int", SqlDbType.Int);
            str_sql.Add("money", SqlDbType.Money);
            str_sql.Add("nchar", SqlDbType.NChar);
            str_sql.Add("ntext", SqlDbType.NText);
            str_sql.Add("nvarchar", SqlDbType.NVarChar);
            str_sql.Add("real", SqlDbType.Real);
            str_sql.Add("smalldatetime", SqlDbType.SmallDateTime);
            str_sql.Add("smallint", SqlDbType.SmallInt);
            str_sql.Add("smallmoney", SqlDbType.SmallMoney);
            str_sql.Add("text", SqlDbType.Text);
            str_sql.Add("time", SqlDbType.Time);
            str_sql.Add("timestamp", SqlDbType.Timestamp);
            str_sql.Add("tinyint", SqlDbType.TinyInt);
            str_sql.Add("uniqueidentifier", SqlDbType.UniqueIdentifier);
            str_sql.Add("varbinary", SqlDbType.VarBinary);
            str_sql.Add("varchar", SqlDbType.VarChar);
            str_sql.Add("variant", SqlDbType.Variant);
            str_sql.Add("xml", SqlDbType.Xml);
        }

        public static SqlDbType ToSql(Type t)
        {
            //todo:
            foreach (KeyValuePair<SqlDbType, Type> item in sqt_types)
            {
                if (item.Value.Equals(t))
                    return item.Key;
            }

            return SqlDbType.NVarChar;
        }

        public static Type ToCLR(SqlDbType t)
        {
            return sqt_types[t];
        }

        public static string ToTSQL(Type t)
        {
            return sql_sql[ToSql(t)];
        }

        public static string ToTSQL(SqlDbType t)
        {
            return sql_sql[t];
        }

        public static T ToGenegicType<T>(object value)
        {
            return (T)value;
        }

        public static object ToGenegicType(Type type, IConvertible value)
        {
            if (type.Equals(typeof(DBNull)) | value.ToString() == "")
            {
                if (type.Equals(typeof(int)) | type.Equals(typeof(decimal)) | type.Equals(typeof(float)))
                    return 0;
                else
                    return "";
            }
            else
            {
                return value.ToType(type, null);
            }
        }

        public static Type ToCLR(string t)
        {
            return ToCLR(ToSql(t));
        }

        public static SqlDbType ToSql(string t)
        {
            t = t.Trim();
            t = t.ToLowerInvariant();

            if (str_sql.ContainsKey(t))
                return str_sql[t];
            else
                return SqlDbType.NVarChar;
        }
    }
}
