/***
 * Printemps: un arbre qui pousse
 * Inspire de http://www.jgallant.com/blog/
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace ClockScreenSaverGL.Fonds.Printemps
{
    class Printemps : Fond
    {
        #region PARAMETRES
        const String CAT = "Tree.GDI" ;
        public static readonly int DELAI_RECOMMENCE = 20000;//conf.getParametre(CAT, "Delai nouvel arbre", 10) * 1000;
        public static readonly float LARGEUR_TRONC = conf.getParametre(CAT, "Largeur Tronc", 10);
        public static readonly int HAUTEUR_TRONC = 200;//conf.getParametre(CAT, "Hauteur Tronc", 200);
        public static readonly int LARGEUR_ARBRE = 1300;//conf.getParametre(CAT, "Largeur Arbre", 1200);
        public static readonly int HAUTEUR_ARBRE = 500;//conf.getParametre(CAT, "Hauteur Arbre", 400);
        public static readonly int LONGUEUR_BRANCHE = 7;//conf.getParametre(CAT, "Longueur Branche", 5);
        public static readonly int DISTANCE_MIN= conf.getParametre(CAT, "Distance Min", 5);
        public static readonly int DISTANCE_MAX = 300;//conf.getParametre(CAT, "Distance Max", 100);
        public static readonly int NB_CIBLES = 100;//conf.getParametre(CAT, "Nb Cibles", 1000);
        float _oscillation = 0;
        #endregion
        DateTime _finCroissance;
        Tree _tree;

        public Printemps(int LargeurEcran, int HauteurEcran)
        {
            _tree = new Tree(LargeurEcran, HauteurEcran * 0.3f, 0, LARGEUR_TRONC, LARGEUR_ARBRE, HAUTEUR_ARBRE, LONGUEUR_BRANCHE, DISTANCE_MIN, DISTANCE_MAX, NB_CIBLES, HAUTEUR_TRONC );
        }

        public override void AfficheGDI(Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            g.Clear(getCouleurOpaqueAvecAlpha(couleur, 64));
            _tree.Draw(g);

#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            if (_tree.DoneGrowing)
            {
                if (maintenant._temps.Subtract(_finCroissance).TotalMilliseconds > DELAI_RECOMMENCE)
                    _tree = new Tree(tailleEcran.Width, tailleEcran.Height * 0.3f, 0, LARGEUR_TRONC, LARGEUR_ARBRE, HAUTEUR_ARBRE, LONGUEUR_BRANCHE, DISTANCE_MIN, DISTANCE_MAX, NB_CIBLES, HAUTEUR_TRONC);
            }
            else
                _finCroissance = maintenant._temps;

            if ( UneFrameSur(2))
                _tree.Grow();

            _oscillation += maintenant._intervalle * 1.5f;
            _tree.Oscillation((float)Math.Sin(_oscillation) * 0.02f);
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }
    }
}
