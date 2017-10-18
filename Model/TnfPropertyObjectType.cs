using System;
using System.Collections.Generic;
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
        int? BaseCatalogueOid { get; }
        int? BasePropertyObjectTypeOid { get; }
        bool? IsDerived { get; }
    }

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
        public int? NetworkReferenceMin  { get; set; }
        public int? NetworkReferenceMax  { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string ShortName { get; set; }
        public string AttributeFormat { get; set; }
        public bool? OrderedNetworkReferences { get; set; }
        public string NetworkReferenceClass { get; set; }
        public int? BaseCatalogueOid { get; set; }
        public int? BasePropertyObjectTypeOid { get; set; }
        public bool? IsDerived { get; set; }

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
                    NetworkReferenceClass == p.NetworkReferenceClass &&
                    BaseCatalogueOid == p.BaseCatalogueOid &&
                    BasePropertyObjectTypeOid == p.BasePropertyObjectTypeOid &&
                    IsDerived == p.IsDerived)
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
                NetworkReferenceClass,
                BaseCatalogueOid,
                BasePropertyObjectTypeOid,
                IsDerived);
        }

        public override string ToString()
        {
            return
                $"TnfPropertyObjectType: Oid = {Oid}, CatalogueOid = {CatalogueOid}, Name = {Name}, Description = {Description}, " +
                $"BaseCatalogueOid = {BaseCatalogueOid}, BasePropertyObjectTypeOid={BasePropertyObjectTypeOid} " +
                $"NetworkReferenceType = {NetworkReferenceType}, HasSide = {HasSide}, HasDirection = {HasDirection}, MustCover = {MustCover}, " +
                $"CanOverlap = {CanOverlap}, HasHistory = {HasHistory}, HasLaneCode = {HasLaneCode}, NetworkReferenceMin = {NetworkReferenceMin}, " +
                $"NetworkReferenceMax = {NetworkReferenceMax}, ValidFrom = {ValidFrom}, ValidTo = {ValidTo}, ShortName = {ShortName}, " +
                $"AttributeFormat = {AttributeFormat}, OrderedNetworkReferences = {OrderedNetworkReferences}, NetworkReferenceClass = {NetworkReferenceClass}, " +
                $"IsDerived = {IsDerived}";
        }
        public static int UnlimitedNetworkReferencesMax = -1;
    }

    public class TnfPropertyObjectTypeManager : TableManager
    {
        private const string PrimaryKey = "oid, catalogue_oid";
        public static string TnfPropertyObjectTypeTableName = "tnf_property_object_type";

        public TnfPropertyObjectTypeManager(GeoPackageDatabase db) : base(db, TnfPropertyObjectTypeTableName, GetColumnInfos(),PrimaryKey)
        {
        }

        protected override string[] Constraints()
        {
            return new[]
                {
                    String.Format("CONSTRAINT fk_tpot_co FOREIGN KEY (catalogue_oid) REFERENCES {0}(oid)",TnfCatalogueManager.TnfCatalogueTableName),
                    "CONSTRAINT check_tpot_network_references CHECK (network_references_min >= 0 AND network_references_max >= -1 )"

                    // ToDo: Add constraints for base types
                };
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
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
                    SqlType = "DATETIME",
                    DataType = Type.GetType("System.DateTime")
                },
                new ColumnInfo
                {
                    Name = "valid_to",
                    SqlType = "DATETIME",
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
                    DataType = Type.GetType("System.Boolean")
                },
                new ColumnInfo
                {
                    Name = "network_reference_class",
                    SqlType = "STRING",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "base_catalogue_oid",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "base_property_object_type_oid",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "is_derived",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Boolean"),
                    HandleMissing = true
                }
            };
        }

        public void Add(TnfPropertyObjectType tnfPropertyObjectType)
        {
            Add(new object[]
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
                    tnfPropertyObjectType.ValidFrom?.Date,
                    tnfPropertyObjectType.ValidTo?.Date,
                    tnfPropertyObjectType.ShortName,
                    tnfPropertyObjectType.AttributeFormat,
                    tnfPropertyObjectType.OrderedNetworkReferences,
                    tnfPropertyObjectType.NetworkReferenceClass,
                    tnfPropertyObjectType.BaseCatalogueOid,
                    tnfPropertyObjectType.BasePropertyObjectTypeOid,
                    tnfPropertyObjectType.IsDerived
                });
        }

        public TnfPropertyObjectType Get(int oid, int catalogueOid)
        {
            return Get(ReadObject, new object[] {oid, catalogueOid});
        }

        public List<TnfPropertyObjectType> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(TnfPropertyObjectType tnfPropertyObjectType)
        {
            return Update(new object[]
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
                    tnfPropertyObjectType.ValidFrom?.Date,
                    tnfPropertyObjectType.ValidTo?.Date,
                    tnfPropertyObjectType.ShortName,
                    tnfPropertyObjectType.AttributeFormat,
                    tnfPropertyObjectType.OrderedNetworkReferences,
                    tnfPropertyObjectType.NetworkReferenceClass,
                    tnfPropertyObjectType.BaseCatalogueOid,
                    tnfPropertyObjectType.BasePropertyObjectTypeOid,
                    tnfPropertyObjectType.IsDerived
                });
        }

        public int Delete(int oid, int catalogueOid)
        {
            return Delete(new object[] { oid, catalogueOid });
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
            tnfPropertyObjectType.BaseCatalogueOid = reader["base_catalogue_oid"].ToInt32();
            tnfPropertyObjectType.BasePropertyObjectTypeOid = reader["base_property_object_type_oid"].ToInt32();
            tnfPropertyObjectType.IsDerived = reader.ReadIfExists("is_derived").ToBoolean() ?? false;

            return tnfPropertyObjectType;
        }


        /// <summary>
        /// Use this function to receive an oid that is not currently in use for a property object type in the data catalogue.
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
        /// Get all property object types in a data catalogue, including historical and future.
        /// </summary>
        /// <param name="catalogueOid">OID for the data catalogue.</param>
        /// <returns></returns>
        public List<TnfPropertyObjectType> GetAll(int catalogueOid)
        {
            return GetAll(catalogueOid, 0, 0, false);
        }

        /// <summary>
        /// Get all property object types in a data catalogue that are valid in a certain time interval.
        /// </summary>
        /// <param name="catalogueOid">OID for the data catalogue</param>
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
    }
}
