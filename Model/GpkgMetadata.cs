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
        public const string GpkgMetadataTableName = "gpkg_metadata";

        public GpkgMetadataManager(GeoPackageDatabase db) : base(db, GpkgMetadataTableName, GetColumnInfos(), null)
        {
        }

        protected override bool ShallCreateGpkgContentsEntry => false;

        protected override string[] Triggers()
        {
            return new[]
            {
                @"CREATE TRIGGER 'gpkg_metadata_md_scope_update' BEFORE UPDATE OF 'md_scope' ON 'gpkg_metadata' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'update on table gpkg_metadata violates constraint: md_scope must be one of undefined | fieldSession | collectionSession | series | dataset | featureType | feature | attributeType | attribute | tile | model | catalogue | schema | taxonomy software | service | collectionHardware | nonGeographicDataset | dimensionGroup') WHERE NOT(NEW.md_scope IN ('undefined','fieldSession','collectionSession','series','dataset', 'featureType','feature','attributeType','attribute','tile','model', 'catalogue','schema','taxonomy','software','service', 'collectionHardware','nonGeographicDataset','dimensionGroup')); END",
                @"CREATE TRIGGER 'gpkg_metadata_md_scope_insert' BEFORE INSERT ON 'gpkg_metadata' FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'insert on table gpkg_metadata violates constraint: md_scope must be one of undefined | fieldSession | collectionSession | series | dataset | featureType | feature | attributeType | attribute | tile | model | catalogue | schema | taxonomy software | service | collectionHardware | nonGeographicDataset | dimensionGroup') WHERE NOT(NEW.md_scope IN ('undefined','fieldSession','collectionSession','series','dataset', 'featureType','feature','attributeType','attribute','tile','model', 'catalogue','schema','taxonomy','software','service', 'collectionHardware','nonGeographicDataset','dimensionGroup')); END"
            };
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
