using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfValidValue
    {
        int ValueDomainOid { get; }
        int CatalogueOid { get; }
        string Description { get; }
        int SeqNo { get; }
        DateTime? ValidFrom { get; }
        DateTime? ValidTo { get; }
        double? MinValueDouble { get; }
        double? MaxValueDouble { get; }
        int? MinValueInteger { get; }
        int? MaxValueInteger { get; }
        string ValueString { get; }
        DateTime? MinValueDateTime { get; }
        DateTime? MaxValueDateTime { get; }
        int? EnumCode { get; }
        int? Rank { get; }
    }

    public class TnfValidValue : ITnfValidValue
    {
        public int ValueDomainOid { get; set; }
        public int CatalogueOid { get; set; }
        public string Description { get; set; }
        public int SeqNo { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public double? MinValueDouble { get; set; }
        public double? MaxValueDouble { get; set; }
        public int? MinValueInteger { get; set; }
        public int? MaxValueInteger { get; set; }
        public string ValueString { get; set; }
        public DateTime? MinValueDateTime { get; set; }
        public DateTime? MaxValueDateTime { get; set; }
        public int? EnumCode { get; set; }
        public int? Rank { get; set; }

        public override bool Equals(object obj)
        {
            bool retVal = false;
            if (obj is TnfValidValue)
            {
                var v = obj as TnfValidValue;

                if (ValueDomainOid == v.ValueDomainOid &&
                    CatalogueOid == v.CatalogueOid &&
                    Description == v.Description &&
                    SeqNo == v.SeqNo &&
                    ValidFrom == v.ValidFrom &&
                    ValidTo == v.ValidTo &&
                    MinValueDouble == v.MinValueDouble && // ToDo: Compare with tolerance!
                    MaxValueDouble == v.MaxValueDouble && // ToDo: Compare with tolerance!
                    MinValueInteger == v.MinValueInteger &&
                    MaxValueInteger == v.MaxValueInteger &&
                    ValueString == v.ValueString &&
                    MinValueDateTime == v.MinValueDateTime &&
                    MaxValueDateTime == v.MaxValueDateTime &&
                    EnumCode == v.EnumCode &&
                    Rank == v.Rank)
                {
                    retVal = true;
                }
            }
            return retVal;
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(
                ValueDomainOid,
                CatalogueOid,
                Description,
                SeqNo,
                ValidFrom,
                ValidTo,
                MinValueDouble,
                MaxValueDouble,
                MinValueInteger,
                MaxValueInteger,
                ValueString,
                MinValueDateTime,
                MaxValueDateTime,
                EnumCode,
                Rank);
        }

        public override string ToString()
        {
            return String.Format("TnfValidValue: ValueDomainOid = {0}, CatalogueOid = {1}, Description = {2}, SeqNo = {3}, " +
                                 "ValidFrom = {4}, ValidTo = {5}, MinValueDouble = {6}, MaxValueDouble = {7}, MinValueInteger = {8}, " +
                                 "MaxValueInteger = {9}, ValueString = {10}, MinValueDateTime = {11}, MaxValueDateTime = {12}, EnumCode = {13}, Rank = {14}",
                ValueDomainOid,
                CatalogueOid,
                Description,
                SeqNo,
                ValidFrom,
                ValidTo,
                MinValueDouble,
                MaxValueDouble,
                MinValueInteger,
                MaxValueInteger,
                ValueString,
                MinValueDateTime,
                MaxValueDateTime,
                EnumCode,
                Rank);
        }
    }

    public class TnfValidValueManager : TableManager
    {
        private const string PrimaryKey = "value_domain_oid, catalogue_oid, seq_no";
        public static string TnfValidValueTableName = "tnf_valid_value";

        public TnfValidValueManager(GeoPackageDatabase db) : base(db, TnfValidValueTableName, GetColumnInfos(), PrimaryKey)
        {
        }

        protected override string[] Constraints()
        {
            return new[]
            {
                String.Format("CONSTRAINT fk_tvv_vdo_co FOREIGN KEY (value_domain_oid, catalogue_oid) REFERENCES {0}(oid, catalogue_oid)",TnfValueDomainManager.TnfValueDomainTableName),
                String.Format("CONSTRAINT fk_tvv_co FOREIGN KEY (catalogue_oid) REFERENCES {0}(oid)",
                    TnfCatalogueManager.TnfCatalogueTableName)
            };
        }

        protected override string[] Triggers()
        {
            return new[]
                {
                    @"CREATE TRIGGER ""tnf_valid_value_insert1""
BEFORE INSERT ON ""tnf_valid_value""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'insert on table ''tnf_property_object_property_type''
 violates constraint: only one conditional argument range may be set.')
WHERE (
  ((NEW.min_value_double IS NOT NULL OR NEW.max_value_double IS NOT NULL) AND
    (NEW.min_value_integer IS NOT NULL OR NEW.max_value_integer IS NOT NULL OR
     NEW.min_value_datetime IS NOT NULL OR NEW.max_value_datetime IS NOT NULL OR
     NEW.value_string IS NOT NULL OR
     NEW.enum_code IS NOT NULL
    )
  )
  OR
  ((NEW.min_value_integer IS NOT NULL OR NEW.max_value_integer IS NOT NULL) AND
    (NEW.min_value_double IS NOT NULL OR NEW.max_value_double IS NOT NULL OR
     NEW.min_value_datetime IS NOT NULL OR NEW.max_value_datetime IS NOT NULL OR
     NEW.value_string IS NOT NULL OR
     NEW.enum_code IS NOT NULL
    )
  )
  OR
  ((NEW.min_value_datetime IS NOT NULL OR NEW.max_value_datetime IS NOT NULL) AND
    (NEW.min_value_double IS NOT NULL OR NEW.max_value_double IS NOT NULL OR
     NEW.min_value_integer IS NOT NULL OR NEW.max_value_integer IS NOT NULL OR
     NEW.value_string IS NOT NULL OR
     NEW.enum_code IS NOT NULL
    )
  )
  OR
  (NEW.enum_code IS NOT NULL AND
    (NEW.min_value_double IS NOT NULL OR NEW.max_value_double IS NOT NULL OR
     NEW.min_value_integer IS NOT NULL OR NEW.max_value_integer IS NOT NULL OR
     NEW.min_value_datetime IS NOT NULL OR NEW.max_value_datetime IS NOT NULL OR
     NEW.value_string IS NOT NULL
    )
  )
  OR
  (NEW.value_string IS NOT NULL AND
    (NEW.min_value_double IS NOT NULL OR NEW.max_value_double IS NOT NULL OR
     NEW.min_value_integer IS NOT NULL OR NEW.max_value_integer IS NOT NULL OR
     NEW.min_value_datetime IS NOT NULL OR NEW.max_value_datetime IS NOT NULL OR
     NEW.enum_code IS NOT NULL
    )
  )
);
END",
                    @"CREATE TRIGGER ""tnf_valid_value_update1""
BEFORE UPDATE OF min_value_double, max_value_double, min_value_integer,
  max_value_integer, min_value_datetime, max_value_datetime, enum_code, value_string ON ""tnf_valid_value""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'update on table ''tnf_property_object_property_type''
 violates constraint: only one conditional argument range may be set.')
WHERE (
  ((NEW.min_value_double IS NOT NULL OR NEW.max_value_double IS NOT NULL) AND
    (NEW.min_value_integer IS NOT NULL OR NEW.max_value_integer IS NOT NULL OR
     NEW.min_value_datetime IS NOT NULL OR NEW.max_value_datetime IS NOT NULL OR
     NEW.value_string IS NOT NULL OR
     NEW.enum_code IS NOT NULL
    )
  )
  OR
  ((NEW.min_value_integer IS NOT NULL OR NEW.max_value_integer IS NOT NULL) AND
    (NEW.min_value_double IS NOT NULL OR NEW.max_value_double IS NOT NULL OR
     NEW.max_value_datetime IS NOT NULL OR NEW.max_value_datetime IS NOT NULL OR
     NEW.value_string IS NOT NULL OR
     NEW.enum_code IS NOT NULL
    )
  )
  OR
  ((NEW.min_value_datetime IS NOT NULL OR NEW.max_value_datetime IS NOT NULL) AND
    (NEW.min_value_double IS NOT NULL OR NEW.max_value_double IS NOT NULL OR
     NEW.min_value_integer IS NOT NULL OR NEW.max_value_integer IS NOT NULL OR
     NEW.value_string IS NOT NULL OR
     NEW.enum_code IS NOT NULL
    )
  )
  OR
  (NEW.enum_code IS NOT NULL AND
    (NEW.min_value_double IS NOT NULL OR NEW.max_value_double IS NOT NULL OR
     NEW.min_value_integer IS NOT NULL OR NEW.max_value_integer IS NOT NULL OR
     NEW.min_value_datetime IS NOT NULL OR NEW.max_value_datetime IS NOT NULL OR
     NEW.value_string IS NOT NULL
    )
  )
  OR
  (NEW.value_string IS NOT NULL AND
    (NEW.min_value_double IS NOT NULL OR NEW.max_value_double IS NOT NULL OR
     NEW.min_value_integer IS NOT NULL OR NEW.max_value_integer IS NOT NULL OR
     NEW.min_value_datetime IS NOT NULL OR NEW.max_value_datetime IS NOT NULL OR
     NEW.enum_code IS NOT NULL
    )
  )
);
END",
    @"CREATE TRIGGER ""tnf_valid_value_insert2""
BEFORE INSERT ON ""tnf_valid_value""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'insert on table ''tnf_property_object_property_type''
 violates constraint: both conditional arguments in a range must be set.')
WHERE (
  (NEW.min_value_double IS NOT NULL AND NEW.max_value_double IS NULL) OR
  (NEW.min_value_double IS NULL AND NEW.max_value_double IS NOT NULL) OR
  (NEW.min_value_integer IS NOT NULL AND NEW.max_value_integer IS NULL) OR
  (NEW.min_value_integer IS NULL AND NEW.max_value_integer IS NOT NULL) OR
  (NEW.min_value_datetime IS NOT NULL AND NEW.max_value_datetime IS NULL) OR
  (NEW.min_value_datetime IS NULL AND NEW.max_value_datetime IS NOT NULL)
);
END",
    @"CREATE TRIGGER ""tnf_valid_value_update2""
BEFORE UPDATE OF min_value_double, max_value_double, min_value_integer, max_value_integer, min_value_datetime, max_value_datetime ON ""tnf_valid_value""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'update on table ''tnf_property_object_property_type''
 violates constraint: both conditional arguments in a range must be set.')
WHERE (
  (NEW.min_value_double IS NOT NULL AND NEW.max_value_double IS NULL) OR
  (NEW.min_value_double IS NULL AND NEW.max_value_double IS NOT NULL) OR
  (NEW.min_value_integer IS NOT NULL AND NEW.max_value_integer IS NULL) OR
  (NEW.min_value_integer IS NULL AND NEW.max_value_integer IS NOT NULL) OR
  (NEW.min_value_datetime IS NOT NULL AND NEW.max_value_datetime IS NULL) OR
  (NEW.min_value_datetime IS NULL AND NEW.max_value_datetime IS NOT NULL)
);
END",
    @"CREATE TRIGGER ""tnf_valid_value_insert3""
BEFORE INSERT ON ""tnf_valid_value""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'insert on table ''tnf_property_object_property_type''
 violates constraint: enum_code must be only conditional argument.')
WHERE
  NEW.enum_code IS NOT NULL AND
  (NEW.min_value_double IS NOT NULL OR NEW.max_value_double IS NOT NULL OR
   NEW.min_value_integer IS NOT NULL OR NEW.max_value_integer IS NOT NULL OR
   NEW.min_value_datetime IS NOT NULL OR NEW.max_value_datetime IS NOT NULL);
END",
    @"CREATE TRIGGER ""tnf_valid_value_update3""
BEFORE UPDATE OF min_value_double, max_value_double, min_value_integer, max_value_integer, min_value_datetime, max_value_datetime, enum_code, value_string ON ""tnf_valid_value""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'update on table ''tnf_property_object_property_type''
violates constraint: enum_code must be only conditional argument.')
WHERE
  NEW.enum_code IS NOT NULL AND
  (NEW.min_value_double IS NOT NULL OR NEW.max_value_double IS NOT NULL OR
   NEW.min_value_integer IS NOT NULL OR NEW.max_value_integer IS NOT NULL OR
   NEW.min_value_datetime IS NOT NULL OR NEW.max_value_datetime IS NOT NULL);
END",
    @"CREATE TRIGGER ""tnf_valid_value_insert4""
BEFORE INSERT ON ""tnf_valid_value""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'insert on table ''tnf_property_object_property_type''
 violates constraint: value_string must be only conditional argument.')
WHERE
  NEW.value_string IS NOT NULL AND
  (NEW.min_value_double IS NOT NULL OR NEW.max_value_double IS NOT NULL OR
   NEW.min_value_integer IS NOT NULL OR NEW.max_value_integer IS NOT NULL OR
   NEW.min_value_datetime IS NOT NULL OR NEW.max_value_datetime IS NOT NULL OR
   NEW.enum_code IS NOT NULL);
END",
    @"CREATE TRIGGER ""tnf_valid_value_update4""
BEFORE UPDATE OF min_value_double, max_value_double, min_value_integer, max_value_integer, min_value_datetime, max_value_datetime, enum_code, value_string ON ""tnf_valid_value""
FOR EACH ROW BEGIN
SELECT RAISE(ABORT, 'update on table ''tnf_property_object_property_type''
violates constraint: value_string must be only conditional argument.')
WHERE
  NEW.value_string IS NOT NULL AND
  (NEW.min_value_double IS NOT NULL OR NEW.max_value_double IS NOT NULL OR
   NEW.min_value_integer IS NOT NULL OR NEW.max_value_integer IS NOT NULL OR
   NEW.min_value_datetime IS NOT NULL OR NEW.max_value_datetime IS NOT NULL OR
   NEW.enum_code IS NOT NULL);
END"
                };
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "value_domain_oid",
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
                    Name = "description",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "seq_no",
                    SqlType = "INT",
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
                    Name = "min_value_double",
                    SqlType = "DOUBLE",
                    DataType = Type.GetType("System.Double")
                },
                new ColumnInfo
                {
                    Name = "max_value_double",
                    SqlType = "DOUBLE",
                    DataType = Type.GetType("System.Double")
                },
                new ColumnInfo
                {
                    Name = "min_value_integer",
                    SqlType = "INT",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "max_value_integer",
                    SqlType = "INT",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "value_string",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "min_value_datetime",
                    SqlType = "DATETIME",
                    DataType = Type.GetType("System.DateTime")
                },
                new ColumnInfo
                {
                    Name = "max_value_datetime",
                    SqlType = "DATETIME",
                    DataType = Type.GetType("System.DateTime")
                },
                new ColumnInfo
                {
                    Name = "enum_code",
                    SqlType = "INT",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "rank",
                    SqlType = "INT",
                    DataType = Type.GetType("System.Int32")
                },
            };
        }


        public void Add(TnfValidValue tnfValidValue)
        {
            Add(new object[]
                {
                    tnfValidValue.ValueDomainOid,
                    tnfValidValue.CatalogueOid,
                    tnfValidValue.Description,
                    tnfValidValue.SeqNo,
                    tnfValidValue.ValidFrom?.Date,
                    tnfValidValue.ValidTo?.Date,
                    tnfValidValue.MinValueDouble,
                    tnfValidValue.MaxValueDouble,
                    tnfValidValue.MinValueInteger,
                    tnfValidValue.MaxValueInteger,
                    tnfValidValue.ValueString,
                    tnfValidValue.MinValueDateTime?.ToUniversalTime(),
                    tnfValidValue.MaxValueDateTime?.ToUniversalTime(),
                    tnfValidValue.EnumCode,
                    tnfValidValue.Rank
                });
        }

        public TnfValidValue Get(int valueDomainOid, int catalogueOid, int seqNo)
        {
            return Get(ReadObject, new object[] { valueDomainOid, catalogueOid, seqNo });
        }

        public List<TnfValidValue> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }



        /// <summary>
        /// Use this function to receive an SEQ_NO that is not currently in use for valid value in the associated value domain.
        /// </summary>
        /// <param name="catalogueOid"></param>
        /// <param name="valueDomainOid"></param>
        /// <returns></returns>
        public int GetTnfValidValueFreeSeqNo(int catalogueOid, int valueDomainOid)
        {
            string commandText = String.Format("SELECT COALESCE(MAX(SEQ_NO),0)+1 FROM {0} WHERE catalogue_oid = {1} and value_domain_oid = {2}", 
                TnfValidValueTableName, catalogueOid, valueDomainOid);
            int oid = Db.ExecuteScalar(commandText);

            return oid;
        }

        public int Update(TnfValidValue tnfValidValue)
        {
            return Update(new object[]
                {
                    tnfValidValue.ValueDomainOid,
                    tnfValidValue.CatalogueOid,
                    tnfValidValue.Description,
                    tnfValidValue.SeqNo,
                    tnfValidValue.ValidFrom?.Date,
                    tnfValidValue.ValidTo?.Date,
                    tnfValidValue.MinValueDouble,
                    tnfValidValue.MaxValueDouble,
                    tnfValidValue.MinValueInteger,
                    tnfValidValue.MaxValueInteger,
                    tnfValidValue.ValueString,
                    tnfValidValue.MinValueDateTime?.ToUniversalTime(),
                    tnfValidValue.MaxValueDateTime?.ToUniversalTime(),
                    tnfValidValue.EnumCode,
                    tnfValidValue.Rank
                });
        }

        public int Delete(int valueDomainOid, int catalogueOid, int seqNo)
        {
            return Delete(new object[] { valueDomainOid, catalogueOid, seqNo });
        }

        private static TnfValidValue ReadObject(IDataRecord reader)
        {
            var validValue = new TnfValidValue();

            validValue.ValueDomainOid = reader["value_domain_oid"].ToInt();
            validValue.CatalogueOid = reader["catalogue_oid"].ToInt();
            validValue.Description = reader["description"].FromDbString();
            validValue.SeqNo = reader["seq_no"].ToInt();
            validValue.ValidFrom = reader["valid_from"].ToDateTime();
            validValue.ValidTo = reader["valid_to"].ToDateTime();
            validValue.MinValueDouble = reader["min_value_double"].ToDouble();
            validValue.MaxValueDouble = reader["max_value_double"].ToDouble();
            validValue.MinValueInteger = reader["min_value_integer"].ToInt32();
            validValue.MaxValueInteger = reader["max_value_integer"].ToInt32();
            validValue.ValueString = reader["value_string"].FromDbString();
            validValue.MinValueDateTime = reader["min_value_datetime"].ToDateTime();
            validValue.MaxValueDateTime = reader["max_value_datetime"].ToDateTime();
            validValue.EnumCode = reader["enum_code"].ToInt32();
            validValue.Rank = reader["rank"].ToInt32();

            return validValue;
        }

        /// <summary>
        /// Get all valid values for a certain value domain
        /// </summary>
        /// <param name="catalogueOid">OID for the data catalogue</param>
        /// <param name="valueDomainOid">OID for the value domain</param>
        /// <returns></returns>
        public List<TnfValidValue> GetAll(int catalogueOid, int valueDomainOid)
        {
            var validValues = new List<TnfValidValue>();
            string commandText = String.Format("SELECT * FROM {0} WHERE catalogue_oid = '{1}' AND value_domain_oid = '{2}'",
                TnfValidValueTableName, catalogueOid, valueDomainOid);

            using (IDataReader idataReader = Db.ExecuteReader(commandText))
            {
                while (idataReader != null && idataReader.Read())
                {
                    TnfValidValue validValue = ReadObject(idataReader);
                    validValues.Add(validValue);
                }
            }

            return validValues;
        }

        /// <summary>
        /// Get all valid values for a certain data catalogue, including historical and future
        /// </summary>
        /// <param name="catalogueOid">OID for the data catalogue</param>
        /// <returns></returns>
        public List<TnfValidValue> GetAll(int catalogueOid)
        {
            return GetAll(catalogueOid, 0, 0, false);
        }

        /// <summary>
        /// Get all valid values for a certain data catalogue that are valid during a certain time interval
        /// </summary>
        /// <param name="catalogueOid">OID for the data catalogue</param>
        /// <param name="fromDate">Start date of the interval (inclusive)</param>
        /// <param name="toDate">End date of the interval (exclusive)</param>
        /// <returns></returns>
        public List<TnfValidValue> GetAll(int catalogueOid, int fromDate, int toDate)
        {
            return GetAll(catalogueOid, fromDate, toDate, true);
        }

        private List<TnfValidValue> GetAll(int catalogueOid, int fromDate, int toDate, bool bTimeInterval)
        {
            var validValues = new List<TnfValidValue>();
            string commandText = String.Format("SELECT * FROM {0} WHERE catalogue_oid = {1}", TnfValidValueTableName, catalogueOid);

            if (bTimeInterval)
            {
                commandText += " AND from_date >= " + fromDate + " AND to_date <= " + toDate;
            }

            using (IDataReader idataReader = Db.ExecuteReader(commandText))
            {
                while (idataReader != null && idataReader.Read())
                {
                    TnfValidValue validValue = ReadObject(idataReader);
                    validValues.Add(validValue);
                }
            }

            return validValues;
        }


        /// <summary>
        /// Delete all valid values for a certain value domain
        /// </summary>
        /// <param name="catalogueOid">OID for the data catalogue</param>
        /// <param name="valueDomainOid">OID for the value domain</param>
        public int DeleteAll(int catalogueOid, int valueDomainOid)
        {
            return Db.ExecuteNonQuery(
                string.Format(
                    "DELETE FROM {0} WHERE catalogue_oid = '{1}' AND value_domain_oid = '{2}'",
                    TnfValidValueTableName, catalogueOid, valueDomainOid));
        }
    }
}