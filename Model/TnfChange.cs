using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfChange
    {
        string Oid { get; }
        string ClassId { get; }
        string ChangeTransactionOid { get; }
        int OrderNumber { get; }
        int ChangeType { get; }
        string ChangeReason { get; }
        DateTime Timestamp { get; }
        string OldVid { get; }
        string NewVid { get; }
        string CreatorId { get; }
        string Remark { get; }
    }

    public static class TnfClassId
    {
        public const string LINK_SEQUENCE = "LINK_SEQUENCE";
        public const string NODE = "NODE";
        public const string PROPERTY_OBJECT = "PROPERTY_OBJECT";
        // "PROPERTY_OBJECT/{CATALOGUE_OID}/{PROPERTY_OBJECT_TYPE_OID}"
        // Example: "PROPERTY_OBJECT/ABC123/12345", where "ABC123" is the CATALOGUE_OID and "12345" is the OID of the property object type


        public static string PropertyObjectClassId(int catalogueOid, int propetyObjectTypeOid)
        {
            return $"{PROPERTY_OBJECT}/{catalogueOid}/{propetyObjectTypeOid}";
        }
    }

    public static class TnfChangeType
    {
        public const int Comment = 0;
        public const int Add = 1;
        public const int Modify = 2;
        public const int Delete = 3;
        public const int Append = 4;
    }

    public class TnfChange : ITnfChange
    {
        public string Oid { get; set; }
        public string ClassId { get; set; }
        public string ChangeTransactionOid { get; set; }
        public int OrderNumber { get; set; }
        public int ChangeType { get; set; }
        public string ChangeReason { get; set; }
        public DateTime Timestamp { get; set; }
        public string OldVid { get; set; }
        public string NewVid { get; set; }
        public string CreatorId { get; set; }
        public string Remark { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TnfChange)
            {
                var v = obj as TnfChange;

                if (Oid == v.Oid &&
                    ClassId == v.ClassId &&
                    ChangeTransactionOid == v.ChangeTransactionOid &&
                    OrderNumber == v.OrderNumber &&
                    ChangeType == v.ChangeType &&
                    ChangeReason == v.ChangeReason &&
                    Timestamp == v.Timestamp &&
                    OldVid == v.OldVid &&
                    NewVid == v.NewVid &&
                    CreatorId == v.CreatorId &&
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
                ClassId,
                ChangeTransactionOid,
                OrderNumber,
                ChangeType,
                ChangeReason,
                Timestamp,
                OldVid,
                NewVid,
                CreatorId,
                Remark);
        }

        public override string ToString()
        {
            return string.Format("TnfChange: Oid = {0}, ClassId = {1}, ChangeTransactionOid = {2}, OrderNumber = {3}, " +
                                 "ChangeType = {4}, ChangeReason = {5}, Timestamp = {6}, OldVid = {7}, NewVid = {8}, " +
                                 "CreatorId = {9}, Remark = {10}",
                Oid,
                ClassId,
                ChangeTransactionOid,
                OrderNumber,
                ChangeType,
                ChangeReason,
                Timestamp,
                OldVid,
                NewVid,
                CreatorId,
                Remark);
        }
    }

    public class TnfChangeManager : TableManager
    {
        private static readonly string[] m_primaryKey = new string[] { "change_transaction_oid", "order_number" };
        public const string TnfChangeTableName = "tnf_change";

        public TnfChangeManager(GeoPackageDatabase db) : base(db, TnfChangeTableName, GetColumnInfos(), string.Join(",", m_primaryKey))
        {
        }

        protected override string[] Constraints()
        {
            return new[]
                {
                    "CONSTRAINT fk_tc_cto FOREIGN KEY (change_transaction_oid) " +
                    $"REFERENCES {TnfChangeTransactionManager.TnfChangeTransactionTableName}(oid)"
                };
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
                    Name = "class_id",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "change_transaction_oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "order_number",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "change_type",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "change_reason",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "timestamp",
                    SqlType = "DATETIME NOT NULL",
                    DataType = Type.GetType("System.DateTime")
                },
                new ColumnInfo
                {
                    Name = "old_vid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String"),
                },
                new ColumnInfo
                {
                    Name = "new_vid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String"),
                },
                new ColumnInfo
                {
                    Name = "creator_id",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String"),
                },
                new ColumnInfo
                {
                    Name = "remark",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String"),
                }
            };
        }

        public void Add(TnfChange tnfChange)
        {
            Add(new object[]
                {
                    tnfChange.Oid,
                    tnfChange.ClassId,
                    tnfChange.ChangeTransactionOid,
                    tnfChange.OrderNumber,
                    tnfChange.ChangeType,
                    tnfChange.ChangeReason,
                    tnfChange.Timestamp.ToDateTimeString(),
                    tnfChange.OldVid,
                    tnfChange.NewVid,
                    tnfChange.CreatorId,
                    tnfChange.Remark
                });
        }

        public List<TnfChange> Get(int maxResults)
        {

            return Get(ReadObject, maxResults);
        }

        public List<TnfChange> GetPage(int offset, int limit)
        {
            return GetPage(ReadObject, offset, limit);
        }

        public List<TnfChange> GetForTransaction(string transactionOid)
        {
            List<TnfChange> deleteChanges = new List<TnfChange>();
            using (var command = Db.Command)
            {
                command.CommandText = $"SELECT * FROM {TnfChangeTableName} WHERE change_transaction_oid = '{transactionOid}'";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        deleteChanges.Add(ReadObject(reader));
                    }
                }
            }
            return deleteChanges;
        }

        public List<TnfChange> GetForOid(string changeObjectOid)
        {
            List<TnfChange> deleteChanges = new List<TnfChange>();
            using (var command = Db.Command)
            {
                command.CommandText = $"SELECT * FROM {TnfChangeTableName} WHERE oid = '{changeObjectOid}'";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        deleteChanges.Add(ReadObject(reader));
                    }
                }
            }
            return deleteChanges;
        }

        public List<TnfChange> GetDeleteChangesForOids(List<string> changeObjectOids)
        {
            List<TnfChange> deleteChanges = new List<TnfChange>();
            if (changeObjectOids.Count == 0)
            {
                return deleteChanges;
            }
            using (var command = Db.Command)
            {
                command.CommandText =
                    $"SELECT * FROM {TnfChangeTableName} " +
                    $"WHERE change_type = {TnfChangeType.Delete} " +
                    $"AND oid in ({string.Join(",", changeObjectOids.Select(oid => $"'{oid}'"))})";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        deleteChanges.Add(ReadObject(reader));
                    }
                }
            }
            return deleteChanges;
        }

        public int Update(TnfChange tnfChange)
        {
            return Update(new object[]
                {
                    tnfChange.Oid,
                    tnfChange.ClassId,
                    tnfChange.ChangeTransactionOid,
                    tnfChange.OrderNumber,
                    tnfChange.ChangeType,
                    tnfChange.ChangeReason,
                    tnfChange.Timestamp.ToDateTimeString(),
                    tnfChange.OldVid,
                    tnfChange.NewVid,
                    tnfChange.CreatorId,
                    tnfChange.Remark
                });
        }

        /// <summary>
        /// Delete:s a tnfChange
        /// </summary>
        /// <param name="tnfChange"></param>
        /// <returns></returns>
        public int Delete(TnfChange tnfChange)
        {
            if (m_primaryKey[0] != "change_transaction_oid" || m_primaryKey[1] != "order_number")
            {
                throw new ApplicationException("Delete method expects primary key to be change_transaction_oid,order_number");
            }
            return Delete(new object[]
                {
                    tnfChange.ChangeTransactionOid,
                    tnfChange.OrderNumber,
                });
        }

        public int DeleteForTransaction(string transactionOid)
        {
            using (var command = Db.Command)
            {
                command.CommandText = $"DELETE FROM {TnfChangeTableName} WHERE change_transaction_oid = '{transactionOid}'";

                return command.ExecuteNonQuery();
            }
        }

        public int DeleteForChangedObject(string changedObjectOid)
        {
            using (var command = Db.Command)
            {
                command.CommandText = $"DELETE FROM {TnfChangeTableName} WHERE oid = '{changedObjectOid}'";

                return command.ExecuteNonQuery();
            }
        }

        private static TnfChange ReadObject(IDataRecord reader)
        {
            var tnfChange = new TnfChange();

            tnfChange.Oid = reader["oid"].FromDbString();
            tnfChange.ClassId = reader["class_id"].FromDbString();
            tnfChange.ChangeTransactionOid = reader["change_transaction_oid"].FromDbString();
            tnfChange.OrderNumber = (int)reader["order_number"].ToInt32();
            tnfChange.ChangeType = (int)reader["change_type"].ToInt32();
            tnfChange.ChangeReason = reader["change_reason"].FromDbString();
            tnfChange.Timestamp = (DateTime)reader["timestamp"].ToDateTime();
            tnfChange.OldVid = reader["old_vid"].FromDbString();
            tnfChange.NewVid = reader["new_vid"].FromDbString();
            tnfChange.CreatorId = reader["creator_id"].FromDbString();
            tnfChange.Remark = reader["remark"].FromDbString();

            return tnfChange;
        }
    }
}
