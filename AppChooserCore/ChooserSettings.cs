using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SPTR.AppChooser.Core
{
    /// <summary>
    /// Settings module for the program
    /// </summary>
    public class ChooserSettings
    {
        string _stgsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SPECtrumBIM", "AppChooser", "ChooserSettings.xml");

        public ObservableCollection<SavedConfig> Configs { get; set; }

        public ChooserSettings()
        {
            Configs = new ObservableCollection<SavedConfig>();
            LoadSettings();
        }

        /// <summary>
        /// Save a configuration state to the settings file.  Modifies if existing or creates if new.
        /// </summary>
        /// <param name="name">The name that should be used</param>
        /// <param name="cfg">The configuration holding the state to be saved</param>
        public void SaveConfig(string name, RevitConfig cfg)
        {
            //Check for existing
            SavedConfig sv = Configs.Where(x => x.Name == name && x.RevitYear == cfg.RevitYear).FirstOrDefault();
            if(sv==null)
            {
                sv = new SavedConfig() { RevitYear = cfg.RevitYear, Name = name };
                Configs.Add(sv);
            }

            //Clear it out and reset to the new state, then save
            sv.AppSettings.Clear();
            foreach(AddinFileData afd in cfg.AddinData)
            { sv.AppSettings.Add(afd.FilePath, afd.IsEnabled); }

            SaveSettings();
        }
        /// <summary>
        /// Remove an existing configuration from settings.
        /// </summary>
        /// <param name="name">The name to be removed</param>
        /// <param name="cfg">The config that it resides in.</param>
        public void DeleteConfig(string name, RevitConfig cfg)
        {
            SavedConfig sv = Configs.Where(x => x.Name == name && x.RevitYear == cfg.RevitYear).FirstOrDefault();
            if(sv!=null)
            {
                Configs.Remove(sv);
                SaveSettings();
            }
        }

        void LoadSettings()
        {
            Configs.Clear();
            XmlDocument xDoc = new XmlDocument();
            if (File.Exists(_stgsPath))
            {
                xDoc.Load(_stgsPath);
                foreach (XmlNode nd in xDoc.LastChild.ChildNodes)
                {
                    SavedConfig cfg = new SavedConfig() { Name = nd.Attributes["Name"].Value, RevitYear = int.Parse(nd.Attributes["RevitYear"].Value) };
                    foreach (XmlNode ndApp in nd.ChildNodes)
                    { cfg.AppSettings.Add(ndApp.Attributes["Name"].Value, bool.Parse(ndApp.Attributes["Enabled"].Value)); }
                    Configs.Add(cfg);
                }
            }
        }
        void SaveSettings()
        {
            XmlDocument xDoc = new XmlDocument();
            XmlDeclaration XmlProc = xDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            xDoc.AppendChild(XmlProc);

            XmlElement root = xDoc.CreateElement("SavedConfigs");

            foreach(SavedConfig cfg in Configs)
            {
                XmlElement ndRoot = xDoc.CreateElement("Configuration");
                XmlAttribute atName = xDoc.CreateAttribute("Name");
                atName.Value = cfg.Name;
                ndRoot.Attributes.Append(atName);

                XmlAttribute atRevitYear=xDoc.CreateAttribute("RevitYear");
                atRevitYear.Value=cfg.RevitYear.ToString();
                ndRoot.Attributes.Append(atRevitYear);
                
                foreach(KeyValuePair<string,bool> kv in cfg.AppSettings)
                {
                    XmlElement ndApp = xDoc.CreateElement("App");
                    XmlAttribute atAppName = xDoc.CreateAttribute("Name");
                    atAppName.Value = kv.Key;
                    ndApp.Attributes.Append(atAppName);
                    XmlAttribute atVal = xDoc.CreateAttribute("Enabled");
                    atVal.Value = kv.Value.ToString();
                    ndApp.Attributes.Append(atVal);
                    ndRoot.AppendChild(ndApp);
                }
                root.AppendChild(ndRoot);
            }

            string dirPath = Path.GetDirectoryName(_stgsPath);
            if(!Directory.Exists(dirPath))
            { Directory.CreateDirectory(dirPath); }

            xDoc.AppendChild(root);
            xDoc.Save(_stgsPath);
        }
    }
}
