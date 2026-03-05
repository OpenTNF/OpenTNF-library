using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfArea
    {
        string Oid { get; set; }
        string Name { get; set; }
        string Type { get; set; }
        int? Code { get; set; }
        byte[] Shape { get; set; }
    }
    public class TnfArea : ITnfArea
    {
        public string Oid { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? Code { get; set; }
        public byte[] Shape { get; set; }

        public override string ToString()
        {
            return $"TnfArea: Oid = {Oid}, Name = {Name}";
        }
    }

    public class TnfAreaManager : TableManager
    {
        public const string TnfAreaTableName = "tnf_area";

        public TnfAreaManager(GeoPackageDatabase db) : base(db, TnfAreaTableName, GetColumnInfos())
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
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "type",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String"),
                    Requirement = ColumnRequirement.OptionalReadOnly
                },
                new ColumnInfo
                {
                    Name = "code",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32"),
                    Requirement = ColumnRequirement.OptionalReadOnly
                },
                new ColumnInfo
                {
                    Name = "shape",
                    SqlType = "POLYGON",
                    DataType = Type.GetType("System.Byte[]")
                }
            };
        }

        public void Add(TnfArea tnfArea)
        {
            Add(new object[]
                {
                    tnfArea.Oid,
                    tnfArea.Name,
                    tnfArea.Type,
                    tnfArea.Code,
                    tnfArea.Shape
                });
        }

        public TnfArea Get(string oid)
        {
            return Get(ReadObject, new object[] { oid });
        }

        public List<TnfArea> GetByMaxResult(int maxresult)
        {
            return Get(ReadObject, maxresult);
        }

        public List<TnfArea> GetPage(int offset, int limit)
        {
            return GetPage(ReadObject, offset, limit);
        }

        public int Update(TnfArea tnfArea)
        {
            return Update(new object[]
                {
                    tnfArea.Oid,
                    tnfArea.Name,
                    tnfArea.Type,
                    tnfArea.Code,
                    tnfArea.Shape
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

        private static TnfArea ReadObject(IDataReader reader)
        {
            var tnfArea = new TnfArea();

            tnfArea.Oid = reader["oid"].FromDbString();
            tnfArea.Name = reader["name"].FromDbString();
            tnfArea.Type = reader.ReadIfExists("type").FromDbString();
            tnfArea.Code = reader.ReadIfExists("code").ToInt32();

            object shape = reader["shape"];
            if (!(shape is DBNull))
            {
                tnfArea.Shape = (byte[])shape;
            }

            return tnfArea;
        }
    }
}
