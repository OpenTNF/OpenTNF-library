namespace OpenTNF.Library
{
    internal class Hashing
    {
        /// <summary> 
        /// This is a simple hashing function from Robert Sedgwicks Hashing in C book.
        /// Also, some simple optimizations to the algorithm in order to speed up
        /// its hashing process have been added. from: www.partow.net
        /// </summary>
        /// <param name="values">array of objects, parameters combination that you need
        /// to get a unique hash code for them</param>
        /// <returns>Hash code</returns>
        internal static int RsHash(params object[] values)
        {
            const int b = 378551;
            int a = 63689;
            int hash = 0;

            unchecked
            {
                foreach (object value in values)
                {
                    if (value != null)
                    {
                        hash = hash * a + value.GetHashCode();
                        a = a * b;
                    }
                }
            }

            return hash;
        }
    }
}
