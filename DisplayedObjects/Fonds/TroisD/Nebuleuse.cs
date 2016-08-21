using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Drawing;

using GLfloat = System.Single;
using GLuint = System.UInt32;
namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD
{
    /// <summary>
    /// Description of Neige.
    /// </summary>
    public sealed class Nebuleuse : TroisD, IDisposable
    {
        #region Parametres
        public const string CAT = "Nebuleuse.OpenGL";

        private static readonly byte ALPHA_ETOILE = 255;// conf.getParametre(CAT, "Alpha", (byte)255);
        private static readonly byte ALPHA_NUAGE = 32;// conf.getParametre(CAT, "Alpha", (byte)255);
        private static readonly float TAILLE_ETOILE = conf.getParametre(CAT, "Taille", 0.15f);
        private static readonly float TAILLE_NUAGE =10.0f;
        private static readonly int NB_ETOILES = 500;// conf.getParametre(CAT, "NbEtoiles", 2000);
        private static readonly int NB_NUAGES =30;// conf.getParametre(CAT, "NbEtoiles", 2000);
        private static readonly float PERIODE_TRANSLATION = conf.getParametre(CAT, "PeriodeTranslation", 13.0f);
        private static readonly float PERIODE_ROTATION = conf.getParametre(CAT, "PeriodeRotation", 10.0f);
        private static readonly float VITESSE_ROTATION = 10f;// conf.getParametre(CAT, "VitesseRotation", 50f);
        private static readonly float VITESSE_TRANSLATION = conf.getParametre(CAT, "VitesseTranslation", 0.2f);
        private static readonly float VITESSE = conf.getParametre(CAT, "Vitesse", 8f);
        private static readonly float DELTA_COULEUR = conf.getParametre(CAT, "Delta Couleur", 0.1f);
        #endregion
        const float VIEWPORT_X = 5f;
        const float VIEWPORT_Y = 5f;
        const float VIEWPORT_Z = 5f;
        private static readonly GLfloat[] fogcolor = { 0, 0, 0, 1 };

        const int TYPE_ETOILE = 0;
        const int TYPE_NUAGE = 1;
        private class Etoile
        {
            public float x, y, z;
            public float tx, ty;
            public float rR, rG, rB;
        }

        private class Nuage
        {
            public float x, y, z;
            public float tx, ty;
            public float rR, rG, rB;
            public float Alpha;
        }
        private readonly Etoile[] _etoiles;
        private readonly Nuage[] _nuages;
        private DateTime _dernierDeplacement = DateTime.Now;
        private DateTime _debutAnimation = DateTime.Now;
        Texture _textureNuage = new Texture();
        Texture _textureEtoile = new Texture();

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="gl"></param>
        public Nebuleuse(OpenGL gl)
            : base(gl, VIEWPORT_X, VIEWPORT_Y, VIEWPORT_Z, 100)
        {
            _etoiles = new Etoile[NB_ETOILES];
            _nuages = new Nuage[NB_NUAGES];
            _textureNuage.Create(gl, Config.getImagePath("nuage_nebuleuse.png"));
            _textureEtoile.Create(gl, Config.getImagePath("etoile.png"));

            // Initialiser les etoiles
            for (int i = 0; i < NB_ETOILES; i++)
            {
                NouvelleEtoile(ref _etoiles[i]);
                // Au debut, on varie la distance des etoiles
                _etoiles[i].z = FloatRandom(-VIEWPORT_Z, _zCamera);
            }

            // Initialiser les nuages
            for (int i = 0; i < NB_NUAGES; i++)
            {
                NouveauNuage(ref _nuages[i]);
                // Au debut, on varie la distance des etoiles
                _nuages[i].z = FloatRandom(-VIEWPORT_Z, _zCamera);
            }
        }


        public override void Dispose()
        {
            base.Dispose();
            _textureNuage?.Destroy(_gl);
            _textureEtoile?.Destroy(_gl);
        }

        protected static bool Probabilite(float f)
        {
            return FloatRandom(0, 1.0f) < f;
        }

        private static void NouvelleEtoile(ref Etoile f)
        {
            if (f == null)
                f = new Etoile();
            f.x = FloatRandom(-VIEWPORT_X * 6, VIEWPORT_X * 6);
            f.z = -VIEWPORT_Z;
            f.y = FloatRandom(-VIEWPORT_Y * 6, VIEWPORT_Y * 6);

            f.rR = FloatRandom(0.7f, 1.3f);
            f.rG = FloatRandom(0.7f, 1.3f);
            f.rB = FloatRandom(0.7f, 1.3f);

            f.tx = TAILLE_ETOILE;
            f.ty = TAILLE_ETOILE;
        }

        private static void NouveauNuage(ref Nuage f)
        {
            if (f == null)
                f = new Nuage();
            f.x = FloatRandom(-VIEWPORT_X * 6, VIEWPORT_X * 6);
            f.z = -VIEWPORT_Z;
            f.y = FloatRandom(-VIEWPORT_Y * 3, VIEWPORT_Y * 3);

            f.rR = FloatRandom(0.4f, 1.7f)/ 256.0f;
            f.rG = FloatRandom(0.4f, 1.7f)/ 256.0f;
            f.rB = FloatRandom(0.4f, 1.7f)/ 256.0f;

            f.tx = TAILLE_NUAGE * FloatRandom(0.75f, 1.5f);
            f.ty = TAILLE_NUAGE * FloatRandom(0.75f, 1.5f);
            f.Alpha = ((float)ALPHA_NUAGE / 256.0f) * FloatRandom(0.5f, 1.2f);
        }

        /// <summary>
        /// Affichage des flocons
        /// </summary>
        /// <param name="g"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleur"></param>
        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            float depuisdebut = (float)(_debutAnimation.Subtract(_dernierDeplacement).TotalMilliseconds / 1000.0);
            float vitesseCamera = (float)Math.Sin(depuisdebut / PERIODE_ROTATION) * VITESSE_ROTATION;

            float[] fcolor = { couleur.R / 1024.0f, couleur.G / 1024.0f, couleur.B / 1024.0f, 1 };

            gl.ClearColor(fcolor[0], fcolor[1], fcolor[2], fcolor[3]);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH_TEST);
            gl.Disable(OpenGL.GL_DEPTH);

            gl.Enable(OpenGL.GL_FOG);
            gl.Fog(OpenGL.GL_FOG_MODE, OpenGL.GL_LINEAR);
            gl.Fog(OpenGL.GL_FOG_COLOR, fcolor);
            gl.Fog(OpenGL.GL_FOG_DENSITY, 0.1f);
            gl.Fog(OpenGL.GL_FOG_START, VIEWPORT_Z * 0.5f);
            gl.Fog(OpenGL.GL_FOG_END, _zCamera);

            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Translate(0, 0, -_zCamera);
            gl.Rotate(0, 0, vitesseCamera);

            _textureNuage.Bind(gl);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.Begin(OpenGL.GL_QUADS);
            foreach (Nuage nuage in _nuages)
            {
                gl.Color(couleur.R * nuage.rR, couleur.G * nuage.rG, couleur.B * nuage.rB, nuage.Alpha);
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(nuage.x - nuage.tx, nuage.y - nuage.ty, nuage.z);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(nuage.x - nuage.tx, nuage.y + nuage.ty, nuage.z);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(nuage.x + nuage.tx, nuage.y + nuage.ty, nuage.z);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(nuage.x + nuage.tx, nuage.y - nuage.ty, nuage.z);
            }
            gl.End();

            _textureEtoile.Bind(gl);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE);
            gl.Begin(OpenGL.GL_QUADS);
            foreach (Etoile o in _etoiles)
            {
                gl.Color(couleur.R * o.rR / 256.0f, couleur.G * o.rG / 256.0f, couleur.B * o.rB / 256.0f, ALPHA_ETOILE/256.0f);
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(o.x - o.tx, o.y - o.ty, o.z);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(o.x - o.tx, o.y + o.ty, o.z);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(o.x + o.tx, o.y + o.ty, o.z);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(o.x + o.tx, o.y - o.ty, o.z);
            }
            gl.End();


#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }



        /// <summary>
        /// Deplacement de tous les objets: flocons, camera...
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif

            float depuisdebut = (float)(_debutAnimation.Subtract(_dernierDeplacement).TotalMilliseconds / 1000.0);
            float vitesseCamera = (float)Math.Sin(depuisdebut / PERIODE_ROTATION) * VITESSE_ROTATION;
            float deltaZ = VITESSE * maintenant._intervalle;
            float deltaWind = (float)Math.Sin(depuisdebut / PERIODE_TRANSLATION) * VITESSE_TRANSLATION * maintenant._intervalle;

            // Deplace les etoiles
            bool trier = false;
            for (int i = 0; i < NB_ETOILES; i++)
            {
                if (_etoiles[i].z > _zCamera)
                {
                    NouvelleEtoile(ref _etoiles[i]);
                    trier = true;
                }
                else
                {
                    _etoiles[i].z += deltaZ;
                    _etoiles[i].x += deltaWind;
                }
            }

            if (trier)
                Array.Sort(_etoiles, delegate (Etoile O1, Etoile O2)
                {
                    if (O1.z > O2.z) return 1;
                    if (O1.z < O2.z) return -1;
                    return 0;
                });

            // Deplace les nuages
            trier = false;
            for (int i = 0; i < NB_NUAGES; i++)
            {
                if (_nuages[i].z > _zCamera)
                {
                    NouveauNuage(ref _nuages[i]);
                    trier = true;
                }
                else
                {
                    _nuages[i].z += deltaZ;
                    _nuages[i].x += deltaWind;
                }
            }

            if (trier)
                Array.Sort(_nuages, delegate (Nuage O1, Nuage O2)
                {
                    if (O1.z > O2.z) return 1;
                    if (O1.z < O2.z) return -1;
                    return 0;
                });
            _dernierDeplacement = maintenant._temps;

#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif

        }

#if TRACER
        public override String DumpRender()
        {
            return base.DumpRender() + " NbParticules:" + NB_ETOILES;
        }

#endif
    }
}
