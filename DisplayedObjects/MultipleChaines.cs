﻿using ClockScreenSaverGL.DisplayedObjects.Fonds;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ClockScreenSaverGL.DisplayedObjects.Metaballes;
using System.Windows.Forms;
using ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Opengl;
using ClockScreenSaverGL.DisplayedObjects.Fonds.Particules;
using ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity;
using ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds
{
    class MultiplesChaines : Fond

    {
        const String CAT = "MultiChaines";
        static readonly float ANGLE_ECRANS = conf.getParametre(CAT, "Angle ecrans", 180.0f);
        static readonly int NB_ECRANS_LARGEUR   = conf.getParametre(CAT, "Nb ecrans largeur", 6);
        static readonly int NB_ECRANS_HAUTEUR = conf.getParametre(CAT, "Nb ecrans Hauteur", 3);
        static readonly float MARGE_ECRAN       = conf.getParametre(CAT, "Marge ecrans", 0.3f);
        static readonly float HAUTEUR_ECRAN = conf.getParametre(CAT, "Hauteur ecrans", 0.75f);
        static readonly float LARGEUR_ECRAN = conf.getParametre(CAT, "Largeur ecrans", 0.9f);
        static readonly float RAYON_RONDE = conf.getParametre(CAT, "Rayon courbe", 4.0f);
        static readonly int NB_CHAINES = conf.getParametre(CAT, "Nb chaines", 4);
        static readonly int LARGEUR_TEXTURE = conf.getParametre(CAT, "Largeur texture", 256);
        static readonly int HAUTEUR_TEXTURE = conf.getParametre(CAT, "Hauteur texture", 256);
        static readonly float FOV = conf.getParametre(CAT, "FOV", 75.0f);
        static readonly float VITESSE_PANORAMIQUE = conf.getParametre(CAT, "Vitesse panoramique", 0.5f);
        int[,] Chaines;
        DisplayedObject[] _objets;
        float angle = 0;
        TimerIsole timerNouvelleChaine = new TimerIsole(10000);
        TimerIsole timerEcranChangeChaine = new TimerIsole(1000);
        uint[] textures;

        ///////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="gl"></param>
        public MultiplesChaines(OpenGL gl) : base(gl)
        {
            Chaines = new int[NB_ECRANS_LARGEUR, NB_ECRANS_HAUTEUR];
            for (int i = 0; i < NB_ECRANS_LARGEUR; i++)
                for (int j = 0; j < NB_ECRANS_HAUTEUR; j++)
                    Chaines[i, j] = r.Next(NB_CHAINES);

            _objets = new DisplayedObject[NB_CHAINES];
            textures = new uint[NB_CHAINES];

            for (int i = 0; i < NB_CHAINES; i++)
            {
                _objets[i] = InitObjet(gl);
                textures[i] = createEmptyTexture(LARGEUR_TEXTURE, HAUTEUR_TEXTURE);
            }

        }

        ///////////////////////////////////////////////////////////////////////
        public override void Dispose()
        {
            for (int i = 0; i < NB_CHAINES; i++)
                deleteEmptyTexture(textures[i]);

            foreach (DisplayedObject o in _objets)
                o.Dispose();
        }

        ///////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initialisation d'un objet a afficher dans une des teles
        /// </summary>
        /// <param name="gl"></param>
        /// <returns></returns>
        protected DisplayedObject InitObjet(OpenGL gl)
        {
            switch (r.Next(16))
            {
                case 0: return new Neige(gl, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case 1: return new Encre(gl, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case 2: return new Bacteries(gl, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case 3: return new Life(gl);
                case 4: return new Couronnes(gl);
                case 5: return new Nuages2(gl);
                case 6: return new Tunnel(gl);
                case 7: return new CarresEspace(gl);
                case 8: return new GravitationParticules(gl);
                case 9: return new TerreOpenGL(gl);
                case 10: return new ParticulesGalaxie(gl);
                case 11: return new ParticulesFusees(gl);
                case 12: return new FeuDArtifice(gl);
                case 13: return new AttracteurParticules(gl);
                case 14: return new Engrenages(gl);
                case 15: return new ADN(gl);
                default:
                    return new Metaballes.Metaballes(gl, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
            }
        }

        ///////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Affichage OpenGL
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleur"></param>
        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            if (timerNouvelleChaine.Ecoule())
            {
                int noChaine = r.Next(NB_CHAINES);
                _objets[noChaine].Dispose();
                _objets[noChaine] = InitObjet(gl);
            }

            RenderToTexture(gl, maintenant, tailleEcran, couleur);
            gl.ClearColor(couleur.R / 1024.0f, couleur.G / 1024.0f, couleur.B / 1024.0f, 1);				// Set The Clear Color To Medium Blue
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);      // Clear The Screen And Depth Buffer

             //gl.LoadIdentity();
            gl.Enable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_DEPTH_TEST);

            gl.MatrixMode(OpenGL.GL_PROJECTION);                        // Select The Projection Matrix
            gl.LoadIdentity();                                   // Reset The Projection Matrix

            // Calculate The Aspect Ratio Of The Window
            gl.Perspective(FOV, (float)tailleEcran.Width / (float)tailleEcran.Height, 1.0f, RAYON_RONDE*2);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);                         // Select The Modelview Matrix
            gl.LoadIdentity();									// Reset The Modelview Matri
            
            gl.LookAt(0, 0f, -RAYON_RONDE*0.3f, 0, 0, 0, 0, 1, 0);
            gl.Rotate(0, (float)Math.Sin(angle) *6.0f, 0);
            // Disable AutoTexture Coordinates
            // gl.Disable(OpenGL.GL_TEXTURE_GEN_S);
            //gl.Disable(OpenGL.GL_TEXTURE_GEN_T);
            //gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture);

            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1 };
            gl.Color(col);

            float ANGLE = ANGLE_ECRANS / NB_ECRANS_LARGEUR;
            gl.Rotate(0, -(ANGLE_ECRANS*1.2f)/2.0f, 0);
            for (int x = 0; x < NB_ECRANS_LARGEUR; x++)
            {
                gl.Rotate(0, ANGLE, 0);
                gl.PushMatrix();
                gl.Translate(0, -(HAUTEUR_ECRAN + MARGE_ECRAN) * NB_ECRANS_HAUTEUR * 0.8f, 0);
                for (int y = 0; y < NB_ECRANS_HAUTEUR; y++)
                {
                    // Front Face
                    gl.Disable(OpenGL.GL_TEXTURE_2D);
                    gl.Begin(OpenGL.GL_QUADS);
                    gl.Normal(0.0f, 0.0f, 1.0f);                  // Normal Pointing Towards Viewer
                    gl.TexCoord(0.0f, 0.0f); gl.Vertex(-LARGEUR_ECRAN*1.05f, -HAUTEUR_ECRAN * 1.05f, RAYON_RONDE * 1.01f);  // Point 1 (Front)
                    gl.TexCoord(1.0f, 0.0f); gl.Vertex(LARGEUR_ECRAN * 1.05f, -HAUTEUR_ECRAN * 1.05f, RAYON_RONDE * 1.01f);  // Point 2 (Front)
                    gl.TexCoord(1.0f, 1.0f); gl.Vertex(LARGEUR_ECRAN * 1.05f, HAUTEUR_ECRAN * 1.05f, RAYON_RONDE * 1.01f);  // Point 3 (Front)
                    gl.TexCoord(0.0f, 1.0f); gl.Vertex(-LARGEUR_ECRAN * 1.05f, HAUTEUR_ECRAN * 1.05f, RAYON_RONDE * 1.01f);  // Point 4 (Front)
                    gl.End();

                    gl.Enable(OpenGL.GL_TEXTURE_2D);
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, textures[Chaines[x, y]]);
                    gl.Begin(OpenGL.GL_QUADS);
                    gl.Normal(0.0f, 0.0f, 1.0f);                  // Normal Pointing Towards Viewer
                    gl.TexCoord(1.0f, 0.0f); gl.Vertex(-LARGEUR_ECRAN, -HAUTEUR_ECRAN, RAYON_RONDE);  // Point 1 (Front)
                    gl.TexCoord(0.0f, 0.0f); gl.Vertex(LARGEUR_ECRAN, -HAUTEUR_ECRAN, RAYON_RONDE);  // Point 2 (Front)
                    gl.TexCoord(0.0f, 1.0f); gl.Vertex(LARGEUR_ECRAN, HAUTEUR_ECRAN, RAYON_RONDE);  // Point 3 (Front)
                    gl.TexCoord(1.0f, 1.0f); gl.Vertex(-LARGEUR_ECRAN, HAUTEUR_ECRAN, RAYON_RONDE);  // Point 4 (Front)
                    gl.End();

                    gl.Translate(0, (HAUTEUR_ECRAN + MARGE_ECRAN) * 2.0f, 0);
                }

                gl.PopMatrix();
            }

            Console c = Console.getInstance(gl);
            foreach (DisplayedObject o in _objets)
                c.AddLigne(Color.Green, o.GetType().Name);

#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }
        public void RenderToTexture(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
            Rectangle r = new Rectangle(0, 0, LARGEUR_TEXTURE, HAUTEUR_TEXTURE);
            gl.Viewport(0, 0, r.Width, r.Height);                    // Set Our Viewport (Match Texture Size)

            for (int i = 0; i < NB_CHAINES; i++)
            {
                gl.PushAttrib(OpenGL.GL_ENABLE_BIT|OpenGL.GL_COLOR_BUFFER_BIT);

                _objets[i].ClearBackGround(gl, couleur);
                _objets[i].AfficheOpenGL(gl, maintenant, r, couleur);

                gl.Enable(OpenGL.GL_TEXTURE_2D);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, textures[i]);          // Bind To The Blur Texture

                // Copy Our ViewPort To The Blur Texture (From 0,0 To 128,128... No Border)
                gl.CopyTexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB16, 0, 0, LARGEUR_TEXTURE, HAUTEUR_TEXTURE, 0);

                gl.PopAttrib();
            }

            gl.Viewport(0, 0, tailleEcran.Width, tailleEcran.Height);
        }


        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
            base.Deplace(maintenant, tailleEcran);
            angle += maintenant._intervalle * VITESSE_PANORAMIQUE;
            foreach (DisplayedObject o in _objets)
                o.Deplace(maintenant, tailleEcran);

            if (timerEcranChangeChaine.Ecoule())
            {
                int x = r.Next(NB_ECRANS_LARGEUR);
                int y = r.Next(NB_ECRANS_HAUTEUR);

                Chaines[x,y] = (Chaines[x, y]+1) % NB_CHAINES ; 
            }
        }
    }
}
