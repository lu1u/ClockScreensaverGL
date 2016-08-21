using ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity
{
    class C3D
    {

        //////////////////////////////////////////////////////////////////////
        // Gestion de la camera
        //////////////////////////////////////////////////////////////////////
        public static void Camera(OpenGL gl, Rectangle tailleEcran, Vecteur3D From, Vecteur3D To, float Angle)
        {
             //gl.Viewport(0, 0, Largeur, Hauteur);         // Reset The Current Viewport
            gl.MatrixMode(OpenGL.GL_PROJECTION);                        // Select The Projection Matrix
            gl.LoadIdentity();                                   // Reset The Projection Matrix

            // Calculate The Aspect Ratio Of The Window
            gl.Perspective(Angle, (float)tailleEcran.Width / (float)tailleEcran.Height, 1.0f, Gravitation.DistanceMax * 3);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);                         // Select The Modelview Matrix
            gl.LoadIdentity();                                   // Reset The Modelview Matrix


            gl.LookAt(From.x, From.y, From.z, To.x, To.y, To.z,
                        0, 1, 0);
            //CalculeChampDeVision();
        }

        //////////////////////////////////////////////////////////////////////
        // Dessine une croix ( axes X, Y et Z ) autour d'une position donnee
        // ENTREES:	Position de la croix
        //			Taille de la croix
        ///////////////////////////////////////////////////////////////////////////////
        public static void DessineCroix(OpenGL gl, Vecteur3D Position, float Taille, Color Couleur)
        {
            gl.PushAttrib(OpenGL.GL_ENABLE_BIT);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Color(Couleur.R / 256.0f, Couleur.G / 256.0f, Couleur.B / 256.0f);
            gl.LineWidth(2);
            {
                gl.PushMatrix();
                gl.Translate(Position.x, Position.y, Position.z);
                {
                    gl.Begin(OpenGL.GL_LINES);
                    // X: vert
                    gl.Color(0, 1.0f, 0);
                    gl.Vertex(-Taille, 0, 0);
                    gl.Vertex(Taille, 0, 0);

                    // Y: rouge
                    gl.Color( 1.0f, 0, 0);
                    gl.Vertex(0, -Taille, 0);
                    gl.Vertex(0, Taille, 0);

                    // Z: rouge
                    gl.Color(0, 0, 1.0f);
                    gl.Vertex(0, 0, -Taille);
                    gl.Vertex(0, 0, Taille);
                    gl.End();
                }
                gl.PopMatrix();
            }
            gl.PopAttrib();
        }

        public static void DessineBillboard(OpenGL gl, Vecteur3D position, float taille)
        {

            Vecteur3D vPoint0 ;//= new Vecteur3D(-1.0f, -1.0f, 0.0f);
            Vecteur3D vPoint1 ;//= new Vecteur3D(1.0f, -1.0f, 0.0f);
            Vecteur3D vPoint2 ;//= new Vecteur3D(1.0f, 1.0f, 0.0f);
            Vecteur3D vPoint3 ;//= new Vecteur3D(-1.0f, 1.0f, 0.0f);
            CalculePoints(gl, position, taille, out vPoint0, out vPoint1, out vPoint2, out vPoint3);
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
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(vPoint0.x, vPoint0.y, vPoint0.z);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(vPoint1.x, vPoint1.y, vPoint1.z);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(vPoint2.x, vPoint2.y, vPoint2.z);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(vPoint3.x, vPoint3.y, vPoint3.z);
            gl.End();

            gl.PopAttrib();
        }

        static void CalculePoints(OpenGL gl, Vecteur3D Position, float taille, out Vecteur3D vPoint0, out Vecteur3D vPoint1, out Vecteur3D vPoint2, out Vecteur3D vPoint3)
        {

            float[] mat = new float[16];
            gl.GetFloat(OpenGL.GL_MODELVIEW_MATRIX, mat);

            Vecteur3D vRight = new Vecteur3D(mat[0], mat[4], mat[8]);
            Vecteur3D vUp = new Vecteur3D(mat[1], mat[5], mat[9]);

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
