using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Systeme_Particules.Modificateurs
{
    class ModificateurAttracteurMutuelle : Modificateur
    {
        private float _g;
        const float SEUIL = 0.005f;

        static RectangleF bounds = new RectangleF(SystemeParticules.MIN_X, SystemeParticules.MIN_Y, SystemeParticules.LARGEUR, SystemeParticules.HAUTEUR);
        static SizeF tailleEmetteur = new SizeF(0.1f, 0.1f);
        public ModificateurAttracteurMutuelle(float G)
        {
            _g = G;
        }

        public override void Applique(SystemeParticules s, Temps maintenant)
        {
            int NbParticules = s._nbParticules;
            float dG = _g * maintenant._intervalle;

            for (int i = 0; i < NbParticules; i++)
                if (s._particules[i].active)
                    for (int j = i + 1; j < NbParticules; j++)
                        if (s._particules[j].active)
                        {
                            // Distance de la particule a l'attracteur
                            float distX = s._particules[i].x - s._particules[j].x;
                            float distY = s._particules[i].y - s._particules[j].y;
                            double dist = Math.Sqrt((distX * distX) + (distY * distY));
                            if (dist > SEUIL)
                            {
                                // Attraction proportionnelle a la distance au carre
                                double DistanceCube = dist * dist * dist;

                                s._particules[i].vx -= s._particules[j].taille * (float)(dG * (distX / DistanceCube));
                                s._particules[i].vy -= s._particules[j].taille * (float)(dG * (distY / DistanceCube));
                                s._particules[j].vx += s._particules[i].taille * (float)(dG * (distX / DistanceCube));
                                s._particules[j].vy += s._particules[i].taille * (float)(dG * (distY / DistanceCube));
                            }
                            else
                            {
                                s._particules[i].taille = (float)Math.Sqrt(s._particules[i].taille* s._particules[i].taille + s._particules[j].taille*s._particules[j].taille);
                                s._particules[j].active = false;
                            }
                        }

        }
    }
}
