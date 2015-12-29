using ClockScreenSaverGL.DisplayedObjects.Fonds.Systeme_Particules;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class EmetteurGravite: Emetteur
    {
        
        static RectangleF bounds = new RectangleF(SystemeParticules.MIN_X, SystemeParticules.MIN_Y, SystemeParticules.LARGEUR, SystemeParticules.HAUTEUR);
        static SizeF tailleEmetteur = new SizeF(0.2f, 0.2f);
        float _taille;
        float _vitesseParticule;
        TimerIsole _timer;        
        public EmetteurGravite(float taille, float vitesseParticule )
        {
            _taille = taille;
            _vitesseParticule = vitesseParticule;
            _timer = new TimerIsole(2, true);
        }
        public override void Deplace(SystemeParticules s, Temps maintenant, Color couleur)
        {
            // Ajouter une particule ?            
            if (_timer.Ecoule())
            {
                if( s._nbParticules < s.NB_MAX_PARTICULES)
                {
                    int indice = s.FindUnusedParticle();
                    s._particules[indice].x = DisplayedObject.FloatRandom(SystemeParticules.MIN_X, SystemeParticules.MAX_X);
                    s._particules[indice].y = DisplayedObject.FloatRandom(SystemeParticules.MIN_Y, SystemeParticules.MAX_Y);
                    s._particules[indice].debutVie = maintenant._totalMillisecondes;
                    s._particules[indice].finVie = maintenant._totalMillisecondes + 300000;
                    s._particules[indice].alpha = 1.0f;
                    s._particules[indice].vx = _vitesseParticule * DisplayedObject.FloatRandom(-0.1f, 0.1f);
                    s._particules[indice].vy = _vitesseParticule * DisplayedObject.FloatRandom(-0.1f, 0.1f);
                    s._particules[indice].taille = _taille * DisplayedObject.FloatRandom(0.5f, 1.5f);
                    s._particules[indice].textureIndex = r.Next(0, s._listeTextures.Count);
                    s._particules[indice].active = true;
                }
                s._trier = true;
            }
        }

        
    }



}
