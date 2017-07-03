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
        private Rysownik rysownik;
        private Point p0;
        private Point p1;
        private double dpi;
        private Action<int, int, int, int> akcja;
        private Size canvasSize;

        public MainWindow()
        {
            InitializeComponent();

            dpi = 96;
            canvasSize.Width = 1366;
            canvasSize.Height = 768;
            tmpPixs = new byte[4 * (int)canvasSize.Width * (int)canvasSize.Height];
            pixs = new byte[4 * (int)canvasSize.Width * (int)canvasSize.Height];
            rysownik = new Rysownik(ref tmpPixs, (int)canvasSize.Width, (int)canvasSize.Height);
            Screen.Source = BitmapSource.Create((int)canvasSize.Width, (int)canvasSize.Height, dpi, dpi, PixelFormats.Bgra32, null, tmpPixs,
                4 * (int)canvasSize.Width);       
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
            p1 = e.GetPosition(Screen);

            // Show on window actual mouse position on the screen.
            MousePos.Content = $"{Convert.ToInt32(p1.X)} x {Convert.ToInt32(p1.Y)}";

            // After pressing left mouse button on the screen, do the right action.
            if (e.LeftButton == MouseButtonState.Pressed && akcja != null)
            {
                akcja((int)p0.X, (int)p0.Y, (int)p1.X, (int)p1.Y);
                Screen.Source = BitmapSource.Create((int)canvasSize.Width, (int)canvasSize.Height, dpi, dpi, PixelFormats.Bgra32, null,
                    tmpPixs, 4 * (int)canvasSize.Width);
            }
        }

        private void BtnDrawLine_Click(object sender, RoutedEventArgs e)
        {
            BtnDrawCircle.IsChecked = false;
            BtnRubber.IsChecked = false;
            BtnRysujElipse.IsChecked = false;

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
            BtnRysujElipse.IsChecked = false;

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
            BtnRysujElipse.IsChecked = false;

            akcja = (x0, y0, x1, y1) => { rysownik.Gumka(x1, y1); };
        }

        private void BtnRysujElipse_Click(object sender, RoutedEventArgs e)
        {
            BtnDrawLine.IsChecked = false;
            BtnDrawCircle.IsChecked = false;
            BtnRubber.IsChecked = false;

            akcja = (x0, y0, x1, y1) =>
            {
                Array.Copy(pixs, tmpPixs, pixs.Length);
                rysownik.RysujElipse(x0, y0, x1, y1, (int)EllipseSlider.Value);
            };
        }

        private void EllipseSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MousePos.Content = EllipseSlider.Value.ToString();
            //Array.Copy(pixs, tmpPixs, pixs.Length);
            rysownik.RysujElipse((int)p0.X, (int)p0.Y, (int)p1.X, (int)p1.Y, (int)EllipseSlider.Value);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            rysownik.CzyscEkran();
            Array.Copy(tmpPixs, pixs, tmpPixs.Length);
            Screen.Source = BitmapSource.Create((int)canvasSize.Width, (int)canvasSize.Height, dpi, dpi, PixelFormats.Bgra32, null, tmpPixs, 
                4 * (int)canvasSize.Width);
        }
    }
}
