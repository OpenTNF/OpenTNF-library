using System.Xml.Schema;
using System.Xml.Serialization;

namespace OpenTNF.Library.Model
{
    [System.Serializable]
    [System.Diagnostics.DebuggerStepThrough]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(Namespace = "http://www.triona.se/tnf")]
    [XmlRoot("Attributes", Namespace = "http://www.triona.se/tnf", IsNullable = false)]
    public class AttributesType
    {
        [XmlElement("SimpleAttribute", typeof(SimpleAttributeType), Namespace = "http://www.triona.se/tnf")]
        [XmlElement("StructuredAttribute", typeof(StructuredAttributeType), Namespace = "http://www.triona.se/tnf")]
        public AttributeType[] Items { get; set; }

        [XmlAttribute("catalogueOID", Namespace = "http://www.triona.se/tnf")]
        public string CatalogueOid { get; set; }

        [XmlAttribute("propertyObjectTypeOID", Namespace = "http://www.triona.se/tnf")]
        public string PropertyObjectTypeOid { get; set; }

        [XmlNamespaceDeclarations]
        private XmlSerializerNamespaces _xmlns = new XmlSerializerNamespaces();

        public AttributesType()
        {
            _xmlns.Add("tnf", "http://www.triona.se/tnf");
            _xmlns.Add("sch", "http://www.ascc.net/xml/schematron");
            _xmlns.Add("gco", "http://www.isotc211.org/2005/gco");
            _xmlns.Add("gmd", "http://www.isotc211.org/2005/gmd");
            _xmlns.Add("gsr", "http://www.isotc211.org/2005/gsr");
            _xmlns.Add("gss", "http://www.isotc211.org/2005/gss");
            _xmlns.Add("gts", "http://www.isotc211.org/2005/gts");
            _xmlns.Add("gml", "http://www.opengis.net/gml");
            _xmlns.Add("xlink", "http://www.w3.org/1999/xlink");
            _xmlns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            _xmlns.Add("xs", "http://www.w3.org/2001/XMLSchema");
        }

        public XmlSerializerNamespaces Namespaces { get { return _xmlns; } }

        [XmlAttribute(Namespace = XmlSchema.InstanceNamespace)]
        public string SchemaLocation = "http://www.opentnf.org/schemas/tnf_attr.xsd";
    }

    [System.Serializable]
    [System.Diagnostics.DebuggerStepThrough]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(Namespace = "http://www.triona.se/tnf")]
    [XmlRoot("SimpleAttribute", Namespace = "http://www.triona.se/tnf", IsNullable = false)]
    public class SimpleAttributeType : AttributeType
    {
        [XmlElement("values", Namespace = "http://www.triona.se/tnf")]
        public object[] Values { get; set; }
    }

    [XmlInclude(typeof (StructuredAttributeType))]
    [XmlInclude(typeof (SimpleAttributeType))]
    [System.Serializable]
    [System.Diagnostics.DebuggerStepThrough]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(Namespace = "http://www.triona.se/tnf")]
    public abstract class AttributeType
    {
        [XmlAttribute("attributeType", Namespace = "http://www.triona.se/tnf")]
        public string attributeType { get; set; }
    }

    [System.Serializable]
    [System.Diagnostics.DebuggerStepThrough]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(Namespace = "http://www.triona.se/tnf")]
    [XmlRoot("StructuredAttribute", Namespace = "http://www.triona.se/tnf", IsNullable = false)]
    public class StructuredAttributeType : AttributeType
    {
        [XmlElement("SimpleAttribute", typeof(SimpleAttributeType), Namespace = "http://www.triona.se/tnf")]
        [XmlElement("StructuredAttribute", typeof(StructuredAttributeType), Namespace = "http://www.triona.se/tnf")]
        public AttributeType[] Items { get; set; }

        [XmlNamespaceDeclarations]
        private XmlSerializerNamespaces _xmlns = new XmlSerializerNamespaces();

        public StructuredAttributeType()
        {
            _xmlns.Add("tnf", "http://www.triona.se/tnf");
        }
    }
}
