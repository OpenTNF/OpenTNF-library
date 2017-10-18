using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfMetadata
    {
        string MetaKey { get; }
        string MetaValue { get; }
    }

    public class TnfMetadata : ITnfMetadata
    {
        public string MetaKey { get; set; }
        public string MetaValue { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TnfMetadata)
            {
                var v = obj as TnfMetadata;

                if (MetaKey == v.MetaKey &&
                    MetaValue == v.MetaValue)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(
                MetaKey,
                MetaValue);
        }

        public override string ToString()
        {
            return String.Format("TnfMetadata: MetaKey = {0}, MetaValue = {1}",
                MetaKey,
                MetaValue);
        }
    }


    public class TnfMetadataManager : TableManager
    {
        private const string PrimaryKey = "meta_key";
        public static string TnfMetadataTableName = "tnf_metadata";

        public const string MetadataKeyCrs = "TNF_CRS_NAME";
        public const string MetadataKeyVersion = "TNF_VERSION";

        public TnfMetadataManager(GeoPackageDatabase db) : base(db, TnfMetadataTableName, GetColumnInfos(),PrimaryKey)
        {
        }

        protected override string[] Constraints()
        {
            return null;
        }

        public void Add(TnfMetadata tnfMetadata)
        {
            Add(new object[]
                {
                    tnfMetadata.MetaKey,
                    tnfMetadata.MetaValue
                });
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "meta_key",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "meta_value",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
            };
        }

        public TnfMetadata Get(string metaKey)
        {
            return Get(ReadObject, new object[] { metaKey });
        }

        public List<TnfMetadata> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(TnfMetadata tnfMetadata)
        {
            return Update(new object[]
                {
                    tnfMetadata.MetaKey,
                    tnfMetadata.MetaValue
                });
        }

        public int Delete(string metaKey)
        {
            return Delete(new object[] { metaKey });
        }

        private static TnfMetadata ReadObject(IDataRecord reader)
        {
            var tnfMetadata = new TnfMetadata();

            tnfMetadata.MetaKey = reader["meta_key"].FromDbString();
            tnfMetadata.MetaValue = reader["meta_value"].FromDbString();

            return tnfMetadata;
        }

        public int GetCrs()
        {
            TnfMetadata crsMetadata = Get(MetadataKeyCrs);
            if (crsMetadata == null)
            {
                return -1;
            }
            string crsString = crsMetadata.MetaValue;
            string[] splitCrsString = crsString.Split(':');
            int crs;
            if (splitCrsString.Length != 2 || !int.TryParse(splitCrsString[1], out crs))
            {
                return -1;
            }
            return crs;
        }
    }
}
