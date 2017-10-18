namespace OpenTNF.Library.Model
{
    interface ITnfPropertyObjectTypeValidForTypeOfTransport
    {
        int PropertyObjectTypeOid { get; }
        int CatalogueOid { get; }
        string TypeOfTransport { get; }
    }
}
