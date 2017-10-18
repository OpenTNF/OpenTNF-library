using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfSecondaryLrs
    {
        string Oid { get; }
        string Name { get; }
        int Type { get; }
        int CatalogueOid { get; }
        string PropertyObjectTypeOid { get; }
        int AssocReferentPropertyObjectTypeOid { get; }
        string Measure1PropertyTypeOid { get; }
        string Measure2PropertyTypeOid { get; }
        string WhereClause { get; }
        string SequencePropertyTypeOid { get; }
        bool OrderDescending { get; }
    }

    public class TnfSecondaryLrs : ITnfSecondaryLrs
    {
        public string Oid { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public int CatalogueOid { get; set; }
        public string PropertyObjectTypeOid { get; set; }
        public int AssocReferentPropertyObjectTypeOid { get; set; }
        public string Measure1PropertyTypeOid { get; set; }
        public string Measure2PropertyTypeOid { get; set; }
        public string WhereClause { get; set; }
        public string SequencePropertyTypeOid { get; set; }
        public bool OrderDescending { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TnfSecondaryLrs)
            {
                var v = obj as TnfSecondaryLrs;

                if (Oid == v.Oid &&
                    Name == v.Name &&
                    Type == v.Type &&
                    CatalogueOid == v.CatalogueOid &&
                    PropertyObjectTypeOid == v.PropertyObjectTypeOid &&
                    AssocReferentPropertyObjectTypeOid == v.AssocReferentPropertyObjectTypeOid &&
                    Measure1PropertyTypeOid == v.Measure1PropertyTypeOid &&
                    Measure2PropertyTypeOid == v.Measure2PropertyTypeOid &&
                    WhereClause == v.WhereClause &&
                    SequencePropertyTypeOid == v.SequencePropertyTypeOid &&
                    OrderDescending == v.OrderDescending)
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
                Name,
                Type,
                CatalogueOid,
                PropertyObjectTypeOid,
                AssocReferentPropertyObjectTypeOid,
                Measure1PropertyTypeOid,
                Measure2PropertyTypeOid,
                WhereClause,
                SequencePropertyTypeOid,
                OrderDescending);
        }

        public override string ToString()
        {
            return
                string.Format(
                    "TnfSecondaryLrs: Oid = {0}, Name = {1}, Type = {2}, CatalogueOid = {3}, PropertyObjectTypeOid = {4}, " +
                    "AssocReferentPropertyObjectTypeOid = {5}, Measure1PropertyTypeOid = {6}, Measure2PropertyTypeOid = {7}, " +
                    "WhereClause = {8}, SequencePropertyTypeOid = {9}, OrderDescending = {10}",
                    Oid,
                    Name,
                    Type,
                    CatalogueOid,
                    PropertyObjectTypeOid,
                    AssocReferentPropertyObjectTypeOid,
                    Measure1PropertyTypeOid,
                    Measure2PropertyTypeOid,
                    WhereClause,
                    SequencePropertyTypeOid,
                    OrderDescending);
        }
    }

    public class TnfSecondaryLrsManager : TableManager
    {
        public static string TnfSecondaryLrsTableName = "tnf_secondary_lrs";

        public TnfSecondaryLrsManager(GeoPackageDatabase db) : base(db, TnfSecondaryLrsTableName, GetColumnInfos())
        {
        }

        protected override string[] Constraints()
        {
            return new[]
                {
                    String.Format("CONSTRAINT fk_tsl_co FOREIGN KEY (catalogue_oid) REFERENCES {0}(oid)",TnfCatalogueManager.TnfCatalogueTableName)
                };
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "name",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "type",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "catalogue_oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "property_object_type_oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "assoc_referent_property_object_type_oid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "measure1_property_type_oid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "measure2_property_type_oid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "where_clause",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "sequence_property_type_oid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "order_descending",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Boolean")
                },
            };
        }

        public void Add(TnfSecondaryLrs tnfSecondaryLrs)
        {
            Add(new object[]
                {
                    tnfSecondaryLrs.Oid,
                    tnfSecondaryLrs.Name,
                    tnfSecondaryLrs.Type,
                    tnfSecondaryLrs.CatalogueOid,
                    tnfSecondaryLrs.PropertyObjectTypeOid,
                    tnfSecondaryLrs.AssocReferentPropertyObjectTypeOid,
                    tnfSecondaryLrs.Measure1PropertyTypeOid,
                    tnfSecondaryLrs.Measure2PropertyTypeOid,
                    tnfSecondaryLrs.WhereClause,
                    tnfSecondaryLrs.SequencePropertyTypeOid,
                    tnfSecondaryLrs.OrderDescending
                });
        }

        public TnfSecondaryLrs GetByOid(string oid)
        {
            return Get(ReadObject, new object[] { oid });
        }

        public List<TnfSecondaryLrs> GetMany(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(TnfSecondaryLrs tnfSecondaryLrs)
        {
            return Update(new object[]
                {
                    tnfSecondaryLrs.Oid,
                    tnfSecondaryLrs.Name,
                    tnfSecondaryLrs.Type,
                    tnfSecondaryLrs.CatalogueOid,
                    tnfSecondaryLrs.PropertyObjectTypeOid,
                    tnfSecondaryLrs.AssocReferentPropertyObjectTypeOid,
                    tnfSecondaryLrs.Measure1PropertyTypeOid,
                    tnfSecondaryLrs.Measure2PropertyTypeOid,
                    tnfSecondaryLrs.WhereClause,
                    tnfSecondaryLrs.SequencePropertyTypeOid,
                    tnfSecondaryLrs.OrderDescending
                });
        }

        public int Delete(string oid)
        {
            return Delete(new object[] { oid });
        }

        private static TnfSecondaryLrs ReadObject(IDataRecord reader)
        {
            var tnfSecondaryLrs = new TnfSecondaryLrs();

            tnfSecondaryLrs.Oid = reader["oid"].FromDbString();
            tnfSecondaryLrs.Name = reader["name"].FromDbString();
            tnfSecondaryLrs.Type =  reader["type"].ToInt();
            tnfSecondaryLrs.CatalogueOid = reader["catalogue_oid"].ToInt();
            tnfSecondaryLrs.PropertyObjectTypeOid = reader["property_object_type_oid"].FromDbString();
            tnfSecondaryLrs.AssocReferentPropertyObjectTypeOid = reader["assoc_referent_property_object_type_oid"].ToInt();
            tnfSecondaryLrs.Measure1PropertyTypeOid = reader["measure1_property_type_oid"].FromDbString();
            tnfSecondaryLrs.Measure2PropertyTypeOid = reader["measure2_property_type_oid"].FromDbString();
            tnfSecondaryLrs.WhereClause = reader["where_clause"].FromDbString();
            tnfSecondaryLrs.SequencePropertyTypeOid = reader["sequence_property_type_oid"].FromDbString();
            tnfSecondaryLrs.OrderDescending = reader["order_descending"].ToInt() > 0;
            return tnfSecondaryLrs;
        }

    }
}

