using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPTR.AppChooser.Core
{
    public static class SharedElements
    {
        static ChooserSettings _settings = new ChooserSettings();

        public static ChooserSettings Settings
        { get { return _settings; } }
    }
}
