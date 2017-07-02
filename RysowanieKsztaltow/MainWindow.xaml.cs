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
        private byte[] pixs, tmpPixs;
        private int pixsW, pixsH;
        private Rysownik rysownik;
        private Point p0;
        private double dpi;
        private Point kursorPos;
        private Action<int, int, int, int> akcja;

        public MainWindow()
        {
            InitializeComponent();

            dpi = 96;
            Loaded += delegate { ResetScreen(); };            
        }

        private void ResetScreen()
        {
            pixsW = (int)Ramka.ActualWidth;
            pixsH = (int)Ramka.ActualHeight;
            tmpPixs = new byte[4 * pixsW * pixsH];
            pixs = new byte[4 * pixsW * pixsH];
            rysownik = new Rysownik(ref tmpPixs, pixsW, pixsH);
            Screen.Source = BitmapSource.Create(pixsW, pixsH, dpi, dpi, PixelFormats.Bgra32, null, tmpPixs, 4 * pixsW);
        }

        private void Screen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            p0 = new Point(e.GetPosition(Screen).X, e.GetPosition(Screen).Y);
        }

        private void Screen_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Array.Copy(tmpPixs, pixs, tmpPixs.Length);
        }

        private void Screen_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                kursorPos = e.GetPosition(Screen);

                if (akcja != null)
                {
                    akcja((int)p0.X, (int)p0.Y, (int)e.GetPosition(Screen).X, (int)e.GetPosition(Screen).Y);
                    Screen.Source = BitmapSource.Create(pixsW, pixsH, dpi, dpi, PixelFormats.Bgra32, null, tmpPixs, 4 * pixsW);
                }
            }
        }

        private void BtnDrawLine_Click(object sender, RoutedEventArgs e)
        {
            BtnDrawCircle.IsChecked = false;
            BtnRubber.IsChecked = false;

            akcja = (x0, y0, x1, y1) => 
            {
                Array.Copy(pixs, tmpPixs, pixs.Length);
                rysownik.RysujLinie(x0, y0, x1, y1);
            };
        }

        private void BtnDrawCircle_Click(object sender, RoutedEventArgs e)
        {
            BtnDrawLine.IsChecked = false;
            BtnRubber.IsChecked = false;

            akcja = (x0, y0, x1, y1) =>
            {
                Array.Copy(pixs, tmpPixs, pixs.Length);
                rysownik.RysujKolo(x0, y0, x1, y1);
            };
        }

        private void BtnRubber_Click(object sender, RoutedEventArgs e)
        {
            BtnDrawLine.IsChecked = false;
            BtnDrawCircle.IsChecked = false;

            akcja = (x0, y0, x1, y1) => { rysownik.Gumka(x1, y1); };
        }
        
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            rysownik.CzyscEkran();
            Array.Copy(tmpPixs, pixs, tmpPixs.Length);
            Screen.Source = BitmapSource.Create(pixsW, pixsH, dpi, dpi, PixelFormats.Bgra32, null, tmpPixs, 4 * pixsW);
        }
        
        private void Screen_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int oldW = pixsW;
            int oldH = pixsH;

            pixsW = (int)Ramka.ActualWidth;
            pixsH = (int)Ramka.ActualHeight;
            tmpPixs = new byte[4 * pixsW * pixsH];
            rysownik = new Rysownik(ref tmpPixs, pixsW, pixsH);

            for (int i = 0; i < oldH; ++i)
            {
                for(int j = 0; j < oldW; ++j)
                {
                    var kolor = Rysownik.SprawdzKolor(j, i, pixs, oldW, oldH);
                    rysownik.RysujPiksel(j, i, kolor.R, kolor.G, kolor.B, kolor.A);
                }
            }

            pixs = new byte[4 * pixsW * pixsH];
            Array.Copy(tmpPixs, pixs, tmpPixs.Length);
            
            Screen.Source = BitmapSource.Create(pixsW, pixsH, dpi, dpi, PixelFormats.Bgra32, null, tmpPixs, 4 * pixsW);
        }
    }
}
