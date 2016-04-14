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
    public class NuagesOpenGL : TroisD, IDisposable
    {
        #region PARAMETRES
        const string CAT = "Nuages.OpenGL";
        static readonly float ALPHA = 0.04f;// conf.getParametre(CAT, "Alpha", 0.25f);
        static bool ADDITIVE = conf.getParametre(CAT, "Additive", false);
        static readonly float VITESSE = 0.5f;// conf.getParametre(CAT, "Vitesse", 2f);
        static readonly float RAYON_MAX = 0.25f;// conf.getParametre(CAT, "RayonMax", 8f);
        static readonly float RAYON_MIN = 0.15f;// conf.getParametre(CAT, "RayonMin", 5f);
        static readonly float TAILLE_PARTICULE = 0.15f;// conf.getParametre(CAT, "TailleParticules", 2.5f);
        static readonly int NB_NUAGES = conf.getParametre(CAT, "Nb", 10);
        static readonly int MAX_NIVEAU = conf.getParametre(CAT, "NbNiveaux", 6);
        static readonly int NB_EMBRANCHEMENTS = 6;// conf.getParametre(CAT, "NbEmbranchements", 3);
        static readonly bool CIEL_DEGRADE = conf.getParametre(CAT, "CielDegrade", true);
        float POSITION_NUAGE = conf.getParametre(CAT, "EnHaut", true) ? 1f : -1f;
        private static readonly GLfloat[] fogcolor = { 0, 0, 0, 1 };
        #endregion
        const float VIEWPORT_X = 0.5f;
        const float VIEWPORT_Y = 0.5f;
        const float VIEWPORT_Z = 5f;

        // Un nuage
        private sealed class Nuage
        {
            public float x, y, z;
            public float zmin;
            public uint iList;
            public int iTexture;
        }

        private Nuage[] _nuages;					// Pour creer des nuages ressemblants
        uint _iGenList;

        const int NB_TEXTURES = 3;
        private Texture[] texture = new Texture[NB_TEXTURES];
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="gl"></param>
        public NuagesOpenGL(OpenGL gl)
            : base(gl, VIEWPORT_X, VIEWPORT_Y, VIEWPORT_Z, 0.5f)
        {
            _nuages = new Nuage[NB_NUAGES];
            texture[0] = new Texture();
            texture[0].Create(gl, Resources.nuage1);
            texture[1] = new Texture();
            texture[1].Create(gl, Resources.nuage2);
            texture[2] = new Texture();
            texture[2].Create(gl, Resources.nuage3);
            
            _iGenList = gl.GenLists(NB_NUAGES);
            for (uint i = 0; i < NB_NUAGES; i++)
            {
                _nuages[i] = new Nuage();
                creerNuage(i, true);
            }
            //NBPARTICULES_MAX = _NbParticules * NB_NUAGES;
            //_particules = new Particule[NBPARTICULES_MAX];
            //_NbParticules = 0;
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (Texture t in texture)
                t?.Destroy(_gl);

            _gl.DeleteLists(_iGenList, NB_NUAGES);
        }

        /// <summary>
        /// Creer un nuage
        /// </summary>
        /// <param name="nuage"></param>
        /// <param name="init">true s'il s'agit de la creation initiale, false si c'est un
        /// recyclage d'un nuage qui est passe derriere la camera</param>
        void creerNuage(uint i, bool init)
        {
            _nuages[i].iList = _iGenList + i;

            _nuages[i].x = FloatRandom(-VIEWPORT_X, VIEWPORT_X) * 10.0f;
            _nuages[i].y = FloatRandom(0, VIEWPORT_Y) * 4.0f * POSITION_NUAGE;
            _nuages[i].z = FloatRandom(-VIEWPORT_Z, _zCamera) ;
            _nuages[i].zmin = _nuages[i].z;
            _nuages[i].iTexture = r.Next(0, NB_TEXTURES);
            // Genere le nuage de facon 'fractale'
            float rayonNuage = FloatRandom(RAYON_MIN, RAYON_MAX);
            _gl.NewList(_nuages[i].iList, OpenGL.GL_COMPILE);
            GenereNuage(ref _nuages[i], rayonNuage, 0, 0, 0, TAILLE_PARTICULE, MAX_NIVEAU);
            _gl.EndList();
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
        private void GenereNuage(ref Nuage nuage, float rayonNuage, float x, float y, float z, float tailleParticule, int niveau)
        {
            float taille = FloatRandom(tailleParticule * 0.75f, tailleParticule * 1.5f);
            float alpha = FloatRandom(ALPHA * 0.5f, ALPHA * 2.0f);
            
            texture[r.Next(0, NB_TEXTURES)].Bind(_gl);
            _gl.TexCoord(0.0f, 0.0f); _gl.Vertex(x - taille, y - taille, z);
            _gl.TexCoord(0.0f, 1.0f); _gl.Vertex(x - taille, y + taille, z);
            _gl.TexCoord(1.0f, 1.0f); _gl.Vertex(x + taille, y + taille, z);
            _gl.TexCoord(1.0f, 0.0f); _gl.Vertex(x + taille, y - taille, z);
            if (z < nuage.zmin)
                nuage.zmin = z;

            if (niveau > 1)
            {
                for (int i = 0; i < NB_EMBRANCHEMENTS; i++)
                {
                    float AngleX = FloatRandom(0, (float)Math.PI * 2);
                    float AngleZ = FloatRandom(0, (float)Math.PI * 2);
                    float distanceCentre = FloatRandom(rayonNuage * 0.5f, rayonNuage);

                    GenereNuage(ref nuage, rayonNuage * 0.75f,
                                x + distanceCentre * 3.0f * (float)Math.Cos(AngleX),
                                y + distanceCentre * 0.9f * (float)Math.Sin(AngleX),
                                z + distanceCentre *2.0f * (float)Math.Sin(AngleZ),
                                tailleParticule * 0.75f,
                                niveau - 1);
                }
            }


        }

        public override void ClearBackGround(OpenGL gl, Color couleur)
        {
            float[] col = { couleur.R / 512.0f, couleur.G / 512.0f, couleur.B / 512.0f, 1 };
            if (CIEL_DEGRADE)
            {
                gl.LoadIdentity();
                gl.Disable(OpenGL.GL_TEXTURE_2D);
                gl.Disable(OpenGL.GL_LIGHTING);
                gl.Disable(OpenGL.GL_COLOR_MATERIAL);
                gl.Disable(OpenGL.GL_DEPTH);
                gl.Disable(OpenGL.GL_FOG);
                gl.DepthMask((byte)OpenGL.GL_FALSE);

                gl.PushMatrix();
                gl.MatrixMode(OpenGL.GL_PROJECTION);
                gl.PushMatrix();
                gl.LoadIdentity();
                gl.Ortho2D(0.0, 1.0, 0.0, 1.0);
                gl.MatrixMode(OpenGL.GL_MODELVIEW);

                gl.Begin(OpenGL.GL_QUADS);
                {
                    gl.Color(col);
                    gl.Vertex(-1f, -1f, -1);
                    gl.Vertex(1f, -1f, -1);
                    gl.Color(0, 0, 0);
                    gl.Vertex(1f, 1f, -1);
                    gl.Vertex(-1f, 1f, -1);
                }
                gl.End();

                gl.MatrixMode(OpenGL.GL_PROJECTION);
                gl.PopMatrix();
                gl.MatrixMode(OpenGL.GL_MODELVIEW);
                gl.PopMatrix();
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
            
            
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, ADDITIVE ? OpenGL.GL_ONE : OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            
            gl.Enable(OpenGL.GL_FOG);
            gl.Fog(OpenGL.GL_FOG_MODE, OpenGL.GL_LINEAR);
            gl.Fog(OpenGL.GL_FOG_COLOR, fogcolor);
            gl.Fog(OpenGL.GL_FOG_DENSITY, 0.0f);
            gl.Fog(OpenGL.GL_FOG_END, -VIEWPORT_Z);
            gl.Fog(OpenGL.GL_FOG_START, 0);

            gl.Translate(0, 0, -_zCamera);
            gl.Color(couleur.R * 3.0f / 256.0f, couleur.G * 3.0f / 256.0f, couleur.B * 3.0f / 256.0f, ALPHA);

            //float taille = 1;
            for (uint i = 0; i < NB_NUAGES; i++)
            {
                gl.PushMatrix();
                gl.Translate(_nuages[i].x, _nuages[i].y, _nuages[i].z);
                //texture[_nuages[i].iTexture].Bind(gl);

                gl.Begin(OpenGL.GL_QUADS);
                gl.CallList(_nuages[i].iList);
                gl.End();
                gl.PopMatrix();
            }


#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
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
            bool trierNuages = false;

            for (int i = 0; i < NB_NUAGES; i++)
            {
                _nuages[i].z += vitesse;
                if (_nuages[i].zmin + _nuages[i].z > _zCamera)

                {
                    // Aucune partie du nuage devant la camera: le recreer au loin
                    _nuages[i].z = -VIEWPORT_Z;
                    _nuages[i].x = FloatRandom(-VIEWPORT_X, VIEWPORT_X) * 10.0f;
                    _nuages[i].y = FloatRandom(0, VIEWPORT_Y) * POSITION_NUAGE;
                    trierNuages = true;
                }
            }

            if (trierNuages)
            {
                Array.Sort(_nuages, delegate (Nuage O1, Nuage O2)
                           {
                               if (O1.z > O2.z) return 1;
                               if (O1.z < O2.z) return -1;
                               return 0;
                           });
            }


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
                POSITION_NUAGE = -POSITION_NUAGE;
                conf.setParametre(CAT, "EnHaut", POSITION_NUAGE > 0 ? true : false);

                //for (int i = 0; i < NB_NUAGES; i++)
                //  creerNuage(r _nuages[i], true);
                return true;
            }
            if (k == TOUCHE_ADDITIVE)
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
            return base.DumpRender() + " NbNuages:" + NB_NUAGES ;
        }

#endif
    }

}

