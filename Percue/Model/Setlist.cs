using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percue.Model
{
    public class Setlist : ObservableCollection<Channel> , INotifyPropertyChanged
    {
        private string name = "New Setlist";
        public string Name {
            get { return name; }
            set { name = value; OnPropertyChanged(new PropertyChangedEventArgs(nameof(Name))); } 
        }

        
        public Setlist() : base()
        {
            
            
        }

        

        
    }
}
