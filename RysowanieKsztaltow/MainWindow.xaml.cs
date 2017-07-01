using System;
using System.Collections;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using d = System.Drawing;
using System.IO;

namespace RysowanieKsztaltow
{
    public partial class MainWindow : Window
    {
        private byte[] pixs;
        private int pixsW, pixsH;
        private Rysownik rysownik;
        private Point p;
        private double dpi;
        private double skala;

        public MainWindow()
        {
            InitializeComponent();

            dpi = 96;
            skala = 1;
            Loaded += delegate { ResetScreen(); };
        }

        private void ResetScreen()
        {
            pixsW = (int)(Ramka.ActualWidth * skala);
            pixsH = (int)(Ramka.ActualHeight * skala);
            pixs = new byte[4 * pixsW * pixsH];
            rysownik = new Rysownik(ref pixs, pixsW, pixsH);
            Screen.Source = BitmapSource.Create(pixsW, pixsH, dpi, dpi, PixelFormats.Bgra32, null, pixs, 4 * pixsW);
        }

        private void Screen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            p = new Point(e.GetPosition(Screen).X, e.GetPosition(Screen).Y);
        }

        private void Screen_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                rysownik.CzyscEkran();
                //rysownik.RysujLinie((int)p.X, (int)p.Y, (int)e.GetPosition(Screen).X, (int)e.GetPosition(Screen).Y);
                rysownik.RysujKolo((int)p.X, (int)p.Y, (int)e.GetPosition(Screen).X, (int)e.GetPosition(Screen).Y);
                Screen.Source = BitmapSource.Create(pixsW, pixsH, dpi, dpi, PixelFormats.Bgra32, null, pixs, 4 * pixsW);
            }
        }

        private void Screen_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResetScreen();
        }
    }
}
