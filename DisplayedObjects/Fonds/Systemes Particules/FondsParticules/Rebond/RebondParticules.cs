using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D;
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs;
using SharpGL;
using System;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class RebondParticules : SystemeParticules2D.SystemeParticules2D, IDisposable
    {
        const String CAT = "RebondParticules";
        static readonly int NB_PARTICULES = 50;// conf.getParametre(CAT, "Nb Particules", 1000);
        readonly float GRAVITE_X = 0.0f;// conf.getParametre(CAT, "Gravite X", 0.05f);
        readonly float GRAVITE_Y = -0.8f;// conf.getParametre(CAT, "Gravite Y", 0.5f);
        readonly float TAILLE_PARTICULE = 0.02f;// conf.getParametre(CAT, "TailleParticule", 0.01f);
        readonly float VITESSE_PARTICULE = conf.getParametre(CAT, "VitesseParticule", 0.2f);
       
        public RebondParticules(OpenGL gl) : base(gl, NB_PARTICULES)
        {
            // Ajouter les particules (pas d'emetteur: le nb de particules reste fixe)
            for (int i = 0; i < NB_PARTICULES; i++)
                AjouteParticule();

            typeFond = TYPE_FOND.FOND_NOIR;
            couleurParticules = COULEUR_PARTICULES.BLANC;
            AjouteTexture(Config.getImagePath("particuleTexture.png"), 1);

            AjouteModificateur(new ModificateurVitesseLineaire());
            AjouteModificateur(new ModificateurGravite(GRAVITE_X, GRAVITE_Y));
            AjouteModificateur(new ModificateurRebond(MIN_X, MAX_X, MIN_Y, MAX_Y));
            AjouteModificateur(new ModificateurCollisions());
        }

        private void AjouteParticule()
        {
            DateTime maintenant = DateTime.Now;

            int indice = FindUnusedParticle();
            _particules[indice].x = FloatRandom(MIN_X, MAX_X);
            _particules[indice].y = FloatRandom(MIN_Y, MAX_Y);
            _particules[indice].alpha = 1;
            _particules[indice].debutVie =maintenant.Ticks;
            
            float vitesse = VITESSE_PARTICULE * FloatRandom(0.8f, 1.2f);
           _particules[indice].vx = FloatRandom( 0.01f, VITESSE_PARTICULE ) * SigneRandom();
           _particules[indice].vy = FloatRandom(0.01f, VITESSE_PARTICULE) * SigneRandom();
           _particules[indice].taille = FloatRandom(0.75f, 1.25f) * TAILLE_PARTICULE;
           _particules[indice].textureIndex = 0;
           _particules[indice].active = true;
        }
    }
}
