using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    public class GravitationParticules: SystemeParticules2D.SystemeParticules2D, IDisposable
    {
        const String CAT = "GravitationParticules";
        static readonly int NB_PARTICULES = conf.getParametre(CAT, "Nb Particules", 100);
        static readonly int TIMER_CREATE = conf.getParametre(CAT, "Delai creation particule", 200);
        static readonly float G = 0.02f;// conf.getParametre(CAT, "G", 0.1f);
        static readonly float VITESSE_RECENTRE = 0.1f;// conf.getParametre(CAT, "G", 0.1f);

        public GravitationParticules(OpenGL gl) : base(gl, NB_PARTICULES)
        {

            AjouteTexture(Resources.particleTexture);

            AjouteEmetteur(new EmetteurGravitation(G, TIMER_CREATE));

            AjouteModificateur(new ModificateurVitesseLineaire());
            AjouteModificateur(new ModificateurAttracteurMutuelle(G));
            AjouteModificateur(new ModificateurVitesseLineaire());
            AjouteModificateur(new ModificateurRecentre(VITESSE_RECENTRE));
            AjouteModificateur(new ModificateurExclusion(MIN_X * 1.5f, MIN_Y * 1.5f, MAX_X * 1.5f, MAX_Y * 1.5f, ModificateurExclusion.Exclusions.EXCLURE_TOUT));
            }
    }
}
