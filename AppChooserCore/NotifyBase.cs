using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SPTR.AppChooser.Core
{
    public class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertyChanged(string propName)
        {
            if(PropertyChanged!=null)
            { PropertyChanged(this, new PropertyChangedEventArgs(propName)); }
        }
    }
}
