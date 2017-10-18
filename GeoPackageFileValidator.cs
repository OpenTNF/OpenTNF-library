using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTNF.Library.Validation;

namespace OpenTNF.Library
{
    public class GeoPackageFileValidator
    {
        public ValidationResult ValidateFileRequirements(string filePath)
        {
            List<ValidationMessage> validationMessages = new List<ValidationMessage>();
            try
            {
                byte[] file = File.ReadAllBytes(filePath);
                validationMessages.AddRange(ValidateRequirement1(file));
                validationMessages.AddRange(ValidateRequirement2(file));
                validationMessages.AddRange(ValidateRequirement3(filePath));
            }
            catch (Exception e)
            {
                validationMessages.Add(new ValidationMessage(ValidationMessageType.Error, 
                    string.Format(Properties.Resources.GpkgValidationErrorOnValidatingFile, filePath, e.Message)));
            }
            return new ValidationResult(validationMessages);
        }

        internal IEnumerable<ValidationMessage> ValidateRequirement1(byte[] file)
        {
            // Requirement 1
            // A GeoPackage SHALL be a SQLite [5] database file using version 3 of the SQLite file format [6] [7].
            // The first 16 bytes of a GeoPackage SHALL contain “SQLite format 3” [1] in ASCII [B4]. [2] 
            byte[] first16BytesInFile = file.Take(16).ToArray();
            string first16Bytes = System.Text.Encoding.ASCII.GetString(first16BytesInFile);
            if (!first16Bytes.Equals("SQLite format 3\0"))
                return new[] { new ValidationMessage(ValidationMessageType.Error, Properties.Resources.GpkgValidationException1) };
            return Enumerable.Empty<ValidationMessage>();
        }

        internal IEnumerable<ValidationMessage> ValidateRequirement2(byte[] file)
        {
            // Requirement 2
            // A GeoPackage SHALL contain 0x47503130 ("GP10" in ASCII) in the application id field of the SQLite 
            // database header to indicate a GeoPackage version 1.0 file.
            byte[] applicationIdFieldBytes = file.Skip(68).Take(4).ToArray();
            string applicationIdField = System.Text.Encoding.ASCII.GetString(applicationIdFieldBytes);
            if (!applicationIdField.Equals("GP10"))
                return new[] { new ValidationMessage(ValidationMessageType.Error, Properties.Resources.GpkgValidationException2) };
            return Enumerable.Empty<ValidationMessage>();
        }

        internal IEnumerable<ValidationMessage> ValidateRequirement3(string filePath)
        {
            // Requirement 3
            // A GeoPackage SHALL have the file extension name “.gpkg”.
            bool extensionOk = Path.GetExtension(filePath).ToLower().Equals(".gpkg") || Path.GetExtension(filePath).ToLower().Equals(".gpkx");
            if (!extensionOk)
                return new[] { new ValidationMessage(ValidationMessageType.Error, Properties.Resources.GpkgValidationException3) };
            return Enumerable.Empty<ValidationMessage>();
        }
    }
}
