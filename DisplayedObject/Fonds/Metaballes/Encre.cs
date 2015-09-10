/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 17/12/2014
 * Heure: 21:43
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Drawing ;
namespace ClockScreenSaverGL.Metaballes
{
	/// <summary>
	/// Description of Fumee.
	/// </summary>
	public class Encre : Metaballes
	{
		const String CAT = "Encre" ;
		
		static float TailleMax		;
		static float TailleMin ;
		static float IntensiteMax ;
		static float IntensiteMin ;
		static float xEmetteur ;
		static DateTime derniereCreation = DateTime.Now ;
		
		int NbMax = 0 ;
		public Encre( int cx, int cy): base( cx, cy)
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
			L = conf.getParametre( CAT, "Largeur", 400 ) ;
			H = conf.getParametre( CAT, "Hauteur", 300 ) ;
			N = conf.getParametre( CAT, "Nombre", 10 ) ;
			C = conf.getParametre( CAT, "Niveaux", 512 ) ;
		}
		
		protected override void ConstruitMetaballes()
		{
			TailleMax		= conf.getParametre( CAT, "TailleMax", 100f ) ;
			TailleMin		= conf.getParametre( CAT, "TailleMin", 30f ) ;
			IntensiteMax	= conf.getParametre( CAT, "IntensiteMax", 1.0f) ;
			
			IntensiteMin	= 0 ;
			NbMax = conf.getParametre( CAT, "Nombre", 10 ) ;
			NbMetaballes = 0 ;
			xEmetteur = Largeur/2 ;
			
			NouvelleMetaballe( ref _metaballes[0] );
			_metaballes[0]._Py = Hauteur+_metaballes[0]._Taille/2;
				NbMetaballes++ ;
		}
		
		/// <summary>
		/// Changer l'image
		/// </summary>
		/// <param name="maintenant"></param>
		/// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
		{
			#if TRACER
			RenderStart(CHRONO_TYPE.DEPLACE) ;
			#endif
			
			// Deplacement des metaballes
			for ( int i = 0 ;i < NbMetaballes ; i++)
			{
                _metaballes[i]._Px += (_metaballes[i]._Vx * maintenant._intervalle);
				
				if ( (_metaballes[i]._Px < -_metaballes[i]._Taille ) && (_metaballes[i]._Vx < 0) ||	// Trop a gauche
				    ( (_metaballes[i]._Px - _metaballes[i]._Taille) > Largeur ) && (_metaballes[i]._Vx > 0) || // Trop a droite
				    (_metaballes[i]._Py < -_metaballes[i]._Taille) ||  // Arrivee en haut
				    (_metaballes[i]._Intensite < IntensiteMin)
				   )
				{
					// Remettre le flocon en bas
					NouvelleMetaballe(ref _metaballes[i]);
				}
				else

                    _metaballes[i]._Py += (_metaballes[i]._Vy * maintenant._intervalle);
				
				// Variations de la vitesse horizontale + Variations sinusoidales en fonction de la profondeur
				_metaballes[i]._Vx += //((float)Math.Sin( _metaballes[i]._Py / Hauteur * Math.PI*4.0 ))*0.25f
                    +FloatRandom(-20, 20) * maintenant._intervalle;
				
				// De moins en moins opaque
				if ( _metaballes[i]._Intensite > IntensiteMin)
                    _metaballes[i]._Intensite -= (_metaballes[i]._Intensite * 0.1 * maintenant._intervalle);
				
				// de plus en plus grandes
				if ( _metaballes[i]._Taille < TailleMax )
				{
                    _metaballes[i]._Taille += (_metaballes[i]._Taille * 0.1 * maintenant._intervalle);
					_metaballes[i]._TailleCarre = _metaballes[i]._Taille * _metaballes[i]._Taille;
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
			
			// Deplacer l'emetteur
			xEmetteur += FloatRandom( -5, 5 ) ;
			if ( xEmetteur < 0 )
				xEmetteur = 0 ;
			else
				if ( xEmetteur > Largeur)
					xEmetteur = Largeur ;

            updateFrame();
			#if TRACER
			RenderStop(CHRONO_TYPE.DEPLACE) ;
			#endif
			
		}
		
		/// <summary>
		/// Creation d'une nouvelle metaballe: elle commencent plus petites et plus opaque et de diluent en montant
		/// </summary>
		/// <returns></returns>
		private void NouvelleMetaballe(ref MetaBalle balle )
		{
			float taille = FloatRandom( TailleMin*0.7f,TailleMin*1.2f);
			
			if ( balle == null )
				balle = new MetaBalle( IntensiteMax,	// Intensite
				                      taille,			// Taille
				                      xEmetteur + FloatRandom(-10, 10), Hauteur+taille,
				                      0, FloatRandom(-20,-10));
			else
				balle.Reset( IntensiteMax,	// Intensite
				            taille,			// Taille
				            xEmetteur + FloatRandom(-10, 10), Hauteur+taille,
				            0, FloatRandom(-20,-10));
		}
		
		/// <summary>
		/// Change les couleurs de la palette
		/// La palette est 'monochrome' avec la teinte globale de l'image
		/// </summary>
		/// <param name="c"></param>
		///
		protected override void updatePalette( Color c )
		{
			try
			{
				double r, g, b ;
				
				if  ( _CouleursInverses )
				{
					r = (255-c.R) ;
					g = (255-c.G) ;
					b = (255-c.B) ;
				}
				else
				{
					r = c.R;
					g = c.G;
					b = c.B;
				}
				
				r = r * RatioCouleur / NiveauxCouleurs ;
				g = g * RatioCouleur / NiveauxCouleurs ;
				b = b * RatioCouleur / NiveauxCouleurs ;
				
				if ( _NegatifCouleurs )
					for (int x = 0; x < NiveauxCouleurs; x++)
				{
					
					_palette[NiveauxCouleurs-1-x] =  ((int)(r * x) << 16 ) | ((int)(g *x) << 8 ) | (int)(b * x) ;
				}
				else
					for (int x = 0; x < NiveauxCouleurs; x++)
				{
					_palette[x] =  ((int)(r * x) << 16 ) | ((int)(g * x) << 8 ) | (int)(b * x) ;
				}
				
				
			}
			catch
			{
				
			}
		}
	}
}
