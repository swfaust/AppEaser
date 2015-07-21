using Microsoft.Win32;
using SPTR.AppChooser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SPTR.AppChooser
{
    /// <summary>
    /// Interaction logic for UC_RevitConfig.xaml
    /// </summary>
    public partial class UC_RevitConfig : UserControl
    {
        public UC_RevitConfig()
        {
            InitializeComponent();
        }

        private void btn_Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Revit Files (*.rvt)|*.rvt";
            dlg.Multiselect = false;
            dlg.Title = "Select Revit File";

            if(dlg.ShowDialog()==true)
            {
                RevitConfig cfg = DataContext as RevitConfig;
                cfg.RevitFile = dlg.FileName;
            }
        }

        private void btn_Clean_Click(object sender, RoutedEventArgs e)
        {
            RevitConfig cfg = DataContext as RevitConfig;
            cfg.LaunchClean();
        }

        private void btn_Full_Click(object sender, RoutedEventArgs e)
        {
            RevitConfig cfg = DataContext as RevitConfig;
            cfg.LaunchFull();
        }

        private void btn_Custom_Click(object sender, RoutedEventArgs e)
        {
            RevitConfig cfg = DataContext as RevitConfig;
            cfg.LaunchCustom();
        }

        private void btn_AllYes_Click(object sender, RoutedEventArgs e)
        {
            RevitConfig cfg = DataContext as RevitConfig;
            cfg.SetEnabledAll(true);
        }

        private void btn_AllNo_Click(object sender, RoutedEventArgs e)
        {
            RevitConfig cfg = DataContext as RevitConfig;
            cfg.SetEnabledAll(false);
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            RevitConfig cfg = DataContext as RevitConfig;
            string cfgName = cfg.SelectedConfig;
            if(cfg.SelectedConfig=="<Unsaved>")
            {
                Dlg_ConfigName dlg = new Dlg_ConfigName();
                if (dlg.ShowDialog() == true)
                { cfgName = dlg.SelectedName; }
            }
            SharedElements.Settings.SaveConfig(cfgName, cfg);
            cfg.NotifyConfigsChanged();
            cfg.SelectedConfig = cfgName;
        }

        private void btn_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            RevitConfig cfg = DataContext as RevitConfig;
            Dlg_ConfigName dlg = new Dlg_ConfigName();
            if (dlg.ShowDialog() == true)
            {
                SharedElements.Settings.SaveConfig(dlg.SelectedName, cfg);
                cfg.NotifyConfigsChanged();
                cfg.SelectedConfig = dlg.SelectedName;
            }
        }
        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            RevitConfig cfg = DataContext as RevitConfig;
            string cfgName = cfg.SelectedConfig;
            if (cfg.SelectedConfig != "<Unsaved>")
            {
                SharedElements.Settings.DeleteConfig(cfg.SelectedConfig, cfg);
                cfg.NotifyConfigsChanged();
                cfg.SelectedConfig = "<Unsaved>";
            }
        }
    }
}
