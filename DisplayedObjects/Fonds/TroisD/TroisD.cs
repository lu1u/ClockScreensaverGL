/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 10/01/2015
 * Heure: 11:58
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Drawing;
using SharpGL;
namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD
{
    /// <summary>
    /// Description of _3d.
    /// </summary>
    public abstract class TroisD : Fond
    {
        protected int _largeur, _hauteur;
        protected float _zCamera;
        protected float _tailleCubeX, _tailleCubeY, _tailleCubeZ;
        public const float MAX_COORD = Int32.MaxValue;
        public const float MIN_COORD = Int32.MinValue;
        protected static float FOV ;

        protected TroisD(OpenGL gl, float vpX, float vpY, float vpZ, float zCam): base(gl)
        {
            _tailleCubeX = vpX;
            _tailleCubeY = vpY;
            _tailleCubeZ = vpZ;
            _zCamera = zCam;
            FOV = getConfiguration().getParametre( "FOV", 60.0f, (a) => { FOV = (float) Convert.ToDouble( a ); } );
        }

        
        /*
        static public void RotateAxeY(ref float x, ref float y, ref float z, float Theta, float axeX, float axeZ)
        {
            float CosTheta = (float)Math.Cos(Theta);
            float SinTheta = (float)Math.Sin(Theta);

            float px = CosTheta * (x - axeX) - SinTheta * (z - axeZ) + axeX;
            float pz = SinTheta * (x - axeX) + CosTheta * (z - axeZ) + axeZ;
            x = px;
            z = pz;
        }

        static public void RotateAxeZ(ref float x, ref float y, ref float z, float Theta, float axeX, float axeY)
        {
            float CosTheta = (float)Math.Cos(Theta);
            float SinTheta = (float)Math.Sin(Theta);

            float px = CosTheta * (x - axeX) - SinTheta * (y - axeY) + axeX;
            float py = SinTheta * (x - axeX) + CosTheta * (y - axeY) + axeY;
            x = px;
            y = py;
        }

        static public void RotateAxeZ(Vecteur3D v, float Theta, float axeX, float axeY)
        {
            float CosTheta = (float)Math.Cos(Theta);
            float SinTheta = (float)Math.Sin(Theta);

            float px = CosTheta * (v.x - axeX) - SinTheta * (v.y - axeY) + axeX;
            float py = SinTheta * (v.x - axeX) + CosTheta * (v.y - axeY) + axeY;
            v.x = px;
            v.y = py;
        }
        */
        /*
        protected void DessineCube( Graphics g )
        {
            float X, Y, X1, Y1, X2, Y2, X3, Y3, X4, Y4, X5, Y5, X6, Y6, X7, Y7, X8, Y8 ;
            Coord2DFrom3D( 0, 0,  0, out X, out Y ) ;
            Coord2DFrom3D(0, 0, 100, out X2, out Y2 ) ;
            g.DrawLine( Pens.Green, X, Y, X2, Y2 ) ;
			
            Coord2DFrom3D( 0, 100, 0, out X2, out Y2 ) ;
            g.DrawLine( Pens.Red, X, Y, X2, Y2 ) ;
			
            Coord2DFrom3D( 100, 0, 0, out X2, out Y2 ) ;
            g.DrawLine( Pens.Cyan, X, Y, X2, Y2 ) ;
			
            Coord2DFrom3D( - _tailleCubeX, 	_tailleCubeY, _tailleCubeZ, out X1, out Y1 ) ;
            Coord2DFrom3D(   _tailleCubeX, 	_tailleCubeY, _tailleCubeZ, out X2, out Y2 ) ;
            Coord2DFrom3D(   _tailleCubeX, - _tailleCubeY, _tailleCubeZ, out X3, out Y3 ) ;
            Coord2DFrom3D(  -_tailleCubeX,  -_tailleCubeY, _tailleCubeZ, out X4, out Y4 ) ;
			
            Coord2DFrom3D( - _tailleCubeX, 	_tailleCubeY, 10, out X5, out Y5 ) ;
            Coord2DFrom3D(   _tailleCubeX, 	_tailleCubeY, 10, out X6, out Y6 ) ;
            Coord2DFrom3D(   _tailleCubeX, - _tailleCubeY, 10, out X7, out Y7 ) ;
            Coord2DFrom3D(  -_tailleCubeX,  -_tailleCubeY, 10, out X8, out Y8 ) ;
			
            g.DrawLine( Pens.Aquamarine, X1, Y1, X2, Y2 ) ;
            g.DrawLine( Pens.Aquamarine, X2, Y2, X3, Y3 ) ;
            g.DrawLine( Pens.Aquamarine, X3, Y3, X4, Y4 ) ;
            g.DrawLine( Pens.Aquamarine, X4, Y4, X1, Y1 ) ;
			
			
            g.DrawLine( Pens.Wheat, X5, Y5, X6, Y6 ) ;
            g.DrawLine( Pens.Wheat, X6, Y6, X7, Y7 ) ;
            g.DrawLine( Pens.Wheat, X7, Y7, X8, Y8 ) ;
            g.DrawLine( Pens.Wheat, X8, Y8, X5, Y5 ) ;

            g.DrawLine( Pens.LightSlateGray, X1, Y1, X5, Y5 ) ;
            g.DrawLine( Pens.LightSlateGray, X2, Y2, X6, Y6 ) ;
            g.DrawLine( Pens.LightSlateGray, X3, Y3, X7, Y7 ) ;
            g.DrawLine( Pens.LightSlateGray, X4, Y4, X8, Y8 ) ;
			
            X = 0 ;
            for ( float Z = - _tailleCubeZ; Z < _tailleCubeZ; Z += 10)
            {
                Y = Coord3DFrom2D( -_hauteur, Z, _hauteur ) ;
                Coord2DFrom3D( X, Y, Z, out X1, out Y1 ) ;
                g.FillEllipse( Brushes.Red, X1-8, Y1-8, 16, 16 ) ;
            }

        }
        */
        /*
        protected void DessineCube(OpenGL gl)
        {

            gl.Begin(OpenGL.GL_LINES);

            gl.Vertex(0, 0); gl.Vertex(0, 1);
            gl.Vertex(0, 0); gl.Vertex(1, 0);
            gl.Vertex(0, 0); gl.Vertex(0, 0, 1);
            gl.End();
        }
    */

        protected static Vecteur3D NormaleTriangle(Vecteur3D P1, Vecteur3D P2, Vecteur3D P3)
        {
            Vecteur3D v = new Vecteur3D();
            v.x = (P2.y - P1.y) * (P3.z - P1.z) - (P2.z - P1.z) * (P3.y - P1.y);
            v.y = (P2.z - P1.z) * (P3.x - P1.x) - (P2.x - P1.x) * (P3.z - P1.z);
            v.z = (P2.x - P1.x) * (P3.y - P1.y) - (P2.y - P1.y) * (P3.x - P1.x);
            v.Normalize();
            return v;
        }
        /*
        protected float AngleEntre(Vecteur3D v1, Vecteur3D v2)
        {
            float angle = v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
            return (float)Math.Acos(angle);
        }*/

        /// <summary>
        /// Change le zoom de la camera
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="FOV"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="nearPlane">ATTENTION Bug! ne marche pas si nearplane = 0</param>
        /// <param name="farPlane"></param>
        protected static void changeZoom(OpenGL gl, int width, int height, float nearPlane, float farPlane)
        {
            gl.Viewport(0, 0, width, height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(FOV, (float)width / height, nearPlane, farPlane);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

    }


}
