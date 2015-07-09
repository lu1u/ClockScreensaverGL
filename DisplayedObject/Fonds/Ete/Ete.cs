using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
namespace ClockScreenSaverGL.Fonds.Ete
{
    class Ete : Fond
    {
        #region PARAMETRES
        const string CAT = "Ete";
        static private readonly int NB_HERBES = conf.getParametre(CAT, "Nb Herbes", 80);
        private readonly int NB_FLARES = conf.getParametre(CAT, "Nb Flares", 6);
        private readonly float VX_SOLEIL = conf.getParametre(CAT, "VX Soleil", 5f);
        private readonly float VY_SOLEIL = conf.getParametre(CAT, "VY Soleil", 5f);

        private readonly int TAILLE_SOLEIL = conf.getParametre(CAT, "Taille Soleil", 300);
        private readonly float RATIO_FLARE_MIN = conf.getParametre(CAT, "Ratio Flare Min", 0.5f);
        private readonly float RATIO_FLARE_MAX = conf.getParametre(CAT, "Ratio Flare Max", 0.9f);
        private readonly byte ALPHA = (byte)conf.getParametre(CAT, "Alpha", 64);
        private readonly byte ALPHA_FLARE_MIN = (byte)conf.getParametre(CAT, "Alpha Flare Min", 3);
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
        static DateTime debut = DateTime.Now;

        // Soleil
        private Bitmap _soleil = Resources.soleil;
        private float _xSoleil, _ySoleil;
        
        // Herbes
        private float _vent = 0;
        private Bitmap _fond = Resources.fondEte;
        private List<Herbe> _herbes;
                
        // Lens Flares (reflets du soleil sur l'objectif
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
            LARGEUR_TOUFFE = LARGEUR /2 ;
            CENTREX = LARGEUR / 2;
            CENTREY = HAUTEUR / 2;
            NB_FLARES = r.Next(NB_FLARES - 2, NB_FLARES + 2);
            Init();
        }

        /**
         * Initialisation du soleil, des lens flares et de l'herbe
         * */
        void Init()
        {
            _xSoleil = 0;
            _ySoleil = CENTREY/2;

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

        /***
         * Choisit un lens flare au hasard
         */
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

        /// <summary>
        /// Genere les brins d'herbe
        /// </summary>
        private void GenereBrinsHerbe()
        {
            _herbes = new List<Herbe>();

            int touffe = r.Next(0, LARGEUR - LARGEUR_TOUFFE);
            for (int i = 0; i < NB_HERBES; i++)
                _herbes.Add(new Herbe(  r.Next(touffe, touffe + LARGEUR_TOUFFE), 
                                        HAUTEUR, 
                                        r.Next(HAUTEUR_TOUFFE / 2, HAUTEUR_TOUFFE * 4 / 3), 
                                        FloatRandom(0.2f, 5.0f)));

        }

        /// <summary>
        /// Affichage
        /// </summary>
        /// <param name="g"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleur"></param>
        public override void AfficheGDI(Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            g.Clear(getCouleurOpaqueAvecAlpha(couleur, ALPHA));

            SmoothingMode s = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

            CompositingQuality q = g.CompositingQuality;
            g.CompositingQuality = CompositingQuality.HighSpeed;

            if (AFFICHE_FOND)
                g.DrawImage(_fond, 0, HAUTEUR - HAUTEUR_TOUFFE * 4, LARGEUR, HAUTEUR_TOUFFE * 4);

            g.DrawImage(_soleil, _xSoleil - TAILLE_SOLEIL / 2, _ySoleil - TAILLE_SOLEIL / 2, TAILLE_SOLEIL, TAILLE_SOLEIL);

            foreach (Herbe h in _herbes)
                h.Affiche(g, _vent);

            float dx = CENTREX - _xSoleil;
            float dy = CENTREY - _ySoleil;

            foreach (Flare f in _flares)
                DrawBitmapNuance(g, f._bmp, _xSoleil + dx * f._distance - f._taille / 2,
                                     _ySoleil + dy * f._distance - f._taille / 2,
                                     f._taille,
                                     f._taille, getCouleurAvecAlpha(couleur, f._alpha));
            g.SmoothingMode = s;
            g.CompositingQuality = q ;

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

        public override void AppendHelpText(StringBuilder s)
        {
            s.Append(Resources.AideEte);
        }
       
        /// <summary>
        /// Deplacement
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            Varie(ref _vent, -300f, 300f, 0.015f, maintenant._intervalle);
            
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
