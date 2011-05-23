using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace DProcessorSE
{
    public class SPProcessor
    {
        public SqlCommand cmd = new SqlCommand();
        public DataTable ds;
        public SqlDataAdapter da;

        public SPProcessor()
        {
            cmd = new SqlCommand();
            cmd.Connection = dalCoreSE.SQLConfigurator.GetConnection();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
        }

        public SPProcessor(string SPName)
        {
            cmd = new SqlCommand(SPName, new SqlConnection(""));
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
        }

        #region AddParameter methods
        int iParameter = 0;
        public void AddParameter(int p, string parameterName)
        {
            AddParameter(parameterName, typeof(int), p);
        }

        public void AddParameter(float p, string parameterName)
        {
            AddParameter(parameterName, typeof(float), p);
        }

        public void AddParameter(decimal p, string parameterName)
        {
            AddParameter(parameterName, typeof(decimal), p);
        }

        public void AddParameter(string p, string parameterName)
        {
            AddParameter(parameterName, typeof(string), p);
        }

        public void AddParameter(object p, string parameterName)
        {
            AddParameter(parameterName, typeof(object), p);
        }

        public void AddParameter(DateTime p, string parameterName)
        {
            AddParameter(parameterName, typeof(DateTime), p);
        }

        public void AddParameter(byte[] p, string parameterName)
        {
            AddParameter(parameterName, typeof(byte[]), p);
        }
        #endregion

        public void SetProcedureName(string SPName)
        {
            cmd.CommandText = SPName;
        }

        protected void AddParameter(string name, Type t, object value)
        {
            if (name != null && name != string.Empty && name != "")
            {
                if (name[0] != '@')
                    name = "@" + name;

                SqlParameter parameter = new SqlParameter(name, t);
                parameter.Value = value;
                if (!cmd.Parameters.Contains(parameter))
                {
                    cmd.Parameters.Add(parameter);
                    iParameter++;
                }
                else
                {
                    cmd.Parameters[parameter.ParameterName] = parameter; //?
                }
            }
            else
            {
                throw new ArgumentNullException("name");
            }
        }

        public DataTable Call()
        {
            ds = new DataTable();
            da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            return ds;
        }
    }

    //public class SPConcrete
    //{
    //    static SPProcessor processor = new SPProcessor();

    //    public static DataTable Call(int id, string name)
    //    {
    //        processor = new SPProcessor();
    //        processor.SetProcedureName("sp_Select");
    //        processor.AddParameter(id, "id");
    //        processor.AddParameter(name, "name");

    //        return processor.Call();
    //    }
    //}
}