using System;
using SharpGL;
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D;
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs;
using ClockScreenSaverGL.Trajectoires;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class AttracteurParticules : SystemeParticules2D.SystemeParticules2D, IDisposable
    {
        const String CAT = "AttracteurParticules";
        static readonly int NB_EMETTEURS = conf.getParametre(CAT, "Nb Emetteurs", 1);
        static readonly int NB_ATTRACTEURS = conf.getParametre(CAT, "Nb Emetteurs", 1);
        static readonly int NB_PARTICULES = conf.getParametre(CAT, "Nb Particules", 100000);
        static readonly int NB_PARTICULES_EMISES = conf.getParametre(CAT, "Nb ParticulesEmises", 10);
        readonly float ALPHA_MODIFIEUR = conf.getParametre(CAT, "Modifieur Alpha", 0.002f);
        readonly float TAILLE_PARTICULE = conf.getParametre(CAT, "TailleParticule", 0.012f);
        readonly float VITESSE_PARTICULE = conf.getParametre(CAT, "VitesseParticule", 0.04f);
        readonly float VITESSE_EMETTEUR = conf.getParametre(CAT, "VitesseEmetteur", 0.05f);
        readonly float VITESSE_ATTRACTEUR = conf.getParametre(CAT, "VitesseAttracteur", 0.02f);

        public AttracteurParticules(OpenGL gl) : base(gl, NB_PARTICULES)
        {
            AjouteTexture(Config.getImagePath("particule.png"), 1);
            for (int i = 0; i < NB_EMETTEURS; i++)
            {
                Trajectoire t = new TrajectoireOvale(0, 0, MAX_X*0.8f, MAX_Y*0.8f, VITESSE_EMETTEUR * FloatRandom(0.5f, 1.5f), -(float)Math.PI/2.0f);
                AjouteEmetteur(new EmetteurJet(TAILLE_PARTICULE, VITESSE_PARTICULE, NB_PARTICULES_EMISES, t));
            }
            AttributBlend = PARTICULES_BLEND_ADDITIVE;
            AjouteModificateur(new ModificateurExclusion(SystemeParticules2D.SystemeParticules2D.MIN_X * 2,
                SystemeParticules2D.SystemeParticules2D.MIN_Y * 2, SystemeParticules2D.SystemeParticules2D.MAX_X * 2, SystemeParticules2D.SystemeParticules2D.MAX_Y * 2,
                ModificateurExclusion.Exclusions.EXCLURE_TOUT));

            AjouteModificateur(new ModificateurAlpha(ALPHA_MODIFIEUR, true));
            AjouteModificateur(new ModificateurLife());
            AjouteModificateur(new ModificateurVitesseLineaire());
            for (int i = 0; i < NB_ATTRACTEURS; i++)
            {
                Trajectoire t = new TrajectoireOvale(0, 0, MAX_X * 0.4f, MAX_Y * 0.4f, -VITESSE_ATTRACTEUR * FloatRandom(0.5f, 1.5f), (float)Math.PI / 2.0f);
                AjouteModificateur(new ModificateurAttracteur(t, FloatRandom(0.01f, 0.02f)));
            }
            AjouteModificateur(new ModificateurAlpha(ALPHA_MODIFIEUR));
        }
    }
}
