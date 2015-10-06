using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ClockScreenSaverGL.DisplayedObject.Meteo
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
        private static readonly int MARGE_H = conf.getParametre(CAT, "MargeH", 12);
        private static readonly int MARGE_V = conf.getParametre(CAT, "MargeV", 12);
        private static readonly int LARGEUR_JAUGE = conf.getParametre(CAT, "LargeurJauge", 8);
        private static readonly int TAILLE_TEXTE_LEVER_COUCHER = conf.getParametre(CAT, "Taille Texte Lever", 24);
        #endregion PARAMETRES ;

        private MeteoInfo _infos;
        private bool _droite;
        private float _X = 0;
        private float _Y = 0;
        private Bitmap _bitmap;
        private Texture _texture = new Texture();
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
        /*
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
                    using (Brush nrushTitre = new SolidBrush(col))
                    using (Font fonteSousTitre = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_PETIT, FontStyle.Regular, GraphicsUnit.Pixel))
                        if (_infos != null && _infos.donneesPretes)
                        {
                            float LargeurMax = _taille.Width;

                            // Titre
                            {
                                g.DrawString(_infos._location, fonteTitre, Brushes.White, _X - 10, Y);
                                Y += TAILLE_TITRE;

                                using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_TITRE, FontStyle.Regular, GraphicsUnit.Pixel))
                                {
                                    g.DrawString(_infos._title, f, Brushes.White, _X, Y);
                                    Y += TAILLE_TEXTE_TITRE * 2;
                                }

                                using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_LEVER_COUCHER, FontStyle.Regular, GraphicsUnit.Pixel))
                                {
                                    g.DrawString(String.Format(Resources.Lever, _infos.lever), f, Brushes.White, _X, Y);
                                    Y += TAILLE_TEXTE_LEVER_COUCHER;
                                    g.DrawString(String.Format(Resources.Coucher, _infos.coucher), f, Brushes.White, _X, Y);
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
                            g.DrawString(t, fonteTitre, Brushes.White, _X, Y);
                        }
        #if TRACER
                    RenderStop(CHRONO_TYPE.RENDER);
        #endif
                }
                */

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            if (_bitmap == null)
                CreerBitmap(gl);

            if (_bitmap == null)
                return;

            gl.Clear(OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D(0.0, tailleEcran.Width, 0.0, tailleEcran.Height);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1 };
            gl.Color(col);

            float Y = (tailleEcran.Height - _taille.Height) / 2.0f;
            _texture.Bind(gl);
            gl.Translate(_X, Y, 0);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(0, _taille.Height);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(0, 0);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(_taille.Width, 0);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(_taille.Width, _taille.Height);
            gl.End();

            gl.Translate(0, _taille.Height,0);
            // Jauge de duree de validite des previsions
            float actuelle = _infos.validitePassee();
            col[3] = 0.7f;
            gl.Color(col);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(0, LARGEUR_JAUGE);
            gl.Vertex(0, 0);
            gl.Vertex(_taille.Width * actuelle, 0);
            gl.Vertex(_taille.Width * actuelle, LARGEUR_JAUGE);
            gl.End();
            
            col[3] = 0.4f;
            gl.Color(col);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(_taille.Width * actuelle, LARGEUR_JAUGE);
            gl.Vertex(_taille.Width * actuelle, 0);
            gl.Vertex(_taille.Width , 0);
            gl.Vertex(_taille.Width , LARGEUR_JAUGE);
            gl.End();

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();

#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif

        }

        /// <summary>
        /// Creer une fois pour toutes la bitmap qui sera affichee
        /// </summary>
        /// <param name="gl"></param>
        private void CreerBitmap(OpenGL gl)
        {
            _bitmap?.Dispose();
            _bitmap = null;

            using (Font fonteTitre = new Font(FontFamily.GenericSansSerif, TAILLE_TITRE, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Font fonteSousTitre = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_PETIT, FontStyle.Regular, GraphicsUnit.Pixel))
                if (_infos != null && _infos.donneesPretes)
                {
                    CalculeTaille();
                    if (_taille.Width < 1)
                        return; // Pas de bitmap
                    _bitmap = new Bitmap((int)Math.Ceiling(_taille.Width), (int)Math.Ceiling(_taille.Height), PixelFormat.Format32bppArgb);
                    float Y = MARGE_V;
                    using (Graphics g = Graphics.FromImage(_bitmap))
                    {
                        g.Clear(Color.FromArgb(96, 64, 64, 64));
                        float LargeurMax = _taille.Width;

                        // Titre
                        {
                            g.DrawString(_infos._location, fonteTitre, Brushes.White, MARGE_H, Y);
                            Y += TAILLE_TITRE;

                            using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_TITRE, FontStyle.Regular, GraphicsUnit.Pixel))
                            {
                                g.DrawString(_infos._title, f, Brushes.White, MARGE_H, Y);
                                Y += TAILLE_TEXTE_TITRE * 2;
                            }

                            using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_LEVER_COUCHER, FontStyle.Regular, GraphicsUnit.Pixel))
                            {
                                g.DrawString(String.Format(Resources.Lever, _infos.lever), f, Brushes.White, MARGE_H, Y);
                                Y += TAILLE_TEXTE_LEVER_COUCHER;
                                g.DrawString(String.Format(Resources.Coucher, _infos.coucher), f, Brushes.White, MARGE_H, Y);
                                Y += TAILLE_TEXTE_LEVER_COUCHER * 2;
                            }
                        }

                        // Lignes de previsions
                        using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE, FontStyle.Regular, GraphicsUnit.Pixel))
                            foreach (LignePrevisionMeteo ligne in _infos._lignes)
                            {
                                g.DrawImage(ligne.bmp, MARGE_H, Y, TAILLE_ICONE, TAILLE_ICONE);
                                string text = String.Format(Resources.Temperatures, ligne.day, ligne.TMin, ligne.TMax);
                                float tailleTexte = g.MeasureString(text, f).Width + TAILLE_ICONE;
                                g.DrawString(text, f, Brushes.White, MARGE_H + TAILLE_ICONE, Y);
                                g.DrawString(ligne.text, fonteSousTitre, Brushes.White, MARGE_H + TAILLE_ICONE + TAILLE_TEXTE_PETIT, Y + TAILLE_TEXTE);

                                Y += TAILLE_ICONE * 1.5f;
                            }

                        float HauteurMax = Y - _Y;
                        _texture.Create(gl, _bitmap);
                    }
                }
        }


        private void CalculeTaille()
        {
            float largeur = 0;
            float hauteur = 0;

            if (_infos != null)
                if (_infos.donneesPretes)
                {
                    hauteur = TAILLE_TITRE + TAILLE_TEXTE_TITRE * 2 + TAILLE_TEXTE_LEVER_COUCHER * 2
                         + (TAILLE_ICONE * 1.5f) * _infos._lignes.Count
                         + MARGE_V * 2;

                    using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                    using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE, FontStyle.Regular, GraphicsUnit.Pixel))
                        foreach (LignePrevisionMeteo ligne in _infos._lignes)
                        {
                            string text = String.Format(Resources.Temperatures, ligne.day, ligne.TMin, ligne.TMax);
                            float tailleTexte = g.MeasureString(text, f).Width + TAILLE_ICONE;
                            if (tailleTexte > largeur)
                                largeur = tailleTexte;
                        }

                    largeur += MARGE_H * 2;
                }
            _taille = new SizeF(largeur, hauteur);
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

