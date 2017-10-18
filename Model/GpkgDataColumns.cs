using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public class GpkgDataColumn
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MimeType { get; set; }
        public string ConstraintName { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is GpkgDataColumn)
            {
                var v = obj as GpkgDataColumn;

                if (TableName == v.TableName &&
                    ColumnName == v.ColumnName &&
                    Name == v.Name &&
                    Title == v.Title &&
                    Description == v.Description &&
                    MimeType == v.MimeType &&
                    ConstraintName == v.ConstraintName)
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
                Name,
                Title,
                Description,
                MimeType,
                ConstraintName);
        }

        public override string ToString()
        {
            return String.Format("GpkgDataColumn: TableName = {0}, ColumnName = {1}, Name = {2}, Title = {3}, " +
                                 "Description = {4}, MimeType = {5}, ConstraintName = {6}",
                TableName,
                ColumnName,
                Name,
                Title,
                Description,
                MimeType,
                ConstraintName);
        }
    }

    public class GpkgDataColumnsManager : TableManager
    {
        private const string PrimaryKey = "table_name, column_name";
        public static string GpkgDataColumnsTableName = "gpkg_data_columns";

        public GpkgDataColumnsManager(GeoPackageDatabase db) : base(db, GpkgDataColumnsTableName, GetColumnInfos(), PrimaryKey)
        {
        }

        protected override string[] Constraints()
        {
            return new[]
                {
                    String.Format("CONSTRAINT fk_gdc_tn FOREIGN KEY (table_name) REFERENCES {0}(table_name)", GpkgContentsManager.GpkgContentsTableName)
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
                    Name = "name",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "title",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "description",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "mime_type",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "constraint_name",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
            };
        }

        public void Add(GpkgDataColumn gpkgDataColumn)
        {
            Add(new object[]
                {
                    gpkgDataColumn.TableName,
                    gpkgDataColumn.ColumnName,
                    gpkgDataColumn.Name,
                    gpkgDataColumn.Title,
                    gpkgDataColumn.Description,
                    gpkgDataColumn.MimeType,
                    gpkgDataColumn.ConstraintName
                });
        }

        public GpkgDataColumn Get(string tableName, string columnName)
        {
            return Get(ReadObject, new object[] { tableName, columnName });
        }

        public List<GpkgDataColumn> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(GpkgDataColumn gpkgDataColumn)
        {
            return Update(new object[]
                {
                    gpkgDataColumn.TableName,
                    gpkgDataColumn.ColumnName,
                    gpkgDataColumn.Name,
                    gpkgDataColumn.Title,
                    gpkgDataColumn.Description,
                    gpkgDataColumn.MimeType,
                    gpkgDataColumn.ConstraintName
                });
        }

        public int Delete(string tableName, string columnName)
        {
            return Delete(new object[] { tableName, columnName });
        }

        private static GpkgDataColumn ReadObject(IDataRecord reader)
        {
            var gpkgDataColumn = new GpkgDataColumn();

            gpkgDataColumn.TableName = reader["table_name"].FromDbString();
            gpkgDataColumn.ColumnName = reader["column_name"].FromDbString();
            gpkgDataColumn.Name = reader["name"].FromDbString();
            gpkgDataColumn.Title = reader["title"].FromDbString();
            gpkgDataColumn.Description = reader["description"].FromDbString();
            gpkgDataColumn.MimeType = reader["mime_type"].FromDbString();
            gpkgDataColumn.ConstraintName = reader["constraint_name"].FromDbString();

            return gpkgDataColumn;
        }
    }
}
