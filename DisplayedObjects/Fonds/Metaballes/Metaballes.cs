/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 14/12/2014
 * Heure: 22:50
 * 
 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Text;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using ClockScreenSaverGL.Config;

namespace ClockScreenSaverGL.DisplayedObjects.Metaballes
{

    public class Metaballes : Fonds.Fond
    {
        #region Parametres
        const string COULEURS_INVERSE = "CouleursInversees";
        const string NEGATIF = "NegatifCouleurs";
        const string CAT = "Metaballes";
        private static CategorieConfiguration cat = Configuration.getCategorie(CAT);
        protected double RatioCouleur;
        protected static bool _CouleursInverses;
        protected static bool _NegatifCouleurs;
        #endregion

        protected readonly int NiveauxCouleurs;
        protected readonly int[] _palette;

        protected Rectangle _rectBitmap;
        protected int NbMetaballes;
        protected MetaBalle[] _metaballes;
        protected readonly int Largeur, Hauteur;

        /// <summary>
        /// Constructeur
        /// </summary>
        public Metaballes(OpenGL gl) : base(gl)
        {
            CategorieConfiguration c = getConfiguration();
            RatioCouleur = c.getParametre("RatioCouleur", 0.3f, true);    // Plus la valeur est grande, plus la couleur sera foncee. 255 au minimum
            _CouleursInverses = c.getParametre(COULEURS_INVERSE, false, true);
            _NegatifCouleurs = c.getParametre(NEGATIF, false, true);


            GetPreferences(ref Largeur, ref Hauteur, ref NbMetaballes, ref NiveauxCouleurs);
            _palette = new int[NiveauxCouleurs];
            if (_metaballes == null)
                _metaballes = new MetaBalle[NbMetaballes];

            _rectBitmap = new Rectangle(0, 0, Largeur, Hauteur);
            ConstruitMetaballes();

            c.setListenerParametreChange(onConfigurationChangee);
        }

        /// <summary>
        /// Appellee en notification d'un changement interactif de configuration
        /// </summary>
        /// <param name="valeur"></param>
        protected override void onConfigurationChangee(string valeur)
        {
            CategorieConfiguration c = getConfiguration();
            RatioCouleur = c.getParametre("RatioCouleur", 0.3f, true);    // Plus la valeur est grande, plus la couleur sera foncee. 255 au minimum
            _CouleursInverses = c.getParametre(COULEURS_INVERSE, false, true);
            _NegatifCouleurs = c.getParametre(NEGATIF, false, true);

            base.onConfigurationChangee(valeur);
        }

        public override CategorieConfiguration getConfiguration()
        {
            return cat;
        }
        /// <summary>
        /// Lit les preferences a chaque version de metaballes
        /// </summary>
        /// <param name="L"></param>
        /// <param name="H"></param>
        /// <param name="N"></param>
        /// <param name="C"></param>
        protected virtual void GetPreferences(ref int L, ref int H, ref int N, ref int C)
        {
            CategorieConfiguration c = getConfiguration();
            L = c.getParametre("Largeur", 400);
            H = c.getParametre("Hauteur", 300);
            N = c.getParametre("Nombre", 8);
            C = c.getParametre("NiveauxCouleur", 255);
        }

        protected virtual void ConstruitMetaballes()
        {
            CategorieConfiguration c = getConfiguration();

            float TailleMax = c.getParametre("TailleMax", 200);
            float TailleMin = c.getParametre("TailleMin", 70);
            float IntensiteMax = c.getParametre("IntensiteMax", 0.8f);
            float IntensiteMin = c.getParametre("IntensiteMin", 0.2f);

            for (int i = 0; i < NbMetaballes; i++)
                _metaballes[i] = new MetaBalle(FloatRandom(IntensiteMin, IntensiteMax),	// Intensite
                                               FloatRandom(TailleMin, TailleMax),			// Taille
                                               FloatRandom(0, Largeur), FloatRandom(0, Hauteur), // Position
                                               FloatRandom(-10, 10), FloatRandom(-10, 10));  // Vitesse
        }

        /// <summary>
        /// Changer l'image
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif

            // Deplacement des metaballes
            for (int i = 0; i < NbMetaballes; i++)
            {
                _metaballes[i]._Px += (_metaballes[i]._Vx * maintenant._intervalle);

                if ((_metaballes[i]._Px < 0) && (_metaballes[i]._Vx < 0))
                    _metaballes[i]._Vx = Math.Abs(_metaballes[i]._Vx);
                else
                    if ((_metaballes[i]._Px > Largeur) && (_metaballes[i]._Vx > 0))
                    _metaballes[i]._Vx = -Math.Abs(_metaballes[i]._Vx);

                _metaballes[i]._Py += (_metaballes[i]._Vy * maintenant._intervalle);
                if ((_metaballes[i]._Py < 0) && (_metaballes[i]._Vy < 0))
                    _metaballes[i]._Vy = Math.Abs(_metaballes[i]._Vy);
                else
                    if ((_metaballes[i]._Py > Hauteur) && (_metaballes[i]._Vy > 0))
                    _metaballes[i]._Vy = -Math.Abs(_metaballes[i]._Vy);
            }



#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }


        /// <summary>
        /// Affiche l'objet
        /// </summary>
        /// <param name="g"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleur"></param>
        /// 
#if USE_GDI_PLUS_FOR_2D
        public override void AfficheGDI(Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            // Mise a jour de la palette de couleurs
            updatePalette(couleur);

            // Construire la bitmap
            updateFrame();
            SmoothingMode q = g.SmoothingMode;
            CompositingQuality c = g.CompositingQuality;
            InterpolationMode m = g.InterpolationMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(_bmp, 0, 0, tailleEcran.Width, tailleEcran.Height);
            g.SmoothingMode = q;
            g.CompositingQuality = c;
            g.InterpolationMode = m;
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
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D(0.0, 1.0, 0.0, 1.0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Disable(OpenGL.GL_BLEND);
            //gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            updatePalette(couleur);

            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1 };
            gl.Color(col);

            using (Bitmap bmp = updateFrame())
            {
                Texture text = new Texture();
                text.Create(gl, bmp);
                text.Bind(gl);
                gl.Begin(OpenGL.GL_QUADS);
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(0, 1);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(0, 0);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(1, 0);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(1, 1);
                gl.End();

                uint[] textures = { text.TextureName };
                gl.DeleteTextures(1, textures);
                text.Destroy(gl);
            }
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
        /// Pression sur une touche, retourner true si l'objet a traite, false = fin de l'economiseur
        /// </summary>
        /// <param name="f"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        /*
        public override bool KeyDown(Form f, Keys k)
        {
            CategorieConfiguration c = getConfiguration();
            
            switch (k)
            {
                case TOUCHE_INVERSER:
                    {
                        _CouleursInverses = !_CouleursInverses;
                        c.setParametre(COULEURS_INVERSE, _CouleursInverses);
                        return true;
                    }

                case TOUCHE_NEGATIF:
                    {
                        _NegatifCouleurs = !_NegatifCouleurs;
                        c.setParametre(NEGATIF, _NegatifCouleurs);
                        return true;
                    }
            }
            return base.KeyDown(f, k); ;
        }
        */


        /// <summary>
        /// Change les couleurs de la palette
        /// La palette est 'monochrome' avec la teinte globale de l'image
        /// </summary>
        /// <param name="c"></param>
        protected virtual void updatePalette(Color c)
        {
            try
            {
                double PI2 = Math.PI * 2;
                double r, g, b;

                if (_CouleursInverses)
                {
                    r = (255 - c.R);
                    g = (255 - c.G);
                    b = (255 - c.B);
                }
                else
                {
                    r = c.R;
                    g = c.G;
                    b = c.B;
                }

                r = r * RatioCouleur / NiveauxCouleurs;
                g = g * RatioCouleur / NiveauxCouleurs;
                b = b * RatioCouleur / NiveauxCouleurs;

                if (_NegatifCouleurs)
                    for (int x = 0; x < NiveauxCouleurs; x++)
                    {
                        double i = Math.Abs(Math.Cos(x * PI2 / NiveauxCouleurs) * NiveauxCouleurs);
                        _palette[NiveauxCouleurs - 1 - x] = ((int)(r * i) << 16) | ((int)(g * i) << 8) | (int)(b * i);
                    }
                else
                    for (int x = 1; x < NiveauxCouleurs; x++)
                    {
                        double i = Math.Abs(Math.Sin(x * PI2 / NiveauxCouleurs) * NiveauxCouleurs);
                        _palette[x] = ((int)(r * i) << 16) | ((int)(g * i) << 8) | (int)(b * i);

                        _palette[0] = 0;
                    }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\ndans : " + ex.Source, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Rempli les pixels de la bitmap
        /// </summary>
        /// <param name="bmpd"></param>
        protected unsafe Bitmap updateFrame()
        {
            Bitmap bmp = new Bitmap(Largeur, Hauteur, PixelFormat.Format32bppRgb);
            BitmapData bmpd = bmp.LockBits(_rectBitmap, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            double x, y;
            int z, Indice;
            double Champs;
            double largeur = bmpd.Width;

            for (y = 0; y < bmpd.Height; y++)
            {
                int* pixels = (int*)(bmpd.Scan0 + ((int)y * bmpd.Stride));

                for (x = 0; x < largeur; x++)
                {
                    // Calcul de la valeur du champs en ce point = somme des champs de toutes les metaballes
                    Champs = 0;
                    for (z = 0; z < NbMetaballes; z++)
                        Champs += _metaballes[z].Champ(x, y);

                    // Convertir en indice dans la palette
                    Indice = (int)Math.Round(Champs * NiveauxCouleurs);
                    if (Indice >= NiveauxCouleurs)
                        Indice = NiveauxCouleurs - 1;
                    else
                        if (Indice < 0)
                        Indice = 0;

                    *pixels++ = _palette[Indice];
                }
            }
            bmp.UnlockBits(bmpd);
            return bmp;
        }


#if TRACER
        public override string DumpRender()
        {
#if USE_GDI_PLUS_FOR_2D
            return  base.DumpRender() + " (GDI)" ;
#else
            return base.DumpRender() + " (OpenGL)";
#endif
        }
#endif
    }
}