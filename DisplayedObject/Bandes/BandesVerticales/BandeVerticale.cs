/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 22/06/2014
 * Time: 20:15
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using System;
using System.Drawing;

namespace ClockScreenSaverGL.Bandes.BandeVerticale
{
	/// <summary>
	/// Description of BandeVerticale.
	/// </summary>
	public abstract class BandeVerticale: Bande
	{
		public const string CAT = "BandeVerticale";
		public static int TailleFonte = conf.getParametre( CAT, "TailleFonte", 30 ) ;
		
		
		public BandeVerticale( int valMax, int intervalle, float largeurcase, float origineY, float Px, int largeur, byte alpha )
			:base( valMax, intervalle, largeurcase, TailleFonte, origineY, largeur, alpha )
		{
			_trajectoire = new TrajectoireDiagonale( Px, _origine, conf.getParametre( CAT, "VY", 20f ), 0 );
			_taillebande = new SizeF( _hauteurFonte*2, largeur) ;
		}
		
		
		

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
		

	}
}

