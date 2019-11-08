using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OpenTNF.Library.Model
{
    public interface ITnfNetworkReference
    {
        string PropertyOid { get; }
        int? NetworkReferenceType { get; }
        string NetworkElementRef { get; }
        int? ApplicableDirection { get; }
        int? ApplicableSide { get; }
        int? SeqNo { get; }
        string TurnOidLinearElementFrom { get; }
        int? TurnFromDirection { get; }
        string TurnOidLinearElementTo { get; }
        int? TurnToDirection { get; }
        double? Measure1 { get; }
        double? Measure2 { get; }
        bool? IsPreferred { get; }
        string Lanecode { get; }
        int? LinkRole { get; }
        bool? IsHost { get; }
    }

    public class TnfNetworkReference : ITnfNetworkReference
    {
        public string PropertyOid { get; set; }
        public int? NetworkReferenceType { get; set; }
        public string NetworkElementRef { get; set; }
        public int? ApplicableDirection { get; set; }
        public int? ApplicableSide { get; set; }
        public int? SeqNo { get; set; }
        public string TurnOidLinearElementFrom { get; set; }
        public int? TurnFromDirection { get; set; }
        public string TurnOidLinearElementTo { get; set; }
        public int? TurnToDirection { get; set; }
        public double? Measure1 { get; set; }
        public double? Measure2 { get; set; }
        public bool? IsPreferred { get; set; }
        public string Lanecode { get; set; }
        public int? LinkRole { get; set; }
        public bool? IsHost { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TnfNetworkReference)
            {
                var v = obj as TnfNetworkReference;

                if (PropertyOid == v.PropertyOid &&
                    NetworkReferenceType == v.NetworkReferenceType &&
                    NetworkElementRef == v.NetworkElementRef &&
                    ApplicableDirection == v.ApplicableDirection &&
                    ApplicableSide == v.ApplicableSide &&
                    SeqNo == v.SeqNo &&
                    TurnOidLinearElementFrom == v.TurnOidLinearElementFrom &&
                    TurnFromDirection == v.TurnFromDirection &&
                    TurnOidLinearElementTo == v.TurnOidLinearElementTo &&
                    TurnToDirection == v.TurnToDirection &&
                    Measure1 == v.Measure1 &&
                    Measure2 == v.Measure2 &&
                    IsPreferred == v.IsPreferred &&
                    Lanecode == v.Lanecode &&
                    LinkRole == v.LinkRole &&
                    IsHost == v.IsHost)
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
                NetworkReferenceType,
                NetworkElementRef,
                ApplicableDirection,
                ApplicableSide,
                SeqNo,
                TurnOidLinearElementFrom,
                TurnFromDirection,
                TurnOidLinearElementTo,
                TurnToDirection,
                Measure1,
                Measure2,
                IsPreferred,
                Lanecode,
                LinkRole,
                IsHost);
        }

        public override string ToString()
        {
            return String.Format(
                "TnfNetworkReference: PropertyOid = {0}, NetworkReferenceType = {1}, NetworkElementRef = {2}, " +
                "ApplicableDirection = {3}, ApplicableSide = {4}, SeqNo = {5}, TurnOidLinearElementFrom = {6}, TurnFromDirection = {7}, " +
                "TurnOidLinearElementTo = {8}, TurnToDirection = {9}, Measure1 = {10}, Measure2 = {11}, IsPreferred = {12}, " +
                "Lanecode = {13}, LinkRole = {14}, IsHost = {15}",
                PropertyOid,
                NetworkReferenceType,
                NetworkElementRef,
                ApplicableDirection,
                ApplicableSide,
                SeqNo,
                TurnOidLinearElementFrom,
                TurnFromDirection,
                TurnOidLinearElementTo,
                TurnToDirection,
                Measure1,
                Measure2,
                IsPreferred,
                Lanecode,
                LinkRole,
                IsHost);
        }
    }

    public class TnfNetworkReferenceManager : TableManager
    {
        public static string TnfNetworkReferenceTableName = "tnf_network_reference";

        public TnfNetworkReferenceManager(GeoPackageDatabase db) : base(db, TnfNetworkReferenceTableName, GetColumnInfos(),null)
        {
        }

        protected override string[] Constraints()
        {
            return new[]
                {
                    String.Format("CONSTRAINT fk_tnrm_po FOREIGN KEY (property_oid) REFERENCES {0}(oid)",TnfPropertyManager.TnfPropertyTableName),
                };
        }

        protected override string[] Indices()
        {
            return new[]
            {
                String.Format("CREATE INDEX IDX_tnf_network_reference_property_oid ON {0}({1})", TnfNetworkReferenceTableName, "property_oid"),
                String.Format("CREATE INDEX IDX_tnf_network_reference_network_element_ref ON {0}({1})", TnfNetworkReferenceTableName, "network_element_ref")
            };
        }


        protected override string[] Triggers()
        {
            return new[]
                {
                    @"CREATE TRIGGER ""tnf_network_reference_insert1""
BEFORE INSERT ON ""tnf_network_reference""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'insert on table ''tnf_network_reference''
 violates constraint: ''applicable_direction'' must be between -1 and 1.')
WHERE (NEW.applicable_direction NOT BETWEEN -1 AND 1);
END;",
                    @"CREATE TRIGGER ""tnf_network_reference_update1""
BEFORE UPDATE OF ""applicable_direction"" ON ""tnf_network_reference""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'update on table ''tnf_network_reference''
 violates constraint: ''applicable_direction'' must be between -1 and 1.')
WHERE (NEW.applicable_direction NOT BETWEEN -1 AND 1);
END;",
                    @"CREATE TRIGGER ""tnf_network_reference_insert2""
BEFORE INSERT ON ""tnf_network_reference""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'insert on table ''tnf_network_reference''
 violates constraint: ''applicable_side'' must be between -1 and 3.')
WHERE (NEW.applicable_side NOT BETWEEN -1 AND 3);
END;",
                    @"CREATE TRIGGER ""tnf_network_reference_update2""
BEFORE UPDATE OF ""applicable_side"" ON ""tnf_network_reference""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'update on table ''tnf_network_reference''
 violates constraint: ''applicable_side'' must be between -1 and 3.')
WHERE (NEW.applicable_side NOT BETWEEN -1 AND 3);
END;",
                    @"CREATE TRIGGER ""tnf_network_reference_insert3""
BEFORE INSERT ON ""tnf_network_reference""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'insert on table ''tnf_network_reference''
 violates constraint: ''turn_oid_linear_element_from'' must be set when
 ''network_reference_type'' = 64.')
WHERE (NEW.network_reference_type = 64 AND NEW.turn_oid_linear_element_from IS NULL);
END;",
                    @"CREATE TRIGGER ""tnf_network_reference_update3""
BEFORE UPDATE OF ""turn_oid_linear_element_from"" ON ""tnf_network_reference""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'update on table ''tnf_network_reference''
 violates constraint: ''turn_oid_linear_element_from'' must be set when
 ''network_reference_type'' = 64.')
WHERE (NEW.network_reference_type = 64 AND NEW.turn_oid_linear_element_from IS NULL);
END;"
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
                    Name = "network_reference_type",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "network_element_ref",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "applicable_direction",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "applicable_side",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "seq_no",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "turn_oid_linear_element_from",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "turn_from_direction",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "turn_oid_linear_element_to",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "turn_to_direction",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "measure1",
                    SqlType = "DOUBLE",
                    DataType = Type.GetType("System.Double")
                },
                new ColumnInfo
                {
                    Name = "measure2",
                    SqlType = "DOUBLE",
                    DataType = Type.GetType("System.Double")
                },
                new ColumnInfo
                {
                    Name = "is_preferred",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Boolean")
                },
                new ColumnInfo
                {
                    Name = "lanecode",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "link_role",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "is_host",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Boolean")
                }
            };
        }

        public void Add(TnfNetworkReference tnfNetworkReference)
        {
            Add(new object[]
                {
                    tnfNetworkReference.PropertyOid,
                    tnfNetworkReference.NetworkReferenceType,
                    tnfNetworkReference.NetworkElementRef,
                    tnfNetworkReference.ApplicableDirection,
                    tnfNetworkReference.ApplicableSide,
                    tnfNetworkReference.SeqNo,
                    tnfNetworkReference.TurnOidLinearElementFrom,
                    tnfNetworkReference.TurnFromDirection,
                    tnfNetworkReference.TurnOidLinearElementTo,
                    tnfNetworkReference.TurnToDirection,
                    tnfNetworkReference.Measure1,
                    tnfNetworkReference.Measure2,
                    tnfNetworkReference.IsPreferred,
                    tnfNetworkReference.Lanecode,
                    tnfNetworkReference.LinkRole,
                    tnfNetworkReference.IsHost
                });
        }

        public List<TnfNetworkReference> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public List<TnfNetworkReference> Get(string propertyOid)
        {
            var tnfList = new List<TnfNetworkReference>();

            using (var command = Db.Command)
            {
                command.CommandText =
                    string.Format(
                        "SELECT * FROM {0} WHERE property_oid = '{1}'",
                        TnfNetworkReferenceTableName, propertyOid);

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

        public List<TnfNetworkReference> GetPage(int offset, int limit)
        {
            return GetPage(ReadObject, offset, limit);
        }

        private static TnfNetworkReference ReadObject(IDataRecord reader)
        {
            var tnfNetworkReference = new TnfNetworkReference();

            tnfNetworkReference.PropertyOid = reader["property_oid"].FromDbString();
            tnfNetworkReference.NetworkReferenceType = reader["network_reference_type"].ToInt32();
            tnfNetworkReference.NetworkElementRef = reader["network_element_ref"].FromDbString();
            tnfNetworkReference.ApplicableDirection = reader["applicable_direction"].ToInt32();
            tnfNetworkReference.ApplicableSide = reader["applicable_side"].ToInt32();
            tnfNetworkReference.SeqNo = reader["seq_no"].ToInt32();
            tnfNetworkReference.TurnOidLinearElementFrom = reader["turn_oid_linear_element_from"].FromDbString();
            tnfNetworkReference.TurnFromDirection = reader["turn_from_direction"].ToInt32();
            tnfNetworkReference.TurnOidLinearElementTo = reader["turn_oid_linear_element_to"].FromDbString();
            tnfNetworkReference.TurnToDirection = reader["turn_to_direction"].ToInt32();
            tnfNetworkReference.Measure1 = reader["measure1"].ToDouble();
            tnfNetworkReference.Measure2 = reader["measure2"].ToDouble();
            tnfNetworkReference.IsPreferred = reader["is_preferred"].ToBoolean();
            tnfNetworkReference.Lanecode = reader["lanecode"].FromDbString();
            tnfNetworkReference.LinkRole = reader["link_role"].ToInt32();
            tnfNetworkReference.IsHost = reader["is_host"].ToBoolean();

            return tnfNetworkReference;
        }
    }
}
