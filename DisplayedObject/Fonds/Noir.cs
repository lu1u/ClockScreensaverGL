/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 22/12/2014
 * Heure: 15:36
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Drawing;
using System.Collections.Generic;

namespace ClockScreenSaverGL.Fonds
{
	/// <summary>
	/// Description of Noir.
	/// </summary>
	public class Noir: Fond
	{
		protected List<DisplayedObject> listeObjets = new List<DisplayedObject>();
		
		public Noir( int Cx, int Cy)
		{
			int CentreX = Cx/2 ;
			int CentreY = Cy/2 ;
			
			listeObjets.Add( new Bandes.BandeHorizontale.BandeSeconde( 50, CentreX, CentreY, Cx )) ;
			listeObjets.Add( new Bandes.BandeHorizontale.BandeMinute( 80, CentreX, CentreY+ Bandes.BandeHorizontale.BandeHorizontale.TailleFonte *2, Cx )) ;
			listeObjets.Add( new Bandes.BandeHorizontale.BandeHeure( 120, CentreX, CentreY+ Bandes.BandeHorizontale.BandeHorizontale.TailleFonte * 4 , Cx )) ;
			
			// Bandes verticales
			listeObjets.Add( new Bandes.BandeVerticale.BandeHeure( 120, CentreY, CentreX, Cx )) ;
			listeObjets.Add( new Bandes.BandeVerticale.BandeMinute( 80, CentreY, CentreX + Bandes.BandeVerticale.BandeVerticale.TailleFonte * 2 , Cx )) ;
			listeObjets.Add( new Bandes.BandeVerticale.BandeSeconde( 50, CentreY, CentreX+ Bandes.BandeVerticale.BandeVerticale.TailleFonte * 4, Cx )) ;
			
		}
		public override void AfficheGDI( Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur )
		{
			#if TRACER
			RenderStart(CHRONO_TYPE.RENDER) ;
			#endif
			g.Clear(  Color.Black) ;
			foreach( DisplayedObject b in listeObjets)
				b.AfficheGDI( g, maintenant, tailleEcran, couleur) ;
			
			#if TRACER
			RenderStart(CHRONO_TYPE.RENDER) ;
			#endif
			
				
		}
        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
		{
			#if TRACER
			RenderStart(CHRONO_TYPE.DEPLACE) ;
			#endif

			foreach( DisplayedObject b in listeObjets)
				b.Deplace( maintenant, ref tailleEcran) ;
			
			#if TRACER
			RenderStop(CHRONO_TYPE.DEPLACE) ;
			#endif		
		}
		
	}
}
