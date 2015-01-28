/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 22/12/2014
 * Heure: 15:38
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Drawing;
namespace ClockScreenSaverGL.Fonds
{
	/// <summary>
	/// Description of Couleur.
	/// </summary>
	public class Couleur : Noir
	{
		const string CAT = "Couleur" ;
		static readonly byte FondCouleur = conf.getParametre( MainForm.CAT, "Valeur", (byte)100 ) ;
		public Couleur( int Cx, int Cy): base( Cx, Cy )
		{
		}
		public override void AfficheGDI( Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur )
		{
			#if TRACER
			RenderStart(CHRONO_TYPE.RENDER) ;
			#endif
			g.Clear(  getCouleurOpaqueAvecAlpha( couleur, FondCouleur )) ;
			foreach( DisplayedObject b in listeObjets)
				b.AfficheGDI( g, maintenant, tailleEcran, couleur) ;
			
			#if TRACER
			RenderStart(CHRONO_TYPE.RENDER) ;
			#endif
				
		}
		
	}
}
