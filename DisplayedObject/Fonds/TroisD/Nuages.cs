using SharpGL;
/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 20/01/2015
 * Heure: 18:59
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using SharpGL.SceneGraph.Assets;
namespace ClockScreenSaverGL.Fonds.TroisD
{
    /// <summary>
    /// Description of Nuage.
    /// </summary>
    public class Nuages : TroisD
    {
        const string CAT = "Nuages";
        static readonly float ALPHA = conf.getParametre( CAT, "Alpha", 0.05f ) ;
        static readonly float VITESSE = conf.getParametre(CAT, "Vitesse", 1f);
        static readonly float RAYON_MAX = conf.getParametre(CAT, "RayonMax", 2f);
        static readonly float RAYON_MIN = conf.getParametre(CAT, "RayonMin", 0.5f);
        static readonly int NBPARTICULES_MAX = conf.getParametre(CAT, "NbParticulesMax", 100);
        static readonly float TAILLE_PARTICULE = conf.getParametre(CAT, "TailleParticules", 0.7f);
       static readonly int NB_NUAGES = conf.getParametre(CAT, "Nb", 150);
        static readonly int NB_SOMMETS_DESSIN = conf.getParametre(CAT, "NbSommets", 8);
        float _positionNuage = conf.getParametre(CAT, "EnHaut", true) ? 1f : -1f;
        const float VIEWPORT_X = 1f;
        const float VIEWPORT_Y = 1f;
        const float VIEWPORT_Z = 10f;

        private class Particule
        {
            public float x, y, z, taille, alpha;
        }

        private class Nuage
        {
            public float _rayon;
            public float x, y, z;
            public Particule[] _particules;
        }

        private Nuage[] _nuages;					// Pour creer des nuages ressemblants
        private Particule[] _particules;			// Pour l'affichage
        private int _NbParticules;

        // The texture identifier.
        public Nuages(OpenGL gl)
            : base(VIEWPORT_X, VIEWPORT_Y, VIEWPORT_Z, 10, NB_SOMMETS_DESSIN, 1.0f)
        {
            _nuages = new Nuage[NB_NUAGES];
            _particules = new Particule[NB_NUAGES * NBPARTICULES_MAX];
            
            for (int i = 0; i < NB_NUAGES; i++)
                creerNuage(ref _nuages[i], true);
        }

        /// <summary>
        /// Creer un nuage
        /// </summary>
        /// <param name="nuage"></param>
        /// <param name="init">true s'il s'agit de la creation initiale, false si c'est un
        /// recyclage d'un nuage qui est passe derriere la camera</param>
        void creerNuage(ref Nuage nuage, bool init)
        {
            if (nuage == null)
            {
                nuage = new Nuage();
                nuage._particules = new Particule[NBPARTICULES_MAX];
            }

            nuage._rayon = FloatRandom(RAYON_MIN, RAYON_MAX);
            nuage.x = FloatRandom(-_tailleCubeX * 50, _tailleCubeX * 50);
            nuage.y = _positionNuage * _tailleCubeY * FloatRandom(-0.2f, 4);

            if (init)
                nuage.z = -FloatRandom(-_zCamera, _tailleCubeZ * 10);
            else
                nuage.z = -_tailleCubeZ * 10;

            for (int i = 0; i < NBPARTICULES_MAX; i++)
            {
                float AngleX = FloatRandom(0, (float)Math.PI * 2);
                float AngleZ = FloatRandom(0, (float)Math.PI * 2);
                float rayon = FloatRandom(nuage._rayon * 0.1f, nuage._rayon);

                nuage._particules[i] = new Particule();
                nuage._particules[i].x = nuage.x + rayon * 2 * (float)Math.Cos(AngleX);
                nuage._particules[i].y = nuage.y + rayon * (float)Math.Sin(AngleX);
                nuage._particules[i].z = nuage.z + rayon * 1.5f * (float)Math.Sin(AngleZ);
                nuage._particules[i].taille = FloatRandom(TAILLE_PARTICULE*.075f, TAILLE_PARTICULE*1.5f);
                nuage._particules[i].alpha = FloatRandom(ALPHA*0.9f, ALPHA*2.0f);
            }
        }

        /// <summary>
        /// Affichage des nuages
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

            float[] col = { couleur.R/255.0f, couleur.G/255.0f, couleur.B/255.0f, 1 };

            gl.ClearColor(col[0] / 4, col[1] / 4, col[2] / 4, col[3]);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Disable(OpenGL.GL_LIGHTING); 	
            gl.Disable(OpenGL.GL_COLOR_MATERIAL);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Disable(OpenGL.GL_FOG);

            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            for (int i = 0; i < _NbParticules; i++)
            {
                float taille = _particules[i].taille;
                gl.Begin(OpenGL.GL_TRIANGLE_FAN);
                gl.Color(1, 1, 1, _particules[i].alpha);
                gl.Vertex(_particules[i].x, _particules[i].y, _particules[i].z);
                gl.Color(col[0], col[1], col[2], 0f);

                for (int z = 0; z < NB_SOMMETS_DESSIN; z++)
                    gl.Vertex(_particules[i].x + _coordPoint[z].X * taille, _particules[i].y + _coordPoint[z].Y * taille, _particules[i].z);

                gl.Vertex(_particules[i].x + _coordPoint[0].X * taille, _particules[i].y + _coordPoint[0].Y * taille, _particules[i].z);
                gl.End();
            }

            gl.Color(0.5f, 0.5f, 0, 1);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(-5, 5, 0);
            gl.Vertex(-5, -5, 0);
            gl.Vertex(5, -5, 0);
            gl.Vertex(5, 5, 0);
            gl.End();
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif

        }

       
        /// <summary>
        /// Deplacer les nuages
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif

            float vitesse = maintenant._intervalle * VITESSE;
            bool derriereCam;
            bool trierNuages = false;
            for (int i = 0; i < NB_NUAGES; i++)
            {
                derriereCam = true;
                _nuages[i].z += vitesse;
                for (int j = 0; j < NBPARTICULES_MAX; j++)
                {
                    _nuages[i]._particules[j].z += vitesse;

                    if (_nuages[i]._particules[j].z < _zCamera)
                        // Au moins une des particules du nuage est devant la camera
                        derriereCam = false;
                }

                if (derriereCam)
                {
                    // Aucune partie du nuage devant la camera: le recreer au loin
                    creerNuage(ref _nuages[i], false);
                    trierNuages = true;
                }
            }

            if (trierNuages)
            {
                Array.Sort(_nuages, delegate(Nuage O1, Nuage O2)
                           {
                               if (O1.z > O2.z) return 1;
                               if (O1.z < O2.z) return -1;
                               return 0;
                           });
            }

            // Recuperer le tableau des particules, et les trier en fonction de la distance
            _NbParticules = 0;
            for (int i = 0; i < NB_NUAGES; i++)
                for (int j = 0; j < NBPARTICULES_MAX; j++)
                    _particules[_NbParticules++] = _nuages[i]._particules[j];
            
            Array.Sort(_particules, delegate(Particule O1, Particule O2)
                       {
                           if (O1.z > O2.z) return 1;
                           if (O1.z < O2.z) return -1;
                           return 0;
                       });
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }

        /// <summary>
        /// Pression sur une touche, retourner true si l'objet a traite, false = fin de l'economiseur
        /// </summary>
        /// <param name="f"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        ///
        public override bool KeyDown(Form f, Keys k)
        {
            if (k == Keys.I)
            {
                _positionNuage = -_positionNuage;
                conf.setParametre(CAT, "EnHaut", _positionNuage > 0 ? true : false);

                for (int i = 0; i < NB_NUAGES; i++)
                {
                    creerNuage(ref _nuages[i], true);
                }
                return true;
            }
            else
                return false;
        }

        public override void AppendHelpText(StringBuilder s)
        {
            s.Append(Resources.AideNuages);
        }

    }

}

