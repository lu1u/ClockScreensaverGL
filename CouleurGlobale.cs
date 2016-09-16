/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 24/06/2014
 * Heure: 21:29
 * 
 * Pour changer ce modèle utiliser Outils  Options  Codage  Editer les en-têtes standards.
 */
using ClockScreenSaverGL.Config;
using System;
using System.Drawing;
namespace ClockScreenSaverGL
{
    /// <summary>
    /// Description of CouleurGlobale.
    /// </summary>
    public class CouleurGlobale
    {
        public const string CAT = "CouleurGlobale";
        const string HUE = "hue";
        const string VALUE = "value";
        const string SATURATION = "saturation";
        static protected CategorieConfiguration conf = Config.Configuration.getCategorie(CAT);

        public double _Hue = conf.getParametre( "Teinte", 0.5f, true);
        public double _Saturation = conf.getParametre("Saturation", 0.9f, true);
        public double _Luminance = conf.getParametre( "Valeur", 0.75f, true);
        private DateTime dernier = DateTime.Now;

        /// <summary>
        /// Change la teinte de la couleur a vitesse constante
        /// </summary>
        public void AvanceCouleur()
        {
            DateTime maintenant = DateTime.Now;

            _Hue += 0.05f * (double)(maintenant.Subtract(dernier).TotalMilliseconds / 1000.0f);
            while (_Hue > 1.0f)
                _Hue -= 1.0f;

            dernier = maintenant;
        }

        public void ChangeHue(int Sens)
        {
            Change(ref _Hue, Sens, HUE);

        }

        public Color ChangeTeinte(float change)
        {
            CouleurGlobale c = new CouleurGlobale();
            c._Hue = this._Hue;
            c._Luminance = this._Luminance;
            c._Saturation = this._Saturation;

            c.ChangeTeinte(change);
            return c.GetRGB();
        }

        
        public void ChangeSaturation(int Sens)
        {
            Change(ref _Saturation, Sens, SATURATION);
        }

        public void ChangeValue(int Sens)
        {
            Change(ref _Luminance, Sens, VALUE);
        }

        /// <summary>
        /// Change une des valeurs H, S ou V, dans un intervalle de 0 à 1
        /// </summary>
        /// <param name="V"></param>
        /// <param name="Sens"></param>
        private void Change(ref double V, int Sens, string parameterName)
        {
            if (Sens > 0)
            {
                if (V < 0.99)
                    V += 0.01f;
            }
            else
            {
                if (V > 0.01) V -= 0.01f;
            }

            conf.setParametre( parameterName, (float)V);
        }


        public override String ToString()
        {
            return "H:" + _Hue.ToString("0.00") + ",S:" + _Saturation.ToString("0.00") + ",V:" + _Luminance.ToString("0.00");
        }

        /// <summary>
        /// Conversion HSV -> RGB
        /// </summary>
        /// <returns></returns>
        public Color GetRGB()
        {
            double v, r, g, b;
            double Luminance = _Luminance;
            double Hue = _Hue;
            double Saturation = _Saturation;

            r = Luminance;   // default to gray
            g = Luminance;
            b = Luminance;
            v = (Luminance <= 0.5) ? (Luminance * (1.0f + Saturation)) : (Luminance + Saturation - Luminance * Saturation);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = Luminance + Luminance - v;
                sv = (v - m) / v;
                Hue *= 6.0f;
                sextant = (int)Hue;
                fract = Hue - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }

            if (r > 1.0f) r = 1.0f; else if (r < 0) r = 0;
            if (g > 1.0f) g = 1.0f; else if (g < 0) g = 0;
            if (b > 1.0f) b = 1.0f; else if (b < 0) b = 0;

            return Color.FromArgb(255, Convert.ToByte(r * 255.0f), Convert.ToByte(g * 255.0f), Convert.ToByte(b * 255.0f));
        }
    }
}
