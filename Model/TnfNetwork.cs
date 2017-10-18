using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfNetwork
    {
        int Oid { get; }
        string TypeOfTransport { get; }
        string GeographicalName { get; }
    }

    public class TnfNetwork : ITnfNetwork
    {
        public int Oid { get; set; }
        public string TypeOfTransport { get; set; }
        public string GeographicalName { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TnfNetwork)
            {
                var v = obj as TnfNetwork;

                if (Oid == v.Oid &&
                    TypeOfTransport == v.TypeOfTransport)
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
                TypeOfTransport,
                GeographicalName);
        }

        public override string ToString()
        {
            return string.Format("TnfNetwork: Oid = {0}, TypeOfTransport = {1}, GeographicalName = {2}",
                Oid,
                TypeOfTransport,
                GeographicalName);
        }
    }

    public class TnfNetworkManager : TableManager
    {
        public static string TnfNetworkTableName = "tnf_network";

        public TnfNetworkManager(GeoPackageDatabase db) : base(db, TnfNetworkTableName, GetColumnInfos())
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
                    Name = "type_of_transport",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "geographical_name",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                }
            };
        }

        public void Add(TnfNetwork tnfNetwork)
        {
            Add(new object[]
                {
                    tnfNetwork.Oid.ToString(),
                    tnfNetwork.TypeOfTransport,
                    tnfNetwork.GeographicalName
                });
        }

        public TnfNetwork Get(int oid)
        {
            return Get(ReadObject, new object[] { oid });
        }

        public List<TnfNetwork> GetByMaxResult(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(TnfNetwork tnfNetwork)
        {
            return Update(new object[]
                {
                    tnfNetwork.Oid,
                    tnfNetwork.TypeOfTransport,
                    tnfNetwork.GeographicalName
                });
        }

        public int Delete(string oid)
        {
            return Delete(new object[] { oid });
        }

        private static TnfNetwork ReadObject(IDataRecord reader)
        {
            var tnfNetwork = new TnfNetwork();

            tnfNetwork.Oid = reader["oid"].ToInt();
            tnfNetwork.TypeOfTransport = reader["type_of_transport"].FromDbString();
            tnfNetwork.GeographicalName = reader["geographical_name"].FromDbString();

            return tnfNetwork;
        }
    }
}
