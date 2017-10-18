using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfDirectLocationReference
    {
        string PropertyOid { get; }
        string LocationReferenceType { get; }
        string LocationReference { get; }
        int? SeqNo { get; }
    }

    public class TnfDirectLocationReference : ITnfDirectLocationReference
    {
        public string PropertyOid { get; set; }
        public string LocationReferenceType { get; set; }
        public string LocationReference { get; set; }
        public int? SeqNo { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TnfDirectLocationReference)
            {
                var v = obj as TnfDirectLocationReference;

                if (PropertyOid.Equals(v.PropertyOid) &&
                    LocationReferenceType.Equals(v.LocationReferenceType) &&
                    LocationReference.Equals(v.LocationReference) &&
                    SeqNo.Equals(v.SeqNo))
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(
                PropertyOid,
                LocationReferenceType,
                LocationReference,
                SeqNo);
        }

        public override string ToString()
        {
            return String.Format("TnfDirectLocationReferenceManager: PropertyOid = {0}, LocationReferenceType = {1}, LocationReference = {2}, SeqNo = {3}",
                PropertyOid,
                LocationReferenceType,
                LocationReference,
                SeqNo);
        }
    }

    public class TnfDirectLocationReferenceManager : TableManager
    {
        private const string PrimaryKey = "property_oid";
        public static string TnfDirectLocationReferenceTableName = "tnf_direct_location_reference";

        public TnfDirectLocationReferenceManager(GeoPackageDatabase db) : base(db, TnfDirectLocationReferenceTableName, GetColumnInfos(),PrimaryKey)
        {
        }

        protected override string[] Constraints()
        {
            return new[]
                {
                    String.Format("CONSTRAINT fk_tdlr_po FOREIGN KEY (property_oid) REFERENCES {0}(oid)", TnfPropertyObjectManager.TnfPropertyObjectTableName)
                };
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "property_oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "location_reference_type",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "location_reference",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "seq_no",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32")
                }
            };
        }

        public void Add(TnfDirectLocationReference tnfDirectLocationReference)
        {
            Add(new object[]
                {
                    tnfDirectLocationReference.PropertyOid,
                    tnfDirectLocationReference.LocationReferenceType,
                    tnfDirectLocationReference.LocationReference,
                    tnfDirectLocationReference.SeqNo
                });
        }

        public TnfDirectLocationReference Get(string propertyOid)
        {
            return Get(ReadObject, new object[] { propertyOid });
        }

        public List<TnfDirectLocationReference> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(TnfDirectLocationReference tnfDirectLocationReference)
        {
            return Update(new object[]
                {
                    tnfDirectLocationReference.PropertyOid,
                    tnfDirectLocationReference.LocationReferenceType,
                    tnfDirectLocationReference.LocationReference,
                    tnfDirectLocationReference.SeqNo
                });
        }

        public int Delete(string propertyOid)
        {
            return Delete(new object[] { propertyOid });
        }

        private static TnfDirectLocationReference ReadObject(IDataRecord reader)
        {
            var tnfDirectLocationReference = new TnfDirectLocationReference();

            tnfDirectLocationReference.PropertyOid = reader["property_oid"].ToString();
            tnfDirectLocationReference.LocationReferenceType = reader["location_reference_type"].ToString();
            tnfDirectLocationReference.LocationReference = reader["location_reference"].ToString();
            tnfDirectLocationReference.SeqNo = reader["seq_no"].ToInt32();

            return tnfDirectLocationReference;
        }
    }
}
