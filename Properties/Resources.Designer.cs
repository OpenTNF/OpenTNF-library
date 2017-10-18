﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpenTNF.Library.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("OpenTNF.Library.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot read file &apos;{0}&apos;; Error message: {1}.
        /// </summary>
        internal static string GpkgValidationCannotReadFile {
            get {
                return ResourceManager.GetString("GpkgValidationCannotReadFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to validate file &apos;{0}&apos;; Error message: {1}.
        /// </summary>
        internal static string GpkgValidationErrorOnValidatingFile {
            get {
                return ResourceManager.GetString("GpkgValidationErrorOnValidatingFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not a valid GeoPackage file. The first 16 bytes of the GeoPackage does not contain &quot;SQLite format 3&quot; in ASCII..
        /// </summary>
        internal static string GpkgValidationException1 {
            get {
                return ResourceManager.GetString("GpkgValidationException1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not a valid GeoPackage file. The application id field of the SQLite databaseheader does not contain 0x47503130 (&quot;GP10&quot; in ASCII)..
        /// </summary>
        internal static string GpkgValidationException2 {
            get {
                return ResourceManager.GetString("GpkgValidationException2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not a valid GeoPackage file. The file does not have the file extension name &quot;.gpkg&quot; or &quot;.gpkx&quot;..
        /// </summary>
        internal static string GpkgValidationException3 {
            get {
                return ResourceManager.GetString("GpkgValidationException3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to More than one srid exist in the database: {0}..
        /// </summary>
        internal static string OpenTnfDatabaseHasMoreThanOneSrid {
            get {
                return ResourceManager.GetString("OpenTnfDatabaseHasMoreThanOneSrid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Srid is missing..
        /// </summary>
        internal static string OpenTnfSridMissing {
            get {
                return ResourceManager.GetString("OpenTnfSridMissing", resourceCulture);
            }
        }
    }
}
