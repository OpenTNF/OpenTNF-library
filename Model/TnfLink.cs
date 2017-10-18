using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OpenTNF.Library.Model
{
    public interface ITnfLink
    {
        int? NetworkOid { get; }
        double? Length { get; }
        double? MeasureFrom { get; }
        double? MeasureTo { get; }
        string LinkSequenceOid { get; }
        DateTime? ValidFrom { get; }
        DateTime? ValidTo { get; }
        string NodeOidStart { get; }
        string NodeOidEnd { get; }
        string Lanecode { get; }
        string SuperLinkSequenceOid { get; }
        double? SuperMeasureFrom { get; }
        double? SuperMeasureTo { get; }
        int? Direction { get; }
        int? TopologyLevelOid { get; }
    }

    public class TnfLink : ITnfLink
    {
        public const string NetworkOidFieldName = "network_oid";
        public const string ValidFromFieldName = "valid_from";
        public const string ValidToFieldName = "valid_to";
        public const string MeasureFromFieldName = "measure_from";
        public const string MeasureToFieldName = "measure_to";
        public const string NodeOidStartFieldName = "node_oid_start";
        public const string NodeOidEndFieldName = "node_oid_end";
        public const string LengthFieldName = "length";
        public const string LinkSequenceOidFieldName = "link_sequence_oid";
        public const string LaneCodeFieldName = "lanecode";
        public const string SuperLinkSequenceOidFieldName = "super_link_sequence_oid";
        public const string SuperMeasureFromFieldName = "super_measure_from";
        public const string SuperMeasureToFieldName = "super_measure_to";
        public const string DirectionFieldName = "direction";
        public const string TopologyLevelOidFieldName = "topology_level_oid";

        public int? NetworkOid { get; set; }
        public double? Length { get; set; }
        public double? MeasureFrom { get; set; }
        public double? MeasureTo { get; set; }
        public string LinkSequenceOid { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string NodeOidStart { get; set; }
        public string NodeOidEnd { get; set; }
        public string Lanecode { get; set; }
        public string SuperLinkSequenceOid { get; set; }
        public double? SuperMeasureFrom { get; set; }
        public double? SuperMeasureTo { get; set; }
        public int? Direction { get; set; }
        public int? TopologyLevelOid { get; set; }
        
        public override bool Equals(object obj)
        {
            if (obj is TnfLink)
            {
                var v = obj as TnfLink;

                if (NetworkOid == v.NetworkOid &&
                    Length == v.Length &&
                    MeasureFrom == v.MeasureFrom &&
                    MeasureTo == v.MeasureTo &&
                    LinkSequenceOid == v.LinkSequenceOid &&
                    ValidFrom == v.ValidFrom &&
                    ValidTo == v.ValidTo &&
                    NodeOidStart == v.NodeOidStart &&
                    NodeOidEnd == v.NodeOidEnd &&
                    Lanecode == v.Lanecode &&
                    SuperLinkSequenceOid == v.SuperLinkSequenceOid &&
                    SuperMeasureFrom == v.SuperMeasureFrom &&
                    SuperMeasureTo == v.SuperMeasureTo &&
                    Direction == v.Direction &&
                    TopologyLevelOid == v.TopologyLevelOid)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(
                NetworkOid,
                Length,
                MeasureFrom,
                MeasureTo,
                LinkSequenceOid,
                ValidFrom,
                ValidTo,
                NodeOidStart,
                NodeOidEnd,
                Lanecode,
                SuperLinkSequenceOid,
                SuperMeasureFrom,
                SuperMeasureTo,
                Direction,
                TopologyLevelOid);
        }

        public override string ToString()
        {
            return string.Format(
                "TnfLink: NetworkOid = {1}, Length = {2}, MeasureFrom = {4}, " +
                "MeasureTo = {5}, LinkSequenceOid = {6}, ValidFrom = {7}, ValidTo = {8}, NodeOidStart = {9}, NodeOidEnd = {10}, " +
                "Lanecode = {11}, SuperLinkSequenceOid = {12}, SuperMeasureFrom = {13}, SuperMeasureTo = {14}, Direction = {15}, " +
                "TopologyLevelOid = {16}",
                null,
                NetworkOid,
                Length,
                null,
                MeasureFrom,
                MeasureTo,
                LinkSequenceOid,
                ValidFrom,
                ValidTo,
                NodeOidStart,
                NodeOidEnd,
                Lanecode,
                SuperLinkSequenceOid,
                SuperMeasureFrom,
                SuperMeasureTo,
                Direction,
                TopologyLevelOid);
        }
    }

    public class TnfLinkManager : TableManager
    {
        public const string TnfLinkTableName = "tnf_link";

        public TnfLinkManager(GeoPackageDatabase db) : base(db, TnfLinkTableName, GetColumnInfos(db.HasTopologyLevel))
        {
        }
        protected override string[] Indices()
        {
            return new[]
            {
                String.Format("CREATE INDEX IDX_tnf_link_link_sequence_oid ON {0}({1})", TnfLinkTableName, "link_sequence_oid")
            };
        }
        protected override string[] Constraints()
        {
            return null;
        }

        private static ColumnInfo[] GetColumnInfos(bool hasTopologyLevel)
        {
            var ret = new[]
            {
                new ColumnInfo {Name = TnfLink.NetworkOidFieldName, SqlType = "TEXT", DataType = Type.GetType("System.String")},
                new ColumnInfo
                    {
                        Name = TnfLink.LengthFieldName,
                        SqlType = "DOUBLE",
                        DataType = Type.GetType("System.Double")
                    },
                new ColumnInfo
                    {
                        Name = TnfLink.MeasureFromFieldName,
                        SqlType = "DOUBLE",
                        DataType = Type.GetType("System.Double")
                    },
                new ColumnInfo
                    {
                        Name = TnfLink.MeasureToFieldName,
                        SqlType = "DOUBLE",
                        DataType = Type.GetType("System.Double")
                    },
                new ColumnInfo
                    {
                        Name = TnfLink.LinkSequenceOidFieldName,
                        SqlType = "TEXT",
                        DataType = Type.GetType("System.String")
                    },
                new ColumnInfo {Name = TnfLink.ValidFromFieldName, SqlType = "DATETIME", DataType = Type.GetType("System.DateTime")},
                new ColumnInfo {Name = TnfLink.ValidToFieldName, SqlType = "DATETIME", DataType = Type.GetType("System.DateTime")},
                new ColumnInfo {Name = TnfLink.NodeOidStartFieldName, SqlType = "TEXT", DataType = Type.GetType("System.String")},
                new ColumnInfo {Name = TnfLink.NodeOidEndFieldName, SqlType = "TEXT", DataType = Type.GetType("System.String")}
            };

            if (hasTopologyLevel)
            {
                ret = ret.Concat(new[]
                {
                    new ColumnInfo {Name = TnfLink.LaneCodeFieldName, SqlType = "TEXT", DataType = Type.GetType("System.String")},
                    new ColumnInfo {Name = TnfLink.SuperLinkSequenceOidFieldName, SqlType = "TEXT", DataType = Type.GetType("System.String")},
                    new ColumnInfo {Name = TnfLink.SuperMeasureFromFieldName, SqlType = "DOUBLE", DataType = Type.GetType("System.Double")},
                    new ColumnInfo {Name = TnfLink.SuperMeasureToFieldName, SqlType = "DOUBLE", DataType = Type.GetType("System.Double")},
                    new ColumnInfo {Name = TnfLink.DirectionFieldName, SqlType = "INTEGER", DataType = Type.GetType("System.Int32")},
                    new ColumnInfo {Name = TnfLink.TopologyLevelOidFieldName, SqlType = "TEXT", DataType = Type.GetType("System.String")}
                }).ToArray();
            }
            return ret;
        }

        public void Add(TnfLink tnfLink)
        {
            var obj = new object[]
            {
                tnfLink.NetworkOid?.ToString(),
                tnfLink.Length,
                tnfLink.MeasureFrom,
                tnfLink.MeasureTo,
                tnfLink.LinkSequenceOid,
                tnfLink.ValidFrom?.Date,
                tnfLink.ValidTo?.Date,
                tnfLink.NodeOidStart,
                tnfLink.NodeOidEnd
            };

            if (Db.HasTopologyLevel)
            {
                obj = obj.Concat(new object[]
                {
                    tnfLink.Lanecode,
                    tnfLink.SuperLinkSequenceOid,
                    tnfLink.SuperMeasureFrom,
                    tnfLink.SuperMeasureTo,
                    tnfLink.Direction,
                    tnfLink.TopologyLevelOid
                }).ToArray();
            }

            Add(obj);
        }

        public List<TnfLink> Get(int maxResults)
        {
            return Get(reader => ReadObject(reader, Db.HasTopologyLevel), maxResults);
        }

        public List<TnfLink> GetPage(int offset, int limit)
        {
            return GetPage(reader => ReadObject(reader, Db.HasTopologyLevel), offset, limit);
        }

        public List<TnfLink> GetByLinkSequence(string linkSequenceOid)
        {
            var tnfLinkList = new List<TnfLink>();

            using (var command = Db.Command)
            {
                command.CommandText =
                    string.Format(
                        "SELECT * FROM {0} WHERE link_sequence_oid = '{1}' ORDER BY measure_from",
                        TnfLinkTableName, linkSequenceOid);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tnfLinkList.Add(ReadObject(reader, Db.HasTopologyLevel));
                    }
                }
            }

            return tnfLinkList;
        }

        public int Update(TnfLink tnfLink)
        {
            var obj = new object[]
            {
                tnfLink.NetworkOid?.ToString(),
                tnfLink.Length,
                tnfLink.MeasureFrom,
                tnfLink.MeasureTo,
                tnfLink.LinkSequenceOid,
                tnfLink.ValidFrom?.Date,
                tnfLink.ValidTo?.Date,
                tnfLink.NodeOidStart,
                tnfLink.NodeOidEnd
            };

            if (Db.HasTopologyLevel)
            {
                obj = obj.Concat(new object[]
                {
                    tnfLink.Lanecode,
                    tnfLink.SuperLinkSequenceOid,
                    tnfLink.SuperMeasureFrom,
                    tnfLink.SuperMeasureTo,
                    tnfLink.Direction,
                    tnfLink.TopologyLevelOid
                }).ToArray();
            }

            return Update(obj);
        }

        public int Delete(string oid)
        {
            return Delete(new object[] { oid });
        }

        public int Count(string oid)
        {
            return Count(new object[] { oid });
        }

        private static TnfLink ReadObject(IDataRecord reader, bool hasTopologyLevel)
        {
            var tnfLink = new TnfLink();
            
            tnfLink.NetworkOid = reader["network_oid"].ToInt32();
            tnfLink.Length = reader["length"].ToDouble();
            tnfLink.MeasureFrom = reader["measure_from"].ToDouble();
            tnfLink.MeasureTo = reader["measure_to"].ToDouble();
            tnfLink.LinkSequenceOid = reader["link_sequence_oid"].FromDbString();
            tnfLink.ValidFrom = reader["valid_from"].ToDateTime();
            tnfLink.ValidTo = reader["valid_to"].ToDateTime();
            tnfLink.NodeOidStart = reader["node_oid_start"].FromDbString();
            tnfLink.NodeOidEnd = reader["node_oid_end"].FromDbString();

            if (hasTopologyLevel)
            {
                tnfLink.Lanecode = reader["lanecode"].FromDbString();
                tnfLink.SuperLinkSequenceOid = reader["super_link_sequence_oid"].FromDbString();
                tnfLink.SuperMeasureFrom = reader["super_measure_from"].ToDouble();
                tnfLink.SuperMeasureTo = reader["super_measure_to"].ToDouble();
                tnfLink.Direction = reader["direction"].ToInt32();
                tnfLink.TopologyLevelOid = reader["topology_level_oid"].ToInt32();
            }
            
            return tnfLink;
        }
    }
}
