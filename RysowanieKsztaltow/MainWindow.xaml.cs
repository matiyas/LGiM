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
            skala = 0.8;
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
                //rysownik.RysujLinie((int)(p.X * skala), (int)(p.Y * skala), (int)(e.GetPosition(Screen).X * skala), (int)(e.GetPosition(Screen).Y * skala));
                rysownik.RysujKolo((int)(p.X * skala), (int)(p.Y * skala), (int)(e.GetPosition(Screen).X * skala), (int)(e.GetPosition(Screen).Y * skala));
                Screen.Source = BitmapSource.Create(pixsW, pixsH, dpi, dpi, PixelFormats.Bgra32, bmp, pixs, 4 * pixsW);
            }
        }

        private void Screen_MouseLeave(object sender, MouseEventArgs e)
        {
            //var img = d.Image.FromFile("spectrum.bmp");
            //using (var ms = new MemoryStream())
            //{
            //    img.Save(ms, d.Imaging.ImageFormat.Bmp);
            //    byte[] pixs = ms.ToArray();
            //    Screen.Source = BitmapSource.Create(img.Width, img.Height, dpi, dpi, PixelFormats.Bgr32, null, pixs, img.Width * 4);

            //}
        }

        private void Screen_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResetScreen();
        }
    }
}
