using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface IGpkgContentsManager
    {
        void Add(GpkgContents gpkgContents);
        int Update(GpkgContents gpkgContents);
    }

    public class GpkgContents
    {
        public string TableName { get; set; }
        public string DataType { get; set; }
        public string Identifier { get; set; }
        public string Description { get; set; }
        public DateTime? LastChange { get; set; }
        public double? MinX { get; set; }
        public double? MinY { get; set; }
        public double? MaxX { get; set; }
        public double? MaxY { get; set; }
        public int? SrsId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is GpkgContents)
            {
                var v = obj as GpkgContents;

                if (TableName == v.TableName &&
                    DataType == v.DataType &&
                    Identifier == v.Identifier &&
                    Description == v.Description &&
                    LastChange == v.LastChange &&
                    MinX == v.MinX &&
                    MinY == v.MinY &&
                    MaxX == v.MaxX &&
                    MaxY == v.MaxY &&
                    SrsId == v.SrsId)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(
                TableName,
                DataType,
                Identifier,
                Description,
                LastChange,
                MinX,
                MinY,
                MaxX,
                MaxY,
                SrsId);
        }

        public override string ToString()
        {
            return String.Format("GpkgContents: TableName = {0}, DataType = {1}, Identifier = {2}, Description = {3}, " +
                                 "LastChange = {4}, MinX = {5}, MinY = {6}, MaxX = {7}, MaxY = {8}, SrsId = {9}",
                TableName,
                DataType,
                Identifier,
                Description,
                LastChange,
                MinX,
                MinY,
                MaxX,
                MaxY,
                SrsId);
        }
    }

    public class GpkgContentsManager : TableManager, IGpkgContentsManager
    {
        private const string PrimaryKey = "table_name";
        public static string GpkgContentsTableName = "gpkg_contents";

        public GpkgContentsManager(GeoPackageDatabase db)
            : base(db, GpkgContentsTableName, GetColumnInfos(), PrimaryKey)
        {
        }

        protected override string[] Constraints()
        {
            return new[]
            {
                String.Format("CONSTRAINT fk_gc_r_srs_id FOREIGN KEY (srs_id) REFERENCES {0}(srs_id)",
                    GpkgSpatialRefSysManager.GpkgSpatialRefSysTableName)
            };
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "table_name",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "data_type",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "identifier",
                    SqlType = "TEXT UNIQUE",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "description",
                    SqlType = "TEXT DEFAULT ''",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "last_change",
                    SqlType = "DATETIME NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ','now'))",
                    DataType = Type.GetType("System.DateTime")
                },
                new ColumnInfo
                {
                    Name = "min_x",
                    SqlType = "DOUBLE",
                    DataType = Type.GetType("System.Double")
                },
                new ColumnInfo
                {
                    Name = "min_y",
                    SqlType = "DOUBLE",
                    DataType = Type.GetType("System.Double")
                },
                new ColumnInfo
                {
                    Name = "max_x",
                    SqlType = "DOUBLE",
                    DataType = Type.GetType("System.Double")
                },
                new ColumnInfo
                {
                    Name = "max_y",
                    SqlType = "DOUBLE",
                    DataType = Type.GetType("System.Double")
                },
                new ColumnInfo
                {
                    Name = "srs_id",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int64")
                },
            };
        }

        public void Add(GpkgContents gpkgContents)
        {
            Add(new object[]
                {
                    gpkgContents.TableName,
                    gpkgContents.DataType,
                    gpkgContents.Identifier,
                    gpkgContents.Description,
                    gpkgContents.LastChange?.ToUniversalTime(),
                    gpkgContents.MinX,
                    gpkgContents.MinY,
                    gpkgContents.MaxX,
                    gpkgContents.MaxY,
                    gpkgContents.SrsId
                });
        }

        public GpkgContents Get(string tableName)
        {
            return Get(ReadObject, new object[] { tableName });
        }

        public List<GpkgContents> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(GpkgContents gpkgContents)
        {
            return Update(new object[]
                {
                    gpkgContents.TableName,
                    gpkgContents.DataType,
                    gpkgContents.Identifier,
                    gpkgContents.Description,
                    gpkgContents.LastChange?.ToUniversalTime(),
                    gpkgContents.MinX,
                    gpkgContents.MinY,
                    gpkgContents.MaxX,
                    gpkgContents.MaxY,
                    gpkgContents.SrsId
                });
        }

        public void UpdateSrid(int srid)
        {
            using (var command = Db.Command)
            {
                command.CommandText = string.Format("UPDATE {0} SET srs_id = {1}", GpkgContentsTableName, srid);
                command.ExecuteScalar();
            }
        }

        public int Delete(string tableName)
        {
            return Delete(new object[] {tableName});
        }

        private static GpkgContents ReadObject(IDataRecord reader)
        {
            var gpkgContents = new GpkgContents();

            gpkgContents.TableName = reader["table_name"].FromDbString();
            gpkgContents.DataType = reader["data_type"].FromDbString();
            gpkgContents.Identifier = reader["identifier"].FromDbString();
            gpkgContents.Description = reader["description"].FromDbString();
            gpkgContents.LastChange = reader["last_change"].ToDateTime();
            gpkgContents.MinX = reader["min_x"].ToDouble();
            gpkgContents.MinY = reader["min_y"].ToDouble();
            gpkgContents.MaxX = reader["max_x"].ToDouble();
            gpkgContents.MaxY = reader["max_y"].ToDouble();
            gpkgContents.SrsId = reader["srs_id"].ToInt32();

            return gpkgContents;
        }
    }
}
