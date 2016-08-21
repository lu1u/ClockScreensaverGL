using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds
{
    class NuagesPerlin : Fond
    {
        const String CAT = "NuagesPerlin";
        readonly int LARGEUR = conf.getParametre( CAT, "Largeur perlin", 512);
        readonly int HAUTEUR = conf.getParametre(CAT, "Hauteur perlin", 512);
        readonly int LARGEUR_QUAD = conf.getParametre(CAT, "Largeur quad", 25);
        readonly int PROFONDEUR_QUAD = conf.getParametre(CAT, "Profondeur quad", 30);
        readonly int YNUAGE = conf.getParametre(CAT, "Y nuage", 10);
        readonly float DISTANCE_ENTRE_COUCHES = 0.02f;// conf.getParametre(CAT, "Distance entre couches", 0.04f);
        int SEUIL= conf.getParametre(CAT, "Seuil alpha", 10 );
        int OCTAVES = conf.getParametre(CAT, "Octaves", 6);
        int NB_COUCHES = conf.getParametre(CAT, "Nombre de couches", 20);
        float FREQUENCE = conf.getParametre(CAT, "Frequence", 200.0f);
        float AMPLITUDE = conf.getParametre(CAT, "Amplitude", 0.03f );
        bool ADDITIVE = conf.getParametre(CAT, "Additif", true) ;
        Bitmap bmp;
        Texture texture;
        PerlinNoise perlin = new PerlinNoise(r.Next());
        int yScroll;
        Texture _textureSoleil;

        /// <summary>
        /// Constructeur, initialiser les textures
        /// </summary>
        /// <param name="gl"></param>
        public NuagesPerlin(OpenGL gl) : base(gl)
        {
            createPerlinNoiseBitmap();
            _textureSoleil = new Texture();
            _textureSoleil.Create(gl, Resources.soleil);
            yScroll = 0;
            yScroll = HAUTEUR - 1;
        }

        /// <summary>
        /// Effacer le fond de l'ecran
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="couleur"></param>
        public override void ClearBackGround(OpenGL gl, Color couleur)
        {
            float[] fogcol = { couleur.R / 512.0f, couleur.G/ 512.0f, couleur.B / 512.0f, 1 };

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);      // Clear The Screen And Depth Buffer
            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_COLOR_MATERIAL);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Disable(OpenGL.GL_DEPTH_TEST);
            gl.DepthMask((byte)OpenGL.GL_FALSE);

            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D(-1.0, 1.0, -1.0, 1.0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Begin(OpenGL.GL_QUADS);
            {
                gl.Color(fogcol);
                gl.Vertex(-1f, -1f);
                gl.Vertex(1f, -1f);
                gl.Color(0, 0, 0);
                gl.Vertex(1f, 1f);
                gl.Vertex(-1f, 1f);
            }
            gl.End();

            float LARGEUR_SOLEIL = 0.4f;
            float HAUTEUR_SOLEIL = 0.6f;
            float _xSoleil = -0.6f;
            float _ySoleil = -0.4f;

            gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            _textureSoleil.Bind(gl);
            gl.Begin(OpenGL.GL_QUADS);
            {
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(_xSoleil - LARGEUR_SOLEIL / 2, _ySoleil - HAUTEUR_SOLEIL / 2);
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(_xSoleil - LARGEUR_SOLEIL / 2, _ySoleil + HAUTEUR_SOLEIL / 2);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(_xSoleil + LARGEUR_SOLEIL / 2, _ySoleil + HAUTEUR_SOLEIL / 2);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(_xSoleil + LARGEUR_SOLEIL / 2, _ySoleil - HAUTEUR_SOLEIL / 2);
            }
            gl.End();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();
        }

        /// <summary>
        /// Affichage OPENGL
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleur"></param>
        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Disable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            gl.LookAt(0, 0, 1.55f, 0, 0.9f, 0.7f, 0, 1, 0);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, ADDITIVE ? OpenGL.GL_ONE : OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            texture.Bind(gl);

            gl.Translate(0, -DISTANCE_ENTRE_COUCHES * NB_COUCHES, 0);
            gl.Begin(OpenGL.GL_QUADS);
            for (int i = 0; i < NB_COUCHES; i++)
            {
                gl.Color(1f,1.0,1f, 1.0f - ((float)i / (float)NB_COUCHES*0.5f));
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(-LARGEUR_QUAD, YNUAGE + (i * DISTANCE_ENTRE_COUCHES), 0);  // Point 1 (Front)
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(LARGEUR_QUAD, YNUAGE + (i * DISTANCE_ENTRE_COUCHES), 0);  // Point 2 (Front)
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(LARGEUR_QUAD, YNUAGE + (i * DISTANCE_ENTRE_COUCHES), -PROFONDEUR_QUAD);  // Point 3 (Front)
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(-LARGEUR_QUAD, YNUAGE + (i * DISTANCE_ENTRE_COUCHES), -PROFONDEUR_QUAD);  // Point 4 (Front)
                /*

                gl.TexCoord(0.0f, 0.0f); gl.Vertex(-LARGEUR_QUAD, YNUAGE - (i * DISTANCE_ENTRE_COUCHES), 0);  // Point 1 (Front)
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(LARGEUR_QUAD, YNUAGE - (i * DISTANCE_ENTRE_COUCHES), 0);  // Point 2 (Front)
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(LARGEUR_QUAD, YNUAGE - (i * DISTANCE_ENTRE_COUCHES), -PROFONDEUR_QUAD);  // Point 3 (Front)
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(-LARGEUR_QUAD, YNUAGE - (i * DISTANCE_ENTRE_COUCHES), -PROFONDEUR_QUAD);  // Point 4 (Front)*/
            }
            gl.End();
            Console c = Console.getInstance(gl);
            c.AddLigne(Color.Red, "");
            c.AddLigne(Color.Red, "Perlin");
            c.AddLigne(Color.Red, "------");
            c.AddLigne(Color.Red, "Octave " + OCTAVES);
            c.AddLigne(Color.Red, "Freq.  " + FREQUENCE);
            c.AddLigne(Color.Red, "Amp.   " + AMPLITUDE);
            c.AddLigne(Color.Red, "Seuil  " + SEUIL);
            c.AddLigne(Color.Red, "Nb couches  " + NB_COUCHES);
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            if (UneFrameSur(2))
                ScrollPerlin();
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }
        /// <summary>
        /// Rempli les pixels de la bitmap
        /// </summary>
        /// <param name="bmpd"></param>
        protected unsafe void createPerlinNoiseBitmap()
        {
            texture?.Destroy(_gl);
            bmp?.Dispose();

            bmp = new Bitmap(LARGEUR, HAUTEUR, PixelFormat.Format32bppArgb);
            for (int y = 0; y < HAUTEUR; y++)
                for (int x = 0; x < LARGEUR; x++)
                    bmp.SetPixel(x, y, getColor(x, y));

            texture = new Texture();
            texture.Create(_gl, (Bitmap)bmp.Clone());
            
        }

        private Color getColor(float x, float y)
        {
            float v = perlin.FractalNoise2D(x, y, OCTAVES, FREQUENCE, AMPLITUDE);
            int a = (int)((v + AMPLITUDE) * 256.0f);
            if (a < SEUIL)
                a = 0;
            if (a > 255)
                a = 255;

            int c = 255-a;
            return Color.FromArgb(a, c, c, c);

        }

        private int ToPixel(float v)
        {
            int c = (int)((v + AMPLITUDE) * 256.0f);
            if (c < 0)
                c = 0;
            if (c > 255)
                c = 255;

            int a = c > 100 ? c : 0;
            c = 255 - a;
            return ((a << 24) | (c << 16) | (c << 8) | (c << 0));
        }

        private unsafe void ScrollPerlin()
        {
            int Hauteur = bmp.Height;
            int Largeur = bmp.Width;
            yScroll++;
            BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, Largeur, Hauteur), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            // Decaler les lignes
            int* pixelsDest = (int*)(bmpd.Scan0 + ((int)0 * bmpd.Stride));
            int* pixelsSource = (int*)(bmpd.Scan0 + ((int)1 * bmpd.Stride));
            int longueur = ((bmp.Height - 1) * bmpd.Stride) / sizeof(int);
            for (int i = 0; i < longueur; i++)
                *pixelsDest++ = *pixelsSource++;

            bmp.UnlockBits(bmpd);
            for (int x = 0; x < Largeur; x++)
            {
                bmp.SetPixel(x, (Hauteur - 1), getColor(x, yScroll));
            }
            texture = new Texture();
            texture.Create(_gl, (Bitmap)bmp.Clone());
        }

        public override bool KeyDown(Form f, Keys k)
        {
            switch (k)
            {
                case Keys.A:
                    ADDITIVE = !ADDITIVE;
                    return true;

                case Keys.Insert:
                    OCTAVES++;
                    createPerlinNoiseBitmap();
                    return true;

                case Keys.Delete:
                    OCTAVES--;
                    createPerlinNoiseBitmap();
                    return true;

                case Keys.Back:
                    SEUIL++;
                    createPerlinNoiseBitmap();
                    return true;

                case Keys.Enter:
                    SEUIL--;
                    createPerlinNoiseBitmap();
                    return true;
                case Keys.Home:
                    FREQUENCE *= 1.1f;
                    createPerlinNoiseBitmap();
                    return true;
                case Keys.End:
                    FREQUENCE /= 1.1f;
                    createPerlinNoiseBitmap();
                    return true;
                case Keys.PageUp:
                    AMPLITUDE *= 1.1f;
                    createPerlinNoiseBitmap();
                    return true;
                case Keys.PageDown:
                    AMPLITUDE /= 1.1f;
                    createPerlinNoiseBitmap();
                    return true;

                case Keys.Subtract:
                    NB_COUCHES--;
                    return true;

                case Keys.Add:
                    NB_COUCHES++;
                    return true;
                default:
                    return base.KeyDown(f, k);
            }
        }

    }
}
