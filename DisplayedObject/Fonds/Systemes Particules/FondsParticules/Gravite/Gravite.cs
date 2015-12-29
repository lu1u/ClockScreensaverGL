using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SharpGL;
using ClockScreenSaverGL.DisplayedObjects.Fonds.Systeme_Particules;
using ClockScreenSaverGL.DisplayedObjects.Fonds.Systeme_Particules.Modificateurs;
using SharpGL.SceneGraph.Assets;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class Gravite : SystemeParticules, IDisposable
    {
        const String CAT = "Gravite";
        static readonly int NB_EMETTEURS = conf.getParametre(CAT, "Nb Emetteurs", 1);
        static readonly int NB_ATTRACTEURS = conf.getParametre(CAT, "Nb Emetteurs", 1);
        static readonly int NB_PARTICULES = 200000;// conf.getParametre(CAT, "Nb Particules", 100000);
        readonly float TAILLE_PARTICULE = 0.01f;// conf.getParametre(CAT, "TailleParticule", 0.01f);
        readonly float VITESSE_PARTICULE = 0.75f;// conf.getParametre(CAT, "VitesseParticule", 0.5f);

        public Gravite(OpenGL gl) : base(gl, NB_PARTICULES)
        {


            AjouteTexture(Resources.particleTexture);
            AjouteEmetteur(new EmetteurGravite(TAILLE_PARTICULE, VITESSE_PARTICULE));
            
            //AttributBlend = PARTICULES_BLEND_ADDITIVE;
            AjouteModificateur(new ModificateurExclusion(SystemeParticules.MIN_X * 2,
                SystemeParticules.MIN_Y * 2, SystemeParticules.MAX_X * 2, SystemeParticules.MAX_Y * 2,
                ModificateurExclusion.Exclusions.EXCLURE_TOUT));

            AjouteModificateur(new ModificateurRecentre(0.5f));
            AjouteModificateur(new ModificateurAttracteurMutuelle(0.01f));
            AjouteModificateur(new ModificateurVitesseLineaire());
        }
    }
}
