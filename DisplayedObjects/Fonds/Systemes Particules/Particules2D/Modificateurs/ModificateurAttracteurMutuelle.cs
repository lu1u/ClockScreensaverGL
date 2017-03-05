using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs
{
    class ModificateurAttracteurMutuelle : Modificateur
    {
        private float _g;
        private float _multDist;
        const float SEUIL = 0.2f;

        static RectangleF bounds = new RectangleF(SystemeParticules2D.MIN_X, SystemeParticules2D.MIN_Y, SystemeParticules2D.LARGEUR, SystemeParticules2D.HAUTEUR);
        static SizeF tailleEmetteur = new SizeF(0.1f, 0.1f);
        public ModificateurAttracteurMutuelle(float G, float MultDist)
        {
            _g = G;
            _multDist = MultDist;
        }

        public override void Applique(SystemeParticules2D s, Temps maintenant)
        {
            int NbParticules = s._nbParticules;
            float dG = _g * maintenant.intervalleDepuisDerniereFrame;

            for (int i = 0; i < NbParticules; i++)
                if (s._particules[i].active)
                    for (int j = i + 1; j < NbParticules; j++)
                        if (s._particules[j].active)
                        {
                            // Distance de la particule a l'attracteur
                            float distX = (s._particules[i].x - s._particules[j].x)*_multDist;
                            float distY = (s._particules[i].y - s._particules[j].y)*_multDist;
                            double dist = Math.Sqrt((distX * distX) + (distY * distY));
                            if (dist > (s._particules[i].taille+ s._particules[j].taille) * SEUIL)
                            {
                                // Attraction proportionnelle a la distance au carre
                                double DistanceCube = dist * dist;

                                s._particules[i].vx -= s._particules[j].taille * (float)(dG * (distX / DistanceCube));
                                s._particules[i].vy -= s._particules[j].taille * (float)(dG * (distY / DistanceCube));
                                s._particules[j].vx += s._particules[i].taille * (float)(dG * (distX / DistanceCube));
                                s._particules[j].vy += s._particules[i].taille * (float)(dG * (distY / DistanceCube));
                            }
                            else
                            {
                                // Particule rentrent en contact -> fusionnent
                                s._particules[i].vx = ((s._particules[i].vx * s._particules[i].taille) + (s._particules[j].vx * s._particules[j].taille)) / (s._particules[i].taille + s._particules[j].taille);
                                s._particules[i].vy = ((s._particules[i].vy * s._particules[i].taille) + (s._particules[j].vy * s._particules[j].taille)) / (s._particules[i].taille + s._particules[j].taille);
                                s._particules[i].taille = (float)Math.Sqrt((s._particules[i].taille * s._particules[i].taille) + (s._particules[j].taille * s._particules[j].taille));
                                s._particules[j].active = false;
                            }
                        }

        }
    }
}
