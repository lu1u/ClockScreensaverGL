using SharpGL;
using SharpGL.SceneGraph.Assets;
/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 30/12/2014
 * Heure: 23:08
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Drawing;
using GLfloat = System.Single;

namespace ClockScreenSaverGL.Fonds.TroisD.Opengl
{
    /// <summary>
    /// Description of Neige.
    /// </summary>
    public sealed class Automne : TroisD
    {
        #region Parametres
        public const string CAT = "Automne.OpenGL";

        private static readonly float VITESSE_ROTATION = 0.5f;// conf.getParametre(CAT, "VitesseRotation", 0.2f);
        private static readonly float PERIODE_ROTATION = 10.0f;// conf.getParametre(CAT, "PeriodeRotation", 20.0f);
        private static readonly float VITESSE_Y = 8.0f;// conf.getParametre(CAT, "VitesseChute", 8);
        private static readonly float VITESSE_DELTA_VENT = conf.getParametre(CAT, "VitesseDeltaVent", 1f);
        private static readonly float MAX_VENT = conf.getParametre(CAT, "MaxVent", 3f);
        private static readonly float ALPHA = 1.0f;// conf.getParametre(CAT, "Alpha", 0.9f);
        private readonly int NB_FEUILLES = 100;// conf.getParametre(CAT, "NbFeuilles", 100);
        private readonly float TAILLE_FEUILLE = 3;// conf.getParametre(CAT, "TailleFeuilles", 4.0f);
        private readonly float DIEDRE_FEUILLE = conf.getParametre(CAT, "DiedreFeuilles", 4.0f);
        private readonly float NB_FACES_FEUILLES = 4;// conf.getParametre(CAT, "Nb Faces", 4);
        #endregion

        sealed private class Feuille
        {
            public float x, y, z;
            public float vx, vy, vz;
            public float ax, ay, az;
            public float diedre;
            public int type;
        }

        private readonly Feuille[] _feuilles;

        private float _xWind = 0;

        static float _xRotation;
        static DateTime debut = DateTime.Now;
        const float VIEWPORT_X = 1f;
        const float VIEWPORT_Y = 1f;
        const float VIEWPORT_Z = 1f;
        
        const int NB_TYPES_FEUILLES = 5;
        Texture[] texture = new Texture[NB_TYPES_FEUILLES];

        public Automne(OpenGL gl)
            : base(VIEWPORT_X, VIEWPORT_Y, VIEWPORT_Z, 100)
        {
            _xRotation = _tailleCubeX * 0.75f;

            _feuilles = new Feuille[NB_FEUILLES];
            for (int i = 0; i < NB_FEUILLES; i++)
            {
                _feuilles[i] = new Feuille();

                _feuilles[i].x = FloatRandom(-_tailleCubeX * 50, _tailleCubeX * 50);
                _feuilles[i].z = FloatRandom(-_tailleCubeZ * 2, _zCamera);
                _feuilles[i].y = FloatRandom(-_tailleCubeY * 16, _tailleCubeY * 16);

                _feuilles[i].vx = FloatRandom(-0.1f, 0.1f);
                _feuilles[i].vy = FloatRandom(VITESSE_Y * 0.75f, VITESSE_Y * 1.5f);
                _feuilles[i].vz = FloatRandom(-0.1f, 0.1f);

                _feuilles[i].ax = FloatRandom(0, 360);
                _feuilles[i].ay = FloatRandom(0, 360);
                _feuilles[i].az = FloatRandom(0, 360);
                _feuilles[i].type = r.Next(0, NB_TYPES_FEUILLES);

                _feuilles[i].diedre = FloatRandom(DIEDRE_FEUILLE * 0.5f, DIEDRE_FEUILLE * 2.0f);
            }

            texture[0] = new Texture();
            texture[0].Create(gl, Resources.automne_feuille1);
            texture[1] = new Texture();
            texture[1].Create(gl, Resources.automne_feuille2);
            texture[2] = new Texture();
            texture[2].Create(gl, Resources.automne_feuille3);
            texture[3] = new Texture();
            texture[3].Create(gl, Resources.automne_feuille4);
            texture[4] = new Texture();
            texture[4].Create(gl, Resources.automne_feuille5);
        }

        private void NouvelleFeuille(ref Feuille f)
        {
            if (f == null)
                f = new Feuille();

            f.x = FloatRandom(-_tailleCubeX * 50, _tailleCubeX * 50);
            f.z = FloatRandom(-_tailleCubeZ * 2, _zCamera);
            f.y = VIEWPORT_Y * 16;

            f.vx = FloatRandom(-0.1f, 0.1f);
            f.vy = FloatRandom(VITESSE_Y * 0.75f, VITESSE_Y * 1.5f);
            f.vz = FloatRandom(-0.1f, 0.1f);

            f.ax = FloatRandom(0, 360);
            f.ay = FloatRandom(0, 360);
            f.az = FloatRandom(0, 360);
            f.type = r.Next(0, NB_TYPES_FEUILLES);
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
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, ALPHA };
            GLfloat[] fogcolor = { couleur.R / 2048.0f, couleur.G / 2048.0f, couleur.B / 2048.0f, 0.5f };

            gl.ClearColor(fogcolor[0], fogcolor[1], fogcolor[2], fogcolor[3]);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.Enable(OpenGL.GL_FOG);
            gl.Fog(OpenGL.GL_FOG_MODE, OpenGL.GL_EXP);
            gl.Fog(OpenGL.GL_FOG_COLOR, fogcolor);
            gl.Fog(OpenGL.GL_FOG_DENSITY, 0.02f);
            gl.Fog(OpenGL.GL_FOG_START, _tailleCubeZ );
            gl.Fog(OpenGL.GL_FOG_END, _tailleCubeZ * 60);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);

            gl.LoadIdentity();
            gl.Translate(0, 0, -_zCamera);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_DEPTH);
            gl.Disable(OpenGL.GL_CULL_FACE);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Color(col);
            
            int derniereTexture = -1;

            foreach (Feuille o in _feuilles)
            {
                if (derniereTexture != o.type)
                {
                    texture[o.type].Bind(gl);
                    derniereTexture = o.type;
                }

                gl.PushMatrix();
                gl.Translate(o.x, o.y-1f, o.z);
                gl.Rotate(o.ax, o.ay, o.az);
                
                gl.Begin(OpenGL.GL_QUAD_STRIP);
                {
                    for ( int i = 0; i <= NB_FACES_FEUILLES; i++)
                    {
                        float f = i / (float)NB_FACES_FEUILLES;
                        float d = o.diedre * (float)Math.Cos((double)f * Math.PI / 2.0);
                        gl.TexCoord(f, 0.0f);   gl.Vertex(f*TAILLE_FEUILLE*2, -TAILLE_FEUILLE, d);
                        gl.TexCoord(f, 1.0f );  gl.Vertex(f*TAILLE_FEUILLE*2, TAILLE_FEUILLE, d);
                    }
                }
                gl.End();
                gl.PopMatrix();
            }

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
            float depuisdebut = (float)(debut.Subtract(maintenant._temps).TotalMilliseconds / 1000.0);
            float vitesseCamera = (float)Math.Sin(depuisdebut / PERIODE_ROTATION) * VITESSE_ROTATION;
            float vitesseRot = maintenant._intervalle * 100;

            float CosTheta = (float)Math.Cos(vitesseCamera * maintenant._intervalle);
            float SinTheta = (float)Math.Sin(vitesseCamera * maintenant._intervalle);
            float px, pz;
            bool trier = false;
            // Deplace les flocons
            for (int i = 0; i < NB_FEUILLES; i++)
            {
                if (_feuilles[i].y < -VIEWPORT_Y * 12)
                {
                    NouvelleFeuille(ref _feuilles[i]);
                    trier = true;
                }
                else
                {
                    // Deplacement
                    _feuilles[i].x += ((_feuilles[i].vx + _xWind) * maintenant._intervalle);
                    _feuilles[i].y -= (_feuilles[i].vy * maintenant._intervalle);
                    _feuilles[i].z += (_feuilles[i].vz * maintenant._intervalle);

                    // Variation de vitesse
                    Varie(ref _feuilles[i].vx, -1, 1, 10, maintenant._intervalle);
                    Varie(ref _feuilles[i].vz, -1, 1, 10, maintenant._intervalle);
                    // Rotation due a la position de la camera
                    px = (CosTheta * (_feuilles[i].x - _xRotation)) - (SinTheta * _feuilles[i].z) + _xRotation;
                    pz = (SinTheta * (_feuilles[i].x - _xRotation)) + (CosTheta * _feuilles[i].z);

                    _feuilles[i].x = px;
                    _feuilles[i].z = pz;
                    _feuilles[i].ax += vitesseRot;
                    _feuilles[i].ay += vitesseRot;
                    _feuilles[i].az += vitesseRot;
                }
            }


            Varie(ref _xWind, -MAX_VENT, MAX_VENT, VITESSE_DELTA_VENT, maintenant._intervalle);
            Varie(ref _xRotation, -_tailleCubeX / 2, _tailleCubeX / 2, 10, maintenant._intervalle);

            if (trier)
                Array.Sort(_feuilles, delegate(Feuille O1, Feuille O2)
                {
                    if (O1.z > O2.z) return 1;
                    if (O1.z < O2.z) return -1;
                    return 0;
                });
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }
    }
}
