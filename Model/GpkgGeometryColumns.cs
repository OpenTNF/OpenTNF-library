using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public class GpkgGeometryColumn
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string GeometryTypeName { get; set; }
        public int SrsId { get; set; }
        public Int16 Z { get; set; }
        public Int16 M { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is GpkgGeometryColumn)
            {
                var v = obj as GpkgGeometryColumn;

                if (TableName == v.TableName &&
                    ColumnName == v.ColumnName &&
                    GeometryTypeName == v.GeometryTypeName &&
                    SrsId == v.SrsId &&
                    Z == v.Z &&
                    M == v.M)
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
                ColumnName,
                GeometryTypeName,
                SrsId,
                Z,
                M);
        }

        public override string ToString()
        {
            return String.Format("GpkgGeometryColumn: TableName = {0}, ColumnName = {1}, GeometryTypeName = {2}, SrsId = {3}, " +
                                 "Z = {4}, M = {5}",
                TableName,
                ColumnName,
                GeometryTypeName,
                SrsId,
                Z,
                M);
        }

    }

    public class GpkgGeometryColumnsManager : TableManager
    {
        private const string PrimaryKey = "table_name, column_name";
        public static string GpkgGeometryColumnsTableName = "gpkg_geometry_columns";

        public GpkgGeometryColumnsManager(GeoPackageDatabase db) : base(db, GpkgGeometryColumnsTableName, GetColumnInfos(),PrimaryKey)
        {
        }

        protected override string[] Constraints()
        {
            return new[]
            {
                "CONSTRAINT uk_gc_table_name UNIQUE (table_name)",
                String.Format("CONSTRAINT fk_gc_tn FOREIGN KEY (table_name) REFERENCES {0}(table_name)",
                    GpkgContentsManager.GpkgContentsTableName),
                String.Format("CONSTRAINT fk_gc_srs FOREIGN KEY (srs_id) REFERENCES {0} (srs_id)",
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
                    Name = "column_name",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "geometry_type_name",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "srs_id",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "z",
                    SqlType = "TINYINT NOT NULL",
                    DataType = Type.GetType("System.Int16")
                },
                new ColumnInfo
                {
                    Name = "m",
                    SqlType = "TINYINT NOT NULL",
                    DataType = Type.GetType("System.Int16")
                },
            };
        }

        public void Add(GpkgGeometryColumn gpkgGeometryColumn)
        {
            Add(new object[]
                {
                    gpkgGeometryColumn.TableName,
                    gpkgGeometryColumn.ColumnName,
                    gpkgGeometryColumn.GeometryTypeName,
                    gpkgGeometryColumn.SrsId,
                    gpkgGeometryColumn.Z,
                    gpkgGeometryColumn.M
                });
        }

        public GpkgGeometryColumn Get(string tableName, string columnName)
        {
            return Get(ReadObject, new object[] { tableName, columnName });
        }

        public List<GpkgGeometryColumn> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(GpkgGeometryColumn gpkgGeometryColumn)
        {
            return Update(new object[]
                {
                    gpkgGeometryColumn.TableName,
                    gpkgGeometryColumn.ColumnName,
                    gpkgGeometryColumn.GeometryTypeName,
                    gpkgGeometryColumn.SrsId,
                    gpkgGeometryColumn.Z,
                    gpkgGeometryColumn.M
                });
        }

        public void UpdateSrid(int srid)
        {
            using (var command = Db.Command)
            {
                command.CommandText = string.Format("UPDATE {0} SET srs_id = {1}", GpkgGeometryColumnsTableName, srid);
                command.ExecuteScalar();
            }
        }

        public int Delete(string tableName, string columnName)
        {
            return Delete(new object[] { tableName, columnName });
        }

        private static GpkgGeometryColumn ReadObject(IDataRecord reader)
        {
            var gpkgGeometryColumn = new GpkgGeometryColumn();

            gpkgGeometryColumn.TableName = reader["table_name"].FromDbString();
            gpkgGeometryColumn.ColumnName = reader["column_name"].FromDbString();
            gpkgGeometryColumn.GeometryTypeName = reader["geometry_type_name"].FromDbString();
            gpkgGeometryColumn.SrsId = (int)reader["srs_id"].ToInt32();
            gpkgGeometryColumn.Z = (Int16)reader["z"].ToInt16();
            gpkgGeometryColumn.M = (Int16)reader["m"].ToInt16();

            return gpkgGeometryColumn;
        }
    }
}
