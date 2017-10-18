using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public interface ITnfToDoListDetails
    {
        string MessageOid { get; }
        IdentityCode Type { get; }
        string Value { get;  }
    }

    public class TnfToDoListDetails : ITnfToDoListDetails
    {
        public string MessageOid { get; set; }
        public IdentityCode Type { get; set; }
        public string Value { get; set; }
    }


    public class TnfToDoListDetailsManager : TableManager
    {
        public static string TnfToDoListDetailsTableName = "tnf_todo_list_details";

        public TnfToDoListDetailsManager(GeoPackageDatabase db) : base(db, TnfToDoListDetailsTableName, GetColumnInfos(),null)
        {
        }

        protected override string[] Constraints()
        {
            return new[]
            {
                String.Format("CONSTRAINT fk_ttdld_tdlmo FOREIGN KEY (message_oid) REFERENCES {0} (oid)",
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
            string commandText = String.Format("SELECT * FROM {0} WHERE message_oid = {1}", TnfToDoListDetailsTableName, messageOid);

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
                case "NETWORKCATALOGUE_NAME_ELEMENT_OID":
                    return IdentityCode.NETWORK_ELEMENT_OID;
                case "CATALOGUE_NAME":
                    return IdentityCode.CATALOGUE_NAME;
                case "VALID_PERIOD":
                    return IdentityCode.VALID_PERIOD;
                case "START_MEASURE":
                    return IdentityCode.START_MEASURE;
                case "END_MEASURE":
                    return IdentityCode.END_MEASURE;
                case "MIN_LENGTH":
                    return IdentityCode.MIN_LENGTH;
                case "NETWORK_REFERENCE_TYPE":
                    return IdentityCode.NETWORK_REFERENCE_TYPE;
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
        VALID_PERIOD,                      /* Valid persiod */
        START_MEASURE,                     /* start measure */
        END_MEASURE,                       /* end measure */
        MIN_LENGTH,                        /* Minimum length */
        TOTAL_LENGTH,                      /* Total length */
        NETWORK_REFERENCE_TYPE,            /* What network reference type */
        VALUE_DOMAIN_OID                   /* ValueDomainId */
    }
}
