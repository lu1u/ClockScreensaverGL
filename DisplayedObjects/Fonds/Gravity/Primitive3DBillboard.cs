using ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity
{
    sealed public class Primitive3DBillboard: Primitive3D
    {
        double _taille;
        public Primitive3DBillboard(OpenGL gl, Vecteur3Ddbl position, double taille) : base(position)
        {
            _taille = taille;
        }

        public override void dessine(OpenGL gl)
        {
            setAttributes(gl);
            Vecteur3Ddbl vPoint0;//= new Vecteur3Ddbl(-1.0f, -1.0f, 0.0f);
            Vecteur3Ddbl vPoint1;//= new Vecteur3Ddbl(1.0f, -1.0f, 0.0f);
            Vecteur3Ddbl vPoint2;//= new Vecteur3Ddbl(1.0f, 1.0f, 0.0f);
            Vecteur3Ddbl vPoint3;//= new Vecteur3Ddbl(-1.0f, 1.0f, 0.0f);
            CalculePoints(gl, _position, _taille, out vPoint0, out vPoint1, out vPoint2, out vPoint3);
            // Dessine un sprite toujours face a la camera
            gl.PushAttrib(OpenGL.GL_TRANSFORM_BIT);


            //---------------------------------------------------------------------
            //
            // vPoint3                vPoint2
            //         +------------+
            //         |            |
            //         |            |
            //         |            |
            //         |     +      |
            //         |  vCenter   |
            //         |            |
            //         |            |
            //         +------------+
            // vPoint0                vPoint1
            //
            //---------------------------------------------------------------------

            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(vPoint0.x, vPoint0.y, vPoint0.z);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(vPoint1.x, vPoint1.y, vPoint1.z);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(vPoint2.x, vPoint2.y, vPoint2.z);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(vPoint3.x, vPoint3.y, vPoint3.z);
            gl.End();

            gl.PopAttrib();
        }

        static void CalculePoints(OpenGL gl, Vecteur3Ddbl Position, double taille, out Vecteur3Ddbl vPoint0, out Vecteur3Ddbl vPoint1, out Vecteur3Ddbl vPoint2, out Vecteur3Ddbl vPoint3)
        {

            float[] mat = new float[16];
            gl.GetFloat(OpenGL.GL_MODELVIEW_MATRIX, mat);

            Vecteur3Ddbl vRight = new Vecteur3Ddbl(mat[0], mat[4], mat[8]);
            Vecteur3Ddbl vUp = new Vecteur3Ddbl(mat[1], mat[5], mat[9]);

            // Now, build a quad around the center point based on the vRight 
            // and vUp vectors. This will guarantee that the quad will be 
            // orthogonal to the view.
            vPoint0 = Position + ((-vRight - vUp) * taille);
            vPoint1 = Position + ((vRight - vUp) * taille);
            vPoint2 = Position + ((vRight + vUp) * taille);
            vPoint3 = Position + ((-vRight + vUp) * taille);
        }
    }
}
