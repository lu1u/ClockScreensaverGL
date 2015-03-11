/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 22/06/2014
 * Time: 16:54
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Diagnostics ;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Text;
using SharpGL ;

namespace ClockScreenSaverGL
{
	/// <summary>
	/// Classe de base pour tous les objets affiches
	/// </summary>
	public abstract class DisplayedObject
	{
        const float PRECISION_RANDOM = 10000.0f;
		static public Random r = new Random() ;
		static protected readonly Config conf = Config.getInstance() ;
		
		protected SizeF 	_taille = new SizeF( -1, -1) ;
		
		public virtual void AfficheGDI( Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur ) {}
		public virtual void AfficheOpenGL( OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur ) {}
		public virtual void AppendHelpText( StringBuilder s ) {}
		
		public abstract void Deplace( Temps maintenant, Rectangle tailleEcran ) ;
		public virtual void OpenGLInitialized( OpenGL gl ) {}
		
		// Cette fonction sera appelee quand un changement de date sera detecte
		public virtual void DateChangee(Graphics g, Temps maintenant ) {}
        private int _noFrame = 0;

		/// <summary>
		/// Retourne une couleur correspondant a la teinte donnee avec la transparence donnee
		/// </summary>
		/// <param name="color"></param>
		/// <param name="alpha"></param>
		/// <returns></returns>
		protected static Color getCouleurAvecAlpha( Color color, byte alpha )
		{
			return Color.FromArgb(alpha, color.R, color.G, color.B ) ;
		}
		
        /***
         * Pour les operations qu'on ne veut pas faire à toutes les frames
         */
        protected bool UneFrameSur( int NbFrames)
        {
            _noFrame++;
            return (_noFrame % NbFrames == 0);
        }
		protected static Color getCouleurOpaqueAvecAlpha( Color color, byte alpha )
		{
			float a = (float)alpha / 255.0f ;
			
			return Color.FromArgb(255, (byte)((float)color.R * a), (byte)((float)color.G * a),(byte)((float) color.B * a) ) ;
		}
		
		/// <summary>
		/// Pression sur une touche, retourner true si l'objet a traite, false = fin de l'economiseur
		/// </summary>
		/// <param name="f"></param>
		/// <param name="k"></param>
		/// <returns>true si touche utilisee</returns>
		public virtual bool KeyDown( Form f, Keys k )
		{
			return false ;
		}
		
		
		/// <summary>
		/// Fait varier aleatoire une valeur donnee
		/// </summary>
		/// <param name="v">Valeur a faire changer</param>
		/// <param name="min">Minimum</param>
		/// <param name="max">Maximum</param>
		/// <param name="vitesse">Vitesse</param>
		/// <param name="intervalle">Intervalle depuis la derniere frame</param>
		protected void Varie( ref float v, float min, float max, float vitesse, float intervalle )
		{
			float dev = FloatRandom( -vitesse, vitesse ) * intervalle ;
			
			if (((v+dev) >= min) && ((v+dev) <= max))
				v += dev ;
		}
		/// <summary>
		/// Retourne une valeur float entre deux bornes
		/// </summary>
		/// <param name="r"></param>
		/// <param name="Min"></param>
		/// <param name="Max"></param>
		/// <returns></returns>
		static protected float FloatRandom(float Min, float Max )
		{
            if (Min < Max)
                return (float)r.Next((int)(Min * PRECISION_RANDOM), (int)(Max * PRECISION_RANDOM)) / PRECISION_RANDOM;
            else
                if (Min > Max)
                    return (float)r.Next((int)(Max * PRECISION_RANDOM), (int)(Min * PRECISION_RANDOM)) / PRECISION_RANDOM;
                else
                    return Min;
		}
		
		static protected Bitmap BitmapNuance( Graphics g,  Bitmap bmp, Color couleur )
		{
			Bitmap bp = new Bitmap( bmp.Width, bmp.Height, g ) ;
			using ( Graphics gMem = Graphics.FromImage( bp ))
			{
				float[][] ptsArray =
				{
					new float[] {couleur.R/255.0f, 0, 0, 0, 0},
					new float[] {0, couleur.G/255.0f, 0, 0, 0},
					new float[] {0, 0, couleur.B/255.0f, 0, 0},
					new float[] {0, 0, 0, couleur.A/255.0f, 0},
					new float[] {0, 0, 0, 0, 1}
				};
				
				ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
				ImageAttributes imgAttribs = new ImageAttributes();
				imgAttribs.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
				
				gMem.DrawImage( bmp,
				               new Rectangle(0, 0, bmp.Width, bmp.Height),
				               0, 0, bmp.Width, bmp.Height,
				               GraphicsUnit.Pixel, imgAttribs) ;
			}
			
			return bp ;
		}
		#region Chrono
		#if TRACER
		long	moyennedureeR = 0 ;
		long	moyennedureeD = 0 ;
		protected enum CHRONO_TYPE { RENDER, DEPLACE } ;
		
		private Stopwatch chronoRender = new Stopwatch() ;
		private Stopwatch chronoDeplace = new Stopwatch() ;
		
		/// <summary>
		/// Demarrage du trace
		/// </summary>
		protected void RenderStart(CHRONO_TYPE t)
		{
			switch( t )
			{
					case CHRONO_TYPE.RENDER :chronoRender.Restart() ; break ;
					case CHRONO_TYPE.DEPLACE :chronoDeplace.Restart() ; break ;
			}
			
		}
		
		/// <summary>
		/// Arret du trace
		/// </summary>
		protected void RenderStop(CHRONO_TYPE t)
		{
			switch( t )
			{
					case CHRONO_TYPE.RENDER : chronoRender.Stop() ; break ;
					case CHRONO_TYPE.DEPLACE :chronoDeplace.Stop() ; break ;
			}
			
		}
		
		public virtual String DumpRender(  )
		{
			moyennedureeR = ((moyennedureeR*10) + chronoRender.ElapsedTicks)/11 ;
			moyennedureeD = ((moyennedureeD*10) + chronoDeplace.ElapsedTicks)/11 ;
			return ( (moyennedureeR/1000.0).ToString("Render:  000.0") + " " +
			        (moyennedureeD/1000.0).ToString("Deplace:  000.0") + " " + this.GetType().Name ) ;
		}

		#endif
		#endregion
	}
}
