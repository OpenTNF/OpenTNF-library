using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OpenTNF.Library.Model
{
    public interface ITnfTaskEditableType
    {
        string TaskOid { get; }
        string CatalogueOid { get; }
        string PropertyObjectTypeOid { get; }
    }

    public class TnfTaskEditableType : ITnfTaskEditableType
    {
        public string TaskOid { get; set; }
        public string CatalogueOid { get; set; }
        public string PropertyObjectTypeOid { get; set; }
    }

    public class TnfTaskEditableTypeManager : TableManager
    {
        public static string TnfTaskEditableTypeTableName = "tnf_task_editable_type";

        public TnfTaskEditableTypeManager(GeoPackageDatabase db) : base(db, TnfTaskEditableTypeTableName, GetColumnInfos(),"task_oid,catalogue_oid,property_object_type_oid")
        {
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "task_oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "catalogue_oid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "property_object_type_oid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                }
            };
        }

        public void Add(TnfTaskEditableType tnfTaskEditableType)
        {
            Add(new object[]
                {
                    tnfTaskEditableType.TaskOid,
                    tnfTaskEditableType.CatalogueOid,
                    tnfTaskEditableType.PropertyObjectTypeOid
                });
        }

        public List<TnfTaskEditableType> GetAllForTask(string taskOid)
        {
            var tnfList = new List<TnfTaskEditableType>();

            using (var command = Db.Command)
            {
                command.CommandText =
                    string.Format(
                        "SELECT * FROM {0} WHERE task_oid = '{1}'", TnfTaskEditableTypeTableName, taskOid);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tnfList.Add(ReadObject(reader));
                    }
                }
            }
            return tnfList;
        }

      

        public TnfTaskEditableType Get(string taskOid, string catalogueOid, string propertyObjectTypeOid)
        {

            return Get(ReadObject, new object[]{taskOid,catalogueOid,propertyObjectTypeOid});
        }

        public List<TnfTaskEditableType> Get(int maxresult)
        {
            return Get(ReadObject, maxresult);
        } 

        public int Update(TnfTaskEditableType tnfTaskEditableType)
        {
            return Update(new object[]
                {
                    tnfTaskEditableType.TaskOid,
                    tnfTaskEditableType.CatalogueOid,
                    tnfTaskEditableType.PropertyObjectTypeOid
                });
        }

        public int Delete(string taskoid, string catalogueoid, string propertyobjecttypeoid)
        {
            //int ret;
            //string cmdtext = string.Format(
            //    "DELETE FROM {0} WHERE task_oid = '{1}' and catalogue_oid = '{2}' and property_object_type_oid = '{3}'",
            //    TableName, oid, catalogueoid, propertyobjecttypeoid);
            //using (var delete =Db.Command)
            //{
            //    delete.CommandText = cmdtext;
            //    delete.Prepare();
            //   ret = delete.ExecuteNonQuery();
            //}      
            //return ret;
            return Delete(new object[] {taskoid,catalogueoid,propertyobjecttypeoid});
        }

        public int Count(string oid)
        {
            return Count(new object[] { oid });
        }

        private static TnfTaskEditableType ReadObject(IDataReader reader)
        {
            TnfTaskEditableType tnfTaskEditableType = new TnfTaskEditableType();
            tnfTaskEditableType.TaskOid = reader["task_oid"].FromDbString();
            tnfTaskEditableType.CatalogueOid = reader["catalogue_oid"].FromDbString();
            tnfTaskEditableType.PropertyObjectTypeOid = reader["property_object_type_oid"].FromDbString();
            return tnfTaskEditableType;
        }
    }
}
