using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dalCoreSE
{
    /// <summary>
    /// Класс для выполнения асинхронных операций с базой данных.
    /// </summary>
    public class BackgroundTransaction
    {
        public BackgroundTransaction(SQLBookSEBase bookClass)
        {
            ;
        }

        public void BeginLoad(int Id)
        {
            ;
        }
        public void BeginLoadList()
        {
            ;
        }
        public void EndLoad()
        {
            ;
        }
        public void EndLoadList()
        {
            ;
        }
        public void BeginInsert()
        {
            ;
        }
        public void EndInsert()
        {
            ;
        }
        public void BeginUpdate()
        {
            ;
        }
        public void EndUpdate()
        {
            ;
        }
        public void BeginDelete()
        {
            ;
        }
        public void EndDelete()
        {
            ;
        }
        public void Cancel()
        {
            ;
        }
        public event EventHandler LoadingBegin;
        public event EventHandler LoadingComplited;

        public OperationState CurrentState
        {
            get { return OperationState.NULL;}
        }

        public enum OperationState { NULL };
    }
}
