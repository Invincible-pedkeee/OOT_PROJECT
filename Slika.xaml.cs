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
using System.Windows.Xps.Serialization;

namespace Muzej
{
    /// <summary>
    /// Interaction logic for Slika.xaml
    /// </summary>
    public partial class Slika : Window
    {
        public Slika(ImageSource slika)
        {
            Width = 60;
            Height = 60;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            IsHitTestVisible = false;
            ShowInTaskbar = false;
            Topmost = true;

            var img = new Image();
            img.Source = slika;
            img.Width = 60;
            img.Height = 60;

            Content = img;
        }
        public void setPosition(Point pos)
        {
            Left = pos.X-Width/2;
            Top = pos.Y-Width/2;
        }
    }
}
