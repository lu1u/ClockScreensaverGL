using ClockScreenSaverGL.Config;
using SharpGL;
using System;
using System.Drawing;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Grilles
{
    /// <summary>
    /// Classe de base pour un objet statique, avec une rotation
    /// Principe: on genere une callList (dans la methode GenererListe)
    /// </summary>
    abstract class GrilleBase : MateriauGlobal, IDisposable
    {
        #region Membres
        private uint _genLists;
        float _angleVue = FloatRandom(0, 6.28f);
        protected static readonly float[] fogcolor = { 0, 0, 0, 1 };
        protected float fogEnd;
        protected float VITESSE_ROTATION;
        protected float TRANSLATE_Z;
        #endregion

        public GrilleBase(OpenGL gl, CategorieConfiguration CAT) : base(gl)
        {
            _genLists = gl.GenLists(1);
            gl.NewList(_genLists, OpenGL.GL_COMPILE);
            GenererListe(gl);
            gl.EndList();
        }
        public override void Dispose()
        {
            base.Dispose();
            _gl.DeleteLists(_genLists, 1);
        }
        protected abstract void GenererListe(OpenGL gl);

        /// <summary>
        /// Genere quatres faces d'un parallelepipede
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        /// <param name="tX"></param>
        /// <param name="tY"></param>
        /// <param name="tZ"></param>
        protected static void Brique(OpenGL gl, float dx, float dy, float dz, float tX, float tY, float tZ)
        {
            //Bas
            gl.Normal(0.0f, -1.0f, 0.0f);
            gl.Vertex(dx - tX, dy - tY, dz + tZ);
            gl.Vertex(dx - tX, dy - tY, dz - tZ);
            gl.Vertex(dx + tX, dy - tY, dz - tZ);
            gl.Vertex(dx + tX, dy - tY, dz + tZ);


            // Haut
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.Vertex(dx + tX, dy + tY, dz + tZ);
            gl.Vertex(dx + tX, dy + tY, dz - tZ);
            gl.Vertex(dx - tX, dy + tY, dz - tZ);
            gl.Vertex(dx - tX, dy + tY, dz + tZ);

            // Droite
            gl.Normal(1.0f, 0.0f, 0.0f);
            gl.Vertex(dx + tX, dy - tY, dz + tZ);
            gl.Vertex(dx + tX, dy - tY, dz - tZ);
            gl.Vertex(dx + tX, dy + tY, dz - tZ);
            gl.Vertex(dx + tX, dy + tY, dz + tZ);

            // Gauche
            gl.Normal(-1.0f, 0.0f, 0.0f);
            gl.Vertex(dx - tX, dy + tY, dz + tZ);
            gl.Vertex(dx - tX, dy + tY, dz - tZ);
            gl.Vertex(dx - tX, dy - tY, dz - tZ);
            gl.Vertex(dx - tX, dy - tY, dz + tZ);

            // Devant
            gl.Normal(0.0f, 0.0f, -1.0f);
            gl.Vertex(dx + tX, dy + tY, dz - tZ);
            gl.Vertex(dx + tX, dy - tY, dz - tZ);
            gl.Vertex(dx - tX, dy - tY, dz - tZ);
            gl.Vertex(dx - tX, dy + tY, dz - tZ);

            // Derriere
            gl.Normal(0.0f, 0.0f, 1.0f);
            gl.Vertex(dx + tX, dy - tY, dz + tZ);
            gl.Vertex(dx + tX, dy + tY, dz + tZ);
            gl.Vertex(dx - tX, dy + tY, dz + tZ);
            gl.Vertex(dx - tX, dy - tY, dz + tZ);
        }

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_ALPHA_TEST);
            gl.DepthMask((byte)OpenGL.GL_TRUE);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.CullFace(OpenGL.GL_BACK);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.Enable(OpenGL.GL_FOG);
            gl.Fog(OpenGL.GL_FOG_MODE, OpenGL.GL_LINEAR);
            gl.Fog(OpenGL.GL_FOG_COLOR, fogcolor);
            gl.Fog(OpenGL.GL_FOG_DENSITY, 0.2f);
            gl.Fog(OpenGL.GL_FOG_START, 0);
            gl.Fog(OpenGL.GL_FOG_END, fogEnd);
            setGlobalMaterial(gl, couleur);

            gl.Translate(0, 0, TRANSLATE_Z);
            gl.Rotate(_angleVue / 2.0f, _angleVue, _angleVue / 3.0f);
            gl.CallList(_genLists);
            
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            _angleVue += VITESSE_ROTATION * maintenant._intervalle;
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }

    }
}
