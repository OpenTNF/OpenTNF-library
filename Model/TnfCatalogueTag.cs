using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfCatalogueTag
    {
        int CatalogueOid { get; }
        string Owner { get; }
        string PropertyKey { get; }
        string Value { get; }
    }

    [Serializable]
    public class TnfCatalogueTag : ITnfCatalogueTag
    {
        public int CatalogueOid { get; set; }
        public string Owner { get; set; }
        public string PropertyKey { get; set; }
        public string Value { get; set; }
        
        public override bool Equals(object obj)
        {
            if (obj is TnfCatalogueTag v)
            {
                if (CatalogueOid == v.CatalogueOid && 
                    Owner == v.Owner &&
                    PropertyKey == v.PropertyKey &&
                    Value == v.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(
                CatalogueOid,
                Owner,
                PropertyKey,
                Value);
        }

        public override string ToString()
        {
            return string.Format("TnfCatalogueTag: CatalogueOid = {0}, Owner = {1}, PropertyKey = {2}, Value = {3}",
                CatalogueOid,
                Owner,
                PropertyKey,
                Value);
        }
    }

    public class TnfCatalogueTagManager : TableManager
    {
        private const string PrimaryKey = "catalogue_oid, owner, property_key";
        public const string TnfCatalogueTagTableName = "tnf_catalogue_tag";
    
        public TnfCatalogueTagManager(GeoPackageDatabase db) : base(db, TnfCatalogueTagTableName, GetColumnInfos(), PrimaryKey)
        {
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "catalogue_oid",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "owner",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "property_key",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "value",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                }
            };
        }

        public void Add(TnfCatalogueTag tnfCatalogueTag)
        {
            Add(new object[]
            {
                tnfCatalogueTag.CatalogueOid,
                tnfCatalogueTag.Owner,
                tnfCatalogueTag.PropertyKey,
                tnfCatalogueTag.Value
            });
        }

        public void AddRange(IEnumerable<TnfCatalogueTag> tnfCatalogueTags)
        {
            foreach (var tnfCatalogueTag in tnfCatalogueTags)
            {
                Add(new object[]
                {
                    tnfCatalogueTag.CatalogueOid,
                    tnfCatalogueTag.Owner,
                    tnfCatalogueTag.PropertyKey,
                    tnfCatalogueTag.Value
                });
            }        
        }

        public TnfCatalogueTag Get(string owner, string propertyKey, int catalogueOid)
        {
            return Get(ReadObject, new object[] { catalogueOid, owner, propertyKey});
        }

        public List<TnfCatalogueTag> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public List<TnfCatalogueTag> GetForOwner(int catalogueOid, string owner)
        {
            var tnfCatalogueTagList = new List<TnfCatalogueTag>();

            using (var command = Db.Command)
            {
                command.CommandText =
                        $"SELECT * FROM {TnfCatalogueTagTableName} WHERE catalogue_oid = {catalogueOid} AND owner = '{owner}'";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tnfCatalogueTagList.Add(ReadObject(reader));
                    }
                }
            }
            return tnfCatalogueTagList;
        }

        public int Update(TnfCatalogueTag tnfCatalogueTag)
        {
            return Update(new object[]
            {
                tnfCatalogueTag.CatalogueOid,
                tnfCatalogueTag.Owner,
                tnfCatalogueTag.PropertyKey,
                tnfCatalogueTag.Value
            });
        }

        public int Delete(string owner, string propertyKey, int catalogueOid)
        {
            return Delete(new object[] { catalogueOid, owner, propertyKey });
        }

        public string CreateOwnerForCatalogue()
        {
            return "CATALOGUE";
        }

        public string CreateOwnerForFeatureType(int featuretypeOid)
        {
            return $"FEATURE_TYPE/{featuretypeOid}";
        }

        public string CreateOwnerForPropertyType(int featuretypeOid,int propertyTypeOid)
        {
            return $"PROPERTY_TYPE/{featuretypeOid}/{propertyTypeOid}";
        }

        public string CreateOwnerForValueDomain(int valueDomainOid)
        {
            return $"VALUE_DOMAIN/{valueDomainOid}";
        }

        public string CreateOwnerForStructuredValueDomainPropertyType(int structuredValueDomainOid, int propertyTypeOid)
        {
            return $"SVD_PROPERTY_TYPE/{structuredValueDomainOid}/{propertyTypeOid}";
        }

        public string CreateOwnerForValidValue(int valueDomainOid,int sequenceNumber)
        {
            return $"VALID_VALUE/{valueDomainOid}/{sequenceNumber}";
        }

        public string CreateOwnerByType(object type)
        {
            if (type is TnfCatalogue c)
            {
                return CreateOwnerForCatalogue();
            }
            if (type is TnfPropertyObjectType ft)
            {
                return CreateOwnerForFeatureType(ft.Oid);
            }
            if (type is TnfPropertyObjectPropertyType pt)
            {
                return CreateOwnerForPropertyType(pt.PropertyObjectTypeOid,pt.Oid);
            }
            if (type is TnfValueDomain vd)
            {
                return CreateOwnerForValueDomain(vd.Oid);
            }
            if (type is TnfStructuredValueDomainPropertyType svpt)
            {
                return CreateOwnerForStructuredValueDomainPropertyType(svpt.StructuredValueDomainOid,svpt.Oid);
            }
            if (type is TnfValidValue vv)
            {
                return CreateOwnerForValidValue(vv.ValueDomainOid, vv.SeqNo);
            }
            return null;
        }

        private static TnfCatalogueTag ReadObject(IDataRecord reader)
        {
            var catalogueTag = new TnfCatalogueTag();

            catalogueTag.CatalogueOid = reader["catalogue_oid"].ToInt();
            catalogueTag.Owner = reader["owner"].FromDbString();
            catalogueTag.PropertyKey = reader["property_key"].FromDbString();
            catalogueTag.Value = reader["value"].FromDbString();

            return catalogueTag;
        }

        public void DeleteCatalogueTagForOwner(int catalogueOid, string owner)
        {
            using (var deleteCommand = Db.Command)
            {
                deleteCommand.CommandText = $"DELETE FROM tnf_catalogue_tag WHERE catalogue_oid = {catalogueOid} AND owner = '{owner}'";
                deleteCommand.ExecuteNonQuery();
            }
        }
    }
}
