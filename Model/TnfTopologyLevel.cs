using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfTopologyLevel
    {
        int Oid { get; }
        int NetworkOid { get; set; }
        string TopologyLevel { get; }
        string TopologyLevelDescr { get; }
    }

    public class TnfTopologyLevel : ITnfTopologyLevel
    {
        public int Oid { get; set; }
        public int NetworkOid { get; set; }
        public string TopologyLevel { get; set; }
        public string TopologyLevelDescr { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TnfTopologyLevel)
            {
                var v = obj as TnfTopologyLevel;

                if (Oid == v.Oid &&
                    NetworkOid == v.NetworkOid &&
                    TopologyLevel == v.TopologyLevel &&
                    TopologyLevelDescr == v.TopologyLevelDescr)
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
                NetworkOid,
                TopologyLevel,
                TopologyLevelDescr);
        }

        public override string ToString()
        {
            return String.Format("TnfTopologyLevel: Oid = {0}, NetworkOid = {1}, TopologyLevel = {2}, TopologyLevelDescr = {3}",
                Oid,
                NetworkOid,
                TopologyLevel,
                TopologyLevelDescr);
        }
    }

    public class TnfTopologyLevelManager : TableManager
    {
        public static string TnfTopologyLevelTableName = "tnf_topology_level";

        private static readonly string[] m_primaryKey = new string[] { "oid", "network_oid" };


        public TnfTopologyLevelManager(GeoPackageDatabase db) : base(db, TnfTopologyLevelTableName, GetColumnInfos(), string.Join(",", m_primaryKey))
        {
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
                    Name = "network_oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String"),
                    HandleMissing = true
                },
                new ColumnInfo
                {
                    Name = "topology_level",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "topology_level_descr",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                }
            };
        }

        public void Add(TnfTopologyLevel tnfTopologyLevel)
        {
            Add(new object[]
                {
                    tnfTopologyLevel.Oid,
                    tnfTopologyLevel.NetworkOid,
                    tnfTopologyLevel.TopologyLevel,
                    tnfTopologyLevel.TopologyLevelDescr
                });
        }

        public TnfTopologyLevel Get(int oid, int networkOid)
        {
            return Get(ReadObject, new object[] { oid, networkOid });
        }

        public List<TnfTopologyLevel> GetAll(int limit)
        {
            return Get(ReadObject, limit);
        }

        public int Update(TnfTopologyLevel tnfTopologyLevel)
        {
            return Update(new object[]
                {
                    tnfTopologyLevel.Oid,
                    tnfTopologyLevel.NetworkOid,
                    tnfTopologyLevel.TopologyLevel,
                    tnfTopologyLevel.TopologyLevelDescr
                });
        }

        public int Delete(int oid, int networkOid)
        {
            return Delete(new object[] { oid, networkOid });
        }

        private static TnfTopologyLevel ReadObject(IDataRecord reader)
        {
            var tnfTopologyLevel = new TnfTopologyLevel();
            tnfTopologyLevel.Oid = reader["oid"].ToInt();
            tnfTopologyLevel.NetworkOid = reader.ReadIfExists("network_oid").ToInt32() ?? -1;
            tnfTopologyLevel.TopologyLevel = reader["topology_level"].FromDbString();
            tnfTopologyLevel.TopologyLevelDescr = reader["topology_level_descr"].FromDbString();

            return tnfTopologyLevel;
        }
    }
}