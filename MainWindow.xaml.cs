using System.CodeDom;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using System.ComponentModel; // ovo je dodano zbog Problema REFRESH 


namespace Muzej
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<UmetnickaDela> Dela { get; set; } = new ObservableCollection<UmetnickaDela>();
        internal ObservableCollection<Kustos> Kustosi { get; set; } = new ObservableCollection<Kustos>();
        internal ObservableCollection<Tip> Tipovi { get; set; } = new ObservableCollection<Tip>();
        private Slika prevuciSliku;

        //private ICollectionView view; // PROBLEM REFRESH POSLE FILTERA
        private string file = "elementi.txt";
        private void Ulazni_podaci(string path)
        {
            try
            {
               

                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] parts = line.Trim().Split('|');
                        if (parts.Length == 6)
                        {
                            
                            
                            elementi.Add(new UmetnickaDela(
                                int.Parse(parts[0]),
                                parts[1],
                                parts[2],
                                int.Parse(parts[3]),
                                parts[4],
                                parts[5]
                            ));
                            
                        }
                        else
                        {
                            MessageBox.Show("Linija u fajlu nije ispravna: " + line);

                        }
                    }
                }
               
            }
            catch (Exception ex)
            {
               MessageBox.Show("Greška pri otvaranju fajla!");
              //  MessageBox.Show($"Ne mogu da otvorim fajl! Tražim ga ovde: {System.IO.Path.GetFullPath(path)}\n\nGreška: {ex.Message}");
            }
        }

        public void Cuvanje_Podataka(string filePath, ObservableCollection<UmetnickaDela> elementi)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(file))
                {
                    foreach (var d in elementi)
                        sw.WriteLine($"{d.Sifra}|{d.Naziv}|{d.Autor}|{d.Godina}|{d.Tip}|{d.IkonicaPath}");
                }


            }
            catch (Exception e)
            {
                MessageBox.Show("Greska pri upisu u fajl: " + e.Message);
            }
        }



        //PRVI TAB MAINWINDOW DIO
        public ObservableCollection<UmetnickaDela> elementi = new();

        public MainWindow()
        {
            InitializeComponent();




            /*     elementi.Add(new UmetnickaDela(001,"Zvezdana noć","Vincent van Gogh",1889,"slika", "Images/starrynight.jpg"));
                 elementi.Add(new UmetnickaDela(002, "Mislilac", "Auguste Rodin", 1904, "skulptura", "Images/thinker.jpg"));
                 elementi.Add(new UmetnickaDela(003, "Fontana", "Marcel Duchamp", 1917, "instalacija", "Images/fountain.jpg"));
                 
            */
            Ulazni_podaci("elementi.txt");
            Ulazni_kustosi("kustosi.txt");
            Datagrid.ItemsSource = elementi;
            // view = CollectionViewSource.GetDefaultView(elementi); // PROBLEM REFRESH POSLE FILTERA
            //  Datagrid.ItemsSource = view; // PROBLEM REFRESH POSLE FILTERA  
            //Nema potrebe za collectionViewSource
            //TreeView za kustose
            SinhronizujTreeView();
            KustosiTree.ItemsSource = Kustosi;

            tvUmetnickaDela.ItemsSource = Tipovi;
            DataContext = this;


        }

        internal ObservableCollection<UmetnickaDela> Elementi { get => elementi; set => elementi = value; }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            this.Pretraga_Naziv.Text = "";
            this.FilterCombo.SelectedIndex = 0;
            
            Datagrid.ItemsSource = elementi;
        }

        private void Izmeni_Click(object sender, RoutedEventArgs e)
        {
           if(Datagrid.SelectedItem  is UmetnickaDela selektovano)
            {
                var editWin = new addWindow(elementi, selektovano);
                editWin.Owner = this;
                editWin.ShowDialog();
                Cuvanje_Podataka("elementi.txt", elementi);
                //Ako budes radio bez INotfyPropertyChanged koristi Datagrid.Items.Refresh(); isto ti radi samo sto moras ovjde da pozoves ovu f ju
            }
            else
            {
                MessageBox.Show("Morate odabrati umetnicko delo za izmenu!", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }                  
            }



        //Menjao sam ga zbog treeview
        private void Dodaj_Click(object sender, RoutedEventArgs e)
        {
               var addWin = new addWindow(elementi);
            //addWin.Owner = this;
            if (addWin.ShowDialog() == true)
            {
                var delo = addWin.novoDelo;
                var tip = Tipovi.FirstOrDefault(t => t.Naziv == delo.Tip);
                if(tip == null)
                {
                    tip = new Tip(delo.Tip);
                    Tipovi.Add(tip);
                }
                if(!tip.Dela.Contains(delo))
                    tip.Dela.Add(delo); 

                PrimeniFilter();
                Cuvanje_Podataka("elementi.txt", elementi);
            }

        }

       

        private void Izbirisi_Click(object sender, RoutedEventArgs e)
        {
            if (Datagrid.SelectedItems == null || Datagrid.SelectedItems.Count == 0)
                return;

            if (MessageBox.Show("Da li ste sigurni da zelite da obrisete odabrane elemente?",
                                "Upozorenje",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (Datagrid.SelectedItems.Count == 1)
                {
                    // ako je selektovan samo jedan
                    elementi.Remove((UmetnickaDela)Datagrid.SelectedItem);
                    var kategorija = Tipovi.FirstOrDefault(t => t.Dela.Contains((UmetnickaDela)Datagrid.SelectedItem));
                    if (kategorija != null)
                    {
                        kategorija.Dela.Remove((UmetnickaDela)Datagrid.SelectedItem);
                        if (kategorija.Dela.Count == 0)
                        {
                            Tipovi.Remove(kategorija);
                        }
                    }
                }
                else
                {
                    // ako je selektovano više
                    List<UmetnickaDela> deleteList = Datagrid.SelectedItems.Cast<UmetnickaDela>().ToList();
                    foreach (UmetnickaDela s in deleteList)
                    {
                        elementi.Remove(s);
                        // sada sledi uklanjanje iz tree view dela
                        var kategorija = Tipovi.FirstOrDefault(t => t.Dela.Contains(s));
                        if(kategorija != null)
                        {
                            kategorija.Dela.Remove(s);
                            if (kategorija.Dela.Count == 0)
                            {
                                Tipovi.Remove(kategorija);
                            }
                        }
                    }
                }

                PrimeniFilter();
                Cuvanje_Podataka("elementi.txt", elementi);
            }
        }

        /*  private void Izbirisi_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
          {
              if (Datagrid.SelectedItems != null && MessageBox.Show("Da li ste sigurni da zelite da obrisete odabrane elemente?", "Uspesno", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
              {

                 List<UmetnickaDela> deleteList = Datagrid.SelectedItems.Cast<UmetnickaDela>().ToList(); //moje rijesenje za problem vise oznacenih elemenata
                  foreach(UmetnickaDela s in deleteList)
                      elementi.Remove(s);
             }
          } */


        //funkcije Filtriranja / Pretrage //
        private void PrimeniFilter()


        {
            string nazivFiltera = Pretraga_Naziv.Text.ToLower(); //prebacujem sve u mala slova , ako je korisnik unio npr slika ili Slika da mi radi  npr
            string tipFiltera = ((ComboBoxItem)FilterCombo.SelectedItem)?.Content.ToString();

            ObservableCollection<UmetnickaDela> filtrirani = new ObservableCollection<UmetnickaDela>(); // ovjde pravim listu u koju cu da ubacim elemente koji zadovoljavaju ifove dole 

            foreach(var d in elementi)
            {
                bool matchNaziv = string.IsNullOrWhiteSpace(nazivFiltera) || d.Naziv.ToLower().Contains(nazivFiltera);
                bool matchTip = tipFiltera == "Filtriranje po tipu" || d.Tip.ToLower() == tipFiltera.ToLower();

                if (matchNaziv && matchTip)
                    filtrirani.Add(d);
            }

            if(Datagrid != null)
            Datagrid.ItemsSource = filtrirani; // datagrid pokazuje samo filtrirane elemenete kolekcije , poslije kada se klikne reset dugme vraca se na =elementi;
        }

        private void FilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PrimeniFilter();
        }

        private void Pretraga_Naziv_TextChanged(object sender, TextChangedEventArgs e)
        {
            PrimeniFilter();
        }


        //Dugme za dodavanje umetnickih dela u drugom TAB-u
        



        //Dugme za dodavanje kustosa u drugom TAB-u
        private void DodajTab22_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new addTWWindow2(Kustosi);
            if (addWindow.ShowDialog() == true)
            {
                Kustosi.Add(addWindow.kus);
                SacuvajKustose("kustosi.txt", Kustosi);
            }
        }

        //Dugme koje menja treeview sa delima
        private void IzmeniDela_Click(object sender, RoutedEventArgs e)
        {
            if (tvUmetnickaDela.SelectedItem is UmetnickaDela selektovanoDelo)
            {
                var izmeniDelo = new addTWWindow1(selektovanoDelo);
                if (izmeniDelo.ShowDialog() == true)
                {
                    selektovanoDelo.Naziv = izmeniDelo.novoDelo.Naziv;
                    selektovanoDelo.Tip = izmeniDelo.novoDelo.Tip;
                    selektovanoDelo.IkonicaPath = izmeniDelo.novoDelo.IkonicaPath;

                    Cuvanje_Podataka("elementi.txt", elementi);
                }
            }
            else
            {
                MessageBox.Show("Morate izabrati delo za izmenu! ", "Obavestenje", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        //Dugme za brisanje dela iz treeview
        private void IzbrisiDela_Click(object sender, RoutedEventArgs e)
        {
           /* if (tvUmetnickaDela.SelectedItem is UmetnickaDela selektovanoDelo)
            {
                if ((MessageBox.Show("Da li ste sigurni da zelite da obrisete odabrane elemente?",
                                "Upozorenje",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) == MessageBoxResult.Yes))
                {
                    {
                        var kategorija = Tipovi.FirstOrDefault(t => t.Dela.Contains(selektovanoDelo));
                        if (kategorija != null)
                        {
                            kategorija.Dela.Remove(selektovanoDelo);
                            if (kategorija.Dela.Count == 0)
                            {
                                Tipovi.Remove(kategorija);
                            }
                        }
                        if (elementi.Contains(selektovanoDelo))
                            elementi.Remove(selektovanoDelo);

                        Datagrid.Items.Refresh();
                    }
                }
            }
            else
            {
                MessageBox.Show("Selektujte delo za brisanje! ", "Obavestenje", MessageBoxButton.OK, MessageBoxImage.Information);
            }*/
           if(KustosiTree.SelectedItem is UmetnickaDela selektovanoDelo)
            {
                var kustos = Kustosi.FirstOrDefault(k=>k.Dela.Contains(selektovanoDelo));
                if (kustos != null)
                {
                    if(MessageBox.Show("Da li ste sigurni da zelite da izbrisete odabrano delo iz kustosa?", "Upozerenje",MessageBoxButton.YesNo, MessageBoxImage.Question)==MessageBoxResult.Yes)
                            {
                        kustos.Dela.Remove(selektovanoDelo);
                        selektovanoDelo.Dodeljeno = false;
                        SacuvajKustose("kustosi.txt", Kustosi);
                    }
                }
            }
            else
            {
                MessageBox.Show("Morate selektovati delo kustosa za brisanje!","Obavestenje",MessageBoxButton.OK,MessageBoxImage.Information);
            }

        }

        //Funkcija koja sinhronizuje treeview
        private void SinhronizujTreeView()
        {
            foreach(var delo in elementi)
            {
                var tip = Tipovi.FirstOrDefault(t => t.Naziv == delo.Tip);
                if(tip==null)
                {
                    tip = new Tip(delo.Tip);
                    Tipovi.Add(tip);
                }
                if (!tip.Dela.Contains(delo))
                    tip.Dela.Add(delo);
            }
        }

        //Dugme koje menja kustose
        private void IzmeniKustose_Click(object sender, RoutedEventArgs e)
        {
            if (KustosiTree.SelectedItem is Kustos SelectedItem)
            {
                var izmenaKustosa = new addTWWindow2(SelectedItem, Kustosi);
                if (izmenaKustosa.ShowDialog() == true)
                {
                    SacuvajKustose("kustosi.txt", Kustosi);
                }
            }
             else
                {
                    MessageBox.Show("Morate izabrati kustosa za izmenu! ", "Obavestenje", MessageBoxButton.OK, MessageBoxImage.Information);
                }
        }

        //Dugme koje brise kustose
        private void IzbrisiKustose_Click(object sender, RoutedEventArgs e)
        {
            if (KustosiTree.SelectedItem is Kustos selectedItem)
            {
                if(MessageBox.Show("Da li ste sigurni da zelite da izbrisete odabranog kustosa?", "Upozorenje", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                   foreach(var delo in selectedItem.Dela)
                    {
                        delo.Dodeljeno = false;
                    }
                    Kustosi.Remove(selectedItem);
                    SacuvajKustose("kustosi.txt", Kustosi);
                }
            }
            else
            {
                MessageBox.Show("Selektujte kustosa za brisanje!", "Obavestenje", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void tvUmetnickaDela_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = (e.OriginalSource as FrameworkElement)?.DataContext as UmetnickaDela;
            if (item != null)
            {
                if (item.Dodeljeno)
                {
                    MessageBox.Show("Ovo delo je vec dodeljeno kustosu i ne moze se ponovo prevuci! ", "Info",MessageBoxButton.OK,MessageBoxImage.Information);
                    return;
                }
                var slika = new BitmapImage(new Uri(item.IkonicaPath, UriKind.RelativeOrAbsolute));
                prevuciSliku = new Slika(slika);
                prevuciSliku.Show();

                var prevuci = new DataObject(typeof(UmetnickaDela), item);

                DragDrop.DoDragDrop(tvUmetnickaDela,prevuci,DragDropEffects.Copy);

                prevuciSliku.Close();
                prevuciSliku = null; 
            }
        }

        private void KustosiTree_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(UmetnickaDela)))
            {
                var delo = e.Data.GetData(typeof(UmetnickaDela)) as UmetnickaDela;

                var treeViewItem = getNearestContainer(e.OriginalSource as DependencyObject);
                if(treeViewItem?.DataContext is Kustos selektovaniKustos)
                {
                    if (delo != null)
                    {
                        selektovaniKustos.Dela.Add(delo);
                        delo.Dodeljeno = true;
                        SacuvajKustose("kustosi.txt", Kustosi);
                    }
                }
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
        }

        private void KustosiTree_DragOver(object sender, DragEventArgs e)
        {
            /* if (prevuciSliku != null)
             {
                 var pos = e.GetPosition(this);
                 var toScreen = PointToScreen(pos);
                 prevuciSliku.setPosition(new Point(toScreen.X, toScreen.Y));
             }
             if (e.Data.GetDataPresent(typeof(UmetnickaDela)))
                 e.Effects = DragDropEffects.Copy;
             else
                 e.Effects = DragDropEffects.None;

             e.Handled = true;
        */
            if (e.Data.GetDataPresent(typeof(UmetnickaDela)))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            if(prevuciSliku != null)
            {
                var pozicija = e.GetPosition(this);
                var screenPoz = PointToScreen(pozicija);

                prevuciSliku.setPosition(new Point(screenPoz.X,screenPoz.Y));
            }
        }

        private TreeViewItem getNearestContainer(DependencyObject source)
        {
            while(source != null && !(source is TreeViewItem))
            {
                source = VisualTreeHelper.GetParent(source);
            }
            return source as TreeViewItem;
        }

        /* private void KustosiTree_PreviewDragOver(object sender, DragEventArgs e)
         {
             if (prevuciSliku != null)
             {
                 var pozicija = e.GetPosition(this);
                 var screenPoz = PointToScreen(pozicija);
                 prevuciSliku.setPosition(new Point(screenPoz.X, screenPoz.Y));
             }

             if (e.Data.GetDataPresent(typeof(UmetnickaDela)))
                 e.Effects = DragDropEffects.Copy;
             else
                 e.Effects = DragDropEffects.None;

             e.Handled = true;
         }*/

        /*private void Global_DragOver(object sender, DragEventArgs e)
        {
            // Pomeranje preview slike
            if (prevuciSliku != null)
            {
                var pozicija = e.GetPosition(this);
                var screenPoz = PointToScreen(pozicija);
                prevuciSliku.setPosition(new Point(screenPoz.X, screenPoz.Y));
            }

            // Dozvoli drop samo za umetnicka dela
            if (e.Data.GetDataPresent(typeof(UmetnickaDela)))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }*/




        // BAZA KUSTOSI TAB 2 


        private void Ulazni_kustosi(string path)
        {
            try
            {
                Kustosi.Clear();
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] parts = line.Trim().Split('|');
                        if (parts.Length == 4)
                        {
                            string ime = parts[0];
                            string prezime = parts[1];
                            string JMBG = parts[2];
                            string dela = parts[3]; 

                            var kustos = new Kustos(ime, prezime, JMBG);

                            if (!string.IsNullOrWhiteSpace(dela))
                            {
                                string[] sifre = dela.Split(',');
                                foreach (var s in sifre)
                                {
                                    if (int.TryParse(s, out int sifra))
                                    {
                                        UmetnickaDela delo = null;

                                        
                                        foreach (var d in elementi)
                                        {
                                            if (d.Sifra == sifra)
                                            {
                                                delo = d;
                                                break;
                                            }
                                        }

                                        if (delo != null)
                                        {
                                            kustos.Dela.Add(delo);
                                            delo.Dodeljeno = true;
                                        }
                                    }
                                }
                            }

                            Kustosi.Add(kustos);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Greška pri otvaranju fajla kustosi.txt: " + e.Message);
            }
        }

        private void SacuvajKustose(string path, ObservableCollection<Kustos> kustosi)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    foreach (var k in kustosi)
                    {
                        string dela = "";
                        for (int i = 0; i < k.Dela.Count; i++)
                        {
                            dela += k.Dela[i].Sifra;
                            if (i < k.Dela.Count - 1)
                                dela += ",";
                        }

                        sw.WriteLine($"{k.KustosIme}|{k.KustosPrezime}|{k.KustosJMBG}|{dela}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri čuvanju kustosa: " + ex.Message);
            }
        }


    }
}