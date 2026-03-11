# Changelog

All notable changes to OpenTNF.Library will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2026-03-05

### Changed

- **Upgraded target framework to .NET 10** (from .NET Framework)
- Converted to SDK-style project format (`OpenTNF.Library.csproj`)
- Migrated assembly metadata from `Properties/AssemblyInfo.cs` to `.csproj`:
  - `AssemblyTitle`: OGC GeoPackage w. OpenTNF extension library
  - `Company`: Triona AB
  - `Product`: Triona.OpenTNF.Library
  - `Copyright`: Copyright © Triona AB
- Enabled implicit usings (`<ImplicitUsings>enable</ImplicitUsings>`)

### Updated

- `log4net` to version 3.2.0
- `System.Configuration.ConfigurationManager` to version 10.0.0
- `System.Data.SQLite.Core` to version 1.0.119

### Added

- Optional property EnumShortName in TnfValidValue

### Removed

- `Properties/AssemblyInfo.cs` (metadata moved to `.csproj`)
- Legacy assembly attributes (`ComVisible`, `Guid`, `AssemblyConfiguration`)
- Support for inheritance in TnfPropertyObjectType (BaseCatalogueOid, BasePropertyObjectTypeOid and IsDerived properties)

### Fixed

- Updated `.gitignore` to exclude `bin/`, `obj/`, `.vs/` and build artifacts

