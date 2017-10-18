using System;
using System.Xml.Serialization;

namespace OpenTNF.Library.Model
{
    [Serializable]
    [XmlType(Namespace = "http://www.opentnf.org")]
    [XmlRoot("Attributes", Namespace = "http://www.opentnf.org", IsNullable = false)]
    public class TnfAttributes
    {
        private TnfAttribute[] m_items;

        private int m_catalogueOid;

        private int m_propertyObjectTypeOid;

        [XmlElement("SimpleAttribute", typeof(SimpleTnfAttribute))]
        [XmlElement("StructuredAttribute", typeof(StructuredTnfAttribute))]
        public TnfAttribute[] Items
        {
            get { return m_items; }
            set { m_items = value; }
        }

        [XmlAttribute("catalogueOID")]
        public int CatalogueOid
        {
            get { return m_catalogueOid; }
            set { m_catalogueOid = value; }
        }

        [XmlAttribute("propertyObjectTypeOID")]
        public int PropertyObjectTypeOid
        {
            get { return m_propertyObjectTypeOid; }
            set { m_propertyObjectTypeOid = value; }
        }
    }

    [XmlInclude(typeof(StructuredTnfAttribute))]
    [XmlInclude(typeof(SimpleTnfAttribute))]
    [Serializable]
    [XmlType(Namespace = "http://www.opentnf.org")]
    public abstract class TnfAttribute
    {
        private int m_attributeType;

        [XmlAttribute("attributeType")]
        public int AttributeType
        {
            get { return m_attributeType; }
            set { m_attributeType = value; }
        }
    }

    [Serializable]
    [XmlType(Namespace = "http://www.opentnf.org")]
    [XmlRoot("SimpleAttribute", Namespace = "http://www.opentnf.org", IsNullable = false)]
    public class SimpleTnfAttribute : TnfAttribute
    {
        private string[] m_values;

        [XmlElement("values")]
        public string[] Values
        {
            get { return m_values; }
            set { m_values = value; }
        }
    }

    [Serializable]
    [XmlType(Namespace = "http://www.opentnf.org")]
    [XmlRoot("StructuredAttribute", Namespace = "http://www.opentnf.org", IsNullable = false)]
    public class StructuredTnfAttribute : TnfAttribute
    {
        private TnfAttribute[] m_items;

        [XmlElement("SimpleAttribute", typeof(SimpleTnfAttribute))]
        [XmlElement("StructuredAttribute", typeof(StructuredTnfAttribute))]
        public TnfAttribute[] Items { get { return m_items; } set { m_items = value; } }
    }
}
