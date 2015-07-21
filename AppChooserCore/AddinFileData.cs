using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace SPTR.AppChooser.Core
{
    /// <summary>
    /// Class for working with a specific addin file.
    /// </summary>
    public class AddinFileData : NotifyBase
    {
        bool _enabled;

        public enum PermissionLevel { User, AllUsers }

        public string FilePath { get; set; }
        public bool IsEnabled
        {
            get { return _enabled; }
            set
            {
                if(_enabled!=value)
                {
                    _enabled = value;
                    OnPropertyChanged("IsEnabled");
                    OnPropertyChanged("IsDisabled");
                }
            }
        }
        public bool IsDisabled
        {
            get { return !_enabled; }
            set
            {
                if(_enabled=value)
                {
                    _enabled = !value;
                    OnPropertyChanged("IsEnabled");
                    OnPropertyChanged("IsDisabled");
                }
            }
        }
        public PermissionLevel Level { get; set; }
        public List<ExternalAppData> ExternalApplications { get; set; }
        public List<ExternalCommandData> ExternalCommands { get; set; }

        public string DisplayName
        {
            get
            {
                if(ExternalApplications.Count==1)
                { return ExternalApplications[0].AppName; }

                if(ExternalCommands.Count==1)
                { return ExternalCommands[0].Text; }

                return Path.GetFileNameWithoutExtension(FilePath);
            }
        }

        public bool HasApplications
        { get { return ExternalApplications != null && ExternalApplications.Count > 0; } }
        public bool HasCommands
        { get { return ExternalCommands != null && ExternalCommands.Count > 0; } }

        public AddinFileData(string filePath, PermissionLevel lvl)
        {
            Level = lvl;
            FilePath = filePath;
            if (!File.Exists(filePath))
            { throw new Exception("The addin file '" + filePath + "' does not exist and could not be read"); }

            ExternalApplications = new List<ExternalAppData>();
            ExternalCommands = new List<ExternalCommandData>();

            IsEnabled = FilePath.ToLower().EndsWith(".addin");

            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(FilePath);

                XmlNode ndAddins = xDoc.GetElementsByTagName("RevitAddIns")[0];

                foreach(XmlNode ndAddin in ndAddins.ChildNodes)
                {
                    ExternalProgram prog = null;

                    switch(ndAddin.Attributes["Type"].Value)
                    {
                        case "Command":
                            prog = new ExternalCommandData();
                            break;
                        case "Application":
                            prog = new ExternalAppData() { IsDbApplication = false };
                            break;
                        case "DBApplication":
                            prog = new ExternalAppData() { IsDbApplication = true };
                            break;
                    }

                    //Set the common properties
                    foreach (XmlNode ndData in ndAddin.ChildNodes)
                    {
                        switch (ndData.Name)
                        {
                            case "Assembly":
                                prog.AssemblyPath = ndData.InnerText;
                                break;
                            case "FullClassName":
                                prog.FullClassName = ndData.InnerText;
                                break;
                            case "AddInId":
                            case "ClientId":
                                prog.AddinId = Guid.Parse(ndData.InnerText);
                                break;
                            case "VendorId":
                                prog.VendorId = ndData.InnerText;
                                break;
                            case "VendorDescription":
                                prog.VendorDescription = ndData.InnerText;
                                break;
                            case "LanguageType":
                                prog.LanguageType = ndData.Value;
                                break;
                        }
                    }

                    //Set type specific properties
                    if(prog is ExternalAppData)
                    {
                        ExternalAppData app = prog as ExternalAppData;
                        foreach (XmlNode ndData in ndAddin.ChildNodes)
                        {
                            switch (ndData.Name)
                            {
                                case "Name":
                                    app.AppName = ndData.InnerText;
                                    break;
                            }
                        }
                        ExternalApplications.Add(app);
                    }
                    else
                    {
                        ExternalCommandData cmd = prog as ExternalCommandData;
                        foreach(XmlNode ndData in ndAddin.ChildNodes)
                        {
                            switch(ndData.Name)
                            {
                                case "Text":
                                    cmd.Text = ndData.InnerText;
                                    break;
                                case "Description":
                                    cmd.Description = ndData.InnerText;
                                    break;
                                case "VisibilityMode":
                                    cmd.VisibilityModes.Add(ndData.InnerText);
                                    break;
                                case "Discipline":
                                    cmd.Disciplines.Add(ndData.InnerText);
                                    break;
                                case "AvailabilityClassName":
                                    cmd.AvailabilityClassName = ndData.InnerText;
                                    break;
                                case "LargeImage":
                                    cmd.LargeImage = ndData.InnerText;
                                    break;
                                case "SmallImage":
                                    cmd.SmallImage = ndData.InnerText;
                                    break;
                                case "LongDescription":
                                    cmd.LongDescription = ndData.InnerText;
                                    break;
                                case "TooltipImage":
                                    cmd.TooltipImage = ndData.InnerText;
                                    break;
                                case "AllowLoadIntoExistinSession":
                                    cmd.AllowLoadIntoExistingSession = bool.Parse(ndData.Value);
                                    break;
                            }
                        }
                        ExternalCommands.Add(cmd);
                    }
                }
            }
            catch (Exception ex)
            { throw new Exception("Error reading the addin file '" + FilePath + "'", ex); }
        }

        /// <summary>
        /// Enables the file by changing its path to .addin if it's not already
        /// </summary>
        public void EnableFile()
        {
            if (Path.GetExtension(FilePath) != "addin")
            {
                File.Move(FilePath, Path.ChangeExtension(FilePath, ".addin"));
                FilePath = Path.ChangeExtension(FilePath, ".addin");
            }
        }
        /// <summary>
        /// Disables the file by changing its path to .disable if it's not already.
        /// </summary>
        public void DisableFile()
        {
            if (Path.GetExtension(FilePath) != "disable")
            {
                File.Move(FilePath, Path.ChangeExtension(FilePath, ".disable"));
                FilePath = Path.ChangeExtension(FilePath, ".disable");
            }
        }
        /// <summary>
        /// Either enables or disables the file by changing its extension to match the enabled state in the UI.
        /// </summary>
        public void MatchFileToSetting()
        {
            if (IsEnabled)
            { EnableFile(); }
            else
            { DisableFile(); }
        }
    }
}
