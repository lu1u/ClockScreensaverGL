using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class EmetteurFeuArtifice: Emetteur2D
    {
        readonly int NB_PARTICULES = 1000;
        
        static RectangleF bounds = new Rectangle(-1, -1, 2, 2);
        float _taille;
        float _vitesseParticule;
        TimerIsole _timer;

        public EmetteurFeuArtifice(float taille, float vitesseParticule, int nbParticules)
        {
            NB_PARTICULES = nbParticules;
            _taille = taille;
            _vitesseParticule = vitesseParticule;
            _timer = new TimerIsole(r.Next(100,2000), true);
        }
        public override void Deplace(SystemeParticules2D.SystemeParticules2D s, Temps maintenant, Color couleur)
        {
            // Ajouter une particule ?            
            if (_timer.Ecoule())
            {
                float X = DisplayedObject.FloatRandom(SystemeParticules2D.SystemeParticules2D.MIN_X, SystemeParticules2D.SystemeParticules2D.MAX_X);
                float Y = DisplayedObject.FloatRandom(SystemeParticules2D.SystemeParticules2D.MIN_Y, SystemeParticules2D.SystemeParticules2D.MAX_Y);
                for (int i = 0; i < NB_PARTICULES; i++)
                {
                    int indice = s.FindUnusedParticle();
                    s._particules[indice].x = X;
                    s._particules[indice].y = Y;
                    s._particules[indice].alpha = 1;
                    s._particules[indice].debutVie = maintenant.totalMilliSecondes;
                    s._particules[indice].finVie = maintenant.totalMilliSecondes + 2000;

                    float vitesse = DisplayedObject.FloatRandom(0.01f, 1.0f) * _vitesseParticule;
                    float angle = DisplayedObject.FloatRandom(0, (float)Math.PI*2.0f);

                    s._particules[indice].vx = (float)Math.Sin(angle) * vitesse;
                    s._particules[indice].vy = (float)Math.Cos(angle) * vitesse;
                    s._particules[indice].taille = _taille * DisplayedObject.FloatRandom(0.8f, 1.2f);
                    s._particules[indice].textureIndex = r.Next(0, s._listeTextures.Count);
                    s._particules[indice].active = true;

                    s._particules[indice]._couleur[0] = couleur.R / 512.0f;
                    s._particules[indice]._couleur[1] = couleur.G / 512.0f;
                    s._particules[indice]._couleur[2] = couleur.B / 512.0f;
                    s._particules[indice]._couleur[3] = 1;
                    s._particules[indice]._couleurIndividuelle = true;
                }
                s._trier = true;
                _timer.Intervalle = r.Next(500, 2000);
            }
        }

        
    }



}
