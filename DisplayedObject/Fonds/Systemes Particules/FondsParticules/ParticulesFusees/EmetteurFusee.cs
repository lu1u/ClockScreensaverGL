﻿using ClockScreenSaverGL.DisplayedObjects.Fonds.Systeme_Particules;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class EmetteurFusee : Emetteur
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
        public override void Deplace(SystemeParticules s, Temps maintenant, Color couleur)
        {
            _pX += (float)Math.Sin(_angle) * _vitesse * maintenant._intervalle ;
            _pY += (float)Math.Cos(_angle) * _vitesse * maintenant._intervalle;
            _angle += _vitesseAngle * maintenant._intervalle;
            DisplayedObject.Varie(ref _vitesseAngle, -4, 4, 25, maintenant._intervalle);

            bool rebond = false;
            if ( _pX < SystemeParticules.MIN_X)
            {
                _pX = SystemeParticules.MIN_X;
                rebond = true;
            }
            if (_pX > SystemeParticules.MAX_X)
            {
                _pX = SystemeParticules.MAX_X;
                rebond = true;
            }
            if (_pY < SystemeParticules.MIN_Y)
            {
                _pY = SystemeParticules.MIN_Y;
                rebond = true;
            }
            if (_pY > SystemeParticules.MAX_Y)
            {
                _pY = SystemeParticules.MAX_Y;
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
                s._particules[indice].debutVie = maintenant._totalMillisecondes;
                s._particules[indice].finVie = maintenant._totalMillisecondes + 2000;

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
