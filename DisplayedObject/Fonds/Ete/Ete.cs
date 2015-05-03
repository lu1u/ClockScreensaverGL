using System;
using System.Drawing;
using System.Collections.Generic;
namespace ClockScreenSaverGL.Fonds.Ete
{
    class Ete : Fond
    {
        #region PARAMETRES
        const string CAT = "Ete";
        static private readonly int NB_HERBES = 4;//conf.getParametre(CAT, "Nb Herbes", 2000);
        #endregion
        private readonly int LARGEUR;
        private readonly int HAUTEUR;
        private float _vent = 0;
        private Bitmap _fond = Resources.ete;
        List<Herbe> _herbes;
        /**
         * Constructeur
         */ 
        public Ete(int LargeurEcran, int HauteurEcran)
        {
            LARGEUR = LargeurEcran;
            HAUTEUR = HauteurEcran;
            GenereBrinsHerbe();
        }

        private void GenereBrinsHerbe()
        {
            _herbes = new List<Herbe>();
            for (int i = 0; i < NB_HERBES; i++)
                _herbes.Add(new Herbe( r.Next(0, LARGEUR), HAUTEUR, r.Next(100,200)));

        }

        /**
         * Affichage
         */
        public override void AfficheGDI(Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            g.Clear(getCouleurOpaqueAvecAlpha(couleur, 64));
            g.DrawImage(_fond, tailleEcran);
            foreach (Herbe h in _herbes)
                h.Affiche(g, _vent);
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }


        /**
         * Deplacement
         */
        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            Varie(ref _vent, -2.5f, 2.5f, 0.005f, maintenant._intervalle);
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }
    }
}
