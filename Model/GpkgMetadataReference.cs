using System;

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
        public static string GpkgMetadataReferenceTableName = "gpkg_metadata_reference";

        public GpkgMetadataReferenceManager(GeoPackageDatabase db) : base(db, GpkgMetadataReferenceTableName, GetColumnInfos(), null)
        {
        }

        protected override string[] Constraints()
        {
            return new []
                {
                    String.Format("CONSTRAINT crmr_mfi_fk FOREIGN KEY (md_file_id) REFERENCES {0}(id)", GpkgMetadataManager.GpkgMetadataTableName),
                    String.Format("CONSTRAINT crmr_mpi_fk FOREIGN KEY (md_parent_id) REFERENCES {0}(id)", GpkgMetadataManager.GpkgMetadataTableName)
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
                    metadataReference.Timestamp?.ToUniversalTime(),
                    metadataReference.MdFileId,
                    metadataReference.MdParentId
                });
        }
    }
}
