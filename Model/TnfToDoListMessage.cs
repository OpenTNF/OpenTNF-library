using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfToDoListMessage
    {
        string Oid { get; }
        long Severity { get; }
        string Message { get;  }
    }

    public class TnfToDoListMessage : ITnfToDoListMessage
    {
        public string Oid { get; set; }
        public long Severity { get; set; }
        public string Message { get; set; }
    }


    public class TnfToDoListMessageManager : TableManager
    {
        private const string PrimaryKey = "oid";
        public static string TnfToDoListMessageTableName = "tnf_todo_list_message";

        public TnfToDoListMessageManager(GeoPackageDatabase db) : base(db, TnfToDoListMessageTableName, GetColumnInfos(),PrimaryKey)
        {
        }

        protected override string[] Constraints()
        {
            return null;
        }

        public void Add(TnfToDoListMessage tnfToDoListMessage)
        {
            Add(new object[]
                {
                    tnfToDoListMessage.Oid,
                    tnfToDoListMessage.Severity,
                    tnfToDoListMessage.Message
                });
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
                    Name = "severity",
                    SqlType = "Integer NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "message",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                }
            };
        }

        public TnfToDoListMessage Get(string id)
        {
            return Get(ReadObject, new object[] { id });
        }

        public List<TnfToDoListMessage> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(TnfToDoListMessage tnfToDoListMessage)
        {
            return Update(new object[]
                {
                    tnfToDoListMessage.Oid,
                    tnfToDoListMessage.Severity,
                    tnfToDoListMessage.Message
                });
        }

        public int Delete(string id)
        {
            return Delete(new object[] { id });
        }

        private static TnfToDoListMessage ReadObject(IDataRecord reader)
        {
            var tnfToDoListMessage = new TnfToDoListMessage();

            tnfToDoListMessage.Oid = reader["oid"].FromDbString();
            tnfToDoListMessage.Severity = long.Parse(reader["severity"].FromDbString());
            tnfToDoListMessage.Message = reader["message"].FromDbString();

            return tnfToDoListMessage;
        }

        /// <summary>
        /// Use this function to receive an oid that is not currently in use for a message.
        /// </summary>
        /// <returns></returns>
        public int GetFreeOid()
        {
            string commandText = $"SELECT COALESCE(MAX(CAST(oid as INTEGER)),0)+1 FROM {TnfToDoListMessageTableName}";
            int oid = Db.ExecuteScalar(commandText);
            return oid;
        }
    }
}
