using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfSecondaryLrsIdentity
    {
        string LrsOid { get; }
        string IdentityPropertyOid { get; }
    }

    public class TnfSecondaryLrsIdentity : ITnfSecondaryLrsIdentity
    {
        public string LrsOid { get; set; }
        public string IdentityPropertyOid { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TnfSecondaryLrsIdentity)
            {
                var v = obj as TnfSecondaryLrsIdentity;

                if (LrsOid.Equals(v.LrsOid) &&
                    IdentityPropertyOid.Equals(v.IdentityPropertyOid))
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(
                LrsOid,
                IdentityPropertyOid);
        }

        public override string ToString()
        {
            return string.Format("TnfNetwork: LrsOid = {0}, IdentityPropertyOid = {1}",
                LrsOid,
                IdentityPropertyOid);
        }
    }

    public class TnfSecondaryLrsIdentityManager : TableManager
    {
        private const string PrimaryKey = "lrs_oid, identity_property_oid";
        public static string TnfSecondaryLrsIdentityTableName = "tnf_secondary_lrs_identity";

        public TnfSecondaryLrsIdentityManager(GeoPackageDatabase db) : base(db, TnfSecondaryLrsIdentityTableName, GetColumnInfos(), PrimaryKey)
        {
        }

        protected override string[] Constraints()
        {
            return new[]
                {
                    String.Format("CONSTRAINT fk_tsli_lo FOREIGN KEY (lrs_oid) REFERENCES {0}(oid)", TnfSecondaryLrsManager.TnfSecondaryLrsTableName)
                };
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "lrs_oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "identity_property_oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
            };
        }

        public void Add(TnfSecondaryLrsIdentity tnfSecondaryLrsIdentity)
        {
            Add(new object[]
                {
                    tnfSecondaryLrsIdentity.LrsOid,
                    tnfSecondaryLrsIdentity.IdentityPropertyOid
                });
        }

        public List<TnfSecondaryLrsIdentity> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Delete(int lrsOid, int identityPropertyOid)
        {
            return Delete(new object[] { lrsOid, identityPropertyOid });
        }

        private static TnfSecondaryLrsIdentity ReadObject(IDataRecord reader)
        {
            var tnfSecondaryLrsIdentity = new TnfSecondaryLrsIdentity();

            tnfSecondaryLrsIdentity.LrsOid = reader["lrs_oid"].FromDbString();
            tnfSecondaryLrsIdentity.IdentityPropertyOid = reader["identity_property_oid"].FromDbString();

            return tnfSecondaryLrsIdentity;
        }

    }
}
