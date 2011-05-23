using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace dalCoreSE
{
    public class s_User_Rule : SQLBookSEBase
    {
        [DalAttribute]
        public Int32 UserNameID { get; set; }
        [DalAttribute]
        public Int32 RuleNameID { get; set; }
        [DalAttribute]
        public Int32 RegionID { get; set; }
        [DalAttribute]
        public String Rules { get; set; }

        public s_User_Rule() { ;}

        //.method private hidebysig specialname rtspecialname 
        //instance void  .ctor(int32 Id) cil managed
        //{
        //// Code size       26 (0x1a)
        //.maxstack  8
        //IL_0000:  ldarg.0
        //IL_0001:  call       instance void dalCoreSE.SQLBookSEBase::.ctor()
        //IL_0006:  nop
        //IL_0007:  nop
        //IL_0008:  ldarg.0
        //IL_0009:  ldarg.1
        //IL_000a:  call       instance void dalCoreSE.SQLBookSEBase::set_Id(int32)
        //IL_000f:  nop
        //IL_0010:  ldarg.0
        //IL_0011:  ldarg.1
        //IL_0012:  call       instance class dalCoreSE.SQLBookSEBase dalCoreSE.SQLBookSEBase::Load(int32)
        //IL_0017:  pop
        //IL_0018:  nop
        //IL_0019:  ret
        //} // end of method s_User_Rule::.ctor
        private s_User_Rule(int Id)
        {
            this.Id = Id;
            this.Load(Id);
        }

        //.method public hidebysig static class dalCoreSE.s_User_Rule 
        //Get(int32 id) cil managed
        //{
        //// Code size       12 (0xc)
        //.maxstack  2
        //.locals init ([0] class dalCoreSE.s_User_Rule CS$1$0000)
        //IL_0000:  nop
        //IL_0001:  ldarg.0
        //IL_0002:  newobj     instance void dalCoreSE.s_User_Rule::.ctor(int32)
        //IL_0007:  stloc.0
        //IL_0008:  br.s       IL_000a
        //IL_000a:  ldloc.0
        //IL_000b:  ret
        //} // end of method s_User_Rule::Get
        public static s_User_Rule Get(int id)
        {
            return new s_User_Rule(id);
        }

        //        .method public hidebysig static class dalCoreSE.s_User_Rule[] 
        //        GetList() cil managed
        //{
        //  // Code size       80 (0x50)
        //  .maxstack  3
        //  .locals init ([0] class dalCoreSE.s_User_Rule t,
        //           [1] class [System.Data]System.Data.DataTable tempTable,
        //           [2] class dalCoreSE.s_User_Rule[] result,
        //           [3] int32 i,
        //           [4] class dalCoreSE.s_User_Rule[] CS$1$0000,
        //           [5] bool CS$4$0001)
        //  IL_0000:  nop
        //  IL_0001:  newobj     instance void dalCoreSE.s_User_Rule::.ctor()
        //  IL_0006:  stloc.0
        //  IL_0007:  ldloc.0
        //  IL_0008:  callvirt   instance class [System.Data]System.Data.DataTable dalCoreSE.SQLBookSEBase::LoadDataTable()
        //  IL_000d:  stloc.1
        //  IL_000e:  ldloc.1
        //  IL_000f:  callvirt   instance class [System.Data]System.Data.DataRowCollection [System.Data]System.Data.DataTable::get_Rows()
        //  IL_0014:  callvirt   instance int32 [System.Data]System.Data.InternalDataCollectionBase::get_Count()
        //  IL_0019:  newarr     dalCoreSE.s_User_Rule
        //  IL_001e:  stloc.2
        //  IL_001f:  ldc.i4.0
        //  IL_0020:  stloc.3
        //  IL_0021:  br.s       IL_003c
        //  IL_0023:  nop
        //  IL_0024:  ldloc.2
        //  IL_0025:  ldloc.3
        //  IL_0026:  newobj     instance void dalCoreSE.s_User_Rule::.ctor()
        //  IL_002b:  stelem.ref
        //  IL_002c:  ldloc.2
        //  IL_002d:  ldloc.3
        //  IL_002e:  ldelem.ref
        //  IL_002f:  ldloc.1
        //  IL_0030:  ldloc.3
        //  IL_0031:  callvirt   instance void dalCoreSE.SQLBookSEBase::Postload(class [System.Data]System.Data.DataTable,
        //                                                                       int32)
        //  IL_0036:  nop
        //  IL_0037:  nop
        //  IL_0038:  ldloc.3
        //  IL_0039:  ldc.i4.1
        //  IL_003a:  add
        //  IL_003b:  stloc.3
        //  IL_003c:  ldloc.3
        //  IL_003d:  ldloc.2
        //  IL_003e:  ldlen
        //  IL_003f:  conv.i4
        //  IL_0040:  clt
        //  IL_0042:  stloc.s    CS$4$0001
        //  IL_0044:  ldloc.s    CS$4$0001
        //  IL_0046:  brtrue.s   IL_0023
        //  IL_0048:  ldloc.2
        //  IL_0049:  stloc.s    CS$1$0000
        //  IL_004b:  br.s       IL_004d
        //  IL_004d:  ldloc.s    CS$1$0000
        //  IL_004f:  ret
        //} // end of method s_User_Rule::GetList
        public static s_User_Rule[] GetList()
        {
            s_User_Rule t = new s_User_Rule();
            DataTable tempTable = t.LoadDataTable();
            s_User_Rule[] result = new s_User_Rule[tempTable.Rows.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new s_User_Rule();
                result[i].Postload(tempTable, i);
            }

            return result;
        }

        //.method public hidebysig static class dalCoreSE.s_User_Rule[] 
        //GetItems() cil managed
        //{
        //// Code size       11 (0xb)
        //.maxstack  1
        //.locals init ([0] class dalCoreSE.s_User_Rule[] CS$1$0000)
        //IL_0000:  nop
        //IL_0001:  call       !!0[] dalCoreSE.DalUtility::GetList<class dalCoreSE.s_User_Rule>()
        //IL_0006:  stloc.0
        //IL_0007:  br.s       IL_0009
        //IL_0009:  ldloc.0
        //IL_000a:  ret
        //} // end of method s_User_Rule::GetItems
        //Нужен Utility класс (обращаться к нему из статического метода)?
        public static s_User_Rule[] GetItems()
        {
            return DalUtility.GetList<s_User_Rule>();
        }
    }
}
