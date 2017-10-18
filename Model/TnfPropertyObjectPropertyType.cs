using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace OpenTNF.Library.Model
{
    public interface ITnfPropertyObjectPropertyType
    {
        int Oid { get; }
        int CatalogueOid { get; }
        int PropertyObjectTypeOid { get; }
        int MultiplicityMin { get; }
        int MultiplicityMax { get; }
        bool Mandatory { get; }
        string Name { get; }
        string Description { get; }
        string ShortName { get; }
        DateTime? ValidFrom { get; }
        DateTime? ValidTo { get; }
        int? AssocPropertyObjectTypeOid { get; }
        string AssocType { get; }
        int? ValueDomainOid { get; }
    }

    public sealed class TnfPropertyObjectPropertyType : ITnfPropertyObjectPropertyType
    {
        public int Oid { get; set; }
        public int CatalogueOid { get; set; }
        public int PropertyObjectTypeOid { get; set; }
        public int MultiplicityMin { get; set; }
        public int MultiplicityMax { get; set; }
        public bool Mandatory { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortName { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public int? AssocPropertyObjectTypeOid { get; set; }
        public string AssocType { get; set; }
        public int? ValueDomainOid { get; set; }

        public override bool Equals(object obj)
        {
            bool retVal = false;

            if (obj is TnfPropertyObjectPropertyType)
            {
                var p = obj as TnfPropertyObjectPropertyType;

                if (Oid == p.Oid &&
                    CatalogueOid == p.CatalogueOid &&
                    PropertyObjectTypeOid == p.PropertyObjectTypeOid &&
                    MultiplicityMin == p.MultiplicityMin &&
                    MultiplicityMax == p.MultiplicityMax &&
                    Mandatory == p.Mandatory &&
                    Name == p.Name &&
                    Description == p.Description &&
                    ShortName == p.ShortName &&
                    ValidFrom == p.ValidFrom &&
                    ValidTo == p.ValidTo &&
                    AssocPropertyObjectTypeOid == p.AssocPropertyObjectTypeOid &&
                    AssocType == p.AssocType &&
                    ValueDomainOid == p.ValueDomainOid)
                {
                    retVal = true;
                }
            }
            return retVal;
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(
                Oid,
                CatalogueOid,
                PropertyObjectTypeOid,
                MultiplicityMin,
                MultiplicityMax,
                Mandatory,
                Name,
                Description,
                ShortName,
                ValidFrom,
                ValidTo,
                AssocPropertyObjectTypeOid,
                AssocType,
                ValueDomainOid);
        }

        public override string ToString()
        {
            return String.Format("TnfPropertyObjectPropertyType: Oid = {0}, CatalogueOid = {1}, PropertyObjectTypeOid = {2}, " +
                                 "MultiplicityMin = {3}, MultiplicityMax = {4}, Mandatory = {5}, Name = {6}, Description = {7}, " +
                                 "ShortName = {8}, ValidFrom = {9}, ValidTo = {10}, AssocPropertyObjectTypeOid = {11}, AssocType = {12}, " +
                                 "ValueDomainOid = {13}",
                Oid,
                CatalogueOid,
                PropertyObjectTypeOid,
                MultiplicityMin,
                MultiplicityMax,
                Mandatory,
                Name,
                Description,
                ShortName,
                ValidFrom,
                ValidTo,
                AssocPropertyObjectTypeOid,
                AssocType,
                ValueDomainOid);
        }
        public static int UnlimitedMultiplicityMax = -1;
    }
    
    public class TnfPropertyObjectPropertyTypeManager : TableManager
    {
        private const string PrimaryKey = "oid, catalogue_oid, property_object_type_oid";
        public static string TnfPropertyObjectPropertyTypeTableName = "tnf_property_object_property_type";

        public TnfPropertyObjectPropertyTypeManager(GeoPackageDatabase db) : base(db, TnfPropertyObjectPropertyTypeTableName, GetColumnInfos(),PrimaryKey)
        {
        }

        protected override string[] Constraints()
        {
            return new[]
                {
                    String.Format("CONSTRAINT fk_tpopt_co FOREIGN KEY (catalogue_oid) REFERENCES {0}(oid)",TnfCatalogueManager.TnfCatalogueTableName),
                    String.Format("CONSTRAINT fk_tpopt_co_poto FOREIGN KEY (property_object_type_oid, catalogue_oid) REFERENCES {0}(oid, catalogue_oid)",TnfPropertyObjectTypeManager.TnfPropertyObjectTypeTableName),
                    String.Format("CONSTRAINT fk_tpopt_co_apoto FOREIGN KEY (assoc_property_object_type_oid, catalogue_oid) REFERENCES {0}(oid, catalogue_oid)",TnfPropertyObjectTypeManager.TnfPropertyObjectTypeTableName),
                    String.Format("CONSTRAINT fk_tpopt_co_vdo FOREIGN KEY (value_domain_oid, catalogue_oid) REFERENCES {0}(oid, catalogue_oid)",TnfValueDomainManager.TnfValueDomainTableName),
                    "CONSTRAINT check_tpopt_multiplicty CHECK (multiplicity_min >= 0 AND multiplicity_max >= -1 )"
                };
        }

        protected override string[] Triggers()
        {
            return new[]
                {
                    @"CREATE TRIGGER ""tnf_property_object_property_type_insert""
BEFORE INSERT ON ""tnf_property_object_property_type""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'insert on table ''tnf_property_object_property_type''
 violates constraint: property_object_type_oid and
 assoc_property_object_type_oid cannot both be null.')
WHERE (NEW.property_object_type_oid IS NULL
  AND NEW.assoc_property_object_type_oid IS NULL);
END;",
                    @"CREATE TRIGGER ""tnf_property_object_property_type_update""
BEFORE UPDATE OF ""property_object_type_oid"", ""assoc_property_object_type_oid""
ON ""tnf_property_object_property_type""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'update on table ''tnf_property_object_property_type''
 violates constraint: property_object_type_oid and
 assoc_property_object_type_oid cannot both be null.')
WHERE (NEW.property_object_type_oid IS NULL
  AND NEW.assoc_property_object_type_oid IS NULL);
END;"
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
                    Name = "property_object_type_oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "multiplicity_min",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "multiplicity_max",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "mandatory",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Boolean")
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
                    Name = "shortname",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
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
                    Name = "assoc_property_object_type_oid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "assoc_type",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "value_domain_oid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
            };
        }

        /// <summary>
        /// Use this function to receive an oid that is not currently in use for a property object property type in the data catalogue.
        /// </summary>
        /// <param name="catalogueOid"></param>
        /// <returns></returns>
        public int GetFreeOid(int catalogueOid)
        {
            string commandText = String.Format("SELECT COALESCE(MAX(CAST(oid as INTEGER)),0)+1 FROM {0} WHERE catalogue_oid = {1}", TnfPropertyObjectPropertyTypeTableName,
                                 catalogueOid);
            int oid = Db.ExecuteScalar(commandText);

            return oid;
        }

        public void Add(TnfPropertyObjectPropertyType tnfPropertyObjectPropertyType)
        {
            Add(new object[]
                {
                    tnfPropertyObjectPropertyType.Oid,
                    tnfPropertyObjectPropertyType.CatalogueOid,
                    tnfPropertyObjectPropertyType.PropertyObjectTypeOid,
                    tnfPropertyObjectPropertyType.MultiplicityMin,
                    tnfPropertyObjectPropertyType.MultiplicityMax,
                    tnfPropertyObjectPropertyType.Mandatory,
                    tnfPropertyObjectPropertyType.Name,
                    tnfPropertyObjectPropertyType.Description,
                    tnfPropertyObjectPropertyType.ShortName,
                    tnfPropertyObjectPropertyType.ValidFrom?.Date,
                    tnfPropertyObjectPropertyType.ValidTo?.Date,
                    tnfPropertyObjectPropertyType.AssocPropertyObjectTypeOid,
                    tnfPropertyObjectPropertyType.AssocType,
                    tnfPropertyObjectPropertyType.ValueDomainOid
                });
        }

        public TnfPropertyObjectPropertyType Get(int oid, int catalogueOid, int propertyObjectTypeOid)
        {
            return Get(ReadObject, new object[] { oid, catalogueOid, propertyObjectTypeOid });
        }

        public List<TnfPropertyObjectPropertyType> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(TnfPropertyObjectPropertyType tnfPropertyObjectPropertyType)
        {
            return Update(new object[]
                {
                    tnfPropertyObjectPropertyType.Oid,
                    tnfPropertyObjectPropertyType.CatalogueOid,
                    tnfPropertyObjectPropertyType.PropertyObjectTypeOid,
                    tnfPropertyObjectPropertyType.MultiplicityMin,
                    tnfPropertyObjectPropertyType.MultiplicityMax,
                    tnfPropertyObjectPropertyType.Mandatory,
                    tnfPropertyObjectPropertyType.Name,
                    tnfPropertyObjectPropertyType.Description,
                    tnfPropertyObjectPropertyType.ShortName,
                    tnfPropertyObjectPropertyType.ValidFrom?.Date,
                    tnfPropertyObjectPropertyType.ValidTo?.Date,
                    tnfPropertyObjectPropertyType.AssocPropertyObjectTypeOid,
                    tnfPropertyObjectPropertyType.AssocType,
                    tnfPropertyObjectPropertyType.ValueDomainOid
                });
        }

        public int Delete(int oid, int catalogueOid, int propertyTypeOid)
        {
            return Delete(new object[] { oid, catalogueOid, propertyTypeOid });
        }

        /// <summary>
        /// Get all property object property types that refer to a certain value domain, including historical and future
        /// </summary>
        /// <param name="catalogueOid">OID for the data catalogue</param>
        /// <param name="valueDomainOid">OID for the value domain</param>
        /// <returns></returns>
        public List<TnfPropertyObjectPropertyType> GetForValueDomain(int catalogueOid, int valueDomainOid)
        {
            return GetForValueDomain(catalogueOid, valueDomainOid, 0, 0, false);
        }

        /// <summary>
        /// Get all property object property types that refer to a certain value domain and are valid in a certain time interval.
        /// </summary>
        /// <param name="catalogueOid">OID for the data catalogue</param>
        /// <param name="valueDomainOid">OID for the value domain</param>
        /// <param name="fromDate">Start date of the interval (inclusive)</param>
        /// <param name="toDate">End date of the interval (exclusive)</param>
        /// <returns></returns>
        public List<TnfPropertyObjectPropertyType> GetForValueDomain(int catalogueOid, int valueDomainOid,
                                                                     int fromDate, int toDate)
        {
            return GetForValueDomain(catalogueOid, valueDomainOid, fromDate, toDate, true);
        }

        private List<TnfPropertyObjectPropertyType> GetForValueDomain(int catalogueOid, int valueDomainOid,
                                                                      int fromDate, int toDate, bool bTimeInterval)
        {
            var propertyObjectPropertyTypes = new List<TnfPropertyObjectPropertyType>();

            string commandText =
                "SELECT catalogue_oid, oid, property_object_type_oid, assoc_property_object_type_oid, value_domain_oid, name, shortname, description, multiplicity_min, " +
                String.Format("multiplicity_max, mandatory, valid_from, valid_to, assoc_type FROM {0} WHERE catalogue_oid = ",TnfPropertyObjectPropertyTypeTableName) +
                catalogueOid + " AND value_domain_oid = " + valueDomainOid;

            if (bTimeInterval)
            {
                commandText += " AND valid_from >= " + fromDate + " AND valid_to <= " + toDate;
            }

            using (IDataReader reader = Db.ExecuteReader(commandText))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        propertyObjectPropertyTypes.Add(ReadObject(reader));
                    }
                }
            }

            return propertyObjectPropertyTypes;
        }


        /// <summary>
        /// Get all property object property types, including historical and future
        /// </summary>
        /// <param name="catalogueOid">OID for the data catalogue</param>
        /// <param name="propertyObjectTypeOid">OID for the property object type</param>
        /// <returns></returns>
        public List<TnfPropertyObjectPropertyType> GetAll(int catalogueOid, int propertyObjectTypeOid)
        {
            return GetAll(catalogueOid, propertyObjectTypeOid, 0, 0, false);
        }

        /// <summary>
        /// Get all property object property types that are valid in a certain time interval
        /// </summary>
        /// <param name="catalogueOid">OID for the data catalogue</param>
        /// <param name="propertyObjectTypeOid">OID for the property object type</param>
        /// <param name="fromDate">Start date of the interval (inclusive)</param>
        /// <param name="toDate">End date of the interval (exclusive)</param>
        /// <returns></returns>
        public List<TnfPropertyObjectPropertyType> GetAll(int catalogueOid, int propertyObjectTypeOid,
                                                          int fromDate, int toDate)
        {
            return GetAll(catalogueOid, propertyObjectTypeOid, fromDate, toDate, true);
        }

        private List<TnfPropertyObjectPropertyType> GetAll(int catalogueOid, int propertyObjectTypeOid,
                                                           int fromDate, int toDate, bool bTimeInterval)
        {
            var propertyObjectPropertyTypes = new List<TnfPropertyObjectPropertyType>();

            string commandText =
                "SELECT catalogue_oid, oid, property_object_type_oid, assoc_property_object_type_oid, value_domain_oid, name, shortname, description, multiplicity_min, " +
                String.Format("multiplicity_max, mandatory, valid_from, valid_to, assoc_type FROM {0} WHERE catalogue_oid = ",TnfPropertyObjectPropertyTypeTableName) +
                catalogueOid + " AND property_object_type_oid = " + propertyObjectTypeOid;

            if (bTimeInterval)
            {
                commandText += " AND valid_from >= " + fromDate + " AND valid_to <= " + toDate;
            }

            using (IDataReader reader = Db.ExecuteReader(commandText))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        propertyObjectPropertyTypes.Add(ReadObject(reader));
                    }
                }
            }

            return propertyObjectPropertyTypes;
        }

        private static TnfPropertyObjectPropertyType ReadObject(IDataRecord reader)
        {
            var propertyObjectPropertType = new TnfPropertyObjectPropertyType();

            propertyObjectPropertType.CatalogueOid = reader["catalogue_oid"].ToInt();
            propertyObjectPropertType.Oid = reader["oid"].ToInt();
            propertyObjectPropertType.PropertyObjectTypeOid = reader["property_object_type_oid"].ToInt();
            propertyObjectPropertType.AssocPropertyObjectTypeOid = reader["assoc_property_object_type_oid"].ToInt32();
            propertyObjectPropertType.ValueDomainOid = reader["value_domain_oid"].ToInt32();
            propertyObjectPropertType.Name = reader["name"].FromDbString();
            propertyObjectPropertType.AssocType = reader["assoc_type"].FromDbString();
            propertyObjectPropertType.ShortName = reader["shortname"].FromDbString();
            propertyObjectPropertType.Description = reader["description"].FromDbString();
            propertyObjectPropertType.MultiplicityMin = (Int32) reader["multiplicity_min"].ToInt32();
            propertyObjectPropertType.MultiplicityMax = (Int32) reader["multiplicity_max"].ToInt32();
            propertyObjectPropertType.Mandatory = (bool) reader["mandatory"].ToBoolean();
            propertyObjectPropertType.ValidFrom = reader["valid_from"].ToDateTime();
            propertyObjectPropertType.ValidTo = reader["valid_to"].ToDateTime();

            return propertyObjectPropertType;
        }
    }
}
