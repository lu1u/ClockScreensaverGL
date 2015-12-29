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
using System.Collections.Generic;
using GLfloat = System.Single;
using GLuint = System.UInt32;
namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Opengl
{
    /// <summary>
    /// Description of Nuage.
    /// </summary>
    public sealed class NuagesOpenGL : TroisD, IDisposable
    {
        #region PARAMETRES
        const string CAT = "Nuages.OpenGL";
        static readonly float ALPHA = 0.2f;// conf.getParametre(CAT, "Alpha", 0.25f);
        static  bool ADDITIVE = conf.getParametre(CAT, "Additive", false);
        static readonly float VITESSE = conf.getParametre(CAT, "Vitesse", 2f);
        static readonly float RAYON_MAX = conf.getParametre(CAT, "RayonMax", 8f);
        static readonly float RAYON_MIN = conf.getParametre(CAT, "RayonMin", 5f);
        static readonly float TAILLE_PARTICULE = 3f;// conf.getParametre(CAT, "TailleParticules", 2.5f);
        static readonly int NB_NUAGES = conf.getParametre(CAT, "Nb", 10);
        static readonly int MAX_NIVEAU = conf.getParametre(CAT, "NbNiveaux", 6);
        static readonly int NB_EMBRANCHEMENTS = conf.getParametre(CAT, "NbEmbranchements", 3);
        static readonly bool CIEL_DEGRADE = conf.getParametre(CAT, "CielDegrade", true);
        float _positionNuage = conf.getParametre(CAT, "EnHaut", true) ? 1f : -1f;
        private static readonly GLfloat[] fogcolor = { 0, 0, 0, 1 };
        #endregion
        const float VIEWPORT_X = 1f;
        const float VIEWPORT_Y = 1f;
        const float VIEWPORT_Z = 10f;
        readonly int NBPARTICULES_MAX;

        // Une des parties elementaires du nuage
        private class Particule : IComparable
        {
            public float x, y, z, taille, alpha;
            public int type;
            int IComparable.CompareTo(Object o)
            {
                if (o is Particule)
                {
                    Particule O2 = (Particule)o;
                    if (z > O2.z) return 1;
                    if (z < O2.z) return -1;
                }
                return 0;
            }
        }

        // Un nuage
        private sealed class Nuage
        {
            public float x, y, z;
            public List<Particule> _particules;
        }

        private Nuage[] _nuages;					// Pour creer des nuages ressemblants
        private Particule[] _particules;			// Pour l'affichage
        private int _NbParticules;

        const int NB_TEXTURES = 3;
        private Texture[] texture = new Texture[NB_TEXTURES];
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="gl"></param>
        public NuagesOpenGL(OpenGL gl)
            : base(gl, VIEWPORT_X, VIEWPORT_Y, VIEWPORT_Z, 0)
        {
            _nuages = new Nuage[NB_NUAGES];
            texture[0] = new Texture();
            texture[0].Create(gl, Resources.nuage1);
            texture[1] = new Texture();
            texture[1].Create(gl, Resources.nuage2);
            texture[2] = new Texture();
            texture[2].Create(gl, Resources.nuage3);

            for (int i = 0; i < NB_NUAGES; i++)
                creerNuage(ref _nuages[i], true);

            NBPARTICULES_MAX = _NbParticules * NB_NUAGES;
            _particules = new Particule[NBPARTICULES_MAX];
            _NbParticules = 0;
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (Texture t in texture)
                t?.Destroy(_gl);
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
                nuage._particules = new List<Particule>();
            }
            else
                nuage._particules.Clear();

            float rayonNuage = FloatRandom(RAYON_MIN, RAYON_MAX);
            nuage.x = FloatRandom(-_tailleCubeX * 50, _tailleCubeX * 50);
            nuage.y = _positionNuage * _tailleCubeY * FloatRandom(-RAYON_MIN, RAYON_MAX * 2);

            if (init)
                nuage.z = -FloatRandom(-_zCamera, _tailleCubeZ * 10);
            else
                nuage.z = -_tailleCubeZ * 10;

            // Genere le nuage de facon 'fractale'
            _NbParticules = GenereNuage(ref nuage, nuage.x, nuage.y, nuage.z, rayonNuage, TAILLE_PARTICULE, MAX_NIVEAU);
        }

        /// <summary>
        /// Genere recursivement un nuage, pour lui donner son aspect fractal
        /// </summary>
        /// <param name="nuage"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="rayonNuage"></param>
        /// <param name="niveau"></param>
        private int GenereNuage(ref Nuage nuage, float x, float y, float z, float rayonNuage, float tailleParticule, int niveau)
        {
            Particule p = new Particule();
            p.x = x;
            p.y = y;
            p.z = z;
            p.taille = FloatRandom(tailleParticule * 0.75f, tailleParticule * 1.5f);
            p.alpha = FloatRandom(ALPHA * 0.5f, ALPHA * 2.0f);
            p.type = r.Next(0, NB_TEXTURES);
            nuage._particules.Add(p);
            int res = 1;

            if (niveau > 1)
            {
                for (int i = 0; i < NB_EMBRANCHEMENTS; i++)
                {
                    float AngleX = FloatRandom(0, (float)Math.PI * 2);
                    float AngleZ = FloatRandom(0, (float)Math.PI * 2);
                    float distanceCentre = FloatRandom(rayonNuage * 0.2f, rayonNuage);

                    res += GenereNuage(ref nuage,
                                x + distanceCentre * 3.0f * (float)Math.Cos(AngleX),
                                y + distanceCentre * 0.5f * (float)Math.Sin(AngleX),
                                z + distanceCentre * 2.0f * (float)Math.Sin(AngleZ),
                                rayonNuage * 0.5f,
                                tailleParticule * 0.75f,
                                niveau - 1);
                }
            }

            return res;
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
            float[] col = { couleur.R / 512.0f, couleur.G / 512.0f, couleur.B / 512.0f, 1 };
            DessineCiel(gl, col);
            gl.Enable(OpenGL.GL_FOG);
            gl.Fog(OpenGL.GL_FOG_MODE, OpenGL.GL_LINEAR);
            gl.Fog(OpenGL.GL_FOG_COLOR, fogcolor);
            gl.Fog(OpenGL.GL_FOG_DENSITY, 1.0f);
            gl.Fog(OpenGL.GL_FOG_START, VIEWPORT_Z);
            gl.Fog(OpenGL.GL_FOG_END, _zCamera/2);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, ADDITIVE ? OpenGL.GL_ONE :  OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);

            texture[0].Create(gl, Resources.particleTexture);
            texture[1].Create(gl, Resources.particleTexture);
            texture[2].Create(gl, Resources.particleTexture);
            int derniereTexture = -1;
            gl.Begin(OpenGL.GL_QUADS);
            for (int i = 0; i < _NbParticules; i++)
            {
                float taille = _particules[i].taille;

               if (derniereTexture != _particules[i].type)
                {
                    texture[_particules[i].type].Bind(gl);
                    derniereTexture = _particules[i].type;
                }
                
                gl.Color(col[0] * 1.3f, col[1] * 1.3f, col[2] * 1.3f, _particules[i].alpha);
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(_particules[i].x - taille, _particules[i].y - taille, _particules[i].z);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(_particules[i].x - taille, _particules[i].y + taille, _particules[i].z);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(_particules[i].x + taille, _particules[i].y + taille, _particules[i].z);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(_particules[i].x + taille, _particules[i].y - taille, _particules[i].z);
            }
            gl.End();
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

        /// <summary>
        /// Dessine le ciel, degrade ou uni en fonction des performances de la machine
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="col"></param>
        private void DessineCiel(OpenGL gl, float[] col)
        {
            if (CIEL_DEGRADE)
            {
                gl.LoadIdentity();
                gl.Disable(OpenGL.GL_TEXTURE_2D);
                gl.Disable(OpenGL.GL_LIGHTING);
                gl.Disable(OpenGL.GL_COLOR_MATERIAL);
                gl.Disable(OpenGL.GL_DEPTH);
                gl.Disable(OpenGL.GL_FOG);
                gl.DepthMask((byte)OpenGL.GL_FALSE);

                gl.Begin(OpenGL.GL_QUADS);
                {
                    gl.Color(col);
                    gl.Vertex(-0.75f, -0.5f, -1);
                    gl.Vertex(0.75f, -0.5f, -1);
                    gl.Color(0, 0, 0);
                    gl.Vertex(0.75f, 0.5f, -1);
                    gl.Vertex(-0.75f, 0.5f, -1);
                }
                gl.End();
            }
            else
            {
                // Ciel uni
                gl.ClearColor(col[0] / 4, col[1] / 4, col[2] / 4, col[3]);
                gl.LoadIdentity();
                gl.Disable(OpenGL.GL_TEXTURE_2D);
                gl.Disable(OpenGL.GL_LIGHTING);
                gl.Disable(OpenGL.GL_COLOR_MATERIAL);
                gl.Disable(OpenGL.GL_DEPTH);
                gl.Disable(OpenGL.GL_FOG);
                gl.DepthMask((byte)OpenGL.GL_FALSE);
            }
        }


        /// <summary>
        /// Deplacer les nuages
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param
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
                foreach (Particule p in _nuages[i]._particules)
                {
                    p.z += vitesse;

                    if (p.z < _zCamera)
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
            foreach (Nuage n in _nuages)
                foreach (Particule p in n._particules)
                    _particules[_NbParticules++] = p;

            //Array.Sort(_particules, 0, _NbParticules);
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
            if (k == TOUCHE_INVERSER)
            {
                _positionNuage = -_positionNuage;
                conf.setParametre(CAT, "EnHaut", _positionNuage > 0 ? true : false);

                for (int i = 0; i < NB_NUAGES; i++)
                    creerNuage(ref _nuages[i], true);
                return true;
            }
            if ( k == TOUCHE_ADDITIVE)
            {
                ADDITIVE = !ADDITIVE;
                conf.setParametre(CAT, "Additive", ADDITIVE);
                return true;
            }
            else
                return false;
        }

        public override void AppendHelpText(StringBuilder s)
        {
            s.Append(Resources.AideNuages);
        }

#if TRACER
        public override String DumpRender()
        {
            return base.DumpRender() + " NbParticules:" + _NbParticules + " Max:" + NBPARTICULES_MAX;
        }

#endif
    }

}

