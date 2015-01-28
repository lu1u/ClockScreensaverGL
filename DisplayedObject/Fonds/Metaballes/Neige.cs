/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 17/12/2014
 * Heure: 21:19
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Drawing;
namespace ClockScreenSaverGL.Metaballes
{
	/// <summary>
	/// Description of Neige.
	/// </summary>
	public class Neige : Metaballes
	{
		const string CAT = "NeigeMeta" ;
		static float TailleMax, TailleMin, IntensiteMax,IntensiteMin ;
		static int NbMax;
		static DateTime derniereCreation = DateTime.Now ;
		
		public Neige( int cx, int cy): base( cx, cy)
		{
		}
		
		/// <summary>
		/// Lit les preferences a chaque version de metaballes
		/// </summary>
		/// <param name="L"></param>
		/// <param name="H"></param>
		/// <param name="N"></param>
		/// <param name="C"></param>
		protected override void GetPreferences( ref int L, ref int H, ref int N, ref int C )
		{
			base.GetPreferences( ref L, ref H, ref N, ref C ) ;
			L = conf.getParametre(CAT, "Largeur", 400 ) ;
			H = conf.getParametre(CAT, "Hauteur", 300 ) ;
			C = conf.getParametre(CAT, "Niveaux", 512 ) ;
		}
		
		protected override void ConstruitMetaballes()
		{
			TailleMax =conf.getParametre(CAT, "TailleMax", 40 ) ;
			TailleMin = conf.getParametre(CAT, "TailleMin", 30 ) ;
			IntensiteMax = conf.getParametre(CAT, "IntensiteMax", 1.0f ) ; ;
			IntensiteMin = conf.getParametre(CAT, "IntensiteMin", 0.5f) ;
			NbMax = conf.getParametre(CAT, "Nombre", 40 ) ;
			NbMetaballes = 0 ;
			_metaballes = new MetaBalle[NbMax] ;
			NouvelleMetaballe(ref _metaballes[0]) ;
			_metaballes[0]._Py = - _metaballes[0]._Taille/2 ;
			NbMetaballes++ ;
		}
		
		/// <summary>
		/// Creation d'une nouvelle metaballe: elle commencent plus petites et plus opaque et de diluent en montant
		/// </summary>
		/// <returns></returns>
		private void NouvelleMetaballe(ref MetaBalle balle )
		{
			float taille = FloatRandom( TailleMin,TailleMax) ;
			if ( balle == null )
				balle = new MetaBalle( FloatRandom( 0, IntensiteMax),	// Intensite
				                      taille,			// Taille
				                      FloatRandom(0,Largeur), -taille,
				                      0,FloatRandom(10,20) );
			else
				balle.Reset( FloatRandom( 0, IntensiteMax),	// Intensite
				            taille,			// Taille
				            FloatRandom(0,Largeur), -taille,
				            0, FloatRandom(10,20));
		}
		/// <summary>
		/// Changer l'image
		/// </summary>
		/// <param name="maintenant"></param>
		/// <param name="tailleEcran"></param>
		public override void Deplace( Temps maintenant, Rectangle tailleEcran )
		{
			#if TRACER
			RenderStart(CHRONO_TYPE.DEPLACE) ;
			#endif
			
			// Deplacement des metaballes
			for ( int i = 0 ;i < NbMetaballes ; i++)
			{
				
				if ((_metaballes[i]._Px < -_metaballes[i]._Taille )  ||	// Trop a gauche
				    (_metaballes[i]._Px  > Largeur) || // Trop a droite
				    (_metaballes[i]._Py -_metaballes[i]._Taille > Hauteur)
				   )
				{
					// Remettre le flocon en haut
					_metaballes[i].Reset(FloatRandom( IntensiteMin, IntensiteMax),	// Intensite
					                     FloatRandom( TailleMin,TailleMax),			// Taille
					                     FloatRandom(0,Largeur), -TailleMax, 0, FloatRandom(10,20));
				}
				else
				{
                    _metaballes[i]._Px += (_metaballes[i]._Vx * maintenant._intervalle);
                    _metaballes[i]._Py += (_metaballes[i]._Vy * maintenant._intervalle);
					_metaballes[i]._Vx += FloatRandom( -2, 2 ) ;
				}
			}
			
			// Ajouter eventuellement une metaballe
			if ( NbMetaballes < NbMax)
				if ( maintenant._temps.Subtract( derniereCreation ).TotalMilliseconds> 800 )
			{
				NouvelleMetaballe(ref _metaballes[NbMetaballes]) ;
				NbMetaballes++ ;
				derniereCreation = maintenant._temps ;
			}
			
			#if TRACER
			RenderStop(CHRONO_TYPE.DEPLACE) ;
			#endif
			
		}
	}
}
