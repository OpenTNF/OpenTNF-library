using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfTask
    {
        string Oid { get; }
        string ExternalId { get; }
        int? AreaOid { get; }
        string TaskType { get; }
        int? EditableNetwork { get; }
    }

    public class TnfTask : ITnfTask
    {
        public string Oid { get; set; }
        public string ExternalId { get; set; }
        public int? AreaOid { get; set; }
        public string TaskType { get; set; }
        public int? EditableNetwork { get; set; }
    }

    public class TnfTaskManager : TableManager
    {
        public static string TnfTaskTableName = "tnf_task";

        public TnfTaskManager(GeoPackageDatabase db) : base(db, TnfTaskTableName, GetColumnInfos())
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
                    Name = "external_id",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "area_oid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "task_type",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "editable_network",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                }
            };
        }

        public void Add(TnfTask tnfTask)
        {
            Add(new object[]
                {
                    tnfTask.Oid,
                    tnfTask.ExternalId,
                    tnfTask.AreaOid,
                    tnfTask.TaskType,
                    tnfTask.EditableNetwork
                });
        }

        public TnfTask Get(string oid)
        {
            return Get(ReadObject, new object[] { oid });
        }

        public List<TnfTask> GetByMaxResult(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(TnfTask tnfTask)
        {
            return Update(new object[]
                {
                    tnfTask.Oid,
                    tnfTask.ExternalId,
                    tnfTask.AreaOid,
                    tnfTask.TaskType,
                    tnfTask.EditableNetwork
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

        private static TnfTask ReadObject(IDataReader reader)
        {
            TnfTask tnfTask = new TnfTask();
            tnfTask.Oid = reader["oid"].FromDbString();
            tnfTask.ExternalId = reader["external_id"].FromDbString();
            tnfTask.AreaOid = reader["area_oid"].ToInt32();
            tnfTask.TaskType = reader["task_type"].FromDbString();
            tnfTask.EditableNetwork = reader["editable_network"].ToInt32();
            return tnfTask;
        }
    }
}
