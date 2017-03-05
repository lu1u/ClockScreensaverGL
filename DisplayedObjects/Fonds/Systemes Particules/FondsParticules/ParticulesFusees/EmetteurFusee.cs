using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D
{
    class EmetteurFusee : Emetteur2D
    {
        static RectangleF bounds = new Rectangle(-1, -1, 2, 2);
        static SizeF tailleEmetteur = new SizeF(0.2f, 0.2f);
        DateTime _derniereParticule = DateTime.Now;
        float _taille;
        float _vitesseAngle;
        float _vitesseParticule;


        float _vitesse;
        float _angle = 0;
        float _pX, _pY;

        TimerIsole _timer = new TimerIsole(10);

        public EmetteurFusee(float taille, float vitesseAngle,float vitesseParticule, float vitesseFusee)
        {
            _vitesseAngle = vitesseAngle * DisplayedObject.FloatRandom(0.5f, 2.0f) * DisplayedObject.SigneRandom();
            _taille = taille * DisplayedObject.FloatRandom(0.5f, 1.5f) ;
            _vitesseParticule = vitesseParticule;
            _vitesse = vitesseFusee;
            _angle = DisplayedObject.FloatRandom(0, (float)Math.PI * 2) * DisplayedObject.SigneRandom();
            _pX = DisplayedObject.FloatRandom(-1, 1);
            _pY = DisplayedObject.FloatRandom(-1, 1);
        }
        public override void Deplace(SystemeParticules2D s, Temps maintenant, Color couleur)
        {
            _pX += (float)Math.Sin(_angle) * _vitesse * maintenant.intervalleDepuisDerniereFrame ;
            _pY += (float)Math.Cos(_angle) * _vitesse * maintenant.intervalleDepuisDerniereFrame;
            _angle += _vitesseAngle * maintenant.intervalleDepuisDerniereFrame;
            DisplayedObject.Varie(ref _vitesseAngle, -4, 4, 25, maintenant.intervalleDepuisDerniereFrame);

            bool rebond = false;
            if (_pX < SystemeParticules2D.MIN_X)
            {
                _pX = SystemeParticules2D.MIN_X;
                rebond = true;
            }
            if (_pX > SystemeParticules2D.MAX_X)
            {
                _pX = SystemeParticules2D.MAX_X;
                rebond = true;
            }
            if (_pY < SystemeParticules2D.MIN_Y)
            {
                _pY = SystemeParticules2D.MIN_Y;
                rebond = true;
            }
            if (_pY > SystemeParticules2D.MAX_Y)
            {
                _pY = SystemeParticules2D.MAX_Y;
                rebond = true;
            }

            if (rebond)
                _angle += (float)Math.PI;

            // Ajouter une particule ?            
            if( _timer.Ecoule())
            {
                int indice = s.FindUnusedParticle();
                s._particules[indice].x = _pX;
                s._particules[indice].y = _pY;
                s._particules[indice].alpha = 1;
                s._particules[indice].debutVie = maintenant.totalMilliSecondes;
                s._particules[indice].finVie = maintenant.totalMilliSecondes + 2000;
                s._particules[indice]._couleur[0] = couleur.R / 512.0f;
                s._particules[indice]._couleur[1] = couleur.G / 512.0f;
                s._particules[indice]._couleur[2] = couleur.B / 512.0f;
                s._particules[indice]._couleur[3] = 1;
                s._particules[indice]._couleurIndividuelle = true;
                float vitesse = _vitesseParticule * DisplayedObject.FloatRandom(0.95f, 1.05f);
                s._particules[indice].vx = -(float)Math.Sin(_angle) * vitesse;
                s._particules[indice].vy = -(float)Math.Cos(_angle) * vitesse;
                s._particules[indice].taille = _taille;
                s._particules[indice].textureIndex = r.Next(0, s._listeTextures.Count);
                s._particules[indice].active = true;
            }

            
        }
    }



}
