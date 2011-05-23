using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DProcessorSE
{
    public class EntityTypes : IEnumerable
    {
        List<EntityType> items = new List<EntityType>();

        public EntityType this[int index]
        {
            get { return items[index]; }
            set { items[index] = value; }
        }

        public void Add(EntityType item)
        {
            items.Add(item);
        }

        public void Remove(int index)
        {
            items.RemoveAt(index);
        }

        public int Count
        {
            get { return items.Count; }
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion
    }
}
