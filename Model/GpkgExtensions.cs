using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public class GpkgExtension
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ExtensionName { get; set; }
        public string Definition { get; set; }
        public string Scope { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is GpkgExtension)
            {
                var v = obj as GpkgExtension;

                if (TableName == v.TableName &&
                    ColumnName == v.ColumnName &&
                    ExtensionName == v.ExtensionName &&
                    Definition == v.Definition &&
                    Scope == v.Scope)
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
                ExtensionName,
                Definition,
                Scope);
        }

        public override string ToString()
        {
            return String.Format("GpkgExtension: TableName = {0}, ColumnName = {1}, ExtensionName = {2}, Definition = {3}, Scope = {4}",
                TableName,
                ColumnName,
                ExtensionName,
                Definition,
                Scope);
        }
    }

    public class GpkgExtensionsManager : TableManager
    {
        public static string GpkgExtensionsTableName = "gpkg_extensions";

        public GpkgExtensionsManager(GeoPackageDatabase db) : base(db, GpkgExtensionsTableName, GetColumnInfos(), null)
        {
        }

        protected override string[] Constraints()
        {
            return new[]
                {
                    "CONSTRAINT ge_tce UNIQUE (table_name, column_name, extension_name)"
                };
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "table_name",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "column_name",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "extension_name",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "definition",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "scope",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                }
            };
        }

        public void Add(GpkgExtension gpkgExtension)
        {
            Add(new object[]
                {
                    gpkgExtension.TableName,
                    gpkgExtension.ColumnName,
                    gpkgExtension.ExtensionName,
                    gpkgExtension.Definition,
                    gpkgExtension.Scope
                });
        }

        public GpkgExtension Get(string tableName, string columnName, string extensionName)
        {
            GpkgExtension gpkgExtension = null;

            using (IDataReader reader =
                Db.ExecuteReader(
                    string.Format(
                        "SELECT table_name, column_name, extension_name, definition, scope FROM {0} WHERE {1}",
                        GpkgExtensionsTableName,
                        GetWhereClause(tableName, columnName, extensionName))))
            {
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        gpkgExtension = ReadObject(reader);
                    }
                }
            }

            return gpkgExtension;
        }

        public List<GpkgExtension> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Delete(string tableName, string columnName, string extensionName)
        {
            return
                Db.ExecuteNonQuery(
                    string.Format("DELETE FROM {0} WHERE {1}",
                        GpkgExtensionsTableName,
                        GetWhereClause(tableName, columnName, extensionName)));
        }

        private static string GetWhereClause(string tableName, string columnName, string extensionName)
        {
            var whereList = new List<string>
                {
                    tableName != null ? string.Format("table_name = '{0}'", tableName) : "table_name IS NULL",
                    columnName != null ? string.Format("column_name = '{0}'", columnName) : "column_name IS NULL",
                    extensionName != null
                        ? string.Format("extension_name = '{0}'", extensionName)
                        : "extension_name IS NULL"
                };

            return string.Join(" AND ", whereList);
        }

        private static GpkgExtension ReadObject(IDataRecord reader)
        {
            var gpkgExtension = new GpkgExtension();

            gpkgExtension.TableName = reader["table_name"].FromDbString();
            gpkgExtension.ColumnName = reader["column_name"].FromDbString();
            gpkgExtension.ExtensionName = reader["extension_name"].FromDbString();
            gpkgExtension.Definition = reader["definition"].FromDbString();
            gpkgExtension.Scope = reader["scope"].FromDbString();

            return gpkgExtension;
        }

    }
}
