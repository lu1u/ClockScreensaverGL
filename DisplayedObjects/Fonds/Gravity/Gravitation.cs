using ClockScreenSaverGL.Config;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity
{
    class Gravitation : ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.TroisD, IDisposable
    {
        public const String CAT = "Gravitation";
        static protected CategorieConfiguration c = Configuration.getCategorie(CAT);
        public static double RAYON_UNIVERS = 10;// c.getParametre("Rayon Univers", 8);
        public static double RATIO_RAYON = c.getParametre("Ratio rayon planetes", 0.0005, (a) => { RATIO_RAYON = Convert.ToDouble(a); } );
        public static double RATIO_DISTANCES = c.getParametre("Ratio distance planetes", 0.5, (a) => { RATIO_DISTANCES = Convert.ToDouble(a); });
        public static double VITESSE_SUIVI_TO = c.getParametre("Vitesse ciblage", 0.4, (a) => { VITESSE_SUIVI_TO = Convert.ToDouble(a); });
        public static double VITESSE_SUIVI_FROM = c.getParametre("Vitesse camera", 0.005, (a) => { VITESSE_SUIVI_FROM = Convert.ToDouble(a); });
        public static double DISTANCE_MIN_CAMERA = c.getParametre("Distance min camera", 0.01, (a) => { DISTANCE_MIN_CAMERA = Convert.ToDouble(a); });
        public static double VITESSE = c.getParametre("Vitesse", 0.001, (a) => { VITESSE = Convert.ToDouble(a); });
        public static int DETAILS = c.getParametre("Details", 5, (a) => { DETAILS = Convert.ToInt32(a); });
        public static int DETAILS_ASTEROIDS = c.getParametre("Details asteroides", 30, (a) => { DETAILS_ASTEROIDS = Convert.ToInt32(a); });
        public static int NIVEAU_DETAIL = c.getParametre("Niveau detail", 800, (a) => { NIVEAU_DETAIL = Convert.ToInt32(a); });
        public static int NB_ROCHEUSES = c.getParametre("Nb Rocheuses", 4);
        public static int NB_ASTEROIDES = c.getParametre("Nb Asteroides", 300);
        public static int NB_GAZEUSES = c.getParametre("Nb Gazeuses", 4);
        public static float ANGLE_VISION = 70;

        public static readonly int DELAI_CHANGE_CAMERA = c.getParametre("Delai Change Camera", 40000);
        private TimerIsole _changeCamera = new TimerIsole(DELAI_CHANGE_CAMERA);

        private Texture _textureTop, _textureBottom, _textureLeft, _textureRight, _textureFront, _textureBack;
        float[] COL_AMBIENT = { 0.5f, 0.5f, 0.5f, 1.0f };//{ 0.1f, 0.1f, 0.1f, 1.0f };
        float[] COL_DIFFUSE = { 1.0f, 1.0f, 1.0f, 1.0f };
        float[] COL_SPECULAR = { 1.0f, 1.0f, 1.0f, 1.0f };// { 0.8f, 0.8f, 0.8f, 1.0f };
        float[] COL_LIGHTPOS = { -2, 1.5f, -2.5f, 1 };
        bool DESSINE_CROIX = false;

        static readonly float[] SPECULAR_LIGHT = { 0.5f, 0.5f, 0.5f, 1.0f };
        static readonly float[] AMBIENT_LIGHT = { 1.0f, 1.0f, 1.0f, 1.0f };
        static readonly float[] DIFFUSE_LIGHT = { 1.0f, 1.0f, 1.0f, 1.0f };

        public static List<Planete> Corps;
        private static List<Primitive3D> _primitives = new List<Primitive3D>();
        int corpsSuivi;
        public static Vecteur3Ddbl cameraFrom, cameraTo, cameraCible;
        private int nbAsteroides;
        public Gravitation(OpenGL gl) : base(gl,0,0,0,0)
        {
            _textureTop = new Texture();
            _textureTop.Create(gl, c.getParametre( "Universe top", Configuration.getImagePath( "universe_top.png" ) ) );
            _textureBottom = new Texture();
            _textureBottom.Create(gl, c.getParametre( "Universe bottom", Configuration.getImagePath( "universe_bottom.png" ) ) );
            _textureLeft = new Texture();
            _textureLeft.Create(gl, c.getParametre( "Universe left", Configuration.getImagePath( "universe_left.png" ) ) );
            _textureRight = new Texture();
            _textureRight.Create(gl, c.getParametre( "Universe right", Configuration.getImagePath( "universe_right.png" ) ) );
            _textureFront = new Texture();
            _textureFront.Create(gl, c.getParametre( "Universe front", Configuration.getImagePath( "universe_front.png" ) ) );
            _textureBack = new Texture();
            _textureBack.Create(gl, c.getParametre( "Universe back", Configuration.getImagePath( "universe_back.png" ) ) );

            #region AjouteCorps
            Planete.InitPlanetes(gl);
            Corps = new List<Planete>();

            // En premier: l'etoile
            Planete pl = new Planete(gl, 0, 0, Planete.ETOILE_MIN, 0, 0, 0);
            Corps.Add(pl);

            // Mercure
            double rayonOrbite = 1 * RATIO_DISTANCES;
            double posOrbite = FloatRandom((float)(Math.PI * -2.0), (float)(Math.PI * 2.0));
            double vOrbite = CalculeVitesseOrbitale(Corps[0]._attraction, rayonOrbite);
            Planete planete = new Planete(gl, FloatRandom(10, 30) * SigneRandom(), FloatRandom(-30, 30), Planete.MERCURE, rayonOrbite, posOrbite, vOrbite, 0);
            Corps.Add(planete);

            // Venus
            rayonOrbite = 2 * RATIO_DISTANCES;
            posOrbite = FloatRandom((float)(Math.PI * -2.0), (float)(Math.PI * 2.0));
            vOrbite = CalculeVitesseOrbitale(Corps[0]._attraction, rayonOrbite);
            planete = new Planete(gl, FloatRandom(10, 30) * SigneRandom(), FloatRandom(-30, 30), Planete.VENUS, rayonOrbite, posOrbite, vOrbite, 0);
            Corps.Add(planete);

            // Terre
            rayonOrbite = 3 * RATIO_DISTANCES;
            posOrbite = FloatRandom((float)(Math.PI * -2.0), (float)(Math.PI * 2.0));
            vOrbite = CalculeVitesseOrbitale(Corps[0]._attraction, rayonOrbite);
            planete = new Planete(gl, FloatRandom(10, 30) * SigneRandom(), FloatRandom(-30, 30), Planete.TERRE, rayonOrbite, posOrbite, vOrbite, 0);
            Corps.Add(planete);

            // Mars
            rayonOrbite = 4 * RATIO_DISTANCES;
            posOrbite = FloatRandom((float)(Math.PI * -2.0), (float)(Math.PI * 2.0));
            vOrbite = CalculeVitesseOrbitale(Corps[0]._attraction, rayonOrbite);
            planete = new Planete(gl, FloatRandom(10, 30) * SigneRandom(), FloatRandom(-30, 30), Planete.MARS, rayonOrbite, posOrbite, vOrbite, 0);
            Corps.Add(planete);

            // Jupiter
            rayonOrbite = 6 * RATIO_DISTANCES;
            posOrbite = FloatRandom((float)(Math.PI * -2.0), (float)(Math.PI * 2.0));
            vOrbite = CalculeVitesseOrbitale(Corps[0]._attraction, rayonOrbite);
            planete = new Planete(gl, FloatRandom(10, 30) * SigneRandom(), FloatRandom(-30, 30), Planete.JUPITER, rayonOrbite, posOrbite, vOrbite, 0);
            Corps.Add(planete);


            // Saturne
            rayonOrbite = 7 * RATIO_DISTANCES;
            posOrbite = FloatRandom((float)(Math.PI * -2.0), (float)(Math.PI * 2.0));
            vOrbite = CalculeVitesseOrbitale(Corps[0]._attraction, rayonOrbite);
            planete = new Planete(gl, FloatRandom(10, 30) * SigneRandom(), FloatRandom(-30, 30), Planete.SATURNE, rayonOrbite, posOrbite, vOrbite, 0);
            Corps.Add(planete);


            // Uranus
            rayonOrbite = 8 * RATIO_DISTANCES;
            posOrbite = FloatRandom((float)(Math.PI * -2.0), (float)(Math.PI * 2.0));
            vOrbite = CalculeVitesseOrbitale(Corps[0]._attraction, rayonOrbite);
            planete = new Planete(gl, FloatRandom(10, 30) * SigneRandom(), FloatRandom(-30, 30), Planete.URANUS, rayonOrbite, posOrbite, vOrbite, 0);
            Corps.Add(planete);


            // Neptune
            rayonOrbite = 9 * RATIO_DISTANCES;
            posOrbite = FloatRandom((float)(Math.PI * -2.0), (float)(Math.PI * 2.0));
            vOrbite = CalculeVitesseOrbitale(Corps[0]._attraction, rayonOrbite);
            planete = new Planete(gl, FloatRandom(10, 30) * SigneRandom(), FloatRandom(-30, 30), Planete.NEPTUNE, rayonOrbite, posOrbite, vOrbite, 0);
            Corps.Add(planete);


            // Quelques asteroides
            /*for (int i = 0; i < NB_ASTEROIDES; i++)
            {
                rayonOrbite = FloatRandom(4.5f, 5.5f) * RATIO_DISTANCES;
                posOrbite = FloatRandom((float)(Math.PI * -2.0), (float)(Math.PI * 2.0));
                vOrbite = CalculeVitesseOrbitale(Corps[0]._attraction, rayonOrbite);
                Corps.Add(new Asteroide(gl, FloatRandom(10, 30) * SigneRandom(), FloatRandom(-30, 30), r.Next(Planete.ASTEROIDE_MIN, Planete.ASTEROIDE_MAX), (float)rayonOrbite, (float)posOrbite, (float)vOrbite, FloatRandom(0.5f, 2.0f), FloatRandom(0.5f, 2.0f), FloatRandom(0.5f, 2.0f), 0));
            }*/
            nbAsteroides = 0;

            #endregion

            ChangePlaneteCible();
            cameraCible = Corps[corpsSuivi]._position;

            ChangePlaneteCible();
            int to = r.Next(Corps.Count);

            int from;
            do
            {
                from = r.Next(Corps.Count);
            }
            while (from == corpsSuivi);

            cameraFrom = new Vecteur3Ddbl(Corps[from]._position);
            cameraFrom.y += RAYON_UNIVERS / 10.0f;
            cameraTo = new Vecteur3Ddbl(Corps[to]._position);            
        }

        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
        private static void construitAffiche()
        {
            /*_affiche?.Destroy(_gl);
            using (Bitmap bmp = new Bitmap(300, 300, PixelFormat.Format32bppArgb))

            using (Graphics g = Graphics.FromImage(bmp))
            {
                Modele3D m = Planete.modeles[Corps[corpsSuivi].type];
                String material = "Details " + NIVEAU_DETAIL; /*"Ambient: "   + MATERIAL.ambient[0] + "," + MATERIAL.ambient[1] + "," + MATERIAL.ambient[2] + "\n" +
                                    "Diffuse: " + MATERIAL.diffuse[0] + "," + MATERIAL.diffuse[1] + "," + MATERIAL.diffuse[2] + "\n" +
                                   "Specular: " + MATERIAL.specular[0] + "," + MATERIAL.specular[1] + "," + MATERIAL.specular[2] + "\n" +
                                   "Color: " + MATERIAL.color[0] + "," + MATERIAL.color[1] + "," + MATERIAL.color[2] + "\n" +
                                   "Shininess: " + MATERIAL.shininess;

                //g.DrawString(m.nom + "\n" + m.rayon + "\n" + material, SystemFonts.DefaultFont, Brushes.White, 3, 3);
                _affiche = new Texture();
                _affiche.Create(_gl, bmp);

            }*/

        }

        public override void Dispose()
        {
            base.Dispose();
            _textureTop?.Destroy(_gl);
            _textureBottom?.Destroy(_gl);
            _textureFront?.Destroy(_gl);
            _textureBack?.Destroy(_gl);
            _textureLeft?.Destroy(_gl);
            _textureRight?.Destroy(_gl);
        }

        static double CalculeVitesseOrbitale(double Attraction, double Distance)
        {
            return (float)(Math.Sqrt(Attraction / Distance) * VITESSE);
        }




        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            #region OPENGL_SETTINGS
            if (nbAsteroides < NB_ASTEROIDES)
            {
                double rayonOrbite = FloatRandom(4.5f, 5.5f) * RATIO_DISTANCES;
                float posOrbite = FloatRandom((float)(Math.PI * -2.0), (float)(Math.PI * 2.0));
                double vOrbite = CalculeVitesseOrbitale(Corps[0]._attraction, rayonOrbite);
                Corps.Add(new Asteroide(gl, FloatRandom(10, 30) * SigneRandom(), FloatRandom(-30, 30), r.Next(Planete.ASTEROIDE_MIN, Planete.ASTEROIDE_MAX), (float)rayonOrbite, (float)posOrbite, (float)vOrbite, FloatRandom(0.5f, 2.0f), FloatRandom(0.5f, 2.0f), FloatRandom(0.5f, 2.0f), 0));
                nbAsteroides++;
            }
            gl.ClearColor(0, 0, 0, 1);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Enable(OpenGL.GL_ALPHA_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Enable(OpenGL.GL_NORMALIZE);
            gl.Enable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Disable(OpenGL.GL_BLEND);
            gl.Disable(OpenGL.GL_FOG);
            //gl.DepthMask((byte)OpenGL.GL_TRUE);
            gl.CullFace(OpenGL.GL_BACK);
            changeZoom(gl, tailleEcran.Width, tailleEcran.Height, 0.001f, (float)RAYON_UNIVERS * 2.0f);


            gl.LookAt(cameraFrom.x, cameraFrom.y, cameraFrom.z, cameraTo.x, cameraTo.y, cameraTo.z, 0, 1, 0);
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1 };

            COL_LIGHTPOS[0] = (float)Corps[0]._position.x;
            COL_LIGHTPOS[1] = (float)Corps[0]._position.y;
            COL_LIGHTPOS[2] = (float)Corps[0]._position.z;
            // Lumiere
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, COL_LIGHTPOS);
            /*gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_COLOR, col);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, SPECULAR_LIGHT);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, DIFFUSE_LIGHT);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, AMBIENT_LIGHT);*/

            // Aspect de la surface
            gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);


            #endregion
            Primitive3D.InitScene(gl);
            _primitives.Clear();
            foreach (Planete p in Corps)
                p.addPrimitives(gl, _primitives);

            DessineUnivers(gl);
            if (DESSINE_CROIX)
                SignaleCible(gl, _primitives);

            _primitives.Sort(delegate (Primitive3D p1, Primitive3D p2)
            {
                if (p1.ALPHABLEND && !p2.ALPHABLEND)
                    return 1;
                else
                    if (!p1.ALPHABLEND && p2.ALPHABLEND)
                    return -1;

                double distance1 = (p1._position - cameraFrom).Longueur();
                double distance2 = (p2._position - cameraFrom).Longueur();

                if (distance1 > distance2)
                    return -1;
                else
                    if (distance1 < distance2)
                    return 1;
                else
                    return 0;
            });

            foreach (Primitive3D pr in _primitives)
                if (!pr.ALPHABLEND)
                    pr.dessine(gl);

            foreach (Primitive3D pr in _primitives)
                if (pr.ALPHABLEND)
                    pr.dessine(gl);

            Console c = Console.getInstance(gl);
            c.AddLigne(Color.Green, "Gravitation");
            c.AddLigne(Color.Green, "Nombre de corps " + Corps.Count + ", nb asteroides " + nbAsteroides);
            c.AddLigne(Color.Green, "Niveau détail" + NIVEAU_DETAIL);
            c.AddLigne(Color.LightGreen, "Corps suivi " + Planete.modeles[Corps[corpsSuivi]._type]._nom);
            c.AddLigne(Color.Red, "Corps suivi from " + VITESSE_SUIVI_FROM);
            c.AddLigne(Color.Red, "Corps suivi to " + VITESSE_SUIVI_TO);
            /*if (Planete.modeles[Corps[corpsSuivi]._type]._material != null)
            {
                Material m = Planete.modeles[Corps[corpsSuivi]._type]._material;
                c.AddLigne(Color.Red, "Shininess: " + m.shininess);
                c.AddLigne(Color.Red, "Ambient: " + m.ambient[0]);
                c.AddLigne(Color.Red, "Diffuse: " + m.diffuse[0]);

                c.AddLigne(Color.Red, "Specular: " + m.specular[0]);
                c.AddLigne(Color.Red, "Color: " + m.color[0]);
            }*/

            //Lensflare(gl, tailleEcran, Corps[0].Position, Color.Red, Planete.modeles[Corps[0].type].tailleMax());
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }


        private void SignaleCible(OpenGL gl, List<Primitive3D> primitives)
        {
            DessineCroix(gl, cameraCible, Planete.modeles[Corps[corpsSuivi]._type].tailleMax() * 2.0f, Color.Green);

            /* Primitive3D p = new Primitive3DBillboard(gl, cameraCible, Corps[corpsSuivi].Taille.x * 2);
             p.TEXTURE = _affiche;
             p.LIGHTING = false;
             p.COLOR = Color.White;
             p.ALPHABLEND = true;
             p.MATERIAL = null;
             primitives.Add(p);*/
        }

        private void DessineUnivers(OpenGL gl)
        {
            double tailleUnivers = RAYON_UNIVERS * 1.1;
            Primitive3DQuadrilatere p = new Primitive3DQuadrilatere(gl,
                new Vecteur3Ddbl(-tailleUnivers + cameraFrom.x, tailleUnivers + cameraFrom.y, tailleUnivers + cameraFrom.z),
                new Vecteur3Ddbl(tailleUnivers + cameraFrom.x, tailleUnivers + cameraFrom.y, tailleUnivers + cameraFrom.z),
                new Vecteur3Ddbl(tailleUnivers + cameraFrom.x, -tailleUnivers + cameraFrom.y, tailleUnivers + cameraFrom.z),
                new Vecteur3Ddbl(-tailleUnivers + cameraFrom.x, -tailleUnivers + cameraFrom.y, tailleUnivers + cameraFrom.z));
            p.TEXTURE = _textureFront;
            p.LIGHTING = false;
            p.COLOR = System.Drawing.Color.White;

            _primitives.Add(p);

            p = new Primitive3DQuadrilatere(gl,
                            new Vecteur3Ddbl(tailleUnivers + cameraFrom.x, -tailleUnivers + cameraFrom.y, -tailleUnivers + cameraFrom.z),
                            new Vecteur3Ddbl(tailleUnivers + cameraFrom.x, tailleUnivers + cameraFrom.y, -tailleUnivers + cameraFrom.z),
                            new Vecteur3Ddbl(-tailleUnivers + cameraFrom.x, tailleUnivers + cameraFrom.y, -tailleUnivers + cameraFrom.z),
                            new Vecteur3Ddbl(-tailleUnivers + cameraFrom.x, -tailleUnivers + cameraFrom.y, -tailleUnivers + cameraFrom.z));
            p.TEXTURE = _textureBack;
            p.LIGHTING = false;
            p.CULLFACE = false;
            _primitives.Add(p);

            p = new Primitive3DQuadrilatere(gl,
                                        new Vecteur3Ddbl(tailleUnivers + cameraFrom.x, tailleUnivers + cameraFrom.y, -tailleUnivers + cameraFrom.z),
                                        new Vecteur3Ddbl(tailleUnivers + cameraFrom.x, tailleUnivers + cameraFrom.y, tailleUnivers + cameraFrom.z),
                                        new Vecteur3Ddbl(-tailleUnivers + cameraFrom.x, tailleUnivers + cameraFrom.y, tailleUnivers + cameraFrom.z),
                                        new Vecteur3Ddbl(-tailleUnivers + cameraFrom.x, tailleUnivers + cameraFrom.y, -tailleUnivers + cameraFrom.z));
            p.TEXTURE = _textureTop;
            p.LIGHTING = false;
            _primitives.Add(p);

            p = new Primitive3DQuadrilatere(gl,
                                        new Vecteur3Ddbl(-tailleUnivers + cameraFrom.x, -tailleUnivers + cameraFrom.y, tailleUnivers + cameraFrom.z),
                                        new Vecteur3Ddbl(tailleUnivers + cameraFrom.x, -tailleUnivers + cameraFrom.y, tailleUnivers + cameraFrom.z),
                                        new Vecteur3Ddbl(tailleUnivers + cameraFrom.x, -tailleUnivers + cameraFrom.y, -tailleUnivers + cameraFrom.z),
                                        new Vecteur3Ddbl(-tailleUnivers + cameraFrom.x, -tailleUnivers + cameraFrom.y, -tailleUnivers + cameraFrom.z));
            p.TEXTURE = _textureBottom;
            p.LIGHTING = false;
            _primitives.Add(p);

            p = new Primitive3DQuadrilatere(gl,
                            new Vecteur3Ddbl(tailleUnivers + cameraFrom.x, -tailleUnivers + cameraFrom.y, tailleUnivers + cameraFrom.z),
                            new Vecteur3Ddbl(tailleUnivers + cameraFrom.x, tailleUnivers + cameraFrom.y, tailleUnivers + cameraFrom.z),
                            new Vecteur3Ddbl(tailleUnivers + cameraFrom.x, tailleUnivers + cameraFrom.y, -tailleUnivers + cameraFrom.z),
                            new Vecteur3Ddbl(tailleUnivers + cameraFrom.x, -tailleUnivers + cameraFrom.y, -tailleUnivers + cameraFrom.z));
            p.TEXTURE = _textureRight;
            p.LIGHTING = false;
            _primitives.Add(p);

            p = new Primitive3DQuadrilatere(gl,
                new Vecteur3Ddbl(-tailleUnivers + cameraFrom.x, tailleUnivers + cameraFrom.y, -tailleUnivers + cameraFrom.z),
                new Vecteur3Ddbl(-tailleUnivers + cameraFrom.x, tailleUnivers + cameraFrom.y, tailleUnivers + cameraFrom.z),
                new Vecteur3Ddbl(-tailleUnivers + cameraFrom.x, -tailleUnivers + cameraFrom.y, tailleUnivers + cameraFrom.z),
                new Vecteur3Ddbl(-tailleUnivers + cameraFrom.x, -tailleUnivers + cameraFrom.y, -tailleUnivers + cameraFrom.z));
            p.TEXTURE = _textureLeft;
            p.LIGHTING = false;
            _primitives.Add(p);
        }





        /// <summary>
        /// Deplacement de tous les objets: flocons, camera...
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif


            for (int i = 1; i < Corps.Count; i++)
                Corps[i].Avance(maintenant.intervalleDepuisDerniereFrame);

            cameraCible = Corps[corpsSuivi]._position;

            Vecteur3Ddbl decalage = cameraCible - cameraTo;
            cameraTo += decalage * (maintenant.intervalleDepuisDerniereFrame * VITESSE_SUIVI_TO);

            if (_changeCamera.Ecoule())
            {
                ChangePlaneteCible();
            }

            decalage = cameraFrom - cameraCible;
            double longueur = decalage.Longueur();
            decalage.Normalize();
            decalage *= (longueur - (Planete.modeles[Corps[corpsSuivi]._type].tailleMax() * DISTANCE_MIN_CAMERA)) * VITESSE_SUIVI_FROM;
            cameraFrom -= decalage;


            if (maintenant.intervalleDepuisDerniereFrame < (1.0f / 40.0))
            {
                NIVEAU_DETAIL++;
                c.setParametre("Niveau detail", NIVEAU_DETAIL);
                construitAffiche();
            }
            else
                if (maintenant.intervalleDepuisDerniereFrame > (1.0f / 20.0f))
                if (NIVEAU_DETAIL > 10)
                {
                    NIVEAU_DETAIL--;
                    c.setParametre("Niveau detail", NIVEAU_DETAIL);
                    construitAffiche();
                }
        }

        private static bool IsAsteroide(int type)
        {
            return ((type >= Planete.ASTEROIDE_MIN) && (type <= Planete.ASTEROIDE_MAX)) || ((type >= Planete.SATELLITE_MIN) && (type <= Planete.SATELLITE_MAX));
        }

        private void ChangePlaneteCible()
        {
            if (Probabilite(0.5f))
            {
                // on admet la possibilite de suivre un asteroide
                corpsSuivi = r.Next(Corps.Count);
            }
            else
                do
                {
                    corpsSuivi = r.Next(Corps.Count);
                }
                while (IsAsteroide(Corps[corpsSuivi]._type));

            construitAffiche();
        }

        /// <summary>
        /// Pression sur une touche, retourner true si l'objet a traite, false = fin de l'economiseur
        /// </summary>
        /// <param name="f"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        ///
        public override bool KeyDown(Form f, Keys k)
        {
            Material m = Planete.modeles[Corps[corpsSuivi]._type]._material;

            switch (k)
            {
                case Keys.T:
                    {
                        ChangePlaneteCible();
                        _changeCamera = new TimerIsole(DELAI_CHANGE_CAMERA);
                        return true;
                    }

                case Keys.X:
                    {
                        DESSINE_CROIX = !DESSINE_CROIX;
                        return true;
                    }

                /*case Keys.Insert:
                    VITESSE_SUIVI_FROM *= 1.1;
                    return true;

                case Keys.Delete:
                    VITESSE_SUIVI_FROM /= 1.1;
                    return true;

                case Keys.Home:
                    VITESSE_SUIVI_TO *= 1.1;
                    return true;

                case Keys.End:
                    VITESSE_SUIVI_TO /= 1.1;
                    return true;*/

                case Keys.Insert:
                    {
                        m?.setAmbient(m.ambient[0] * 1.1f);
                        return true;
                    }
                case Keys.Delete:
                    {
                        m?.setAmbient(m.ambient[0] * 0.9f);
                        return true;
                    }

                case Keys.Home:
                    {
                        m?.setDiffuse(m.diffuse[0] * 1.1f);
                        return true;
                    }
                case Keys.End:
                    {
                        m?.setDiffuse(m.diffuse[0] * 0.9f);
                        return true;
                    }
                case Keys.PageUp:
                    {
                        m?.setSpecular(m.specular[0] * 1.1f);
                        construitAffiche();
                        return true;
                    }
                case Keys.PageDown:
                    {
                        m?.setSpecular(m.specular[0] * 0.9f);
                        return true;
                    }

                case Keys.Back:
                    {
                        m?.setColor(m.color[0] * 1.1f);
                        return true;
                    }
                case Keys.Enter:
                    {
                        m?.setColor(m.color[0] * 0.9f);
                        return true;
                    }

                case Keys.Subtract:
                    m.shininess--;
                    return true;

                case Keys.Add:
                    m.shininess++;
                    return true;
                default:
                    return base.KeyDown(f, k); ;
            }
        }

        //////////////////////////////////////////////////////////////////////
        // Dessine une croix ( axes X, Y et Z ) autour d'une position donnee
        // ENTREES:	Position de la croix
        //			Taille de la croix
        ///////////////////////////////////////////////////////////////////////////////
        public static void DessineCroix(OpenGL gl, Vecteur3Ddbl Position, float Taille, Color Couleur)
        {
            gl.PushAttrib(OpenGL.GL_ENABLE_BIT);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Color(Couleur.R / 256.0f, Couleur.G / 256.0f, Couleur.B / 256.0f, 0.7f);
            gl.LineWidth(2);
            {
                gl.PushMatrix();
                gl.Translate(Position.x, Position.y, Position.z);
                {
                    gl.Begin(OpenGL.GL_LINES);
                    // X: vert
                    //gl.Color(0, 1.0f, 0);
                    gl.Vertex(-Taille, 0, 0);
                    gl.Vertex(Taille, 0, 0);


                    // Z: rouge
                    //gl.Color(0, 0, 1.0f);
                    gl.Vertex(0, 0, -Taille);
                    gl.Vertex(0, 0, Taille);

                    // Y
                    gl.Color(1.0f, 1.0f, 1.0f);
                    gl.Vertex(0, -Taille, 0);
                    gl.Vertex(0, Taille, 0);

                    gl.End();
                }
                gl.PopMatrix();
            }
            gl.PopAttrib();
        }

        private static void GetViewport(OpenGL gl, out float Largeur, out float Hauteur)
        {
            int[] Params = new int[4];
            gl.GetInteger(OpenGL.GL_VIEWPORT, Params);

            Largeur = (float)Params[2];
            Hauteur = (float)Params[3];
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Obtient la position 2D a l'ecran d'un point en 3D, avec en plus la position
        // Z buffer
        // ENTREES:	Position 3D
        // SORTIES:	X et Y a l'ecran, Z dans le Z-Buffer
        ///////////////////////////////////////////////////////////////////////////////
        private static void GetPositionEcran(OpenGL gl, Vecteur3Ddbl Pos, out float X, out float Y, out float Z)
        {
            double[] modelMatrix = new double[16];
            double[] projMatrix = new double[16];
            int[] viewport = new int[4];

            gl.GetDouble(OpenGL.GL_MODELVIEW_MATRIX, modelMatrix);
            gl.GetDouble(OpenGL.GL_PROJECTION_MATRIX, projMatrix);
            gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);

            double[] sx = new double[1];
            double[] sy = new double[1];
            double[] sz = new double[1];
            gl.Project(Pos.x, Pos.y, Pos.z, modelMatrix, projMatrix, viewport, sx, sy, sz); // Find position of light on screen
            X = (float)sx[0];
            Y = (float)sy[0];
            Z = (float)sz[0];
        }

    }

}
