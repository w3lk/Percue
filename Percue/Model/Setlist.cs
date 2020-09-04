using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Percue.Model
{
    public class Setlist : ObservableCollection<Channel>
    {
        public string Name { get; set; } = "New Setlist";

        
        public Setlist() : base()
        {
            
        }
    }
}
