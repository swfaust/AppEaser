using SPTR.AppChooser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPTR.AppChooser
{
    /// <summary>
    /// Interaction logic for Dlg_ConfigName.xaml
    /// </summary>
    public partial class Dlg_ConfigName : Window
    {
        public string SelectedName { get; set; }

        public Dlg_ConfigName()
        {
            InitializeComponent();
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(tb_Name.Text))
            {
                MessageBox.Show("Sorry, no blank names.  Please try again.");
                return;
            }

            if (SharedElements.Settings.Configs.Where(x=>x.Name==tb_Name.Text).FirstOrDefault()!=null)
            {
                MessageBox.Show("You already used that one...");
                return;
            }

            SelectedName = tb_Name.Text;
            DialogResult = true;
        }
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
