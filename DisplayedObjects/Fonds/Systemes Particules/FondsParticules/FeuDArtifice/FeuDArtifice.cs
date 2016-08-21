using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SharpGL;
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D;
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs;
using SharpGL.SceneGraph.Assets;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class FeuDArtifice : SystemeParticules2D.SystemeParticules2D, IDisposable
    {
        const String CAT = "FeuDArtifice";
        static readonly int NB_EMETTEURS = 1;// conf.getParametre(CAT, "Nb Emetteurs", 5);
        static readonly int NB_PARTICULES = 5000;// conf.getParametre(CAT, "Nb Particules", 600);
        readonly float GRAVITE_X = 0;// conf.getParametre(CAT, "Gravite X", 0.05f);
        readonly float GRAVITE_Y = -0.5f;// conf.getParametre(CAT, "Gravite Y", 0.2f);
        readonly float ALPHA_MODIFIEUR = 0.4f;// conf.getParametre(CAT, "Modifieur Alpha", 0.05f);
        readonly float TAILLE_MODIFIEUR = 0.001f;// conf.getParametre(CAT, "Modifieur Taille", 1.1f);
        readonly float TAILLE_PARTICULE = 0.002f;// conf.getParametre(CAT, "TailleParticule", 0.02f);
        readonly float VITESSE_PARTICULE = 0.5f;// conf.getParametre(CAT, "VitesseParticule", 0.2f);

        public FeuDArtifice(OpenGL gl) : base(gl, NB_PARTICULES)
        {
            for (int i = 0; i < NB_EMETTEURS; i++)
                AjouteEmetteur(new EmetteurFeuArtifice(TAILLE_PARTICULE, VITESSE_PARTICULE, 2000));

            AttributBlend = SystemeParticules2D.SystemeParticules2D.PARTICULES_BLEND_ADDITIVE;

            AjouteModificateur(new ModificateurExclusion(SystemeParticules2D.SystemeParticules2D.MIN_X,
                SystemeParticules2D.SystemeParticules2D.MIN_Y, SystemeParticules2D.SystemeParticules2D.MAX_X, SystemeParticules2D.SystemeParticules2D.MAX_Y,
                ModificateurExclusion.Exclusions.EXCLURE_A_DROITE | ModificateurExclusion.Exclusions.EXCLURE_A_GAUCHE | ModificateurExclusion.Exclusions.EXCLURE_EN_DESSOUS));

            AjouteModificateur(new ModificateurLife());
            AjouteModificateur(new ModificateurVitesseLineaire());
            AjouteModificateur(new ModificateurGravite(GRAVITE_X, GRAVITE_Y));
            AjouteModificateur(new ModificateurAlpha(ALPHA_MODIFIEUR));
            AjouteModificateur(new ModificateurTaille(TAILLE_MODIFIEUR));
        }
    }
}
