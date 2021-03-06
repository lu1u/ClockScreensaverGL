﻿/***
 * Printemps: un arbre qui pousse
 * Inspire de http://www.jgallant.com/blog/
 */

using ClockScreenSaverGL.Config;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Printemps
{
    class Printemps : Fond
    {
        #region PARAMETRES
        const String CAT = "Tree";
        static public CategorieConfiguration c = Config.Configuration.getCategorie(CAT);
        public static readonly byte ALPHA = (byte)c.getParametre("ALPHA", 128);
        public static readonly int DELAI_RECOMMENCE = c.getParametre("Delai nouvel arbre", 10) * 1000;
        public static readonly float LARGEUR_TRONC = c.getParametre("Largeur Tronc", 10);
        public static readonly int HAUTEUR_TRONC = c.getParametre("Hauteur Tronc", 200);
        public static readonly int LARGEUR_ARBRE = c.getParametre("Largeur Arbre", 1200);
        public static readonly int HAUTEUR_ARBRE = c.getParametre("Hauteur Arbre", 400);
        public static readonly int LONGUEUR_BRANCHE = c.getParametre("Longueur Branche", 7);
        public static readonly int DISTANCE_MIN = c.getParametre("Distance Min", 5);
        public static readonly int DISTANCE_MAX = c.getParametre("Distance Max", 100);
        public static readonly int NB_CIBLES = c.getParametre("Nb Cibles", 200);
        float _oscillation = 0;
        
        #endregion
        DateTime _finCroissance;
        Tree _tree;
        public Printemps(OpenGL gl, int LargeurEcran, int HauteurEcran) : base(gl)
        {
            _tree = new Tree(LargeurEcran, HauteurEcran * FloatRandom( 0.4f, 0.6f), 0, 
                LARGEUR_TRONC, LARGEUR_ARBRE, HAUTEUR_ARBRE, LONGUEUR_BRANCHE, DISTANCE_MIN, DISTANCE_MAX, NB_CIBLES, HAUTEUR_TRONC);
        }


        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D(0.0, tailleEcran.Width, tailleEcran.Height, 0.0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Disable(OpenGL.GL_BLEND);
           
            gl.Color(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Disable(OpenGL.GL_BLEND);
            _tree.Draw(gl);

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }
        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
        public override void ClearBackGround(OpenGL gl, Color c)
        {
            c = getCouleurOpaqueAvecAlpha(c, ALPHA);

            gl.ClearColor(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            if (_tree.DoneGrowing)
            {
                if (maintenant.temps.Subtract(_finCroissance).TotalMilliseconds > DELAI_RECOMMENCE)
                    _tree = new Tree(tailleEcran.Width, tailleEcran.Height * 0.3f, 0, LARGEUR_TRONC, LARGEUR_ARBRE, HAUTEUR_ARBRE, LONGUEUR_BRANCHE, DISTANCE_MIN, DISTANCE_MAX, NB_CIBLES, HAUTEUR_TRONC);
            }
            else
                _finCroissance = maintenant.temps;
            if (UneFrameSur(2))
                _tree.Grow();
            _oscillation += maintenant.intervalleDepuisDerniereFrame * 1.5f;
            _tree.Oscillation((float)Math.Sin(_oscillation) * 0.02f);
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }
    }
}