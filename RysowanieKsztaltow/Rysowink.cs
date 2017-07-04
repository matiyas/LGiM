﻿using System;
using System.Windows;
using System.Windows.Media;

namespace RysowanieKsztaltow
{
    class Rysownik
    {
        private byte[] pixs;
        private int wysokosc, szerokosc;
        public Color KolorPedzla;

        public Rysownik(ref byte[] pixs, int szerokosc, int wysokosc)
        {
            //KolorPedzla = new Color();
            KolorPedzla.R = KolorPedzla.G = KolorPedzla.B = 0;
            KolorPedzla.A = 255;

            this.pixs = pixs;
            this.szerokosc = szerokosc;
            this.wysokosc = wysokosc;
        }

        public void UstawPedzel(byte r, byte g, byte b, byte a)
        {
            KolorPedzla.R = r;
            KolorPedzla.G = g;
            KolorPedzla.B = b;
            KolorPedzla.A = a;
        }

        public void RysujPiksel(int x, int y)
        {
            if (x >= 0 && x < szerokosc && y >= 0 && y < wysokosc)
            {
                int pozycja = 4 * (y * szerokosc + x);
                pixs[pozycja] = KolorPedzla.B;
                pixs[pozycja + 1] = KolorPedzla.G;
                pixs[pozycja + 2] = KolorPedzla.R;
                pixs[pozycja + 3] = KolorPedzla.A;
            }
        }

        public void RysujPiksel(int x, int y, byte r, byte g, byte b, byte a)
        {
            if (x >= 0 && x < szerokosc && y >= 0 && y < wysokosc)
            {
                int pozycja = 4 * (y * szerokosc + x);
                pixs[pozycja] = b;
                pixs[pozycja + 1] = g;
                pixs[pozycja + 2] = r;
                pixs[pozycja + 3] = a;
            }
        }

        public void CzyscEkran()
        {
            for (int i = 0; i < pixs.Length; ++i)
            {
                pixs[i] = 255;
            }
        }

        public void RysujLinie(int x0, int y0, int x1, int y1)
        {
            int startX = Math.Min(x0, x1);
            int endX = Math.Max(x0, x1);
            int startY = Math.Min(y0, y1);
            int endY = Math.Max(y0, y1);
            int dx = endX - startX;
            int dy = endY - startY;

            if (dx > dy)
            {
                for (int x = startX; x <= endX; ++x)
                {
                    double y = (dy / (double)dx) * (x - x0) + y0;

                    if ((x1 > x0 && y1 > y0) || (x1 < x0 && y1 < y0))
                        RysujPiksel(x, (int)Math.Floor(y));
                    else
                        RysujPiksel(x, 2 * y0 - (int)Math.Floor(y));
                }
            }
            else
            {
                for (int y = startY; y <= endY; ++y)
                {
                    double x = (dx / (double)dy) * (y - y0) + x0;

                    if ((x1 > x0 && y1 > y0) || (x1 < x0 && y1 < y0))
                        RysujPiksel((int)Math.Floor(x), y);
                    else
                        RysujPiksel(2 * x0 - (int)Math.Floor(x), y);
                }
            }
        }

        public void RysujKolo(int x0, int y0, int x1, int y1)
        {
            int r = (int)Math.Abs(Math.Sqrt(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2)));

            for (int x = 0; x <= r; ++x)
            {
                int y = (int)Math.Floor(Math.Sqrt(r * r - x * x));

                // Prawy góra
                RysujPiksel(y + x0, -x + y0);
                RysujPiksel(x + x0, -y + y0);

                // Lewy góra
                RysujPiksel(-y + x0, -x + y0);
                RysujPiksel(-x + x0, -y + y0);

                // Prawy dół
                RysujPiksel(y + x0, x + y0);
                RysujPiksel(x + x0, y + y0);

                // Lewy dół
                RysujPiksel(-y + x0, x + y0);
                RysujPiksel(-x + x0, y + y0);
            }
        }

        public void RysujElipse(int x0, int y0, int x1, int y1, double beta)
        {
            beta *= (2 * Math.PI / 360);

            double x = 0;
            double y = 0;

            // Promienie elipsy
            int r1 = x1 - x0;
            int r2 = y1 - y0;

            for (int i = 0; i <= 360; ++i)
            {
                // Kąt obrotu kolejnych punktów w radianach
                double alfa = (i / 360.0) * (2 * Math.PI);

                // Zapomiętanie położenia poprzednich punktów
                double oldX = x;
                double oldY = y;

                // Obrót punktów
                x = r1 * Math.Cos(alfa);
                y = r2 * Math.Sin(alfa);

                // Obrót elipsy
                double tmp = x;
                x = x * Math.Cos(beta) - y * Math.Sin(beta);
                y = tmp * Math.Sin(beta) + y * Math.Cos(beta);

                if (i > 0)
                    RysujLinie((int)oldX + x0, (int)oldY + y0, (int)x + x0, (int)y + y0);
            }
        }

        public void RysujKrzywa(Point p0, Point p1, Point p2, Point p3)
        {
            Point tmp = p0;
            double step = 1.0 / 25.0;
            double t = 0;
            for (int i = 0; i <= 25; ++i)
            {
                double x = (-Math.Pow(t, 3) + 3 * Math.Pow(t, 2) - 3 * t + 1) / 6 * p0.X 
                         + (3 * Math.Pow(t, 3) - 6 * Math.Pow(t, 2) + 4) / 6 * p1.X 
                         + (-3 * Math.Pow(t, 3) + 3 * Math.Pow(t, 2) + 3 * t + 1) / 6 * p2.X 
                         + Math.Pow(t, 3) / 6 * p3.X;

                double y = (-Math.Pow(t, 3) + 3 * Math.Pow(t, 2) - 3 * t + 1) / 6 * p0.Y 
                         + (3 * Math.Pow(t, 3) - 6 * Math.Pow(t, 2) + 4) / 6 * p1.Y
                         + (-3 * Math.Pow(t, 3) + 3 * Math.Pow(t, 2) + 3 * t + 1) / 6 * p2.Y
                         + Math.Pow(t, 3) / 6 * p3.Y;

                if (i > 0) RysujLinie((int)x, (int)y, (int)tmp.X, (int)tmp.Y);
                tmp = new Point(x, y);
                t += step;
            }
        }

        public void Gumka(int x, int y)
        {
            for(int i = y - 2; i < y + 2; ++i)
            {
                for(int j = x - 2; j < x + 2; ++j)
                {
                    RysujPiksel(j, i, 255, 255, 255, 255);
                }
            }
        }

        public static Color SprawdzKolor(int x, int y, byte[] pixs, int szerokosc, int wysokosc)
        {
            if (x >= 0 && x < szerokosc && y >= 0 && y < wysokosc)
            {
                Color kolor = new Color();

                int pozycja = 4 * (y * szerokosc + x);
                kolor.B = pixs[pozycja];
                kolor.G = pixs[pozycja + 1];
                kolor.R = pixs[pozycja + 2];
                kolor.A = pixs[pozycja + 3];

                return kolor;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Pozycja z poza zakresu tablicy");
            }
        }
    }
}

