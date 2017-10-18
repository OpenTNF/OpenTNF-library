using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfNode
    {
        string Oid { get; }
        string Vid { get; }
        int? NetworkOid { get; }
        byte[] Geometry { get; }
        int? NextFreePortNumber { get; }
    }

    public class TnfNode : ITnfNode
    {
        public string Oid { get; set; }
        public string Vid { get; set; }
        public int? NetworkOid { get; set; }
        public int? NextFreePortNumber { get; set; }

        public byte[] Geometry { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TnfNode)
            {
                var v = obj as TnfNode;

                if (Oid == v.Oid &&
                    Vid == v.Vid &&
                    NetworkOid == v.NetworkOid
                    && NextFreePortNumber == v.NextFreePortNumber)
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
                NetworkOid,
                Geometry,
                NextFreePortNumber);
        }

        public override string ToString()
        {
            return String.Format("TnfNode: Oid = {0}, Vid = {1}, Geometry = {2}, NextFreePortNumber = {3}, NetworkOid = {4}",
                Oid,
                Vid,
                Geometry,
                NextFreePortNumber,
                NetworkOid);
        }
    }

    public class TnfNodeManager : TableManager
    {
        public static string TnfNodeTableName = "tnf_node";

        public TnfNodeManager(GeoPackageDatabase db) : base(db, TnfNodeTableName, GetColumnInfos())
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
                    Name = "vid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "network_oid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "geometry",
                    SqlType = "Point",
                    DataType = Type.GetType("System.Byte[]")
                },
                new ColumnInfo
                {
                    Name = "next_free_port_number",
                    SqlType = "INT",
                    DataType = Type.GetType("System.Int32")
                }
            };
        }

        public void Add(TnfNode tnfNode)
        {
            Add(new object[]
                {
                    tnfNode.Oid,
                    tnfNode.Vid,
                    tnfNode.NetworkOid?.ToString(),
                    tnfNode.Geometry,
                    tnfNode.NextFreePortNumber
                });
        }

        public TnfNode Get(string oid)
        {
            return Get(ReadObject, new object[] { oid });
        }

        public List<TnfNode> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public List<TnfNode> GetPage(int offset, int limit)
        {
            return GetPage(ReadObject, offset, limit);
        }

        public int Update(TnfNode tnfNode)
        {
            return Update(new object[]
                {
                    tnfNode.Oid,
                    tnfNode.Vid,
                    tnfNode.NetworkOid?.ToString(),
                    tnfNode.Geometry,
                    tnfNode.NextFreePortNumber
                });
        }

        public int Delete(string oid)
        {
            return Delete(new object[] { oid });
        }

        public int Count(string oid)
        {
            return Count(new object[] { oid });
        }

        private TnfNode ReadObject(IDataReader reader)
        {
            var tnfNode = new TnfNode();

            tnfNode.Oid = reader["oid"].FromDbString();
            tnfNode.Vid = reader["vid"].FromDbString();
            tnfNode.NetworkOid = (int)reader["network_oid"].ToInt32();
            tnfNode.NextFreePortNumber = reader["next_free_port_number"].ToInt32();
            var geometry = reader["geometry"];
            if (!(geometry is DBNull))
            {
                tnfNode.Geometry = (byte[])geometry;
            }

            return tnfNode;
        }
    }
}
