namespace OpenTNF.Library.Model
{
    public interface ITnfAffectedNetworkElement
    {
        string ChangeTransactionOid { get; }
        int ChangeOrderNumber { get; }
        string AffectedElementRef { get; }
    }

    public class TnfAffectedNetworkElement : ITnfAffectedNetworkElement
    {
        public string ChangeTransactionOid { get; set; }
        public int ChangeOrderNumber { get; set; }
        public string AffectedElementRef { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TnfAffectedNetworkElement v)
            {
                if (ChangeTransactionOid == v.ChangeTransactionOid &&
                    ChangeOrderNumber == v.ChangeOrderNumber &&
                    AffectedElementRef == v.AffectedElementRef)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(ChangeTransactionOid, ChangeOrderNumber, AffectedElementRef);
        }

        public override string ToString()
        {
            return string.Format("TnfAffectedNetworkElement: ChangeTransactionOid = {0}, ChangeOrderNumber = {1}, AffectedElementRef = {2}",
                ChangeTransactionOid, ChangeOrderNumber, AffectedElementRef);
        }
    }

    public class TnfAffectedNetworkElementManager : TableManager
    {
        public const string TnfAffectedNetworkElementTableName = "tnf_affected_network_element";

        public TnfAffectedNetworkElementManager(GeoPackageDatabase db) : base(db, TnfAffectedNetworkElementTableName, GetColumnInfos(), null)
        {
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "change_transaction_oid",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "change_order_number",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "affected_element_ref",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                }
            };
        }

        public void Add(TnfAffectedNetworkElement tnfAffectedNetworkElement)
        {
            Add(new object[]
                {
                    tnfAffectedNetworkElement.ChangeTransactionOid,
                    tnfAffectedNetworkElement.ChangeOrderNumber,
                    tnfAffectedNetworkElement.AffectedElementRef
                });
        }
    }
}
