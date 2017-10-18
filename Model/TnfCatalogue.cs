using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace OpenTNF.Library.Model
{
    public interface ITnfCatalogue
    {
        int Oid { get; }
        string Name { get; }
        string Version { get; }
        string Description { get; set; }
        string DefinitionSource { get; }
    }

    public class TnfCatalogue : ITnfCatalogue
    {
        public int Oid { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string DefinitionSource { get; set; }
        

      public override bool Equals(object obj)
        {
            bool retVal = false;

            if (obj is TnfCatalogue)
            {
                var c = obj as TnfCatalogue;

                if (Oid == c.Oid &&
                    Name == c.Name &&
                    Version == c.Version &&
                    DefinitionSource == c.DefinitionSource &&
                    Description == c.Description)
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(
                Oid,
                Name,
                Version,
                DefinitionSource,
                Description);
        }

        public override string ToString()
        {
            return String.Format("TnfCatalogue: TableName = {0}, DataType = {1}, Identifier = {2}, Description = {3}, DefinitionSource = {4}",
                Oid,
                Name,
                Version,
                Description,
                DefinitionSource);
        }
    }

    public class TnfCatalogueManager : TableManager
    {
        public static string TnfCatalogueTableName = "tnf_catalogue";

        public TnfCatalogueManager(GeoPackageDatabase db) : base(db, TnfCatalogueTableName, GetColumnInfos())
        {
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "oid",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "name",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "version",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "definition_source",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "description",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String"),
                    HandleMissing = true
                }
            };
        }

        public void Add(TnfCatalogue tnfCatalogue)
        {
            Add(new object[]
                {
                    tnfCatalogue.Oid,
                    tnfCatalogue.Name,
                    tnfCatalogue.Version,
                    tnfCatalogue.DefinitionSource,
                    tnfCatalogue.Description
                });
        }

        public TnfCatalogue GetByOid(int oid)
        {
            return Get(ReadObject, new object[] { oid });
        }

        public List<TnfCatalogue> GetMany(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(TnfCatalogue tnfCatalogue)
        {
            return Update(new object[]
                {
                    tnfCatalogue.Oid,
                    tnfCatalogue.Name,
                    tnfCatalogue.Version,
                    tnfCatalogue.DefinitionSource,
                    tnfCatalogue.Description,
                });
        }

        public int Delete(int oid)
        {
            return Delete(new object[] { oid });
        }

        private static TnfCatalogue ReadObject(IDataRecord reader)
        {
            var tnfCatalogue = new TnfCatalogue();

            tnfCatalogue.Oid = reader["oid"].ToInt();
            tnfCatalogue.Name = reader["name"].FromDbString();
            tnfCatalogue.Version = reader["version"].FromDbString();
            tnfCatalogue.DefinitionSource = reader["definition_source"].FromDbString();
            tnfCatalogue.Description = reader.ReadIfExists("description").FromDbString();

            return tnfCatalogue;
        }
    }
}
