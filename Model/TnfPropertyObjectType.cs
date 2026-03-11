using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfPropertyObjectType
    {
        int Oid { get; }
        int CatalogueOid { get; }
        string Name { get; }
        string Description { get; }
        int? NetworkReferenceType { get; }
        bool? HasSide { get; }
        bool? HasDirection { get; }
        bool? MustCover { get; }
        bool? CanOverlap { get; }
        bool? HasHistory { get; }
        bool? HasLaneCode { get; }
        int? NetworkReferenceMin { get; }
        int? NetworkReferenceMax { get; }
        DateTime? ValidFrom { get; }
        DateTime? ValidTo { get; }
        string ShortName { get; }
        string AttributeFormat { get; }
        bool? OrderedNetworkReferences { get; }
        string NetworkReferenceClass { get; }
    }

    [Serializable]
    public class TnfPropertyObjectType : ITnfPropertyObjectType
    {
        public int Oid { get; set; }
        public int CatalogueOid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? NetworkReferenceType { get; set; }
        public bool? HasSide { get; set; }
        public bool? HasDirection { get; set; }
        public bool? MustCover { get; set; }
        public bool? CanOverlap { get; set; }
        public bool? HasHistory { get; set; }
        public bool? HasLaneCode { get; set; }
        public int? NetworkReferenceMin { get; set; }
        public int? NetworkReferenceMax { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string ShortName { get; set; }
        public string AttributeFormat { get; set; }
        public bool? OrderedNetworkReferences { get; set; }
        public string NetworkReferenceClass { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TnfPropertyObjectType)
            {
                var p = obj as TnfPropertyObjectType;

                if (Oid == p.Oid &&
                    CatalogueOid == p.CatalogueOid &&
                    Name == p.Name &&
                    Description == p.Description &&
                    NetworkReferenceType == p.NetworkReferenceType &&
                    HasSide == p.HasSide &&
                    HasDirection == p.HasDirection &&
                    MustCover == p.MustCover &&
                    CanOverlap == p.CanOverlap &&
                    HasHistory == p.HasHistory &&
                    HasLaneCode == p.HasLaneCode &&
                    NetworkReferenceMin == p.NetworkReferenceMin &&
                    NetworkReferenceMax == p.NetworkReferenceMax &&
                    ValidFrom == p.ValidFrom &&
                    ValidTo == p.ValidTo &&
                    ShortName == p.ShortName &&
                    AttributeFormat == p.AttributeFormat &&
                    OrderedNetworkReferences == p.OrderedNetworkReferences &&
                    NetworkReferenceClass == p.NetworkReferenceClass)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(
                Oid,
                CatalogueOid,
                Name,
                Description,
                NetworkReferenceType,
                HasSide,
                HasDirection,
                MustCover,
                CanOverlap,
                HasHistory,
                HasLaneCode,
                NetworkReferenceMin,
                NetworkReferenceMax,
                ValidFrom,
                ValidTo,
                ShortName,
                AttributeFormat,
                OrderedNetworkReferences,
                NetworkReferenceClass);
        }

        public override string ToString()
        {
            return
                $"TnfPropertyObjectType: Oid = {Oid}, CatalogueOid = {CatalogueOid}, Name = {Name}, Description = {Description}, " +
                $"NetworkReferenceType = {NetworkReferenceType}, HasSide = {HasSide}, HasDirection = {HasDirection}, MustCover = {MustCover}, " +
                $"CanOverlap = {CanOverlap}, HasHistory = {HasHistory}, HasLaneCode = {HasLaneCode}, NetworkReferenceMin = {NetworkReferenceMin}, " +
                $"NetworkReferenceMax = {NetworkReferenceMax}, ValidFrom = {ValidFrom}, ValidTo = {ValidTo}, ShortName = {ShortName}, " +
                $"AttributeFormat = {AttributeFormat}, OrderedNetworkReferences = {OrderedNetworkReferences}, NetworkReferenceClass = {NetworkReferenceClass}";
        }
        public static int UnlimitedNetworkReferencesMax = -1;
    }

    public class TnfPropertyObjectTypeManager : TableManager
    {
        private const string PrimaryKey = "oid, catalogue_oid";
        public const string TnfPropertyObjectTypeTableName = "tnf_property_object_type";

        public TnfPropertyObjectTypeManager(GeoPackageDatabase db) : base(db, TnfPropertyObjectTypeTableName, GetColumnInfos(), PrimaryKey)
        {
        }

        protected override string[] Constraints()
        {
            return
                [
                    String.Format("CONSTRAINT fk_tpot_co FOREIGN KEY (catalogue_oid) REFERENCES {0}(oid)",TnfCatalogueManager.TnfCatalogueTableName),
                    "CONSTRAINT check_tpot_network_references CHECK (network_references_min >= 0 AND network_references_max >= -1 )"
                ];
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            var result = new List<ColumnInfo>
            {
                new ColumnInfo
                {
                    Name = "oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "catalogue_oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "name",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "description",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "network_reference_type",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "has_side",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Boolean")
                },
                new ColumnInfo
                {
                    Name = "has_direction",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Boolean")
                },
                new ColumnInfo
                {
                    Name = "must_cover",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Boolean")
                },
                new ColumnInfo
                {
                    Name = "can_overlap",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Boolean")
                },
                new ColumnInfo
                {
                    Name = "has_history",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Boolean")
                },
                new ColumnInfo
                {
                    Name = "has_lanecode",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Boolean")
                },
                new ColumnInfo
                {
                    Name = "network_references_min",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "network_references_max",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "valid_from",
                    SqlType = "DATE",
                    DataType = Type.GetType("System.DateTime")
                },
                new ColumnInfo
                {
                    Name = "valid_to",
                    SqlType = "DATE",
                    DataType = Type.GetType("System.DateTime")
                },
                new ColumnInfo
                {
                    Name = "shortname",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "attribute_format",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "ordered_network_references",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Boolean"),
                },
                new ColumnInfo
                {
                    Name = "network_reference_class",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String"),
                }
            };

            return result.ToArray();
        }

        public void Add(TnfPropertyObjectType tnfPropertyObjectType)
        {
            Add(GetColumnValues(tnfPropertyObjectType));
        }

        private object[] GetColumnValues(TnfPropertyObjectType tnfPropertyObjectType)
        {
            var result = new List<object>()
            {
                tnfPropertyObjectType.Oid,
                tnfPropertyObjectType.CatalogueOid,
                tnfPropertyObjectType.Name,
                tnfPropertyObjectType.Description,
                tnfPropertyObjectType.NetworkReferenceType,
                tnfPropertyObjectType.HasSide,
                tnfPropertyObjectType.HasDirection,
                tnfPropertyObjectType.MustCover,
                tnfPropertyObjectType.CanOverlap,
                tnfPropertyObjectType.HasHistory,
                tnfPropertyObjectType.HasLaneCode,
                tnfPropertyObjectType.NetworkReferenceMin,
                tnfPropertyObjectType.NetworkReferenceMax,
                tnfPropertyObjectType.ValidFrom.ToDateString(),
                tnfPropertyObjectType.ValidTo.ToDateString(),
                tnfPropertyObjectType.ShortName,
                tnfPropertyObjectType.AttributeFormat,
                tnfPropertyObjectType.OrderedNetworkReferences,
                tnfPropertyObjectType.NetworkReferenceClass
            };
            return result.ToArray();
        }

        public TnfPropertyObjectType Get(int oid, int catalogueOid)
        {
            return Get(ReadObject, new object[] { oid, catalogueOid });
        }

        public List<TnfPropertyObjectType> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(TnfPropertyObjectType tnfPropertyObjectType)
        {
            return Update(GetColumnValues(tnfPropertyObjectType));
        }

        public int Delete(int oid, int catalogueOid)
        {
            return Delete([oid, catalogueOid]);
        }

        private static TnfPropertyObjectType ReadObject(IDataRecord reader)
        {
            var tnfPropertyObjectType = new TnfPropertyObjectType();

            tnfPropertyObjectType.Oid = reader["oid"].ToInt();
            tnfPropertyObjectType.CatalogueOid = reader["catalogue_oid"].ToInt();
            tnfPropertyObjectType.Name = reader["name"].FromDbString();
            tnfPropertyObjectType.Description = reader["description"].FromDbString();
            tnfPropertyObjectType.NetworkReferenceType = reader["network_reference_type"].ToInt32();
            tnfPropertyObjectType.HasSide = reader["has_side"].ToBoolean();
            tnfPropertyObjectType.HasDirection = reader["has_direction"].ToBoolean();
            tnfPropertyObjectType.MustCover = reader["must_cover"].ToBoolean();
            tnfPropertyObjectType.CanOverlap = reader["can_overlap"].ToBoolean();
            tnfPropertyObjectType.HasHistory = reader["has_history"].ToBoolean();
            tnfPropertyObjectType.HasLaneCode = reader["has_lanecode"].ToBoolean();
            tnfPropertyObjectType.NetworkReferenceMin = reader["network_references_min"].ToInt32();
            tnfPropertyObjectType.NetworkReferenceMax = reader["network_references_max"].ToInt32();
            tnfPropertyObjectType.ValidFrom = reader["valid_from"].ToDateTime();
            tnfPropertyObjectType.ValidTo = reader["valid_to"].ToDateTime();
            tnfPropertyObjectType.ShortName = reader["shortname"].FromDbString();
            tnfPropertyObjectType.AttributeFormat = reader["attribute_format"].FromDbString();
            tnfPropertyObjectType.OrderedNetworkReferences = reader["ordered_network_references"].ToBoolean();
            tnfPropertyObjectType.NetworkReferenceClass = reader["network_reference_class"].FromDbString();

            return tnfPropertyObjectType;
        }


        /// <summary>
        /// Use this function to receive an oid that is not currently in use for a property object type in the feature catalogue.
        /// </summary>
        /// <param name="catalogueOid"></param>
        /// <returns></returns>
        public int GetFreeOid(int catalogueOid)
        {
            string commandText = String.Format("SELECT COALESCE(MAX(CAST(oid as INTEGER)),0)+1 FROM {0} WHERE catalogue_oid = {1}", TnfPropertyObjectTypeTableName, catalogueOid);
            int oid = Db.ExecuteScalar(commandText);
            return oid;
        }


        /// <summary>
        /// Get all property object types in a feature catalogue, including historical and future.
        /// </summary>
        /// <param name="catalogueOid">OID for the feature catalogue.</param>
        /// <returns></returns>
        public List<TnfPropertyObjectType> GetAll(int catalogueOid)
        {
            return GetAll(catalogueOid, 0, 0, false);
        }

        /// <summary>
        /// Get all property object types in a feature catalogue that are valid in a certain time interval.
        /// </summary>
        /// <param name="catalogueOid">OID for the feature catalogue</param>
        /// <param name="fromDate">Start date of the interval (inclusive)</param>
        /// <param name="toDate">End date of the interval (exclusive)</param>
        /// <returns></returns>
        public List<TnfPropertyObjectType> GetAll(int catalogueOid, int fromDate, int toDate)
        {
            return GetAll(catalogueOid, fromDate, toDate, true);
        }

        private List<TnfPropertyObjectType> GetAll(int catalogueOid, int fromDate, int toDate, bool bTimeInterval)
        {
            var propertyObjectTypes = new List<TnfPropertyObjectType>();

            string commandText = $"SELECT {string.Join(", ", ColumnNames)} " +
                                 $"FROM {TnfPropertyObjectTypeTableName} WHERE catalogue_oid=" +
                                 catalogueOid;

            if (bTimeInterval)
            {
                commandText += " AND valid_from >= " + fromDate + " AND valid_to <= " + toDate;
            }

            using (IDataReader reader = Db.ExecuteReader(commandText))
            {
                while (reader != null && reader.Read())
                {
                    TnfPropertyObjectType propertyObjectType = ReadObject(reader);
                    propertyObjectTypes.Add(propertyObjectType);
                }
            }

            return propertyObjectTypes;
        }

        public void ChangeCatalogueOid(int oldOid, int newOid)
        {
            using (var command = Db.Command)
            {
                command.CommandText =
                    $"UPDATE {TnfPropertyObjectTypeTableName} SET catalogue_oid = '{newOid}' WHERE catalogue_oid = '{oldOid}'";
                command.ExecuteNonQuery();
            }
        }

        public void ChangeOid(int catalogueOid, int oldOid, int newOid)
        {
            using (var command = Db.Command)
            {
                command.CommandText =
                    $"UPDATE {TnfPropertyObjectTypeTableName} SET oid = '{newOid}' " +
                    $"WHERE catalogue_oid = '{catalogueOid}' AND oid = '{oldOid}'";
                command.ExecuteNonQuery();
            }
        }

        public void DeleteLeftOverPropertyObjectTypesForCatalogue(int catalogueOid)
        {
            using (var deleteCommand = Db.Command)
            {
                deleteCommand.CommandText = $"DELETE FROM tnf_property_object WHERE catalogue_oid = {catalogueOid}";
                deleteCommand.ExecuteNonQuery();
            }
        }
    }
}
