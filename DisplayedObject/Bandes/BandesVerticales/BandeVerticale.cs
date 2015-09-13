/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 22/06/2014
 * Time: 20:15
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ClockScreenSaverGL.Bandes.BandeVerticale
{
    /// <summary>
    /// Description of BandeVerticale.
    /// </summary>
    public abstract class BandeVerticale : Bande
    {
        public const string CAT = "BandeVerticale";
        public readonly static int TailleFonte = conf.getParametre(CAT, "TailleFonte", 30);


        public BandeVerticale(OpenGL gl, int valMax, int intervalle, float largeurcase, float origineY, float Px, int largeur, byte alpha)
            : base(gl, valMax, intervalle, largeurcase, TailleFonte, origineY, largeur, alpha)
        {
            _trajectoire = new TrajectoireDiagonale(Px, 0, conf.getParametre(CAT, "VY", 20f), 0);
            _taillebande = new SizeF(_hauteurFonte * 2, largeur);
        }


        /// <summary>
        /// Creer la texture pour une bande verticale
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="Min"></param>
        /// <param name="Max"></param>
        /// <param name="Pas"></param>
        protected override void CreerTexture(OpenGL gl, int Min, int Max, int Pas)
        {
           /* _largeurBande = _hauteurFonte * 2 + 4;
            _hauteurBande = (int)(_largeurCase * (Max - Min));

            Bitmap bmp = new Bitmap(_largeurBande, _hauteurBande, PixelFormat.Format32bppArgb);
            using (Font fonte = new Font(FontFamily.GenericSansSerif, _hauteurFonte, FontStyle.Regular))
            using (Pen pen = new Pen(Color.White, 4))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                int val = Min;
                float X = 0;
                for (float Y = 0; Y < _hauteurBande; Y += (int)_largeurCase)
                {
                    if (val % _intervalleTexte == 0)
                    {
                        g.DrawLine(pen, X, Y, X + _hauteurFonte, Y);
                        g.DrawString(val.ToString(), fonte, Brushes.White, X, Y);
                    }
                    else
                        g.DrawLine(pen, X, Y, X + _hauteurFonte / 2, Y);

                    Y += _largeurCase;
                    val = (val + 1);
                    while (val > _valeurMax)
                        val -= _valeurMax;
                }

                // Deux lignes verticales pour les bords de la bande
                g.DrawLine(pen, X, 0, X, _hauteurBande);
                g.DrawLine(pen, X + _hauteurFonte * 2, 0, X + _hauteurFonte * 2, _hauteurBande);

                // Repere pour la valeur
                g.DrawLine(pen, X, _origine - 4, X + _hauteurFonte * 2, (float)_origine - 4);
                g.DrawLine(pen, X, (float)_origine + 4, X + _hauteurFonte * 2, (float)_origine + 4);
            }
            _texture = new Texture();
            _texture.Create(gl, bmp);*/
        }
        /*
        /// <summary>
        /// Affichage d'une bande verticale 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleurGlobale"></param>
        public override void AfficheGDI( Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleurGlobale)
		{
			#if TRACER
			RenderStart(CHRONO_TYPE.RENDER) ;
			#endif
			
			float decalage, valeur ;
			getValue( maintenant, out valeur, out decalage) ;
			
			Color couleur = getCouleurAvecAlpha( couleurGlobale, _alpha ) ;
			
			using (Brush brush = new SolidBrush(couleur))
				using (Pen pen = new Pen( couleur, 4 ))
			{
				float Decalage = _origine - (decalage *  _largeurCase) ;
				float Y = (float)Decalage ;
				float X = (float)_trajectoire._Px ;
				
				int val = (int)valeur ;
				
				// Reculer jusqu'à la droite de l'écran
				while ( Y > 0 )
				{
					Y -= (float)_largeurCase ;
					val -- ;
				}
				
				// Revenir jusqu'a la gauche de l'ecran
				while (val<0)
					val += (int)_valeurMax ;
				
				// Trace les chiffres et marques
				while ( Y < tailleEcran.Height)
				{
					if ( val % _intervalleTexte == 0)
					{
						g.DrawLine( pen, X, Y, X + _hauteurFonte, Y ) ;
						g.DrawString( val.ToString() , _fonte, brush, X, Y ) ;
					}
					else
						g.DrawLine( pen, X, Y, X + _hauteurFonte/2, Y ) ;
					
					Y+= (float)_largeurCase ;
					val = (val+1) ;
					while (val > _valeurMax )
						val -= _valeurMax ;
				}
				
				// Deux lignes verticales pour les bords de la bande
				g.DrawLine( pen, X, 0, X, tailleEcran.Height ) ;
				g.DrawLine( pen, X+_hauteurFonte*2, 0, X+_hauteurFonte*2, tailleEcran.Height ) ;
				
				// Repere pour la valeur
				g.DrawLine( pen, X, _origine-4, X + _hauteurFonte*2, (float)_origine-4) ;
				g.DrawLine( pen, X, (float)_origine+4, X + _hauteurFonte*2, (float)_origine+4) ;
			}
			#if TRACER
			RenderStop(CHRONO_TYPE.RENDER) ;
			#endif
		}
		*/
        /*
        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            if (_texture == null)
                return;

            gl.Clear(OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D(0.0, tailleEcran.Width, 0.0, tailleEcran.Height);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);


            float[] col = { couleur.R / 255.0f, couleur.G / 255.0f, couleur.B / 255.0f, _alpha / 255.0f };
            gl.Color(col);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            _texture.Bind(gl);

            float decalage, valeur;
            getValue(maintenant, out valeur, out decalage);

            float y = _trajectoire._Py - ((decalage-valeur) * _largeurCase);
            
            // Reculer jusqu'à la droite de l'écran
            while (y > 0)
                y -= (float)_hauteurBande;

            gl.Translate(_trajectoire._Px, _trajectoire._Py - (decalage-_largeurCase) , 0);

            while (y < tailleEcran.Height)
            {
                gl.Begin(OpenGL.GL_QUADS);
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(0, y+_hauteurBande);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(0, y+0);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(_largeurBande, y+0);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(_largeurBande, y+_hauteurBande);
                gl.End();

                y += _hauteurBande;
            }
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();

#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif

        }
        */
    }
}

