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
using ClockScreenSaverGL.DisplayedObject;
namespace ClockScreenSaverGL.DisplayedObject.Fonds
{
    /// <summary>
    /// Description of Couleur.
    /// </summary>
    public class Couleur : Fond
    {
        const string CAT = "Couleur";
        static readonly byte FondCouleur = conf.getParametre(MainForm.CAT, "Valeur", (byte)100);

        protected List<DisplayedObject> listeObjets = new List<DisplayedObject>();

        public Couleur(OpenGL gl, int Cx, int Cy)
        {
            int CentreX = Cx / 2;
            int CentreY = Cy / 2;

            listeObjets.Add(new Bandes.BandeHorizontale.BandeSeconde( 50, CentreX, CentreY, Cx));
            listeObjets.Add(new Bandes.BandeHorizontale.BandeMinute( 80, CentreX, CentreY + Bandes.BandeHorizontale.BandeHorizontale.TailleFonte * 2, Cx));
            listeObjets.Add(new Bandes.BandeHorizontale.BandeHeure( 120, CentreX, CentreY + Bandes.BandeHorizontale.BandeHorizontale.TailleFonte * 4, Cx));

            // Bandes verticales
            listeObjets.Add(new Bandes.BandeVerticale.BandeHeure( 120, CentreY, CentreX, Cx));
            listeObjets.Add(new Bandes.BandeVerticale.BandeMinute( 80, CentreY, CentreX + Bandes.BandeVerticale.BandeVerticale.TailleFonte * 2, Cx));
            listeObjets.Add(new Bandes.BandeVerticale.BandeSeconde( 50, CentreY, CentreX + Bandes.BandeVerticale.BandeVerticale.TailleFonte * 4, Cx));

        }

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

        protected virtual Color getCouleur(Color couleur)
        {
            return getCouleurOpaqueAvecAlpha(couleur, FondCouleur);
        }

        public override void ClearBackGround(OpenGL gl, Color couleur)
        {
            Color c = getCouleur(couleur);
            gl.ClearColor(c.R / 512.0f, c.G / 512.0f, c.B / 512.0f, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT );
        }

        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif

            foreach (DisplayedObject b in listeObjets)
                b.Deplace(maintenant, ref tailleEcran);

#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }

    }
}
