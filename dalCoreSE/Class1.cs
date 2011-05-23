using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dalCoreSE
{
    public class Hie
    {
        protected Hie()
        {
            ;
        }
    }

    public class Inh : Hie
    {
        public Inh()
        {
            ;
        }

        private Inh(int id)
        {
            ;
        }

        public static Inh Get(int id)
        {
            return new Inh(id);
        }

        public static Inh[] GetList()
        {
            return new Inh[] { };
        }
    }
}
