using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace dalCore
{
    public class SQLBookBase : ISQLBook//, ISQLData
    {
        public int Id { get; set; }

        protected SqlConnection conn = SQLConfigurator.GetConnection();
        protected SqlCommand cmdSelect = new SqlCommand();
        protected SqlCommand cmdSelectList = new SqlCommand();
        protected SqlCommand cmdInsert = new SqlCommand();
        protected SqlCommand cmdUpdate = new SqlCommand();
        protected SqlCommand cmdDelete = new SqlCommand();

        public SQLBookBase()
        {
            cmdSelect.Connection = conn;
            cmdSelectList.Connection = conn;
            cmdInsert.Connection = conn;
            cmdUpdate.Connection = conn;
            cmdDelete.Connection = conn;

            Initialize();
        }
        
        private void ExecuteNonQuery(SqlCommand cmd)
        {
            cmd.Connection = SQLConfigurator.GetConnection();
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                    conn.Close();
            }
        }

        private DataTable FillDataset(SqlCommand cmd)
        {
            DataTable result = new DataTable();
            cmd.Connection = SQLConfigurator.GetConnection();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(result);
            return result;
        }

        protected virtual void Initialize()
        {
            cmdSelect.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
            cmdUpdate.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
            cmdDelete.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
        }

        protected virtual void Presave()
        {
            ;
        }

        protected virtual void Preload()
        {
            ;
        }

        protected virtual void Postload(DataTable data, int row)
        {
            ;
        }

        #region ISQLBook Members

        protected virtual SQLBookBase Load(int id)
        {
            Postload(LoadDataItem(id), 0);
            return this;
        }

        public void Update()
        {
            Presave();
            ExecuteNonQuery(cmdUpdate);
        }

        public void Delete()
        {
            Presave();
            ExecuteNonQuery(cmdDelete);
        }

        public void Insert()
        {
            Presave();
            ExecuteNonQuery(cmdInsert);
        }

        #endregion

        #region ISQLData Members

        protected virtual DataTable LoadDataList()
        {
            return FillDataset(cmdSelectList);
        }

        protected virtual DataTable LoadDataItem(int id)
        {
            cmdSelect.Parameters["@Id"].Value = id;
            return FillDataset(cmdSelect);
        }

        #endregion
    }
}
