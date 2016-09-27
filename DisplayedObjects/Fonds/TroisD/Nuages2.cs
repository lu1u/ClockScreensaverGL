using ClockScreenSaverGL.Config;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD
{
    /// <summary>
    /// Description of Neige.
    /// </summary>
    public class Nuages2 : TroisD, IDisposable
    {
        #region Parametres
        public const string CAT = "Nuages2";
        static protected CategorieConfiguration c = Config.Configuration.getCategorie(CAT);
        private static readonly float ALPHA = c.getParametre("Alpha", (byte)1.0f);
        private static readonly int NB_NUAGES = c.getParametre("NbNuages", 200);
        private static readonly float TAILLE_NUAGE = c.getParametre("Taille", 9);
        private static readonly float ROULIS_MAX = c.getParametre("Roulis max", 3);
        private static readonly float VITESSE_ROULIS = c.getParametre("Vitesse roulis", 0.1f);
        private static readonly float VITESSE = c.getParametre("Vitesse", 2.0f);
        private static readonly float VITESSE_LATERALE = c.getParametre("Vitesse laterale", 10.0f);
        private static readonly float COLOR_RATIO = 150;
        #endregion
        const float VIEWPORT_X = 12f;
        const float VIEWPORT_Y = 5f;
        const float VIEWPORT_Z = 5f;

        private class Nuage
        {
            public float x, y, z;
            public float tailleX, tailleY;
            public int texture;
        }
        private readonly Nuage[] _nuages;
        private DateTime _dernierDeplacement = DateTime.Now;
        private DateTime _debutAnimation = DateTime.Now;
        const int NB_TEXTURES = 6;
        Texture _texture = new Texture();
        float angle = 0;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="gl"></param>
        public Nuages2(OpenGL gl)
            : base(gl, VIEWPORT_X, VIEWPORT_Y, VIEWPORT_Z, 100)
        {
            _nuages = new Nuage[NB_NUAGES];

            _texture.Create(gl, c.getParametre("Nuages", Configuration.getImagePath("nuages.png")));
            // Initialiser les etoiles
            for (int i = 0; i < NB_NUAGES; i++)
            {
                NouveauNuage(ref _nuages[i]);
                // Au debut, on varie la distance des etoiles
                _nuages[i].z = FloatRandom(-VIEWPORT_Z, _zCamera);
            }

            Array.Sort(_nuages, delegate (Nuage O1, Nuage O2)
                {
                    if (O1.z > O2.z) return 1;
                    if (O1.z < O2.z) return -1;
                    return 0;
                });
        }

        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
        public sealed override void Dispose()
        {
            base.Dispose();
            _texture.Destroy(_gl);
        }

        private static void NouveauNuage(ref Nuage f)
        {
            if (f == null)
                f = new Nuage();

            f.x = VIEWPORT_X * FloatRandom(-6, 6);
            f.z = -VIEWPORT_Z;
            f.y = FloatRandom(-VIEWPORT_Y * 3, -1.5f);
            f.tailleX = TAILLE_NUAGE * FloatRandom(0.6f, 1.4f);
            f.tailleY = TAILLE_NUAGE * FloatRandom(0.6f, 1.4f);
            f.texture = r.Next(NB_TEXTURES);
        }

        public override void ClearBackGround(OpenGL gl, Color couleur)
        {
            gl.ClearColor(couleur.R / 512.0f, couleur.G / 512.0f, couleur.B / 512.0f, 1);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_COLOR_MATERIAL);
            gl.Disable(OpenGL.GL_DEPTH_TEST);
            //gl.DepthMask((byte)OpenGL.GL_FALSE);

            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D(-1.0, 1.0, -1.0, 1.0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Begin(OpenGL.GL_QUAD_STRIP);
            {
                gl.Color(1.0f, 1.0f, 1.0f); gl.Vertex(-1f, -1f); gl.Vertex(1f, -1f);
                gl.Color(couleur.R / 512.0f, couleur.G / 512.0f, couleur.B / 512.0f, 1); gl.Vertex(-1f, 0.6f); gl.Vertex(1f, 0.6f);
                gl.Color(0f, 0f, 0f); gl.Vertex(-1f, 1f); gl.Vertex(1f, 1f);
            }
            gl.End();

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();
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
            float[] fogcol = { couleur.R / 512.0f, couleur.G / 512.0f, couleur.B / 512.0f, 1 };

            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Disable(OpenGL.GL_DEPTH_TEST);

            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.LookAt(0, TAILLE_NUAGE * 0.8f, _zCamera, VITESSE_LATERALE *(float)Math.Sin(angle), 0, 0, 0, 1, 0);
            gl.Rotate(0, 0, (float)(Math.Sin(angle) * ROULIS_MAX));

            _texture.Bind(gl);
            gl.Begin(OpenGL.GL_QUADS);
            foreach (Nuage o in _nuages)
            {
                gl.Color(couleur.R / COLOR_RATIO, couleur.G / COLOR_RATIO, couleur.B / COLOR_RATIO, ALPHA - (VIEWPORT_Z - o.z) / VIEWPORT_Z);

                float xG = 1.0f / NB_TEXTURES * o.texture;
                float xD = 1.0f / NB_TEXTURES * (o.texture + 1);
                gl.TexCoord(xG, 1.0f); gl.Vertex(o.x - o.tailleX, o.y - o.tailleY, o.z);
                gl.TexCoord(xG, 0.0f); gl.Vertex(o.x - o.tailleX, o.y + o.tailleY, o.z);
                gl.TexCoord(xD, 0.0f); gl.Vertex(o.x + o.tailleX, o.y + o.tailleY, o.z);
                gl.TexCoord(xD, 1.0f); gl.Vertex(o.x + o.tailleX, o.y - o.tailleY, o.z);
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

            angle += VITESSE_ROULIS * maintenant._intervalle;
            float depuisdebut = (float)(_debutAnimation.Subtract(_dernierDeplacement).TotalMilliseconds / 1000.0);
            float deltaZ = VITESSE * maintenant._intervalle;
            float deltaX = VITESSE_LATERALE * maintenant._intervalle * (float)Math.Sin(angle);
            // Deplace les nuages
            bool trier = false;
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
                   // _nuages[i].x -= deltaX;
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
            return base.DumpRender() + " NbParticules:" + NB_NUAGES;
        }

#endif
    }
}
