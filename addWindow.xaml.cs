using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// Interaction logic for addWindow.xaml
    /// </summary>
    public partial class addWindow : Window
    {
      /*   private int sifra ;
         private string naziv ;
         private string autor ;
         private int godina;
         private string tip;
         private string ikonica;
      */
        //private UmetnickaDela novoDelo;

        // internal UmetnickaDela NovoDelo = new UmetnickaDela();

        //ovdje ide internal vidljivost , ne znam zasto baca gresku na public 
        ObservableCollection<UmetnickaDela> elementi;
        private UmetnickaDela selektovano;
        public UmetnickaDela novoDelo { get; private set; } //sluzi da cuva za promenu za treeView


        public addWindow(ObservableCollection<UmetnickaDela> lista, UmetnickaDela deloZaIzmenu = null)
        {
            elementi = lista;
            selektovano = deloZaIzmenu;
            InitializeComponent();

            if(selektovano !=null)
            {
                txtSifra.Text = selektovano.Sifra.ToString();
                txtNaziv.Text = selektovano.Naziv;
                txtAutor.Text = selektovano.Autor;
                txtGodina.Text = selektovano.Godina.ToString();
                cmbTip.Text = selektovano.Tip;
                txtIkonica.Text = selektovano.IkonicaPath;
            }
        }
        
        private void Sacuvaj_Click(object sender,RoutedEventArgs e)
        {
            try { 
            int sifra = int.Parse(txtSifra.Text);
            //trycatch za naziv
            if (string.IsNullOrWhiteSpace(txtNaziv.Text))
                throw new Exception("Unesite naziv!");
                string naziv = txtNaziv.Text.Trim();

            if(string.IsNullOrWhiteSpace(txtAutor.Text))
                throw new Exception("Unesite autora!");
                string autor = txtAutor.Text.Trim();

            int godina = int.Parse(txtGodina.Text);
                if (godina > 2025)
                    throw new Exception("Jos ne postoji vremeplov :)!");



            if (cmbTip.SelectedItem == null)
                throw new Exception("Izaberi tip dela!");
            string tip = ((ComboBoxItem)cmbTip.SelectedItem)?.Content.ToString();


            string ikonicaPath = txtIkonica.Text.Trim();

            if(selektovano == null || selektovano.Sifra != sifra)
            {

                foreach (UmetnickaDela d in elementi)
                    if (d.Sifra == sifra)
                        throw new Exception("Vec postoji umetnicko delo sa tom sifrom!");

            }

            if(selektovano == null)            // ako je selektovano nula , znaci da dodajemo , ako selektovano nije nula mijenjamo to sto je selektovano 
            {
                UmetnickaDela novo = new UmetnickaDela(sifra, naziv, autor, godina, tip, ikonicaPath);
                elementi.Add(novo);
                    novoDelo = novo;
            }else
            {
                selektovano.Sifra = sifra;
                selektovano.Naziv = naziv;
                selektovano.Autor = autor;
                selektovano.Godina = godina;
                selektovano.Tip = tip;
                selektovano.IkonicaPath = ikonicaPath;
                    novoDelo = selektovano;


            }
            this.DialogResult = true;
            this.Close();
            }
            catch(FormatException)
            {
                MessageBox.Show("Sifra i godina moraju biti brojevi!", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }
        private void Otkazi_Click(object sender,RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }


        private void OdaberiIkonicu_Click(object sender,RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Slike|*.png;*.jpg;*.jpeg;*.bmp";
            if (dlg.ShowDialog() == true)
            {
                txtIkonica.Text = dlg.FileName;
            }
        }
    }

    
}
