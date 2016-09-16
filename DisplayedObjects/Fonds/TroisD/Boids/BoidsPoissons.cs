using ClockScreenSaverGL.Config;
using SharpGL;
using System;
using System.Drawing;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Boids
{
    class BoidsPoissons : Boids
    {
        const String CAT = "Boids Poissons";
        static CategorieConfiguration c = Config.Configuration.getCategorie(CAT);

        static readonly int NB = c.getParametre("Nb", 1000);
        static float MAX_SPEED = c.getParametre("Max Speed", 20f,true);
        static float MAX_FORCE = c.getParametre("Max force", 0.1f, true);
        static float TAILLE = c.getParametre("Taille", 2.5f, true);
        static float DISTANCE_VOISINS = c.getParametre("Distance voisins", 25.0f, true);
        static float SEPARATION = c.getParametre("Separation", 7.5f, true);
        static float VITESSE_ANIMATION = c.getParametre("Vitesse animation", 1.5f, true);
        static int NB_IMAGES_ANIMATION = c.getParametre("Nb Images animation", 10, true);
        static readonly float HAUTEUR_CORPS = 0.5f * TAILLE;
        static readonly float LONGUEUR_TETE = 0.75f * TAILLE;
        static readonly float LONGUEUR_CORPS = 1.25f * TAILLE;
        static readonly float LONGUEUR_QUEUE = -0.35f * TAILLE;
        static readonly float HAUTEUR_QUEUE = 0.35f * TAILLE;


        public BoidsPoissons(OpenGL gl) : base(gl, c, NB, TAILLE, MAX_SPEED, MAX_FORCE, DISTANCE_VOISINS, SEPARATION, VITESSE_ANIMATION, NB_IMAGES_ANIMATION)
        {
            c.setListenerParametreChange(onConfigurationChangee);
        }


        protected override void onConfigurationChangee(string valeur)
        {
            MAX_SPEED = c.getParametre("Max Speed", 20f, true);
            MAX_FORCE = c.getParametre("Max force", 0.1f, true);
            TAILLE = c.getParametre("Taille", 2.5f, true);
            DISTANCE_VOISINS = c.getParametre("Distance voisins", 25.0f, true);
            SEPARATION = c.getParametre("Separation", 7.5f, true);
            VITESSE_ANIMATION = c.getParametre("Vitesse animation", 1.5f, true);
            NB_IMAGES_ANIMATION = c.getParametre("Nb Images animation", 10, true);
            base.onConfigurationChangee(valeur);
        }

        public override void ClearBackGround(OpenGL gl, Color couleur)
        {
            couleur = getCouleurOpaqueAvecAlpha(couleur, 32);

            gl.ClearColor(couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1);

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        }


        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
        protected override void InitOpenGL(OpenGL gl, Temps maintenant, Color couleur)
        {
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1 };


            gl.Translate(0, 0, -MAX_Z * 1.5f);
            gl.Rotate(_angleCamera, _angleCamera, _angleCamera);


            gl.Disable(OpenGL.GL_ALPHA_TEST);
            gl.Disable(OpenGL.GL_CULL_FACE);
            gl.Disable(OpenGL.GL_BLEND);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.Enable(OpenGL.GL_DEPTH);
            gl.Disable(OpenGL.GL_FOG);
            /*gl.Fog(OpenGL.GL_FOG_MODE, OpenGL.GL_LINEAR);
            gl.Fog(OpenGL.GL_FOG_COLOR, col);
            gl.Fog(OpenGL.GL_FOG_END, 0);
            gl.Fog(OpenGL.GL_FOG_START, MAX_Z * 3.0f);*/
            setGlobalMaterial(gl, couleur);
            gl.Color(col);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, LIGHTPOS);


        }
        /// <summary>
        /// Preparation de la call list opengl pour dessiner un des boids
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="noImage"></param>
        protected override void DessineBoid(OpenGL gl, float noImage)
        {
            double angle = noImage * 2.0 * Math.PI;

            gl.Begin(OpenGL.GL_TRIANGLE_FAN);
            {
                // Corps Droit
                Vecteur3D.Z.Normal(gl); gl.Vertex(0, 0, -TAILLE * 0.5f);
                Vecteur3D.X.Normal(gl); gl.Vertex(LONGUEUR_TETE, TAILLE * 0.05f, 0);
                Vecteur3D.Y.Normal(gl); gl.Vertex(0, HAUTEUR_CORPS, 0);
                Vecteur3D.MOINS_X.Normal(gl); gl.Vertex(-LONGUEUR_CORPS, 0, 0);
                Vecteur3D.MOINS_Y.Normal(gl); gl.Vertex(0, -HAUTEUR_CORPS, 0);
                Vecteur3D.X.Normal(gl); gl.Vertex(LONGUEUR_TETE, -TAILLE * 0.05f, 0);
            }
            gl.End();

            gl.Begin(OpenGL.GL_TRIANGLE_FAN);
            {
                // Corps Gauche
                Vecteur3D.Z.Normal(gl); gl.Vertex(0, 0, -TAILLE * 0.5f);
                Vecteur3D.X.Normal(gl); gl.Vertex(LONGUEUR_TETE, -TAILLE * 0.05f, 0);
                Vecteur3D.MOINS_Y.Normal(gl); gl.Vertex(0, -HAUTEUR_CORPS, 0);
                Vecteur3D.MOINS_X.Normal(gl); gl.Vertex(-LONGUEUR_CORPS, 0, 0);
                Vecteur3D.Y.Normal(gl); gl.Vertex(0, HAUTEUR_CORPS, 0);
                Vecteur3D.X.Normal(gl); gl.Vertex(LONGUEUR_TETE, TAILLE * 0.05f, 0);
            }
            gl.End();

            gl.Begin(OpenGL.GL_TRIANGLES);
            {
                /* // Nageoire horizontale
                 Vecteur3D.MOINS_Y.Normal(gl);
                 gl.Vertex(0, 0, 0);
                 gl.Vertex(-TAILLE * 0.5f, 0, TAILLE * 0.75f);
                 gl.Vertex(-TAILLE * 0.5f, 0, -TAILLE * 0.75f);
                 */
                // Queue
                float z = TAILLE * 0.5f * (float)Math.Sin(angle);
                float x = -LONGUEUR_CORPS + LONGUEUR_QUEUE * (float)Math.Abs(Math.Cos(angle));
                gl.Vertex(-LONGUEUR_CORPS, 0, 0);
                gl.Vertex(x, HAUTEUR_QUEUE, z);
                gl.Vertex(x, -HAUTEUR_QUEUE, z);
            }
            gl.End();
        }

        protected override Boid newBoid()
        {
            return new Boid(r.Next(-MAX_X, MAX_X), r.Next(-MAX_Y, MAX_Y), r.Next(-MAX_Z, MAX_Z));
        }
    }
}
