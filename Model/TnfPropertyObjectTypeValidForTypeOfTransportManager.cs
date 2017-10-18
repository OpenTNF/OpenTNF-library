using System;
using System.Collections.Generic;
using System.Data;

namespace OpenTNF.Library.Model
{
    public class TnfPropertyObjectTypeValidForTypeOfTransportManager : TableManager
    {
        private const string PrimaryKey = "property_object_type_oid, catalogue_oid, type_of_transport";
        public static string ValidForTypeOfTransportTableName = "tnf_property_object_type_valid_for_type_of_transport";

        public static string PropertyObjectTypeOidColumnName = "property_object_type_oid";
        public static string CatalogueOidColumnName = "catalogue_oid";
        public static string TypeOfTransportColumnName = "type_of_transport";
        
        public TnfPropertyObjectTypeValidForTypeOfTransportManager(GeoPackageDatabase db)
            : base(db, ValidForTypeOfTransportTableName, GetColumnInfos(), PrimaryKey)
        {
        }

        protected override string[] Constraints()
        {
            return new[]
            {
                String.Format(
                    "CONSTRAINT fk_tpotvtt_pot_co FOREIGN KEY ({0}, {1}) REFERENCES {2}(oid, catalogue_oid)",
                    PropertyObjectTypeOidColumnName, CatalogueOidColumnName,TnfPropertyObjectTypeManager.TnfPropertyObjectTypeTableName)
            };
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = PropertyObjectTypeOidColumnName,
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = CatalogueOidColumnName,
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = TypeOfTransportColumnName,
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                }
            };
        }

        public void Add(TnfPropertyObjectTypeValidForTypeOfTransport typeOfTransport)
        {
            Add(new object[]
                {
                    typeOfTransport.PropertyObjectTypeOid,
                    typeOfTransport.CatalogueOid,
                    typeOfTransport.TypeOfTransport
                });
        }

        public int Delete(int propertyObjectTypeOid, int catalogueOid, string typeOfTransport)
        {
            return Delete(new object[] { propertyObjectTypeOid, catalogueOid, typeOfTransport });
        }

        public List<TnfPropertyObjectTypeValidForTypeOfTransport> GetByMaxResult(int maxResults)
        {
            return Get(ReadValidForTypeOfTransport, maxResults);
        }
        public List<TnfPropertyObjectTypeValidForTypeOfTransport> GetAllForType(int catalogueOid, int propertyObjectTypeOid)
        {
            if (!Db.TableExists(ValidForTypeOfTransportTableName))
            {
                return new List<TnfPropertyObjectTypeValidForTypeOfTransport>();
            }

            List<TnfPropertyObjectTypeValidForTypeOfTransport> ret = new List<TnfPropertyObjectTypeValidForTypeOfTransport>();

            string commandText = string.Format(@"select * from {0} where {1}='{2}' and {3}='{4}'",
                    ValidForTypeOfTransportTableName,
                    CatalogueOidColumnName,
                    catalogueOid,
                    PropertyObjectTypeOidColumnName,
                    propertyObjectTypeOid);

            using (IDataReader idataReader = Db.ExecuteReader(commandText))
            {
                while (idataReader.Read())
                {
                    TnfPropertyObjectTypeValidForTypeOfTransport validForTypeOfTransport = ReadValidForTypeOfTransport(idataReader);
                    ret.Add(validForTypeOfTransport);
                }
            }
            return ret;
        }
        private TnfPropertyObjectTypeValidForTypeOfTransport ReadValidForTypeOfTransport(IDataReader idataReader)
        {
            return new TnfPropertyObjectTypeValidForTypeOfTransport(
                idataReader[PropertyObjectTypeOidColumnName].ToInt(),
                idataReader[CatalogueOidColumnName].ToInt(),
                idataReader[TypeOfTransportColumnName].ToString());
        }
    }
}
