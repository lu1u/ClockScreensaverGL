/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 26/06/2014
 * Heure: 09:50
 * Classe de base pour les objets graphiques de type texte
 */
using System;
using System.Drawing ;

namespace ClockScreenSaverGL.Textes
{
	public abstract class Texte: DisplayedObject
	{
		protected Trajectoire		_trajectoire ;
		protected Font				_fonte ;
		protected byte				_alpha ;
		
		/// <summary>
		/// Constructeur
		/// Initialise la trajectoire, la fonte et le niveau de transparence
		/// </summary>
		/// <param name="Px"></param>
		/// <param name="Py"></param>
		/// <param name="Vx"></param>
		/// <param name="Vy"></param>
		/// <param name="tailleFonte"></param>
		/// <param name="alpha"></param>
		public Texte( float Px, float Py, float Vx, float Vy, int tailleFonte, byte alpha )
		{
			_trajectoire = new TrajectoireDiagonale( Px, Py, Vx, Vy ) ;
			_fonte = CreerFonte( tailleFonte ) ;
			_alpha = alpha ;
		}
		
		~Texte()
		{
			_fonte.Dispose() ;
		}
		
		/// <summary>
		/// Deplace l'objet, en tenant compte de la derniere taille calculee de cet objet
		/// </summary>
		/// <param name="maintenant"></param>
		/// <param name="tailleEcran"></param>
		public override void Deplace( Temps maintenant, Rectangle tailleEcran )
		{
			#if TRACER
			RenderStart(CHRONO_TYPE.DEPLACE) ;
			#endif
			
			_trajectoire.Avance( tailleEcran, _taille, maintenant ) ;
			
			#if TRACER
			RenderStop(CHRONO_TYPE.DEPLACE) ;
			#endif
			
		}
		
		/// <summary>
		/// A implementer: retourner le texte
		/// </summary>
		/// <param name="maintenant"></param>
		/// <returns></returns>
		protected abstract String	getTexte(Temps maintenant) ;
		protected abstract SizeF	getTailleTexte(Graphics g) ;
		
		protected virtual Font CreerFonte( int tailleFonte )
		{
			return new Font( FontFamily.GenericSansSerif, tailleFonte, FontStyle.Bold, GraphicsUnit.Pixel ) ; 
		}
		
		/// <summary>
		/// Affiche cet objet
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
			if ( _taille.Width == -1)
				_taille = getTailleTexte(g) ;
			
			string texte = getTexte(maintenant) ;
			
			using (Brush brush = new SolidBrush( getCouleurAvecAlpha(couleur, _alpha) ))
				g.DrawString( texte, _fonte, brush, _trajectoire._Px, _trajectoire._Py ) ;
			
			#if TRACER
			RenderStop(CHRONO_TYPE.RENDER) ;
			#endif
		}

	}
}
