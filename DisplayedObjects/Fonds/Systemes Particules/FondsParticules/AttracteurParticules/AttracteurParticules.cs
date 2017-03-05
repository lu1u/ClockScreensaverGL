using System;
using SharpGL;
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D;
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs;
using ClockScreenSaverGL.Trajectoires;
using ClockScreenSaverGL.Config;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class AttracteurParticules : SystemeParticules2D.SystemeParticules2D, IDisposable
    {
        const String CAT = "AttracteurParticules";
        static protected CategorieConfiguration c = Config.Configuration.getCategorie(CAT);
        static readonly int NB_EMETTEURS = c.getParametre("Nb Emetteurs", 1);
        static readonly int NB_ATTRACTEURS = c.getParametre("Nb Emetteurs", 1);
        static readonly int NB_PARTICULES = c.getParametre("Nb Particules", 100000);
        static readonly int NB_PARTICULES_EMISES = c.getParametre("Nb ParticulesEmises", 10);
        readonly float ALPHA_MODIFIEUR = c.getParametre("Modifieur Alpha", 0.002f);
        readonly float TAILLE_PARTICULE = c.getParametre("TailleParticule", 0.012f);
        readonly float VITESSE_PARTICULE = c.getParametre("VitesseParticule", 0.04f);
        readonly float VITESSE_EMETTEUR = c.getParametre("VitesseEmetteur", 0.05f);
        readonly float VITESSE_ATTRACTEUR = c.getParametre("VitesseAttracteur", 0.02f);

        public AttracteurParticules(OpenGL gl) : base(gl, NB_PARTICULES)
        {
            AjouteTexture(c.getParametre( "Particule", Configuration.getImagePath( "particule.png" ) ), 1);
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

        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
    }
}
