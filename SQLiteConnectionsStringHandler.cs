using System;

namespace OpenTNF.Library
{
	/// <summary>
	/// This class contains help methods for SQLite connection strings
	/// </summary>
	public class SQLiteConnectionsStringHandler
	{
		/// <summary>
		/// Method can be used to determine if a connection string describes a connection to a in memory SQLite db.
		/// </summary>
		/// <param name="fileNameOrUriString"></param>
		/// <returns></returns>
		public static bool IsMemoryDB(string fileNameOrUriString)
		{
			if (fileNameOrUriString.IndexOf(":memory:", StringComparison.InvariantCultureIgnoreCase) > -1)
			{
				return true;
			}
			return false;
		}
		/// <summary>
		/// If the <paramref name="fileNameOrUriString"/> is of 'FullUri' type the returned connection string is unchanged. 
		/// If not the string is assumed to be a filename and a connection string created containing with 'Data source=filename'
		/// </summary>
		/// <param name="fileNameOrUriString"></param>
		/// <returns>A SQLite connections string</returns>
		public static string GetSQLiteConnectionString(string fileNameOrUriString)
		{
			if (fileNameOrUriString.IndexOf("FullUri", StringComparison.InvariantCultureIgnoreCase) > -1)
			{
				return fileNameOrUriString;
			}
			else
			{
				return $"Data Source={fileNameOrUriString};Version=3;New=True;Compress=True;";
			}
		}
	}
}
