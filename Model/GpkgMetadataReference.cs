namespace OpenTNF.Library.Model
{
    public class GpkgMetadataReference
    {
        public string ReferenceScope { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public int? RowIdValue { get; set; }
        public DateTime? Timestamp { get; set; }
        public int MdFileId { get; set; }
        public int? MdParentId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is GpkgMetadataReference)
            {
                var c = obj as GpkgMetadataReference;

                if (string.Equals(ReferenceScope, c.ReferenceScope) &&
                    string.Equals(TableName, c.TableName) &&
                    string.Equals(ColumnName, c.ColumnName) &&
                    RowIdValue.Equals(c.RowIdValue) &&
                    Timestamp.Equals(c.Timestamp) &&
                    MdFileId.Equals(c.MdFileId) &&
                    MdParentId.Equals(c.MdParentId))
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(
                ReferenceScope,
                TableName,
                ColumnName,
                RowIdValue,
                Timestamp,
                MdFileId,
                MdParentId);
        }

        public override string ToString()
        {
            return String.Format("GpkgMetadataReference: ReferenceScope = {0}, TableName = {1}, ColumnName = {2}, RowIdValue = {3}, " +
                                 "Timestamp = {4}, MdFileId = {5}, MdParentId = {6}",
                ReferenceScope,
                TableName,
                ColumnName,
                RowIdValue,
                Timestamp,
                MdFileId,
                MdParentId);
        }
    }

    public class GpkgMetadataReferenceManager : TableManager
    {
        public const string GpkgMetadataReferenceTableName = "gpkg_metadata_reference";

        public GpkgMetadataReferenceManager(GeoPackageDatabase db) : base(db, GpkgMetadataReferenceTableName, GetColumnInfos(), null)
        {
        }

        protected override bool ShallCreateGpkgContentsEntry => false;

        protected override string[] Constraints()
        {
            return new[]
                {
                    String.Format("CONSTRAINT crmr_mfi_fk FOREIGN KEY (md_file_id) REFERENCES {0}(id)", GpkgMetadataManager.GpkgMetadataTableName),
                    String.Format("CONSTRAINT crmr_mpi_fk FOREIGN KEY (md_parent_id) REFERENCES {0}(id)", GpkgMetadataManager.GpkgMetadataTableName)
                };
        }

        protected override string[] Triggers()
        {
            return new[]
            {
                "CREATE TRIGGER 'gpkg_metadata_reference_reference_scope_update' BEFORE UPDATE OF 'reference_scope' ON 'gpkg_metadata_reference' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'update on table gpkg_metadata_reference violates constraint: referrence_scope must be one of \"geopackage\", \"table\", \"column\", \"row\", \"row/col\"') WHERE NOT NEW.reference_scope IN ('geopackage','table','column','row','row/col'); END",
                "CREATE TRIGGER 'gpkg_metadata_reference_column_name_insert' BEFORE INSERT ON 'gpkg_metadata_reference' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'insert on table gpkg_metadata_reference violates constraint: column name must be NULL when reference_scope is \"geopackage\", \"table\" or \"row\"') WHERE (NEW.reference_scope IN ('geopackage','table','row') AND NEW.column_name IS NOT NULL); SELECT RAISE(ABORT, 'insert on table gpkg_metadata_reference violates constraint: column name must be defined for the specified table when reference_scope is \"column\" or \"row/col\"') WHERE (NEW.reference_scope IN ('column','row/col') AND NOT NEW.table_name IN ( SELECT name FROM SQLITE_MASTER WHERE type = 'table' AND name = NEW.table_name AND sql LIKE ('%' || NEW.column_name || '%'))); END",
                "CREATE TRIGGER 'gpkg_metadata_reference_timestamp_update' BEFORE UPDATE OF 'timestamp' ON 'gpkg_metadata_reference' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'update on table gpkg_metadata_reference violates constraint: timestamp must be a valid time in ISO 8601 \"yyyy-mm-ddThh:mm:ss.cccZ\" form') WHERE NOT (NEW.timestamp GLOB '[1-2][0-9][0-9][0-9]-[0-1][0-9]-[0-3][0-9]T[0-2][0-9]:[0-5][0-9]:[0-5][0-9].[0-9][0-9][0-9]Z' AND strftime('%s',NEW.timestamp) NOT NULL); END",
                "CREATE TRIGGER 'gpkg_metadata_reference_column_name_update' BEFORE UPDATE OF column_name ON 'gpkg_metadata_reference' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'update on table gpkg_metadata_reference violates constraint: column name must be NULL when reference_scope is \"geopackage\", \"table\" or \"row\"') WHERE (NEW.reference_scope IN ('geopackage','table','row') AND NEW.column_nameIS NOT NULL); SELECT RAISE(ABORT, 'update on table gpkg_metadata_reference violates constraint: column name must be defined for the specified table when reference_scope is \"column\" or \"row/col\"') WHERE (NEW.reference_scope IN ('column','row/col') AND NOT NEW.table_name IN ( SELECT name FROM SQLITE_MASTER WHERE type = 'table' AND name = NEW.table_name AND sql LIKE ('%' || NEW.column_name || '%'))); END",
                "CREATE TRIGGER 'gpkg_metadata_reference_row_id_value_update' BEFORE UPDATE OF 'row_id_value' ON 'gpkg_metadata_reference' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'update on table gpkg_metadata_reference violates constraint: row_id_value must be NULL when reference_scope is \"geopackage\", \"table\" or \"column\"') WHERE NEW.reference_scope IN ('geopackage','table','column') AND NEW.row_id_value IS NOT NULL; SELECT RAISE(ABORT, 'update on table gpkg_metadata_reference violates constraint: row_id_value must exist in specified table when reference_scope is \"row\" or \"row/col\"') WHERE NEW.reference_scope IN ('row','row/col') AND NOT EXISTS (SELECT rowid FROM (SELECT NEW.table_name AS table_name) WHERE rowid = NEW.row_id_value); END",
                "CREATE TRIGGER 'gpkg_metadata_reference_reference_scope_insert' BEFORE INSERT ON 'gpkg_metadata_reference' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'insert on table gpkg_metadata_reference violates constraint: reference_scope must be one of \"geopackage\", table\", \"column\", \"row\", \"row/col\"') WHERE NOT NEW.reference_scope IN ('geopackage','table','column','row','row/col'); END",
                "CREATE TRIGGER 'gpkg_metadata_reference_row_id_value_insert' BEFORE INSERT ON 'gpkg_metadata_reference' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'insert on table gpkg_metadata_reference violates constraint: row_id_value must be NULL when reference_scope is \"geopackage\", \"table\" or \"column\"') WHERE NEW.reference_scope IN ('geopackage','table','column') AND NEW.row_id_value IS NOT NULL; SELECT RAISE(ABORT, 'insert on table gpkg_metadata_reference violates constraint: row_id_value must exist in specified table when reference_scope is \"row\" or \"row/col\"') WHERE NEW.reference_scope IN ('row','row/col') AND NOT EXISTS (SELECT rowid FROM (SELECT NEW.table_name AS table_name) WHERE rowid = NEW.row_id_value); END",
                "CREATE TRIGGER 'gpkg_metadata_reference_timestamp_insert' BEFORE INSERT ON 'gpkg_metadata_reference' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'insert on table gpkg_metadata_reference violates constraint: timestamp must be a valid time in ISO 8601 \"yyyy-mm-ddThh:mm:ss.cccZ\" form') WHERE NOT (NEW.timestamp GLOB '[1-2][0-9][0-9][0-9]-[0-1][0-9]-[0-3][0-9]T[0-2][0-9]:[0-5][0-9]:[0-5][0-9].[0-9][0-9][0-9]Z' AND strftime('%s',NEW.timestamp) NOT NULL); END"
            };
        }


        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "reference_scope",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
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
                    Name = "row_id_value",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "timestamp",
                    SqlType = "DATETIME NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ','now'))",
                    DataType = Type.GetType("System.DateTime")
                },
                new ColumnInfo
                {
                    Name = "md_file_id",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "md_parent_id",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32")
                },
            };
        }

        public void Add(GpkgMetadataReference metadataReference)
        {
            Add(new object[]
                {
                    metadataReference.ReferenceScope,
                    metadataReference.TableName,
                    metadataReference.ColumnName,
                    metadataReference.RowIdValue,
                    metadataReference.Timestamp?.ToDateTimeString(),
                    metadataReference.MdFileId,
                    metadataReference.MdParentId
                });
        }
    }
}
