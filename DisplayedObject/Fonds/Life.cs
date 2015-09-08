/*
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

namespace ClockScreenSaverGL.Fonds
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
        #endregion

        private byte[,] cellules;
        private byte[,] cellulestemp;
        private static Bitmap bmp = Resources.particleTexture;

        private const byte MORT = 0;
        private const byte NORMAL = 1;
        private const byte NAISSANCE = 2;
        private int _colonneMin, _colonneMax, _largeurCalcul;
        Texture textureCellule = new Texture();

        public Life(OpenGL gl)
        {
            _largeurCalcul = LARGEUR / SKIP;
            _colonneMin = -_largeurCalcul;
            _colonneMax = _colonneMin + _largeurCalcul;
            cellules = new byte[LARGEUR, HAUTEUR];
            cellulestemp = new byte[LARGEUR, HAUTEUR];
            InitCellules();

            textureCellule.Create(gl, Resources.particleTexture);
        }


        /// <summary>
        /// Etat initial des cellules
        /// </summary>
        private void InitCellules()
        {
            Random r = new Random();
            for (int x = 0; x < LARGEUR; x++)
                for (int y = 0; y < HAUTEUR; y++)
                    cellules[x, y] = r.Next(10) > 4 ? MORT : NAISSANCE;
        }
        /*
        /// <summary>
        /// Affichage des cellules
        /// </summary>
        /// <param name="g"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleur"></param>
		public override void AfficheGDI( Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur )
		{
			#if TRACER
			RenderStart(CHRONO_TYPE.RENDER) ;
			#endif
			g.Clear(  Color.Black) ;//getCouleurOpaqueAvecAlpha( couleur, 25 )) ;
			float Rx = (float)tailleEcran.Width / LARGEUR ;
			float Ry = (float)tailleEcran.Height / HAUTEUR ;
			
			using ( Bitmap bpNormal = GetBitmap( g, couleur, COULEUR_NORMAL, Rx, Ry ),
			       bpNaissance  = GetBitmap( g, couleur, COULEUR_NAISSANCE, Rx, Ry ) )
			{
				for ( int x = 0; x < LARGEUR; x++)
				{
					int X = (int)(x * Rx) ;
					for (int y = 0; y < HAUTEUR; y++)
					{
						float Y = y * Ry ;
						
						switch ( cellules[x,y])
						{
								case NAISSANCE	:	g.DrawImageUnscaled( bpNaissance, X, (int)Y ) ; break ;
								case NORMAL		:	g.DrawImageUnscaled( bpNormal, X, (int)Y ) ; break ;
								default: break ;
						}
					}
				}
			}
			#if TRACER
			RenderStop(CHRONO_TYPE.RENDER) ;
			#endif
		}*/

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D(0.0, 1.0, 0.0, 1.0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            textureCellule.Bind(gl);

            float rx = 1.0f / LARGEUR;
            float ry = 1.0f / HAUTEUR;

            Color Naissance = getCouleurOpaqueAvecAlpha(couleur, 128);
            byte[] cNaissance = { Naissance.R, Naissance.G, Naissance.B };
            Color Normal = getCouleurOpaqueAvecAlpha(couleur, 200);
            byte[] cNormal = { Normal.R, Normal.G, Normal.B };

            for (int x = 0; x < LARGEUR; x++)
            {
                float X = x * rx;
                for (int y = 0; y < HAUTEUR; y++)
                {
                    float Y = y * ry;

                    if (cellules[x, y] != MORT)
                    {
                        if (cellules[x, y] == NAISSANCE)
                            gl.Color(cNaissance[0], cNaissance[1], cNaissance[2]);
                        else
                            gl.Color(cNormal[0], cNormal[1], cNormal[2]);

                        gl.Begin(OpenGL.GL_QUADS);
                        gl.TexCoord(0.0f, 0.0f); gl.Vertex(X, Y + ry);
                        gl.TexCoord(0.0f, 1.0f); gl.Vertex(X, Y);
                        gl.TexCoord(1.0f, 1.0f); gl.Vertex(X + rx, Y);
                        gl.TexCoord(1.0f, 0.0f); gl.Vertex(X + rx, Y + ry);
                        gl.End();
                    }
                }
            }

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();

#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }


        private static Bitmap GetBitmap(Graphics g, Color couleur, float col, float Rx, float Ry)
        {
            float ratio = 255.0f / col;
            Bitmap bp = new Bitmap((int)Math.Round(Rx), (int)Math.Round(Ry), g);
            using (Graphics gMem = Graphics.FromImage(bp))
            {
                float[][] ptsArray =
                {
                    new float[] {couleur.R/ratio, 0, 0, 0, 0},
                    new float[] {0, couleur.G/ratio, 0, 0, 0},
                    new float[] {0, 0, couleur.B/ratio, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                };

                ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
                ImageAttributes imgAttribs = new ImageAttributes();
                imgAttribs.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);

                int Bordure = 1;
                gMem.DrawImage(bmp,
                               new Rectangle(0, 0, (int)Math.Round(Rx), (int)Math.Round(Ry)),
                               Bordure, Bordure, bmp.Width - Bordure * 2, bmp.Height - Bordure * 2,
                               GraphicsUnit.Pixel, imgAttribs);
            }

            return bp;
        }

        /// <summary>
        /// Calcul des changements de cellules
        /// On ne calcule a chaque fois qu'une seule partie du tableau (voir parametre Skip)
        /// pour mieux repartir la charge entre les frames
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif

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
                            if (cellules[x, y] == MORT)
                                cellulestemp[x, y] = NAISSANCE;
                            else
                                cellulestemp[x, y] = NORMAL;
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
