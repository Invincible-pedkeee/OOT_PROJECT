using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Muzej
{
    public class UmetnickaDela : INotifyPropertyChanged
    {
        private int sifra;
        private string naziv;
        private string autor;
        private int godina;
        private string tip;
        private string ikonicaPath;


        // Treba nam za Drag and Drop da proveramo da li je odredjeno umetnicko delo vec dodeljeno kustosu 
        private bool dodeljeno = false;
        public UmetnickaDela() { int sifra = -1; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }



        public UmetnickaDela(int id, string name, string author, int year, string type, string imagePath)
        {
            this.sifra = id;
            this.naziv = name;
            this.autor = author;
            this.godina = year;
            this.tip = type;
            this.ikonicaPath = imagePath;
        }

        public int Sifra
        {
            get
            {
                return sifra;
            }
            set
            {
                if(sifra != value)
                {
                    sifra = value;
                    OnPropertyChanged("Sifra");
                }
            }
        }
        public string Naziv
        {
            get
            {
                return naziv;
            }
            set
            {
                if (naziv != value)
                {
                    naziv = value;
                    OnPropertyChanged("Naziv");
                }
            }
        }

        public string Autor
        {
            get
            {
                return autor;
            }
            set
            {
                if (autor != value)
                {
                    autor = value;
                    OnPropertyChanged("Autor");
                }
            }
        }

        public int Godina
        {
            get
            {
                return godina;
            }
            set
            {
                if (godina != value)
                {
                    godina = value;
                    OnPropertyChanged("Godina");
                }
            }
        }
        public string Tip
        {
            get
            {
                return tip;
            }
            set
            {
                if (tip != value)
                {
                    tip = value;
                    OnPropertyChanged("Tip");
                }
            }
        }
        public string IkonicaPath
        {
            get
            {
                return ikonicaPath;
            }
            set
            {
                if (ikonicaPath != value)
                {
                    ikonicaPath = value;
                    OnPropertyChanged("IkonicaPath");
                }
            }
        }

        public bool Dodeljeno
        {
            get
            {
                return dodeljeno;
            }
            set
            {
                if(dodeljeno!=value)
                {
                    dodeljeno = value;
                    OnPropertyChanged("Dodeljeno");
                }
            }
        }

        

        
    }
}
