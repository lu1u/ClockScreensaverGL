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
    class ParticulesFusees : SystemeParticules, IDisposable
    {
        const String CAT = "ParticulesFusees";
        readonly int NB_EMETTEURS = conf.getParametre(CAT, "Nb Emetteurs", 5);
        static readonly int NB_PARTICULES = 500;// conf.getParametre(CAT, "Nb Particules", 1000);
        readonly float GRAVITE_X = 0.02f;// conf.getParametre(CAT, "Gravite X", 0.05f);
        readonly float GRAVITE_Y = 0.02f;// conf.getParametre(CAT, "Gravite Y", 0.5f);
        readonly float ALPHA_MODIFIEUR = 0.6f;// conf.getParametre(CAT, "Modifieur Alpha", 0.5f);
        readonly float TAILLE_MODIFIEUR = 0.05f;// conf.getParametre(CAT, "Modifieur Taille", 1.05f);
        readonly float TAILLE_PARTICULE = conf.getParametre(CAT, "TailleParticule", 0.01f);
        readonly float VITESSE_ANGLE = conf.getParametre(CAT, "VitesseAngle", 0.75f);
        readonly float VITESSE_PARTICULE = conf.getParametre(CAT, "VitesseParticule", 0.2f);
        readonly float VITESSE_FUSEE = conf.getParametre(CAT, "VitesseParticule", 0.5f);

        public ParticulesFusees(OpenGL gl): base(gl, NB_PARTICULES)
        {
            for (int i = 0; i < NB_EMETTEURS; i++)
                AjouteEmetteur(new EmetteurFusee(TAILLE_PARTICULE, VITESSE_ANGLE, VITESSE_PARTICULE, VITESSE_FUSEE));

            AttributBlend = SystemeParticules.PARTICULES_BLEND_ADDITIVE;
            typeFond = SystemeParticules.TYPE_FOND.FOND_COULEUR;
            couleurParticules = SystemeParticules.COULEUR_PARTICULES.BLANC;
            AjouteTexture(Resources.particleTexture);
            AjouteTexture(Resources.nuage1);
            AjouteTexture(Resources.nuage2);
            AjouteTexture(Resources.nuage3);

            AjouteModificateur(new ModificateurExclusion(SystemeParticules.MIN_X,
                SystemeParticules.MIN_Y, SystemeParticules.MAX_X, SystemeParticules.MAX_Y,
                ModificateurExclusion.Exclusions.EXCLURE_AU_DESSUS | ModificateurExclusion.Exclusions.EXCLURE_A_DROITE| ModificateurExclusion.Exclusions.EXCLURE_A_GAUCHE));
            AjouteModificateur(new ModificateurLife());
            AjouteModificateur(new ModificateurVitesseLineaire());
            AjouteModificateur(new ModificateurGravite(GRAVITE_X, GRAVITE_Y));
            AjouteModificateur(new ModificateurAlpha(ALPHA_MODIFIEUR));
            AjouteModificateur(new ModificateurTaille(TAILLE_MODIFIEUR));
        }
    }
}
