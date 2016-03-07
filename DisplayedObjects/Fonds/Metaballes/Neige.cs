/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 17/12/2014
 * Heure: 21:19
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using SharpGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ClockScreenSaverGL.DisplayedObjects.Metaballes
{
	/// <summary>
	/// Description of Neige.
	/// </summary>
	public class Neige : Metaballes
	{
		const string CAT = "NeigeMeta" ;
		static float TailleMax, TailleMin, IntensiteMax,IntensiteMin ;
		static int NbMax;
		//static DateTime derniereCreation = DateTime.Now ;
        TimerIsole _timer = new TimerIsole(800);
		
		public Neige(OpenGL gl, int cx, int cy) : base(gl, cx, cy)
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
            RatioCouleur = conf.getParametre(CAT, "RatioCouleur", 0.9f);
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
        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
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
			//	if ( maintenant._temps.Subtract( derniereCreation ).TotalMilliseconds> 800 )
            if ( _timer.Ecoule())
			{
				NouvelleMetaballe(ref _metaballes[NbMetaballes]) ;
				NbMetaballes++ ;
				//derniereCreation = maintenant._temps ;
			}

            updateFrame();
            
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE) ;
			#endif
			
		}
        /// <summary>
		/// Change les couleurs de la palette
		/// La palette est 'monochrome' avec la teinte globale de l'image
		/// </summary>
		/// <param name="c"></param>
		///
		protected override void updatePalette(Color c)
        {
            try
            {
                double r, g, b;

                if (_CouleursInverses)
                {
                    r = (255 - c.R);
                    g = (255 - c.G);
                    b = (255 - c.B);
                }
                else
                {
                    r = c.R;
                    g = c.G;
                    b = c.B;
                }

                r = r * RatioCouleur / NiveauxCouleurs;
                g = g * RatioCouleur / NiveauxCouleurs;
                b = b * RatioCouleur / NiveauxCouleurs;

                if (_NegatifCouleurs)
                    for (int x = 0; x < NiveauxCouleurs; x++)
                    {

                        _palette[NiveauxCouleurs - 1 - x] = ((int)(r * x) << 16) | ((int)(g * x) << 8) | (int)(b * x);
                    }
                else
                    for (int x = 0; x < NiveauxCouleurs; x++)
                    {
                        _palette[x] = ((int)(r * x) << 16) | ((int)(g * x) << 8) | (int)(b * x);
                    }


            }
            catch
            {

            }
        }
    }
}
