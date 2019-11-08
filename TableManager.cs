using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using OpenTNF.Library.Model;

namespace OpenTNF.Library
{
    public class ColumnInfo
    {
        public string Name { get; set; }
        public string SqlType { get; set; }
        public Type DataType { get; set; }
        public bool HandleMissing { get; set; }
    }

    public abstract class TableManager : IDisposable
    {
        private static readonly List<string> m_reservedWords = new List<string>
            {
                "ABORT",         "ACTION",        "ADD",           "AFTER",         "ALL",           "ALTER",
                "ANALYZE",       "AND",           "AS",            "ASC",           "ATTACH",        "AUTOINCREMENT",
                "BEFORE",        "BEGIN",         "BETWEEN",       "BY",            "CASCADE",       "CASE",
                "CAST",          "CHECK",         "COLLATE",       "COLUMN",        "COMMIT",        "CONFLICT",
                "CONSTRAINT",    "CREATE",        "CROSS",         "CURRENT_DATE",  "CURRENT_TIME",  "CURRENT_TIMESTAMP",
                "DATABASE",      "DEFAULT",       "DEFERRABLE",    "DEFERRED",      "DELETE",        "DESC",
                "DETACH",        "DISTINCT",      "DROP",          "EACH",          "ELSE",          "END",
                "ESCAPE",        "EXCEPT",        "EXCLUSIVE",     "EXISTS",        "EXPLAIN",       "FAIL",
                "FOR",           "FOREIGN",       "FROM",          "FULL",          "GLOB",          "GROUP",
                "HAVING",        "IF",            "IGNORE",        "IMMEDIATE",     "IN",            "INDEX",
                "INDEXED",       "INITIALLY",     "INNER",         "INSERT",        "INSTEAD",       "INTERSECT",
                "INTO",          "IS",            "ISNULL",        "JOIN",          "KEY",           "LEFT",
                "LIKE",          "LIMIT",         "MATCH",         "NATURAL",       "NO",            "NOT",
                "NOTNULL",       "NULL",          "OF",            "OFFSET",        "ON",            "OR",
                "ORDER",         "OUTER",         "PLAN",          "PRAGMA",        "PRIMARY",       "QUERY",
                "RAISE",         "RECURSIVE",     "REFERENCES",    "REGEXP",        "REINDEX",       "RELEASE",
                "RENAME",        "REPLACE",       "RESTRICT",      "RIGHT",         "ROLLBACK",      "ROW",
                "SAVEPOINT",     "SELECT",        "SET",           "TABLE",         "TEMP",          "TEMPORARY",
                "THEN",          "TO",            "TRANSACTION",   "TRIGGER",       "UNION",         "UNIQUE",
                "UPDATE",        "USING",         "VACUUM",        "VALUES",        "VIEW",          "VIRTUAL",
                "WHEN",          "WHERE",         "WITH",          "WITHOUT"
            };

        protected GeoPackageDatabase Db;
        protected IDbCommand InsertCmd = null;
        protected IDbCommand SelectCmd = null;
        protected IDbCommand SelectAllCmd = null;
        protected IDbCommand SelectPageCmd = null;
        protected IDbCommand UpdateCmd = null;
        protected IDbCommand DeleteCmd = null;
        protected IDbCommand CountAllCmd = null;
        protected IDbCommand CountCmd = null;
        private readonly string m_tableName;
        private string[] m_primaryKeyColumnNames;
        private ColumnInfo[] m_columns;
        private string[] m_missingColumns;

        private const string DefaultPrimaryKey = "oid";
        
        /// <summary>
        /// TableManager is a abstract class that is used to read and write tables in OpenTNF geopackage file.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName">Name of the table, specified in sub class.</param>
        /// <param name="columns">Column <see cref="ColumnInfo"/> description, specified in sub class.</param>
        /// <param name="primaryKey">Comma seperated list of columnnames that describes the primary key. Must have the same order as in the columns list.</param>
        protected TableManager(GeoPackageDatabase db, string tableName, ColumnInfo[] columns, string primaryKey = DefaultPrimaryKey)
        {
            Db = db;
            m_tableName = tableName;
            m_primaryKeyColumnNames = GetPrimaryKeyColumnNamesFromStr(primaryKey);
            m_columns = columns;
            InitColumns();

            if (!TableExists())
            {
                CreateTable();
            }
            else
            {
                HandleMissingColumns();
            }
        }

        private void HandleMissingColumns()
        {
            List<ColumnInfo> filteredColumnsList = new List<ColumnInfo>();
            List<string> filteredPrimaryKeyList = new List<string>();
            List<string> missingColumnsList = new List<string>();
            bool missingColumnRemoved = false;
            foreach (ColumnInfo columnInfo in m_columns)
            {
                if (columnInfo.HandleMissing && !ColumnExists(columnInfo.Name))
                {
                    missingColumnsList.Add(columnInfo.Name);
                    missingColumnRemoved = true;
                }
                else
                {
                    if (m_primaryKeyColumnNames.Contains(columnInfo.Name))
                    {
                        filteredPrimaryKeyList.Add(columnInfo.Name);
                    }
                    filteredColumnsList.Add(columnInfo);
                }
            }
            if (missingColumnRemoved)
            {
                m_columns = filteredColumnsList.ToArray();
                m_primaryKeyColumnNames = filteredPrimaryKeyList.ToArray();
                m_missingColumns = missingColumnsList.ToArray();
            }
        }

        private string[] GetPrimaryKeyColumnNamesFromStr(string primaryKey)
        {
            if (String.IsNullOrEmpty(primaryKey)) return new string[] {};
            return primaryKey.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim()).ToArray();
        }

        public string TableName { get { return m_tableName; } }

        protected virtual string[] Constraints()
        {
            return null;
        }

        protected virtual string[] Indices()
        {
            return null;
        }

        protected string[] ColumnNames => m_columns.Select(c => c.Name).ToArray();

        public void CreateIndices()
        {
            if (Indices() != null)
            {
                foreach (var index in Indices())
                {
                    Db.ExecuteNonQuery(index);
                }
            }
        }

        protected virtual string[] Triggers()
        {
            return null;
        }

        private void InitColumns()
        {
            foreach (ColumnInfo ci in m_columns)
            {
                if (string.IsNullOrEmpty(ci.SqlType))
                {
                    DetermineSqlDataType(ci);
                }
            }
        }

        private static void DetermineSqlDataType(ColumnInfo ci)
        {
            string dataType = ci.DataType.ToString();
            int index = dataType.IndexOf(' ');

            if (index >= 0)
            {
                dataType = dataType.Substring(0, index);
            }

            switch (dataType)
            {
                case "System.String":
                    ci.SqlType = "TEXT";
                    break;
                case "System.DateTime":
                    ci.SqlType = "DATETIME";
                    break;
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                    ci.SqlType = "INTEGER";
                    break;
                case "System.Double":
                    ci.SqlType = "DOUBLE";
                    break;
                case "System.Decimal":
                    ci.SqlType = "DOUBLE";
                    break;
                case "System.Byte[]":
                    ci.SqlType = "BLOB";
                    break;
                case "System.Boolean":
                    ci.SqlType = "INTEGER";
                    break;
                default:
                    throw new NotImplementedException(string.Format(
                        "typemapper implementation of type {0} not implemented", ci.DataType));
            }
        }

        private static DbType GetDbDataType(ColumnInfo ci)
        {
            string dataType = ci.DataType.ToString();
            int index = dataType.IndexOf(' ');

            if (index >= 0)
            {
                dataType = dataType.Substring(0, index);
            }

            switch (dataType)
            {
                case "System.String":
                    return DbType.String;
                case "System.DateTime":
                    return DbType.DateTime;
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                    return DbType.Int64;
                case "System.Double":
                    return DbType.Double;
                case "System.Decimal":
                    return DbType.Double;
                case "System.Boolean":
                    return DbType.Int64;
                case "System.Byte[]":
                    return DbType.Binary;
                default:
                    throw new NotImplementedException(string.Format(
                        "typemapper implementation of type {0} not implemented", ci.DataType));
            }
        }

        protected void Add(object[] values)
        {
            if (m_missingColumns != null)
            {
                throw new OpenTnfException("Unable to add object because columns are missing in the table. " +
                                           $"Missing columns: {string.Join(", ", m_missingColumns)}");
            }
            if (InsertCmd == null)
            {
                InsertCmd = Db.Command;
                InsertCmd.Transaction = Db.GetSqLiteTransaction();
                var columnList = new List<string>();
                var parameterList = new List<string>();

                foreach (var column in m_columns)
                {
                    columnList.Add(GetColumnName(column));
                    parameterList.Add("@" + column.Name);

                    InsertCmd.Parameters.Add(
                        new System.Data.SQLite.SQLiteParameter
                            {
                                ParameterName = "@" + column.Name,
                                DbType = GetDbDataType(column)
                            });
                }

                string cmdtext = string.Format(
                    "INSERT INTO {0} ({1}) VALUES ({2}) ",
                    TableName,
                    string.Join(", ", columnList),
                    string.Join(", ", parameterList));
                InsertCmd.CommandText = cmdtext;
                InsertCmd.Prepare();
            }

            for (int paramNo = 0; paramNo < m_columns.Length; paramNo++)
            {
                var dbDataParameter = InsertCmd.Parameters[paramNo] as IDbDataParameter;

                if (dbDataParameter != null)
                {
                    dbDataParameter.Value = values[paramNo] ?? DBNull.Value;
                }
            }

            InsertCmd.ExecuteNonQuery();
        }

        private static string GetColumnName(ColumnInfo columnInfo)
        {
            string columnName = columnInfo.Name;
            if (m_reservedWords.Contains(columnName.ToUpper()))
            {
                columnName = "\"" + columnName + "\"";
            }
            return columnName;
        }

        protected T Get<T>(Func<IDataReader, T> func, object[] values) where T : new()
        {
            if (SelectCmd == null)
            {
                SelectCmd = Db.Command;
                SelectCmd.Transaction = Db.GetSqLiteTransaction();
                var columnList = new List<string>();
                var whereList = new List<string>();

                foreach (ColumnInfo column in m_columns)
                {
                    string columnName = GetColumnName(column);
                    columnList.Add(columnName);

                    if (m_primaryKeyColumnNames.Contains(column.Name))
                    {
                        whereList.Add(string.Format("{0} = @{1}", columnName, column.Name));

                        SelectCmd.Parameters.Add(
                            new System.Data.SQLite.SQLiteParameter
                                {
                                    ParameterName = "@" + column.Name,
                                    DbType = GetDbDataType(column)
                                });
                    }
                }

                string cmdtext = string.Format(
                    "SELECT {0} FROM {1} WHERE {2}",
                    string.Join(", ", columnList),
                    TableName,
                    string.Join(" AND ", whereList));
                SelectCmd.CommandText = cmdtext;
                SelectCmd.Prepare();
            }

            for (int paramNo = 0; paramNo < values.Length; paramNo++)
            {
                if (SelectCmd.Parameters.Count > paramNo)
                {
                    var dbDataParameter = SelectCmd.Parameters[paramNo] as IDbDataParameter;

                    if (dbDataParameter != null)
                    {
                        dbDataParameter.Value = values[paramNo] ?? DBNull.Value;
                    }
                }
            }

            var item = default(T);
            using (IDataReader reader = SelectCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    item = func(reader);
                }
            }

            return item;
        }

        protected List<T> Get<T>(Func<IDataReader, T> parameter, int limit)
        {
            if (SelectAllCmd == null)
            {
                SelectAllCmd = Db.Command;
                SelectAllCmd.Transaction = Db.GetSqLiteTransaction();
                string colStr = "";

                foreach (var column in m_columns)
                {
                    colStr += GetColumnName(column) + ", ";
                }

                colStr = colStr.Remove(colStr.LastIndexOf(", ", StringComparison.Ordinal));
                string cmdtext = string.Format("SELECT {0} FROM {1} LIMIT @limit", colStr, TableName);
                SelectAllCmd.CommandText = cmdtext;
                SelectAllCmd.Parameters.Add(
                    new System.Data.SQLite.SQLiteParameter { ParameterName = "@limit", DbType = DbType.Int64 });
                SelectAllCmd.Prepare();
            }

            var dbDataParameter = SelectAllCmd.Parameters[0] as IDbDataParameter;

            if (dbDataParameter != null)
            {
                dbDataParameter.Value = limit;
            }

            var list = new List<T>();
            using (IDataReader reader = SelectAllCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(parameter(reader));
                }
            }

            return list;
        }

        public List<T> GetPage<T>(Func<IDataReader, T> parameter, int offset, int limit)
        {
            if (SelectPageCmd == null)
            {
                SelectPageCmd = Db.Command;
                SelectPageCmd.Transaction = Db.GetSqLiteTransaction();
                string colStr = "";

                foreach (var column in m_columns)
                {
                    colStr += GetColumnName(column) + ", ";
                }

                colStr = colStr.Remove(colStr.LastIndexOf(", ", StringComparison.Ordinal));
                string cmdtext = string.Format("SELECT {0} FROM {1} ORDER BY rowid LIMIT @offset, @limit", colStr, TableName);
                SelectPageCmd.CommandText = cmdtext;
                SelectPageCmd.Parameters.Add(
                    new System.Data.SQLite.SQLiteParameter { ParameterName = "@offset", DbType = DbType.Int64 });
                SelectPageCmd.Parameters.Add(
                    new System.Data.SQLite.SQLiteParameter { ParameterName = "@limit", DbType = DbType.Int64 });
                SelectPageCmd.Prepare();
            }

            var dbDataParameter = SelectPageCmd.Parameters[0] as IDbDataParameter;

            if (dbDataParameter != null)
            {
                dbDataParameter.Value = offset;
            }

            dbDataParameter = SelectPageCmd.Parameters[1] as IDbDataParameter;

            if (dbDataParameter != null)
            {
                dbDataParameter.Value = limit;
            }

            var list = new List<T>();
            var reader = SelectPageCmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(parameter(reader));
            }

            reader.Close();

            return list;
        }

        protected int Update(object[] values)
        {
            if (m_missingColumns != null)
            {
                throw new OpenTnfException("Unable to update object because columns are missing in the table. " +
                                           $"Missing columns: {string.Join(", ", m_missingColumns)}");
            }
            if (UpdateCmd == null)
            {
                UpdateCmd = Db.Command;
                UpdateCmd.Transaction = Db.GetSqLiteTransaction();
                var setList = new List<string>();
                var whereList = new List<string>();

                foreach (var column in m_columns)
                {
                    string columnName = GetColumnName(column);

                    if (m_primaryKeyColumnNames.Contains(column.Name))
                    {
                        whereList.Add(string.Format("{0} = @{1}", columnName, column.Name));
                    }
                    else
                    {
                        setList.Add(string.Format("{0} = @{1}", columnName, column.Name));
                    }

                    UpdateCmd.Parameters.Add(
                        new System.Data.SQLite.SQLiteParameter
                            {
                                ParameterName = "@" + column.Name,
                                DbType = GetDbDataType(column)
                            });
                }

                string cmdtext = string.Format(
                    "UPDATE {0} SET {1} WHERE {2}",
                    TableName,
                    string.Join(", ", setList),
                    string.Join(" AND ", whereList));
                UpdateCmd.CommandText = cmdtext;
                UpdateCmd.Prepare();
            }

            for (int paramNo = 0; paramNo < m_columns.Length; paramNo++)
            {
                var dbDataParameter = UpdateCmd.Parameters[paramNo] as IDbDataParameter;

                if (dbDataParameter != null)
                {
                    dbDataParameter.Value = values[paramNo] ?? DBNull.Value;
                }
            }

            return UpdateCmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes a row from the table using the primaryKey. The primary key value must be ordered the same way as in m_primaryKeyColumnNames (and Columns list).
        /// </summary>
        /// <param name="orderdPrimaryKeyValue"></param>
        /// <returns></returns>
        protected int Delete(object[] orderdPrimaryKeyValue)
        {
            if (m_missingColumns != null)
            {
                throw new OpenTnfException("Unable to delete object because columns are missing in the table. " +
                                           $"Missing columns: {string.Join(", ", m_missingColumns)}");
            }
            if (DeleteCmd == null)
            {
                DeleteCmd = Db.Command;
                DeleteCmd.Transaction = Db.GetSqLiteTransaction();
                var whereList = new List<string>();

                foreach (var column in m_columns)
                {
                    if (m_primaryKeyColumnNames.Contains(column.Name))
                    {
                        whereList.Add(string.Format("{0} = @{1}", GetColumnName(column), column.Name));

                        DeleteCmd.Parameters.Add(
                            new System.Data.SQLite.SQLiteParameter
                                {
                                    ParameterName = "@" + column.Name,
                                    DbType = GetDbDataType(column)
                                });
                    }
                }

                string cmdtext = string.Format(
                    "DELETE FROM {0} WHERE {1}",
                    TableName,
                    string.Join(" AND ", whereList));
                DeleteCmd.CommandText = cmdtext;
                DeleteCmd.Prepare();
            }

            for (int paramNo = 0; paramNo < orderdPrimaryKeyValue.Count(); paramNo++)
            {
                var dbDataParameter = DeleteCmd.Parameters[paramNo] as IDbDataParameter;

                if (dbDataParameter != null)
                {
                    dbDataParameter.Value = orderdPrimaryKeyValue[paramNo] ?? DBNull.Value;
                }
            }

            return DeleteCmd.ExecuteNonQuery();
        }

        public int Count()
        {
            if (CountAllCmd == null)
            {
                CountAllCmd = Db.Command;
                CountAllCmd.Transaction = Db.GetSqLiteTransaction();

                string cmdtext = string.Format("SELECT COUNT(*) FROM {0}", TableName);
                CountAllCmd.CommandText = cmdtext;
                CountAllCmd.Prepare();
            }

            return CountAllCmd.ExecuteScalar().ToInt32() ?? 0;
        }

        public int Count(object[] values)
        {
            if (CountCmd == null)
            {
                CountCmd = Db.Command;
                CountCmd.Transaction = Db.GetSqLiteTransaction();
                var whereList = new List<string>();

                foreach (var column in m_columns)
                {
                    if (m_primaryKeyColumnNames.Contains(column.Name))
                    {
                        whereList.Add(string.Format("{0} = @{1}", GetColumnName(column), column.Name));

                        CountCmd.Parameters.Add(
                            new System.Data.SQLite.SQLiteParameter
                            {
                                ParameterName = "@" + column.Name,
                                DbType = GetDbDataType(column)
                            });
                    }
                }

                string cmdtext = string.Format(
                    "SELECT COUNT(*) FROM {0} WHERE {1}",
                    TableName,
                    string.Join(" AND ", whereList));
                CountCmd.CommandText = cmdtext;
                CountCmd.Prepare();
            }

            for (int paramNo = 0; paramNo < values.Count(); paramNo++)
            {
                var dbDataParameter = CountCmd.Parameters[paramNo] as IDbDataParameter;

                if (dbDataParameter != null)
                {
                    dbDataParameter.Value = values[paramNo] ?? DBNull.Value;
                }
            }


            return CountCmd.ExecuteScalar().ToInt32() ?? 0;
        }

        private bool TableExists()
        {
            string commandText = string.Format("SELECT name = '{0}' FROM sqlite_master WHERE type='table' AND name='{0}'", TableName);

            return (Db.ExecuteScalar(commandText) != 0);
        }


        private bool ColumnExists(string columnName)
        {
            bool columnExists = false;
            IDataReader reader = Db.ExecuteReader("PRAGMA table_info(" + TableName + ")");
            while (reader.Read())
            {
                string col = reader["name"].FromDbString();
                if (string.Equals(col, columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    columnExists = true;
                }
            }
            return columnExists;
        }

        /// <summary>
        /// Creates the table
        /// </summary>
        private void CreateTable()
        {
            StringBuilder sbCommandColumns = new StringBuilder();

            sbCommandColumns.Append(String.Join(String.Format(",{0}", Environment.NewLine), m_columns.Select(ConstructCreateTableCommandColumnRow).ToArray()));

            if (Constraints() != null)
            {
                sbCommandColumns.AppendFormat(",{0}{1}", Environment.NewLine, String.Join(String.Format(",{0}", Environment.NewLine), Constraints()));
            }

            if (m_primaryKeyColumnNames.Count() > 1)
            {
                sbCommandColumns.AppendFormat(",{0}PRIMARY KEY({1})", Environment.NewLine, String.Join(",", m_primaryKeyColumnNames));
            }
          

            string commandText = string.Format("CREATE TABLE {0} ({2}{1}{2})", TableName, sbCommandColumns, Environment.NewLine);

            Db.ExecuteNonQuery(commandText);

            if (Triggers() != null)
            {
                foreach (var trigger in Triggers())
                {
                    Db.ExecuteNonQuery(trigger);
                }
            }

            

            CreateGeometryInfo();
        }

        private string ConstructCreateTableCommandColumnRow(ColumnInfo col)
        {
            if (IsSingleColumnPrimaryKey(col))
            {
                return String.Format(@"  {0} {1} PRIMARY KEY", GetColumnName(col), col.SqlType);
            }
            return String.Format(@"  {0} {1}", GetColumnName(col), col.SqlType);
        }

        private void CreateGeometryInfo()
        {
            var geometryColumns = m_columns.Where(col => col.SqlType == "LineString" || col.SqlType == "Point" || col.SqlType == "Polygon").ToArray();
            if (geometryColumns.Any())
            {
                var gpkgContentsManager = Db.GetTableManager<GpkgContentsManager>();
                var gpkgGeometryColumnsManager = Db.GetTableManager<GpkgGeometryColumnsManager>();

                gpkgContentsManager.Add(
                    new GpkgContents
                    {
                        TableName = TableName,
                        DataType = "features",
                        Identifier = TableName,
                        Description = TableName,
                        LastChange = DateTime.Now,
                        SrsId = Db.Srid
                    }
                    );

                foreach (ColumnInfo col in geometryColumns)
                {
                    gpkgGeometryColumnsManager.Add(
                        new GpkgGeometryColumn
                        {
                            TableName = TableName,
                            ColumnName = col.Name,
                            GeometryTypeName = col.SqlType,
                            SrsId = Db.Srid,
                            Z = 2,
                            M = 2
                        }
                        );
                }
            }
        }

        private bool IsSingleColumnPrimaryKey(ColumnInfo col)
        {
            return m_primaryKeyColumnNames.Count() == 1 && string.Equals(m_primaryKeyColumnNames.Single(), col.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Create a RTree spatial index
        /// </summary>
        public void ApplySpatialIndex()
        {
            string commandText = String.Format("SELECT CreateSpatialIndex('{0}', 'CENTRE_LINE_GEOMETRY')",TnfLinkManager.TnfLinkTableName);
            Db.ExecuteNonQuery(commandText);
        }

    

        public void Dispose()
        {
            if (InsertCmd != null)
            {
                InsertCmd.Dispose();
                InsertCmd = null;
            }

            if (SelectCmd != null)
            {
                SelectCmd.Dispose();
                SelectCmd = null;
            }

            if (SelectAllCmd != null)
            {
                SelectAllCmd.Dispose();
                SelectAllCmd = null;
            }

            if (SelectPageCmd != null)
            {
                SelectPageCmd.Dispose();
                SelectPageCmd = null;
            }

            if (UpdateCmd != null)
            {
                UpdateCmd.Dispose();
                UpdateCmd = null;
            }

            if (DeleteCmd != null)
            {
                DeleteCmd.Dispose();
                DeleteCmd = null;
            }

            if (CountCmd != null)
            {
                CountCmd.Dispose();
                CountCmd = null;
            }
        }

     
    }
}
