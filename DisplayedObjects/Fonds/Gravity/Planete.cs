using SharpGL;
using System;
using System.Collections.Generic;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity
{
    public class Planete
    {
        public static List<Modele3D> modeles;

        public Vecteur3Ddbl _position;
        public int _type;
        public double _angle;
        public double _vangle;
        public double _anglePoles;
        public double _posEcliptique;
        public double _attraction;
        public double _rayonOrbite, _positionOrbite, _vitesseOrbite;
        private int _planeteCentrale;

        static public  int SOLEIL, MERCURE, VENUS, TERRE, MARS, JUPITER, SATURNE, URANUS, NEPTUNE;

        private static OpenGL _gl;
        private static Random r = new Random();
        static public int ETOILE_MIN, ETOILE_MAX, GAZEUSE_MIN, GAZEUSE_MAX, ROCHEUSE_MIN, ROCHEUSE_MAX, SATELLITE_MIN, SATELLITE_MAX, ASTEROIDE_MIN, ASTEROIDE_MAX;

        public static void InitPlanetes(OpenGL gl)
        {
            modeles = new List<Modele3D>();

            modeles.Add(new Modele3D(gl, "Soleil", 139, 1000000, "sun.png", null, 0, 0, null, true));
            SOLEIL = 0;
            ETOILE_MIN = 0;
            ETOILE_MAX = 1;
            Material mat = new Material(0.0f, 0.08f, 6.9f, 0.1f, 5);
            modeles.Add(new Modele3D(gl, "Mercure", 4.8f, 0.25f, "mercury.png", mat, 0, 0, null));
            MERCURE = 1;
            modeles.Add(new Modele3D(gl, "Venus", 12.1f, 487.0f, "venus.png", mat, 0, 0, null));
            VENUS = 2;
            modeles.Add(new Modele3D(gl, "Terre", 12.7f, 598.0f, "earth.png", mat, 0, 0, null));
            TERRE = 3;
            modeles.Add(new Modele3D(gl, "Mars", 6.7f, 64.2f, "mars.png", mat, 0, 0, null));
            MARS = 4;
            ROCHEUSE_MIN = 1;
            ROCHEUSE_MAX = 5;

            mat = new Material(0.001f, 0.02f, 0.7f, 0.16f, 3);

            modeles.Add(new Modele3D(gl, "Jupiter", 140f, 190000.0f, "jupiter.png", mat, 190f, 240f, "jupiter ring.png"));
            JUPITER = 5;
            modeles.Add(new Modele3D(gl, "Saturne", 120f, 56900.0f, "saturn.png", mat, 180f, 320f, "saturn ring.png"));
            SATURNE = 6;
            modeles.Add(new Modele3D(gl, "Neptune", 49f, 10200.0f, "neptune.png", mat, 55, 100, "neptune ring.png"));
            NEPTUNE = 7;
            modeles.Add(new Modele3D(gl, "Uranus", 51f, 8670.0f, "uranus.png", mat, 0, 0, null));
            URANUS = 8;
            GAZEUSE_MIN = 5;
            GAZEUSE_MAX = 9;

            mat = new Material(0.2f, 145.0f, 2.3f, 0.08f, 2);

            modeles.Add(new Modele3D(gl, "Io", 0.45f, 3.6f, "io.png", mat, 0, 0, null));
            modeles.Add(new Modele3D(gl, "Ganymede", 5.2f, 1.49f, "ganymede.png", mat, 0, 0, null));
            modeles.Add(new Modele3D(gl, "Europa", 3.1f, 1.48f, "europa.png", mat, 0, 0, null));
            modeles.Add(new Modele3D(gl, "Callisto",  4.8f, 1.47f, "callisto.png", mat,0, 0, null));
            modeles.Add(new Modele3D(gl, "Pluton", 2f, 0.2f, "pluto.png", mat, 0, 0, null));
            SATELLITE_MIN = 9;
            SATELLITE_MAX = 14;
            modeles.Add(new Modele3D(gl, "Asteroïde", 4f, 3f, "asteroid1.png", mat, 0, 0, null));
            modeles.Add(new Modele3D(gl, "Asteroïde", 4f, 3f, "asteroid2.png", mat, 0, 0, null));
            modeles.Add(new Modele3D(gl, "Asteroïde", 4f, 3f, "asteroid3.png", mat, 0, 0, null));
            ASTEROIDE_MIN = 14;
            ASTEROIDE_MAX = 17;

        }
        public Planete(OpenGL gl, double vitesseRotation, double anglePOles, int type, double rOrbite, double posOrbite, double vOrbite, int centrale = -1)
        {
            _gl = gl;

            if (modeles == null)
                InitPlanetes(gl);

            _type = type;
            _attraction = (float)(modeles[_type]._masse * Gravitation.VITESSE);
            //Taille = new Vecteur3Ddbl(modeles[type].rayon, modeles[type].rayon, modeles[type].rayon) * Gravitation.RATIO_RAYON;
            _vangle = vitesseRotation;
            _anglePoles = anglePOles;
            _position = new Vecteur3Ddbl();// X, Y, Z);
            _posEcliptique = (float)(r.Next((int)(Math.PI * 20000.0)) / 10000.0);
            _rayonOrbite = rOrbite;
            _positionOrbite = posOrbite;
            _vitesseOrbite = vOrbite;
            _planeteCentrale = centrale;
            Avance(0);
        }

        public Planete(OpenGL gl, float VA, float ax, int t, float rOrbite, float posOrbite, float vOrbite, float rX, float rY, float rZ, int centrale = -1)
        {
            _gl = gl;

            if (modeles == null)
                InitPlanetes(gl);

            _type = t;
            _attraction = (float)(modeles[_type]._masse * Gravitation.VITESSE);
            //Taille = new Vecteur3Ddbl(modeles[type].rayon * rX, modeles[type].rayon * rY, modeles[type].rayon * rZ) * Gravitation.RATIO_RAYON;
            _vangle = VA;
            _anglePoles = ax;
            _position = new Vecteur3Ddbl();// X, Y, Z);
            _posEcliptique = (float)(r.Next((int)(Math.PI * 15000.0)) / 10000.0);
            _rayonOrbite = rOrbite;
            _positionOrbite = posOrbite;
            _vitesseOrbite = vOrbite;
            _planeteCentrale = centrale;
            Avance(0);
        }

        public void Avance(float intervalle)
        {
            _positionOrbite += _vitesseOrbite * intervalle;


            _position.x = (_rayonOrbite * Math.Sin(_positionOrbite));
            _position.z = (_rayonOrbite * Math.Cos(_positionOrbite));
            _position.y = (_rayonOrbite / 10.0 * Math.Cos(_positionOrbite + _posEcliptique));

            if (_planeteCentrale != -1)
            {
                _position += Gravitation.Corps[_planeteCentrale]._position;
            }
            _angle += _vangle * intervalle;
        }

        public virtual void addPrimitives(OpenGL gl, List<Primitive3D> primitives)
        {
            modeles[_type].addPrimitives(gl, primitives, _position, /*Taille, */ _anglePoles, _angle);
        }

    }
}
