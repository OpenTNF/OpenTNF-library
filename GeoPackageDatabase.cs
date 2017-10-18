using System.Diagnostics;
using System.IO;
using System;
using System.Data;
using System.Data.SQLite;
using System.Collections;
using OpenTNF.Library.Model;
using OpenTNF.Library.Properties;

namespace OpenTNF.Library
{
    public enum DataSetType
    {
        Snapshot,
        Updates
    };

    public class GeoPackageDatabase : IDisposable
    {
        private readonly IDbConnection m_connection;
        private IDbTransaction m_transaction;
        private readonly string m_filename;
        private int? m_srid;
        public bool HasTopologyLevel { get; private set; }

        readonly Hashtable m_tableManagers = new Hashtable();

        public int Srid
        {
            get
            {
                if (!m_srid.HasValue)
                {
                    OpenGeoPackageDatabase();
                }
                // ReSharper disable once PossibleInvalidOperationException
                // Kontrolleras i OpenGeoPackageDatabase
                return m_srid.Value;
            }
        }

        public GeoPackageDatabase(string filename)
        {
            SQLiteInteropLoader.LoadSQLiteInterop();
            m_filename = filename;
            m_connection = new SQLiteConnection("Data Source=" + HandleUncPath(m_filename) + ";Version=3;New=True;Compress=True;");
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
                        if (columnName.ToLower() == row["ColumnName"].ToString().ToLower())
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
        /// <param name="srid">Default SRS ID.</param>
        /// <param name="gtransSystem">The name identifying the coordinate system for GTrans coordinate transformations.</param>
        /// <param name="datasetIdentifier">
        /// Identifier of the creator of the dataset. 
        /// Will be stored in the metadata key 'TNF_DATASET_IDENTIFIER'.
        /// </param>
        /// <param name="dataSetType">
        /// A <see cref="DataSetType"/> declaring what type of dataset the database contains.
        /// Will be stored in the metadata key 'TNF_DATASET_TYPE'.
        /// </param>
        /// <param name="viewDate">The view date of the dataset. If set, it will be stored in the metadata key 'TNF_VIEW_DATE'.</param>
        /// <param name="hasTopologyLevel">If set, will create columns in tnf_link to store topology level information.</param>
        /// <param name="createTables">If set, create all OpenTNF tables. If not set, only create OpenTNF tables when inserting data.</param>
        /// <param name="spatialRefSysUrlFormat">Spatial reference system information will be automatically downloaded from the given URL.</param>
        public bool CreateGeoPackageDatabase(int srid, string gtransSystem, string datasetIdentifier, DataSetType dataSetType, DateTime? viewDate,
            bool hasTopologyLevel = false, bool createTables = false, string spatialRefSysUrlFormat = GpkgSpatialRefSysManager.DefaultSpatialRefSysUrlFormat)
        {
            HasTopologyLevel = hasTopologyLevel;
            m_srid = srid;

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
            
            File.WriteAllBytes(m_filename, Resource.origin);

            OpenGeoPackageDatabase();

            var tnfMetadataManager = GetTableManager<TnfMetadataManager>();

            tnfMetadataManager.Add(new TnfMetadata {MetaKey = "TNF_VERSION", MetaValue = Resource.OpenTNFVersion});
            tnfMetadataManager.Add(new TnfMetadata {MetaKey = "TNF_DATASET_IDENTIFIER", MetaValue = datasetIdentifier});
            tnfMetadataManager.Add(new TnfMetadata
                {
                    MetaKey = "TNF_DATASET_TIMESTAMP",
                    MetaValue = DateTime.Now.ToString("O")
                });
            tnfMetadataManager.Add(new TnfMetadata
            {
                MetaKey = "TNF_VIEW_DATE",
                MetaValue = viewDate?.ToString("O") ?? string.Empty
            });
            tnfMetadataManager.Add(new TnfMetadata {MetaKey = "TNF_CRS_NAME", MetaValue = "EPSG:" + srid});
            tnfMetadataManager.Add(new TnfMetadata
            {
                MetaKey = "TNF_DATASET_TYPE",
                MetaValue = dataSetType.ToString().ToUpper()
            });
            tnfMetadataManager.Add(new TnfMetadata {MetaKey = "TNF_SPATIAL_ATTRIBUTE_ENCODING", MetaValue = Resource.SpatialAttributeEncoding});
            if (gtransSystem != null)
            {
                tnfMetadataManager.Add(new TnfMetadata {MetaKey = "GT_COORD_SYSTEM_ID", MetaValue = gtransSystem});
            }
            if (createTables)
            {
                CreateAllTables();
            }
            var gpkgContentsManager = GetTableManager<GpkgContentsManager>();
            gpkgContentsManager.UpdateSrid(srid);

            var gpkgGeometryColumnsManager = GetTableManager<GpkgGeometryColumnsManager>();
            gpkgGeometryColumnsManager.UpdateSrid(srid);

            var gpkgSpatialRefSysManager = GetTableManager<GpkgSpatialRefSysManager>();
            gpkgSpatialRefSysManager.AddOrUpdateFromUrl(spatialRefSysUrlFormat ?? GpkgSpatialRefSysManager.DefaultSpatialRefSysUrlFormat, srid);
            return true;
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
            OpenGeoPackageDatabase(hasTopologyLevel);
            GetTableManager<TnfNetworkManager>();
            GetTableManager<TnfNetworkReferenceManager>();
            GetTableManager<TnfDirectLocationReferenceManager>();

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

            GetTableManager<TnfTopologyLevelManager>();

            GetTableManager<TnfAreaManager>();
            GetTableManager<TnfTaskManager>();
            GetTableManager<TnfTaskEditableTypeManager>();

            GetTableManager<TnfMetadataManager>();
        }

        public void OpenGeoPackageDatabase(bool? hasTopologyLevel = null)
        {
            OpenConnection();

            using (IDbCommand cmd = m_connection.CreateCommand())
            {
                cmd.CommandText = String.Format("SELECT table_name, srs_id FROM {0} WHERE LOWER(table_name) = '{1}' OR LOWER(table_name) = '{2}'",
                    GpkgGeometryColumnsManager.GpkgGeometryColumnsTableName, TnfLinkSequenceManager.TnfLinkSequenceTableName.ToLower(), TnfNodeManager.TnfNodeTableName.ToLower());

                int? linkSrsId=null;
                int? nodeSrsId=null;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tableName = reader["table_name"].FromDbString();
                        if (tableName.ToLower().Equals(TnfLinkSequenceManager.TnfLinkSequenceTableName.ToLower()))
                        {
                            linkSrsId = reader["srs_id"].ToInt32();
                        }
                        else if (tableName.ToLower().Equals(TnfNodeManager.TnfNodeTableName.ToLower()))
                        {
                            nodeSrsId = reader["srs_id"].ToInt32();
                        }
                    }
                }

                if (!linkSrsId.HasValue || !nodeSrsId.HasValue)
                {
                    throw new OpenTnfException(Resources.OpenTnfSridMissing);
                }
                if (linkSrsId.Value != nodeSrsId.Value)
                {
                    throw new OpenTnfException(Resources.OpenTnfDatabaseHasMoreThanOneSrid, String.Format("tnf_link={0}, tnf_node={1}", linkSrsId, nodeSrsId));
                }

                m_srid = linkSrsId.Value;

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

        #region IDisposable Members

        public void Dispose()
        {
            foreach (DictionaryEntry entry in m_tableManagers)
            {
                var tableManager = entry.Value as TableManager;

                if (tableManager != null)
                {
                    tableManager.Dispose();
                }
            }

            EndTransactionAbort();
            CloseConnection();
        }

        #endregion
    }
}
