/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 14/12/2014
 * Heure: 22:50
 * 
 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Text ;

namespace ClockScreenSaverGL.Metaballes
{
	
	public class Metaballes : Fonds.Fond, IDisposable
	{
		const string CAT = "Metaballes" ;
		
		protected readonly int NiveauxCouleurs ;
		protected int 	NbMetaballes ;
		
		protected readonly int[] _palette;
		Bitmap _bmp ;
		
		Rectangle _rectBitmap ;
		protected MetaBalle [] _metaballes ;
		protected readonly int Largeur, Hauteur ;
		static protected DateTime _DernierDeplacement = DateTime.Now ;
		
		// Attributs partages entre tous les objets
		const string COULEURS_INVERSE = "CouleursInversees" ;
		const string NEGATIF = "NegatifCouleurs" ;
		
		protected readonly static double RatioCouleur = conf.getParametre( CAT, "RatioCouleur", 0.3f ) ;	// Plus la valeur est grande, plus la couleur sera foncee. 255 au minimum
		protected static bool _CouleursInverses = conf.getParametre( CAT, COULEURS_INVERSE, false ) ;
		protected static bool _NegatifCouleurs = conf.getParametre( CAT, NEGATIF, false) ;
		
		/// <summary>
		/// Constructeur
		/// </summary>
		public Metaballes(int cx, int cy )
		{
			GetPreferences( ref Largeur, ref Hauteur, ref NbMetaballes, ref NiveauxCouleurs ) ;
			_palette = new int[NiveauxCouleurs];
			if (_metaballes == null)
				_metaballes= new MetaBalle[NbMetaballes] ;
			
			_bmp = new Bitmap(Largeur, Hauteur, PixelFormat.Format32bppRgb);
			_rectBitmap = new Rectangle(0,0, Largeur, Hauteur) ;
			ConstruitMetaballes() ;
		}
		
		~Metaballes()
		{
			Dispose();
		}

		public void Dispose()
		{
			if ( _bmp != null)
			{
				_bmp.Dispose();
				_bmp = null;
			}
		}
		/// <summary>
		/// Lit les preferences a chaque version de metaballes
		/// </summary>
		/// <param name="L"></param>
		/// <param name="H"></param>
		/// <param name="N"></param>
		/// <param name="C"></param>
		protected virtual void GetPreferences( ref int L, ref int H, ref int N, ref int C )
		{
			L = conf.getParametre( CAT, "Largeur", 400 ) ;
			H = conf.getParametre( CAT, "Hauteur", 300 ) ;
			N = conf.getParametre( CAT, "Nombre", 8 ) ;
			C = conf.getParametre( CAT, "NiveauxCouleur", 255 ) ;
		}
		
		protected virtual void ConstruitMetaballes()
		{
			float TailleMax		= conf.getParametre( CAT, "TailleMax", 200 ) ;
			float TailleMin		= conf.getParametre( CAT, "TailleMin", 70 ) ;
			float IntensiteMax	= conf.getParametre( CAT, "IntensiteMax", 0.8f ) ;
			float IntensiteMin	= conf.getParametre( CAT, "IntensiteMin", 0.2f ) ;
			
			for (int i = 0;i < NbMetaballes; i++)
				_metaballes[i] = new MetaBalle( FloatRandom( IntensiteMin, IntensiteMax),	// Intensite
				                               FloatRandom( TailleMin,TailleMax),			// Taille
				                               FloatRandom(0,Largeur), FloatRandom(0, Hauteur), // Position
				                               FloatRandom(-10,10), FloatRandom(-10,10));  // Vitesse
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
			
			TimeSpan diff = maintenant._temps.Subtract(_DernierDeplacement);
			float intervalle = (float)(diff.TotalMilliseconds / 1000.0);
			
			// Deplacement des metaballes
			for ( int i = 0 ;i < NbMetaballes ; i++)
			{
				_metaballes[i]._Px += (_metaballes[i]._Vx * intervalle) ;
				
				if ((_metaballes[i]._Px < 0) && (_metaballes[i]._Vx < 0))
					_metaballes[i]._Vx = Math.Abs(_metaballes[i]._Vx) ;
				else
					if ( (_metaballes[i]._Px  > Largeur ) && (_metaballes[i]._Vx > 0))
						_metaballes[i]._Vx = - Math.Abs(_metaballes[i]._Vx ) ;
				
				_metaballes[i]._Py += (_metaballes[i]._Vy * intervalle) ;
				if ((_metaballes[i]._Py < 0) && (_metaballes[i]._Vy < 0))
					_metaballes[i]._Vy = Math.Abs(_metaballes[i]._Vy) ;
				else
					if ((_metaballes[i]._Py  >Hauteur) && (_metaballes[i]._Vy >0))
						_metaballes[i]._Vy = - Math.Abs(_metaballes[i]._Vy ) ;
			}
			
			
			
			_DernierDeplacement = maintenant._temps ;
			#if TRACER
			RenderStop(CHRONO_TYPE.DEPLACE) ;
			#endif
			
		}
		
		
		/// <summary>
		/// Affiche l'objet
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
			
			// Mise a jour de la palette de couleurs
			updatePalette(couleur) ;
			
			// Construire la bitmap
			BitmapData bmpd = _bmp.LockBits(_rectBitmap, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
			updateFrame(bmpd);
			_bmp.UnlockBits(bmpd);
			
			/*
			#if TRACER
						using ( Graphics gMemGraphics = Graphics.FromImage(_bmp))
			for (int z = 0; z < NB_METABALLES; z++)
					gMemGraphics.DrawRectangle( Pens.White,(float)( _metaballes[z]._Px - _metaballes[z].Taille),
					                           (float)(_metaballes[z]._Py - _metaballes[z].Taille),
					                           (float)(_metaballes[z].Taille*2.0f), (float)(_metaballes[z].Taille*2.0f)) ;
			#endif
			 */
			
			System.Drawing.Drawing2D.SmoothingMode q = g.SmoothingMode ;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
			
			CompositingQuality c = g.CompositingQuality ;
			g.CompositingQuality = CompositingQuality.HighSpeed;
			
			g.DrawImage( _bmp, tailleEcran, _rectBitmap, GraphicsUnit.Pixel ) ;
			g.SmoothingMode = q ;
			g.CompositingQuality = c ;
			#if TRACER
			RenderStop(CHRONO_TYPE.RENDER) ;
			#endif
			
		}
		
		/// <summary>
		/// Pression sur une touche, retourner true si l'objet a traite, false = fin de l'economiseur
		/// </summary>
		/// <param name="f"></param>
		/// <param name="k"></param>
		/// <returns></returns>
		public override bool KeyDown( Form f, Keys k )
		{
			switch ( k )
			{
				case Keys.R:
					{
						ConstruitMetaballes() ;
						return true ;
					}
					
				case Keys.I:
					{
						_CouleursInverses = ! _CouleursInverses ;
						conf.setParametre(  CAT, COULEURS_INVERSE, _CouleursInverses ) ;
						return true ;
					}
					
				case Keys.N:
					{
						_NegatifCouleurs = ! _NegatifCouleurs;
						conf.setParametre( CAT, NEGATIF, _NegatifCouleurs ) ;
						return true ;
					}
			}
			return false ;
		}
		

		
		/// <summary>
		/// Change les couleurs de la palette
		/// La palette est 'monochrome' avec la teinte globale de l'image
		/// </summary>
		/// <param name="c"></param>
		protected virtual void updatePalette( Color c )
		{
			try
			{
				double PI2 = Math.PI * 2;
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
					double i = Math.Abs(Math.Cos(x * PI2 / NiveauxCouleurs ) * NiveauxCouleurs);
					_palette[NiveauxCouleurs-1-x] =  ((int)(r * i) << 16 ) | ((int)(g * i) << 8 ) | (int)(b * i) ;
				}
				else
					for (int x = 1; x < NiveauxCouleurs; x++)
				{
					double i = Math.Abs(Math.Sin(x * PI2 / NiveauxCouleurs ) * NiveauxCouleurs);
					_palette[x] =  ((int)(r * i) << 16 ) | ((int)(g * i) << 8 ) | (int)(b * i) ;
					
					_palette[0] = 0 ;
				}
				
				
			}
			catch( Exception ex )
			{
				MessageBox.Show(ex.Message + "\ndans : " + ex.Source, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error ) ;
			}
		}

		/// <summary>
		/// Rempli les pixels de la bitmap
		/// </summary>
		/// <param name="bmpd"></param>
		unsafe void updateFrame(BitmapData bmpd)
		{
			#if TRACER
			RenderStart(CHRONO_TYPE.RENDER) ;
			#endif
			
			double x, y ;
			int z, Indice ;
			double Champs ;
			double largeur = bmpd.Width;
			for (y = 0; y < bmpd.Height; y++)
			{
				int *pixels = (int *)(bmpd.Scan0  + ((int)y*bmpd.Stride));
				
				for (x = 0; x < largeur; x++)
				{
					// Calcul de la valeur du champs en ce point = somme des champs de toutes les metaballes
					Champs = 0 ;
					for ( z = 0; z < NbMetaballes; z++)
						Champs+= _metaballes[z].Champ( x, y ) ;
					
					// Convertir en indice dans la palette
					Indice = (int) Math.Round(Champs*NiveauxCouleurs) ;
					if ( Indice >= NiveauxCouleurs )
						Indice = NiveauxCouleurs-1 ;
					else
						if ( Indice < 0 )
							Indice = 0 ;
					
					*pixels++ = _palette[Indice] ;
				}
			}
			
			#if TRACER
			RenderStop(CHRONO_TYPE.RENDER) ;
			#endif

		}
		
		public override void AppendHelpText( StringBuilder s )
		{
			s.Append( Resources.AideMetaballes );
		}
	}
}