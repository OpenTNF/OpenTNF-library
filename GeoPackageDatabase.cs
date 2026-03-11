using log4net;
using OpenTNF.Library.Model;
using OpenTNF.Library.Properties;
using System.Collections;
using System.Data;
using System.Data.SQLite;

namespace OpenTNF.Library
{
    public enum DataSetType
    {
        Snapshot,
        Updates
    };

    public class GeoPackageDatabase : IDisposable
    {
        // These constants are set in the database header for created files
        private const int DbHeaderAppicationId = 0x47504B47; // GPKG
        private const int DbHeaderUserVersion = 10100; // GeoPackage version: 1.1.0

        private readonly IDbConnection m_connection;
        private IDbTransaction m_transaction;
        private readonly string m_filename;
        private readonly ILog m_logger;
        private int? m_crs;

        public bool HasTopologyLevel { get; private set; }

        public bool RequireZ
        {
            get; private set;
        }

        public bool RequireM
        {
            get; private set;
        }

        readonly Hashtable m_tableManagers = new Hashtable();

        public int? Crs
        {
            get
            {
                return m_crs;
            }
        }

        public GeoPackageDatabase(string filename)
        {
            m_filename = filename;
            m_connection = new SQLiteConnection(SQLiteConnectionsStringHandler.GetSQLiteConnectionString(HandleUncPath(m_filename)));
        }
        public GeoPackageDatabase(string filename, ILog logger)
        {
            m_logger = logger;
            m_filename = filename;
            m_connection = new SQLiteConnection(SQLiteConnectionsStringHandler.GetSQLiteConnectionString(HandleUncPath(m_filename)));
        }

        public T GetTableManager<T>() where T : TableManager
        {
            Type type = typeof(T);

            if (m_tableManagers.ContainsKey(type))
            {
                return (T)m_tableManagers[type];
            }

            T tableManager = (T)Activator.CreateInstance(typeof(T), (object)this);
            m_tableManagers.Add(typeof(T), tableManager);
            return tableManager;
        }

        public IDbCommand Command
        {
            get
            {
                if (m_connection != null && m_connection.State != ConnectionState.Broken && m_connection.State != ConnectionState.Closed)
                {
                    return m_connection.CreateCommand();
                }

                return null;
            }
        }

        public void CloseConnection()
        {
            if (m_connection != null && m_connection.State != ConnectionState.Closed)
            {
                m_connection.Close();

                // To release database
                GC.Collect();
            }
        }

        internal IDbTransaction GetSqLiteTransaction()
        {
            return m_transaction;
        }

        public void EndTransactionCommit()
        {
            if (InTransaction())
            {
                m_transaction.Commit();
                m_transaction = null;
            }
        }

        public void EndTransactionAbort()
        {
            if (InTransaction())
            {
                m_transaction.Rollback();
                m_transaction = null;
            }
        }

        public bool InTransaction()
        {
            return m_transaction != null;
        }

        public void BeginTransaction()
        {
            OpenConnection();

            m_transaction = m_connection.BeginTransaction();
        }

        public bool TableExists(string tableName)
        {
            OpenConnection();
            using (IDbCommand cmd = m_connection.CreateCommand())
            {
                cmd.CommandText = "SELECT name FROM sqlite_master WHERE name='" + tableName + "'";
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    return rdr.Read();
                }
            }
        }

        public bool ColumnExists(string tableName, string columnName)
        {
            OpenConnection();
            using (IDbCommand cmd = m_connection.CreateCommand())
            {
                cmd.CommandText = $"select * from {tableName} where 1=0";
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    DataTable dt = rdr.GetSchemaTable();
                    foreach (DataRow row in dt.Rows)
                    {
                        if (columnName.Equals(row["ColumnName"].ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }

                }
            }
            return false;
        }
        public enum TnfExtenttype { Node = 1, PointOnRefLink = 4, SegmentOnRefLink = 8, Road = 16, Turn = 64, RoadWithHost = 256 };
        public enum TnfAttributeformat { Text, Binary };

        public class InsertItem
        {
            public string Name;
            public DbType Type;
            public object Value;

            public InsertItem(string name, DbType type, object value)
            {
                Name = name;
                Type = type;
                Value = value;
            }
        }

        /// <summary>
        /// Creates a GeoPackage database with the OpenTNF extension.
        /// </summary>
        public bool CreateGeoPackageDatabase(CreateGeoPackageDatabaseParameters parameters)
        {
            m_logger?.Debug("Into CreateGeoPackageDatabase");
            HasTopologyLevel = parameters.HasTopologyLevel;
            m_crs = parameters.Crs;
            RequireM = !parameters.ExcludeM;
            RequireZ = !parameters.ExcludeZ;
            if (File.Exists(m_filename))
            {
                try
                {
                    File.Delete(m_filename);
                }
                catch (IOException)
                {
                    return false;
                }
            }

            m_logger?.Debug("OpenConnection");
            OpenConnection();

            bool shallCommit = false;
            if (!InTransaction())
            {
                m_logger?.Debug("BeginTransaction");
                BeginTransaction();
                shallCommit = true;
            }
            m_logger?.Debug("SetDatabaseHeaders");
            SetDatabaseHeaders();
            m_logger?.Debug("CreateGpkgTables");
            CreateGpkgTables();

            m_logger?.Debug("TnfMetadat");
            var tnfMetadataManager = GetTableManager<TnfMetadataManager>();

            tnfMetadataManager.Add(new TnfMetadata { MetaKey = "TNF_VERSION", MetaValue = Resource.OpenTNFVersion });
            tnfMetadataManager.Add(new TnfMetadata { MetaKey = "TNF_DATASET_IDENTIFIER", MetaValue = parameters.DatasetIdentifier });
            tnfMetadataManager.Add(new TnfMetadata
            {
                MetaKey = "TNF_DATASET_TIMESTAMP",
                MetaValue = DateTime.Now.ToDateTimeString()
            });
            tnfMetadataManager.Add(new TnfMetadata
            {
                MetaKey = "TNF_VIEW_DATE",
                MetaValue = parameters.ViewDate.ToDateString() ?? string.Empty
            });
            tnfMetadataManager.Add(new TnfMetadata { MetaKey = "TNF_CRS_NAME", MetaValue = "EPSG:" + parameters.Crs });
            tnfMetadataManager.Add(new TnfMetadata
            {
                MetaKey = "TNF_DATASET_TYPE",
                MetaValue = parameters.DataSetType.ToString().ToUpper()
            });
            tnfMetadataManager.Add(new TnfMetadata { MetaKey = "TNF_SPATIAL_ATTRIBUTE_ENCODING", MetaValue = Resource.SpatialAttributeEncoding });
            if (parameters.CreateTables)
            {
                CreateAllTables();
            }
            m_logger?.Debug("GpkgContents");
            var gpkgContentsManager = GetTableManager<GpkgContentsManager>();
            gpkgContentsManager.UpdateSrsId(parameters.Crs);

            var gpkgGeometryColumnsManager = GetTableManager<GpkgGeometryColumnsManager>();
            gpkgGeometryColumnsManager.UpdateSrsId(parameters.Crs);
            m_logger?.Debug("GpkgSpatialRefSys");
            var gpkgSpatialRefSysManager = GetTableManager<GpkgSpatialRefSysManager>();
            gpkgSpatialRefSysManager.AddOrUpdateFromUrl(parameters.SpatialRefSysUrlFormat, parameters.Crs);
            m_logger?.Debug("CheckSrsId");
            CheckSrsId();
            if (shallCommit)
            {
                m_logger?.Debug("EndTransactionCommit");
                EndTransactionCommit();
            }
            return true;
        }

        private void SetDatabaseHeaders()
        {
            using (IDbCommand cmd = m_connection.CreateCommand())
            {
                cmd.CommandText = $"PRAGMA main.application_id = {DbHeaderAppicationId}";
                cmd.ExecuteNonQuery();
                cmd.CommandText = $"PRAGMA main.user_version = {DbHeaderUserVersion}";
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates all needed tables for a Geopackage-file.
        /// </summary>
        public void CreateGpkgTables()
        {
            GetTableManager<GpkgContentsManager>();
            var gpkgExtensionsManager = GetTableManager<GpkgExtensionsManager>();
            gpkgExtensionsManager.Add(new GpkgExtension
            {
                ExtensionName = "triona_opentnf",
                Definition = "http://opentnf.org/geopackage",
                Scope = "read-write"
            });
            GetTableManager<GpkgGeometryColumnsManager>();
            GetTableManager<GpkgMetadataManager>();
            GetTableManager<GpkgMetadataReferenceManager>();
            var gpkgSpatialRefSysManager = GetTableManager<GpkgSpatialRefSysManager>();
            var spatialRefSystems = new[]
            {
                new GpkgSpatialRefSys
                {
                    SrsName = "Undefined cartesian SRS",
                    SrsId = -1,
                    Organization = "NONE",
                    OrganizationCoordsysId = -1,
                    Definition = "undefined",
                    Description = "undefined cartesian coordinate reference system"
                },
                new GpkgSpatialRefSys
                {
                    SrsName = "Undefined geographic SRS",
                    SrsId = 0,
                    Organization = "NONE",
                    OrganizationCoordsysId = 0,
                    Definition = "undefined",
                    Description = "undefined geographic coordinate reference system"
                },
                new GpkgSpatialRefSys
                {
                    SrsName = "SWEREF99 TM",
                    SrsId = 3006,
                    Organization = "EPSG",
                    OrganizationCoordsysId = 3006,
                    Definition = "PROJCS[\"SWEREF99 TM\",GEOGCS[\"SWEREF99\",DATUM[\"SWEREF99\",SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]],TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\",\"6619\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4619\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",15],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AUTHORITY[\"EPSG\",\"3006\"]]",
                },
                new GpkgSpatialRefSys
                {
                    SrsName = "SWEREF99 18 00",
                    SrsId = 3011,
                    Organization = "EPSG",
                    OrganizationCoordsysId = 3011,
                    Definition = "PROJCS[\"SWEREF99 18 00\",GEOGCS[\"SWEREF99\",DATUM[\"SWEREF99\",SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]],TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\",\"6619\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4619\"]],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",18],PARAMETER[\"scale_factor\",1],PARAMETER[\"false_easting\",150000],PARAMETER[\"false_northing\",0],AUTHORITY[\"EPSG\",\"3011\"],AXIS[\"y\",EAST],AXIS[\"x\",NORTH]]",
                },
                new GpkgSpatialRefSys
                {
                    SrsName = "WGS 84 geodetic",
                    SrsId = 4326,
                    Organization = "EPSG",
                    OrganizationCoordsysId = 4326,
                    Definition = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]",
                    Description = "longitude/latitude coordinates in decimal degrees on the WGS 84 spheroid"
                },
                new GpkgSpatialRefSys
                {
                    SrsName = "ETRS89 / UTM zone 33N",
                    SrsId = 25833,
                    Organization = "EPSG",
                    OrganizationCoordsysId = 25833,
                    Definition = "PROJCS[\"ETRS89 / UTM zone 33N\",GEOGCS[\"ETRS89\",DATUM[\"European_Terrestrial_Reference_System_1989\",SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]],AUTHORITY[\"EPSG\",\"6258\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4258\"]],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",15],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],AUTHORITY[\"EPSG\",\"25833\"],AXIS[\"Easting\",EAST],AXIS[\"Northing\",NORTH]]",
                }
            };
            foreach (var refSys in spatialRefSystems)
            {
                gpkgSpatialRefSysManager.Add(refSys);
            }

        }

        /// <summary>
        /// Creates all OpenTNF tables in the file that do not already exist.
        /// Assumes that the file exists and contains the default tables defined by the GeoPackage standard.
        /// </summary>
        /// <param name="hasTopologyLevel">
        /// If the table tnf_link does not already exist, this parameter determines whether or not it will be created with topology levels.
        /// </param>
        public void CreateAllTables(bool? hasTopologyLevel = null)
        {
            CheckLinkHasTopologyLevel(hasTopologyLevel);
            GetTableManager<TnfNetworkManager>();
            GetTableManager<TnfNetworkReferenceManager>();

            GetTableManager<TnfLinkSequenceManager>();
            GetTableManager<TnfLinkManager>();
            GetTableManager<TnfNodeManager>();
            GetTableManager<TnfConnectionPortManager>();

            GetTableManager<TnfCatalogueManager>();
            GetTableManager<TnfPropertyObjectManager>();
            GetTableManager<TnfPropertyManager>();
            GetTableManager<TnfPropertyObjectTypeManager>();
            GetTableManager<TnfPropertyObjectPropertyTypeManager>();
            GetTableManager<TnfPropertyObjectTypeValidForTypeOfTransportManager>();
            GetTableManager<TnfValueDomainManager>();
            GetTableManager<TnfStructuredValueDomainPropertyTypeManager>();
            GetTableManager<TnfValidValueManager>();
            GetTableManager<TnfSecondaryLrsManager>();
            GetTableManager<TnfSecondaryLrsIdentityManager>();

            GetTableManager<TnfChangeTransactionManager>();
            GetTableManager<TnfChangeManager>();
            GetTableManager<TnfAffectedNetworkElementManager>();

            GetTableManager<TnfTopologyLevelManager>();

            GetTableManager<TnfAreaManager>();
            GetTableManager<TnfTaskManager>();
            GetTableManager<TnfTaskEditableTypeManager>();

            GetTableManager<TnfMetadataManager>();

            GetTableManager<TnfCatalogueTagManager>();
        }

        public void CreateForeignKeyIndexes()
        {
            GetTableManager<TnfNetworkReferenceManager>().CreateIndices();
            GetTableManager<TnfPropertyManager>().CreateIndices();
            GetTableManager<TnfLinkManager>().CreateIndices();
            GetTableManager<TnfConnectionPortManager>().CreateIndices();
        }

        public void OpenGeoPackageDatabase(bool? hasTopologyLevel = null)
        {
            OpenConnection();
            CheckSrsId();
            CheckLinkHasTopologyLevel(hasTopologyLevel);
        }

        private void CheckSrsId()
        {
            using (IDbCommand cmd = m_connection.CreateCommand())
            {
                cmd.CommandText = string.Format("SELECT table_name, srs_id FROM {0} WHERE LOWER(table_name) = '{1}' OR LOWER(table_name) = '{2}'",
                    GpkgGeometryColumnsManager.GpkgGeometryColumnsTableName, TnfLinkSequenceManager.TnfLinkSequenceTableName.ToLower(), TnfNodeManager.TnfNodeTableName.ToLower());

                int? linkSequenceSrsId = null;
                int? nodeSrsId = null;
                bool tablesExist = false;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tablesExist = true;
                        string tableName = reader["table_name"].FromDbString();
                        if (tableName.ToLower().Equals(TnfLinkSequenceManager.TnfLinkSequenceTableName.ToLower()))
                        {
                            linkSequenceSrsId = reader["srs_id"].ToInt32();
                        }
                        else if (tableName.ToLower().Equals(TnfNodeManager.TnfNodeTableName.ToLower()))
                        {
                            nodeSrsId = reader["srs_id"].ToInt32();
                        }
                    }
                }

                if (!tablesExist)
                {
                    return;
                }

                if (!linkSequenceSrsId.HasValue || !nodeSrsId.HasValue)
                {
                    m_crs = null;
                    return;
                }
                if (linkSequenceSrsId.Value != nodeSrsId.Value)
                {
                    throw new OpenTnfException(Resources.OpenTnfDatabaseHasMoreThanOneSrsId, string.Format("tnf_link={0}, tnf_node={1}", linkSequenceSrsId, nodeSrsId));
                }

                m_crs = linkSequenceSrsId.Value;
            }
        }

        private void CheckLinkHasTopologyLevel(bool? hasTopologyLevel)
        {
            using (IDbCommand cmd = m_connection.CreateCommand())
            {
                cmd.CommandText = string.Format("PRAGMA table_info({0});", TnfLinkManager.TnfLinkTableName);
                bool tnfLinkTableExists = false;
                bool tnfLinkHasTopologyLevels = false;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tnfLinkTableExists = true;
                        string colName = reader["name"].FromDbString();
                        if (colName.ToLower().Equals("topology_level_oid"))
                        {
                            tnfLinkHasTopologyLevels = true;
                            break;
                        }
                    }
                }

                if (tnfLinkTableExists)
                {
                    HasTopologyLevel = tnfLinkHasTopologyLevels;
                }
                else
                {
                    HasTopologyLevel = hasTopologyLevel ?? HasTopologyLevel;
                }
            }
        }

        internal void OpenConnection()
        {
            if (m_connection != null && m_connection.State != ConnectionState.Open)
            {
                m_connection.Open();
            }
        }

        public void DisableForeignKeysConstraints()
        {
            OpenConnection();
            ExecuteNonQuery("PRAGMA foreign_keys = OFF");
        }

        public void EnableForeignKeysConstraints()
        {
            OpenConnection();
            ExecuteNonQuery("PRAGMA foreign_keys = ON");
        }

        public int ExecuteNonQuery(string commandText)
        {
            OpenConnection();
            using (IDbCommand command = m_connection.CreateCommand())
            {
                command.CommandText = commandText;
                return command.ExecuteNonQuery();
            }
        }

        internal int ExecuteScalar(string commandText)
        {
            OpenConnection();
            using (IDbCommand command = m_connection.CreateCommand())
            {
                command.CommandText = commandText;
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        internal IDataReader ExecuteReader(string commandText)
        {
            OpenConnection();
            using (IDbCommand command = m_connection.CreateCommand())
            {
                command.CommandText = commandText;
                IDataReader idataReader = command.ExecuteReader();
                return idataReader;
            }
        }

        private string HandleUncPath(string path)
        {
            if (path != null && path.StartsWith(@"\\", StringComparison.InvariantCulture)
                && !path.StartsWith(@"\\\\", StringComparison.InvariantCulture))
            {
                return @"\\" + path;
            }

            return path;
        }

        internal void LogInfo(string message)
        {
            m_logger?.Info(message);
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (DictionaryEntry entry in m_tableManagers)
            {
                var tableManager = entry.Value as TableManager;
                tableManager?.Dispose();
            }

            EndTransactionAbort();
            CloseConnection();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
