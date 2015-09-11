/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 22/06/2014
 * Time: 17:11
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace ClockScreenSaverGL
{
    /// <summary>
    /// Description of HorlogeRonde.
    /// </summary>
    public sealed class HorlogeRonde : DisplayedObject, IDisposable
    {
        #region Parametres
        public const string CAT = "HorlogeRonde";

        private readonly byte ALPHA = conf.getParametre(CAT, "Alpha", (byte)200);
        private static readonly int HAUTEUR_FONTE = conf.getParametre(CAT, "HauteurFonte1", (byte)42);
        private static readonly int HAUTEUR_FONTE2 = conf.getParametre(CAT, "HauteurFonte2", (byte)20);
        public static readonly byte ALPHA_AIGUILLES = conf.getParametre(CAT, "AlphaAiguilles", (byte)250);
        public static readonly float EPAISSEUR_TROTTEUSE = conf.getParametre(CAT, "EpaisseurTrotteuse", 8.0f);
        public static readonly float EPAISSEUR_MINUTES = conf.getParametre(CAT, "EpaisseurMinutes", 15.0f);
        public static readonly float EPAISSEUR_HEURES = conf.getParametre(CAT, "EpaisseurHeure", 25.0f);
        public static readonly float EPAISSEUR_TROTTEUSE_CONTINUE = conf.getParametre(CAT, "EpaisseurTrotteuseContinue", 2.0f);
        public static readonly float RATIO_TROTTEUSE_CONTINUE = conf.getParametre(CAT, "RatioTrotteuseContinue", 0.15f);

        private static readonly Color COULEUR_AIGUILLES = Color.FromArgb(ALPHA_AIGUILLES, 0, 0, 0);
        private static readonly Color COULEUR_GRADUATIONS = Color.FromArgb(ALPHA_AIGUILLES, 0, 0, 0);
        #endregion

        Trajectoire _trajectoire;
        private int _diametre;
        private float _rayon;
#if USE_GDI_PLUS_FOR_2D
        private Pen _penTrotteuse = new Pen(COULEUR_AIGUILLES, EPAISSEUR_TROTTEUSE);
        private Pen _penMinutes = new Pen(COULEUR_AIGUILLES, EPAISSEUR_MINUTES);
        private Pen _penHeures = new Pen(COULEUR_AIGUILLES, EPAISSEUR_HEURES);
        private Pen _penTrotteuseC = new Pen(COULEUR_AIGUILLES, EPAISSEUR_TROTTEUSE_CONTINUE);
#else
        Texture _textureFondHorloge;
#endif

        private Bitmap _bmpFondHorloge;

        // Optimisation, pour eviter de les passer en parametre
        private float CentreX, CentreY;


        private Lune lune = new Lune();

        public HorlogeRonde(int d, float Px, float Py)
        {
            _trajectoire = new TrajectoireDiagonale(Px, Py, conf.getParametre(CAT, "VX", 35), conf.getParametre(CAT, "VY", -34));
            _diametre = (d + 1) / 2 * 2;
            _rayon = _diametre / 2.0f;
            _taille = new SizeF(_diametre, _diametre);
#if USE_GDI_PLUS_FOR_2D
            _penTrotteuse.EndCap = System.Drawing.Drawing2D.LineCap.Triangle;
            _penTrotteuse.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;

            _penMinutes.EndCap = System.Drawing.Drawing2D.LineCap.Triangle;
            _penMinutes.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;

            _penHeures.EndCap = System.Drawing.Drawing2D.LineCap.Triangle;
            _penHeures.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
#endif
        }

        ~HorlogeRonde()
        {
            Dispose();
        }

        public void Dispose()
        {
            try
            {
                _bmpFondHorloge.Dispose();
#if USE_GDI_PLUS_FOR_2D
                _penTrotteuse.Dispose();
                _penTrotteuseC.Dispose();
                _penMinutes.Dispose();
                _penHeures.Dispose();
#endif
                lune.Dispose();
            }
            finally
            {
            }
        }

#if USE_GDI_PLUS_FOR_2D
        /// <summary>
        /// Date changee: changer l'image du fond de l'horloge qui contient
        /// la date et la phase lunaire
        /// </summary>
        /// <param name="g"></param>
        /// <param name="maintenant"></param>
        public override void DateChangee(Graphics g, Temps maintenant)
        {

            CreerBitmapFond(g);
        }
#else
        /// <summary>
        /// Date changee: changer l'image du fond de l'horloge qui contient
        /// la date et la phase lunaire
        /// </summary>
        /// <param name="g"></param>
        /// <param name="maintenant"></param>
        public override void DateChangee(OpenGL gl, Temps maintenant)
        {

            CreerBitmapFond(gl);
        }
#endif
    
#if USE_GDI_PLUS_FOR_2D
        /// <summary>
        /// Dessine l'horloge une fois pour toutes et la garde en memoire
        /// </summary>
        void CreerBitmapFond(Graphics gr)
        {
            if (_bmpFondHorloge != null)
            {
                _bmpFondHorloge.Dispose();
                _bmpFondHorloge = null;
            }

            _bmpFondHorloge = new Bitmap(_diametre, _diametre, gr);

            using (Graphics g = Graphics.FromImage(_bmpFondHorloge))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                float Centre = _diametre / 2.0f;
                DessineFondHorloge(g, Centre, Centre);
            }
        }
#else
        /// <summary>
        /// Dessine l'horloge une fois pour toutes et la garde en memoire
        /// </summary>
        void CreerBitmapFond(OpenGL gl)
        {
            if (_bmpFondHorloge != null)
            {
                _bmpFondHorloge.Dispose();
                _bmpFondHorloge = null;
            }

            _bmpFondHorloge = new Bitmap(_diametre, _diametre, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(_bmpFondHorloge))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                float Centre = _diametre / 2.0f;
                DessineFondHorloge(g, Centre, Centre, true);
            }

            _textureFondHorloge = new Texture();
            _textureFondHorloge.Create(gl, _bmpFondHorloge);
        }
#endif
        /// <summary>
        /// Dessine une des aiguilles de l'horloge
        /// </summary>
        /// <param name="g"></param>
        /// <param name="CentreX"></param>
        /// <param name="CentreY"></param>
        /// <param name="val"></param>
        /// <param name="max"></param>
        /// <param name="ratioRayon1"></param>
        /// <param name="ratioRayon2"></param>
        /// <param name="pen"></param>
        private void DessineAiguille(Graphics g, float val, float max, float ratioRayon1, float ratioRayon2, Pen pen)
        {
            // Calcul de l'angle en radian
            float Longueur1 = _rayon * ratioRayon1;
            float Longueur2 = _rayon * ratioRayon2;

            double Angle = (((val / max) * (Math.PI * 2.0f)) - (Math.PI / 2.0f));
            float c = (float)Math.Cos(Angle);
            float s = (float)Math.Sin(Angle);

            g.DrawLine(pen, CentreX + (Longueur2 * c), CentreY + (Longueur2 * s), CentreX + (Longueur1 * c), CentreY + (Longueur1 * s));
        }

        /// <summary>
        /// Dessine une des aiguilles de l'horloge
        /// </summary>
        /// <param name="g"></param>
        /// <param name="CentreX"></param>
        /// <param name="CentreY"></param>
        /// <param name="val"></param>
        /// <param name="max"></param>
        /// <param name="ratioRayon1"></param>
        /// <param name="ratioRayon2"></param>
        /// <param name="pen"></param>
        private void DessineAiguille(OpenGL gl, float val, float max, float ratioRayon1, float ratioRayon2, float largeurSurDeux)
        {
            // Calcul de l'angle en radian
            float Longueur1 = _rayon * ratioRayon1;
            float Longueur2 = _rayon * ratioRayon2;

            float Angle = -((val / max) * (360.0f));

            gl.PushMatrix();
            gl.Rotate(0, 0, Angle);
            gl.Color(0, 0, 0, 0.9);
            gl.Enable(OpenGL.GL_MULTISAMPLE);
            gl.Begin(OpenGL.GL_QUADS);

            {
                gl.Color(0, 0, 0, 0.7); gl.Vertex(-largeurSurDeux, Longueur1);
                gl.Color(0, 0, 0, 0.7); gl.Vertex(-largeurSurDeux, Longueur2);
                gl.Color(0, 0, 0, 1.0); gl.Vertex(0, Longueur2 - largeurSurDeux);
                gl.Color(0, 0, 0, 1.0); gl.Vertex(0, Longueur1 + largeurSurDeux);
            }

            {
                gl.Color(0, 0, 0, 0.7); gl.Vertex(largeurSurDeux, Longueur1);
                gl.Color(0, 0, 0, 0.7); gl.Vertex(largeurSurDeux, Longueur2);
                gl.Color(0, 0, 0, 1.0); gl.Vertex(0, Longueur2 - largeurSurDeux);
                gl.Color(0, 0, 0, 1.0); gl.Vertex(0, Longueur1 + largeurSurDeux);
            }
            gl.End();

            gl.PopMatrix();
        }

        /// <summary>
        /// Deplacer l'horloge
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif

            _trajectoire.Avance(tailleEcran, _taille, maintenant);
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif

        }

        /// <summary>
        /// Dessine le jour du mois une fois pour toute dans le fond de l'horloge
        /// </summary>
        /// <param name="g"></param>
        /// <param name="CentreX"></param>
        /// <param name="CentreY"></param>
        /// <param name="Rayon"></param>
        /// <param name="Jour"></param>
        /// <param name="b"></param>
        /// <param name="p"></param>
        public void DessineJourDuMois(Graphics g, float CentreX, float CentreY, float Rayon, int Jour, Brush b, Pen p)
        {
            using (Font fonte = new Font(FontFamily.GenericMonospace, (HAUTEUR_FONTE / 2), FontStyle.Bold, GraphicsUnit.Pixel))
            {
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Near;

                String texte = Jour.ToString("00");
                SizeF s = g.MeasureString(texte, fonte);

                int X = (int)(CentreX + (Rayon / 4.0f));
                int Y = (int)(CentreY - s.Height / 2.0f);
                Rectangle rect = new Rectangle(X, Y, (int)s.Width, (int)s.Height);

                rect.Inflate(4, 4);
                g.DrawRectangle(p, rect);
                rect.Inflate(-2, -2);
                g.DrawRectangle(p, rect);
                rect.Offset(0, 3);

                g.DrawString(texte, fonte, b, rect, stringFormat);
            }
        }

        /// <summary>
        /// Dessine une icone representant la phase lunaire
        /// </summary>
        /// <param name="g"></param>
        /// <param name="CentreX"></param>
        /// <param name="CentreY"></param>
        /// <param name="Rayon"></param>
        /// <param name="maintenant"></param>
        /// <param name="brush"></param>
        public void DessinePhaseLunaire(Graphics g, float CentreX, float CentreY, float Rayon, DateTime maintenant, Brush brush)
        {
            using (Bitmap bmpLune = lune.getImageLune(g, maintenant))
                if (bmpLune != null)
                {
                    float X = CentreX - (Rayon / 4.0f) - bmpLune.Width;
                    float Y = CentreY - bmpLune.Height / 2.0f;
                    g.DrawImage(bmpLune, X, Y);
                }
        }

#if USE_GDI_PLUS_FOR_2D
        /// <summary>
        /// Affiche l'objet
        /// </summary>
        /// <param name="g"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleur"></param>
        public override void AfficheGDI(Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
            if (_bmpFondHorloge == null)
                CreerBitmapFond(g);

#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            CentreX = _trajectoire._Px + _rayon;
            CentreY = _trajectoire._Py + _rayon;

            using (Brush b = new SolidBrush(getCouleurAvecAlpha(couleur, ALPHA)))
            {
                SmoothingMode q = g.SmoothingMode;
                CompositingQuality c = g.CompositingQuality;
                InterpolationMode m = g.InterpolationMode;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                // NB: les infos journalieres sont dessinees dans CreateBitmapFond

                // Fond de l'horloge
                g.FillEllipse(b, CentreX - _rayon, CentreY - _rayon, _diametre, _diametre);
                g.SmoothingMode = q;
                g.CompositingQuality = c;
                g.InterpolationMode = m;

                // Graduations
                g.DrawImage(_bmpFondHorloge, CentreX - _rayon, CentreY - _rayon);

                // Trotteuse
                DessineAiguille(g, maintenant._Seconde, 60.0f, 0.8f, -0.2f, _penTrotteuse);
                //Minutes
                DessineAiguille(g, maintenant._Minute + maintenant._Seconde / 60.0f, 60.0f, 0.7f, 0, _penMinutes);
                // Heure
                DessineAiguille(g, maintenant._Heure + maintenant._Minute / 60.0f, 12.0f, 0.7f, 0, _penHeures);

                // Secondes continues
                CentreY += ((_diametre * RATIO_TROTTEUSE_CONTINUE) * 0.9f);
                DessineAiguille(g, maintenant._Millieme, 1000.0f, 0.12f, -0.02f, _penTrotteuseC);
            }
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

#else
        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif

            if (_textureFondHorloge == null)
                CreerBitmapFond(gl);
            CentreX = _trajectoire._Px + _rayon;
            CentreY = _trajectoire._Py + _rayon;
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

            float[] col = { couleur.R / 512.0f, couleur.G / 512.0f, couleur.B / 512.0f, 1 };
            gl.Color(col);

            _textureFondHorloge.Bind(gl);
            gl.Translate(CentreX, CentreY, 0);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(-_rayon, _rayon);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(-_rayon, -_rayon);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(_rayon, -_rayon);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(_rayon, _rayon);
            gl.End();

            gl.Disable(OpenGL.GL_TEXTURE_2D);
            DessineAiguille(gl, maintenant._Seconde, 60.0f, 0.8f, -0.2f, EPAISSEUR_TROTTEUSE / 2.0f);
            //Minutes
            DessineAiguille(gl, maintenant._Minute + maintenant._Seconde / 60.0f, 60.0f, 0.7f, 0, EPAISSEUR_MINUTES / 2.0f);
            // Heure
            DessineAiguille(gl, maintenant._Heure + maintenant._Minute / 60.0f, 12.0f, 0.7f, 0, EPAISSEUR_HEURES / 2.0f);

            // Secondes continues
            //CentreY += ((_diametre * RATIO_TROTTEUSE_CONTINUE) * 0.9f);
            gl.Translate(0, -((_diametre * RATIO_TROTTEUSE_CONTINUE) * 0.9f), 0);
            DessineAiguille(gl, maintenant._Millieme, 1000.0f, 0.12f, -0.02f, EPAISSEUR_TROTTEUSE_CONTINUE / 2.0f);


            gl.DrawText(10, 10, 0, 1, 0, "Arial.ttf", 172, "texte");
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();

#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }
#endif
        /// <summary>
        /// Dessine le fond de l'horloge (partie fixe)
        /// </summary>
        /// <param name="g"></param>
        /// <param name="CentreX"></param>
        /// <param name="CentreY"></param>
        private void DessineFondHorloge(Graphics g, float CentreX, float CentreY, bool dessingFond = false)
        {
            float _rayon = _diametre / 2.0f;
            float Longueur1 = _diametre * 0.49f;
            float Longueur2 = _diametre * 0.46f;
            float Longueur3 = _diametre * 0.42f;
            float Longueur4 = _diametre * 0.30f;
            float Longueur5 = _diametre * 0.38f;
            float Longueur6 = _diametre * 0.28f;
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;
            if (dessingFond)
                g.FillEllipse(Brushes.White, 0, 0, _diametre, _diametre);

            DateTime maintenant = DateTime.Now;

            DessineJourDuMois(g, CentreX, CentreY, _rayon, maintenant.Day, Brushes.Black, new Pen(Color.Black, EPAISSEUR_TROTTEUSE_CONTINUE));
            DessinePhaseLunaire(g, CentreX, CentreY, _rayon, maintenant, Brushes.White);

            using (Brush bTexte = new SolidBrush(COULEUR_GRADUATIONS))
            using (Pen pinceauNoir = new Pen(COULEUR_GRADUATIONS, 8), pinceauNoir2 = new Pen(COULEUR_GRADUATIONS, 6),
                   p = new Pen(COULEUR_GRADUATIONS))
            using (Font fonte = new Font(FontFamily.GenericSansSerif, HAUTEUR_FONTE, FontStyle.Bold, GraphicsUnit.Pixel),
                   fonte2 = new Font(FontFamily.GenericSansSerif, HAUTEUR_FONTE2, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                pinceauNoir.EndCap = System.Drawing.Drawing2D.LineCap.Triangle;
                pinceauNoir2.EndCap = System.Drawing.Drawing2D.LineCap.Triangle;

                // Tour exterieur de l'horloge: graduations, chiffres des heures, chiffres des minutes
                for (int i = 1; i <= 60; i++)
                {
                    double Angle = (((i / 60.0) * (Math.PI * 2.0)) - (Math.PI / 2.0));
                    float s = (float)Math.Sin(Angle);
                    float c = (float)Math.Cos(Angle);

                    // Traits de graduation
                    if (i % 5 == 0)
                        g.DrawLine(pinceauNoir, CentreX + (Longueur1 * c), CentreY + (Longueur1 * s), CentreX + (Longueur3 * c), CentreY + (Longueur3 * s));
                    else
                        g.DrawLine(pinceauNoir2, CentreX + (Longueur1 * c), CentreY + (Longueur1 * s), CentreX + (Longueur2 * c), CentreY + (Longueur2 * s));

                    // Chiffres
                    if (i % 5 == 0)
                    {
                        g.DrawString((i / 5).ToString(), fonte, bTexte, CentreX + (Longueur4 * c), CentreY + (Longueur4 * s), stringFormat);
                        g.DrawString((i).ToString(), fonte2, bTexte, CentreX + (Longueur5 * c), CentreY + (Longueur5 * s), stringFormat);
                    }
                }

                // Inscription publicitaire
                using (Font f = new Font(FontFamily.GenericSansSerif, (int)(HAUTEUR_FONTE * 0.3), FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    int y = (int)(CentreY - (_rayon * 0.3f));
                    g.DrawString("Lucien Pilloni\nKinésithérapeute\n04 56 00 29 78\nlpilloni.kine@gmail.com", f, bTexte, CentreX, y, stringFormat);
                }

                // Cadran de la troteuse seconde
                float rayon = _diametre * RATIO_TROTTEUSE_CONTINUE;
                Longueur1 = rayon * 0.48f;
                Longueur2 = rayon * 0.46f;
                Longueur3 = rayon * 0.39f;
                CentreY = CentreY + (rayon * 0.9f);

                for (int i = 1; i <= 100; i++)
                {
                    double Angle = (((i / 100.0f) * (Math.PI * 2.0)) - (Math.PI / 2.0));
                    float s = (float)Math.Sin(Angle);
                    float c = (float)Math.Cos(Angle);

                    // Traits de graduation
                    if (i % 10 == 0)
                        g.DrawLine(p, CentreX + (Longueur1 * c), CentreY + (Longueur1 * s), CentreX + (Longueur3 * c), CentreY + (Longueur3 * s));
                    else
                        g.DrawLine(p, CentreX + (Longueur1 * c), CentreY + (Longueur1 * s), CentreX + (Longueur2 * c), CentreY + (Longueur2 * s));
                }

                g.FillEllipse(bTexte, CentreX - rayon * 0.05f, CentreY - rayon * 0.05f, rayon * 0.1f, rayon * 0.1f);
            }
        }

    }
}
