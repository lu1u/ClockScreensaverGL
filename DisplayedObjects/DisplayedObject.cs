/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 22/06/2014
 * Time: 16:54
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Text;
using SharpGL;
using System.Diagnostics;
using ClockScreenSaverGL.Config;

namespace ClockScreenSaverGL.DisplayedObjects
{
    /// <summary>
    /// Classe de base pour tous les objets affiches
    /// </summary>
    public abstract class DisplayedObject
    {

        public const Keys TOUCHE_PROCHAIN_FOND = Keys.F;
        public const Keys TOUCHE_DE_SAISON = Keys.S;
        public const Keys TOUCHE_INVERSER = Keys.I;
        public const Keys TOUCHE_REINIT = Keys.R;
        public const Keys TOUCHE_WIREFRAME = Keys.W;
        public const Keys TOUCHE_CITATION = Keys.C;
        public const Keys TOUCHE_ADDITIVE = Keys.A;
        public const Keys TOUCHE_NEGATIF = Keys.N;
        public const Keys TOUCHE_DEEZER = Keys.D;
        public const Keys TOUCHE_PARTICULES = Keys.P;

        const float PRECISION_RANDOM = 100000.0f;
        static readonly public Random r = new Random();
        protected readonly OpenGL _gl;

        protected SizeF _taille = new SizeF(-1, -1);

        public abstract CategorieConfiguration getConfiguration();


        public DisplayedObject(OpenGL gl)
        {
            _gl = gl;
        }

        public virtual void Dispose()
        {
        }

        public virtual void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur) { }
        public virtual void AppendHelpText(StringBuilder s) { }



        public virtual void Deplace(Temps maintenant, Rectangle tailleEcran) { }

        // Cette fonction sera appelee quand un changement de date sera detecte
        public virtual void DateChangee(OpenGL gl, Temps maintenant) { }
        public virtual void ClearBackGround(OpenGL gl, Color c) { }

        private int _noFrame = 0;



        /// <summary>
        /// Retourne une couleur correspondant a la teinte donnee avec la transparence donnee
        /// </summary>
        /// <param name="color"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color getCouleurAvecAlpha(Color color, byte alpha)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        /***
         * Pour les operations qu'on ne veut pas faire à toutes les frames
         */
        protected bool UneFrameSur(int NbFrames)
        {
            _noFrame++;
            return (_noFrame % NbFrames == 0);
        }

        /// <summary>
        /// Retourne True avec une certaine probabilite
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        protected static bool Probabilite(float f)
        {
            return FloatRandom(0, 1.0f) < f;
        }

        public static Color getCouleurOpaqueAvecAlpha(Color color, byte alpha)
        {
            float a = (float)alpha / 255.0f;

            return Color.FromArgb(255, (byte)((float)color.R * a), (byte)((float)color.G * a), (byte)((float)color.B * a));
        }

        /// <summary>
        /// Pression sur une touche, retourner true si l'objet a traite, false = fin de l'economiseur
        /// </summary>
        /// <param name="f"></param>
        /// <param name="k"></param>
        /// <returns>true si touche utilisee</returns>
        public virtual bool KeyDown(Form f, Keys k)
        {
            return false;
        }


        /// <summary>
        /// Fait varier aleatoire une valeur donnee
        /// </summary>
        /// <param name="v">Valeur a faire changer</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <param name="vitesse">Vitesse</param>
        /// <param name="intervalle">Intervalle depuis la derniere frame</param>
        public static void Varie(ref float v, float min, float max, float vitesse, float intervalle)
        {
            float dev = FloatRandom(-vitesse, vitesse) * intervalle;

            if (((v + dev) >= min) && ((v + dev) <= max))
                v += dev;
        }
        /// <summary>
        /// Retourne une valeur float entre deux bornes
        /// </summary>
        /// <param name="r"></param>
        /// <param name="Min"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        static public float FloatRandom(float Min, float Max)
        {
            if (Min < Max)
                return (float)r.Next((int)(Min * PRECISION_RANDOM), (int)(Max * PRECISION_RANDOM)) / PRECISION_RANDOM;
            else
                if (Min > Max)
                return (float)r.Next((int)(Max * PRECISION_RANDOM), (int)(Min * PRECISION_RANDOM)) / PRECISION_RANDOM;
            else
                return Min;
        }

        static public int SigneRandom()
        {
            if (r.Next(2) > 0)
                return 1;
            else
                return -1;
        }

        static protected Bitmap BitmapNuance(Graphics g, Image bmp, Color couleur)
        {
            Bitmap bp = new Bitmap(bmp.Width, bmp.Height, g);
            using (Graphics gMem = Graphics.FromImage(bp))
            {
                float[][] ptsArray =
                {
                    new float[] {couleur.R/255.0f, 0, 0, 0, 0},
                    new float[] {0, couleur.G/255.0f, 0, 0, 0},
                    new float[] {0, 0, couleur.B/255.0f, 0, 0},
                    new float[] {0, 0, 0, couleur.A/255.0f, 0},
                    new float[] {0, 0, 0, 0, 1}
                };

                ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
                ImageAttributes imgAttribs = new ImageAttributes();
                imgAttribs.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);

                gMem.DrawImage(bmp,
                               new Rectangle(0, 0, bmp.Width, bmp.Height),
                               0, 0, bmp.Width, bmp.Height,
                               GraphicsUnit.Pixel, imgAttribs);
            }

            return bp;
        }

        /// <summary>
        /// Retourne la copie de la bitmap, version niveaux de gris
        /// </summary>
        /// <param name="g"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        static public Bitmap BitmapNiveauDeGris(Bitmap source, float ratioR = 1.0f)
        {
            Bitmap destination = new Bitmap(source.Width, source.Height);

            for (int i = 0; i < source.Width; i++)
            {
                for (int x = 0; x < source.Height; x++)
                {
                    Color oc = source.GetPixel(i, x);
                    int grayScale = (int)((oc.R * 0.3f * ratioR) + (oc.G * 0.59f) + (oc.B * 0.11f)) % 255;
                    //int grayScale = (int)((oc.R * 0.33) + (oc.G * 0.33) + (oc.B * 0.33));
                    destination.SetPixel(i, x, Color.FromArgb(oc.A, grayScale, grayScale, grayScale));
                }
            }

            return destination;
        }

        /// <summary>
        /// Retourne la copie de la bitmap, version desaturee
        /// </summary>
        /// <param name="g"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        static public Bitmap BitmapDesaturee(Image source, float saturation)
        {
            Bitmap destination = new Bitmap(source.Width, source.Height);

            float rWeight = 0.3086f;
            float gWeight = 0.6094f;
            float bWeight = 0.0820f;

            float a = (1.0f - saturation) * rWeight + saturation;
            float b = (1.0f - saturation) * rWeight;
            float c = (1.0f - saturation) * rWeight;
            float d = (1.0f - saturation) * gWeight;
            float e = (1.0f - saturation) * gWeight + saturation;
            float f = (1.0f - saturation) * gWeight;
            float g = (1.0f - saturation) * bWeight;
            float h = (1.0f - saturation) * bWeight;
            float i = (1.0f - saturation) * bWeight + saturation;

            // Create a Graphics
            using (Graphics gr = Graphics.FromImage(destination))
            {
                {
                    // ColorMatrix elements
                    float[][] ptsArray = {
                                     new float[] {a,  b,  c,  0, 0},
                                     new float[] {d,  e,  f,  0, 0},
                                     new float[] {g,  h,  i,  0, 0},
                                     new float[] {0,  0,  0,  1, 0},
                                     new float[] {0, 0, 0, 0, 1}
                                 };
                    // Create ColorMatrix
                    ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
                    // Create ImageAttributes
                    ImageAttributes imgAttribs = new ImageAttributes();
                    // Set color matrix
                    imgAttribs.SetColorMatrix(clrMatrix,
                        ColorMatrixFlag.Default,
                        ColorAdjustType.Default);
                    // Draw Image with no effects
                    gr.DrawImage(source, 0, 0);
                    // Draw Image with image attributes
                    gr.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height),
                        0, 0, source.Width, source.Height,
                        GraphicsUnit.Pixel, imgAttribs);
                }
            }

            return destination;
        }
        /// <summary>
        /// Affiche une bitmap monochrome en lui faisant prendre une couleur donnee
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bmp"></param>
        /// <param name="couleur"></param>
        /// <returns></returns>
        static protected void DrawBitmapNuance(Graphics g, Image bmp, int x, int y, int l, int h, Color couleur)
        {

            float[][] ptsArray =
                {
                    new float[] {couleur.R/255.0f, 0, 0, 0, 0},
                    new float[] {0, couleur.G/255.0f, 0, 0, 0},
                    new float[] {0, 0, couleur.B/255.0f, 0, 0},
                    new float[] {0, 0, 0, couleur.A/255.0f, 0},
                    new float[] {0, 0, 0, 0, 1}
                };

            ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
            ImageAttributes imgAttribs = new ImageAttributes();
            imgAttribs.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);

            g.DrawImage(bmp, new Rectangle(x, y, l, h),
                           0, 0, bmp.Width, bmp.Height,
                          GraphicsUnit.Pixel, imgAttribs);
        }

        static protected void DrawBitmapNuance(Graphics g, Bitmap bmp, float x, float y, float l, float h, Color couleur)
        {
            float[][] ptsArray =
                {
                    new float[] {couleur.R/255.0f, 0, 0, 0, 0},
                    new float[] {0, couleur.G/255.0f, 0, 0, 0},
                    new float[] {0, 0, couleur.B/255.0f, 0, 0},
                    new float[] {0, 0, 0, couleur.A/255.0f, 0},
                    new float[] {0, 0, 0, 0, 1}
                };

            ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
            ImageAttributes imgAttribs = new ImageAttributes();
            imgAttribs.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);

            PointF[] ppt = { new PointF(x, y), new PointF(x + l, y), new PointF(x, y + h) };
            g.DrawImage(bmp, ppt, new RectangleF(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel, imgAttribs);

        }
        /// <summary>
        /// Create an empty texture.
        /// </summary>
        /// <returns></returns>
        protected uint createEmptyTexture(int LARGEUR_TEXTURE, int HAUTEUR_TEXTURE)
        {
            uint[] txtnumber = new uint[1];                     // Texture ID

            // Create Storage Space For Texture Data (128x128x4)
            byte[] data = new byte[((LARGEUR_TEXTURE * HAUTEUR_TEXTURE) * 4 * sizeof(uint))];

            _gl.GenTextures(1, txtnumber);					// Create 1 Texture
            _gl.BindTexture(OpenGL.GL_TEXTURE_2D, txtnumber[0]);			// Bind The Texture
            _gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, 4, LARGEUR_TEXTURE, LARGEUR_TEXTURE, 0, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, data);			// Build Texture Using Information In data
            _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);

            return txtnumber[0];						// Return The Texture ID
        }

        ///////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DEstruction d'une texture OpenGL cree par createEmptyTexture
        /// </summary>
        /// <param name="texture"></param>
        protected void deleteEmptyTexture(uint texture)
        {
            uint[] textures = { texture };
            _gl.DeleteTextures(1, textures);
        }


        protected void DessineCube(OpenGL gl, float MAX_X, float MAX_Y, float MAX_Z)
        {
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(-MAX_X, +MAX_Y, -MAX_Z); gl.Vertex(+MAX_X, +MAX_Y, -MAX_Z);
            gl.Vertex(+MAX_X, +MAX_Y, -MAX_Z); gl.Vertex(+MAX_X, -MAX_Y, -MAX_Z);
            gl.Vertex(+MAX_X, -MAX_Y, -MAX_Z); gl.Vertex(-MAX_X, -MAX_Y, -MAX_Z);
            gl.Vertex(-MAX_X, -MAX_Y, -MAX_Z); gl.Vertex(-MAX_X, +MAX_Y, -MAX_Z);

            gl.Vertex(-MAX_X, +MAX_Y, +MAX_Z); gl.Vertex(+MAX_X, +MAX_Y, +MAX_Z);
            gl.Vertex(+MAX_X, +MAX_Y, +MAX_Z); gl.Vertex(+MAX_X, -MAX_Y, +MAX_Z);
            gl.Vertex(+MAX_X, -MAX_Y, +MAX_Z); gl.Vertex(-MAX_X, -MAX_Y, +MAX_Z);

            gl.Vertex(-MAX_X, +MAX_Y, -MAX_Z); gl.Vertex(-MAX_X, +MAX_Y, +MAX_Z);
            gl.Vertex(+MAX_X, +MAX_Y, -MAX_Z); gl.Vertex(+MAX_X, +MAX_Y, +MAX_Z);
            gl.Vertex(+MAX_X, -MAX_Y, -MAX_Z); gl.Vertex(+MAX_X, -MAX_Y, +MAX_Z);
            gl.Vertex(-MAX_X, -MAX_Y, -MAX_Z); gl.Vertex(-MAX_X, -MAX_Y, +MAX_Z);
            gl.End();
        }

        #region Chrono
#if TRACER
        long moyennedureeR = 0;
            long moyennedureeD = 0;
        protected enum CHRONO_TYPE { RENDER, DEPLACE };

        private Stopwatch chronoRender = new Stopwatch();
        private Stopwatch chronoDeplace = new Stopwatch();

        /// <summary>
        /// Demarrage du trace
        /// </summary>
        protected void RenderStart(CHRONO_TYPE t)
        {
            switch (t)
            {
                case CHRONO_TYPE.RENDER: chronoRender.Restart(); break;
                case CHRONO_TYPE.DEPLACE: chronoDeplace.Restart(); break;
            }

        }

        /// <summary>
        /// Arret du trace
        /// </summary>
        protected void RenderStop(CHRONO_TYPE t)
        {
            switch (t)
            {
                case CHRONO_TYPE.RENDER: chronoRender.Stop(); break;
                case CHRONO_TYPE.DEPLACE: chronoDeplace.Stop(); break;
            }

        }

        public virtual String DumpRender()
        {
            moyennedureeR = ((moyennedureeR * 10) + chronoRender.ElapsedTicks) / 11;
            moyennedureeD = ((moyennedureeD * 10) + chronoDeplace.ElapsedTicks) / 11;
            return ((moyennedureeR / 1000.0).ToString("Render:  000.0") + " " +
                    (moyennedureeD / 1000.0).ToString("Deplace:  000.0") + " " + this.GetType().Name);
        }

#endif
        #endregion
    }
}
