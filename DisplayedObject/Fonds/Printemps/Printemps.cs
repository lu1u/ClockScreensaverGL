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
        public static readonly int DELAI_RECOMMENCE = conf.getParametre(CAT, "Delai nouvel arbre", 10) * 1000;
        public static readonly float LARGEUR_TRONC = conf.getParametre(CAT, "Largeur Tronc", 10);
        public static readonly int LARGEUR_ARBRE = conf.getParametre(CAT, "Largeur Arbre", 1200);
        public static readonly int HAUTEUR_ARBRE = conf.getParametre(CAT, "Hauteur Arbre", 400);
        public static readonly int LONGUEUR_BRANCHE = conf.getParametre(CAT, "Longueur Branche", 5);
        public static readonly int DISTANCE_MIN= conf.getParametre(CAT, "Distance Min", 5);
        public static readonly int DISTANCE_MAX = conf.getParametre(CAT, "Distance Max", 100);
        float _oscillation = 0;
        #endregion
        DateTime finGrowing;

        Tree tree;
        public Printemps(int LargeurEcran, int HauteurEcran)
        {
            tree = new Tree(LargeurEcran, HauteurEcran * 0.3f, 0, LARGEUR_TRONC, LARGEUR_ARBRE, HAUTEUR_ARBRE, LONGUEUR_BRANCHE, DISTANCE_MIN, DISTANCE_MAX);
        }

        public override void AfficheGDI(Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            g.Clear(getCouleurOpaqueAvecAlpha(couleur, 64));
            tree.Draw(g);

#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }


        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            if (tree.DoneGrowing)
            {
                if (maintenant._temps.Subtract(finGrowing).TotalMilliseconds > DELAI_RECOMMENCE)
                    tree = new Tree(tailleEcran.Width, tailleEcran.Height *0.3f, 0, LARGEUR_TRONC, LARGEUR_ARBRE, HAUTEUR_ARBRE, LONGUEUR_BRANCHE, DISTANCE_MIN, DISTANCE_MAX);
            }
            else
                finGrowing = maintenant._temps;

            tree.Grow();

            _oscillation += maintenant._intervalle * 1.5f;
            tree.Oscillation((float)Math.Sin(_oscillation) * 0.02f);
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }
    }
}
