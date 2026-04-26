using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muzej
{
    internal class Kustos : INotifyPropertyChanged
    {
        public string kustosIme;
        public string kustosPrezime;
        public string kustosJmbg;

        public ObservableCollection<UmetnickaDela> Dela { get; set; } = new ObservableCollection<UmetnickaDela>();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public Kustos(string ime = null, string prezime = null, string jMBG = null)
        {
            KustosIme = ime;
            KustosPrezime = prezime;
            KustosJMBG = jMBG;
        }


        public string KustosIme
        {
            get { return kustosIme; }
            set
            {
                if (kustosIme != value)
                {
                    kustosIme = value;
                    OnPropertyChanged("KustosIme");
                }
            }
        }
        public string KustosPrezime
        {
            get { return kustosPrezime; }
            set
            {
                if (kustosPrezime != value)
                {
                    kustosPrezime = value;
                    OnPropertyChanged("KustosPrezime");
                }
            }
        }
        public string KustosJMBG
        {
            get { return kustosJmbg; }
            set
            {
                if (kustosJmbg != value)
                {
                    kustosJmbg = value;
                    OnPropertyChanged("KustosJMBG");
                }
            }
        }
    }
}
