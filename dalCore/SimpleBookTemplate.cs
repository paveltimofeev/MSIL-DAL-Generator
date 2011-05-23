using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace dalCore
{
    public class _Test1_: SQLBookBase
    {
        public object _Field_ { get; set; }

        public _Test1_()
        {
            Initialize();
        }

        private _Test1_(int Id)
        {
            this.Id = Id;
            Initialize();
            this.Load(Id);
        }

        protected override void Initialize()
        {
            cmdSelect = new SqlCommand("SELECT Id, Field FROM Book_Test1 WHERE Id = @Id");
            cmdSelectList = new SqlCommand("SELECT Id, Field FROM Book_Test1");
            cmdDelete = new SqlCommand("DELETE FROM Book_Test1 WHERE Id = @Id");
            cmdInsert = new SqlCommand("INSERT INTO Book_Test1(Field) VALUES(@Field)");
            cmdUpdate = new SqlCommand("UPDATE Book_Test1 SET Field = @Field WHERE Id = @Id");

            cmdInsert.Parameters.Add("@Field", SqlDbType.NText);
            cmdUpdate.Parameters.Add("@Field", SqlDbType.NText);

            base.Initialize();
        }

        protected override void Preload()
        {
            base.Preload();
        }

        protected override void Presave()
        {
            cmdInsert.Parameters["@Field"].Value = _Field_;
            cmdUpdate.Parameters["@Field"].Value = _Field_;

            base.Presave();
        }

        protected override void Postload(DataTable data, int row)
        {
            Id = Convert.ToInt32(data.Rows[row]["Id"].ToString());
            _Field_ = data.Rows[row]["Field"].ToString();

            base.Postload(data, row);
        }

        public static _Test1_ Get(int Id)
        {
            _Test1_ temp = new _Test1_(Id);
            return temp;
        }

        public static _Test1_[] LoadList()
        {
            _Test1_ t = new _Test1_();
            DataTable tempTable = t.LoadDataList();
            _Test1_[] result = new _Test1_[tempTable.Rows.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new _Test1_();
                result[i].Postload(tempTable, i);
            }

            return result;
        }
    }
}