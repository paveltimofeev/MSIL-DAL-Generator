using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dalCore
{
    public interface ISQLBook
    {
        //SQLBookBase Load(int id);
        void Update();
        void Delete();
        void Insert();
    }
}
