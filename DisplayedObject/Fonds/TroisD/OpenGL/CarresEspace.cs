using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Drawing;

using GLfloat = System.Single;
using GLuint = System.UInt32;
namespace ClockScreenSaverGL.DisplayedObject.Fonds.TroisD.Opengl
{
    /// <summary>
    /// Description of Neige.
    /// </summary>
    public sealed class CarresEspace : TroisD
    {
        #region Parametres
        public const string CAT = "CarresEspace";

        private static readonly byte ALPHA = conf.getParametre(CAT, "Alpha", (byte)250);
        private static readonly float TAILLE_CARRE = conf.getParametre(CAT, "Taille", 5.0f);
        private static readonly int NB_PAVES = conf.getParametre(CAT, "Nb", 200);
        private static readonly float PERIODE_TRANSLATION = conf.getParametre(CAT, "PeriodeTranslation", 13.0f);
        private static readonly float PERIODE_ROTATION = conf.getParametre(CAT, "PeriodeRotation", 10.0f);
        private static readonly float VITESSE_ROTATION = conf.getParametre(CAT, "VitesseRotation", 50f);
        private static readonly float VITESSE_TRANSLATION = conf.getParametre(CAT, "VitesseTranslation", 0.2f);
        private static readonly float VITESSE = conf.getParametre(CAT, "Vitesse", 8f);
        #endregion
        const int VIEWPORT_X = 80;
        const int VIEWPORT_Y = 15;
        const float VIEWPORT_Z = 10.0f;
        private static readonly GLfloat[] fogcolor = { 0, 0, 0, 1 };

        private class Carre
        {
            public float x, y, z;
            public bool aSupprimer;
        }
        private readonly Carre[] _Carres;
        private DateTime _dernierDeplacement = DateTime.Now;
        private DateTime _debutAnimation = DateTime.Now;
        Texture text = new Texture();

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="gl"></param>
        public CarresEspace(OpenGL gl)
            : base(VIEWPORT_X, VIEWPORT_Y, VIEWPORT_Z, 100)
        {
            _Carres = new Carre[NB_PAVES];
            text.Create(gl, Resources.particleTexture);

            // Initialiser les etoiles
            for (int i = 0; i < NB_PAVES; i++)
            {
                NouveauCarre(ref _Carres[i]);
                // Au debut, on varie la distance des etoiles
                _Carres[i].z = FloatRandom(-VIEWPORT_Z, _zCamera);
            }
        }

        private void NouveauCarre(ref Carre f)
        {
            if (f == null)
                f = new Carre();

            f.aSupprimer = false;
            f.x = GetXCoord();
            f.y = GetYCoord();
            f.z = -VIEWPORT_Z; 

        }

        private int GetXCoord()
        {
            int c;
            do
            {
                c = (int)r.Next(-VIEWPORT_X, VIEWPORT_X );
            }
            while (c == 0);
            return  (int)((int)(c/(TAILLE_CARRE*2)) * TAILLE_CARRE * 2) ;
        }

        private int GetYCoord()
        {
            int c = (int)r.Next(-VIEWPORT_Y, VIEWPORT_Y);
            return (int)(c * TAILLE_CARRE);
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

            gl.ClearColor(0, 0, 0, 1);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_DEPTH);

            gl.Enable(OpenGL.GL_FOG);
            gl.Fog(OpenGL.GL_FOG_MODE, OpenGL.GL_LINEAR);
            gl.Fog(OpenGL.GL_FOG_COLOR, fogcolor);
            gl.Fog(OpenGL.GL_FOG_DENSITY, 0.01f);
            gl.Fog(OpenGL.GL_FOG_START, VIEWPORT_Z);
            gl.Fog(OpenGL.GL_FOG_END, _zCamera);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
            gl.Translate(0, 0, -_zCamera);
            gl.Rotate(0, 0, vitesseCamera);

            couleur = getCouleurOpaqueAvecAlpha(couleur, ALPHA);
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1 };
            gl.Color(col);
            gl.Begin(OpenGL.GL_QUADS);
            foreach (Carre o in _Carres)
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(o.x - TAILLE_CARRE, o.y, o.z - TAILLE_CARRE);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(o.x - TAILLE_CARRE, o.y, o.z + TAILLE_CARRE);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(o.x + TAILLE_CARRE, o.y, o.z + TAILLE_CARRE);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(o.x + TAILLE_CARRE, o.y, o.z - TAILLE_CARRE);
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
        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif

            float depuisdebut = (float)(_debutAnimation.Subtract(_dernierDeplacement).TotalMilliseconds / 1000.0);
            float vitesseCamera = (float)Math.Sin(depuisdebut / PERIODE_ROTATION) * VITESSE_ROTATION;
            float deltaZ = VITESSE * maintenant._intervalle;
            float deltaWind = (float)Math.Sin(depuisdebut / PERIODE_TRANSLATION) * VITESSE_TRANSLATION * maintenant._intervalle;
            // Deplace les flocons
            bool trier = false;
            for (int i = 0; i < NB_PAVES; i++)
            {
                if (_Carres[i].aSupprimer)
                {
                    NouveauCarre(ref _Carres[i]);
                    trier = true;
                }
                else
                {
                    if (_Carres[i].z > _zCamera)
                        _Carres[i].aSupprimer = true;
                    else
                    {
                        _Carres[i].z += deltaZ;
                        _Carres[i].x += deltaWind;
                    }
                }
            }

            if (trier)
                Array.Sort(_Carres, delegate(Carre O1, Carre O2)
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
            return base.DumpRender() + " NbParticules:" + NB_PAVES;
        }

#endif
    }
}
