using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfLinkSequence
    {
        string Oid { get; }
        string Vid { get; }
        int? NetworkOid { get; }
        int? NextFreePortNumber { get; }
        byte[] Geometry { get; }
    }

    public class TnfLinkSequence : ITnfLinkSequence
    {

        public string Oid { get; set; }
        public string Vid { get; set; }
        public int? NetworkOid { get; set; }
        public int? NextFreePortNumber { get; set; }
        public byte[] Geometry { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TnfLinkSequence)
            {
                var v = obj as TnfLinkSequence;

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
                NextFreePortNumber);
        }

        public override string ToString()
        {
            return String.Format("TnfLinkSequence: Oid = {0}, Vid = {1}, NextFreePortNumber = {2}",
                Oid,
                Vid,
                NextFreePortNumber);
        }
    }

    public class TnfLinkSequenceManager : TableManager
    {
        public static string TnfLinkSequenceTableName = "tnf_link_sequence";

        public TnfLinkSequenceManager(GeoPackageDatabase db) : base(db, TnfLinkSequenceTableName, GetColumnInfos())
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
                    Name = "next_free_port_number",
                    SqlType = "INT",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "geometry",
                    SqlType = "LineString",
                    DataType = Type.GetType("System.Byte[]")
                }
            };
        }

        public void Add(TnfLinkSequence tnfLinkSequence)
        {
            Add(new object[]
                {
                    tnfLinkSequence.Oid,
                    tnfLinkSequence.Vid,
                    tnfLinkSequence.NetworkOid?.ToString(),
                    tnfLinkSequence.NextFreePortNumber,
                    tnfLinkSequence.Geometry
                });
        }

        public TnfLinkSequence Get(string oid)
        {
            return Get(ReadObject, new object[] { oid });
        }

        public List<TnfLinkSequence> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public List<TnfLinkSequence> GetPage(int offset, int limit)
        {
            return GetPage(ReadObject, offset, limit);
        }

        public int Update(TnfLinkSequence tnfLinkSequence)
        {
            return Update(new object[]
                {
                    tnfLinkSequence.Oid,
                    tnfLinkSequence.Vid,
                    tnfLinkSequence.NetworkOid?.ToString(),
                    tnfLinkSequence.NextFreePortNumber,
                    tnfLinkSequence.Geometry
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

        private static TnfLinkSequence ReadObject(IDataReader reader)
        {
            var tnfLinkSequence = new TnfLinkSequence();

            tnfLinkSequence.Oid = reader["oid"].FromDbString();
            tnfLinkSequence.Vid = reader["vid"].FromDbString();
            tnfLinkSequence.NetworkOid = (int)reader["network_oid"].ToInt32();
            tnfLinkSequence.NextFreePortNumber = reader["next_free_port_number"].ToInt32();
            
            object geometry = reader["geometry"];
            if (!(geometry is DBNull))
            {
                tnfLinkSequence.Geometry = (byte[])geometry;
            }

            return tnfLinkSequence;
        }
    }
}
