using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Drawing;

using GLfloat = System.Single;
using GLuint = System.UInt32;
using System.Windows.Forms;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Opengl
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
        private static bool WIRE_FRAME = conf.getParametre(CAT, "WireFrame", false);
        #endregion
        const int VIEWPORT_X = 60;
        const int VIEWPORT_Y = 30;
        const float VIEWPORT_Z = 20.0f;
        private static readonly GLfloat[] fogcolor = { 0, 0, 0, 1 };

        private class Carre
        {
            public float x, y, z;
            //public bool aSupprimer;
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
            : base(gl, VIEWPORT_X, VIEWPORT_Y, VIEWPORT_Z, 100)
        {
            _Carres = new Carre[NB_PAVES];
            text.Create(gl, Resources.particleTexture);

            // Initialiser les carres
            for (int i = 0; i < NB_PAVES; i++)
                NouveauCarre(ref _Carres[i]);

            TrierTableau();
        }

        private void NouveauCarre(ref Carre f)
        {
            if (f == null)
            {
                f = new Carre();
                f.z = -VIEWPORT_Z + TAILLE_CARRE * r.Next(0, (int)(_zCamera + VIEWPORT_Z) / (int)TAILLE_CARRE);
            }
            else
                while (f.z > -VIEWPORT_Z)
                    f.z -= VIEWPORT_Z;

            //f.aSupprimer = false;
            f.x = GetXCoord();
            f.y = GetYCoord();

        }

        private int GetYCoord()
        {
            int c;
            do
            {
                c = (int)r.Next(-VIEWPORT_Y / 5, VIEWPORT_Y / 5) * 5;
            }
            while (c == 0);
            return c;
        }

        private int GetXCoord()
        {
            int c = (int)r.Next(-VIEWPORT_X, VIEWPORT_X);
            return (int)((int)(c / TAILLE_CARRE) * TAILLE_CARRE);
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
            gl.Disable(OpenGL.GL_BLEND);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.Enable(OpenGL.GL_FOG);
            gl.Fog(OpenGL.GL_FOG_MODE, OpenGL.GL_LINEAR);
            gl.Fog(OpenGL.GL_FOG_COLOR, fogcolor);
            gl.Fog(OpenGL.GL_FOG_DENSITY, 0.2f);
            gl.Fog(OpenGL.GL_FOG_START, VIEWPORT_Z);
            gl.Fog(OpenGL.GL_FOG_END, _zCamera);
            gl.Translate(0, 0, -_zCamera);
            gl.Rotate(0, 0, vitesseCamera + 90);
            if (WIRE_FRAME)
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);

            couleur = getCouleurOpaqueAvecAlpha(couleur, ALPHA);
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1 };
            gl.Color(col);
            gl.Begin(OpenGL.GL_QUADS);
            foreach (Carre o in _Carres)
            {
                gl.Vertex(o.x, o.y, o.z);
                gl.Vertex(o.x, o.y, o.z + TAILLE_CARRE);
                gl.Vertex(o.x + TAILLE_CARRE, o.y, o.z + TAILLE_CARRE);
                gl.Vertex(o.x + TAILLE_CARRE, o.y, o.z);
            }
            gl.End();
            if (WIRE_FRAME)
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
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
            float deltaZ = VITESSE * maintenant._intervalle;
            // Deplace les flocons
            bool trier = false;
            for (int i = 0; i < NB_PAVES; i++)
            {
                if (_Carres[i].z > (_zCamera + TAILLE_CARRE))
                {
                    NouveauCarre(ref _Carres[i]);
                    trier = true;
                }
                else
                {
                    _Carres[i].z += deltaZ;
                }
            }

            if (trier)
                TrierTableau();


            _dernierDeplacement = maintenant._temps;

#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif

        }

        private void TrierTableau()
        {
            Array.Sort(_Carres, delegate (Carre O1, Carre O2)
            {
                if (DistanceCarre(O1) > DistanceCarre(O2)) return -1;
                if (DistanceCarre(O1) < DistanceCarre(O2)) return 1;
                return 0;
            });
        }

        /// <summary>
        /// Calcule la distance au carre du point à la camera
        /// on n'a pas besoin de la racine carre, donc on ne perd pas de temps
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        private double DistanceCarre(Carre C)
        {
            return (C.x * C.x) + (C.y * C.y) + ((C.z - _zCamera) * (C.z - _zCamera));
        }

        public override bool KeyDown(Form f, Keys k)
        {
            switch (k)
            {
                case TOUCHE_INVERSER:
                    {
                        WIRE_FRAME = !WIRE_FRAME;
                        conf.setParametre(CAT, "WireFrame", WIRE_FRAME);
                        return true;
                    }

            }
            return false;
        }
#if TRACER
        public override String DumpRender()
        {
            return base.DumpRender() + " NbCarres:" + NB_PAVES;
        }

#endif

    }
}
