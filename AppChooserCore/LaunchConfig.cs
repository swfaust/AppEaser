using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPTR.AppChooser.Core
{
    class LaunchConfig : NotifyBase
    {
        string _name;
        bool _selected;

        public string Name
        {
            get { return _name; }
            set
            {
                if(_name!=value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
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
    }
}
