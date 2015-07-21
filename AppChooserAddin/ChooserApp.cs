using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SPTR.AppChooser.Revit
{
    public class ChooserApp : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        { return Result.Succeeded; }

        public Result OnStartup(UIControlledApplication application)
        {
            string dir = @"C:\ProgramData\Autodesk\Revit\Addins\2016";

            foreach(string s in Directory.GetFiles(dir).Where(x=>x.Contains(".copy")))
            { File.Move(s, s.Replace(".copy",string.Empty)); }
            return Result.Succeeded;
        }
    }
}
