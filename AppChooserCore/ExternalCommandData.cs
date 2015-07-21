using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPTR.AppChooser.Core
{
    /// <summary>
    /// Holds data about an external command
    /// </summary>
    public class ExternalCommandData : ExternalProgram
    {
        public string Text { get; set; }
        public string Description { get; set; }
        public List<string> VisibilityModes { get; set; }
        public List<string> Disciplines { get; set; }
        public string AvailabilityClassName { get; set; }
        public string LargeImage { get; set; }
        public string SmallImage { get; set; }
        public string LongDescription { get; set; }
        public string TooltipImage { get; set; }
        public bool AllowLoadIntoExistingSession { get; set; }

        public override string DisplayName
        { get { return Text; } }

        public ExternalCommandData()
        {
            VisibilityModes = new List<string>();
            Disciplines = new List<string>();
        }
    }
}
