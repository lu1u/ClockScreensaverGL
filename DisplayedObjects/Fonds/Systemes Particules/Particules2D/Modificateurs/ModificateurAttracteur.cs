using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs
{
    class ModificateurAttracteur : Modificateur
    {
        private float _g;
        private Trajectoire _traj;
        const float SEUIL = 0.000002f;

        static RectangleF bounds = new RectangleF(SystemeParticules2D.MIN_X, SystemeParticules2D.MIN_Y, SystemeParticules2D.LARGEUR, SystemeParticules2D.HAUTEUR);
        static SizeF tailleEmetteur = new SizeF(0.1f, 0.1f);
        public ModificateurAttracteur(Trajectoire t, float G)
        {
            _traj = t;
            _g = G;
        }

        public override void Applique(SystemeParticules2D s, Temps maintenant)
        {

            _traj.Avance(bounds, tailleEmetteur, maintenant);
            float dG = _g * maintenant._intervalle;
            int NbParticules = s._nbParticules;

            for (int i = 0; i < NbParticules; i++)
                if (s._particules[i].active)
                {
                    // Distance de la particule a l'attracteur
                    float distX = s._particules[i].x - _traj._Px;
                    float distY = s._particules[i].y - _traj._Py;
                    double dist = Math.Sqrt((distX * distX) + (distY * distY));

                    // Attraction proportionnelle a la distance au carre
                    double DistanceCube = dist * dist * dist;

                    if (DistanceCube > SEUIL )
                    {
                        s._particules[i].vx -= (float)(dG * (distX / DistanceCube));
                        s._particules[i].vy -= (float)(dG * (distY / DistanceCube));
                    }
                    else
                    {
                        s._particules[i].vx = _traj._Vx;
                        s._particules[i].vy = _traj._Vy;
                    }
                }
        
    }
}
}
