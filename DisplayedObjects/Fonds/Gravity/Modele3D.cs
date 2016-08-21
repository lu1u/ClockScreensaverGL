using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Collections.Generic;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity
{
    public class Modele3D : IDisposable
    {
        public String _nom;
        public Texture texture;
        public Vecteur3Ddbl _taille ;
        public float _masse;
        public bool _asDesAnneaux;
        public float _rAnneauInt, _rAnneauExt;
        public Texture textureAnneaux;
        public bool isBillboard;
        private static OpenGL _gl;
        public Material _material;
        public Modele3D(OpenGL gl, String nom, float rayon, float masse, String texture,  Material material, float rAnneauI, float rAnneauE, String textureAnneau,bool isbb = false)
        {
            _nom = nom;
            _gl = gl;
            this.texture = new Texture();
            this.texture.Create(gl, Config.getImagePath(texture));
            _material = material;
            _taille = new Vecteur3Ddbl( rayon * Gravitation.RATIO_RAYON, rayon * Gravitation.RATIO_RAYON, rayon * Gravitation.RATIO_RAYON);
            _masse = masse;
            if (textureAnneau == null)
            {
                _asDesAnneaux = false;
                _rAnneauExt = 0;
            }
            else
            {
                _asDesAnneaux = true;
                _rAnneauInt = (float)(rAnneauI * Gravitation.RATIO_RAYON);
                _rAnneauExt = (float)(rAnneauE * Gravitation.RATIO_RAYON);
                textureAnneaux = new Texture();
                textureAnneaux.Create(gl, Config.getImagePath(textureAnneau));
            }
            isBillboard = isbb;
        }

        public void Dispose()
        {
            texture?.Destroy(_gl);
            textureAnneaux?.Destroy(_gl);
        }

        public void addPrimitives(OpenGL gl, List<Primitive3D> primitives, Vecteur3Ddbl Position, /*Vecteur3Ddbl Taille,*/ double angleX, double angle)
        {
            Vecteur3Ddbl pos = Position;// / Gravitation.DiviseurDistance;
            if (isBillboard)
            {
                if (Primitive3D.isVisible(Position, _taille.x))
                {
                    Primitive3DBillboard p = new Primitive3DBillboard(gl, pos, _taille.x);
                    p.TEXTURE = texture;
                    p.LIGHTING = false;
                    p.COLOR = System.Drawing.Color.White;
                    p.ALPHABLEND = true;
                    p.BLEND_FUNC = OpenGL.GL_ONE_MINUS_SRC_ALPHA;
                    p.MATERIAL = _material;
                    primitives.Add(p);
                }
            }
            else
            {
                if (Primitive3D.isVisible(Position, _taille.x))
                {
                    Primitive3DSphere p = new Primitive3DSphere(gl, pos, _taille, new Vecteur3Ddbl(angleX, 0, angle));
                    p.TEXTURE = texture;
                    p.ALPHABLEND = false;
                    p.LIGHTING = true;
                    p.MATERIAL = _material;
                    primitives.Add(p);
                }
            }

            if (_asDesAnneaux)
            {
                // Dessine des anneaux
                //const float Details = 10;
                
                int NbTranches = (int)(Primitive3D.CalculeNiveauDetail(Position, _rAnneauExt) / 20.0);

                double angleY = 0;
                
                for (int i = 0; i < NbTranches; i++)
                {
                    double Angle = angleY;
                    double Cos1 =   Math.Cos(Angle);
                    double Sin1 =   Math.Sin(Angle);
                    double y1 = Math.Sin(DEG_TO_RAD(angleX)) * Math.Sin(Angle- Math.PI/2.0);

                    double Angle2 = Angle + (float)(Math.PI * 2.0 / (double)NbTranches);
                    double Cos2 = Math.Cos(Angle2);
                    double Sin2 = Math.Sin(Angle2);
                    double y2 =   Math.Sin(DEG_TO_RAD(angleX)) * Math.Sin(Angle2 - Math.PI / 2.0);

                    Vecteur3Ddbl Point1 = new Vecteur3Ddbl((float)(Sin1 * _rAnneauExt), y1 * _rAnneauExt, (float)(Cos1 * _rAnneauExt));
                    Vecteur3Ddbl Point2 = new Vecteur3Ddbl((float)(Sin2 * _rAnneauExt), y2 * _rAnneauExt, (float)(Cos2 * _rAnneauExt));
                    Vecteur3Ddbl Point3 = new Vecteur3Ddbl((float)(Sin2 * _rAnneauInt), y2 * _rAnneauInt, (float)(Cos2 * _rAnneauInt));
                    Vecteur3Ddbl Point4 = new Vecteur3Ddbl((float)(Sin1 * _rAnneauInt), y1* _rAnneauInt, (float)(Cos1 * _rAnneauInt));

                    Point1.additionner(pos);
                    Point2.additionner(pos);
                    Point3.additionner(pos);
                    Point4.additionner(pos);
                    if (Primitive3D.isVisible(Point1, Point2, Point3, Point4))
                    {
                        Primitive3D p = new Primitive3DQuadrilatere(gl, Point1, Point2, Point3, Point4);
                        p.CULLFACE = false;
                        p.ALPHABLEND = true;
                        p.COLOR = System.Drawing.Color.White;
                        p.TEXTURE = textureAnneaux;
                        p.BLEND_FUNC = OpenGL.GL_ONE_MINUS_SRC_ALPHA;
                        primitives.Add(p);
                    }
                    angleY = Angle2; //(Math.PI  /(double)NbTranches);

                }
            }
        }

        private static double DEG_TO_RAD(double angleX)
        {
            return angleX * Math.PI / 180.0;
        }

        private static float max(float rY, float rZ)
        {
            return rY > rZ ? rY : rZ;
        }
        public float tailleMax()
        {
            return max((float)_taille.x, max((float)_taille.y, max((float)_taille.z, _rAnneauExt)));
        }
    };
}
