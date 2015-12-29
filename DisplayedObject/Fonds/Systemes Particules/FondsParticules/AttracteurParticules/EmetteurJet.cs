using ClockScreenSaverGL.DisplayedObjects.Fonds.Systeme_Particules;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class EmetteurJet: Emetteur
    {
        readonly int NB_PARTICULES =1000;
        
        static RectangleF bounds = new RectangleF(SystemeParticules.MIN_X, SystemeParticules.MIN_Y, SystemeParticules.LARGEUR, SystemeParticules.HAUTEUR);
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
            _timer = new TimerIsole(1);
            _traj = traj;
        }
        public override void Deplace(SystemeParticules s, Temps maintenant, Color couleur)
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
                    s._particules[indice].debutVie = maintenant._totalMillisecondes;
                    s._particules[indice].finVie = maintenant._totalMillisecondes + 300000;

                    s._particules[indice].vx = -_traj._Vx + 0.02f * DisplayedObject.FloatRandom(-1f, 1f);
                    s._particules[indice].vy = -_traj._Vy + 0.02f * DisplayedObject.FloatRandom(-1f, 1f);
                    s._particules[indice].taille = _taille * DisplayedObject.FloatRandom(0.5f, 1.5f);
                    s._particules[indice].textureIndex = r.Next(0, s._listeTextures.Count);
                    s._particules[indice].active = true;
                    s._particules[indice]._couleur[0] = couleur.R / 512.0f;
                    s._particules[indice]._couleur[1] = couleur.G / 512.0f;
                    s._particules[indice]._couleur[2] = couleur.B / 512.0f;
                    s._particules[indice]._couleur[3] = 0.5f ;
                    s._particules[indice]._couleurIndividuelle = true;
                }
                s._trier = true;
            }
        }

        
    }



}
