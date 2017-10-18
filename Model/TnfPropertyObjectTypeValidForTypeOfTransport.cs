using System;

namespace OpenTNF.Library.Model
{
    public class TnfPropertyObjectTypeValidForTypeOfTransport : ITnfPropertyObjectTypeValidForTypeOfTransport
    {
        public int PropertyObjectTypeOid { get; private set; }
        public int CatalogueOid { get; private set; }
        public string TypeOfTransport { get; private set; }

        public TnfPropertyObjectTypeValidForTypeOfTransport(int propertyObjectTypeOid, int catalogueOid, string typeOfTransport)
        {
            PropertyObjectTypeOid = propertyObjectTypeOid;
            CatalogueOid = catalogueOid;
            TypeOfTransport = typeOfTransport;
        }

        public override bool Equals(object obj)
        {
            var other = obj as TnfPropertyObjectTypeValidForTypeOfTransport;
            if (other == null) return false;

            return String.Equals(other.CatalogueOid, CatalogueOid)
                   && String.Equals(other.PropertyObjectTypeOid, PropertyObjectTypeOid)
                   && String.Equals(other.TypeOfTransport, TypeOfTransport);
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(PropertyObjectTypeOid, CatalogueOid, TypeOfTransport);
        }

        public override string ToString()
        {
            return
                String.Format(
                    "TnfPropertyObjectTypeValidForTypeOfTransport: PropertyObjectTypeOid = {0}, CatalogueOid = {1}, TypeOfTransport = {2}",
                    PropertyObjectTypeOid, CatalogueOid, TypeOfTransport);
        }
    }
}
