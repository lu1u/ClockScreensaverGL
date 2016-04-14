using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace ClockScreenSaverGL.DisplayedObjects.Meteo
{
    public class LignePrevisionMeteo : IDisposable
    {
        const int NB_HEURES_PREVI = 4;
         private Image _bmp;
        private string _date;
        private string _temperature;
        private string _texte;
        private string _vent;
        private string _pluie;

        public LignePrevisionMeteo(string icone, string date, string temperature, string texte, string vent, string pluie)
        {
            if (icone.StartsWith("background-position:") || icone.StartsWith("BACKGROUND-POSITION:"))
                icone = Regex.Match(icone, @"\d+").Value;
            _texte = texte;

            try
            {
                _bmp = Image.FromFile(Config.getImagePath(@"Meteo\" + icone + ".png"));
            }
            catch (Exception)
            {
                _bmp = Image.FromFile(Config.getImagePath(@"Meteo\inconnu.png"));
                _texte = "{" + icone + "}" + _texte;
            }
            _date = date;
            _temperature = temperature;
            _vent = vent;
            _pluie = pluie;
        }

        public void Dispose()
        {
            _bmp?.Dispose();
        }
        private float KelvinToCelsius(float v)
        {
            return v - 273.15f;
        }
        public float affiche(Graphics g, Font fTitre, Font fSousTitre, float Y)
        {
            if (_bmp != null)
                g.DrawImage(_bmp, 0, Y, PanneauInfos.TAILLE_ICONE, PanneauInfos.TAILLE_ICONE);
            float H = 0;
            SizeF size = g.MeasureString(_date, fTitre);
            g.DrawString(_date, fTitre, Brushes.White, PanneauInfos.TAILLE_ICONE, Y);
            H += size.Height;

            g.DrawString(_temperature, fTitre, Brushes.White, PanneauInfos.TAILLE_ICONE, Y + H);
            size = g.MeasureString(_temperature, fSousTitre);
            H += size.Height;

            g.DrawString(_texte, fTitre, Brushes.White, PanneauInfos.TAILLE_ICONE, Y + H);
            size = g.MeasureString(_temperature, fSousTitre);
            H += size.Height;

            return H;
        }


    }
}
