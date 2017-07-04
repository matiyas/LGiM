using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private Rysownik rysownik;
        private double dpi;
        private Action<int, int, int, int> akcja;
        private Size canvasSize;
        private List<Point> kliknietePunkty;
        private Action<MouseButtonEventArgs> DodajPunkt;

        public MainWindow()
        {
            InitializeComponent();

            kliknietePunkty = new List<Point>();
            DodajPunkt = e =>
            {
                if (kliknietePunkty.Count >= 4)
                {
                    kliknietePunkty.RemoveAt(0);
                }
                kliknietePunkty.Add(e.GetPosition(Screen));
            };

            dpi = 96;
            canvasSize.Width = 1366;
            canvasSize.Height = 768;
            tmpPixs = new byte[4 * (int)canvasSize.Width * (int)canvasSize.Height];
            pixs = new byte[4 * (int)canvasSize.Width * (int)canvasSize.Height];
            rysownik = new Rysownik(ref tmpPixs, (int)canvasSize.Width, (int)canvasSize.Height);
            Screen.Source = BitmapSource.Create((int)canvasSize.Width, (int)canvasSize.Height, dpi, dpi, PixelFormats.Bgra32, null, 
                tmpPixs, 4 * (int)canvasSize.Width);       
        }

        private void Screen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DodajPunkt(e);
            Array.Copy(pixs, tmpPixs, pixs.Length);
            
            if (kliknietePunkty.Count == 4 && RadioDrawCurve.IsChecked == true)
            {
                rysownik.RysujKrzywa(kliknietePunkty[0], kliknietePunkty[1], kliknietePunkty[2], Mouse.GetPosition(Screen)); 
            }
            Screen.Source = BitmapSource.Create((int)canvasSize.Width, (int)canvasSize.Height, dpi, dpi, PixelFormats.Bgra32, null,
                    tmpPixs, 4 * (int)canvasSize.Width);
        }

        private void Screen_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Array.Copy(tmpPixs, pixs, tmpPixs.Length);
        }

        private void Screen_MouseMove(object sender, MouseEventArgs e)
        {
            // Show on window actual mouse position on the screen.
            MousePos.Content = $"{Convert.ToInt32(e.GetPosition(Screen).X)} x {Convert.ToInt32(e.GetPosition(Screen).Y)}";

            // After pressing left mouse button on the screen, do the right action.
            if (e.LeftButton == MouseButtonState.Pressed && akcja != null)
            {
                kliknietePunkty[kliknietePunkty.Count - 1] = e.GetPosition(Screen);
                akcja((int)kliknietePunkty[kliknietePunkty.Count - 2].X, (int)kliknietePunkty[kliknietePunkty.Count - 2].Y,
                    (int)kliknietePunkty[kliknietePunkty.Count - 1].X, (int)kliknietePunkty[kliknietePunkty.Count - 1].Y);

                Screen.Source = BitmapSource.Create((int)canvasSize.Width, (int)canvasSize.Height, dpi, dpi, PixelFormats.Bgra32, null,
                    tmpPixs, 4 * (int)canvasSize.Width);
            }
        }

        private void RadioDrawLine_Click(object sender, RoutedEventArgs e)
        {
            akcja = (x0, y0, x1, y1) =>
            {
                Array.Copy(pixs, tmpPixs, pixs.Length);
                rysownik.RysujLinie(x0, y0, x1, y1);
            };
        }

        private void RadioDrawCircle_Click(object sender, RoutedEventArgs e)
        {
            akcja = (x0, y0, x1, y1) =>
            {
                Array.Copy(pixs, tmpPixs, pixs.Length);
                rysownik.RysujKolo(x0, y0, x1, y1);
            };
        }

        private void RadioDrawEllipse_Click(object sender, RoutedEventArgs e)
        {
            akcja = (x0, y0, x1, y1) =>
            {
                Array.Copy(pixs, tmpPixs, pixs.Length);
                rysownik.RysujElipse(x0, y0, x1, y1, 0);
            };
        }

        private void RadioDrawCurve_Click(object sender, RoutedEventArgs e)
        {
            akcja = (p0, p1, p2, p3) =>
            {
                if (kliknietePunkty.Count == 4)
                {
                    Array.Copy(pixs, tmpPixs, pixs.Length);
                    rysownik.RysujKrzywa(kliknietePunkty[0], kliknietePunkty[1], kliknietePunkty[2], Mouse.GetPosition(Screen));
                }
            };
        }

        private void RadioRubber_Click(object sender, RoutedEventArgs e)
        {
            akcja = (x0, y0, x1, y1) => { rysownik.Gumka(x1, y1); };
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            rysownik.CzyscEkran();
            Array.Copy(tmpPixs, pixs, tmpPixs.Length);
            Screen.Source = BitmapSource.Create((int)canvasSize.Width, (int)canvasSize.Height, dpi, dpi, PixelFormats.Bgra32, null,
                tmpPixs, 4 * (int)canvasSize.Width);
        }
    }
}
