using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SharpGL;
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D;
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs;
using ClockScreenSaverGL.DisplayedObjects.Fonds.Particules;
using SharpGL.SceneGraph.Assets;
using ClockScreenSaverGL.Config;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.FontaineParticulesPluie
{
    class FontaineParticulesPluie : SystemeParticules2D.SystemeParticules2D, IDisposable
    {
        const String CAT = "Fontaine Pluie";
        static protected CategorieConfiguration c = Config.Configuration.getCategorie(CAT);
        static readonly int NB_PARTICULES = c.getParametre("Nb Particules", 2000);

        readonly int NB_EMETTEURS = 16;// c.getParametre("NB Emetteurs", 8);
        readonly float MODIFICATEUR_TAILLE = 0.01f;// c.getParametre("Modifieur Taille", 1.01f);
        readonly float MODIFICATEUR_ALPHA = 0.2f;// c.getParametre("Modifieur Alpha", 0.75f);
        readonly float VITESSE_X = 0.1f;
        readonly float VITESSE_Y = 0.7f;
        readonly float GRAVITE_X = 0.1f;
        readonly float GRAVITE_Y = -0.4f;
        

        public FontaineParticulesPluie(OpenGL gl): base( gl, NB_PARTICULES)
        {
            //AttributBlend = SystemeParticules.PARTICULES_BLEND_ADDITIVE;
            couleurParticules = SystemeParticules2D.SystemeParticules2D.COULEUR_PARTICULES.NOIR;
            typeFond = SystemeParticules2D.SystemeParticules2D.TYPE_FOND.FOND_COULEUR;
            for (int i = 0; i < NB_EMETTEURS; i++)
                AjouteEmetteur( new EmetteurPluie(VITESSE_X, VITESSE_Y));

            AjouteTexture(Configuration.getImagePath(@"ete\flares.png"), 4);

            AjouteModificateur(new ModificateurExclusion(SystemeParticules2D.SystemeParticules2D.MIN_X,
               SystemeParticules2D.SystemeParticules2D.MIN_Y, SystemeParticules2D.SystemeParticules2D.MAX_X, SystemeParticules2D.SystemeParticules2D.MAX_Y,
               ModificateurExclusion.Exclusions.EXCLURE_EN_DESSOUS));
            AjouteModificateur(new ModificateurLife());
            AjouteModificateur(new ModificateurVitesseLineaire());
            AjouteModificateur(new ModificateurGravite(GRAVITE_X, GRAVITE_Y));
            AjouteModificateur(new ModificateurAlpha(MODIFICATEUR_ALPHA));
            AjouteModificateur(new ModificateurTaille(MODIFICATEUR_TAILLE));
        }

        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
    }
}
