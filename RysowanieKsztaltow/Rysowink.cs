using System;

namespace RysowanieKsztaltow
{
    class Rysownik
    {
        private byte kolorR, kolorG, kolorB, alpha;
        private byte[] pixs;
        private int wysokosc, szerokosc;

        public Rysownik(ref byte[] pixs, int szerokosc, int wysokosc)
        {
            kolorR = kolorG = kolorB = 0;
            alpha = 255;
            this.pixs = pixs;
            this.szerokosc = szerokosc;
            this.wysokosc = wysokosc;
        }

        public void UstawPedzel(byte r, byte g, byte b, byte a)
        {
            kolorR = r;
            kolorG = g;
            kolorB = b;
            alpha = a;
        }

        public void RysujPiksel(int x, int y)
        {
            if (x >= 0 && x < szerokosc && y >= 0 && y < wysokosc)
            {
                int pozycja = 4 * (y * szerokosc + x);
                pixs[pozycja] = kolorB;
                pixs[pozycja + 1] = kolorG;
                pixs[pozycja + 2] = kolorR;
                pixs[pozycja + 3] = alpha;
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
    }
}

