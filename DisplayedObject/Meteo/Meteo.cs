﻿using System;
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

        //private TrajectoireDiagonale _trajectoire;
        private MeteoInfo _infos;
        private bool _droite;
        private float X = 0;
        public Meteo()
        {
            if (r.Next(0, 2) > 0)
                _droite = true;
            else
                _droite = false;

            //_trajectoire = new TrajectoireDiagonale(ORIGINE_X, ORIGINE_Y, VITESSE_X, VITESSE_Y);
            _infos = new MeteoInfo(METEO_URL);
            //_taille = new SizeF(LARGEUR, HAUTEUR);
        }


        public override void AfficheGDI(Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif

            if (_infos != null)
            {
                float LargeurMax = _taille.Width;
                float HauteurMax = _taille.Height;
                float Y = tailleEcran.Top + 100;
                float dx = (tailleEcran.Height - 200) / (2 + _infos._lignes.Count);
                Color col = getCouleurAvecAlpha(couleur, ALPHA);
                using (Brush b = new SolidBrush(col))
                {
                    using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TITRE, FontStyle.Bold, GraphicsUnit.Pixel))
                        g.DrawString(_infos._location, f, b, X - 10, Y);
                    Y += TAILLE_TITRE;

                    using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_TITRE, FontStyle.Regular, GraphicsUnit.Pixel))
                    {
                        g.DrawString(_infos._title, f, b, X, Y);
                        Y += TAILLE_TEXTE_TITRE * 2 ;
                    }

                    using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_LEVER_COUCHER, FontStyle.Regular, GraphicsUnit.Pixel))
                    {
                        g.DrawString("Lever: " + _infos.lever, f, b, X, Y);
                        Y += TAILLE_TEXTE_LEVER_COUCHER;
                        g.DrawString("Coucher: " + _infos.coucher, f, b, X, Y);
                        Y += TAILLE_TEXTE_LEVER_COUCHER * 2;
                    }


                }

                float falpha = ALPHA;

                using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE, FontStyle.Regular, GraphicsUnit.Pixel))
                using (Font fp = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_PETIT, FontStyle.Regular, GraphicsUnit.Pixel))
                    foreach (MeteoInfo.TInfo info in _infos._lignes)
                    {
                        Color c = getCouleurAvecAlpha(couleur, (byte)falpha);
                        using (Bitmap bmp = BitmapNuance(g, info.bmp, c))
                            g.DrawImage(bmp, X, Y, TAILLE_ICONE, TAILLE_ICONE);

                        string text = info.day + " " + info.TMin + "°/" + info.TMax + "°";
                        float tailleTexte = g.MeasureString(text, f).Width + TAILLE_ICONE;
                        if (tailleTexte > LargeurMax)
                            LargeurMax = tailleTexte;
                        using (Brush b = new SolidBrush(c))
                        {
                            g.DrawString(text, f, b, X + TAILLE_ICONE, Y);
                            g.DrawString(info.text, fp, b, X + TAILLE_ICONE + TAILLE_TEXTE_PETIT, Y + TAILLE_TEXTE);
                        }
                        Y += TAILLE_ICONE * 1.5f;
                        falpha *= DELTA_ALPHA;
                    }

                if (_taille.Width < LargeurMax || _taille.Height < HauteurMax)
                    _taille = new SizeF((float)Math.Max(_taille.Width, LargeurMax), (float)Math.Max(_taille.Height, HauteurMax));
            }

            
            //g.FillRectangle(new SolidBrush(Color.FromArgb(128, 255, 0, 0)), tailleEcran);
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

            //_trajectoire.Avance(tailleEcran, _taille, maintenant);
            if (_droite)
                X = tailleEcran.Width - _taille.Width;
            else
                X = 0;

            if (_droite)
                tailleEcran = new Rectangle((int)tailleEcran.X, (int)tailleEcran.Y, (int)X, (int)tailleEcran.Height);
            else
                tailleEcran = new Rectangle((int)_taille.Width, (int)tailleEcran.Y, (int)(tailleEcran.Width - _taille.Width), (int)tailleEcran.Height);

#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }



        private bool RaffraichirMeteo(Temps maintenant)
        {
            return false;
        }
    }
}