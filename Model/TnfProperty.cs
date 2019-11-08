using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OpenTNF.Library.Model
{
    public interface ITnfProperty
    {
        string Oid { get; }
        string PropertyObjectOid { get; }
        DateTime? ValidFrom  {get;}
        DateTime? ValidTo {get;}
        TnfAttributes AttributeValues { get; }
    }

    public class TnfProperty : ITnfProperty
    {

        public string Oid { get; set; }
        public string PropertyObjectOid { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public TnfAttributes AttributeValues { get; set; }
        
        public override bool Equals(object obj)
        {
            if (obj is TnfProperty)
            {
                var v = obj as TnfProperty;

                if (Oid == v.Oid &&
                    PropertyObjectOid == v.PropertyObjectOid &&
                    ValidFrom == v.ValidFrom &&
                    ValidTo == v.ValidTo &&
                    AttributeValues == v.AttributeValues)
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
                PropertyObjectOid,
                ValidFrom,
                ValidTo,
                AttributeValues);
        }

        public override string ToString()
        {
            return String.Format("TnfProperty: Oid = {0}, PropertyObjectOid = {1}, ValidFrom = {2}, ValidTo = {3}, AttributeValues = {4}",
                Oid,
                PropertyObjectOid,
                ValidFrom,
                ValidTo,
                AttributeValues);
        }
    }

    public class TnfPropertyManager : TableManager
    {
       
        public static string TnfPropertyTableName = "tnf_property";

        private static XmlSerializer m_serializer;
        private static XmlSerializer Serializer
        {
            get
            {
                if (m_serializer == null)
                {
                    m_serializer = new XmlSerializer(typeof(TnfAttributes));
                }
                return m_serializer;
            }
        }

        public TnfPropertyManager(GeoPackageDatabase db) : base(db, TnfPropertyTableName, GetColumnInfos())
        {
        }

        protected override string[] Constraints()
        {
            return new[]
                {
                    String.Format("CONSTRAINT fk_tpm_poo FOREIGN KEY (property_object_oid) REFERENCES {0}(oid)", TnfPropertyObjectManager.TnfPropertyObjectTableName)
                };
        }
        protected override string[] Indices()
        {
            return new[]
            {
                String.Format("CREATE INDEX IDX_tnf_property_property_object_oid ON {0}({1})", TnfPropertyTableName,
                    "property_object_oid")
            };
        }
        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "oid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "property_object_oid",
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
                    Name = "attribute_values",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                }
            };
        }

        public void Add(TnfProperty tnfProperty)
        {
            

            Add(new object[]
                {
                    tnfProperty.Oid,
                    tnfProperty.PropertyObjectOid,
                    tnfProperty.ValidFrom?.Date,
                    tnfProperty.ValidTo?.Date,
                    AttributeValuesToXmlString(tnfProperty.AttributeValues)
                });
        }

        public TnfProperty Get(string oid)
        {
            return Get(ReadProperty, new object[] { oid });
        }

        public List<TnfProperty> GetByPropertyObjectOid(string propertyOid)
        {
            var tnfPropertyList = new List<TnfProperty>();

            using (var command = Db.Command)
            {
                command.CommandText =
                    string.Format(
                        "SELECT * FROM {0} WHERE property_object_oid = '{1}' ORDER BY valid_from",
                        TnfPropertyTableName, propertyOid);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tnfPropertyList.Add(ReadProperty(reader));
                    }
                }
            }

            return tnfPropertyList;
        }

        public List<TnfProperty> Get(int maxResults)
        {
            return Get(ReadProperty, maxResults);
        }

        public List<TnfProperty> GetPage(int offset, int limit)
        {
            return GetPage(ReadProperty, offset, limit);
        }

        public int Update(TnfProperty tnfProperty)
        {
            return Update(new object[]
                {
                    tnfProperty.Oid,
                    tnfProperty.PropertyObjectOid,
                    tnfProperty.ValidFrom?.Date,
                    tnfProperty.ValidTo?.Date,
                    AttributeValuesToXmlString(tnfProperty.AttributeValues)
                });
        }

        public int Delete(string oid)
        {
            return Delete(new object[] { oid });
        }

        private static TnfProperty ReadProperty(IDataRecord reader)
        {
            var tnfProperty = new TnfProperty();

            tnfProperty.Oid = reader["oid"].FromDbString();
            tnfProperty.PropertyObjectOid = reader["property_object_oid"].FromDbString();
            tnfProperty.ValidFrom = reader["valid_from"].ToDateTime();
            tnfProperty.ValidTo = reader["valid_to"].ToDateTime();
            string attributeValuesXml = reader["attribute_values"].ToString();
            tnfProperty.AttributeValues = Serializer.Deserialize(new XmlTextReader(new StringReader(attributeValuesXml))) as TnfAttributes;
            return tnfProperty;
        }

        public static string AttributeValuesToXmlString(TnfAttributes tnfAttributes)
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            using (var writer = new StringWriter(xmlStringBuilder))
            {
                Serializer.Serialize(writer, tnfAttributes);
            }
            return xmlStringBuilder.ToString();
        }
    }
}
