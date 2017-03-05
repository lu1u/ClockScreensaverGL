using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D;
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class EmetteurGalaxie : Emetteur2D
    {
        static RectangleF bounds = new RectangleF(SystemeParticules2D.SystemeParticules2D.MIN_X, SystemeParticules2D.SystemeParticules2D.MIN_Y, SystemeParticules2D.SystemeParticules2D.LARGEUR, SystemeParticules2D.SystemeParticules2D.HAUTEUR);
        static SizeF tailleEmetteur = new SizeF(0.1f, 0.1f);
        TrajectoireDiagonale traj;
        float angle = 0;
        float _taille;
        float _vitesseAngle;
        float _vitesseParticule;
        TimerIsole _timer;
        int _nbBras;

        public EmetteurGalaxie(float taille, float vitesseAngle, float vitesseParticule, int nbBras )
        {
            traj = new TrajectoireDiagonale(
                DisplayedObject.FloatRandom(SystemeParticules2D.SystemeParticules2D.MIN_X, SystemeParticules2D.SystemeParticules2D.MAX_X) * 0.9f,
                DisplayedObject.FloatRandom(SystemeParticules2D.SystemeParticules2D.MIN_Y, SystemeParticules2D.SystemeParticules2D.MAX_Y) * 0.9f,
                DisplayedObject.FloatRandom(-0.1f, 0.1f), DisplayedObject.FloatRandom(-0.1f, 0.1f));
            _vitesseAngle = vitesseAngle * DisplayedObject.FloatRandom(0.5f, 2.0f) * DisplayedObject.SigneRandom();
            _taille = taille * DisplayedObject.FloatRandom(0.5f, 1.5f);
            _vitesseParticule = vitesseParticule;
            _timer = new TimerIsole(1);
            _nbBras = nbBras;
        }
        public override void Deplace(SystemeParticules2D.SystemeParticules2D s, Temps maintenant, Color couleur)
        {
            traj.Avance(bounds, tailleEmetteur, maintenant);
            angle += _vitesseAngle * maintenant.intervalleDepuisDerniereFrame;
            // Ajouter une particule ?            
            if (_timer.Ecoule())
                for ( int i = 0; i < _nbBras;i++)
                    Ajoute(s, angle + ((double)i*(Math.PI*2.0)/(double)_nbBras), maintenant.totalMilliSecondes, couleur);
        }

        private void Ajoute(SystemeParticules2D.SystemeParticules2D s, double angleBras, double maintenant, Color couleur)
        {
            float vitesse = _vitesseParticule * DisplayedObject.FloatRandom(0.9f, 1.1f);
            int indice = s.FindUnusedParticle();
            s._particules[indice].x = traj._Px;
            s._particules[indice].y = traj._Py;
            s._particules[indice].alpha = 0.5f;
            s._particules[indice].debutVie = maintenant;
            s._particules[indice].finVie = maintenant + 8000;

            s._particules[indice].vx = traj._Vx + (float)Math.Sin(angleBras) * vitesse;
            s._particules[indice].vy = traj._Vy + (float)Math.Cos(angleBras) * vitesse;
            s._particules[indice].taille = _taille * DisplayedObject.FloatRandom(0.9f, 1.1f);
            s._particules[indice].active = true;
            s._particules[indice].textureIndex = r.Next(0, s._listeTextures.Count);

            s._particules[indice]._couleur[0] = couleur.R / 512.0f;
            s._particules[indice]._couleur[1] = couleur.G / 512.0f;
            s._particules[indice]._couleur[2] = couleur.B / 512.0f;
            s._particules[indice]._couleur[3] = 0.5f;
            s._particules[indice]._couleurIndividuelle = true;
            s.Trier = true;
        }
    }



}
