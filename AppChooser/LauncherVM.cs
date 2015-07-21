using SPTR.AppChooser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPTR.AppChooser
{
    public class LauncherVM
    {
        public List<RevitConfig> RevitVersions { get; set; }
        public LauncherVM()
        {
            RevitVersions = RevitConfig.GetConfigs();
        }
    }
}
