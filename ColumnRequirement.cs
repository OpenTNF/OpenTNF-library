namespace OpenTNF.Library
{
    public enum ColumnRequirement
    {
        /// <summary>
        /// The column must exist in the database
        /// </summary>
        Mandatory,
        /// <summary>
        /// The column may be missing, but if it exists it is editable
        /// </summary>
        OptionalEditable,
        /// <summary>
        /// The column may be missing, but editing of the table it belongs to will not be allowed
        /// </summary>
        OptionalReadOnly
    }
}
