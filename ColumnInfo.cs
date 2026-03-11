namespace OpenTNF.Library
{
    public class ColumnInfo
    {
        public string Name { get; set; }
        public string SqlType { get; set; }
        public Type DataType { get; set; }
        public ColumnRequirement Requirement { get; set; } = ColumnRequirement.Mandatory;
        public bool HasZ { get; set; }
        public bool HasM { get; set; }
    }
}
