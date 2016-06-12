using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Grilles
{
    abstract class GrilleBase : MateriauGlobal, IDisposable
    {
        #region Membres
        private uint _genLists;
        float _angleVue = FloatRandom(0, 6.28f);
        protected static readonly float[] fogcolor = { 0, 0, 0, 1 };
        protected float fogEnd ;
        protected float VITESSE_ROTATION;
        protected float TRANSLATE_Z;
        #endregion

        public GrilleBase(OpenGL gl, string CAT): base(gl, CAT)
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

        protected void Cube(OpenGL gl, float dx, float dy, float dz, float tX, float tY, float tZ)
        {
            //Bas
            gl.Begin(OpenGL.GL_QUADS);
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

            gl.End();
        }

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            //float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1f };
            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_ALPHA_TEST);
            gl.DepthMask((byte)OpenGL.GL_TRUE);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.CullFace(OpenGL.GL_BACK);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            
            gl.Enable(OpenGL.GL_FOG);
            gl.Fog(OpenGL.GL_FOG_MODE, OpenGL.GL_LINEAR);
            gl.Fog(OpenGL.GL_FOG_COLOR, fogcolor);
            gl.Fog(OpenGL.GL_FOG_DENSITY, 0.2f);
            gl.Fog(OpenGL.GL_FOG_START, 0);
            gl.Fog(OpenGL.GL_FOG_END, fogEnd);
            setGlobalMaterial(gl, couleur);
            // Aspect de la surface

            gl.Translate(0, 0, TRANSLATE_Z );
            gl.Rotate(_angleVue / 2.0f, _angleVue, _angleVue / 3.0f);
            gl.CallList(_genLists);

            fillConsole(gl);

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
