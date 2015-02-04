/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 30/12/2014
 * Heure: 23:08
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
namespace ClockScreenSaverGL.Fonds.TroisD.GDI
	
{
	/// <summary>
	/// Description of Neige.
	/// </summary>
	public class NeigeGDI : TroisDGDI
	{
		public const string CAT = "Neige.GDI" ;
		
		protected static readonly byte ALPHA = conf.getParametre(CAT, "Alpha", (byte)10);
		protected static readonly float VITESSE_ROTATION = conf.getParametre(CAT, "VitesseRotation", 0.25f) ;
		protected static readonly float PERIODE_ROTATION = conf.getParametre(CAT, "PeriodeRotation", 20.0f) ;
		protected static readonly float VITESSE_Y = conf.getParametre(CAT, "VitesseChute", 200f) ;
		protected static readonly float MAX_VENT = conf.getParametre(CAT, "MaxVent", 100f) ;
		protected static readonly float VITESSE_DELTA_VENT = conf.getParametre(CAT, "VitesseDeltaVent", 500f) ;
		
		protected readonly int NB_FLOCONS = conf.getParametre(CAT, "NbFlocons", 200 )  ;
		protected readonly int TAILLE_MIN = conf.getParametre(CAT, "TailleMin", 10 )  ;
		protected readonly int TAILLE_MAX = conf.getParametre(CAT, "TailleMax", 64 )  ;
		protected readonly Objet3D [] _flocons ;
		
		protected Bitmap _bmpFlocon1 = Resources.flocon1 ;
		protected Bitmap _bmpFlocon2 = Resources.flocon2 ;
		static protected DateTime _DernierDeplacement = DateTime.Now ;
		
		protected float _xWind = 0 ;
		
		static float	_xRotation ;
		static DateTime derniereCreation = DateTime.Now ;
		static DateTime debut = DateTime.Now ;
		public NeigeGDI( int Cx, int Cy )
		{
			_largeur = Cx ;
			_hauteur = Cy ;
			
			_centreX = _largeur/2 ;
			_centreY = _hauteur/2 ;
			
			_tailleCubeX = _largeur /2 ;
			_tailleCubeY = _hauteur /2 ;
			_tailleCubeZ = _largeur   ;
			
			_zEcran = - 0 ;
			_zCamera = - _tailleCubeZ/2  ;
			_xRotation = _tailleCubeX * 0.75f;
			
			_zEcran	 = -_tailleCubeZ ;
			_zCamera = _zEcran * 2 ;
			
			_flocons = new Objet3D[NB_FLOCONS] ;
			
			for (int i = 0; i < NB_FLOCONS; i++)
				//NouveauFlocon( ref _flocons[i] ) ;
			{
				_flocons[i] = new Objet3D() ;
				
				_flocons[i].aSupprimer = false ;
				_flocons[i].x = FloatRandom( - _tailleCubeX*2, _tailleCubeX*2 ) ;
				_flocons[i].z = FloatRandom( - _tailleCubeZ*2, _tailleCubeZ*3) ;
				_flocons[i].y = FloatRandom( - _tailleCubeY*2, _tailleCubeY*2 ) ;
				
				_flocons[i].type = r.Next(0,2) ;
				_flocons[i].rayon = FloatRandom( 32, 64) ;
				_flocons[i].vx = FloatRandom( -10, 10 ) ;
				_flocons[i].vy = FloatRandom( 100, 200 ) ;
				_flocons[i].vz = FloatRandom( -5, 5 ) ;
			}
		}
		
		private void NouveauFlocon( ref Objet3D f )
		{
			if ( f == null )
				f = new Objet3D() ;
			
			f.aSupprimer = false ;
			f.x = FloatRandom(	- _tailleCubeX*2, _tailleCubeX*2 ) ;
			f.z = FloatRandom( -_tailleCubeZ*2, _tailleCubeZ*3) ;
			f.y = Coord3DFrom2D( - _hauteur/4, f.z, _hauteur ) ;
			
			f.type = r.Next(0,2) ;
			f.rayon = FloatRandom( TAILLE_MIN, TAILLE_MAX) ;
			f.vx = FloatRandom( -10, 10 ) ;
			f.vy = FloatRandom( VITESSE_Y/2, VITESSE_Y ) ;
			f.vz = FloatRandom( -5, 5 ) ;
		}
		
		/// <summary>
		/// Affichage des flocons
		/// </summary>
		/// <param name="g"></param>
		/// <param name="maintenant"></param>
		/// <param name="tailleEcran"></param>
		/// <param name="couleur"></param>
		public override void AfficheGDI( Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur )
		{
			#if DEBUG
            RenderStart(CHRONO_TYPE.RENDER);
			#endif
			
			TimeSpan diff = maintenant._temps.Subtract(_DernierDeplacement);
			float intervalle = (float)(diff.TotalMilliseconds / 1000.0);
			float X, Y, X2, Y2;
			
			using ( Bitmap bmp1 = BitmapNuance(g, _bmpFlocon1, getCouleurAvecAlpha( couleur, ALPHA) ),
			       bmp2 = BitmapNuance(g, _bmpFlocon2, getCouleurAvecAlpha( couleur, ALPHA) ))
				for (int i = 0; i < NB_FLOCONS; i++)
			{
				if ( ! _flocons[i].aSupprimer )
					if ( _flocons[i].z >= _zCamera)
				{
					Coord2DFrom3D( _flocons[i].x, _flocons[i].y, _flocons[i].z, out X, out Y ) ;
					Coord2DFrom3D( _flocons[i].x + _flocons[i].rayon, _flocons[i].y + _flocons[i].rayon, _flocons[i].z, out X2, out Y2 ) ;
					NormalizeCoord( ref X, ref X2, ref Y, ref Y2 ) ;
					
					if ( Y > _hauteur )
						NouveauFlocon(ref _flocons[i]);
					else
						try
						{
						g.DrawImage( _flocons[i].type == 0 ? bmp1 : bmp2, X,Y, X2-X, Y2-Y ) ;
						}
						catch
						{
						}
				}
				
			}
			
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
			
			if  (X > _largeur)
			{
				X -= _largeur ;
				X2 -= _largeur ;
			}
			else
				if ( X2 < 0)
			{
				X += _largeur ;
				X2 += _largeur ;
			}
		}
		
		/// <summary>
		/// Deplacement de tous les objets: flocons, camera...
		/// </summary>
		/// <param name="maintenant"></param>
		/// <param name="tailleEcran"></param>
		public override void Deplace( Temps maintenant, Rectangle tailleEcran )
		{
			float intervalle = (float)(maintenant._temps.Subtract(_DernierDeplacement).TotalMilliseconds / 1000.0);
			float depuisdebut = (float)(debut.Subtract(_DernierDeplacement).TotalMilliseconds / 1000.0);
			float vitesseCamera = (float)Math.Sin(depuisdebut / PERIODE_ROTATION) * VITESSE_ROTATION ;
			
			float CosTheta = (float) Math.Cos(vitesseCamera * intervalle) ;
			float SinTheta = (float) Math.Sin(vitesseCamera * intervalle) ;
			float px, pz ;
			
			// Deplace les flocons
			for (int i = 0; i < NB_FLOCONS; i++)
			{
				if ( _flocons[i].aSupprimer)
				{
					if ( DateTime.Now.Subtract( derniereCreation ).TotalMilliseconds>= 1 )
					{
						NouveauFlocon( ref _flocons[i] ) ;
						derniereCreation = DateTime.Now ;
					}
					
				}
				else
				{
					// Deplacement
					_flocons[i].x += ((_flocons[i].vx+_xWind) * intervalle) ;
					_flocons[i].y += (_flocons[i].vy * intervalle) ;
					_flocons[i].z += (_flocons[i].vz * intervalle) ;
					
					// Variation de vitesse
					Varie( ref _flocons[i].vx, -200, 200, 1000, intervalle ) ;
					Varie( ref _flocons[i].vz, -200, 200, 1000, intervalle ) ;
					
					// Rotation due a la position de la camera
					px = (CosTheta * (_flocons[i].x-_xRotation)) - (SinTheta * _flocons[i].z) + _xRotation ;
					pz = (SinTheta * (_flocons[i].x-_xRotation)) + (CosTheta * _flocons[i].z) ;
					
					_flocons[i].x = px ;
					_flocons[i].z = pz ;
				}
			}
			
			
			Varie( ref _xWind, -MAX_VENT, MAX_VENT, VITESSE_DELTA_VENT, intervalle ) ;
			Varie( ref _xRotation, - _tailleCubeX/2,_tailleCubeX/2, 500, intervalle) ;
			_DernierDeplacement = maintenant._temps ;
		}
		
		
		
		
		#if DEBUG
		public override String DumpRender(  )
		{
			return ( base.DumpRender() + " vent " + _xWind.ToString("00.0") ) ;
		}
		#endif
		
	}
}
