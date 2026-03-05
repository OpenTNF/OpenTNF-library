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
        DateTime? ValidFrom { get; }
        DateTime? ValidTo { get; }
    }

    public class TnfNode : ITnfNode
    {
        public string Oid { get; set; }
        public string Vid { get; set; }
        public int? NetworkOid { get; set; }
        public int? NextFreePortNumber { get; set; }

        public byte[] Geometry { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

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
                NextFreePortNumber,
                ValidFrom,
                ValidTo);
        }

        public override string ToString()
        {
            return String.Format("TnfNode: Oid = {0}, Vid = {1}, Geometry = {2}, NextFreePortNumber = {3}, NetworkOid = {4}, ValidFrom = {5}, ValidTo = {6}",
                Oid,
                Vid,
                Geometry,
                NextFreePortNumber,
                NetworkOid,
                ValidFrom,
                ValidTo);
        }
    }

    public class TnfNodeManager : TableManager
    {
        public const string TnfNodeTableName = "tnf_node";

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
                    DataType = Type.GetType("System.String"),
                },
                new ColumnInfo
                {
                    Name = "network_oid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String"),
                },
                new ColumnInfo
                {
                    Name = "geometry",
                    SqlType = "POINT",
                    DataType = Type.GetType("System.Byte[]"),
                    HasZ = true,
                },
                new ColumnInfo
                {
                    Name = "next_free_port_number",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32"),
                },
                new ColumnInfo
                {
                    Name = "valid_from",
                    SqlType = "DATE",
                    DataType = Type.GetType("System.DateTime"),
                    Requirement = ColumnRequirement.OptionalReadOnly,
                },
                new ColumnInfo
                {
                    Name = "valid_to",
                    SqlType = "DATE",
                    DataType = Type.GetType("System.DateTime"),
                    Requirement = ColumnRequirement.OptionalReadOnly,
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
                    tnfNode.NextFreePortNumber,
                    tnfNode.ValidFrom.ToDateString(),
                    tnfNode.ValidTo.ToDateString()
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

        public List<string> GetExistingOids(List<string> oids)
        {
            List<string> existingOids = new List<string>();
            if (oids.Count == 0)
            {
                return existingOids;
            }
            using (var command = Db.Command)
            {
                command.CommandText =
                    $"SELECT oid FROM {TnfNodeTableName} " +
                    $"WHERE oid in ({string.Join(",", oids.Select(oid => $"'{oid}'"))})";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existingOids.Add(reader["oid"].FromDbString());
                    }
                }
            }
            return existingOids;
        }

        public int Update(TnfNode tnfNode)
        {
            return Update(new object[]
                {
                    tnfNode.Oid,
                    tnfNode.Vid,
                    tnfNode.NetworkOid?.ToString(),
                    tnfNode.Geometry,
                    tnfNode.NextFreePortNumber,
                    tnfNode.ValidFrom.ToDateString(),
                    tnfNode.ValidTo.ToDateString()
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
            tnfNode.ValidFrom = reader.ReadIfExists("valid_from").ToDateTime();
            tnfNode.ValidTo = reader.ReadIfExists("valid_to").ToDateTime();

            return tnfNode;
        }
    }
}
