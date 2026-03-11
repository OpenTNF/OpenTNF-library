using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfToDoListDetails
    {
        string MessageOid { get; }
        IdentityCode Type { get; }
        string Value { get; }
    }

    public class TnfToDoListDetails : ITnfToDoListDetails
    {
        public string MessageOid { get; set; }
        public IdentityCode Type { get; set; }
        public string Value { get; set; }
    }


    public class TnfToDoListDetailsManager : TableManager
    {
        public const string TnfToDoListDetailsTableName = "tnf_todo_list_details";

        public TnfToDoListDetailsManager(GeoPackageDatabase db) : base(db, TnfToDoListDetailsTableName, GetColumnInfos(), null)
        {
        }

        protected override string[] Constraints()
        {
            return new[]
            {
                string.Format("CONSTRAINT fk_ttdld_tdlmo FOREIGN KEY (message_oid) REFERENCES {0} (oid)",
                    TnfToDoListMessageManager.TnfToDoListMessageTableName)
            };
        }

        public void Add(TnfToDoListDetails tnfToDoListMessage)
        {
            Add(new object[]
                {
                    tnfToDoListMessage.MessageOid,
                    tnfToDoListMessage.Type,
                    tnfToDoListMessage.Value
                });
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "message_oid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "type",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "value",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                }
            };
        }

        public List<TnfToDoListDetails> GetAll(string messageOid)
        {
            var tnfToDoListDetails = new List<TnfToDoListDetails>();
            string commandText = string.Format("SELECT * FROM {0} WHERE message_oid = {1}", TnfToDoListDetailsTableName, messageOid);

            using (IDataReader idataReader = Db.ExecuteReader(commandText))
            {
                while (idataReader != null && idataReader.Read())
                {
                    TnfToDoListDetails tnfToDoListDetail = ReadObject(idataReader);
                    tnfToDoListDetails.Add(tnfToDoListDetail);
                }
            }
            return tnfToDoListDetails;
        }

        public List<TnfToDoListDetails> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(TnfToDoListDetails tnfToDoListMessage)
        {
            return Update(new object[]
                {
                    tnfToDoListMessage.MessageOid,
                    tnfToDoListMessage.Type,
                    tnfToDoListMessage.Value
                });
        }

        public int DeleteAll(string messageOid)
        {
            return Db.ExecuteNonQuery(
                string.Format(
                    "DELETE FROM {0} WHERE CATALOGUE_NAME = '{1}'",
                    TnfToDoListDetailsTableName, messageOid));
        }

        private static TnfToDoListDetails ReadObject(IDataRecord reader)
        {
            var tnfToDoListDetails = new TnfToDoListDetails();

            tnfToDoListDetails.MessageOid = reader["message_oid"].FromDbString();
            tnfToDoListDetails.Type = GetIdentityCode(reader["type"].FromDbString());
            tnfToDoListDetails.Value = reader["value"].FromDbString();

            return tnfToDoListDetails;
        }

        private static IdentityCode GetIdentityCode(string value)
        {
            switch (value)
            {
                case "MESSAGE_SOURCE":
                    return IdentityCode.MESSAGE_SOURCE;
                case "PROPERTY_OBJECT_OID":
                    return IdentityCode.PROPERTY_OBJECT_OID;
                case "NETWORK_ELEMENT_OID":
                    return IdentityCode.NETWORK_ELEMENT_OID;
                case "PROPERTY_OBJECT_TYPE_OID":
                    return IdentityCode.PROPERTY_OBJECT_TYPE_OID;
                case "PROPERTY_OID":
                    return IdentityCode.PROPERTY_OID;
                case "PROPERTY_OBJECT_PROPERTY_TYPE_OID":
                    return IdentityCode.PROPERTY_OBJECT_PROPERTY_TYPE_OID;
                case "PORTNUMBER":
                    return IdentityCode.PORTNUMBER;
                case "REF_NETWORK_ELEMENT_OID":
                    return IdentityCode.REF_NETWORK_ELEMENT_OID;
                case "REF_PORTNUMBER":
                    return IdentityCode.REF_PORTNUMBER;
                case "DETAILS":
                    return IdentityCode.DETAILS;
                case "POSITION":
                    return IdentityCode.POSITION;
                case "CATALOGUE_NAME":
                    return IdentityCode.CATALOGUE_NAME;
                case "VALID_PERIOD":
                    return IdentityCode.VALID_PERIOD;
                case "START_MEASURE":
                    return IdentityCode.START_MEASURE;
                case "END_MEASURE":
                    return IdentityCode.END_MEASURE;
                case "MEASURE":
                    return IdentityCode.MEASURE;
                case "MIN_LENGTH":
                    return IdentityCode.MIN_LENGTH;
                case "TOTAL_LENGTH":
                    return IdentityCode.TOTAL_LENGTH;
                case "NETWORK_REFERENCE_TYPE":
                    return IdentityCode.NETWORK_REFERENCE_TYPE;
                case "VALUE_DOMAIN_OID":
                    return IdentityCode.VALUE_DOMAIN_OID;
                case "MESSAGE_ID":
                    return IdentityCode.MESSAGE_ID;
                case "SUMMARY":
                    return IdentityCode.SUMMARY;
                case "OID_OBJECT_TYPE":
                    return IdentityCode.OID_OBJECT_TYPE;
                case "VALID_FROM":
                    return IdentityCode.VALID_FROM;
                case "VALID_TO":
                    return IdentityCode.VALID_TO;
                default:
                    return IdentityCode.Undefined;
            }
        }
    }
    public enum IdentityCode
    {
        Undefined,
        MESSAGE_SOURCE,                    /* What validation gave this message */
        PROPERTY_OBJECT_OID,               /* Feature oid. */
        NETWORK_ELEMENT_OID,               /* NetElement oid.*/
        PROPERTY_OBJECT_TYPE_OID,          /* FeatureTypeId.*/
        PROPERTY_OID,                      /* TimeversionId.*/
        PROPERTY_OBJECT_PROPERTY_TYPE_OID, /* AttributeTypeId.*/
        PORTNUMBER,                        /* Port nummer.*/
        REF_NETWORK_ELEMENT_OID,           /* Referert NetElementId.*/
        REF_PORTNUMBER,                    /* Referert port nummer.*/
        DETAILS,                           /* A more detailed description */
        POSITION,                          /* For location of error wkt point. */
        CATALOGUE_NAME,                    /* catalogue name */
        VALID_PERIOD,                      /* Valid period */
        START_MEASURE,                     /* start measure */
        END_MEASURE,                       /* end measure */
        MEASURE,                           /* measure */
        MIN_LENGTH,                        /* Minimum length */
        TOTAL_LENGTH,                      /* Total length */
        NETWORK_REFERENCE_TYPE,            /* What network reference type */
        VALUE_DOMAIN_OID,                  /* ValueDomainId */
        MESSAGE_ID,                        /* Unique ID for this type of message */
        SUMMARY,                           /* Short message describing the validation */
        OID_OBJECT_TYPE,                   /* OID Object type: LINK_SEQUENCE, NODE or PROPERTY_OBJECT */
        VALID_FROM,                        /* Valid from */
        VALID_TO,                          /* Valid from */
    }

    /// <summary>
    /// Static values for IdentityCode keys.
    /// </summary>
    public class IdentityCodeValues
    {
        /// <summary>
        /// Allowed values for <see cref="IdentityCode.OID_OBJECT_TYPE"/>.
        /// </summary>
        public static class OID_OBJECT_TYPE
        {
            public const string LINK_SEQUENCE = "LINK_SEQUENCE";
            public const string NODE = "NODE";
            public const string PROPERTY_OBJECT = "PROPERTY_OBJECT";
        }
    }
}
