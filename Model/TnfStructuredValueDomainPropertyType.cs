using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OpenTNF.Library.Model
{
    public interface ITnfStructuredValueDomainPropertyType
    {
        int Oid { get; }
        int CatalogueOid { get; }
        int StructuredValueDomainOid { get; }
        int ValueDomainOid { get; }
        int? MultiplicityMin { get; }
        int? MultiplicityMax { get; }
        bool Mandatory { get; }
        string Name { get; }
        string Description { get; }
        string ShortName { get; }
        DateTime? ValidFrom { get; }
        DateTime? ValidTo { get; }
    }

    public class TnfStructuredValueDomainPropertyType : ITnfStructuredValueDomainPropertyType
    {
        public int Oid { get; set; }
        public int CatalogueOid { get; set; }
        public int StructuredValueDomainOid { get; set; }
        public int ValueDomainOid { get; set; }
        public int? MultiplicityMin { get; set; }
        public int? MultiplicityMax { get; set; }
        public bool Mandatory { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortName { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TnfStructuredValueDomainPropertyType)
            {
                var v = obj as TnfStructuredValueDomainPropertyType;

                if (Oid == v.Oid &&
                    CatalogueOid == v.CatalogueOid &&
                    StructuredValueDomainOid == v.StructuredValueDomainOid &&
                    ValueDomainOid == v.ValueDomainOid &&
                    MultiplicityMin == v.MultiplicityMin &&
                    MultiplicityMax == v.MultiplicityMax &&
                    Mandatory == v.Mandatory &&
                    Name == v.Name &&
                    Description == v.Description &&
                    ShortName == v.ShortName &&
                    ValidFrom == v.ValidFrom &&
                    ValidTo == v.ValidTo)
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
                StructuredValueDomainOid,
                ValueDomainOid,
                MultiplicityMin,
                MultiplicityMax,
                Mandatory,
                Name,
                Description,
                ShortName,
                ValidFrom,
                ValidTo);
        }

        public override string ToString()
        {
            return String.Format("TnfStructuredValueDomainPropertyType: Oid = {0}, CatalogueOid = {1}, StructuredValueDomainOid = {2}, " +
                                 "ValueDomainOid = {3}, MultiplicityMin = {4}, MultiplicityMax = {5}, Mandatory = {6}, Name = {7}, " +
                                 "Description = {8}, ShortName = {9}, ValidFrom = {10}, ValidTo = {11}",
                Oid,
                CatalogueOid,
                StructuredValueDomainOid,
                ValueDomainOid,
                MultiplicityMin,
                MultiplicityMax,
                Mandatory,
                Name,
                Description,
                ShortName,
                ValidFrom,
                ValidTo);
        }

        public static int UnlimitedMultiplicityMax = -1;
    }
    
    public class TnfStructuredValueDomainPropertyTypeManager : TableManager
    {
        private const string PrimaryKey = "oid, catalogue_oid, structured_value_domain_oid";
        public static string TnfStructuredValueDomainPropertyTableName = "tnf_structured_value_domain_property_type";

        public TnfStructuredValueDomainPropertyTypeManager(GeoPackageDatabase db) : base(db, TnfStructuredValueDomainPropertyTableName, GetColumnInfos(),PrimaryKey)
        {
        }

        protected override string[] Constraints()
        {
            return new[]
                {
                    String.Format("CONSTRAINT fk_tsvdpt_co FOREIGN KEY (catalogue_oid) REFERENCES {0}(oid)",TnfCatalogueManager.TnfCatalogueTableName),
                    String.Format("CONSTRAINT fk_tsvdpt_co_svdo FOREIGN KEY (structured_value_domain_oid, catalogue_oid) REFERENCES {0}(oid, catalogue_oid)",TnfValueDomainManager.TnfValueDomainTableName),
                    String.Format("CONSTRAINT fk_tsvdpt_co_svdo FOREIGN KEY (value_domain_oid, catalogue_oid) REFERENCES {0}(oid, catalogue_oid)",TnfValueDomainManager.TnfValueDomainTableName),
                    "CONSTRAINT check_tsvdpt_multiplicity CHECK (multiplicity_min >= 0 AND multiplicity_max >= -1 )"
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
                    Name = "structured_value_domain_oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "value_domain_oid",
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
            };
        }

        public void Add(TnfStructuredValueDomainPropertyType tnfStructuredValueDomainPropertyType)
        {
            Add(new object[]
                {
                    tnfStructuredValueDomainPropertyType.Oid,
                    tnfStructuredValueDomainPropertyType.CatalogueOid,
                    tnfStructuredValueDomainPropertyType.StructuredValueDomainOid,
                    tnfStructuredValueDomainPropertyType.ValueDomainOid,
                    tnfStructuredValueDomainPropertyType.MultiplicityMin,
                    tnfStructuredValueDomainPropertyType.MultiplicityMax,
                    tnfStructuredValueDomainPropertyType.Mandatory,
                    tnfStructuredValueDomainPropertyType.Name,
                    tnfStructuredValueDomainPropertyType.Description,
                    tnfStructuredValueDomainPropertyType.ShortName,
                    tnfStructuredValueDomainPropertyType.ValidFrom?.Date,
                    tnfStructuredValueDomainPropertyType.ValidTo?.Date
                });
        }

        public TnfStructuredValueDomainPropertyType Get(int oid, int catalogueOid, int structuredValueDomainOid)
        {
            return Get(ReadObject, new object[] { oid, catalogueOid, structuredValueDomainOid });
        }

        public List<TnfStructuredValueDomainPropertyType> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(TnfStructuredValueDomainPropertyType tnfStructuredValueDomainPropertyType)
        {
            return Update(new object[]
                {
                    tnfStructuredValueDomainPropertyType.Oid,
                    tnfStructuredValueDomainPropertyType.CatalogueOid,
                    tnfStructuredValueDomainPropertyType.StructuredValueDomainOid,
                    tnfStructuredValueDomainPropertyType.ValueDomainOid,
                    tnfStructuredValueDomainPropertyType.MultiplicityMin,
                    tnfStructuredValueDomainPropertyType.MultiplicityMax,
                    tnfStructuredValueDomainPropertyType.Mandatory,
                    tnfStructuredValueDomainPropertyType.Name,
                    tnfStructuredValueDomainPropertyType.Description,
                    tnfStructuredValueDomainPropertyType.ShortName,
                    tnfStructuredValueDomainPropertyType.ValidFrom?.Date,
                    tnfStructuredValueDomainPropertyType.ValidTo?.Date
                });
        }

        public int Delete(int oid, int catalogueOid, int structuredValueDomainOid)
        {
            return Delete(new object[] { oid, catalogueOid, structuredValueDomainOid });
        }

        /// <summary>
        /// Use this function to receive an oid that is not currently in use for a structured value domain property type in the data catalogue.
        /// </summary>
        /// <param name="catalogueOid"></param>
        /// <returns></returns>
        public int GetFreeOid(int catalogueOid)
        {
            string commandText =
                String.Format("SELECT COALESCE(MAX(CAST(oid as INTEGER)),0)+1 FROM {0} WHERE catalogue_oid = {1}", TnfStructuredValueDomainPropertyTableName, catalogueOid);
            int oid = Db.ExecuteScalar(commandText);
            return oid;
        }

        /// <summary>
        /// Get all structured value domain property that reference a the specified value domain, including historical and future
        /// </summary>
        /// <param name="catalogueOid"></param>
        /// <param name="associatedValueDomainOid">The value domain the property references (not the structured value domain the property belongs to)</param>
        /// <returns></returns>
        public List<TnfStructuredValueDomainPropertyType> GetAllAssociatedProperties(int catalogueOid, int associatedValueDomainOid)
        {
            return GetAll(catalogueOid, null, 0, 0, false, associatedValueDomainOid);
        }

        /// <summary>
        /// Get all structured value domain property types that belong to a certain value domain, including historical and future
        /// </summary>
        /// <param name="catalogueOid">OID for the data catalogue</param>
        /// <param name="structuredValueDomainOid">OID for the structured value domain the property belongs to</param>
        /// <returns></returns>
        public List<TnfStructuredValueDomainPropertyType> GetAll(int catalogueOid, int structuredValueDomainOid)
        {
            return GetAll(catalogueOid, structuredValueDomainOid, 0, 0, false);
        }

        /// <summary>
        /// Get all structured value domain property types that are belong to a certain value domain and valid during a certain time interval
        /// </summary>
        /// <param name="catalogueOid">OID for the data catalogue</param>
        /// <param name="structuredValueDomainOid">OID for the structured value domain the property belongs to</param>
        /// <param name="fromDate">Start date of the interval (inclusive)</param>
        /// <param name="toDate">End date of the interval (exclusive)</param>
        /// <returns></returns>
        public List<TnfStructuredValueDomainPropertyType> GetAll(int catalogueOid, int structuredValueDomainOid, int fromDate, int toDate)
        {
            return GetAll(catalogueOid, structuredValueDomainOid, fromDate, toDate, true);
        }

        private List<TnfStructuredValueDomainPropertyType> GetAll(int catalogueOid, int? structuredValueDomainOid,
                                                                  int fromDate, int toDate, bool bTimeInterval, int? associatedValueDomainOid = null)
        {
            var structuredValueDomainPropertyTypes = new List<TnfStructuredValueDomainPropertyType>();

            string commandText =
                "SELECT catalogue_oid, oid, structured_value_domain_oid, name, shortname, description, multiplicity_min, " +
                String.Format("multiplicity_max, mandatory, valid_from, valid_to, value_domain_oid FROM {0} WHERE catalogue_oid = ",
                    TnfStructuredValueDomainPropertyTableName) + catalogueOid;

            if (structuredValueDomainOid != null)
            {
                commandText += " AND structured_value_domain_oid = " + structuredValueDomainOid;
            }

            if (associatedValueDomainOid != null)
            {
                commandText += " AND value_domain_oid = " + associatedValueDomainOid;
            }

            if (bTimeInterval)
            {
                commandText += " AND valid_from >= " + fromDate + " AND valid_to <= " + toDate;
            }

            using (IDataReader idataReader = Db.ExecuteReader(commandText))
            {
                while (idataReader != null && idataReader.Read())
                {
                    TnfStructuredValueDomainPropertyType structuredValueDomainPropertyType =
                        ReadObject(idataReader);
                    structuredValueDomainPropertyTypes.Add(structuredValueDomainPropertyType);
                }
            }

            return structuredValueDomainPropertyTypes;
        }

        /// <summary>
        /// Get all structured value domain property types, including historical and future
        /// </summary>
        /// <param name="catalogueOid">OID for the data catalogue</param>
        /// <returns></returns>
        public List<TnfStructuredValueDomainPropertyType> GetAll(int catalogueOid)
        {
            return GetAll(catalogueOid, 0, 0, false);
        }

        /// <summary>
        /// Get all structured value domain property types that are valid during a certain time interval
        /// </summary>
        /// <param name="catalogueOid">OID for the data catalogue</param>
        /// <param name="fromDate">Start date of the interval (inclusive)</param>
        /// <param name="toDate">End date of the interval (exclusive)</param>
        /// <returns></returns>
        public List<TnfStructuredValueDomainPropertyType> GetAll(int catalogueOid, int fromDate, int toDate)
        {
            return GetAll(catalogueOid, fromDate, toDate, true);
        }

        private List<TnfStructuredValueDomainPropertyType> GetAll(int catalogueOid, int fromDate, int toDate,
                                                                  bool bTimeInterval)
        {
            var tnfStructuredValueDomainPropertyTypes = new List<TnfStructuredValueDomainPropertyType>();

            string commandText =
                "SELECT catalogue_oid, oid, structured_value_domain_oid, name, shortname, description, multiplicity_min, " +
                String.Format("multiplicity_max, mandatory, valid_from, valid_to, value_domain_oid FROM {0} ", TnfStructuredValueDomainPropertyTableName) +
                "WHERE catalogue_oid = " + catalogueOid;

            if (bTimeInterval)
            {
                commandText += " AND from_date >= " + fromDate + " AND to_date <= " + toDate;
            }

            using (IDataReader idataReader = Db.ExecuteReader(commandText))
            {
                while (idataReader != null && idataReader.Read())
                {
                    TnfStructuredValueDomainPropertyType structuredValueDomainPropertyType =
                        ReadObject(idataReader);

                    tnfStructuredValueDomainPropertyTypes.Add(structuredValueDomainPropertyType);
                }
            }

            return tnfStructuredValueDomainPropertyTypes;
        }

        /// <summary>
        /// Get a single structured value domain property type
        /// </summary>
        /// <param name="oid">OID for the structured value domain property type</param>
        /// <param name="catalogueOid">OID for the data catalogue</param>
        /// <returns></returns>
        public TnfStructuredValueDomainPropertyType Get(int oid, int catalogueOid)
        {
            TnfStructuredValueDomainPropertyType structuredValueDomainPropertyType = null;

            string commandText =
                "SELECT catalogue_oid, oid, structured_value_domain_oid, name, shortname, description, multiplicity_min, " +
                String.Format("multiplicity_max, mandatory, valid_from, valid_to, value_domain_oid FROM {0} ",TnfStructuredValueDomainPropertyTableName) +
                "WHERE catalogue_oid = " + catalogueOid + " AND oid = '" + oid + "'";

            using (IDataReader idataReader = Db.ExecuteReader(commandText))
            {
                if (idataReader != null && idataReader.Read())
                {
                    structuredValueDomainPropertyType = ReadObject(idataReader);
                }
            }

            return structuredValueDomainPropertyType;
        }

        private static TnfStructuredValueDomainPropertyType ReadObject(IDataRecord reader)
        {
            var tnfStructuredValueDomainPropertyType = new TnfStructuredValueDomainPropertyType();

            tnfStructuredValueDomainPropertyType.CatalogueOid = reader["catalogue_oid"].ToInt();
            tnfStructuredValueDomainPropertyType.Oid = reader["oid"].ToInt();
            tnfStructuredValueDomainPropertyType.StructuredValueDomainOid = reader["structured_value_domain_oid"].ToInt();
            tnfStructuredValueDomainPropertyType.Name = reader["name"].FromDbString();
            tnfStructuredValueDomainPropertyType.ShortName = reader["shortname"].FromDbString();
            tnfStructuredValueDomainPropertyType.Description = reader["description"].FromDbString();
            tnfStructuredValueDomainPropertyType.MultiplicityMin = reader["multiplicity_min"].ToInt32();
            tnfStructuredValueDomainPropertyType.MultiplicityMax = reader["multiplicity_max"].ToInt32();
            tnfStructuredValueDomainPropertyType.Mandatory = (bool) reader["mandatory"].ToBoolean();
            tnfStructuredValueDomainPropertyType.ValidFrom = reader["valid_from"].ToDateTime();
            tnfStructuredValueDomainPropertyType.ValidTo = reader["valid_to"].ToDateTime();
            tnfStructuredValueDomainPropertyType.ValueDomainOid = reader["value_domain_oid"].ToInt();

            return tnfStructuredValueDomainPropertyType;
        }
    }
}
