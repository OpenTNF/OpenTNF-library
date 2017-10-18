using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace OpenTNF.Library.Model
{
    public interface ITnfArea
    {
        int Oid { get; set; }
        string Name { get; set; }
        byte[] Shape { get; set; }
    }
    public class TnfArea : ITnfArea
    {
        public int Oid { get; set; }
        public string Name { get; set; }
        public byte[] Shape { get; set; }

        public override string ToString()
        {
            return $"TnfArea: Oid = {Oid}, Name = {Name}";
        }
    }

    public class TnfAreaManager : TableManager
    {
        public static string TnfAreaTableName = "tnf_area";

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
                    Name = "shape",
                    SqlType = "Polygon",
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
                    tnfArea.Shape
                });
        }

        public TnfArea Get(int oid)
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
                    tnfArea.Shape
                });
        }

        public int Delete(int oid)
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

            tnfArea.Oid = Convert.ToInt32(reader["oid"].FromDbString());
            tnfArea.Name = reader["name"].FromDbString();
            object shape = reader["shape"];
            if (!(shape is DBNull))
            {
                tnfArea.Shape = (byte[])shape;
            }

            return tnfArea;
        }
    }
}
