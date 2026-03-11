using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfValueDomain
    {
        int Oid { get; set; }
        int CatalogueOid { get; set; }
        string ValueDomainType { get; set; }
        string Name { get; set; }
        string ShortName { get; set; }
        string Description { get; set; }
        string DataType { get; set; }
        int? NrDec { get; set; }
        bool? IsUnion { get; set; }
        string Unit { get; set; }
        int? NrChar { get; set; }
    }

    [Serializable]
    public class TnfValueDomain : ITnfValueDomain
    {
        public int Oid { get; set; }
        public int CatalogueOid { get; set; }
        public string ValueDomainType { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public int? NrDec { get; set; }
        public bool? IsUnion { get; set; }
        public string Unit { get; set; }
        public int? NrChar { get; set; }

        public override bool Equals(object obj)
        {
            bool retVal = false;
            if (obj is TnfValueDomain)
            {
                var v = obj as TnfValueDomain;
                if (Oid == v.Oid &&
                    CatalogueOid == v.CatalogueOid &&
                    ValueDomainType == v.ValueDomainType &&
                    Name == v.Name &&
                    ShortName == v.ShortName &&
                    Description == v.Description &&
                    DataType == v.DataType &&
                    NrDec == v.NrDec &&
                    IsUnion == v.IsUnion &&
                    Unit == v.Unit &&
                    NrChar == v.NrChar)
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
                ValueDomainType,
                Name,
                ShortName,
                Description,
                DataType,
                NrDec,
                IsUnion,
                Unit,
                NrChar);
        }

        public override string ToString()
        {
            return String.Format("TnfValueDomain: Oid = {0}, CatalogueOid = {1}, ValueDomainType = {2}, Name = {3}, ShortName = {4}, " +
                                 "Description = {5}, DataType = {6}, NrDec = {7}, IsUnion = {8}, Unit = {9}, NrChar = {10}",
                Oid,
                CatalogueOid,
                ValueDomainType,
                Name,
                ShortName,
                Description,
                DataType,
                NrDec,
                IsUnion,
                Unit,
                NrChar);
        }


        public static List<string> ValidDataTypes
        {
            get
            {
                return new List<string>
                {
                    "Boolean",
                    "Date",
                    "DateTime",
                    "ShortDate",
                    "Time",
                    "Enum",
                    "Real",
                    "Integer",
                    "CharacterString",
                    "Point",
                    "LineString",
                    "Polygon",
                    "MultiPoint",
                    "MultiLineString",
                    "MultiPolygon",
                    "BLOB"
                };
            }
        }
    }

    public class TnfValueDomainManager : TableManager
    {
        private const string PrimaryKey = "oid, catalogue_oid";
        public const string TnfValueDomainTableName = "tnf_value_domain";

        public TnfValueDomainManager(GeoPackageDatabase db) : base(db, TnfValueDomainTableName, GetColumnInfos(), PrimaryKey)
        {
        }

        protected override string[] Constraints()
        {
            return new[]
            {
                String.Format("CONSTRAINT fk_tvd_co FOREIGN KEY (catalogue_oid) REFERENCES {0}(oid)",
                    TnfCatalogueManager.TnfCatalogueTableName)
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
                    Name = "value_domain_type",
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
                    Name = "shortname",
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
                    Name = "datatype",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String"),
                },
                new ColumnInfo
                {
                    Name = "nr_dec",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32"),
                },
                new ColumnInfo
                {
                    Name = "is_union",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Boolean"),
                    Requirement = ColumnRequirement.OptionalReadOnly,
                },
                new ColumnInfo
                {
                    Name = "unit",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String"),
                },
                new ColumnInfo
                {
                    Name = "nr_char",
                    SqlType = "INTEGER",
                    DataType = Type.GetType("System.Int32"),
                },
            };
        }

        public void Add(TnfValueDomain tnfValueDomain)
        {
            try
            {
                Add(new object[]
                    {
                    tnfValueDomain.Oid,
                    tnfValueDomain.CatalogueOid,
                    tnfValueDomain.ValueDomainType,
                    tnfValueDomain.Name,
                    tnfValueDomain.ShortName,
                    tnfValueDomain.Description,
                    tnfValueDomain.DataType,
                    tnfValueDomain.NrDec,
                    tnfValueDomain.IsUnion,
                    tnfValueDomain.Unit,
                    tnfValueDomain.NrChar
                    });
            }
            catch (Exception ex)
            {
                throw new OpenTnfException($"Error when adding value domain '{tnfValueDomain.Oid}' in catalogue '{tnfValueDomain.CatalogueOid}': {ex.Message}", ex);
            }
        }

        public TnfValueDomain Get(int oid, int catalogueOid)
        {
            return Get(ReadObject, new object[] { oid, catalogueOid });
        }

        public List<TnfValueDomain> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(TnfValueDomain tnfValueDomain)
        {
            return Update(new object[]
                {
                    tnfValueDomain.Oid,
                    tnfValueDomain.CatalogueOid,
                    tnfValueDomain.ValueDomainType,
                    tnfValueDomain.Name,
                    tnfValueDomain.ShortName,
                    tnfValueDomain.Description,
                    tnfValueDomain.DataType,
                    tnfValueDomain.NrDec,
                    tnfValueDomain.IsUnion,
                    tnfValueDomain.Unit,
                    tnfValueDomain.NrChar
                });
        }

        public int Delete(int oid, int catalogueOid)
        {
            return Delete(new object[] { oid, catalogueOid });
        }


        private static TnfValueDomain ReadObject(IDataRecord reader)
        {
            var tnfValueDomain = new TnfValueDomain();

            tnfValueDomain.Oid = reader["oid"].ToInt();
            tnfValueDomain.CatalogueOid = reader["catalogue_oid"].ToInt();
            tnfValueDomain.ValueDomainType = reader["value_domain_type"].FromDbString();
            tnfValueDomain.Name = reader["name"].FromDbString();
            tnfValueDomain.ShortName = reader["shortname"].FromDbString();
            tnfValueDomain.Description = reader["description"].FromDbString();
            tnfValueDomain.DataType = reader["datatype"].FromDbString();
            tnfValueDomain.NrDec = reader["nr_dec"].ToInt32();
            tnfValueDomain.IsUnion = reader.ReadIfExists("is_union").ToBoolean() ?? false;
            tnfValueDomain.Unit = reader["unit"].FromDbString();
            tnfValueDomain.NrChar = reader["nr_char"].ToInt32();

            return tnfValueDomain;
        }

        /// <summary>
        /// Use this function to receive an oid that is not currently in use for a value domain in the feature catalogue.
        /// </summary>
        /// <param name="catalogueOid"></param>
        /// <returns></returns>
        public int GetFreeOid(int catalogueOid)
        {
            string commandText = String.Format("SELECT COALESCE(MAX(CAST(oid as INTEGER)),0)+1 FROM {0} WHERE catalogue_oid = {1}", TnfValueDomainTableName, catalogueOid);
            int oid = Db.ExecuteScalar(commandText);
            return oid;
        }

        /// <summary>
        /// Get all value domains in a feature catalogue, including historical and future
        /// </summary>
        /// <param name="catalogueOid">OID for the feature catalogue</param>
        /// <returns></returns>
        public List<TnfValueDomain> GetAll(int catalogueOid)
        {
            return GetAll(catalogueOid, 0, 0, false);
        }

        /// <summary>
        /// Get all value domains in a feature catalogue that are valid during a certain time interval
        /// </summary>
        /// <param name="catalogueOid">OID for the feature catalogue</param>
        /// <param name="fromDate">Start date of the interval (inclusive)</param>
        /// <param name="toDate">End date of the interval (exclusive)</param>
        /// <returns></returns>
        public List<TnfValueDomain> GetAll(int catalogueOid, int fromDate, int toDate)
        {
            return GetAll(catalogueOid, fromDate, toDate, true);
        }

        private List<TnfValueDomain> GetAll(int catalogueOid, int fromDate, int toDate, bool bTimeInterval)
        {
            var valueDomains = new List<TnfValueDomain>();
            string commandText = String.Format("SELECT * FROM {0} WHERE catalogue_oid = {1}", TnfValueDomainTableName, catalogueOid);

            if (bTimeInterval)
            {
                commandText += " AND valid_from >= " + fromDate + " AND valid_to <= " + toDate;
            }

            using (IDataReader idataReader = Db.ExecuteReader(commandText))
            {
                while (idataReader != null && idataReader.Read())
                {
                    TnfValueDomain valueDomain = ReadObject(idataReader);
                    valueDomains.Add(valueDomain);
                }
            }

            return valueDomains;
        }

        public void ChangeCatalogueOid(int oldOid, int newOid)
        {
            using (var command = Db.Command)
            {
                command.CommandText = $"UPDATE {TnfValueDomainTableName} SET catalogue_oid = {newOid} WHERE catalogue_oid = '{oldOid}'";
                command.ExecuteNonQuery();
            }
        }

        public void ChangeOid(int catalogueOid, int oldOid, int newOid)
        {
            using (var command = Db.Command)
            {
                command.CommandText = $"UPDATE {TnfValueDomainTableName} SET oid = {newOid} " +
                                      $"WHERE catalogue_oid = '{catalogueOid}' AND oid = '{oldOid}'";
                command.ExecuteNonQuery();
            }
        }

        public void DeleteLeftOverValueDomainForCatalogue(int catalogueOid)
        {
            using (var deleteCommand = Db.Command)
            {
                deleteCommand.CommandText = $"DELETE FROM tnf_value_domain WHERE catalogue_oid = {catalogueOid}";
                deleteCommand.ExecuteNonQuery();
            }
        }
    }
}
