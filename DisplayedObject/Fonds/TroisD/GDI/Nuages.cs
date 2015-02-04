/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 20/01/2015
 * Heure: 18:59
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ClockScreenSaver ;
namespace ClockScreenSaverGL.Fonds.TroisD.GDI
{
	/// <summary>
	/// Description of Nuage.
	/// </summary>
	public class NuagesGDI : TroisDGDI
	{
        const string CAT = "Nuages.GDI";
		readonly byte ALPHA = conf.getParametre( CAT, "Alpha", (byte)20 ) ;
		readonly float VITESSE = - conf.getParametre( CAT, "Vitesse", 10f ) ;
		readonly float RAYON_MAX = conf.getParametre( CAT, "RayonMax", 1000f) ;
		readonly float RAYON_MIN = conf.getParametre( CAT, "RayonMin", 200f) ;
		readonly int NBPARTICULES_MIN = conf.getParametre( CAT, "NbParticulesMin", 60) ;
		readonly int NBPARTICULES_MAX = conf.getParametre( CAT, "NbParticulesMax", 80) ;
		readonly int TAILLE_PARTICULE_MIN = conf.getParametre(CAT, "TailleParticulesMin", 100 ) ;
		readonly int TAILLE_PARTICULE_MAX = conf.getParametre(CAT, "TailleParticulesMax", 400 ) ;
		readonly int NB_NUAGES = conf.getParametre( CAT, "Nb", 100) ;
		float _positionNuage = conf.getParametre(CAT, "EnHaut", true ) ? -1f : 1f;
		private class Nuage
		{
			public int _nbParticules ;
			public int[] _type ;
			public float _rayon ;
			public float _centreX, _centreY, _centreZ ;
			public Vecteur3D[] _points ;
			public float[] _tailles ;
		}
		
		private Bitmap _nuage1 = Resources.nuage1 ;
		private Bitmap _nuage2 = Resources.nuage2 ;
		private Bitmap _nuage3 = Resources.nuage3 ;
		private Nuage[] _nuages  ;
		static protected DateTime _DernierDeplacement = DateTime.Now ;
		
		public NuagesGDI( int Cx, int Cy)
		{
			_nuages = new Nuage[NB_NUAGES] ;
			_largeur = Cx ;
			_hauteur = Cy ;
			
			_centreX = _largeur/2 ;
			_centreY = _hauteur/2 ;
			
			_tailleCubeX = _largeur /2 ;
			_tailleCubeY = _hauteur /2 ;
			_tailleCubeZ = _largeur   ;
			
			_zEcran	 = -_tailleCubeZ/160 ;
			_zCamera = _zEcran * 5f ;
			
			
			for ( int i = 0; i < NB_NUAGES; i++)
			{
				CreateNuage( ref _nuages[i], true ) ;
			}
		}
		
		void CreateNuage(ref Nuage nuage, bool init)
		{
			if ( nuage == null)
			{
				nuage = new Nuage() ;
			}
			
			nuage._nbParticules = r.Next(NBPARTICULES_MIN, NBPARTICULES_MAX) ;
			nuage._rayon = FloatRandom( RAYON_MIN, RAYON_MAX) ;
			nuage._centreX = FloatRandom( - _tailleCubeX*50, _tailleCubeX*50) ;
			nuage._centreY = _positionNuage *_tailleCubeY*4 ; //FloatRandom( _tailleCubeY/2, _tailleCubeY ) ;
			
			if ( init )
				nuage._centreZ = FloatRandom( _zCamera, _tailleCubeZ*8 ) ;
			else
				nuage._centreZ = FloatRandom( _tailleCubeZ/2, _tailleCubeZ*8) ; ;
			
			nuage._points = new Vecteur3D[nuage._nbParticules] ;
			nuage._tailles = new float[nuage._nbParticules] ;
			nuage._type = new int[nuage._nbParticules] ;
			
			for ( int i = 0; i < nuage._nbParticules; i++)
			{
				nuage._points[i] = new Vecteur3D(nuage._centreX + FloatRandom( -nuage._rayon, nuage._rayon ),
				                                 nuage._centreY + FloatRandom( -nuage._rayon, nuage._rayon ),
				                                 nuage._centreZ + FloatRandom( -nuage._rayon/8, nuage._rayon/8 ) ) ;
				
				nuage._tailles[i] = FloatRandom( TAILLE_PARTICULE_MIN, TAILLE_PARTICULE_MAX) ;
				nuage._type[i] = r.Next(0,2) ;
				
			}
		}
		public override void AfficheGDI( Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur )
		{
			#if DEBUG
            RenderStart(CHRONO_TYPE.RENDER);
			#endif
			System.Drawing.Drawing2D.SmoothingMode s = g.SmoothingMode ;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
			CompositingQuality q = g.CompositingQuality ;
			g.CompositingQuality = CompositingQuality.HighSpeed;
			InterpolationMode In = g.InterpolationMode ;
			g.InterpolationMode = InterpolationMode.NearestNeighbor ;
			
			using ( LinearGradientBrush linGrBrush = new LinearGradientBrush( tailleEcran,
			                                                                 Color.Black,   // Opaque red
			                                                                 getCouleurOpaqueAvecAlpha(couleur, 255),
			                                                                 LinearGradientMode.Vertical))

				g.FillRectangle(linGrBrush, tailleEcran);
			
			float X, Y, X2, Y2 ;
			
			bool nuageAffiche ;
			
			using ( Bitmap bmp1 = BitmapNuance(g, _nuage1, getCouleurAvecAlpha( couleur, ALPHA) ),
			       bmp2 = BitmapNuance(g, _nuage2, getCouleurAvecAlpha( couleur, ALPHA) ),
			       bmp3 = BitmapNuance(g, _nuage3, getCouleurAvecAlpha( couleur, ALPHA) )
			      )
				for ( int i = 0; i < NB_NUAGES; i++)
			{
				nuageAffiche = false ;
				
				for ( int j = 0; j < _nuages[i]._nbParticules; j++)
					
					if  (_nuages[i]._points[j].z > _zCamera )
				{
					Coord2DFrom3D( _nuages[i]._points[j].x - _nuages[i]._tailles[j],
					              _nuages[i]._points[j].y - _nuages[i]._tailles[j],
					              _nuages[i]._points[j].z,
					              out X, out Y ) ;
					
					Coord2DFrom3D( _nuages[i]._points[j].x + _nuages[i]._tailles[j],
					              _nuages[i]._points[j].y + _nuages[i]._tailles[j],
					              _nuages[i]._points[j].z,
					              out X2, out Y2 ) ;
					try
					{
						NormalizeCoord( ref X, ref X2, ref Y, ref Y2 ) ;
						if ( (X < _largeur) && (X2 > 0) && (Y < _hauteur) && (Y2 > 0) )
						{
							switch( _nuages[i]._type[j] )
							{
								case 0:
									g.DrawImage(bmp1, X, Y, X2-X, Y2-Y ) ;
									break ;
									
								case 1:
									g.DrawImage(bmp2, X, Y, X2-X, Y2-Y ) ;
									break ;
								case 2:
									g.DrawImage(bmp3, X, Y, X2-X, Y2-Y ) ;
									break ;
									
							}
							nuageAffiche = true ;
						}
					}
					catch
					{
						nuageAffiche = false ;
					}
				}
				
				if ( ! nuageAffiche )
					CreateNuage( ref _nuages[i], false ) ;
			}
			
			g.SmoothingMode = s ;
			g.CompositingQuality = q ;
			g.InterpolationMode = In ;
			#if DEBUG
            RenderStop(CHRONO_TYPE.RENDER);
			#endif
			
		}
		
		public void NormalizeCoord(ref float X, ref float X2, ref float Y, ref float Y2 )
		{
			if ( Y2 < Y)
			{
				float t = Y2 ;
				Y2 = Y ;
				Y = t ;
			}
			
			if ( X2 < X)
			{
				float t = X2 ;
				X2 = X ;
				X = t ;
			}
		}
		
		public override void Deplace( Temps maintenant, Rectangle tailleEcran )
		{
			float vitesse = maintenant._intervalle * VITESSE ;
			bool derriereCam ;
			
			for ( int i = 0; i < NB_NUAGES; i++)
			{
				derriereCam = true ;
				for ( int j = 0; j < _nuages[i]._nbParticules; j++)
				{
					_nuages[i]._points[j].z += vitesse ;
					_nuages[i]._centreZ  += vitesse ;
					
					if (_nuages[i]._points[j].z > _zCamera)
						derriereCam = false ;
				}
				
				if ( derriereCam )
					CreateNuage( ref _nuages[i], false );
			}
		}
		
		/// <summary>
		/// Pression sur une touche, retourner true si l'objet a traite, false = fin de l'economiseur
		/// </summary>
		/// <param name="f"></param>
		/// <param name="k"></param>
		/// <returns></returns>
		///
		public  override bool KeyDown( Form f, Keys k )
		{
			if (k == Keys.I )
			{
				_positionNuage = - _positionNuage ;
				conf.setParametre(CAT, "EnHaut", _positionNuage > 0  ? true : false );
				
				for ( int i = 0; i < NB_NUAGES; i++)
				{
					CreateNuage( ref _nuages[i], true ) ;
				}
				return true ;
			}
			else
				return false ;
		}
	}
	
}

