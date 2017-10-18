using System;
using System.Collections.Generic;
using System.Data;
using System.Net;

namespace OpenTNF.Library.Model
{
    public class GpkgSpatialRefSys
    {
        public string SrsName { get; set; }
        public int SrsId { get; set; }
        public string Organization { get; set; }
        public int OrganizationCoordsysId { get; set; }
        public string Definition { get; set; }
        public string Description  { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is GpkgSpatialRefSys)
            {
                var v = obj as GpkgSpatialRefSys;

                if (SrsName == v.SrsName &&
                    SrsId == v.SrsId &&
                    Organization == v.Organization &&
                    OrganizationCoordsysId == v.OrganizationCoordsysId &&
                    Definition == v.Definition &&
                    Description == v.Description)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Hashing.RsHash(
                SrsName,
                SrsId,
                Organization,
                OrganizationCoordsysId,
                Definition,
                Description);
        }

        public override string ToString()
        {
            return String.Format("GpkgSpatialRefSys: SrsName = {0}, SrsId = {1}, Organization = {2}, OrganizationCoordsysId = {3}, " +
                                 "Definition = {4}, Description = {5}",
                SrsName,
                SrsId,
                Organization,
                OrganizationCoordsysId,
                Definition,
                Description);
        }
    }

    public class GpkgSpatialRefSysManager : TableManager
    {
        public const string DefaultSpatialRefSysUrlFormat = "http://spatialreference.org/ref/epsg/{0}/ogcwkt/";
        private const string PrimaryKey = "srs_id";
        public static string GpkgSpatialRefSysTableName = "gpkg_spatial_ref_sys";

        public GpkgSpatialRefSysManager(GeoPackageDatabase db) : base(db, GpkgSpatialRefSysTableName, GetColumnInfos(),PrimaryKey)
        {
        }

        private static ColumnInfo[] GetColumnInfos()
        {
            return new[]
            {
                new ColumnInfo
                {
                    Name = "srs_name",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "srs_id",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "organization",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "organization_coordsys_id",
                    SqlType = "INTEGER NOT NULL",
                    DataType = Type.GetType("System.Int32")
                },
                new ColumnInfo
                {
                    Name = "definition",
                    SqlType = "TEXT NOT NULL",
                    DataType = Type.GetType("System.String")
                },
                new ColumnInfo
                {
                    Name = "description",
                    SqlType = "TEXT",
                    DataType = Type.GetType("System.String")
                },
            };
        }

        public void Add(GpkgSpatialRefSys gpkgSpatialRefSys)
        {
            Add(new object[]
                {
                    gpkgSpatialRefSys.SrsName,
                    gpkgSpatialRefSys.SrsId,
                    gpkgSpatialRefSys.Organization,
                    gpkgSpatialRefSys.OrganizationCoordsysId,
                    gpkgSpatialRefSys.Definition,
                    gpkgSpatialRefSys.Description
                });
        }

        public GpkgSpatialRefSys GetBySrsId(int srsId)
        {
            return Get(ReadObject, new object[] { srsId });
        }

        public List<GpkgSpatialRefSys> Get(int maxResults)
        {
            return Get(ReadObject, maxResults);
        }

        public int Update(GpkgSpatialRefSys gpkgSpatialRefSys)
        {
            return Update(new object[]
                {
                    gpkgSpatialRefSys.SrsName,
                    gpkgSpatialRefSys.SrsId,
                    gpkgSpatialRefSys.Organization,
                    gpkgSpatialRefSys.OrganizationCoordsysId,
                    gpkgSpatialRefSys.Definition,
                    gpkgSpatialRefSys.Description
                });
        }

        public int Delete(int srsId)
        {
            return Delete(new object[] { srsId });
        }

        public void AddOrUpdateFromUrl(string urlFormat, int srid)
        {
            bool isPlaceholder;
            var refSys = TryDownload(urlFormat, srid, out isPlaceholder);
            if (GetBySrsId(srid) == null)
            {
                Add(refSys);
            }
            else if(!isPlaceholder) // only replace existing entry for non-placeholder instances.
            {
                Update(refSys);
            }
        }

        private static GpkgSpatialRefSys ReadObject(IDataRecord reader)
        {
            var gpkgSpatialRefSys = new GpkgSpatialRefSys();

            gpkgSpatialRefSys.SrsName = reader["srs_name"].FromDbString();
            gpkgSpatialRefSys.SrsId = (Int32) reader["srs_id"].ToInt32();
            gpkgSpatialRefSys.Organization = reader["organization"].ToString();
            gpkgSpatialRefSys.OrganizationCoordsysId = (Int32) reader["organization_coordsys_id"].ToInt32();
            gpkgSpatialRefSys.Definition = reader["definition"].FromDbString();
            gpkgSpatialRefSys.Description = reader["description"].FromDbString();

            return gpkgSpatialRefSys;
        }

        public static GpkgSpatialRefSys TryDownload(string urlFormat, int srid, out bool isPlaceholder)
        {
            isPlaceholder = false;
            string name, description, definition;

            name = $"EPSG:{srid}";
            if (urlFormat == null)
            {
                definition = "N/A";
                description = "No definition found.";
                isPlaceholder = true;
            }
            else
            {
                string url = string.Format(urlFormat, srid);
                try
                {
                    using (var webClient = new WebClient())
                    {
                        definition = webClient.DownloadString(url);
                    }
                    string parsedName;
                    if (TryParseRefSysName(definition, out parsedName))
                    {
                        name = parsedName;
                    }
                    description = $"Definition downloaded from {url}";
                }
                catch (Exception e)
                {
                    definition = "N/A";
                    description = $"Unable to download definition from {url}: {e.Message}";
                    isPlaceholder = true;
                }
            }
            return new GpkgSpatialRefSys
            {
                SrsName = name,
                SrsId = srid,
                Organization = "EPSG",
                OrganizationCoordsysId = srid,
                Definition = definition,
                Description = description
            };
        }

        private static bool TryParseRefSysName(string definition, out string name)
        {
            name = null;
            int startIndex = definition.IndexOf("\"");
            if (startIndex == -1)
            {
                return false;
            }
            startIndex++;
            int endIndex = definition.IndexOf("\"", startIndex);
            if (endIndex == -1)
            {
                return false;
            }
            name = definition.Substring(startIndex, (endIndex - startIndex));
            return true;
        }

    }
}
