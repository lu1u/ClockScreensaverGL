using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class EmetteurJet: Emetteur2D
    {
        readonly int NB_PARTICULES =1000;
        
        static RectangleF bounds = new RectangleF(SystemeParticules2D.SystemeParticules2D.MIN_X, SystemeParticules2D.SystemeParticules2D.MIN_Y, 
            SystemeParticules2D.SystemeParticules2D.LARGEUR, SystemeParticules2D.SystemeParticules2D.HAUTEUR);
        static SizeF tailleEmetteur = new SizeF(0.2f, 0.2f);
        float _taille;
        float _vitesseParticule;
        TimerIsole _timer;
        Trajectoire _traj;

        public EmetteurJet(float taille, float vitesseParticule, int nbParticules, Trajectoire traj)
        {
            NB_PARTICULES = nbParticules;
            _taille = taille;
            _vitesseParticule = vitesseParticule;
            _timer = new TimerIsole(10);
            _traj = traj;
        }
        public override void Deplace(SystemeParticules2D.SystemeParticules2D s, Temps maintenant, Color couleur)
        {
            _traj.Avance(bounds, tailleEmetteur, maintenant);

            // Ajouter une particule ?            
            if (_timer.Ecoule())
            {
                for (int i = 0; i < NB_PARTICULES; i++)
                {
                    int indice = s.FindUnusedParticle();
                    s._particules[indice].x = _traj._Px;
                    s._particules[indice].y = _traj._Py;
                    s._particules[indice].debutVie = maintenant.totalMilliSecondes;
                    s._particules[indice].finVie = maintenant.totalMilliSecondes + 300000;

                    s._particules[indice].vx = -_traj._Vx + _vitesseParticule * DisplayedObject.FloatRandom(-0.2f, 0.2f);
                    s._particules[indice].vy = -_traj._Vy + _vitesseParticule * DisplayedObject.FloatRandom(-0.2f, 0.2f);
                    s._particules[indice].taille = _taille * DisplayedObject.FloatRandom(0.5f, 1.2f);
                    s._particules[indice].textureIndex = r.Next(0, s._listeTextures.Count);
                    s._particules[indice].active = true;
                    s._particules[indice]._couleur[0] = couleur.R / 512.0f;
                    s._particules[indice]._couleur[1] = couleur.G / 512.0f;
                    s._particules[indice]._couleur[2] = couleur.B / 512.0f;
                    s._particules[indice]._couleur[3] = 1 ;
                    s._particules[indice]._couleurIndividuelle = true;
                }
                s._trier = true;
            }
        }

        
    }



}
