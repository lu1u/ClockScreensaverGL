using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Saisons.Ete
{
    class Ete : Fond
    {
        #region PARAMETRES
        const string CAT = "Ete";
        static private readonly string REPERTOIRE_ETE = conf.getParametre(CAT, "Repertoire", "ete");
        static private readonly int NB_HERBES = conf.getParametre(CAT, "Nb Herbes", 80);
        static private readonly int NB_FLARES = conf.getParametre(CAT, "Nb Flares", 6);
        static private readonly float VX_SOLEIL = conf.getParametre(CAT, "VX Soleil", 5f);
        static private readonly float VY_SOLEIL = conf.getParametre(CAT, "VY Soleil", -5f);
        static private readonly int TAILLE_SOLEIL = conf.getParametre(CAT, "Taille Soleil", 300);
        static private readonly float RATIO_FLARE_MIN = conf.getParametre(CAT, "Ratio Flare Min", 0.7f);
        static private readonly float RATIO_FLARE_MAX = conf.getParametre(CAT, "Ratio Flare Max", 1.3f);
        static private readonly byte ALPHA = (byte)conf.getParametre(CAT, "Alpha", 64);
        static private readonly byte ALPHA_FLARE_MIN = (byte)conf.getParametre(CAT, "Alpha Flare Min", 3);
        static private readonly byte ALPHA_FLARE_MAX = (byte)conf.getParametre(CAT, "Alpha Flare Max", 16);
        static private readonly int HAUTEUR_TOUFFE = conf.getParametre(CAT, "Hauteur touffe", 200);
        static private bool AFFICHE_FOND = conf.getParametre(CAT, "Affiche Fond", true);
        static private readonly float DISTANCE_FLARE_MIN = conf.getParametre(CAT, "Distance Flare Min", 0.1f);
        static private readonly float DISTANCE_FLARE_MAX = conf.getParametre(CAT, "Distance Flare Max", 1.7f);

        const int NB_IMAGES_FLARES = 4;
        const float DECALAGE_TEXTURE = 1.0f / NB_IMAGES_FLARES;
        #endregion
        private readonly int LARGEUR_TOUFFE;
        private readonly int LARGEUR;
        private readonly int HAUTEUR;
        private readonly int CENTREX;
        private readonly int CENTREY;
        static DateTime debut = DateTime.Now;
        // Soleil
        private Texture _textureSoleil;
        private Texture _textureFond;
        private Texture _textureFlares;

        private float _xSoleil, _ySoleil;
        // Herbes
        private float _vent = 0;
        private List<Herbe> _herbes;
        
        // Lens Flares (reflets du soleil sur l'objectif
        private class Flare
        {
            public float _distance;
            public int _taille;
            public byte _alpha;
            public int _texture;
            public float rR, rG, rB;
        };
        private Flare[] _flares;
        /**
         * Constructeur
         */
        public Ete(OpenGL gl, int LargeurEcran, int HauteurEcran) : base(gl)
        {
            LARGEUR = LargeurEcran;
            HAUTEUR = HauteurEcran;
            LARGEUR_TOUFFE = LARGEUR / 2;
            CENTREX = LARGEUR / 2;
            CENTREY = HAUTEUR / 2;
            _textureSoleil = new Texture();
            _textureSoleil.Create(gl, Config.getImagePath(REPERTOIRE_ETE + @"\soleil.png"));
            _textureFond = new Texture();
            _textureFond.Create(gl, Config.getImagePath(REPERTOIRE_ETE + @"\fondEte.png"));
            _textureFlares = new Texture();
            _textureFlares.Create(gl, Config.getImagePath(REPERTOIRE_ETE + @"\flares.png"));
            Init(gl);
        }
        /**
         * Initialisation du soleil, des lens flares et de l'herbe
         * */
        void Init(OpenGL gl)
        {
            _xSoleil = 0;
            _ySoleil = CENTREY / 2;
            _flares = new Flare[NB_FLARES];
            for (int i = 0; i < NB_FLARES; i++)
            {
                _flares[i] = new Flare();
                _flares[i]._distance = FloatRandom(DISTANCE_FLARE_MIN, DISTANCE_FLARE_MAX);
                _flares[i]._taille = r.Next((int)(TAILLE_SOLEIL * RATIO_FLARE_MIN), (int)(TAILLE_SOLEIL * RATIO_FLARE_MAX));
                _flares[i]._alpha = (byte)r.Next(ALPHA_FLARE_MIN, ALPHA_FLARE_MAX);
                _flares[i]._texture = r.Next(0, NB_IMAGES_FLARES);
                _flares[i].rR = FloatRandom(RATIO_FLARE_MIN, RATIO_FLARE_MAX);
                _flares[i].rG = FloatRandom(RATIO_FLARE_MIN, RATIO_FLARE_MAX);
                _flares[i].rB = FloatRandom(RATIO_FLARE_MIN, RATIO_FLARE_MAX);
            }
            GenereBrinsHerbe();
        }

        /// <summary>
        /// Genere les brins d'herbe
        /// </summary>
        private void GenereBrinsHerbe()
        {
            _herbes = new List<Herbe>();
            int touffe = r.Next(0, LARGEUR - LARGEUR_TOUFFE);
            for (int i = 0; i < NB_HERBES; i++)
                _herbes.Add(new Herbe(r.Next(touffe, touffe + LARGEUR_TOUFFE),
                                        HAUTEUR,
                                        r.Next(HAUTEUR_TOUFFE / 2, HAUTEUR_TOUFFE * 4 / 3),
                                        FloatRandom(0.2f, 5.0f)));
        }

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D(0.0, tailleEcran.Width, tailleEcran.Height, 0.0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Disable(OpenGL.GL_BLEND);
            couleur = getCouleurOpaqueAvecAlpha(couleur, ALPHA);
            float[] col = { couleur.R / 255.0f, couleur.G / 255.0f, couleur.B / 255.0f, 1 };
            gl.Color(col);
            // Affichage du fond
            if (AFFICHE_FOND)
            {
                _textureFond.Bind(gl);
                gl.Begin(OpenGL.GL_QUADS);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(tailleEcran.Left, tailleEcran.Bottom);
                gl.TexCoord(0.0f, 0.01f); gl.Vertex(tailleEcran.Left, tailleEcran.Bottom - (HAUTEUR_TOUFFE * 4));
                gl.TexCoord(1.0f, 0.01f); gl.Vertex(tailleEcran.Right, tailleEcran.Bottom - (HAUTEUR_TOUFFE * 4));
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(tailleEcran.Right, tailleEcran.Bottom);
                gl.End();
            }

            gl.Color(1.0f, 1.0f, 1.0f, 1.0f);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE);

            _textureSoleil.Bind(gl);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(_xSoleil - TAILLE_SOLEIL / 2, _ySoleil - TAILLE_SOLEIL / 2);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(_xSoleil - TAILLE_SOLEIL / 2, _ySoleil + TAILLE_SOLEIL / 2);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(_xSoleil + TAILLE_SOLEIL / 2, _ySoleil + TAILLE_SOLEIL / 2);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(_xSoleil + TAILLE_SOLEIL / 2, _ySoleil - TAILLE_SOLEIL / 2);
            gl.End();
            float dx = CENTREX - _xSoleil;
            float dy = CENTREY - _ySoleil;


            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.Color(0, 0, 0, 0.5);
            foreach (Herbe h in _herbes)
                h.Affiche(gl, _vent);

            //gl.Color(col);
            gl.Enable(OpenGL.GL_BLEND);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE);
            _textureFlares.Bind(gl);
            gl.Begin(OpenGL.GL_QUADS);
            foreach (Flare f in _flares)
            {
                gl.Color(col[0] * f.rR, col[1] * f.rG, col[2] * f.rB, f._alpha);
                gl.TexCoord(DECALAGE_TEXTURE * f._texture, 1.0f); gl.Vertex(_xSoleil + dx * f._distance - f._taille / 2, _ySoleil + dy * f._distance - f._taille / 2);
                gl.TexCoord(DECALAGE_TEXTURE * f._texture, 0.0f); gl.Vertex(_xSoleil + dx * f._distance - f._taille / 2, _ySoleil + dy * f._distance + f._taille / 2);
                gl.TexCoord(DECALAGE_TEXTURE * (f._texture + 1), 0.0f); gl.Vertex(_xSoleil + dx * f._distance + f._taille / 2, _ySoleil + dy * f._distance + f._taille / 2);
                gl.TexCoord(DECALAGE_TEXTURE * (f._texture + 1), 1.0f); gl.Vertex(_xSoleil + dx * f._distance + f._taille / 2, _ySoleil + dy * f._distance - f._taille / 2);

            }
            gl.End();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

        public override void ClearBackGround(OpenGL gl, Color c)
        {
            c = getCouleurOpaqueAvecAlpha(c, ALPHA);

            gl.ClearColor(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT);
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
                /*case Keys.R:
                    {
                        Init();
                        return true;
                    }
                    */
                case TOUCHE_INVERSER:
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
        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            Varie(ref _vent, -100f, 300f, 0.4f, maintenant._intervalle); //_vent = (float)Math.Cos((double)maintenant._temps.Ticks * 0.00000005) * 0.5f + 0.4f;
            _xSoleil += VX_SOLEIL * maintenant._intervalle;
            _ySoleil += VY_SOLEIL * maintenant._intervalle;
            /*if (_xSoleil > LARGEUR + TAILLE_SOLEIL || _ySoleil < -TAILLE_SOLEIL)
                Init();*/
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }

#if TRACER
        public override string DumpRender()
        {
            return base.DumpRender() + _vent.ToString("Vent:  000.000");
        }
#endif
    }
}


