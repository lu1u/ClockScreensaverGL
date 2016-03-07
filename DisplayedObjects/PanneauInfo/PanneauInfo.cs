using ClockScreenSaverGL.DisplayedObjects.Textes;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace ClockScreenSaverGL.DisplayedObjects.Meteo
{
    class PanneauInfos : DisplayedObject, IDisposable
    {
        #region PARAMETRES
        public const string CAT = "PanneauInfos";
        private static readonly float ORIGINE_X = conf.getParametre(CAT, "OrigineX", 1000.0f);
        private static readonly float ORIGINE_Y = conf.getParametre(CAT, "OrigineY", 500.0f);
        private static readonly float VITESSE_X = conf.getParametre(CAT, "VitesseX", 30.0f);
        private static readonly float VITESSE_Y = conf.getParametre(CAT, "VitesseY", -21.0f);
        private static readonly byte ALPHA = (byte)conf.getParametre(CAT, "Alpha", 200);
        private static readonly float DELTA_ALPHA = conf.getParametre(CAT, "DeltaAlpha", 0.7f);
        private static readonly string METEO_URL = conf.getParametre(CAT, "URL", @"http://weather.yahooapis.com/forecastrss?w=588014&u=c");
        private static readonly string EXE_DEEZERINFO = conf.getParametre(CAT, "DeezerInfo", getDeezerInfoDirectory());
        public static readonly bool EXE_KILL = conf.getParametre(CAT, "Tuer deezer info", false);
        private static readonly int DIAMETRE_HORLOGE = conf.getParametre(CAT, "Diametre Horloge", 360);
        private static readonly int MARGE_HORLOGE = 20;
        private static readonly int TAILLE_TEXTE_HEURE = 32;
        private static readonly int MARGE_TEXTE_HEURE = 20;
        private static readonly int TAILLE_TEXTE_DATE = 30;
        //private static readonly int MARGE_TEXTE_DATE = 20;


        private static string getDeezerInfoDirectory()
        {
            string myExeDir = (new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location)).FullName;
            FileInfo p = new FileInfo(myExeDir);
            return Path.Combine(p.DirectoryName, "GetDeezerInformation.exe");

        }

        private static readonly int DELAI_DEEZER = conf.getParametre(CAT, "DeezerInfo Delai (secondes)", 5);
        private static readonly int TAILLE_TITRE = conf.getParametre(CAT, "Taille Titre", 48);
        private static readonly int TAILLE_ICONE = conf.getParametre(CAT, "Taille Icone", 56);
        private static readonly int TAILLE_TEXTE = conf.getParametre(CAT, "Taille Texte", 32);
        private static readonly int TAILLE_TEXTE_PETIT = conf.getParametre(CAT, "Taille Texte Petit", 12);
        private static readonly int TAILLE_TEXTE_TITRE = conf.getParametre(CAT, "Taille Texte Titre", 12);
        private static readonly float RATIO_INTERLIGNE = 0.99f; // conf.getParametre(CAT, "RatioInterligne", 1.2f);
        private static readonly int MARGE_H = conf.getParametre(CAT, "MargeH", 12);
        private static readonly int MARGE_V = conf.getParametre(CAT, "MargeV", 12);
        private static readonly int LARGEUR_JAUGE = conf.getParametre(CAT, "LargeurJauge", 8);
        private static readonly int TAILLE_TEXTE_LEVER_COUCHER = conf.getParametre(CAT, "Taille Texte Lever", 24);

        #endregion PARAMETRES ;

        private MeteoInfo _infos;
        private DeezerInfo _deezer;
        private HorlogeRonde _horloge;
        private HeureTexte _heureTexte;
        private DateTexte _dateTexte;

        private bool _droite;
        private float _X = 0;
        private float _Y = 0;
        private Bitmap _bitmap;
        private Texture _texture = new Texture();
        /// <summary>
        /// Constructeur
        /// </summary>
        public PanneauInfos(OpenGL gl, bool droite) : base(gl)
        {
            _droite = droite;
            _taille = new SizeF(DIAMETRE_HORLOGE + MARGE_HORLOGE * 2, SystemInformation.VirtualScreen.Height);
            _infos = new MeteoInfo(METEO_URL);
            _deezer = new DeezerInfo(EXE_DEEZERINFO, DELAI_DEEZER);
            _horloge = new HorlogeRonde(gl, _droite, DIAMETRE_HORLOGE, 0, 0);
            _heureTexte = new HeureTexte(gl, 0, 0, TAILLE_TEXTE_HEURE);
            _dateTexte = new DateTexte(gl, 0, 0, TAILLE_TEXTE_DATE);
            CreerBitmap(gl);
        }

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            if (_bitmap == null)
                CreerBitmap(gl);
            if (_bitmap != null)
            {
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

                gl.Translate(0, _taille.Height, 0);
                // Jauge de duree de validite des previsions
                float actuelle = _infos.validitePassee();
                col[3] = 0.7f;
                gl.Color(col);
                gl.Disable(OpenGL.GL_TEXTURE_2D);
                gl.Begin(OpenGL.GL_QUADS);
                {
                    gl.Vertex(0, LARGEUR_JAUGE);
                    gl.Vertex(0, 0);
                    gl.Vertex(_taille.Width * actuelle, 0);
                    gl.Vertex(_taille.Width * actuelle, LARGEUR_JAUGE);
                }
                gl.End();

                col[3] = 0.4f;
                gl.Color(col);
                gl.Begin(OpenGL.GL_QUADS);
                {
                    gl.Vertex(_taille.Width * actuelle, LARGEUR_JAUGE);
                    gl.Vertex(_taille.Width * actuelle, 0);
                    gl.Vertex(_taille.Width, 0);
                    gl.Vertex(_taille.Width, LARGEUR_JAUGE);
                }
                gl.End();

                gl.MatrixMode(OpenGL.GL_PROJECTION);
                gl.PopMatrix();
                gl.MatrixMode(OpenGL.GL_MODELVIEW);
                gl.PopMatrix();
                gl.LoadIdentity();
            }
            _heureTexte.AfficheOpenGL(gl, maintenant, tailleEcran, couleur);
            _dateTexte.AfficheOpenGL(gl, maintenant, tailleEcran, couleur);
            _horloge.AfficheOpenGL(gl, maintenant, tailleEcran, couleur);
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

            if (_infos?.donneesPretes == true)
            {
                CalculeTaille();
                if (_taille.Width < 1)
                    return; // Pas de bitmap
            }

            using (Font fonteTitre = new Font(FontFamily.GenericSansSerif, TAILLE_TITRE, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Font fonteSousTitre = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_PETIT, FontStyle.Regular, GraphicsUnit.Pixel))
            using (Font fLever = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_LEVER_COUCHER, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                _bitmap = new Bitmap((int)Math.Ceiling(_taille.Width), (int)Math.Ceiling(_taille.Height), PixelFormat.Format32bppArgb);
                float Y = MARGE_V + DIAMETRE_HORLOGE + MARGE_HORLOGE * 2 + TAILLE_TEXTE_HEURE + MARGE_TEXTE_HEURE + TAILLE_TEXTE_DATE;
                using (Graphics g = Graphics.FromImage(_bitmap))
                {
                    g.Clear(Color.FromArgb(96, 64, 64, 64));
                    float LargeurMax = _taille.Width;

                    if (_infos?.donneesPretes == true)
                    // Titre
                    {
                        g.DrawString(_infos._location, fonteTitre, Brushes.White, MARGE_H, Y);
                        Y += TAILLE_TITRE;

                        using (Font fLocation = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_TITRE, FontStyle.Regular, GraphicsUnit.Pixel))
                        {
                            g.DrawString(_infos._title, fLocation, Brushes.White, MARGE_H, Y);
                            Y += TAILLE_TEXTE_TITRE * RATIO_INTERLIGNE;
                        }

                        {
                            g.DrawString(String.Format(Resources.Lever, _infos.lever), fLever, Brushes.White, MARGE_H, Y);
                            Y += TAILLE_TEXTE_LEVER_COUCHER;
                            g.DrawString(String.Format(Resources.Coucher, _infos.coucher), fLever, Brushes.White, MARGE_H, Y);
                            Y += TAILLE_TEXTE_LEVER_COUCHER * RATIO_INTERLIGNE;
                        }


                        // Lignes de previsions
                        using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE, FontStyle.Regular, GraphicsUnit.Pixel))
                        {
                            foreach (LignePrevisionMeteo ligne in _infos._lignes)
                            {
                                g.DrawImage(ligne.bmp, MARGE_H, Y, TAILLE_ICONE, TAILLE_ICONE);
                                string text = String.Format(Resources.Temperatures, ligne.day, ligne.TMin, ligne.TMax);
                                float tailleTexte = g.MeasureString(text, f).Width + TAILLE_ICONE;
                                g.DrawString(text, f, Brushes.White, MARGE_H + TAILLE_ICONE, Y);
                                g.DrawString(ligne.text, fonteSousTitre, Brushes.White, MARGE_H + TAILLE_ICONE + TAILLE_TEXTE_PETIT, Y + TAILLE_TEXTE);

                                Y += TAILLE_ICONE * RATIO_INTERLIGNE;
                            }
                        }



                        // Chanson en cours de lecture dans deezer
                        // Taille de l'information sur la chanson en cours de lecture
                        if (_deezer != null)
                        {
                            using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE, FontStyle.Regular, GraphicsUnit.Pixel))
                            {
                                using (Pen p = new Pen(Color.White, 4))
                                    g.DrawLine(p, MARGE_H, Y, LargeurMax - MARGE_H, Y);

                                Y += TAILLE_TITRE / 2;
                                g.DrawImage(Resources.music_note, MARGE_H, Y, TAILLE_ICONE, TAILLE_ICONE);

                                g.DrawString(_deezer.Infos, fLever, Brushes.White, MARGE_H + TAILLE_ICONE, Y);
                                Y += g.MeasureString(_deezer.Infos, f).Height * RATIO_INTERLIGNE;
                            }
                        }
                    }
                    float HauteurMax = Y - _Y;
                    _texture.Create(gl, _bitmap);
                }
            }
        }


        /// <summary>
        /// Calcule la taille de l'image necessaire pour afficher les informations
        /// </summary>
        private void CalculeTaille()
        {
            float largeur = 0;
            float hauteur = 0;

            if (_infos != null)
                if (_infos.donneesPretes)
                {
                    hauteur = SystemInformation.VirtualScreen.Height;
                    /*hauteur = TAILLE_TITRE + TAILLE_TEXTE_TITRE * RATIO_INTERLIGNE + TAILLE_TEXTE_LEVER_COUCHER * RATIO_INTERLIGNE
                         + (TAILLE_ICONE * RATIO_INTERLIGNE) * _infos._lignes.Count
                         + MARGE_V * 2;
                         */
                    using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                    using (Font f = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE, FontStyle.Regular, GraphicsUnit.Pixel))
                    using (Font fLever = new Font(FontFamily.GenericSansSerif, TAILLE_TEXTE_LEVER_COUCHER, FontStyle.Regular, GraphicsUnit.Pixel))
                    {
                        // Taille des previsions meteo
                        foreach (LignePrevisionMeteo ligne in _infos._lignes)
                        {
                            string text = String.Format(Resources.Temperatures, ligne.day, ligne.TMin, ligne.TMax);
                            float tailleTexte = g.MeasureString(text, f).Width + TAILLE_ICONE;
                            if (tailleTexte > largeur)
                                largeur = tailleTexte;
                        }

                        // Taille de l'information sur la chanson en cours de lecture
                        /*if (_deezer != null)
                            hauteur += g.MeasureString(_deezer.Infos, fLever, new SizeF(largeur, hauteur)).Height * RATIO_INTERLIGNE + TAILLE_TITRE;*/
                    }
                    largeur += MARGE_H * 2;
                    largeur = Math.Max(largeur, DIAMETRE_HORLOGE + MARGE_HORLOGE * 2);
                }
            _taille = new SizeF(largeur, hauteur);
        }

        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            if (_infos.MustRefresh(maintenant))
                _infos = new MeteoInfo(METEO_URL);

            if (_deezer.MustRefresh(maintenant))
                _deezer.Refresh();

            if (_infos.HasNewInfo() || _deezer.HasNewInfo())
                _bitmap = null;


            if (_droite)
                _X = tailleEcran.Width - _taille.Width;
            else
                _X = 0;

            _Y = tailleEcran.Top;

            /*if (_droite)
                tailleEcran = new Rectangle((int)tailleEcran.X, (int)tailleEcran.Y, (int)_X, (int)tailleEcran.Height);
            else
                tailleEcran = new Rectangle((int)_taille.Width, (int)tailleEcran.Y, (int)(tailleEcran.Width - _taille.Width), (int)tailleEcran.Height);
                */
            _horloge._pX = _X + (_taille.Width - DIAMETRE_HORLOGE) / 2;
            _horloge._pY = tailleEcran.Bottom - DIAMETRE_HORLOGE - MARGE_HORLOGE;
            _heureTexte._trajectoire._Px = _X + MARGE_TEXTE_HEURE;
            _heureTexte._trajectoire._Py = _horloge._pY - MARGE_HORLOGE - TAILLE_TEXTE_HEURE - MARGE_TEXTE_HEURE;
            _dateTexte._trajectoire._Px = _X + MARGE_TEXTE_HEURE;
            _dateTexte._trajectoire._Py = _heureTexte._trajectoire._Py - TAILLE_TEXTE_HEURE;

#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }

        public override void Dispose()
        {
            _bitmap.Dispose();
            _deezer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

