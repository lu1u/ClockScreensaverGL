using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace ClockScreenSaverGL.Fonds.Ete
{
    class Ete : Fond
    {
        #region PARAMETRES
        const string CAT = "Ete";
        static private readonly int NB_HERBES = conf.getParametre(CAT, "Nb Herbes", 80 );
        private readonly int NB_FLARES = conf.getParametre(CAT, "Nb Flares", 6 ) ;
        private readonly float VX_SOLEIL = conf.getParametre(CAT, "VX Soleil", 5f) ;
        private readonly float VY_SOLEIL = -conf.getParametre(CAT, "VY Soleil", 5f) ;
        
        private readonly int TAILLE_SOLEIL = conf.getParametre(CAT, "Taille Soleil", 300 );
        private readonly float RATIO_FLARE_MIN = conf.getParametre(CAT, "Ratio Flare Min", 0.5f );
        private readonly float RATIO_FLARE_MAX = conf.getParametre(CAT, "Ratio Flare Max", 0.9f);
        private readonly byte ALPHA_FLARE_MIN = (byte) conf.getParametre(CAT, "Alpha Flare Min", 3);
        private readonly byte ALPHA_FLARE_MAX = (byte)conf.getParametre(CAT, "Alpha Flare Max", 16);
        private readonly int HAUTEUR_TOUFFE = conf.getParametre(CAT, "Hauteur touffe", 200);
        private bool AFFICHE_FOND = conf.getParametre(CAT, "Affiche Fond", true);
        private readonly float DISTANCE_FLARE_MIN = conf.getParametre(CAT, "Distance Flare Min", 0.1f);
        private readonly float DISTANCE_FLARE_MAX = conf.getParametre(CAT, "Distance Flare Max", 1.7f);
        #endregion
        private readonly int LARGEUR_TOUFFE;
        private readonly int LARGEUR;
        private readonly int HAUTEUR;
        private readonly int CENTREX;
        private readonly int CENTREY;
        private float _vent = 0;
        //private Bitmap _fond = Resources.ete;

        private Bitmap _soleil = Resources.soleil;
        List<Herbe> _herbes;
        static DateTime debut = DateTime.Now;

        private float _xSoleil, _ySoleil;
        private Bitmap _fond = Resources.fondEte;

        private class Flare
        {
            public float _distance;
            public int _taille;
            public byte _alpha;
            public Bitmap _bmp;
        } ;

        private Flare[] _flares;

        /**
         * Constructeur
         */
        public Ete(int LargeurEcran, int HauteurEcran)
        {
            LARGEUR = LargeurEcran;
            HAUTEUR = HauteurEcran;
            LARGEUR_TOUFFE = LARGEUR ;
            CENTREX = LARGEUR / 2;
            CENTREY = HAUTEUR / 2;
            NB_FLARES = r.Next(NB_FLARES - 2, NB_FLARES + 2);
            Init();
        }


        void Init()
        {          
            _xSoleil = 0;
            _ySoleil = CENTREY;
           
            _flares = new Flare[NB_FLARES];

            for (int i = 0; i < NB_FLARES; i++)
            {

                _flares[i] = new Flare();

                _flares[i]._distance = FloatRandom(DISTANCE_FLARE_MIN, DISTANCE_FLARE_MAX);
                _flares[i]._taille = r.Next((int)(TAILLE_SOLEIL * RATIO_FLARE_MIN), (int)(TAILLE_SOLEIL * RATIO_FLARE_MAX));
                _flares[i]._alpha = (byte)r.Next(ALPHA_FLARE_MIN, ALPHA_FLARE_MAX);
                _flares[i]._bmp = RandomFlare();
            }


            GenereBrinsHerbe();
        }
        private Bitmap RandomFlare()
        {
            switch (r.Next(0, 4))
            {
                case 0: return Resources.flare2;
                case 1: return Resources.flare3;
                case 3: return Resources.flare4;
                default: return Resources.flare1;
            }
        }

        private void GenereBrinsHerbe()
        {
            _herbes = new List<Herbe>();

            int touffe = r.Next(0, LARGEUR - LARGEUR_TOUFFE);
            for (int i = 0; i < NB_HERBES; i++)
                _herbes.Add(new Herbe(r.Next(touffe, touffe + LARGEUR_TOUFFE), HAUTEUR, r.Next(HAUTEUR_TOUFFE / 2, HAUTEUR_TOUFFE*3/2), FloatRandom(0.7f, 1.1f)));

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

            if ( AFFICHE_FOND)
                g.DrawImage(_fond, 0, HAUTEUR - HAUTEUR_TOUFFE*3, LARGEUR, HAUTEUR_TOUFFE*3);
            g.DrawImage(_soleil, _xSoleil - TAILLE_SOLEIL / 2, _ySoleil - TAILLE_SOLEIL / 2, TAILLE_SOLEIL, TAILLE_SOLEIL);

            //..            using ( Brush b = new SolidBrush(getCouleurOpaqueAvecAlpha( couleur, 255 )))
            //g.FillEllipse( b, _xSoleil - TAILLE_SOLEIL/2, _ySoleil - TAILLE_SOLEIL/2, TAILLE_SOLEIL, TAILLE_SOLEIL ) ;
            foreach (Herbe h in _herbes)
                h.Affiche(g, _vent);

            float dx = CENTREX - _xSoleil;
            float dy = CENTREY - _ySoleil;

            foreach (Flare f in _flares)
            {
                DrawBitmapNuance(g, f._bmp, _xSoleil + dx * f._distance - f._taille / 2,
                                     _ySoleil + dy * f._distance - f._taille / 2,
                                     f._taille,
                                     f._taille, getCouleurAvecAlpha(couleur, f._alpha));
            }
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

        /// <summary>
        /// Pression sur une touche, retourner true si l'objet a traite, false = fin de l'economiseur
        /// </summary>
        /// <param name="f"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public override bool KeyDown(Form f, Keys k)
        {
            switch (k)
            {
                case Keys.R:
                    {
                        Init();
                        return true;
                    }

                case Keys.I:
                    AFFICHE_FOND = !AFFICHE_FOND;
                    conf.setParametre(CAT, "Affiche Fond", AFFICHE_FOND);
                    return true;
            }
            return false;
        }


        /**
         * Deplacement
         */
        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            Varie(ref _vent, -300f, 300f, 0.02f, maintenant._intervalle);
            //float depuisdebut = (float)(debut.Subtract(maintenant._temps).TotalMilliseconds / 1000.0);
            //_vent = (float)Math.Sin(depuisdebut / 5.0f) * 0.5f;

            _xSoleil += VX_SOLEIL * maintenant._intervalle;
            _ySoleil += VY_SOLEIL * maintenant._intervalle;

            if (_xSoleil > LARGEUR + TAILLE_SOLEIL || _ySoleil < -TAILLE_SOLEIL)
                Init();
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }
    }
}
