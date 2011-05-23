using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.IO;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using dalCoreSE;

namespace DProcessorSE
{
    public class DFacto
    {
        DataSet ds;
        SqlConnection conn;
        string filter = "%%";
        string spfilter = "%%";

        public DFacto(string connectionString)
        {
            conn = new SqlConnection(connectionString);
            UploadSchema(conn);
        }

        #region Работа со сборкой

        /// <summary>
        /// Сохраняет сборку на жёсткий диск
        /// </summary>
        /// <param name="fileName">Full path to file to save dll. If file exsist it will overwrite.</param>
        public void Save(string fileName, string assemblyName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            AssemblyBuilder a = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new System.Reflection.AssemblyName(assemblyName),
                AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder m = a.DefineDynamicModule("dlaModule", Path.GetFileName(fileName));

            foreach (EntityData e in GetEntities())
            {
                
                if (!e.IsStoredProcedure)
                {
                    string className = (e.Namespace != "") ? string.Format("{0}.{1}", e.Namespace, e.ClassName) : e.ClassName;
                    GenerateType(m, className, e.DatabaseTableName, e.Fields);
                }
                else
                {
                    string className = (e.Namespace != "") ? string.Format("{0}.StoredProcedures.{1}", e.Namespace, e.ClassName) : e.ClassName;
                    GenerateSPType(m, className, e.ClassName, e.Fields);
                }
            }

            a.DefineVersionInfoResource("Data access layer component.", "1.0.0.0", "Pavel Timofeev, Saint-Petersburg, Russia", "© pavel timofeev", "DLA");
            a.Save(Path.GetFileName(fileName));
        }

        #endregion

        #region Работа с базой данных

        /// <summary>
        /// Инициализирует первичное подключение и запрашивает данные из базы
        /// </summary>
        /// <param name="conn"></param>
        private void UploadSchema(SqlConnection conn)
        {
            if (filter == "")
                filter = "%%";

            if (spfilter == "")
                spfilter = "%%";

            SqlCommand cmd;

            cmd = new SqlCommand("select TABLE_CATALOG, TABLE_NAME, * from INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME LIKE @filter ; select INFORMATION_SCHEMA.COLUMNS.TABLE_CATALOG, INFORMATION_SCHEMA.COLUMNS.TABLE_NAME, COLUMN_NAME, DATA_TYPE from INFORMATION_SCHEMA.COLUMNS JOIN INFORMATION_SCHEMA.TABLES ON INFORMATION_SCHEMA.TABLES.TABLE_NAME = INFORMATION_SCHEMA.COLUMNS.TABLE_NAME WHERE TABLE_TYPE = 'BASE TABLE' AND INFORMATION_SCHEMA.COLUMNS.TABLE_NAME LIKE @filter; select INFORMATION_SCHEMA.PARAMETERS.SPECIFIC_CATALOG as [dataBase], INFORMATION_SCHEMA.PARAMETERS.SPECIFIC_SCHEMA as [schema], INFORMATION_SCHEMA.PARAMETERS.SPECIFIC_NAME as [procedure] from INFORMATION_SCHEMA.PARAMETERS JOIN INFORMATION_SCHEMA.ROUTINES ON INFORMATION_SCHEMA.ROUTINES.SPECIFIC_NAME = INFORMATION_SCHEMA.PARAMETERS.SPECIFIC_NAME WHERE INFORMATION_SCHEMA.ROUTINES.ROUTINE_TYPE = 'PROCEDURE' and INFORMATION_SCHEMA.PARAMETERS.SPECIFIC_NAME like @spfilter GROUP BY INFORMATION_SCHEMA.PARAMETERS.SPECIFIC_CATALOG, INFORMATION_SCHEMA.PARAMETERS.SPECIFIC_SCHEMA, INFORMATION_SCHEMA.PARAMETERS.SPECIFIC_NAME ORDER BY INFORMATION_SCHEMA.PARAMETERS.SPECIFIC_NAME; select INFORMATION_SCHEMA.PARAMETERS.SPECIFIC_CATALOG as [dataBase], INFORMATION_SCHEMA.PARAMETERS.SPECIFIC_SCHEMA as [schema], INFORMATION_SCHEMA.PARAMETERS.SPECIFIC_NAME as [procedure], INFORMATION_SCHEMA.PARAMETERS.ORDINAL_POSITION as [parameterPosition], INFORMATION_SCHEMA.PARAMETERS.PARAMETER_NAME as [parameterName], INFORMATION_SCHEMA.PARAMETERS.DATA_TYPE as [parameterType] from INFORMATION_SCHEMA.PARAMETERS JOIN INFORMATION_SCHEMA.ROUTINES ON INFORMATION_SCHEMA.ROUTINES.SPECIFIC_NAME = INFORMATION_SCHEMA.PARAMETERS.SPECIFIC_NAME WHERE INFORMATION_SCHEMA.ROUTINES.ROUTINE_TYPE = 'PROCEDURE' and INFORMATION_SCHEMA.PARAMETERS.SPECIFIC_NAME like @spfilter ORDER BY INFORMATION_SCHEMA.PARAMETERS.SPECIFIC_NAME, INFORMATION_SCHEMA.PARAMETERS.ORDINAL_POSITION;", conn);
            cmd.Parameters.Add("@filter", SqlDbType.NVarChar, filter.Length).Value = filter;
            cmd.Parameters.Add("@spfilter", SqlDbType.NVarChar, filter.Length).Value = spfilter;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);
        }

        /// <summary>
        /// Запрашивает данные из базы
        /// </summary>
        public void RefreshDatabaseSchema()
        {
            UploadSchema(conn);
            AllEntities = GetEntities();
        }

        public EntityData[] AllEntities { get; set; }

        /// <summary>
        /// Запрашивает данные из базы
        /// </summary>
        /// <param name="connectionString"></param>
        public void RefreshDatabaseSchema(string connectionString)
        {
            conn = new SqlConnection(connectionString);
            UploadSchema(conn);
            AllEntities = GetEntities();
        }

        /// <summary>
        /// Получает информацию о сущностях из базы данных
        /// </summary>
        /// <returns></returns>
        public EntityData[] GetEntities()
        {
            Dictionary<string, EntityData> entities = new Dictionary<string, EntityData>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string className = dr["TABLE_NAME"].ToString();
                EntityData edata =new EntityData(conn.Database, className);
                edata.DatabaseTableName = dr["TABLE_NAME"].ToString();
                entities.Add(className, edata);
            }

            foreach (DataRow dr in ds.Tables[1].Rows)
            {
                EntityType t = new EntityType();
                t.Name = dr["COLUMN_NAME"].ToString();
                t.TypeName = DalTypeConverter.ToCLR(dr["DATA_TYPE"].ToString());
                t.SqlTypeName = DalTypeConverter.ToSql(dr["DATA_TYPE"].ToString());
                
                t.IsKey = (t.Name.ToUpper() == "ID") ? true : false;
                entities[dr["TABLE_NAME"].ToString()].Fields.Add(t);
            }

            foreach (DataRow dr in ds.Tables[2].Rows)
            {
                //stored procedures
                EntityData spEntity = new EntityData(dr["dataBase"].ToString(), dr["procedure"].ToString());
                spEntity.IsStoredProcedure = true;
                entities.Add(spEntity.ClassName, spEntity);
            }

            foreach (DataRow dr in ds.Tables[3].Rows)
            {
                //stored procedures
                EntityType parameter = new EntityType();
                parameter.Name = dr["parameterName"].ToString().Replace("@", "");
                parameter.TypeName = DalTypeConverter.ToCLR(dr["parameterType"].ToString());
                entities[dr["procedure"].ToString()].Fields.Add(parameter);
            }

            EntityData[] result = new EntityData[entities.Count];
            entities.Values.CopyTo(result, 0);

            return result;
        }

        #endregion

        #region Создание типа для справочника

        /// <summary>
        /// Генерирует тип для работы со справочником
        /// </summary>
        /// <param name="m"></param>
        /// <param name="typeName"></param>
        /// <param name="entityProps"></param>
        private void GenerateType(ModuleBuilder m, String typeName, String databaseTableName, EntityTypes entityProps)
        {
            TypeBuilder t = m.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass, typeof(SQLBookSEBase));

            CustomAttributeBuilder attr = new CustomAttributeBuilder(
                typeof(DalClassAttribute).GetConstructor(new Type[] { typeof(string) }),
                new object[] { databaseTableName });
            t.SetCustomAttribute(attr);

            t.DefineDefaultConstructor(MethodAttributes.Public);

            MethodInfo accessorId = t.BaseType.GetMethod("set_Id", new Type[] { typeof(int) });

            //
            //Generate constructor by Id
            //

            ConstructorBuilder ctor = t.DefineConstructor(
                MethodAttributes.Private,
                CallingConventions.Standard,
                new Type[] { typeof(int) });

            ctor.DefineParameter(1, ParameterAttributes.In, "Id");

            ILGenerator genCtor = ctor.GetILGenerator();
            genCtor.Emit(OpCodes.Ldarg_0);
            genCtor.Emit(OpCodes.Call, t.BaseType.GetConstructor(Type.EmptyTypes));
            genCtor.Emit(OpCodes.Nop);
            genCtor.Emit(OpCodes.Nop);
            genCtor.Emit(OpCodes.Ldarg_0);
            genCtor.Emit(OpCodes.Ldarg_1);
            genCtor.Emit(OpCodes.Call, accessorId);
            genCtor.Emit(OpCodes.Nop);
            genCtor.Emit(OpCodes.Ldarg_0);
            genCtor.Emit(OpCodes.Ldarg_1);
            genCtor.Emit(OpCodes.Call, t.BaseType.GetMethod("Load", new Type[] { typeof(int) }));
            genCtor.Emit(OpCodes.Pop);
            genCtor.Emit(OpCodes.Nop);
            genCtor.Emit(OpCodes.Ret);

            //
            //End of generate constructor
            //

            GenerateStaticGet(t, ctor);
            GenerateStaticGetList(t, ctor);

            foreach (EntityType et in entityProps)
            {
                if (!et.IsKey)
                    GenerateProp(t, et.TypeName, et.Name);
            }

            t.CreateType();
        }

        /// <summary>
        /// Генерирует свойство класса
        /// </summary>
        /// <param name="t"></param>
        /// <param name="propType"></param>
        /// <param name="propName"></param>
        private void GenerateProp(TypeBuilder t, Type propType, String propName)
        {
            FieldBuilder f = t.DefineField(
                "_" + propName,
                propType,
                FieldAttributes.Private);

            PropertyBuilder p = t.DefineProperty(
                propName,
                System.Reflection.PropertyAttributes.HasDefault,
                propType,
                Type.EmptyTypes);

            CustomAttributeBuilder cab = new CustomAttributeBuilder(
                typeof(DalAttribute).GetConstructor(Type.EmptyTypes),
                new object[] { });
            p.SetCustomAttribute(cab);

            MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            MethodBuilder testGetProp = t.DefineMethod(
                "get_" + propName,
                getSetAttr,
                propType,
                Type.EmptyTypes);

            ILGenerator genG = testGetProp.GetILGenerator();
            genG.Emit(OpCodes.Ldarg_0);
            genG.Emit(OpCodes.Ldfld, f);
            genG.Emit(OpCodes.Ret);

            MethodBuilder testSetProp = t.DefineMethod(
                "set_" + propName,
                getSetAttr,
                null,
                new Type[] { propType });

            ILGenerator genS = testSetProp.GetILGenerator();
            genS.Emit(OpCodes.Ldarg_0);
            genS.Emit(OpCodes.Ldarg_1);
            genS.Emit(OpCodes.Stfld, f);
            genS.Emit(OpCodes.Ret);

            p.SetGetMethod(testGetProp);
            p.SetSetMethod(testSetProp);
        }

        private void GenerateStaticGet(TypeBuilder t, ConstructorBuilder ctor)
        {
            MethodBuilder mb = t.DefineMethod("Get", 
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, 
                ctor.DeclaringType, new Type[] { typeof(Int32) });
            
            ILGenerator gen = mb.GetILGenerator();
            Label l1 = gen.DefineLabel();
            gen.DeclareLocal(ctor.DeclaringType);
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Newobj, ctor);
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Br_S, l1);
            gen.MarkLabel(l1);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);
        }

        private void GenerateStaticGetList(TypeBuilder t, ConstructorBuilder ctor)
        {
            Type tt = ctor.DeclaringType.MakeArrayType();

            MethodInfo mi = typeof(DalUtility).GetMethod("GetList",
                BindingFlags.Static |
                BindingFlags.Public);
            MethodInfo genericMi = mi.MakeGenericMethod(new Type[] { ctor.DeclaringType });

            MethodBuilder mb = t.DefineMethod("GetList",
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig,
                tt, Type.EmptyTypes);
            ILGenerator gen = mb.GetILGenerator();
            gen.DeclareLocal(tt);
            Label l1 = gen.DefineLabel();
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Call, genericMi);
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Br_S, l1);
            gen.MarkLabel(l1);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);

        }

        #endregion

        #region Создание типа для хранимых процедур

        /// <summary>
        /// Генерирует статический метод для вызова хранимой процедуры, атрибуты которого являются входными параметрами хранимой процедуры.
        /// </summary>
        /// <param name="t">Type builder</param>
        /// <param name="methodName">Name of the generating static method</param>
        /// <param name="param">Key-Value pair of parameters (names as key and types as value)</param>
        /// <param name="SPName">Name of the stored procadure to call.</param>
        private static void GenerateStaticMethod(TypeBuilder t, string methodName, Dictionary<string, Type> param, string SPName)
        {
            Type[] args = new Type[param.Count];
            param.Values.CopyTo(args, 0);
            string[] argsNames = new string[param.Count];
            param.Keys.CopyTo(argsNames, 0);

            FieldBuilder fb = t.DefineField("processor", typeof(SPProcessor), FieldAttributes.Static | FieldAttributes.Private | FieldAttributes.HasDefault);
            MethodBuilder mb = t.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, typeof(DataTable), args);
            mb.InitLocals = true;

            #region MSIL generate

            ILGenerator gen = mb.GetILGenerator();
            Label l1 = gen.DefineLabel();
            gen.DeclareLocal(typeof(DataTable));
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Newobj, typeof(SPProcessor).GetConstructor(Type.EmptyTypes));
            gen.Emit(OpCodes.Stsfld, fb);
            gen.Emit(OpCodes.Ldsfld, fb);
            gen.Emit(OpCodes.Ldstr, SPName);
            gen.Emit(OpCodes.Callvirt, typeof(SPProcessor).GetMethod("SetProcedureName", new Type[] { typeof(String) }));
            gen.Emit(OpCodes.Nop);

            for (int i = 0; i < args.Length; i++)
            {
                gen.Emit(OpCodes.Ldsfld, fb);
                gen.Emit(OpCodes.Ldarg_S, i);
                gen.Emit(OpCodes.Ldstr, argsNames[i]);
                gen.Emit(OpCodes.Callvirt, typeof(SPProcessor).GetMethod("AddParameter", new Type[] { args[i], typeof(string) }));
                gen.Emit(OpCodes.Nop);
            }

            gen.Emit(OpCodes.Ldsfld, fb);
            gen.Emit(OpCodes.Callvirt, typeof(SPProcessor).GetMethod("Call", Type.EmptyTypes));
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Br_S, l1);
            gen.MarkLabel(l1);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);

            #endregion

            int iDefPar = 1;
            foreach (string parName in param.Keys)
            {
                mb.DefineParameter(iDefPar, ParameterAttributes.In, parName);
                iDefPar++;
            }
        }

        /// <summary>
        /// Генерирует класс для работы с хранимыми процедурами
        /// </summary>
        /// <param name="m"></param>
        /// <param name="nameSpace"></param>
        /// <param name="typeName"></param>
        private void GenerateSPType(ModuleBuilder m, string className, string storedProcedureName, EntityTypes entityTypes)
        {
            TypeBuilder t = m.DefineType(className,
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass,
                null);
            t.DefineDefaultConstructor(MethodAttributes.Public);

            Dictionary<string, Type> dict = new Dictionary<string, Type>();

            foreach (EntityType e in entityTypes)
            {
                dict.Add(e.Name, e.TypeName);
            }

            GenerateStaticMethod(t, "Call", dict, storedProcedureName);

            t.CreateType();
        }

        #endregion

        /// <summary>
        /// Фильтр для использования определённых таблиц
        /// </summary>
        public string Filter
        {
            get { return filter; }
            set { filter = value; }
        }

        /// <summary>
        /// Фильтр для использования определённых хранимых процедур
        /// </summary>
        public string StoredProcedureFilter
        {
            get { return spfilter; }
            set { spfilter = value; }
        }
    }
}