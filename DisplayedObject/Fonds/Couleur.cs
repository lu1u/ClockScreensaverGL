/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 22/12/2014
 * Heure: 15:38
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using ClockScreenSaverGL.DisplayedObjects;
namespace ClockScreenSaverGL.DisplayedObjects.Fonds
{
    /// <summary>
    /// Description of Couleur.
    /// </summary>
    public class Couleur : Fond
    {
        const string CAT = "Couleur";
        static readonly byte FondCouleur = conf.getParametre(MainForm.CAT, "Valeur", (byte)100);

        protected List<DisplayedObject> listeObjets = new List<DisplayedObject>();

        public Couleur(OpenGL gl, int Cx, int Cy): base(gl)
        {
            int CentreX = Cx / 2;
            int CentreY = Cy / 2;

            listeObjets.Add(new Bandes.BandeHorizontale.BandeSeconde( gl, 50, CentreX, CentreY, Cx));
            listeObjets.Add(new Bandes.BandeHorizontale.BandeMinute( gl, 80, CentreX, CentreY + Bandes.BandeHorizontale.BandeHorizontale.TailleFonte * 2, Cx));
            listeObjets.Add(new Bandes.BandeHorizontale.BandeHeure( gl, 120, CentreX, CentreY + Bandes.BandeHorizontale.BandeHorizontale.TailleFonte * 4, Cx));

            // Bandes verticales
            listeObjets.Add(new Bandes.BandeVerticale.BandeHeure( gl, 120, CentreY, CentreX, Cx));
            listeObjets.Add(new Bandes.BandeVerticale.BandeMinute( gl, 80, CentreY, CentreX + Bandes.BandeVerticale.BandeVerticale.TailleFonte * 2, Cx));
            listeObjets.Add(new Bandes.BandeVerticale.BandeSeconde( gl, 50, CentreY, CentreX + Bandes.BandeVerticale.BandeVerticale.TailleFonte * 4, Cx));

        }

        /*
        public override void AfficheGDI(Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif

            foreach (DisplayedObject b in listeObjets)
                b.AfficheGDI(g, maintenant, tailleEcran, couleur);

#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif

        }
        */
        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.Clear(OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D(0.0, tailleEcran.Width, 0.0, tailleEcran.Height);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_BLEND);
            
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1 };
            gl.Color(col);
            gl.LineWidth(4);

            foreach (DisplayedObject b in listeObjets)
                b.AfficheOpenGL(gl, maintenant, tailleEcran, couleur);

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();
        }

        protected virtual Color getCouleur(Color couleur)
        {
            return getCouleurOpaqueAvecAlpha(couleur, FondCouleur);
        }

        public override void ClearBackGround(OpenGL gl, Color couleur)
        {
            Color c = getCouleur(couleur);
            gl.ClearColor(c.R / 256.0f, c.G / 256.0f, c.B / 256.0f, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT );
        }

        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif

            foreach (DisplayedObject b in listeObjets)
                b.Deplace(maintenant, tailleEcran);

#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }

    }
}
