using OpenTNF.Library.Model;
using System.Data;
using System.Text;

namespace OpenTNF.Library
{
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
        protected IDbCommand IndexExistsCmd = null;
        private readonly string m_tableName;
        private string[] m_primaryKeyColumnNames;
        private readonly ColumnInfo[] m_missingNonEditableColumns = Array.Empty<ColumnInfo>();
        private readonly ColumnInfo[] m_existingColumns;

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
            InitColumns(columns);

            if (!TableExists())
            {
                CreateTable(columns);
                m_existingColumns = columns;
            }
            else
            {
                m_existingColumns = columns.Where(c => ColumnExists(c.Name)).ToArray();
                var missingColumns = columns.Where(c => !m_existingColumns.Contains(c)).ToArray();
                var missingMandatoryColumns = missingColumns.Where(c => c.Requirement == ColumnRequirement.Mandatory).ToArray();
                if (missingMandatoryColumns.Length != 0)
                {
                    var missingColumnsText = string.Join(", ", missingMandatoryColumns.Select(c => c.Name));
                    throw new OpenTnfException($"Mandatory columns are missing in table '{m_tableName}': {missingColumnsText}");
                }
                var existingColumnNames = m_existingColumns.Select(c => c.Name).ToArray();

                m_missingNonEditableColumns = missingColumns.Where(c => c.Requirement != ColumnRequirement.OptionalEditable).ToArray();
                m_primaryKeyColumnNames = m_primaryKeyColumnNames.Where(n => existingColumnNames.Contains(n)).ToArray();
            }
        }

        private string[] GetPrimaryKeyColumnNamesFromStr(string primaryKey)
        {
            if (string.IsNullOrEmpty(primaryKey)) return [];
            return primaryKey.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim()).ToArray();
        }

        public string TableName { get { return m_tableName; } }

        protected virtual bool ShallCreateGpkgContentsEntry => true;

        protected virtual string[] Constraints()
        {
            return null;
        }

        protected virtual IndexInfo[] Indices()
        {
            return null;
        }

        protected string[] ColumnNames => m_existingColumns.Select(c => c.Name).ToArray();

        public void CreateIndices()
        {
            if (Indices() != null)
            {
                foreach (IndexInfo index in Indices())
                {
                    if (!IndexExists(index))
                    {
                        Db.LogInfo($"Creating index {TableName}.{index.Name}");
                        Db.ExecuteNonQuery(index.Sql);
                    }
                }
            }
        }

        private bool IndexExists(IndexInfo index)
        {
            if (IndexExistsCmd == null)
            {
                IndexExistsCmd = Db.Command;
                IndexExistsCmd.Transaction = Db.GetSqLiteTransaction();

                string cmdText = "SELECT COUNT(*) FROM sqlite_master " +
                                 "WHERE type = 'index' " +
                                 $"AND lower(tbl_name) = lower('{TableName}') " +
                                 "AND lower(name) = lower(@indexName)";
                IndexExistsCmd.CommandText = cmdText;
                IndexExistsCmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter
                {
                    ParameterName = "@indexName",
                    DbType = DbType.String
                });
                IndexExistsCmd.Prepare();
            }

            ((IDbDataParameter)IndexExistsCmd.Parameters["@indexName"]).Value = index.Name;


            return IndexExistsCmd.ExecuteScalar().ToInt32() > 0;
        }

        protected virtual string[] Triggers()
        {
            return null;
        }

        private void InitColumns(ColumnInfo[] columns)
        {
            foreach (ColumnInfo ci in columns)
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
                    return DbType.String; // In SQLite, All Date/DateTime values are stored internally as String
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
            if (m_missingNonEditableColumns.Length != 0)
            {
                throw new OpenTnfException("Unable to add object because columns are missing in the table. " +
                                           $"Missing columns: {string.Join(", ", m_missingNonEditableColumns.Select(c => c.Name))}");
            }
            if (InsertCmd == null)
            {
                InsertCmd = Db.Command;
                InsertCmd.Transaction = Db.GetSqLiteTransaction();
                var columnList = new List<string>();
                var parameterList = new List<string>();

                foreach (var column in m_existingColumns)
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

            for (int paramNo = 0; paramNo < m_existingColumns.Length; paramNo++)
            {
                if (InsertCmd.Parameters[paramNo] is IDbDataParameter dbDataParameter)
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

                foreach (ColumnInfo column in m_existingColumns)
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
                    if (SelectCmd.Parameters[paramNo] is IDbDataParameter dbDataParameter)
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

        protected string GetExistingColumnNamesString()
        {
            string colStr = "";

            foreach (var column in m_existingColumns)
            {
                colStr += GetColumnName(column) + ", ";
            }

            colStr = colStr.Remove(colStr.LastIndexOf(", ", StringComparison.Ordinal));
            return colStr;
        }

        protected List<T> Get<T>(Func<IDataReader, T> parameter, int limit)
        {
            if (SelectAllCmd == null)
            {
                SelectAllCmd = Db.Command;
                SelectAllCmd.Transaction = Db.GetSqLiteTransaction();
                string cmdtext = string.Format("SELECT {0} FROM {1} LIMIT @limit", GetExistingColumnNamesString(), TableName);
                SelectAllCmd.CommandText = cmdtext;
                SelectAllCmd.Parameters.Add(
                    new System.Data.SQLite.SQLiteParameter { ParameterName = "@limit", DbType = DbType.Int64 });
                SelectAllCmd.Prepare();
            }

            if (SelectAllCmd.Parameters[0] is IDbDataParameter dbDataParameter)
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

        protected List<T> GetPage<T>(Func<IDataReader, T> parameter, int offset, int limit)
        {
            if (SelectPageCmd == null)
            {
                SelectPageCmd = Db.Command;
                SelectPageCmd.Transaction = Db.GetSqLiteTransaction();
                string colStr = "";

                foreach (var column in m_existingColumns)
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


            if (SelectPageCmd.Parameters[0] is IDbDataParameter dbDataParameter)
            {
                dbDataParameter.Value = offset;
            }

            dbDataParameter = SelectPageCmd.Parameters[1] as IDbDataParameter;

            if (dbDataParameter != null)
            {
                dbDataParameter.Value = limit;
            }

            var list = new List<T>();
            using (var reader = SelectPageCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(parameter(reader));
                }
            }

            return list;
        }

        protected int Update(object[] values)
        {
            if (m_missingNonEditableColumns.Length != 0)
            {
                throw new OpenTnfException("Unable to update object because columns are missing in the table. " +
                                           $"Missing columns: {string.Join(", ", m_missingNonEditableColumns.Select(c => c.Name))}");
            }
            if (UpdateCmd == null)
            {
                UpdateCmd = Db.Command;
                UpdateCmd.Transaction = Db.GetSqLiteTransaction();
                var setList = new List<string>();
                var whereList = new List<string>();

                foreach (var column in m_existingColumns)
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

            for (int paramNo = 0; paramNo < m_existingColumns.Length; paramNo++)
            {
                if (UpdateCmd.Parameters[paramNo] is IDbDataParameter dbDataParameter)
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
            if (m_missingNonEditableColumns.Length != 0)
            {
                throw new OpenTnfException("Unable to delete object because columns are missing in the table. " +
                                           $"Missing columns: {string.Join(", ", m_missingNonEditableColumns.Select(c => c.Name))}");
            }
            if (DeleteCmd == null)
            {
                DeleteCmd = Db.Command;
                DeleteCmd.Transaction = Db.GetSqLiteTransaction();
                var whereList = new List<string>();

                foreach (var column in m_existingColumns)
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

            for (int paramNo = 0; paramNo < orderdPrimaryKeyValue.Length; paramNo++)
            {
                if (DeleteCmd.Parameters[paramNo] is IDbDataParameter dbDataParameter)
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

                foreach (var column in m_existingColumns)
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

            for (int paramNo = 0; paramNo < values.Length; paramNo++)
            {
                if (CountCmd.Parameters[paramNo] is IDbDataParameter dbDataParameter)
                {
                    dbDataParameter.Value = values[paramNo] ?? DBNull.Value;
                }
            }


            return CountCmd.ExecuteScalar().ToInt32() ?? 0;
        }

        private bool TableExists()
        {
            string commandText = string.Format("SELECT name = '{0}' FROM sqlite_master WHERE type='table' AND name='{0}'", TableName);

            return Db.ExecuteScalar(commandText) != 0;
        }


        protected bool ColumnExists(string columnName)
        {
            bool columnExists = false;
            using (IDataReader reader = Db.ExecuteReader("PRAGMA table_info(" + TableName + ")"))
            {
                while (reader.Read())
                {
                    string col = reader["name"].FromDbString();
                    if (string.Equals(col, columnName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        columnExists = true;
                    }
                }
            }
            return columnExists;
        }

        /// <summary>
        /// Creates the table
        /// </summary>
        private void CreateTable(ColumnInfo[] columns)
        {
            StringBuilder sbCommandColumns = new StringBuilder();

            sbCommandColumns.Append(string.Join(string.Format(",{0}", Environment.NewLine), columns.Select(ConstructCreateTableCommandColumnRow).ToArray()));

            if (Constraints() != null)
            {
                sbCommandColumns.AppendFormat(",{0}{1}", Environment.NewLine, string.Join(string.Format(",{0}", Environment.NewLine), Constraints()));
            }

            if (m_primaryKeyColumnNames.Length > 1)
            {
                sbCommandColumns.AppendFormat(",{0}PRIMARY KEY({1})", Environment.NewLine, string.Join(",", m_primaryKeyColumnNames));
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

            if (ShallCreateGpkgContentsEntry)
            {
                CreateGpkgContentsAndGeometryColumnEntries(columns);
            }
        }

        private string ConstructCreateTableCommandColumnRow(ColumnInfo col)
        {
            if (IsSingleColumnPrimaryKey(col))
            {
                return string.Format(@"  {0} {1} PRIMARY KEY", GetColumnName(col), col.SqlType);
            }
            return string.Format(@"  {0} {1}", GetColumnName(col), col.SqlType);
        }

        private static readonly string[] m_gpkgGeometryTypes =
        {
            "GEOMETRY",
            "POINT",
            "LINESTRING",
            "POLYGON",
            "MULTIPOINT",
            "MULTILINESTRING",
            "MULTIPOLYGON",
            "GEOMETRYCOLLECTION"
        };

        private void CreateGpkgContentsAndGeometryColumnEntries(ColumnInfo[] columns)
        {
            var gpkgContentsManager = Db.GetTableManager<GpkgContentsManager>();
            var gpkgGeometryColumnsManager = Db.GetTableManager<GpkgGeometryColumnsManager>();

            ColumnInfo[] geometryColumns = columns
                .Where(col => m_gpkgGeometryTypes.Any(geomType => string.Equals(col.SqlType, geomType, StringComparison.InvariantCultureIgnoreCase)))
                .ToArray();
            if (geometryColumns.Length > 0)
            {
                gpkgContentsManager.Add(
                    new GpkgContents
                    {
                        TableName = TableName,
                        DataType = GpkgContentsManager.DataTypeFeatures,
                        Identifier = TableName,
                        Description = TableName,
                        LastChange = DateTime.Now,
                        SrsId = Db.Crs
                    }
                );
                if (Db.Crs != null)
                {
                    foreach (ColumnInfo col in geometryColumns)
                    {
                        short z = (short)((col.HasZ && Db.RequireZ) ? 1 : 0);
                        short m = (short)((col.HasM && Db.RequireM) ? 1 : 0);
                        gpkgGeometryColumnsManager.Add(
                            new GpkgGeometryColumn
                            {
                                TableName = TableName,
                                ColumnName = col.Name,
                                GeometryTypeName = col.SqlType,
                                SrsId = Db.Crs.Value,
                                Z = z,
                                M = m
                            }
                        );
                    }
                }
            }
            else
            {
                gpkgContentsManager.Add(
                    new GpkgContents
                    {
                        TableName = TableName,
                        DataType = GpkgContentsManager.DataTypeAttributes,
                        Identifier = TableName,
                        Description = TableName,
                        LastChange = DateTime.Now,
                        SrsId = null
                    }
                );
            }
        }

        private bool IsSingleColumnPrimaryKey(ColumnInfo col)
        {
            return m_primaryKeyColumnNames.Length == 1 && string.Equals(m_primaryKeyColumnNames.Single(), col.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Create a RTree spatial index
        /// </summary>
        public void ApplySpatialIndex()
        {
            string commandText = string.Format("SELECT CreateSpatialIndex('{0}', 'CENTRE_LINE_GEOMETRY')", TnfLinkManager.TnfLinkTableName);
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

            if (CountAllCmd != null)
            {
                CountAllCmd.Dispose();
                CountAllCmd = null;
            }

            if (IndexExistsCmd != null)
            {
                IndexExistsCmd.Dispose();
                IndexExistsCmd = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
