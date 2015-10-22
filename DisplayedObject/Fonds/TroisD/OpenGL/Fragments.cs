/*
 * Un tunnel infini et mouvant
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Drawing;
using SharpGL;
using GLfloat = System.Single;
using System.Windows.Forms;

namespace ClockScreenSaverGL.DisplayedObject.Fonds.TroisD.Opengl
{
    /// <summary>
    /// Description of Tunnel.
    /// </summary>
    public sealed class Fragments : TroisD
    {
        private const string CAT = "Fragments";
        private static readonly int TAILLE_ANNEAU_MIN = conf.getParametre(CAT, "NbFacettesMin", 4);
        private static readonly int TAILLE_ANNEAU_MAX = conf.getParametre(CAT, "NbFacettesMax", 16);
        private readonly int TAILLE_ANNEAU;
        private static readonly int NB_ANNEAUX = conf.getParametre(CAT, "Nombre", 200);
        private static readonly float VITESSE_ANNEAU = 0.5f;// conf.getParametre(CAT, "Vitesse", 2f);
        private static readonly float DECALAGE_MAX = conf.getParametre(CAT, "DecalageMax", 5f);
        private static readonly float PERIODE_ROTATION = conf.getParametre(CAT, "PeriodeRotation", 10.0f);
        private static readonly float VITESSE_ROTATION = conf.getParametre(CAT, "VitesseRotation", 0.2f);
        private readonly int BANDES_PLEINES = conf.getParametre(CAT, "CouleursPleines", 16 + 1);
        private static readonly float RATIO_DEPLACEMENT = conf.getParametre(CAT, "DeplacementTunnel", 0.5f);
        private static readonly float RAYON_ANNEAU = RATIO_DEPLACEMENT * 5f;
        private static readonly GLfloat PERIODE_DEP_X = conf.getParametre(CAT, "PeriodeDEcalageX", 5f);
        private static readonly GLfloat PERIODE_DEP_Y = conf.getParametre(CAT, "PeriodeDEcalageY", 7f);
        private static bool WIRE_FRAME = conf.getParametre(CAT, "WireFrame", false);
        float _CentreAnneauX;
        float _CentreAnneauY;

        static private DateTime _DernierDeplacement = DateTime.Now;
        static DateTime debut = DateTime.Now;

        Vecteur3D[,] _anneaux;
        bool[,] _afficher;
        readonly float _zMax;
        const float VIEWPORT_X = 2f;
        const float VIEWPORT_Y = 2f;
        const float VIEWPORT_Z = 4f;
        GLfloat[] LightPos = { 0, RAYON_ANNEAU * 0.8f, -RAYON_ANNEAU * 0.8f, 1 };

        /// <summary>
        /// Constructeur: initialiser les anneaux
        /// </summary>
        /// <param name="gl"></param>
        public Fragments(OpenGL gl)
            : base(VIEWPORT_X, VIEWPORT_Y, VIEWPORT_Z, VIEWPORT_Y / 2)
        {
            TAILLE_ANNEAU = r.Next(TAILLE_ANNEAU_MIN, TAILLE_ANNEAU_MAX + 1);
            if (r.Next(0, 3) > 0)
                BANDES_PLEINES = TAILLE_ANNEAU + 1;
            else
                BANDES_PLEINES = r.Next(1, BANDES_PLEINES + 1);
            _anneaux = new Vecteur3D[NB_ANNEAUX, TAILLE_ANNEAU];
            _afficher = new bool[NB_ANNEAUX, TAILLE_ANNEAU];

            _zMax = -_tailleCubeZ;
            _CentreAnneauX = 0;
            _CentreAnneauY = 0;
            for (int x = 0; x < NB_ANNEAUX; x++)
            {
                PlaceAnneau(x);
                float CosTheta = (float)Math.Cos(0.1);
                float SinTheta = (float)Math.Sin(0.1);
                float px, py;

                for (int i = 0; i < x; i++)
                    for (int j = 0; j < TAILLE_ANNEAU; j++)
                    {
                        // Tourner autour de l'axe Z
                        px = (CosTheta * (_anneaux[i, j].x)) - (SinTheta * _anneaux[i, j].y);
                        py = (SinTheta * (_anneaux[i, j].x)) + (CosTheta * _anneaux[i, j].y);

                        _anneaux[i, j].x = px;
                        _anneaux[i, j].y = py;
                    }
            }
        }

        /// <summary>
        /// Placer un anneau
        /// </summary>
        /// <param name="i"></param>
        void PlaceAnneau(int i)
        {
            float profondeur = _tailleCubeZ * 50f;
            float ecart = profondeur / NB_ANNEAUX;
            float z = _tailleCubeZ - (i * ecart);

            for (int j = 0; j < TAILLE_ANNEAU; j++)
            {
                double angle = (Math.PI * 2.0 * j) / (double)TAILLE_ANNEAU;
                _anneaux[i, j] = new Vecteur3D(_CentreAnneauX + (float)(RAYON_ANNEAU * Math.Cos(angle)),
                                              _CentreAnneauY + (float)(RAYON_ANNEAU * Math.Sin(angle)),
                                              z);
                _afficher[i, j] = r.Next(0, 8) > 6;
            }
        }

        /// <summary>
        /// Affichage
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
            float depuisdebut = (float)(debut.Subtract(_DernierDeplacement).TotalMilliseconds / 1000.0);
            float rotation = (float)Math.Cos(depuisdebut / PERIODE_ROTATION) * VITESSE_ROTATION;
            float[] col = { couleur.R / 512.0f, couleur.G / 512.0f, couleur.B / 512.0f, 1.0f };

            gl.ClearColor(0, 0, 0, 1);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Translate(0, 0, -_zCamera);

            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Disable(OpenGL.GL_FOG);
            gl.Disable(OpenGL.GL_DEPTH);

            // Lumiere
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Disable(OpenGL.GL_AUTO_NORMAL);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, LightPos);
            
            if (WIRE_FRAME)
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
            gl.Rotate(0, 0, rotation);
            gl.Color(col);

            // Tracer les anneaux
            gl.Begin(OpenGL.GL_QUADS);
                for (int i = 0; i < NB_ANNEAUX - 1; i++)
            {
                {
                    int iPlusUn = i < (NB_ANNEAUX - 1) ? i + 1 : 0;

                    for (int j = 0; j < TAILLE_ANNEAU; j++)
                    {
                        if (_afficher[i,j])
                        {
                            int jPlusUn = j < (TAILLE_ANNEAU - 1) ? j + 1 : 0;

                            gl.Vertex(_anneaux[i, j].x, _anneaux[i,j].y, _anneaux[i, j].z);
                            gl.Vertex(_anneaux[i, j].x, _anneaux[i, j].y + 0.2f, _anneaux[i, j].z + 0.2f);
                            gl.Vertex(_anneaux[i, j].x + 0.2f, _anneaux[i, j].y + 0.2f, _anneaux[i, j].z + 0.2f);
                            gl.Vertex(_anneaux[i, j].x + 0.2f, _anneaux[i, j].y, _anneaux[i, j].z);
                        }
                    }
                }
               
            }
             gl.End();
            if (WIRE_FRAME)
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

        /// <summary>
        /// Pression sur une touche, retourner true si l'objet a traite, false = fin de l'economiseur
        /// </summary>
        /// <param name="f"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public override bool KeyDown(Form f, Keys k)
        {
            switch (k)
            {
               case Keys.I:
                    {
                        WIRE_FRAME = !WIRE_FRAME;
                        conf.setParametre(CAT, "WireFrame", WIRE_FRAME);
                        return true;
                    }

            }
            return false;
        }



        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            float depuisdebut = (float)(debut.Subtract(maintenant._temps).TotalMilliseconds / 1000.0);
            float vitesseCamera = (float)Math.Sin(depuisdebut / PERIODE_ROTATION) * VITESSE_ROTATION;
            float vitesseRot = maintenant._intervalle * 100;

            float CosTheta = (float)Math.Cos(vitesseCamera * maintenant._intervalle);
            float SinTheta = (float)Math.Sin(vitesseCamera * maintenant._intervalle);
            float px, py;

            float dZ = (VITESSE_ANNEAU * maintenant._intervalle);

            for (int i = 0; i < NB_ANNEAUX; i++)
                for (int j = 0; j < TAILLE_ANNEAU; j++)
                {
                    _anneaux[i, j].z += dZ;

                    // Tourner autour de l'axe Z
                    px = (CosTheta * (_anneaux[i, j].x)) - (SinTheta * _anneaux[i, j].y);
                    py = (SinTheta * (_anneaux[i, j].x)) + (CosTheta * _anneaux[i, j].y);

                    _anneaux[i, j].x = px;
                    _anneaux[i, j].y = py;
                }

            if (_anneaux[0, 0].z > _tailleCubeZ)
            {
                for (int i = 0; i < NB_ANNEAUX - 1; i++)
                    for (int j = 0; j < TAILLE_ANNEAU; j++)
                        _anneaux[i, j] = _anneaux[i + 1, j];

                _CentreAnneauX = (RAYON_ANNEAU * RATIO_DEPLACEMENT) * (float)Math.Sin(depuisdebut / PERIODE_DEP_X);
                _CentreAnneauY = (RAYON_ANNEAU * RATIO_DEPLACEMENT) * (float)Math.Cos(depuisdebut / PERIODE_DEP_Y);

                PlaceAnneau(NB_ANNEAUX - 1);
            }

            _DernierDeplacement = maintenant._temps;
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif

        }
    }
}
