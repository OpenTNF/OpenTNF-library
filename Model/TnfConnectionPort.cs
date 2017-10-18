using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfConnectionPort
    {
        string LinkSequenceOid { get; }
        int PortNumber { get; }
        double Distance { get; }
        string NodeOid { get; }
        int NodePortNumber { get; }
    }

    public class TnfConnectionPort : ITnfConnectionPort
    {
        private const double Tolerance = 0.000000001;

        public const string LinkSequenceOidFieldName = "link_sequence_oid";
        public const string PortNumberFieldName = "port_number";
        public const string DistanceFieldName = "distance";
        public const string NodeOidFieldName = "node_oid";
        public const string NodePortNumberFieldName = "node_port_number";

        public string LinkSequenceOid { get; set; }
        public int PortNumber { get; set; }
        public double Distance { get; set; }
        public string NodeOid { get; set; }
        public int NodePortNumber { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TnfConnectionPort)
            {
                var v = obj as TnfConnectionPort;

                if (LinkSequenceOid == v.LinkSequenceOid &&
                    PortNumber == v.PortNumber &&
                    Math.Abs(Distance - v.Distance) < Tolerance &&
                    NodeOid == v.NodeOid &&
                    NodePortNumber == v.NodePortNumber)
                {
                    return true;
                }
            }

            return false;
        }


        public override int GetHashCode()
        {
            return Hashing.RsHash(
                LinkSequenceOid,
                PortNumber,
                Distance,
                NodeOid,
                NodePortNumber);
        }

        public override string ToString()
        {
            return String.Format("TnfConnectionPort: LinkSequenceOid = {0}, PortNumber = {1}, Distance = {2}, NodeOid = {3}, NodePortNumber = {4}",
                LinkSequenceOid,
                PortNumber,
                Distance,
                NodeOid,
                NodePortNumber);
        }
    }

    public class TnfConnectionPortManager : TableManager
    {
        private const string PrimaryKey = "link_sequence_oid, port_number, node_oid, node_port_number";
        public static string TnfConnectionPortTableName = "tnf_connection_port";

        public TnfConnectionPortManager(GeoPackageDatabase db) : base(db,TnfConnectionPortTableName, GetColumnInfos(),PrimaryKey)
        {
        }

        protected override string[] Indices()
        {
            return new[]
            {
                String.Format("CREATE INDEX IDX_tnf_connection_port_node_oid ON {0}({1})", TnfConnectionPortTableName, "node_oid"),
                String.Format("CREATE INDEX IDX_tnf_connection_port_link_sequence_oid ON {0}({1})", TnfConnectionPortTableName, "link_sequence_oid")
            };
        }
        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "link_sequence_oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "port_number",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "distance",
                    SqlType = "DOUBLE NOT NULL",
                    DataType = Type.GetType("System.Double")
                },
                new ColumnInfo
                {
                    Name = "node_oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "node_port_number",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                }
            };
        }

        public void Add(TnfConnectionPort tnfConnectionPort)
        {
            Add(new object[]
                {
                    tnfConnectionPort.LinkSequenceOid,
                    tnfConnectionPort.PortNumber,
                    tnfConnectionPort.Distance,
                    tnfConnectionPort.NodeOid,
                    tnfConnectionPort.NodePortNumber
                });
        }

        public List<TnfConnectionPort> GetByLinkSequence(string oid)
        {
            var tnfConnectionPortList = new List<TnfConnectionPort>();

            using (var command = Db.Command)
            {
                command.CommandText = string.Format(
                    "SELECT * FROM {0} WHERE link_sequence_oid = '{1}'",
                    TnfConnectionPortTableName,
                    oid);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tnfConnectionPortList.Add(ReadObject(reader));
                    }
                }
            }

            return tnfConnectionPortList;
        }

        public TnfConnectionPort GetByLinkSequence(string linkSequenceOid, int portNumber)
        {
            TnfConnectionPort tnfConnectionPort = null;
            using (var command = Db.Command)
            {
                command.CommandText = string.Format(
                    "SELECT * FROM {0} WHERE link_sequence_oid = '{1}' and port_number = {2}",
                    TnfConnectionPortTableName,
                    linkSequenceOid,
                    portNumber);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (tnfConnectionPort == null)
                        {
                            tnfConnectionPort = ReadObject(reader);
                        }
                        else
                        {
                            throw new OpenTnfException(string.Format("More than one port found for LinkSequence {0} and port number {1}.",
                                linkSequenceOid, portNumber));
                        }
                    }
                }
            }
            return tnfConnectionPort;
        }

        public List<TnfConnectionPort> GetByPort(string oid)
        {
            var tnfConnectionPortList = new List<TnfConnectionPort>();

            using (var command = Db.Command)
            {
                command.CommandText = string.Format("SELECT * FROM {0} WHERE node_oid = '{1}'", TnfConnectionPortTableName, oid);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tnfConnectionPortList.Add(ReadObject(reader));
                }
            }

            return tnfConnectionPortList;
        }

        public List<TnfConnectionPort> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(TnfConnectionPort tnfConnectionPort)
        {
            return Update(new object[]
                {
                    tnfConnectionPort.LinkSequenceOid,
                    tnfConnectionPort.PortNumber,
                    tnfConnectionPort.Distance,
                    tnfConnectionPort.NodeOid,
                    tnfConnectionPort.NodePortNumber
                });
        }

        public int Delete(TnfConnectionPort tnfConnectionPort)
        {
            return Delete(new object[]
            {
                tnfConnectionPort.LinkSequenceOid, 
                tnfConnectionPort.PortNumber, 
                tnfConnectionPort.NodeOid, 
                tnfConnectionPort.NodePortNumber
            });
        }

        private static TnfConnectionPort ReadObject(IDataRecord reader)
        {
            var tnfConnectionPort = new TnfConnectionPort();

            tnfConnectionPort.LinkSequenceOid = reader["link_sequence_oid"].FromDbString();
            tnfConnectionPort.PortNumber = (int) reader["port_number"].ToInt32();
            tnfConnectionPort.Distance = (double) reader["distance"].ToDouble();
            tnfConnectionPort.NodeOid = reader["node_oid"].FromDbString();
            tnfConnectionPort.NodePortNumber = (int) reader["node_port_number"].ToInt32();

            return tnfConnectionPort;
        }
    }
}