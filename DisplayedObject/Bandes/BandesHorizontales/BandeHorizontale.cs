/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 22/06/2014
 * Time: 16:11
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using SharpGL;
using System;
using System.Drawing;

namespace ClockScreenSaverGL.Bandes.BandeHorizontale
{
	/// <summary>
	/// Description of Bande.
	/// </summary>
	public abstract class BandeHorizontale: Bande
	{
		public const string CAT = "BandeHorizontale";
		public readonly static int TailleFonte = conf.getParametre( CAT, "TailleFonte", 30 ) ;
		
		public BandeHorizontale( OpenGL gl, int valMax, int intervalle, float largeurcase, float origineX, float Py, int largeur, byte alpha )
			:base( gl, valMax, intervalle, largeurcase, TailleFonte, origineX, largeur, alpha )
			
		{
			_trajectoire = new TrajectoireDiagonale( _origine, Py, 0.0f, conf.getParametre( CAT, "VY", 20f ));
			_taillebande = new SizeF( largeur, _hauteurFonte*2) ;
		}
        protected override void CreerTexture(OpenGL gl, int Min, int Max, int Pas)
        {

        }
        /*
        public override void AfficheGDI( Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleurGlobale)
		{
			SizeF stringSize ;
			String texte;
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
				float X = Decalage ;
				float Y = _trajectoire._Py ;
				
				int val = (int)valeur ;
				
				g.DrawLine( pen, 0, Y, tailleEcran.Width, Y ) ;
				g.DrawLine( pen, 0, Y+_hauteurFonte*2, tailleEcran.Width, Y + _hauteurFonte*2 ) ;
				
				// Reculer jusqu'à la droite de l'écran
				int NbRecul = (int)(X / _largeurCase) + 1 ;
				X -= (NbRecul * _largeurCase) ;
				val -= NbRecul ;
				while (val<0)
					val += (int)_valeurMax ;
				
				// Tracer les graduations
				while ( X < (tailleEcran.Width-1))
				{
					if ( val % _intervalleTexte == 0)
					{
						texte = val.ToString() ;
						stringSize = g.MeasureString(texte, _fonte);
						g.DrawString( texte, _fonte, brush, X, Y + _hauteurFonte  ) ;
						g.DrawLine( pen, X, Y, X, Y + _hauteurFonte ) ;
					}
					else
						g.DrawLine( pen, X, Y, X, Y + _hauteurFonte/2 ) ;
					
					X+= _largeurCase ;
					val = (val+1) ;
					while (val > _valeurMax)
						val -= _valeurMax ;
				}
				
				// Repere de l'origine
				g.DrawLine( pen, _origine-4, Y-4, _origine-4, Y + _hauteurFonte*2 + 4) ;
				g.DrawLine( pen, _origine+4, Y-4, _origine+4, Y + _hauteurFonte*2 + 4) ;
				
				#if TRACER
				RenderStop(CHRONO_TYPE.RENDER) ;
				#endif
			}
		}
        */
	}
}
