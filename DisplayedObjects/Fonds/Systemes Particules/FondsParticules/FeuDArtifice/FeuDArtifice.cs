using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SharpGL;
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D;
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs;
using SharpGL.SceneGraph.Assets;
using ClockScreenSaverGL.Config;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class FeuDArtifice : SystemeParticules2D.SystemeParticules2D, IDisposable
    {
        const String CAT = "FeuDArtifice";
        static CategorieConfiguration c = Configuration.getCategorie(CAT);
        static readonly int NB_EMETTEURS =c.getParametre("Nb Emetteurs", 1);
        static readonly int NB_PARTICULES =c.getParametre("Nb Particules", 5000);
        readonly float GRAVITE_X = c.getParametre("Gravite X", 0.0f, true);
        readonly float GRAVITE_Y = -c.getParametre("Gravite Y", -0.5f, true);
        readonly float ALPHA_MODIFIEUR = c.getParametre("Modifieur Alpha", 0.4f, true);
        readonly float TAILLE_MODIFIEUR = c.getParametre("Modifieur Taille", 0.001f);
        readonly float TAILLE_PARTICULE = c.getParametre("TailleParticule", 0.002f);
        readonly float VITESSE_PARTICULE =  c.getParametre("VitesseParticule", 0.5f);

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

        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
    }
}
