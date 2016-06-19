using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class EmetteurPluie : Emetteur2D
    {
        static RectangleF bounds = new Rectangle(-1, -1, 2, 2);
        static SizeF tailleEmetteur = new SizeF(0.2f, 0.2f);
        DateTime _derniereParticule = DateTime.Now;
        private float _vitesseX;
        private float _vitesseY;
        private TimerIsole timer = new TimerIsole(10);

        public EmetteurPluie(float vitesseX, float vitesseY)
        {
            _vitesseX = vitesseX;
            _vitesseY = vitesseY;
            _derniereParticule = _derniereParticule.Subtract(TimeSpan.FromMilliseconds(5000));
        }
        public override void Deplace(SystemeParticules2D.SystemeParticules2D s, Temps maintenant, Color couleur)
        {
            // Ajouter une particule ?            
            if (timer.Ecoule())
            {
                int indice = s.FindUnusedParticle();
                s._particules[indice].x = DisplayedObject.FloatRandom(SystemeParticules2D.SystemeParticules2D.MIN_X, SystemeParticules2D.SystemeParticules2D.MAX_X);
                s._particules[indice].y = SystemeParticules2D.SystemeParticules2D.MIN_Y * 0.8f;
                s._particules[indice].alpha = 1;
                s._particules[indice].debutVie = maintenant._totalMillisecondes;
                s._particules[indice].finVie = maintenant._totalMillisecondes + 20000;

                s._particules[indice].vx = (float)DisplayedObject.FloatRandom(-1f, 1f) * _vitesseX;
                s._particules[indice].vy = (float)DisplayedObject.FloatRandom(-0f, 2f) * _vitesseY;
                s._particules[indice].taille = 0.02f ;
                s._particules[indice].textureIndex = r.Next(0, s.nbImages);
                s._particules[indice].active = true;

                s._trier = true;
            }            
        }
    }
}
