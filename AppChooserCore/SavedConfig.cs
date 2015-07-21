using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPTR.AppChooser.Core
{
    /// <summary>
    /// Represents a saved configuration, used by the settings class
    /// </summary>
    public class SavedConfig
    {
        public string Name { get; set; }
        public int RevitYear { get; set; }

        public Dictionary<string, bool> AppSettings { get; set; }

        public SavedConfig()
        {
            AppSettings = new Dictionary<string, bool>();
        }
    }
}
