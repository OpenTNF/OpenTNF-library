namespace OpenTNF.Library
{
    public class CreateGeoPackageDatabaseParameters
    {
        /// <summary>
        /// If set, will create columns in tnf_link to store topology level information.
        /// </summary>
        public bool HasTopologyLevel { get; set; }
        /// <summary>
        /// The EPSG code of the default SRS ID.
        /// </summary>
        public int Crs { get; set; }

        public bool ExcludeM { get; set; }
        public bool ExcludeZ { get; set; }
        /// <summary>
        /// Identifier of the creator of the dataset. 
        /// Will be stored in the metadata key 'TNF_DATASET_IDENTIFIER'.
        /// </summary>
        public string DatasetIdentifier { get; set; }
        /// <summary>
        /// The view date of the dataset. If set, it will be stored in the metadata key 'TNF_VIEW_DATE'.
        /// </summary>
        public DateTime? ViewDate { get; set; }
        /// <summary>
        /// A <see cref="DataSetType"/> declaring what type of dataset the database contains.
        /// Will be stored in the metadata key 'TNF_DATASET_TYPE'.
        /// </summary>
        public DataSetType DataSetType { get; set; }
        /// <summary>
        /// If set, create all OpenTNF tables. If not set, only create OpenTNF tables when inserting data.
        /// </summary>
        public bool CreateTables { get; set; }
        /// <summary>
        /// Spatial reference system information will be automatically downloaded from the given URL.
        /// </summary>
        public string SpatialRefSysUrlFormat { get; set; } = null;
    }
}
