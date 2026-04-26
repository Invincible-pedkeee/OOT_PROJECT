using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muzej
{
    internal class Tip
    {
        public string Naziv { get; set; }
        public ObservableCollection<UmetnickaDela> Dela { get; set; }

        public Tip(string naziv)
        {
            Naziv = naziv;
            Dela = new ObservableCollection<UmetnickaDela>();        }


    }
}
