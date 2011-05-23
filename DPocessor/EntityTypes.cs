using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DPocessor
{
    public class EntityTypes
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

        public string GetFF()
        {
            string temp = "";
            for (int i = 0; i < items.Count; i++)
            {
                temp += items[i].Name + ", ";
            }
            temp = temp.Remove(temp.Length - 2);
            return temp;
        }

        public string GetFf()
        {
            string temp = "";
            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].Name.ToUpper().Equals("ID"))
                    temp += items[i].Name + ", ";
            }
            temp = temp.Remove(temp.Length - 2);

            return temp;
        }

        public string GetFP()
        {
            string temp = "";
            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].Name.ToUpper().Equals("ID"))
                    temp += "@" + items[i].Name + ", ";
            }
            temp = temp.Remove(temp.Length - 2);

            return temp;
        }

        public string GetFS()
        {
            string temp = "";
            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].Name.ToUpper().Equals("ID"))
                    temp += string.Format("{0} = @{0}, ", items[i].Name);
            }
            temp = temp.Remove(temp.Length - 2);

            return temp;
        }
    }
}
