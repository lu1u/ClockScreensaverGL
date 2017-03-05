using ClockScreenSaverGL.Config;
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    public class GravitationParticules : SystemeParticules2D.SystemeParticules2D
    {
        const String CAT = "GravitationParticules";
        static protected CategorieConfiguration c = Config.Configuration.getCategorie(CAT);

        static int NB_PARTICULES = c.getParametre("Nb Particules", 100);
        readonly int TIMER_CREATE = c.getParametre("Delai creation particule", 200);
        readonly float G = c.getParametre("G", 0.02f);
        readonly float MULT_DIST = c.getParametre("Mult dist", 1.0f);
        readonly float VITESSE_RECENTRE = c.getParametre("Vitesse recentre", 0.1f);

        public GravitationParticules(OpenGL gl) : base(gl, NB_PARTICULES)
        {

            AjouteTexture(c.getParametre( "Particule", Configuration.getImagePath( "particuleTexture.png" ) ), 1);

            AjouteEmetteur(new EmetteurGravitation(G, MULT_DIST, TIMER_CREATE));

            AjouteModificateur(new ModificateurVitesseLineaire());
            AjouteModificateur(new ModificateurAttracteurMutuelle(G, MULT_DIST));
            AjouteModificateur(new ModificateurVitesseLineaire());
            AjouteModificateur(new ModificateurRecentre(VITESSE_RECENTRE));
            AjouteModificateur(new ModificateurExclusion(MIN_X * 1.5f, MIN_Y * 1.5f, MAX_X * 1.5f, MAX_Y * 1.5f, ModificateurExclusion.Exclusions.EXCLURE_TOUT));
        }
        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
    }

}
