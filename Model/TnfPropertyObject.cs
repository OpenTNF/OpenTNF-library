using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfPropertyObject
    {
        string Oid { get; }
        string Vid { get; }
        int CatalogueOid { get; }
        int PropertyObjectTypeOid { get; }
    }

    public class TnfPropertyObject : ITnfPropertyObject
    {
        public string Oid { get; set; }
        public string Vid { get; set; }
        public int CatalogueOid { get; set; }
        public int PropertyObjectTypeOid { get; set; } 

        public override bool Equals(object obj)
        {
            if (obj is TnfPropertyObject)
            {
                var v = obj as TnfPropertyObject;

                if (Oid == v.Oid &&
                    Vid == v.Vid &&
                    CatalogueOid == v.CatalogueOid &&
                    PropertyObjectTypeOid == v.PropertyObjectTypeOid)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(
                Oid,
                Vid,
                CatalogueOid,
                PropertyObjectTypeOid);
        }

        public override string ToString()
        {
            return String.Format("TnfPropertyObject: Oid = {0}, Vid = {1}, CatalogueOid = {2}, PropertyTypeOid = {3}",
                Oid,
                Vid,
                CatalogueOid,
                PropertyObjectTypeOid);
        }
    }

    public class TnfPropertyObjectManager : TableManager
    {
        public static string TnfPropertyObjectTableName = "tnf_property_object";

        public TnfPropertyObjectManager(GeoPackageDatabase db) : base(db, TnfPropertyObjectTableName, GetColumnInfos())
        {
        }

        protected override string[] Constraints()
        {
            return new[]
            {
                String.Format("CONSTRAINT fk_tpo_co FOREIGN KEY (catalogue_oid) REFERENCES {0}(oid)",
                    TnfCatalogueManager.TnfCatalogueTableName),
                String.Format("CONSTRAINT fk_tpo_co_poto FOREIGN KEY (property_object_type_oid, catalogue_oid) REFERENCES {0}(oid, catalogue_oid)", TnfPropertyObjectTypeManager.TnfPropertyObjectTypeTableName)
            };
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "oid",
                    SqlType = "TEXT",
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
                },
                new ColumnInfo
                {
                    Name = "vid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
            };
        }

        public void Add(TnfPropertyObject tnfPropertyObject)
        {
            Add(new object[]
                {
                    tnfPropertyObject.Oid,
                    tnfPropertyObject.CatalogueOid,
                    tnfPropertyObject.PropertyObjectTypeOid,
                    tnfPropertyObject.Vid
                });
        }

        public TnfPropertyObject Get(string oid)
        {
            return Get(ReadPropertyObject, new object[] { oid });
        }

        public List<TnfPropertyObject> Get(int maxResults)
        {
            return Get(ReadPropertyObject, maxResults);
        }

        public List<TnfPropertyObject> GetPage(int offset, int limit)
        {
            return GetPage(ReadPropertyObject, offset, limit);
        }

        public int Update(TnfPropertyObject tnfPropertyObject)
        {
            return Update(new object[]
                {
                    tnfPropertyObject.Oid,
                    tnfPropertyObject.CatalogueOid,
                    tnfPropertyObject.PropertyObjectTypeOid,
                    tnfPropertyObject.Vid
                });
        }

        public int Delete(string oid)
        {
            return Delete(new object[] { oid });
        }

        private static TnfPropertyObject ReadPropertyObject(IDataRecord reader)
        {
            var tnfPropertyObject = new TnfPropertyObject();

            tnfPropertyObject.Oid = reader["oid"].FromDbString();
            tnfPropertyObject.CatalogueOid = reader["catalogue_oid"].ToInt();
            tnfPropertyObject.PropertyObjectTypeOid = reader["property_object_type_oid"].ToInt();
            tnfPropertyObject.Vid = reader["vid"].FromDbString();

            return tnfPropertyObject;
        }
    }
}
