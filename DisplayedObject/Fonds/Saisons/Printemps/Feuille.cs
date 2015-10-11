using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ClockScreenSaverGL.DisplayedObject.Fonds.Printemps
{
    class Feuille
    {
        static Random r = new Random();
        const int NB_TYPES_FEUILLES =2;
        static int TYPE_FEUILLES = DisplayedObject.r.Next(0, 2);
        static Bitmap _b = TYPE_FEUILLES == 0 ? Resources.feuille1 :  Resources.feuille2 ;
        public Vector3 Position { get; set; }
        public float _taille;
        private static Texture _texture;
        private float _angle;


        static Feuille()
            {
            InitTexture();
            }
        public Feuille(Vector3 position)
        {
            Position = position;
            _taille = 20;
            _angle = r.Next(-45, 90);
        }

        public static void InitTexture()
        {
            TYPE_FEUILLES = DisplayedObject.r.Next(0, 2);
            _b = TYPE_FEUILLES == 0 ? Resources.feuille1 : Resources.feuille2;

            _texture = null;
        }

        public void Grow(float intervalle)
        {
            if (_taille < 180)
                _taille += intervalle * 1.5f;
        }

        /// <summary>
        /// Retourne l'image donne, pivoter de angle degres
        /// http://stackoverflow.com/questions/4320531/rotating-an-image-modifies-the-resolution-and-clarity/4320581#4320581
        /// </summary>
        /// <param name="image"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Bitmap RotateImage(Image image, float angle)
        {
            const double pi2 = Math.PI / 2.0;
            double oldWidth = (double)image.Width;
            double oldHeight = (double)image.Height;

            // Convert degrees to radians
            double theta = ((double)angle) * Math.PI / 180.0;
            double locked_theta = theta;

            // Ensure theta is now [0, 2pi)
            while (locked_theta < 0.0)
                locked_theta += 2 * Math.PI;

            double newWidth, newHeight;
            int nWidth, nHeight; // The newWidth/newHeight expressed as ints
            double adjacentTop, oppositeTop;
            double adjacentBottom, oppositeBottom;


            if ((locked_theta >= 0.0 && locked_theta < pi2) ||
                (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2)))
            {
                adjacentTop = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
                oppositeTop = Math.Abs(Math.Sin(locked_theta)) * oldWidth;

                adjacentBottom = Math.Abs(Math.Cos(locked_theta)) * oldHeight;
                oppositeBottom = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
            }
            else
            {
                adjacentTop = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
                oppositeTop = Math.Abs(Math.Cos(locked_theta)) * oldHeight;

                adjacentBottom = Math.Abs(Math.Sin(locked_theta)) * oldWidth;
                oppositeBottom = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
            }

            newWidth = adjacentTop + oppositeBottom;
            newHeight = adjacentBottom + oppositeTop;
            nWidth = (int)Math.Ceiling(newWidth);
            nHeight = (int)Math.Ceiling(newHeight);

            Bitmap rotatedBmp = new Bitmap(nWidth, nHeight);
            using (Graphics g = Graphics.FromImage(rotatedBmp))
            {
                Point[] points;

                if (locked_theta >= 0.0 && locked_theta < pi2)
                {
                    points = new Point[] { 
                                             new Point( (int) oppositeBottom, 0 ), 
                                             new Point( nWidth, (int) oppositeTop ),
                                             new Point( 0, (int) adjacentBottom )
                                         };

                }
                else if (locked_theta >= pi2 && locked_theta < Math.PI)
                {
                    points = new Point[] { 
                                             new Point( nWidth, (int) oppositeTop ),
                                             new Point( (int) adjacentTop, nHeight ),
                                             new Point( (int) oppositeBottom, 0 )                        
                                         };
                }
                else if (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2))
                {
                    points = new Point[] { 
                                             new Point( (int) adjacentTop, nHeight ), 
                                             new Point( 0, (int) adjacentBottom ),
                                             new Point( nWidth, (int) oppositeTop )
                                         };
                }
                else
                {
                    points = new Point[] { 
                                             new Point( 0, (int) adjacentBottom ), 
                                             new Point( (int) oppositeBottom, 0 ),
                                             new Point( (int) adjacentTop, nHeight )        
                                         };
                }

                g.DrawImage(image, points);
            }

            return rotatedBmp;
        }
        public void Draw(OpenGL gl, float dx, float dy)
        {
            if ( _texture== null)
            {
                _texture = new Texture();
                _texture.Create(gl, _b);// RotateImage(_b, r.Next(-45, 90)));
            }

            _texture.Bind(gl);
            gl.PushMatrix();
            gl.Translate(Position.X + dx, Position.Y + dy, 0);
            gl.Rotate(0, 0, _angle + dy*0.5f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(- (_taille / 2), - (_taille / 2));
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(- (_taille / 2), + (_taille / 2));
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(+ (_taille / 2), + (_taille / 2));
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(+ (_taille / 2), - (_taille / 2));
            gl.End();
            gl.PopMatrix();
        }
    }
}
