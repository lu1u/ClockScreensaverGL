using ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity
{
    class Primitive3DQuadrilatere : Primitive3D
    {
        Vecteur3Ddbl p1, p2, p3, p4;
        public Primitive3DQuadrilatere(OpenGL gl, Vecteur3Ddbl P1, Vecteur3Ddbl P2, Vecteur3Ddbl P3, Vecteur3Ddbl P4) : base((P1 + P2 + P3 + P4) / 4.0f)
        {
            p1 = P1;
            p2 = P2;
            p3 = P3;
            p4 = P4;
        }

        public override void dessine(OpenGL gl)
        {
            if (isVisible(p1,p2,p3,p4))
            {
                setAttributes(gl);
                gl.Begin(OpenGL.GL_QUADS);
                if (TEXTURE!=null)
                {
                    gl.TexCoord(0.0f, 1.0f); gl.Vertex(p1.tabd);
                    gl.TexCoord(0.0f, 0.01f); gl.Vertex(p2.tabd);
                    gl.TexCoord(1.0f, 0.01f); gl.Vertex(p3.tabd);
                    gl.TexCoord(1.0f, 1.0f); gl.Vertex(p4.tabd);
                }
                else
                {
                    gl.Vertex(p1.tabd);
                    gl.Vertex(p2.tabd);
                    gl.Vertex(p3.tabd);
                    gl.Vertex(p4.tabd);
                }
                gl.End();
            }
        }
    }
}
