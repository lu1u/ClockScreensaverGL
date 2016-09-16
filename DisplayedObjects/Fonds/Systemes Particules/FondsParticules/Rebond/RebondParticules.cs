using ClockScreenSaverGL.Config;
///
/// Particules qui rebondissent les unes contre les autres
///
///
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs;
using SharpGL;
using System;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class RebondParticules : SystemeParticules2D.SystemeParticules2D, IDisposable
    {
        #region Parametres
        const String CAT = "RebondParticules";
        static protected CategorieConfiguration c = Config.Configuration.getCategorie(CAT);
        static readonly int NB_PARTICULES = c.getParametre("Nb Particules", 50);
        readonly float GRAVITE_X = c.getParametre("Gravite X", 0.0f);
        readonly float GRAVITE_Y = c.getParametre("Gravite Y", -0.5f);
        readonly float TAILLE_PARTICULE = c.getParametre("TailleParticule", 0.05f);
        readonly float VITESSE_PARTICULE = c.getParametre("VitesseParticule", 0.2f);
        #endregion

        public RebondParticules(OpenGL gl) : base(gl, NB_PARTICULES)
        {
            // Ajouter les particules (pas d'emetteur: le nb de particules reste fixe)
            for (int i = 0; i < NB_PARTICULES; i++)
                AjouteParticule();

            couleurParticules = COULEUR_PARTICULES.BLANC;
            AjouteTexture(Configuration.getImagePath("balle.png"), 1);

            AjouteModificateur(new ModificateurGravite(GRAVITE_X, GRAVITE_Y));
            AjouteModificateur(new ModificateurRebond(MIN_X, MAX_X, MIN_Y, MAX_Y));
            AjouteModificateur(new ModificateurCollisions());
            AjouteModificateur(new ModificateurVitesseLineaire());
        }

        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
        /// <summary>
        /// Ajoute une particule
        /// </summary>
        private void AjouteParticule()
        {
            int indice = FindUnusedParticle();
            _particules[indice].x = FloatRandom(MIN_X, MAX_X);
            _particules[indice].y = FloatRandom(MIN_Y, MAX_Y);
            _particules[indice].alpha = 1;
            
            float vitesse = VITESSE_PARTICULE * FloatRandom(0.8f, 1.2f);
           _particules[indice].vx = FloatRandom( 0.01f, VITESSE_PARTICULE ) * SigneRandom();
           _particules[indice].vy = FloatRandom(0.01f, VITESSE_PARTICULE) * SigneRandom();
           _particules[indice].taille = FloatRandom(0.75f, 1.25f) * TAILLE_PARTICULE;
           _particules[indice].textureIndex = 0;
           _particules[indice].active = true;
        }
    }
}
