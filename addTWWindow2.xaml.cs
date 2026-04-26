using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Muzej
{
    /// <summary>
    /// Interaction logic for addTWWindow2.xaml
    /// </summary>
    public partial class addTWWindow2 : Window
    {
        private bool isEdit = false;

        private ObservableCollection<Kustos> sviKustosi;
        internal Kustos kus;
        internal addTWWindow2(ObservableCollection<Kustos> kustosi)
        {
            InitializeComponent();
            sviKustosi = kustosi;
            kus = new Kustos();
            isEdit = false;
        }

        internal addTWWindow2(Kustos kustosZaIzmenu, ObservableCollection<Kustos> kustosi)
        {
            InitializeComponent();
            sviKustosi = kustosi;
            kus = kustosZaIzmenu;
            DataContext = kus;
            isEdit = true;

            ImeKustosa.Text = kus.KustosIme;
            PrezimeKustosa.Text = kus.KustosPrezime;
            JMBGKustosa.Text = kus.KustosJMBG;
        }


        private void Sacuvaj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ImeKustosa.Text))
                    throw new Exception("Unesite ime kustosa!");
                if (string.IsNullOrWhiteSpace(PrezimeKustosa.Text))
                    throw new Exception("Unesite prezime kustosa!");
                if (string.IsNullOrWhiteSpace(JMBGKustosa.Text))
                    throw new Exception("Unesite JMBG kustosa!");
                if (JMBGKustosa.Text.Length != 13 || !JMBGKustosa.Text.All(char.IsDigit))
                {
                    MessageBox.Show("JMBG mora imati 13 cifara!", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string jmbg = JMBGKustosa.Text;
                if (!isEdit || kus.KustosJMBG != jmbg)
                {
                    foreach (Kustos k in sviKustosi)
                    {
                        if (k.KustosJMBG == jmbg)
                            throw new Exception("Vec postoji kustos sa datim JMBG-om!");
                    }
                }
                kus.KustosIme = ImeKustosa.Text;
                kus.KustosPrezime = PrezimeKustosa.Text;
                kus.KustosJMBG = JMBGKustosa.Text;
                this.DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Otkazi_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
