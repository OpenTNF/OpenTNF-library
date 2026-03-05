namespace OpenTNF.Library.Model
{
    /// <summary>
    /// Describes the types of network references a TnfPropertyObjectType (for example) can have.
    /// Represents NetworkReferenceType in the info model.
    /// </summary>
    public enum TnfNetworkReferenceType
    {
        /// <summary>
        /// Default undefined value
        /// </summary>
        Undefined = -1,
        /// <summary>
        /// Default undefined value
        /// </summary>
        None = 0,
        /// <summary>
        /// Describes a point on a networkelement (node, link or linksequence)
        /// </summary>
        PointOnNodeOrLinearElement = 5,
        /// <summary>
        /// Describes a point on  a linksequence
        /// </summary>
        PointOnLinearElement = 4,
        /// <summary>
        /// Represents a part of a linksequence or a whole linksequence, that has a length > 0
        /// </summary>
        SegmentOnLinearElement = 8, 
        /// <summary>
        /// Represents a turn (i.e an extent from a link to another link via a node)
        /// </summary>
        Turn = 64,
        /// <summary>
        /// Represents a node reference
        /// </summary>
        Node = 1,
        /// <summary>
        /// Represents a road reference
        /// </summary>
        Road = 16,
        /// <summary>
        /// Represents a turn on a railway network element
        /// </summary>
        RailwayTurn = 512,
    }

    public class TnfNetworkReferenceTypeUtils
    {
        public static TnfNetworkReferenceType TranslateToNetworkReferenceType(int networkReferenceType, out bool hasHost)
        {
            TnfNetworkReferenceType ret;
            hasHost = false;
            int extType = networkReferenceType & 1023;
            switch (extType)
            {
                case 0:
                    ret = TnfNetworkReferenceType.None;
                    break;
                case 1:
                    ret = TnfNetworkReferenceType.Node;
                    break;
                case 4:
                    ret = TnfNetworkReferenceType.PointOnLinearElement;
                    break;
                case 5:
                    ret = TnfNetworkReferenceType.PointOnNodeOrLinearElement;
                    break;
                case 8:
                    ret = TnfNetworkReferenceType.SegmentOnLinearElement;
                    break;
                case 16:
                    ret = TnfNetworkReferenceType.Road;
                    break;
                case 64:
                    ret = TnfNetworkReferenceType.Turn;
                    break;
                case 256:
                    ret = TnfNetworkReferenceType.Road;
                    hasHost = true;
                    break;
                case 512:
                    ret = TnfNetworkReferenceType.RailwayTurn;
                    break;
                default:
                    ret = TnfNetworkReferenceType.Undefined;
                    break;
            }
            return ret;
        }
    }
}
