using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SPTR.AppChooser.Core
{
    /// <summary>
    /// View Model class for a single Revit Year configuration
    /// </summary>
    public class RevitConfig : NotifyBase
    {
        #region Private Variables
        bool _selected = true;
        string _rvtFile = string.Empty;
        string _flavor = string.Empty;
        string _selectedConfig = "<Unsaved>";
        #endregion

        #region Public Properties
        /// <summary>
        /// List of the addin files related to this Revit version
        /// </summary>
        public List<AddinFileData> AddinData { get; set; }
        /// <summary>
        /// The Revit year this config relates to
        /// </summary>
        public int RevitYear { get; set; }
        /// <summary>
        /// Tab selection property for data binding
        /// </summary>
        public bool IsSelected
        {
            get { return _selected; }
            set
            {
                if(_selected!=value)
                {
                    _selected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }
        /// <summary>
        /// The File to open in Revit (or empty string if no file to be opened)
        /// </summary>
        public string RevitFile
        {
            get { return _rvtFile; }
            set
            {
                if(_rvtFile!=value)
                {
                    _rvtFile = value;
                    OnPropertyChanged("RevitFile");
                }
            }
        }
        /// <summary>
        /// The flavor of Revit that is currently selected.  May be empty if only one flavor is available (UI turns off)
        /// </summary>
        public string SelectedFlavor
        {
            get { return _flavor; }
            set
            {
                if(_flavor!=value)
                {
                    _flavor = value;
                    OnPropertyChanged("SelectedFlavor");
                }
            }
        }

        /// <summary>
        /// List of all installed flavors and their corresponding Revit exe file.
        /// </summary>
        public Dictionary<string, string> RevitFlavors { get; set; }

        /// <summary>
        /// The list of all saved configurations
        /// </summary>
        public List<string> ConfigOptions
        { get { return SharedElements.Settings.Configs.Where(x => x.RevitYear == RevitYear).Select(x => x.Name).Concat(new List<string>() { "<Unsaved>" }).ToList(); } }
        /// <summary>
        /// Bound property for selected config.  Changing this updates the enabled state for addin files.
        /// </summary>
        public string SelectedConfig
        {
            get { return _selectedConfig; }
            set
            {
                if(_selectedConfig!=value)
                {
                    _selectedConfig = value;
                    OnPropertyChanged("SelectedConfig");
                    if(_selectedConfig!="<Unsaved>")
                    {
                        SavedConfig cfg = SharedElements.Settings.Configs.Where(x => x.Name == _selectedConfig).FirstOrDefault();
                        if(cfg!=null)
                        {
                            foreach(AddinFileData afd in AddinData.Where(x=>cfg.AppSettings.ContainsKey(x.FilePath)))
                            { afd.IsEnabled = cfg.AppSettings[afd.FilePath]; }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Binding property for flavor selection combo box
        /// </summary>
        public List<string> FlavorOptions
        { get { return RevitFlavors.Select(x => x.Key).ToList(); } }
        /// <summary>
        /// Binding property that determines flavor selector visibility
        /// </summary>
        public bool MultiFlavor
        { get { return RevitFlavors.Count > 1; } }
        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="revitYear">The Revit year this will relate to</param>
        /// <param name="userPath">The preconstructed user location path</param>
        /// <param name="allUserPath">The preconstructed all user location path</param>
        private RevitConfig(int revitYear, string userPath, string allUserPath)
        {
            RevitYear = revitYear;
            AddinData = new List<AddinFileData>();
            RevitFlavors = new Dictionary<string, string>();

            //Get from user directory
            if (Directory.Exists(userPath))
            {
                foreach (string s in Directory.GetFiles(userPath, "*.addin", SearchOption.AllDirectories)
                    .Concat(Directory.GetFiles(userPath, "*.disable", SearchOption.AllDirectories)))
                { AddinData.Add(new AddinFileData(s, AddinFileData.PermissionLevel.User)); }
            }

            //Get from all users
            if (Directory.Exists(allUserPath))
            {
                foreach (string s in Directory.GetFiles(allUserPath, "*.addin", SearchOption.AllDirectories)
                    .Concat(Directory.GetFiles(allUserPath, "*.disable", SearchOption.AllDirectories)))
                { AddinData.Add(new AddinFileData(s, AddinFileData.PermissionLevel.AllUsers)); }
            }
        }

        /// <summary>
        /// cheap hack wrapper for when the configs change...
        /// </summary>
        public void NotifyConfigsChanged()
        { OnPropertyChanged("ConfigOptions"); }

        public static List<RevitConfig> GetConfigs()
        {
            List<RevitConfig> configs = new List<RevitConfig>();
            string exeBase = @"C:\Program Files\Autodesk";

            for (int i = 2011; i < 2030; i++)
            {
                //Get the .addin file paths for standard
                string userPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Autodesk\\Revit\\Addins", i.ToString());
                string allUserPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "Autodesk\\Revit\\Addins", i.ToString());

                //Look for exchange apps
                List<AddinFileData> exchangeApps = GetExchangeAddins(i);

                //Check if there is anything valid here.
                if (Directory.Exists(userPath) || Directory.Exists(allUserPath) || exchangeApps.Count>0)
                {
                    //Search for exe files as .addin's may exist without a valid Revit program
                    RevitConfig cfg = new RevitConfig(i, userPath, allUserPath);
                    
                    //Get different flavors if any exist
                    foreach (string rvtDirectory in Directory.GetDirectories(exeBase, "Revit *" + i.ToString()))
                    {
                        if (File.Exists(Path.Combine(rvtDirectory, "Revit.exe")))
                        {
                            cfg.RevitFlavors.Add(rvtDirectory.Replace(exeBase, string.Empty).Replace(@"\Revit ", string.Empty),
                                Path.Combine(rvtDirectory, "Revit.exe"));
                        }
                        else if (File.Exists(Path.Combine(rvtDirectory, "Program", "Revit.exe"))) //2013 and earlier support
                        {
                            cfg.RevitFlavors.Add(rvtDirectory.Replace(exeBase, string.Empty).Replace(@"\Revit ", string.Empty),
                                Path.Combine(rvtDirectory, "Program", "Revit.exe"));
                        }
                    }

                    //Add in exchange apps if any found
                    if(exchangeApps.Count>0)
                    { cfg.AddinData.AddRange(exchangeApps); }

                    //Add it to the set
                    configs.Add(cfg);
                }
            }

            return configs.Where(x => x.AddinData.Count > 0 && x.RevitFlavors.Count > 0).ToList();
        }

        public void SetEnabledAll(bool enable)
        { AddinData.ForEach(x => x.IsEnabled = enable); }

        public void LaunchClean()
        {
            AddinData.ForEach(x => x.DisableFile());
            string exe = MultiFlavor ? RevitFlavors[_flavor] : RevitFlavors.First().Value;
            if (string.IsNullOrEmpty(RevitFile))
            { Process.Start(exe); }
            else
            { Process.Start(exe, "\"" + RevitFile + "\""); }
        }
        public void LaunchFull()
        {
            AddinData.ForEach(x => x.EnableFile());
            string exe = MultiFlavor ? RevitFlavors[_flavor] : RevitFlavors.First().Value;
            if (string.IsNullOrEmpty(RevitFile))
            { Process.Start(exe); }
            else
            { Process.Start(exe, "\"" + RevitFile + "\""); }
        }
        public void LaunchCustom()
        {
            AddinData.ForEach(x => x.MatchFileToSetting());
            string exe = MultiFlavor ? RevitFlavors[_flavor] : RevitFlavors.First().Value;
            if (string.IsNullOrEmpty(RevitFile))
            { Process.Start(exe); }
            else
            { Process.Start(exe, "\"" + RevitFile + "\""); }
        }

        /// <summary>
        /// Since apps from exchange can use the bundle format they need to be retrieved separately.  This method retrieves them.
        /// </summary>
        /// <param name="revitYear"></param>
        /// <returns></returns>
        private static List<AddinFileData> GetExchangeAddins(int revitYear)
        {
            //Look in the exchange base for .bundle folders that have a PackageContents.xml file in them
            List<AddinFileData> addins = new List<AddinFileData>();

            string exchangeBase = @"C:\ProgramData\Autodesk\ApplicationPlugins";

            foreach (string pth in Directory.GetDirectories(exchangeBase, "*.bundle", SearchOption.TopDirectoryOnly)
                .Where(x => File.Exists(Path.Combine(x, "PackageContents.xml"))))
            {
                //Read the package contents file.
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(Path.Combine(pth, "PackageContents.xml"));

                //Look for component tags
                foreach(XmlNode ndComp in xDoc.GetElementsByTagName("Components"))
                {
                    //Use these variables to make sure we have a Revit addin that is for the correct Revit year
                    int revitMax = -1;
                    int revitMin = -1;
                    string addinPath = string.Empty;

                    foreach(XmlNode ndChild in ndComp.ChildNodes)
                    {
                        switch(ndChild.Name)
                        {
                            case "RuntimeRequirements":
                                //Only analyze if the platform includes Revit
                                if(ndChild.Attributes["Platform"].Value.ToLower().Contains("revit"))
                                {
                                    revitMax = int.Parse(ndChild.Attributes["SeriesMax"].Value.Replace("R", string.Empty));
                                    revitMin = int.Parse(ndChild.Attributes["SeriesMin"].Value.Replace("R", string.Empty));
                                }
                                break;
                            case "ComponentEntry":
                                addinPath = ndChild.Attributes["ModuleName"].Value;
                                break;
                        }
                    }

                    //Make sure it's valid and add it if so.
                    if (revitMax >= revitYear && revitMin <= revitYear)
                    {
                        string usablePath = Path.Combine(pth, addinPath);
                        if(!File.Exists(usablePath))
                        { usablePath = Path.ChangeExtension(usablePath, "disable"); }
                        AddinFileData fd = new AddinFileData(usablePath, AddinFileData.PermissionLevel.AllUsers);
                        addins.Add(fd);
                    }
                }
            }

            return addins;
        }
    }
}
