using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPTR.AppChooser.Core
{
    /// <summary>
    /// Parent class holding data about external program (command or application).  Many properties are not currently used.
    /// </summary>
    public abstract class ExternalProgram
    {
        public string AssemblyPath { get; set; }
        public Guid AddinId { get; set; }
        public string FullClassName { get; set; }
        public string VendorId { get; set; }
        public string VendorDescription { get; set; }
        public string LanguageType { get; set; }

        public abstract string DisplayName { get; }
    }
}
