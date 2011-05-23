using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dalCoreSE
{
    public interface ISQLBook
    {
        //SQLBookBase Load(int id);
        void Update();
        void Delete();
        void Insert();
    }
}
