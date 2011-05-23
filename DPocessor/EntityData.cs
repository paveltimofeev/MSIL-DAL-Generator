using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DPocessor
{
    public class EntityData
    {
        public EntityData()
            :this("")
        {
            ;
        }

        public EntityData(string name)
        {
            CN = name;
            F = new EntityTypes();
        }
        public string CN { get; set; }
        public EntityTypes F { get; set; }
    }
}
