using System;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs
{
    class ModificateurCollisions : Modificateur
    {
        public override void Applique(SystemeParticules2D s, Temps maintenant)
        {
            for (int i = 0; i < s._nbParticules - 1; i++)
                if (s._particules[i].active)
                {
                    for (int j = i + 1; j < s._nbParticules; j++)
                        if (s._particules[j].active)
                            collision(ref s._particules[i], ref s._particules[j], maintenant.intervalleDepuisDerniereFrame);
                }
        }

        /// <summary>
        /// Calcul de la collision entre deux particules
        /// </summary>
        /// <param name="premiere"></param>
        /// <param name="deuxieme"></param>
        /// <param name="intervalle"></param>
        private void collision(ref Particule2D premiere, ref Particule2D deuxieme, float intervalle )
        {
            float sommeTailles = premiere.taille + deuxieme.taille;
            if (abs(premiere.x - deuxieme.x) > sommeTailles || abs(premiere.y - deuxieme.y) > sommeTailles)
                // Particules eloignees
                return;

            double distance = calculeDistance(premiere.x, premiere.y, deuxieme.x, deuxieme.y); 
            if (distance > sommeTailles)
                // Particules eloignees
                return;
        
            double distanceApres = calculeDistance(premiere.x + premiere.vx*intervalle, premiere.y + premiere.vy * intervalle, deuxieme.x + deuxieme.vx * intervalle, deuxieme.y + deuxieme.vy * intervalle);
            if (distanceApres > distance)
                // Particules deja en eloignement
                return;

            float vX1 = (premiere.vx * (premiere.taille - deuxieme.taille) +(2.0f * deuxieme.taille * deuxieme.vx)) / (premiere.taille + deuxieme.taille);
            float vY1 = (premiere.vy * (premiere.taille - deuxieme.taille) +(2.0f * deuxieme.taille * deuxieme.vy)) / (premiere.taille + deuxieme.taille);
            float vX2 = (deuxieme.vx * (deuxieme.taille - premiere.taille) +(2.0f * premiere.taille * premiere.vx)) / (premiere.taille + deuxieme.taille);
            float vY2 = (deuxieme.vy * (deuxieme.taille - premiere.taille) +(2.0f * premiere.taille * premiere.vy)) / (premiere.taille + deuxieme.taille);

            premiere.vx = vX1;
            premiere.vy = vY1;
            deuxieme.vx = vX2;
            deuxieme.vy = vY2;
        }

        /// <summary>
        /// Calcule la distance entre deux points
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        private double calculeDistance( float x1, float y1, float x2, float y2)
        {
            return Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
        }

        private static float abs(float v)
        {
            return v < 0 ? -v : v;
        }
    }
}
