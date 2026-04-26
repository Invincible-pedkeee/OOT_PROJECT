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

namespace Muzej
{
    /// <summary>
    /// Interaction logic for addTWWindow1.xaml
    /// </summary>
    public partial class addTWWindow1 : Window
    {
        public UmetnickaDela novoDelo { get; private set; }
        private bool editMode;
        public addTWWindow1()
        {
            InitializeComponent();
            novoDelo = new UmetnickaDela();
            editMode = false;
        }

        public addTWWindow1(UmetnickaDela postojeceDelo)
        {
            InitializeComponent();
            if (postojeceDelo != null)
            {
                novoDelo = new UmetnickaDela();
                novoDelo.Naziv = postojeceDelo.Naziv;
                novoDelo.Tip = postojeceDelo.Tip;
                novoDelo.IkonicaPath = postojeceDelo.IkonicaPath;
               
                txtNazivTW.Text = postojeceDelo.Naziv;
                txtIkonica.Text = postojeceDelo.IkonicaPath;

            foreach(ComboBoxItem item in cmbTip.Items)
            {
                if(item.Content.ToString() == postojeceDelo.Tip)
                {
                    cmbTip.SelectedItem = item;
                    break;
                }
            }
            editMode = true;

        }
 }

        private void BiranjeIkonice_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Slike|*.png;*.jpg;*.jpeg;*.bmp";
            if (dlg.ShowDialog() == true)
            {
                txtIkonica.Text = dlg.FileName;
            }
        }

        private void Sacuvaj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbTip.SelectedIndex == -1)
                    throw new Exception("Morate izabrati tip umetnickog dela");
                if (string.IsNullOrWhiteSpace(txtNazivTW.Text))
                    throw new Exception("Morate uneti naziv dela");
                if (string.IsNullOrWhiteSpace(txtIkonica.Text))
                    throw new Exception("Morate uneti sliku umetnickog dela");

                if (cmbTip.SelectedItem is ComboBoxItem tipItem)
                {
                    novoDelo.Tip = tipItem.Content.ToString();
                    novoDelo.Naziv = txtNazivTW.Text;
                    novoDelo.IkonicaPath = txtIkonica.Text;
                    this.DialogResult = true;
                    this.Close();
                }
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
