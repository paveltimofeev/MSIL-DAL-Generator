using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace dalCore
{
    public interface ISQLData
    {
        DataTable LoadDataList();
        DataTable LoadDataItem(int id);
    }
}
