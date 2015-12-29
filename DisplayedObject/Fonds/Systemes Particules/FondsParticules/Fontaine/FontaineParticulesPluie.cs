using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SharpGL;
using ClockScreenSaverGL.DisplayedObjects.Fonds.Systeme_Particules;
using ClockScreenSaverGL.DisplayedObjects.Fonds.Systeme_Particules.Modificateurs;
using ClockScreenSaverGL.DisplayedObjects.Fonds.Particules;
using SharpGL.SceneGraph.Assets;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.FontaineParticulesPluie
{
    class FontaineParticulesPluie : SystemeParticules, IDisposable
    {
        const String CAT = "Fontaine Pluie";
        static readonly int NB_PARTICULES = conf.getParametre(CAT, "Nb Particules", 2000);

        readonly int NB_EMETTEURS = 16;// conf.getParametre(CAT, "NB Emetteurs", 8);
        readonly float MODIFICATEUR_TAILLE = 0.01f;// conf.getParametre(CAT, "Modifieur Taille", 1.01f);
        readonly float MODIFICATEUR_ALPHA = 0.2f;// conf.getParametre(CAT, "Modifieur Alpha", 0.75f);
        readonly float VITESSE_X = 0.1f;
        readonly float VITESSE_Y = 0.7f;
        readonly float GRAVITE_X = 0.1f;
        readonly float GRAVITE_Y = -0.4f;
        

        public FontaineParticulesPluie(OpenGL gl): base( gl, NB_PARTICULES)
        {
            //AttributBlend = SystemeParticules.PARTICULES_BLEND_ADDITIVE;
            couleurParticules = SystemeParticules.COULEUR_PARTICULES.NOIR;
            typeFond = SystemeParticules.TYPE_FOND.FOND_COULEUR;
            for (int i = 0; i < NB_EMETTEURS; i++)
                AjouteEmetteur( new EmetteurPluie(VITESSE_X, VITESSE_Y));

            AjouteTexture(Resources.flare1);
            AjouteTexture(Resources.flare2);
            AjouteTexture(Resources.flare3);
            AjouteTexture(Resources.flare4);

            AjouteModificateur(new ModificateurExclusion(SystemeParticules.MIN_X,
               SystemeParticules.MIN_Y, SystemeParticules.MAX_X, SystemeParticules.MAX_Y,
               ModificateurExclusion.Exclusions.EXCLURE_EN_DESSOUS));
            AjouteModificateur(new ModificateurLife());
            AjouteModificateur(new ModificateurVitesseLineaire());
            AjouteModificateur(new ModificateurGravite(GRAVITE_X, GRAVITE_Y));
            AjouteModificateur(new ModificateurAlpha(MODIFICATEUR_ALPHA));
            AjouteModificateur(new ModificateurTaille(MODIFICATEUR_TAILLE));
        }
    }
}
