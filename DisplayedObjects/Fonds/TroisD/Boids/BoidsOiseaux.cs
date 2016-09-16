﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using ClockScreenSaverGL.Config;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Boids
{
    class BoidsOiseaux : Boids
    {
        const String CAT = "Boids Oiseaux";
        static CategorieConfiguration c = Config.Configuration.getCategorie(CAT);

        static readonly int NB = c.getParametre("Nb", 1000);
        static float MAX_SPEED         = c.getParametre("Max Speed", 40.0f, true);
        static float MAX_FORCE         = c.getParametre("Max force", 0.4f, true);
        static float TAILLE            = c.getParametre("Taille", 2.0f, true);
        static float DISTANCE_VOISINS  = c.getParametre("Distance voisins", 25.0f, true);
        static float SEPARATION        = c.getParametre("Separation", 10.0f, true);
        static float VITESSE_ANIMATION = c.getParametre("Vitesse animation", 2.0f, true);
        static float DIMINUE_VITESSE_V = c.getParametre("Diminition vitesse verticale", 0.99f, true);
        static int NB_IMAGES_ANIMATION = c.getParametre("Nb Images animation", 10, true);
        static float DIMINUE_ACCELERATION_V = c.getParametre("Diminution acceleration verticale", 0.99f, true);

        Texture _texture = new Texture();
        

        public BoidsOiseaux(OpenGL gl) : base(gl, c, NB, TAILLE, MAX_SPEED, MAX_FORCE, DISTANCE_VOISINS, SEPARATION, VITESSE_ANIMATION, NB_IMAGES_ANIMATION)
        {
            _texture.Create(gl, Configuration.getImagePath("oiseau.png"));
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

        public override void Dispose()
        {
            base.Dispose();
            _texture?.Destroy(_gl);
        }


        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
        protected override void DessineBoid(OpenGL gl, float noImage)
        {
            double angle = noImage * 2.0 * Math.PI;

            gl.Begin(OpenGL.GL_QUAD_STRIP);
            gl.TexCoord(0, 0);          gl.Vertex( TAILLE, TAILLE * Math.Sin(angle), -TAILLE);
            gl.TexCoord(0, 1.0f);       gl.Vertex(-TAILLE, TAILLE * Math.Sin(angle), -TAILLE);

            gl.TexCoord(0.4f, 0.0f); gl.Vertex(+TAILLE, -TAILLE * 0.2 * Math.Sin(angle), -TAILLE * 0.2f);
            gl.TexCoord(0.4f, 1.0f); gl.Vertex(-TAILLE, -TAILLE * 0.2 * Math.Sin(angle), -TAILLE * 0.2f);

            gl.TexCoord(0.6f, 0.0f); gl.Vertex(+TAILLE, TAILLE * 0.2 * Math.Sin(angle), TAILLE * 0.2f);
            gl.TexCoord(0.6f, 1.0f); gl.Vertex(-TAILLE, TAILLE * 0.2 * Math.Sin(angle), TAILLE * 0.2f);

            gl.TexCoord(1.0f, 0.0f);    gl.Vertex( TAILLE, TAILLE * Math.Sin(angle), TAILLE);
            gl.TexCoord(1.0f, 1.0f);    gl.Vertex(-TAILLE, TAILLE * Math.Sin(angle), TAILLE);
            gl.End();
        }

        protected override void InitOpenGL(OpenGL gl, Temps maintenant, Color couleur)
        {
            couleur = getCouleurOpaqueAvecAlpha(couleur, 64);
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1 };

            gl.Disable(OpenGL.GL_COLOR_MATERIAL);

            gl.Translate(0, 0, -MAX_Z*2);
            //gl.Rotate(_angleCamera, _angleCamera, _angleCamera);

            
            gl.Disable(OpenGL.GL_ALPHA_TEST);
            gl.Disable(OpenGL.GL_CULL_FACE);
            gl.Enable(OpenGL.GL_BLEND);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            gl.Disable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_FOG);
            gl.Fog(OpenGL.GL_FOG_MODE, OpenGL.GL_LINEAR);
            gl.Fog(OpenGL.GL_FOG_COLOR, col);
            gl.Fog(OpenGL.GL_FOG_START, MAX_Z * 0.5f);
            gl.Fog(OpenGL.GL_FOG_END, MAX_Z * 4.0f);
            
            //gl.LineWidth(4.0f);
            //gl.Color(0.1f, 0.1f, 0.1f);
            //DessineCube(gl, MAX_X, MAX_Y, MAX_Z);


            gl.Color(0.0f,0.0f,0.0f,1.0f);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            _texture.Bind(gl);

            _boids.Sort(delegate (Boid O1, Boid O2)
            {
                if (O1._Position.z > O2._Position.z) return 1;
                if (O1._Position.z < O2._Position.z) return -1;
                return 0;
            });
        }

        /// <summary>
        /// Initialisation du tableau de boids
        /// </summary>
        /// <param name="_boids"></param>
        protected override void InitBoids(List<Boid> _boids)
        {
            for (int i = 0; i < NB_BOIDS; i++)
                _boids.Add(new BoidOiseau(r.Next(-MAX_X, MAX_X), r.Next(-MAX_Y, MAX_Y), r.Next(-MAX_Z, MAX_Z)));
        }
        public override void ClearBackGround(OpenGL gl, Color c)
        {
            c = getCouleurOpaqueAvecAlpha(c, 128);
            gl.ClearColor(c.R / 256.0f, c.G / 256.0f, c.B / 256.0f, 1);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        }

        protected override Boid newBoid()
        {
            return new BoidOiseau(r.Next(-MAX_X, MAX_X), r.Next(-MAX_Y, MAX_Y), r.Next(-MAX_Z, MAX_Z));
        }

        protected class BoidOiseau : Boid
        {
            public BoidOiseau(float x, float y, float z) : base(x,y,z)
            {

            }

            public override void update(Temps maintenant)
            {

                _Acceleration.y *= DIMINUE_ACCELERATION_V;
                _Vitesse.y *= DIMINUE_VITESSE_V;
                _couleur = Color.Black;
                base.update(maintenant);
            }
        }
    }
}
