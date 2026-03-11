using System.Data;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OpenTNF.Library.Model
{
    public interface ITnfProperty
    {
        string Oid { get; }
        string PropertyObjectOid { get; }
        DateTime? ValidFrom { get; }
        DateTime? ValidTo { get; }
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
            return string.Format("TnfProperty: Oid = {0}, PropertyObjectOid = {1}, ValidFrom = {2}, ValidTo = {3}, AttributeValues = {4}",
                Oid,
                PropertyObjectOid,
                ValidFrom,
                ValidTo,
                AttributeValues);
        }
    }

    public class TnfPropertyManager : TableManager
    {

        public const string TnfPropertyTableName = "tnf_property";

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
                    string.Format("CONSTRAINT fk_tpm_poo FOREIGN KEY (property_object_oid) REFERENCES {0}(oid)", TnfPropertyObjectManager.TnfPropertyObjectTableName)
                };
        }

        protected override IndexInfo[] Indices()
        {
            return new[]
            {
                new IndexInfo
                {
                    Name = "IDX_tnf_property_property_object_oid",
                    Sql = $"CREATE INDEX IDX_tnf_property_property_object_oid ON {TnfPropertyTableName}(property_object_oid)"
                }
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
                    SqlType = "DATE",
                    DataType = Type.GetType("System.DateTime"),
                },
                new ColumnInfo
                {
                    Name = "valid_to",
                    SqlType = "DATE",
                    DataType = Type.GetType("System.DateTime"),
                },
                new ColumnInfo
                {
                    Name = "attribute_values",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String"),
                }
            };
        }

        public void Add(TnfProperty tnfProperty)
        {
            Add(new object[]
                {
                    tnfProperty.Oid,
                    tnfProperty.PropertyObjectOid,
                    tnfProperty.ValidFrom.ToDateString(),
                    tnfProperty.ValidTo.ToDateString(),
                    AttributeValuesToXmlString(tnfProperty.AttributeValues)
                });
        }

        public TnfProperty Get(string oid)
        {
            return Get(ReadProperty, new object[] { oid });
        }

        public List<TnfProperty> GetByPropertyObjectTypeOid(string catalogueOid, string propertyObjectTypeOid)
        {
            var tnfPropertyList = new List<TnfProperty>();

            using (var command = Db.Command)
            {
                command.CommandText =
                    string.Format(
                        "SELECT * FROM {0} WHERE exists " +
                        "(" +
                        " select 1 from tnf_property_object where catalogue_oid = '{1}' and property_object_type_oid = '{2}' and tnf_property.property_object_oid = tnf_property_object.oid )"
                        , TnfPropertyTableName, catalogueOid, propertyObjectTypeOid);

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

        public List<TnfProperty> GetPage(int offset, int limit)
        {
            return GetPage(ReadProperty, offset, limit);
        }

        public List<TnfProperty> Get(int maxResults)
        {
            return Get(ReadProperty, maxResults);
        }

        public int Update(TnfProperty tnfProperty)
        {
            return Update(new object[]
                {
                    tnfProperty.Oid,
                    tnfProperty.PropertyObjectOid,
                    tnfProperty.ValidFrom.ToDateString(),
                    tnfProperty.ValidTo.ToDateString(),
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
            var sb = new StringBuilder();

            using var xmlWriter = XmlWriter.Create(sb, new XmlWriterSettings()
            {
                CheckCharacters = false,
                Indent = true,
                IndentChars = " "
            });

            Serializer.Serialize(xmlWriter, tnfAttributes);

            return sb.ToString();
        }
    }
}
