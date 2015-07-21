using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPTR.AppChooser.Core
{
    /// <summary>
    /// Holds data about an external application
    /// </summary>
    public class ExternalAppData : ExternalProgram
    {
        public string AppName { get; set; }
        public bool IsDbApplication { get; set; }

        public override string DisplayName
        { get { return AppName; } }
    }
}
