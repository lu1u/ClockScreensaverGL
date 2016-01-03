using ClockScreenSaverGL.DisplayedObjects.Fonds.Systeme_Particules;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class EmetteurGravite : Emetteur
    {

        static RectangleF bounds = new RectangleF(SystemeParticules.MIN_X, SystemeParticules.MIN_Y, SystemeParticules.LARGEUR, SystemeParticules.HAUTEUR);
        float _taille;
        float _vitesseParticule;
        TimerIsole _timer;
        float sensRotation = DisplayedObject.SigneRandom();

        public EmetteurGravite(float taille, float vitesseParticule)
        {
            _taille = taille;
            _vitesseParticule = vitesseParticule;
            _timer = new TimerIsole(1000, true);
        }
        public override void Deplace(SystemeParticules s, Temps maintenant, Color couleur)
        {
            // Ajouter une particule ?            
            if (_timer.Ecoule())
            {
                if( s._nbParticules < s.NB_MAX_PARTICULES)
                {
                    int indice = s.FindUnusedParticle();
                    /*
                    s._particules[indice].x = DisplayedObject.FloatRandom(SystemeParticules.MIN_X, SystemeParticules.MAX_X);
                    s._particules[indice].y = DisplayedObject.FloatRandom(SystemeParticules.MIN_Y, SystemeParticules.MAX_Y);
                    s._particules[indice].vx = _vitesseParticule * DisplayedObject.FloatRandom(-0.1f, 0.1f);
                    s._particules[indice].vy = _vitesseParticule * DisplayedObject.FloatRandom(-0.1f, 0.1f);
                    */
                    float Angle = DisplayedObject.FloatRandom(0, (float)Math.PI * 2);
                    float Distance = DisplayedObject.FloatRandom(0.01f, SystemeParticules.MAX_X);

                    // Position
                    s._particules[indice].x = Distance * (float)Math.Sin(Angle);
                    s._particules[indice].y = Distance * (float)Math.Cos(Angle);

                    // Vitesse
                    float vitesse = (float)Math.Sqrt(_vitesseParticule / Distance);
                    Angle += (float)Math.PI / 2.0f;
                    s._particules[indice].vx = vitesse * (float)Math.Sin(Angle) * sensRotation;
                    s._particules[indice].vy = vitesse * (float)Math.Cos(Angle) * sensRotation;

                    s._particules[indice].debutVie = maintenant._totalMillisecondes;
                    s._particules[indice].finVie = maintenant._totalMillisecondes + 300000;
                    s._particules[indice].alpha = 1.0f;
                    s._particules[indice].taille = _taille * DisplayedObject.FloatRandom(0.5f, 1.5f);
                    s._particules[indice].textureIndex = r.Next(0, s._listeTextures.Count);
                    s._particules[indice].active = true;
                }
                s._trier = true;
            }
        }


    }



}
