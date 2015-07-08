using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ClockScreenSaverGL.Meteo
{
    class Meteo : DisplayedObject
    {
        #region PARAMETRES
        public const string CAT = "Meteo";
        private static readonly float ORIGINE_X = conf.getParametre(CAT, "OrigineX", 1000.0f);
        private static readonly float ORIGINE_Y = conf.getParametre(CAT, "OrigineY", 500.0f);
        private static readonly float VITESSE_X = conf.getParametre(CAT, "VitesseX", 30.0f);
        private static readonly float VITESSE_Y = conf.getParametre(CAT, "VitesseY", -21.0f);
        private static readonly byte ALPHA = (byte)conf.getParametre(CAT, "Alpha", 200);
        private static readonly float DELTA_ALPHA = conf.getParametre(CAT, "DeltaAlpha", 0.7f);
        private static readonly string METEO_URL = conf.getParametre(CAT, "URL", @"http://weather.yahooapis.com/forecastrss?w=588014&u=c");
        private static readonly int TAILLE_TITRE = conf.getParametre(CAT, "Taille Titre", 48);
        private static readonly int TAILLE_ICONE = conf.getParametre(CAT, "Taille Icone", 56);
        private static readonly int TAILLE_TEXTE = conf.getParametre(CAT, "Taille Texte", 32);
        private static readonly int TAILLE_TEXTE_PETIT = conf.getParametre(CAT, "Taille Texte Petit", 12);
        private static readonly int TAILLE_TEXTE_TITRE = conf.getParametre(CAT, "Taille Texte Titre", 12);
        private static readonly int TAILLE_TEXTE_LEVER_COUCHER = conf.getParametre(CAT, "Taille Texte Lever", 24);
        #endregion PARAMETRES ;

        private MeteoInfo _infos;
        private bool _droite;
        private float _X = 0;
        private float _Y = 0;

        /// <summary>
        /// Constructeur
        /// </summary>
        public Meteo()
        {
            if (r.Next(0, 2) > 0)
                _droite = true;
            else
                _droite = false;
            _taille = new SizeF(100, 0);
            _infos = new MeteoInfo(METEO_URL);
        }

        /// <summary>
        /// Affichage
        /// </summary>
        /// <param name="g"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleur"></param>
        public override void AfficheGDI(Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            Color col = getCouleurAvecAlpha(couleur, ALPHA);
            float Y = _Y ;

            using (Font fonteTitre = new Font(FontFamily.GenericSansSerif, TAILLE_TITRE, FontStyle.Bold, GraphicsUnit.Pixel)) 
            using (Brush brushTitre = new SolidBrush(col))
            using (Font fonteSousTitre = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_PETIT, FontStyle.Regular, GraphicsUnit.Pixel))
                if (_infos != null && _infos.donneesPretes)
                {
                    float LargeurMax = _taille.Width;

                    // Titre
                    {
                        g.DrawString(_infos._location, fonteTitre, brushTitre, _X - 10, Y);
                        Y += TAILLE_TITRE;

                        using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_TITRE, FontStyle.Regular, GraphicsUnit.Pixel))
                        {
                            g.DrawString(_infos._title, f, brushTitre, _X, Y);
                            Y += TAILLE_TEXTE_TITRE * 2;
                        }

                        using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_LEVER_COUCHER, FontStyle.Regular, GraphicsUnit.Pixel))
                        {
                            g.DrawString(String.Format(Resources.Lever, _infos.lever), f, brushTitre, _X, Y);
                            Y += TAILLE_TEXTE_LEVER_COUCHER;
                            g.DrawString(String.Format(Resources.Coucher, _infos.coucher), f, brushTitre, _X, Y);
                            Y += TAILLE_TEXTE_LEVER_COUCHER * 2;
                        }
                    }

                    Color c = getCouleurAvecAlpha(couleur, (byte)ALPHA);
                            
                    // Lignes de previsions
                    using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE, FontStyle.Regular, GraphicsUnit.Pixel))
                        foreach (LignePrevisionMeteo ligne in _infos._lignes)
                        {
                            DrawBitmapNuance(g, ligne.bmp, _X, Y, TAILLE_ICONE, TAILLE_ICONE, c);

                            string text = String.Format(Resources.Temperatures, ligne.day, ligne.TMin, ligne.TMax);
                            float tailleTexte = g.MeasureString(text, f).Width + TAILLE_ICONE;
                            if (tailleTexte > LargeurMax)
                                LargeurMax = tailleTexte;
                            using (Brush bLigne = new SolidBrush(c))
                            {
                                g.DrawString(text, f, bLigne, _X + TAILLE_ICONE, Y);
                                g.DrawString(ligne.text, fonteSousTitre, bLigne, _X + TAILLE_ICONE + TAILLE_TEXTE_PETIT, Y + TAILLE_TEXTE);
                            }
                            Y += TAILLE_ICONE * 1.5f;
                        }

                    float HauteurMax = Y - _Y;
                    if (_taille.Width < LargeurMax || _taille.Height < HauteurMax)
                        _taille = new SizeF((float)Math.Max(_taille.Width, LargeurMax), (float)Math.Max(_taille.Height, HauteurMax));

                    // Jauge de duree de validite des previsions
                    float actuelle = _infos.validitePassee();
                    using (Pen p = new Pen(col, 4))
                        g.DrawLine(p, _X, Y, _X + (_taille.Width * actuelle), Y);
                    using (Pen p = new Pen(getCouleurAvecAlpha(couleur, (byte)(ALPHA / 4)), 4))
                        g.DrawLine(p, _X + _taille.Width, Y, _X + _taille.Width - (_taille.Width * (1.0f - actuelle)), Y);
                }
                else
                {
                    // Meteo pas encore chargee
                    string t = "Chargement\nmétéo" ;
                    SizeF s = g.MeasureString(t, fonteTitre);
                    if (_taille.Width < s.Width || _taille.Height < s.Height)
                        _taille = s ;
                    g.DrawString(t, fonteTitre, brushTitre, _X, Y);
                }
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }


        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            if (_infos.MustRefresh(maintenant))
                _infos = new MeteoInfo(METEO_URL);

            if (_droite)
                _X = tailleEcran.Width - _taille.Width;
            else
                _X = 0;

            _Y = tailleEcran.Top + (tailleEcran.Height - _taille.Height) / 2;

            if (_droite)
                tailleEcran = new Rectangle((int)tailleEcran.X, (int)tailleEcran.Y, (int)_X, (int)tailleEcran.Height);
            else
                tailleEcran = new Rectangle((int)_taille.Width, (int)tailleEcran.Y, (int)(tailleEcran.Width - _taille.Width), (int)tailleEcran.Height);

#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }
    }
}
