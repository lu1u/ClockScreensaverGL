﻿using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Drawing;

using GLfloat = System.Single;
using GLuint = System.UInt32;
namespace ClockScreenSaverGL.Fonds.TroisD.Opengl
{
    /// <summary>
    /// Description of Neige.
    /// </summary>
    public sealed class EspaceOpenGL : TroisD
    {
        #region Parametres
        public const string CAT = "Espace.OpenGL";

        private static readonly byte ALPHA = conf.getParametre(CAT, "Alpha", (byte)10);
        private static readonly float TAILLE_ETOILE = conf.getParametre(CAT, "Taille", 0.15f);
        private static readonly int NB_ETOILES = conf.getParametre(CAT, "NbEtoiles", 2000);
        private static readonly float PERIODE_TRANSLATION = conf.getParametre(CAT, "PeriodeTranslation", 13.0f);
        private static readonly float PERIODE_ROTATION = conf.getParametre(CAT, "PeriodeRotation", 10.0f);
        private static readonly float VITESSE_ROTATION = conf.getParametre(CAT, "VitesseRotation", 50f);
        private static readonly float VITESSE_TRANSLATION = conf.getParametre(CAT, "VitesseTranslation", 0.2f);
        private static readonly float VITESSE = conf.getParametre(CAT, "Vitesse", 8f);
        #endregion
        const float VIEWPORT_X = 5f;
        const float VIEWPORT_Y = 5f;
        const float VIEWPORT_Z = 5f;
        private static readonly GLfloat[] fogcolor = { 0, 0, 0, 1 };

        private class Etoile
        {
            public float x, y, z;
            public bool aSupprimer;
        }
        private readonly Etoile[] _etoiles;
        private DateTime _dernierDeplacement = DateTime.Now;
        private DateTime _debutAnimation = DateTime.Now;
        Texture text = new Texture();

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="gl"></param>
        public EspaceOpenGL(OpenGL gl)
            : base(VIEWPORT_X, VIEWPORT_Y, VIEWPORT_Z, 100)
        {
            _etoiles = new Etoile[NB_ETOILES];
            text.Create(gl, Resources.particleTexture);

            // Initialiser les etoiles
            for (int i = 0; i < NB_ETOILES; i++)
            {
                NouvelleEtoile(ref _etoiles[i]);
                // Au debut, on varie la distance des etoiles
                _etoiles[i].z = FloatRandom(-VIEWPORT_Z, _zCamera);
            }
        }

        private void NouvelleEtoile(ref Etoile f)
        {
            if (f == null)
                f = new Etoile();

            f.aSupprimer = false;
            f.x = FloatRandom(-VIEWPORT_X * 6, VIEWPORT_X * 6);
            f.z = -VIEWPORT_Z;
            f.y = FloatRandom(-VIEWPORT_Y * 6, VIEWPORT_Y * 6);
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
            gl.Disable(OpenGL.GL_DEPTH);

            gl.Enable(OpenGL.GL_FOG);
            gl.Fog(OpenGL.GL_FOG_MODE, OpenGL.GL_LINEAR);
            gl.Fog(OpenGL.GL_FOG_COLOR, fogcolor);
            gl.Fog(OpenGL.GL_FOG_DENSITY, 0.1f);
            gl.Fog(OpenGL.GL_FOG_START, VIEWPORT_Z * 1);
            gl.Fog(OpenGL.GL_FOG_END, _zCamera);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);

            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Translate(0, 0, -_zCamera);
            gl.Rotate(0, 0, vitesseCamera);

            float[] col = { couleur.R / 512.0f, couleur.G / 512.0f, couleur.B / 512.0f, 1 };
            text.Bind(gl);
            gl.Color(col);
            gl.Begin(OpenGL.GL_QUADS);
            foreach (Etoile o in _etoiles)
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(o.x - TAILLE_ETOILE, o.y - TAILLE_ETOILE, o.z);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(o.x - TAILLE_ETOILE, o.y + TAILLE_ETOILE, o.z);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(o.x + TAILLE_ETOILE, o.y + TAILLE_ETOILE, o.z);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(o.x + TAILLE_ETOILE, o.y - TAILLE_ETOILE, o.z);
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
            // Deplace les flocons
            bool trier = false;
            for (int i = 0; i < NB_ETOILES; i++)
            {
                if (_etoiles[i].aSupprimer)
                {
                    NouvelleEtoile(ref _etoiles[i]);
                    trier = true;
                }
                else
                {
                    if (_etoiles[i].z > _zCamera)
                        _etoiles[i].aSupprimer = true;
                    else
                    {
                        _etoiles[i].z += deltaZ;
                        _etoiles[i].x += deltaWind;
                    }
                }
            }

            if (trier)
                Array.Sort(_etoiles, delegate(Etoile O1, Etoile O2)
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
