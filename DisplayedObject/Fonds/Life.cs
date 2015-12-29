﻿/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 22/12/2014
 * Heure: 14:32
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds
{
    /// <summary>
    /// Description of Life.
    /// </summary>
    public class Life : Fond
    {
        #region Parametres
        public const string CAT = "JeuDeLaVie";

        protected readonly byte ALPHA = conf.getParametre(CAT, "Alpha", (byte)40);
        private readonly float COULEUR_NAISSANCE = conf.getParametre(CAT, "CouleurNaissance", 0.3f);
        private readonly float COULEUR_NORMAL = conf.getParametre(CAT, "CouleurNormale", 0.4f);
        private readonly int LARGEUR = conf.getParametre(CAT, "Largeur", 60);
        private readonly int HAUTEUR = conf.getParametre(CAT, "Hauteur", 50);
        private readonly int SKIP = conf.getParametre(CAT, "Skip", 2);
        private readonly float VITESSE_ANGLE = conf.getParametre(CAT, "Vitesse Angle", 2.0f);
        private readonly float LOOK_AT_X = conf.getParametre(CAT, "LookAtX", 0.1f);
        private readonly float LOOK_AT_Y = 0.03f;// conf.getParametre(CAT, "LookAtY", 0.05f);
        private readonly float LOOK_AT_Z = conf.getParametre(CAT, "LookAtZ", -0.3f);
        #endregion

        private byte[,] cellules;
        private byte[,] cellulestemp;

        private const byte MORT = 0;
        private const byte NORMAL = 1;
        private const byte NAISSANCE = 2;
        private int _colonneMin, _colonneMax, _largeurCalcul;
        float _angle = 0;
        Texture textureCellule = new Texture();
        public Life(OpenGL gl): base(gl)
        {
            _largeurCalcul = LARGEUR / SKIP;
            _colonneMin = -_largeurCalcul;
            _colonneMax = _colonneMin + _largeurCalcul;
            cellules = new byte[LARGEUR, HAUTEUR];
            cellulestemp = new byte[LARGEUR, HAUTEUR];
            InitCellules();
            textureCellule.Create(gl, Resources.life);
        }


        /// <summary>
        /// Etat initial des cellules
        /// </summary>
        private void InitCellules()
        {
            Random r = new Random();
            for (int x = 0; x < LARGEUR; x++)
                for (int y = 0; y < HAUTEUR; y++)
                    cellules[x, y] = r.Next(11) > 5 ? MORT : NAISSANCE;
        }

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.ClearColor(0, 0, 0, 0);
            //gl.PushMatrix();
            //gl.MatrixMode(OpenGL.GL_PROJECTION);
            //gl.PushMatrix();
            // gl.LoadIdentity();
            //gl.Ortho2D(0.0, LARGEUR, 0.0, HAUTEUR);
            //gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.ClearColor(0, 0, 0, 1);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            Color Naissance = getCouleurOpaqueAvecAlpha(couleur, 70);
            byte[] cNaissance = { Naissance.R, Naissance.G, Naissance.B };
            Color Normal = getCouleurOpaqueAvecAlpha(couleur, 150);
            byte[] cNormal = { Normal.R, Normal.G, Normal.B };


            gl.LookAt(LOOK_AT_X, LOOK_AT_Y, LOOK_AT_Z, 0,-0.1f, 0, 0, -1, 0);
            gl.Scale(1.0f / LARGEUR, 1.0f / HAUTEUR, 1);
            gl.Rotate(0, 0, _angle);
            byte ancienType = MORT;
            
            //gl.Begin(OpenGL.GL_QUADS);
            //gl.Disable(OpenGL.GL_TEXTURE_2D);
            //gl.Color(cNormal[0], cNormal[1], cNormal[2]);
            //gl.Vertex(0, HAUTEUR);
            //gl.Vertex(0, 0);
            //gl.Vertex(LARGEUR, 0);
            //gl.Vertex(LARGEUR, HAUTEUR);
            //gl.Enable(OpenGL.GL_TEXTURE_2D);
            //gl.End();
             textureCellule.Bind(gl);
            gl.Translate(-LARGEUR / 2, -HAUTEUR / 2, 0);
            gl.Begin(OpenGL.GL_QUADS);

            for (int x = 0; x < LARGEUR; x++)
                for (int y = 0; y < HAUTEUR; y++)
                {
                    if (cellules[x, y] != MORT)
                    {
                        if (cellules[x, y] != ancienType)
                        {
                            if (cellules[x, y] == NAISSANCE)
                                gl.Color(cNaissance[0], cNaissance[1], cNaissance[2]);
                            else
                                gl.Color(cNormal[0], cNormal[1], cNormal[2]);
                            ancienType = cellules[x, y];
                        }

                        gl.TexCoord(0.0f, 0.0f); gl.Vertex(x, y + 1);
                        gl.TexCoord(0.0f, 1.0f); gl.Vertex(x, y);
                        gl.TexCoord(1.0f, 1.0f); gl.Vertex(x + 1, y);
                        gl.TexCoord(1.0f, 0.0f); gl.Vertex(x + 1, y + 1);
                    }
                   
                }
            gl.End();
            
            //gl.MatrixMode(OpenGL.GL_PROJECTION);
            // gl.PopMatrix();
            // gl.MatrixMode(OpenGL.GL_MODELVIEW);
            //gl.PopMatrix();
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

        /// <summary>
        /// Calcul des changements de cellules
        /// On ne calcule a chaque fois qu'une seule partie du tableau (voir parametre Skip)
        /// pour mieux repartir la charge entre les frames
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif

            _angle += maintenant._intervalle * VITESSE_ANGLE;
            int xMin, xMax;
            DecoupeEnBandes(out xMin, out xMax);

            int NbVoisines;
            int XM1, XP1, YM1, YP1;

            for (int x = xMin; x < xMax; x++)
            {
                XM1 = LimiteTore(x - 1, 0, LARGEUR);
                XP1 = LimiteTore(x + 1, 0, LARGEUR);

                for (int y = 0; y < HAUTEUR; y++)
                {
                    YM1 = LimiteTore(y - 1, 0, HAUTEUR);
                    YP1 = LimiteTore(y + 1, 0, HAUTEUR);

                    NbVoisines = GetNbVoisines(x, y, XM1, XP1, YM1, YP1);
                    switch (NbVoisines)
                    {
                        case 3:
                            cellulestemp[x, y] = (cellules[x, y] == MORT) ? NAISSANCE : NORMAL;
                            break;

                        case 2:
                            cellulestemp[x, y] = cellules[x, y] == MORT ? MORT : NORMAL;
                            break;

                        default:
                            cellulestemp[x, y] = MORT;
                            break;
                    }
                }
            }
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }

        /// <summary>
        /// Comme on ne calcule pas tout a chaque frame, on partage le calcule
        /// </summary>
        /// <param name="xMin"></param>
        /// <param name="xMax"></param>
        void DecoupeEnBandes(out int xMin, out int xMax)
        {
            if (_colonneMax < LARGEUR)
            {
                _colonneMin += _largeurCalcul;
                _colonneMax += _largeurCalcul;
                if (_colonneMax > LARGEUR)
                    _colonneMax = LARGEUR;
            }
            else
            {
                _colonneMin = 0;
                _colonneMax = _largeurCalcul;

                // On a tout calcule, echanger les tableaux
                byte[,] t = cellules;
                cellules = cellulestemp;
                cellulestemp = t;
            }

            xMin = _colonneMin;
            xMax = _colonneMax;
        }

        private int GetNbVoisines(int x, int y, int XM1, int XP1, int YM1, int YP1)
        {
            return Voisine(XM1, YM1) + Voisine(x, YM1) + Voisine(XP1, YM1)
                + Voisine(XM1, y) + Voisine(XP1, y)
                + Voisine(XM1, YP1) + Voisine(x, YP1) + Voisine(XP1, YP1);
        }


        static int LimiteTore(int val, int Min, int Max)
        {
            if (val < Min)
                return Max - 1;

            if (val >= Max)
                return Min;

            return val;
        }
        private int Voisine(int x, int y)
        {
            if (cellules[x, y] == MORT)
                return 0;
            else
                return 1;
        }

    }
}
