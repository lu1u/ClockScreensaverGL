using System;
using System.Collections.Generic;
using System.Drawing;

namespace ClockScreenSaverGL.Fonds.Printemps
{
    class Feuille
    {
        static Random r = new Random();
        const int NB_TYPES_FEUILLES = 3;

        public int _typeFeuille { get; private set;  }
        public Vector2 Position { get; set; }
        public float _taille;
        public Feuille(Vector2 position)
        {
            Position = position;
            _taille = 20;
            _typeFeuille = r.Next(0, NB_TYPES_FEUILLES) ;
        }

        public void Grow(float intervalle)
        {
            if ( _taille < 70)
            _taille += intervalle * 1.2f;
        }

        public void Draw(Graphics g, float dx, float dy )
        {
            switch (_typeFeuille)
            {
                case 0:
                    g.DrawImage(Resources.feuille1, Position.X + dx - _taille, Position.Y + dy , _taille, _taille);
                    break;
                case 1:
                    g.DrawImage(Resources.feuille2, Position.X + dx - _taille, Position.Y + dy , _taille, _taille);
                    break;
                default:
                    g.DrawImage(Resources.feuille3, Position.X + dx - _taille, Position.Y + dy , _taille, _taille);
                    break;
            }

        }
    }
}
