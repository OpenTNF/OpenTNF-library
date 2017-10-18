using System;

namespace OpenTNF.Library.Model
{
    public class GpkgMetadata
    {
        public int Id { get; set; }
        public string MdScope { get; set; }
        public string MdStandardUri { get; set; }
        public string MimeType { get; set; }
        public string MetaData { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is GpkgMetadata)
            {
                var c = obj as GpkgMetadata;

                if (Id.Equals(c.Id) &&
                    string.Equals(MdScope, c.MdScope) &&
                    string.Equals(MdStandardUri, c.MdStandardUri) &&
                    string.Equals(MimeType, c.MimeType) &&
                    string.Equals(MetaData, c.MetaData))
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(
                Id,
                MdScope,
                MdStandardUri,
                MimeType,
                MetaData);
        }

        public override string ToString()
        {
            return String.Format("GpkgMetadata: Id = {0}, MdScope = {1}, MdStandardUri = {2}, MimeType = {3}, MetaData = {4}",
                Id,
                MdScope,
                MdStandardUri,
                MimeType,
                MetaData);
        }
    }

    public class GpkgMetadataManager : TableManager
    {
        public static string GpkgMetadataTableName = "gpkg_metadata";

        public GpkgMetadataManager(GeoPackageDatabase db) : base(db, GpkgMetadataTableName, GetColumnInfos(), null)
        {
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "id",
                    SqlType = "INTEGER CONSTRAINT m_pk PRIMARY KEY ASC NOT NULL UNIQUE",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "srs_id",
                    SqlType = "TEXT NOT NULL DEFAULT 'dataset'",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "md_standard_uri",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "mime_type",
                    SqlType = "TEXT NOT NULL DEFAULT 'text/xml'",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "metadata",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
            };
        }

        public void Add(GpkgMetadata metadata)
        {
            Add(new object[]
                {
                    metadata.Id,
                    metadata.MdScope,
                    metadata.MdStandardUri,
                    metadata.MimeType,
                    metadata.MetaData
                });
        }
    }
}
