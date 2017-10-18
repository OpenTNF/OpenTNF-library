using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfChangeTransaction
    {
        string Oid { get; }
        string Name { get; }
        DateTime CreationTime { get; }
        string Creator { get; }
        string Remark { get; }
    }

    public class TnfChangeTransaction : ITnfChangeTransaction
    {
        public string Oid { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public string Creator { get; set; }
        public string Remark { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TnfChangeTransaction)
            {
                var v = obj as TnfChangeTransaction;

                if (Oid == v.Oid &&
                    Name == v.Name &&
                    CreationTime == v.CreationTime &&
                    Creator == v.Creator &&
                    Remark == v.Remark)
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
                Name,
                CreationTime,
                Creator,
                Remark);
        }

        public override string ToString()
        {
            return String.Format("TnfChangeTransaction: Oid = {0}, Name = {1}, CreationTime = {2}, Creator = {3}, Remark = {4}",
                Oid,
                Name,
                CreationTime,
                Creator,
                Remark);
        }
    }

    public class TnfChangeTransactionManager : TableManager
    {
        public static string TnfChangeTransactionTableName = "tnf_change_transaction";

        public TnfChangeTransactionManager(GeoPackageDatabase db) : base(db, TnfChangeTransactionTableName, GetColumnInfos())
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
                    Name = "name",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "creation_time",
                    SqlType = "DATETIME NOT NULL",
                    DataType = Type.GetType("System.DateTime")
                },
                new ColumnInfo
                {
                    Name = "creator",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "remark",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                }
            };
        }

        public void Add(TnfChangeTransaction tnfChangeTransaction)
        {
            Add(new object[]
                {
                    tnfChangeTransaction.Oid,
                    tnfChangeTransaction.Name,
                    tnfChangeTransaction.CreationTime.ToUniversalTime(),
                    tnfChangeTransaction.Creator,
                    tnfChangeTransaction.Remark
                });
        }

        public TnfChangeTransaction Get(string oid)
        {
            return Get(ReadObject, new object[] { oid });
        }

        public List<TnfChangeTransaction> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(TnfChangeTransaction tnfChangeTransaction)
        {
            return Update(new object[]
                {
                    tnfChangeTransaction.Oid,
                    tnfChangeTransaction.Name,
                    tnfChangeTransaction.CreationTime.ToUniversalTime(),
                    tnfChangeTransaction.Creator,
                    tnfChangeTransaction.Remark
                });
        }

        public int Delete(string oid)
        {
            return Delete(new object[] { oid });
        }

        private static TnfChangeTransaction ReadObject(IDataRecord reader)
        {
            var tnfChangeTransaction = new TnfChangeTransaction();

            tnfChangeTransaction.Oid = reader["oid"].FromDbString();
            tnfChangeTransaction.Name = reader["name"].FromDbString();
            tnfChangeTransaction.CreationTime = (DateTime) reader["creation_time"].ToDateTime();
            tnfChangeTransaction.Creator = reader["creator"].FromDbString();
            tnfChangeTransaction.Remark = reader["remark"].FromDbString();

            return tnfChangeTransaction;
        }
    }
}
