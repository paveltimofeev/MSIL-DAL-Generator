using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Collections;

namespace dalCoreSE
{
    public class SQLBookSEBase : ISQLBook
    {
        #region Fields & Properties

        protected SqlConnection conn = SQLConfigurator.GetConnection();
        protected SqlCommand cmdSelect = new SqlCommand();
        protected SqlCommand cmdSelectList = new SqlCommand();
        protected SqlCommand cmdInsert = new SqlCommand();
        protected SqlCommand cmdUpdate = new SqlCommand();
        protected SqlCommand cmdDelete = new SqlCommand();

        public int Id { get; set; }

        private string typeName = "";
        private Dictionary<string, Type> properties = new Dictionary<string, Type>();

        #endregion

        #region Events

        public delegate void EventHandler(object sender, EventArgs e);

        public event EventHandler Initializing;
        public event EventHandler Loading;
        public event EventHandler Loaded;

        protected void OnInitializing()
        {
            if (this.Initializing != null)
                this.Initializing(this, null);
        }

        protected void OnLoading()
        {
            if (this.Loading != null)
                this.Loading(this, null);
        }

        protected void OnLoaded()
        {
            if (this.Loaded != null)
                this.Loaded(this, null);
        }


        #endregion

        public SQLBookSEBase()
        {
            cmdSelect.Connection = conn;
            cmdSelectList.Connection = conn;
            cmdInsert.Connection = conn;
            cmdUpdate.Connection = conn;
            cmdDelete.Connection = conn;

            Initialize();
        }

        /// <summary>
        /// Первичная инициализация объекта
        /// </summary>
        protected virtual void Initialize()
        {
            Type t = this.GetType();
            typeName = t.Name;
            foreach (PropertyInfo p in t.GetProperties())
            {
                if (Attribute.IsDefined(p, typeof(DalAttribute)))
                    properties.Add(p.Name, p.PropertyType);
            }
            /////////////////////////////////////////////////////////////

            string parametersSelect = "";
            string parametersSelectList = "";
            string parametersInsert = "";
            string parametersInsertValues = "";
            string parametersUpdate = "";

            foreach (KeyValuePair<string, Type> propr in properties)
            {
                parametersSelect += string.Format("{0}, ", propr.Key);
                parametersSelectList += string.Format("{0}, ", propr.Key);
                parametersInsert += string.Format("{0}, ", propr.Key);
                parametersInsertValues += string.Format("@{0}, ", propr.Key);
                parametersUpdate += string.Format("{0} = @{0}, ", propr.Key);
            }

            parametersSelect = parametersSelect.Remove(parametersSelect.Length - 2, 2);
            parametersSelectList = parametersSelectList.Remove(parametersSelectList.Length - 2, 2);
            parametersInsert = parametersInsert.Remove(parametersInsert.Length - 2, 2);
            parametersInsertValues = parametersInsertValues.Remove(parametersInsertValues.Length - 2, 2);
            parametersUpdate = parametersUpdate.Remove(parametersUpdate.Length - 2, 2);

            cmdSelect = new SqlCommand(string.Format("SELECT ID, {0} FROM {1} WHERE Id = @Id", parametersSelect, typeName));
            cmdSelectList = new SqlCommand(string.Format("SELECT ID, {0} FROM {1}", parametersSelectList, typeName));
            cmdDelete = new SqlCommand(string.Format("DELETE FROM {0} WHERE Id = @Id", typeName));
            cmdInsert = new SqlCommand(string.Format("INSERT INTO {0}({1}) VALUES({2})", typeName, parametersInsert, parametersInsertValues));
            cmdUpdate = new SqlCommand(string.Format("UPDATE {0} SET {1} WHERE Id = @Id", typeName, parametersUpdate));

            foreach (KeyValuePair<string, Type> propr in properties)
            {
                cmdInsert.Parameters.Add("@" + propr.Key, DalTypeConverter.ToSql(propr.Value));
                cmdUpdate.Parameters.Add("@" + propr.Key, DalTypeConverter.ToSql(propr.Value));
            }

            /////////////////////////////////////////////////////////////

            cmdSelect.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
            cmdUpdate.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
            cmdDelete.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
        }

        #region  Работа с базой данных

        /// <summary>
        /// Выполнение произвольного запроса
        /// </summary>
        /// <param name="cmd"></param>
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

        /// <summary>
        /// Заполнение DataTable результатом выполнения произвольного запроса
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private DataTable FillDataset(SqlCommand cmd)
        {
            DataTable result = new DataTable();
            cmd.Connection = SQLConfigurator.GetConnection();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(result);
            return result;
        }

        #endregion

        #region Предварительная обработка данных перед сохранением или загрузкой

        protected virtual void Presave()
        {
            Type t = this.GetType();
            cmdUpdate.Parameters["@Id"].Value = Id;
            foreach (KeyValuePair<string, Type> propr in properties)
            {
                cmdInsert.Parameters[string.Format("@{0}", propr.Key)].Value = t.GetProperty(propr.Key).GetValue(this, null);
                cmdUpdate.Parameters[string.Format("@{0}", propr.Key)].Value = t.GetProperty(propr.Key).GetValue(this, null);
            }
        }

        protected virtual void Preload() { ;}

        #endregion

        #region Работа с экземплярами
       
        /// <summary>
        /// Возвращает массив элементов
        /// </summary>
        /// <returns></returns>
        public ArrayList LoadLists()
        {
            ArrayList result = new ArrayList();

            Type t = this.GetType();
            object item = t.InvokeMember(
                 null,
                 BindingFlags.CreateInstance,
                 null,
                 this,
                 null);

            MethodInfo caller = t.GetMethod("LoadDataList");
            DataTable tempTable = (DataTable)caller.Invoke(item, null);

            if (tempTable != null && tempTable.Rows != null)
                for (int i = 0; i < tempTable.Rows.Count; i++)
                {
                    object tempItem = t.InvokeMember(
                     null,
                     BindingFlags.CreateInstance,
                     null,
                     this,
                     null);

                    t.GetMethod("Postload").Invoke(tempItem, new object[] { tempTable, i });

                    result.Add(tempItem);
                }

            return result;
        }

        [Obsolete("Дла получения экземпляра класса по Id записи используйне статический метоа Get необходимого класса.")]
        public SQLBookSEBase Load(int id)
        {
            Postload(LoadDataItem(id), 0);
            return this;
        }        
        
        protected virtual DataTable LoadDataItem(int id)
        {
            cmdSelect.Parameters["@Id"].Value = id;
            return FillDataset(cmdSelect);
        }

        #endregion
        
        //hack: protected -> public - сможем достать метод рефлектором, но уменьшим инкапсуляцию.
        public virtual void Postload(DataTable data, int row)
        {
            if (data.Rows.Count >= row + 1)
            {
                Id = DalTypeConverter.ToGenegicType<Int32>(data.Rows[row]["Id"]);

                foreach (KeyValuePair<string, Type> propr in properties)
                {
                    Type t = this.GetType();
                    object value = DalTypeConverter.ToGenegicType(propr.Value, data.Rows[row][propr.Key].ToString());
                    t.GetProperty(propr.Key).SetValue
                        (
                        this,
                        value,
                        null);
                }
            }
            else
                throw new Exception("Postload. Item not found.");
        }

        //hack: protected -> public - сможем достать метод рефлектором, но уменьшим инкапсуляцию, с другой стороны предоставим доступ к данным объекта в виде датасета.
        public virtual DataTable LoadDataTable()
        {
            return FillDataset(cmdSelectList);
        }

        #region ISQLBook Members

        /// <summary>
        /// Обновление информации об элементе
        /// </summary>
        public void Update()
        {
            Presave();
            ExecuteNonQuery(cmdUpdate);
        }

        /// <summary>
        /// Удаление текущей записи
        /// </summary>
        public void Delete()
        {
            Presave();
            ExecuteNonQuery(cmdDelete);
        }

        /// <summary>
        /// Добавление новой записи
        /// </summary>
        public void Insert()
        {
            Presave();
            ExecuteNonQuery(cmdInsert);
        }

        #endregion

    }
}
