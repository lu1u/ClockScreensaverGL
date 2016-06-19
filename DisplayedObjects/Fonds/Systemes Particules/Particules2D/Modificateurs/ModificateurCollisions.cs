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
                            collision(ref s._particules[i], ref s._particules[j], maintenant._intervalle);
                }
        }

        private void collision(ref Particule2D firstBall, ref Particule2D secondBall, float intervalle )
        {
            float sommeTailles = firstBall.taille + secondBall.taille;
            if (abs(firstBall.x - secondBall.x) > sommeTailles || abs(firstBall.y - secondBall.y) > sommeTailles)
                return;

            double distance = Math.Sqrt(((firstBall.x - secondBall.x) * (firstBall.x - secondBall.x))
                                        + ((firstBall.y - secondBall.y) * (firstBall.y - secondBall.y)));

            if (distance > sommeTailles)
                return;

            float collisionPointX = ((firstBall.x * secondBall.taille) + (secondBall.x * firstBall.taille)) / sommeTailles;
            float collisionPointY = ((firstBall.y * secondBall.taille) + (secondBall.y * firstBall.taille)) / sommeTailles;
            float newVelX1 = (firstBall.vx * (firstBall.taille - secondBall.taille) +(2 * secondBall.taille * secondBall.vx)) / (firstBall.taille + secondBall.taille);
            float newVelY1 = (firstBall.vy * (firstBall.taille - secondBall.taille) +(2 * secondBall.taille * secondBall.vy)) / (firstBall.taille + secondBall.taille);
            float newVelX2 = (secondBall.vx * (secondBall.taille - firstBall.taille) +(2 * firstBall.taille * firstBall.vx)) / (firstBall.taille + secondBall.taille);
            float newVelY2 = (secondBall.vy * (secondBall.taille - firstBall.taille) +(2 * firstBall.taille * firstBall.vy)) / (firstBall.taille + secondBall.taille);

            firstBall.x = firstBall.x + newVelX1 * intervalle ;
            firstBall.y = firstBall.y + newVelY1 * intervalle;
            secondBall.x = secondBall.x + newVelX2 * intervalle;
            secondBall.y = secondBall.y + newVelY2 * intervalle;

            firstBall.vx = newVelX1;
            firstBall.vy = newVelY1;
            secondBall.vx = newVelX2;
            secondBall.vy = newVelY2;
        }

        private static float abs(float v)
        {
            return v < 0 ? -v : v;
        }
    }
}
