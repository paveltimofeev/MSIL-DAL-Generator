using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dalCoreSE
{
    [global::System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class DalAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string positionalString;

        // This is a positional argument

        public DalAttribute()
        {
            ;
        }

        public DalAttribute(string positionalString)
        {
            this.positionalString = positionalString;

            // TODO: Implement code here
            throw new NotImplementedException();
        }

        public string PositionalString { get; private set; }

        // This is a named argument
        public int NamedInt { get; set; }
    }

    [global::System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class DalClassAttribute : Attribute
    {
        readonly string databaseTableName;

        public DalClassAttribute(string databaseTableName)
        {
            this.databaseTableName = databaseTableName;
        }

        public string DatabaseTableName
        {
            get { return databaseTableName; }
        }
    }
}
