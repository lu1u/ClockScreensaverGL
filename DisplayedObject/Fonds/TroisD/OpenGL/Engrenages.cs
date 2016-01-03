using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ClockScreenSaverGL.DisplayedObjects.Fonds;
using SharpGL;
using SharpGL.SceneGraph.Quadrics;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD
{
    class Engrenages : TroisD, IDisposable
    {
        const float DEUX_PI = (float)(Math.PI * 2.0);
        const String CAT = "Engrenages";
        static readonly int NB_ENGRENAGES = conf.getParametre(CAT, "NbEngrenages", 500);
        static readonly float LONGUEUR_AXE = conf.getParametre(CAT, "Longueur Axe", 1.5f);
        static readonly float VITESSE = conf.getParametre(CAT, "Vitesse", 20.0f);
        static readonly float RAYON_MIN = conf.getParametre(CAT, "Rayon Min", 0.2f);
        static readonly float RAYON_MAX = conf.getParametre(CAT, "Rayon Max", 0.8f);
        static readonly float RATIO_COULEUR_MIN = conf.getParametre(CAT, "Ration Couleur Min", 0.8f);
        static readonly float RATIO_COULEUR_MAX = conf.getParametre(CAT, "Ration Couleur Max", 1.2f);
        static readonly int NB_DENTS = conf.getParametre(CAT, "Nb Dents", 40);
        static readonly float EPAISSEUR = conf.getParametre(CAT, "Epaisseur", 0.5f);
        static readonly float TAILLE_DENT = conf.getParametre(CAT, "Taille dent", 0.05f);
        static readonly int SPEC = 0;// conf.getParametre(CAT, "Specular", 128);
        static readonly int AMB = 0;// conf.getParametre(CAT, "Ambient", 0);
        static readonly int DIF = conf.getParametre(CAT, "Diffuse", 100);
        static readonly int SHININESS = conf.getParametre(CAT, "Shininess", 70);
        static readonly int[] COL_SPECULAR = { SPEC, SPEC, SPEC, 1 };
        static readonly int[] COL_AMBIENT = { AMB, AMB, AMB, 1 };
        static readonly int[] COL_DIFFUSE = { DIF, DIF, DIF, 1 };
        static readonly float[] COL_LIGHTPOS = { -15f, 10f, -15f, 1 };

        private class Engrenage
        {
            public float x, y, z, diametre1, diametre2, largeur1, largeur2, largeur3, angle, vitesse, epaisseur, R, G, B;
            public int nbdents;
        }


        Engrenage[] _engrenages = new Engrenage[NB_ENGRENAGES];
        float _angleVue = FloatRandom(0, DEUX_PI);
        private uint _genLists;
        public Engrenages(OpenGL gl) : base(gl, 0, 0, 0, 0)
        {
            _angleVue = FloatRandom(0, DEUX_PI);
            _genLists = gl.GenLists(NB_ENGRENAGES);

            float x = 0;
            float y = 0;
            float z = 0;
            float rayon = FloatRandom(RAYON_MIN, RAYON_MAX);
            float vitesse = VITESSE;
            float angle = 0;
            int nbDents = (int)Math.Round(rayon * NB_DENTS);
            float epaisseur = EPAISSEUR * FloatRandom(0.1f, 1.5f);

            for (uint i = 0; i < NB_ENGRENAGES; i++)
            {
                _engrenages[i] = CreerEngrenage(x, y, z, rayon, vitesse, nbDents, epaisseur);
                gl.NewList(i + _genLists, OpenGL.GL_COMPILE);
                DessineRoue(gl, _engrenages[i]);
                gl.EndList();
                angle += FloatRandom(-1, 2.0f);
                rayon = FloatRandom(RAYON_MIN, RAYON_MAX);
                epaisseur = EPAISSEUR * FloatRandom(0.1f, 1.5f);

                x += (rayon + _engrenages[i].diametre2) * (float)Math.Sin(angle);
                y += (rayon + _engrenages[i].diametre2) * (float)Math.Cos(angle);
                z += 0.25f * epaisseur;// * SigneRandom() ;
                vitesse = -vitesse * _engrenages[i].diametre1 / rayon;
                nbDents = (int)Math.Round(rayon * NB_DENTS);
            }
        }


        public override void  Dispose()
        {
            base.Dispose();
            _gl.DeleteLists(_genLists, NB_ENGRENAGES);
        }

        private Engrenage CreerEngrenage(float x, float y, float z, float rayon, float vitesse, int nbDents, float epaisseur)
        {
            Engrenage e = new Engrenage();
            e.x = x;
            e.y = y;
            e.z = z;
            e.angle = 0;
            e.vitesse = vitesse;
            e.nbdents = nbDents;
            e.diametre1 = rayon;
            e.diametre2 = e.diametre1 + TAILLE_DENT;
            e.epaisseur = epaisseur;
            e.R = FloatRandom(RATIO_COULEUR_MIN, RATIO_COULEUR_MAX);
            e.G = FloatRandom(RATIO_COULEUR_MIN, RATIO_COULEUR_MAX);
            e.B = FloatRandom(RATIO_COULEUR_MIN, RATIO_COULEUR_MAX);
            float pas = DEUX_PI / (float)e.nbdents;
            e.largeur1 = pas * FloatRandom(0.2f, 0.4f);
            e.largeur2 = pas * FloatRandom(0.2f, 0.4f);
            e.largeur3 = (pas - (e.largeur1 + e.largeur2)) / 2.0f;
            return e;
        }

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1f };
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_ALPHA_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Disable(OpenGL.GL_BLEND);
            gl.Disable(OpenGL.GL_FOG);
            gl.DepthMask((byte)OpenGL.GL_TRUE);
            gl.Enable(OpenGL.GL_DEPTH);
            gl.CullFace(OpenGL.GL_BACK);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            // Lumiere
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, COL_LIGHTPOS);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_COLOR, col);

            // Aspect de la surface
            gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SPECULAR, COL_SPECULAR);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, COL_AMBIENT);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, COL_DIFFUSE);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SHININESS, SHININESS);


            /*gl.PushMatrix();
            gl.Color(1.0f, 0, 0, 1.0f);
            //gl.Translate(-5f, 2f, -2f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(- 2, - 2, 0);
            gl.Vertex(- 2, + 2, 0);
            gl.Vertex(+ 2, + 2, 0);
            gl.Vertex(+ 2, - 2, 0);
            gl.End();
            gl.PopMatrix();
            */

            //gl.LookAt(0, 0, -8, 0,0, 0, 0, 1, 0);
            gl.Translate(0, 0, -8);
            gl.Rotate(0, _angleVue, 0);
            gl.Translate(-_engrenages[NB_ENGRENAGES / 2].x, -_engrenages[NB_ENGRENAGES / 2].y, -_engrenages[NB_ENGRENAGES / 2].z);


            //foreach (Engrenage e in _engrenages)
            for (uint i = 0; i < NB_ENGRENAGES; i++)
            {
                col[0] = couleur.R * _engrenages[i].R / 256.0f;
                col[1] = couleur.G * _engrenages[i].G / 256.0f;
                col[2] = couleur.B * _engrenages[i].B / 256.0f;
                gl.Color(col);

                gl.PushMatrix();
                gl.Translate(_engrenages[i].x, _engrenages[i].y, _engrenages[i].z);
                gl.Rotate(0, 0, _engrenages[i].angle);
                gl.CallList(_genLists + i);
                gl.PopMatrix();
            }
        }

        private void DessineRoue(OpenGL gl, Engrenage e)
        {
            float angle = 0;
            float ep = e.epaisseur / 2.0f;
            Vecteur3D na = NormaleTriangle(new Vecteur3D(0, 0, 0), new Vecteur3D(1, 0, 0), new Vecteur3D(0, 1, 0));
            Vecteur3D nb = NormaleTriangle(new Vecteur3D(0, 0, 0), new Vecteur3D(0, 1, 0), new Vecteur3D(1, 0, 0));
            
            gl.Begin(OpenGL.GL_TRIANGLES);
            for (int i = 0; i < e.nbdents; i++)
            {
                float x0 = 0;
                float y0 = 0;
                float x1 = (float)(e.diametre1 * Math.Cos(angle));
                float y1 = (float)(e.diametre1 * Math.Sin(angle));
                angle += e.largeur1;
                float x2 = (float)(e.diametre1 * Math.Cos(angle));
                float y2 = (float)(e.diametre1 * Math.Sin(angle));
                angle += e.largeur3;
                float x3 = (float)(e.diametre2 * Math.Cos(angle));
                float y3 = (float)(e.diametre2 * Math.Sin(angle));
                angle += e.largeur2;
                float x4 = (float)(e.diametre2 * Math.Cos(angle));
                float y4 = (float)(e.diametre2 * Math.Sin(angle));
                angle += e.largeur3;
                float x5 = (float)(e.diametre1 * Math.Cos(angle));
                float y5 = (float)(e.diametre1 * Math.Sin(angle));
                
                // Dents
                Vecteur3D v0a = new Vecteur3D(x0, y0, ep);
                Vecteur3D v1a = new Vecteur3D(x1, y1, ep);
                Vecteur3D v0b = new Vecteur3D(x0, y0, -ep);
                Vecteur3D v1b = new Vecteur3D(x1, y1, -ep);
                Vecteur3D v2a = new Vecteur3D(x2, y2, ep);
                Vecteur3D v2b = new Vecteur3D(x2, y2, -ep);
                Vecteur3D v3a = new Vecteur3D(x3, y3, ep);
                Vecteur3D v3b = new Vecteur3D(x3, y3, -ep);
                Vecteur3D v4a = new Vecteur3D(x4, y4, ep);
                Vecteur3D v4b = new Vecteur3D(x4, y4, -ep);
                Vecteur3D v5a = new Vecteur3D(x5, y5, ep);
                Vecteur3D v5b = new Vecteur3D(x5, y5, -ep);
                Vecteur3D n1 = NormaleTriangle(v1a, v1b, v2a);
                Vecteur3D n2 = NormaleTriangle(v2a, v2b, v3a);
                Vecteur3D n3 = NormaleTriangle(v3a, v3b, v4a);
                Vecteur3D n4 = NormaleTriangle(v4a, v4b, v5a);

                {
                    gl.Normal(n1.x, n1.y, n1.z);
                    gl.Vertex(x1, y1, ep);
                    gl.Vertex(x1, y1, -ep);
                    gl.Vertex(x2, y2, ep);
                    gl.Vertex(x2, y2, ep);
                    gl.Vertex(x1, y1, -ep);
                    gl.Vertex(x2, y2, -ep);

                }

                {
                    gl.Normal(n2.x, n2.y, n2.z);
                    gl.Vertex(x2, y2, ep);
                    gl.Vertex(x2, y2, -ep);
                    gl.Vertex(x3, y3, ep);
                    gl.Vertex(x2, y2, -ep);
                    gl.Vertex(x3, y3, -ep);
                    gl.Vertex(x3, y3, ep);
                }

                {
                    gl.Normal(n3.x, n3.y, n3.z);
                    gl.Vertex(x3, y3, ep);
                    gl.Vertex(x3, y3, -ep);
                    gl.Vertex(x4, y4, ep);

                    gl.Vertex(x3, y3, -ep);
                    gl.Vertex(x4, y4, -ep);
                    gl.Vertex(x4, y4, ep);
                }
                {
                    gl.Normal(n4.x, n4.y, n4.z);
                    gl.Vertex(x4, y4, ep);
                    gl.Vertex(x4, y4, -ep);
                    gl.Vertex(x5, y5, ep);

                    gl.Vertex(x4, y4, -ep);
                    gl.Vertex(x5, y5, -ep);
                    gl.Vertex(x5, y5, ep);
                }

                // Face avant
                gl.Normal(na.x, na.y, na.z);
                gl.Vertex(x0, y0, ep);
                gl.Vertex(x1, y1, ep);
                gl.Vertex(x2, y2, ep);

                gl.Vertex(x0, y0, ep);
                gl.Vertex(x2, y2, ep);
                gl.Vertex(x3, y3, ep);

                gl.Vertex(x0, y0, ep);
                gl.Vertex(x3, y3, ep);
                gl.Vertex(x4, y4, ep);

                gl.Vertex(x0, y0, ep);
                gl.Vertex(x4, y4, ep);
                gl.Vertex(x5, y5, ep);
                
                // Face arriere
                gl.Normal(nb.x, nb.y, nb.z);
                gl.Vertex(x0, y0, -ep);
                gl.Vertex(x2, y2, -ep);
                gl.Vertex(x1, y1, -ep);

                gl.Vertex(x0, y0, -ep);
                gl.Vertex(x3, y3, -ep);
                gl.Vertex(x2, y2, -ep);

                gl.Vertex(x0, y0, -ep);
                gl.Vertex(x4, y4, -ep);
                gl.Vertex(x3, y3, -ep);

                gl.Vertex(x0, y0, -ep);
                gl.Vertex(x5, y5, -ep);
                gl.Vertex(x4, y4, -ep);
            }
            

            // Axe
            double NB_TRANCHES = 30;
            float X1 = 0.1f * (float)Math.Cos((double)0 / NB_TRANCHES * DEUX_PI);
            float Y1 = 0.1f * (float)Math.Sin((double)0 / NB_TRANCHES * DEUX_PI);
            ep = e.epaisseur * 0.75f;
            for ( int i = 0; i < NB_TRANCHES; i++)
            {
                double a = (double)(i + 1) * DEUX_PI / NB_TRANCHES;
                float X2 = 0.1f * (float)Math.Cos(a);
                float Y2 = 0.1f * (float)Math.Sin(a);
                gl.Normal(na.x, na.y, na.z);
                gl.Vertex(0, 0, ep);
                gl.Vertex(X1, Y1, ep);
                gl.Vertex(X2, Y2, ep);

                Vecteur3D v1a = new Vecteur3D(X1, Y1, ep);
                Vecteur3D v1b = new Vecteur3D(X1, Y1, -ep);
                Vecteur3D v2a = new Vecteur3D(X2, Y2, ep);
                Vecteur3D n1 = NormaleTriangle(v1a, v1b, v2a);
                gl.Normal(n1.x, n1.y, n1.z);
                gl.Vertex(X1, Y1, ep);
                gl.Vertex(X1, Y1, -ep);
                gl.Vertex(X2, Y2, ep);

                gl.Vertex(X2, Y2, ep);
                gl.Vertex(X1, Y1, -ep);
                gl.Vertex(X2, Y2, -ep);


                gl.Normal(nb.x, nb.y, nb.z);
                gl.Vertex(0, 0, -ep);
                gl.Vertex(X1, Y1, -ep);
gl.Vertex(X2, Y2, -ep);
                
                X1 = X2;
                Y1 = Y2;
            }

            gl.End();
        }

        private void Vertexes(OpenGL gl, float angle, float diametre, float ep)
        {
            float c = (float)Math.Cos(angle);
            float s = (float)Math.Sin(angle);

            gl.Normal(-s, -c, 0.0);
            gl.Vertex(c * diametre, s * diametre, -ep);
            gl.Normal(-s, -c, 0.0);
            gl.Vertex(c * diametre, s * diametre, 0);
        }

        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
            _angleVue += 5.0f * maintenant._intervalle;
            foreach (Engrenage e in _engrenages)
                e.angle += e.vitesse * maintenant._intervalle;
        }
    }
}
