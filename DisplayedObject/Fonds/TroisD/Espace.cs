using SharpGL;

using System;
using System.Drawing;
namespace ClockScreenSaverGL.Fonds.TroisD
	
{
	/// <summary>
	/// Description of Neige.
	/// </summary>
	public class Espace : TroisD
	{
		#region Parametres
		public const string CAT = "Espace" ;
		
		protected readonly byte ALPHA = conf.getParametre(CAT, "Alpha", (byte)10 ) ;
		protected readonly float TAILLE_ETOILE = conf.getParametre(CAT, "Taille", 0.25f ) ;
		protected readonly int NB_ETOILES = conf.getParametre(CAT, "NbEtoiles", 5000 ) ;
		protected readonly float _periodeTranslation = conf.getParametre(CAT, "PeriodeTranslation", 13.0f ) ;
		protected readonly float _periodeRotation = conf.getParametre(CAT, "PeriodeRotation", 10.0f ) ;
		protected readonly float _vitesseRotation = conf.getParametre(CAT, "VitesseRotation", 100 ) ;
		protected readonly float _vitesseTranslation = conf.getParametre(CAT, "VitesseTranslation", 1f ) ;
		protected static readonly float _vitesse = conf.getParametre(CAT, "Vitesse", 5f ) ;
		protected readonly int NB_SOMMETS_DESSIN = conf.getParametre(CAT, "NbSommets", 12 ) ;
		
		const float VIEWPORT_X = 5f ;
		const float VIEWPORT_Y = 5f ;
		#endregion
		
		private class Etoile
		{
			public float x, y, z ;
			public bool aSupprimer ;
		}
		private readonly Etoile [] _etoiles ;
		private DateTime _dernierDeplacement = DateTime.Now ;
		private DateTime _debutAnimation = DateTime.Now ;
		private PointF[] _coordPoint ;
		public Espace(OpenGL gl)
		{
			_tailleCubeX = VIEWPORT_X ;
			_tailleCubeY = VIEWPORT_Y ;
			_tailleCubeZ = VIEWPORT_Y ;
			
			_zCamera = 100 ;
			
			_etoiles = new Etoile[NB_ETOILES] ;
			
			// Precalcul des sommets des particules
			_coordPoint = new PointF[NB_SOMMETS_DESSIN] ;
			for ( int i = 0; i < NB_SOMMETS_DESSIN; i++)
				_coordPoint[i] = new PointF((float)( TAILLE_ETOILE * Math.Sin( i * (Math.PI*2.0) / NB_SOMMETS_DESSIN )),
				                            (float)( TAILLE_ETOILE * Math.Cos( i * (Math.PI*2.0) / NB_SOMMETS_DESSIN )));
			
			// Initialiser les etoiles
			for (int i = 0; i < NB_ETOILES; i++)
			{
				NouvelleEtoile( ref _etoiles[i] ) ;
				_etoiles[i].z = FloatRandom( - _tailleCubeZ, _zCamera ) ;
			}

            InitGL(gl);
		}

        private void InitGL(OpenGL gl)
        {
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);

            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
        }
		
		private void NouvelleEtoile( ref Etoile f )
		{
			if ( f == null )
				f = new Etoile() ;
			
			f.aSupprimer = false ;
			f.x = FloatRandom( - _tailleCubeX*8, _tailleCubeX*8 ) ;
			f.z = - _tailleCubeZ ;
			f.y = FloatRandom( - _tailleCubeY*8, _tailleCubeY*8 ) ;
			}
		
		/// <summary>
		/// Affichage des flocons
		/// </summary>
		/// <param name="g"></param>
		/// <param name="maintenant"></param>
		/// <param name="tailleEcran"></param>
		/// <param name="couleur"></param>
		public override void AfficheOpenGL( OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur )
		{
			#if TRACER
			RenderStart(CHRONO_TYPE.RENDER) ;
			#endif
			float depuisdebut = (float)(_debutAnimation.Subtract(_dernierDeplacement).TotalMilliseconds / 1000.0);
			float vitesseCamera = (float)Math.Sin(depuisdebut / _periodeRotation) * _vitesseRotation ;
			
			gl.ClearColor(0, 0, 0, 1) ;
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT); 
			gl.LoadIdentity();
			gl.Translate( 0, 0, - _zCamera) ;
            gl.Rotate(0, 0, vitesseCamera);
            
            float [] col = { couleur.R/512.0f, couleur.G/512.0f,couleur.B/512.0f, 1 } ;
			
			foreach (Etoile o in _etoiles)
			{
				gl.Begin(OpenGL.GL_TRIANGLE_FAN);
				gl.Color( col ) ;
				gl.Vertex(o.x, o.y, o.z);
				gl.Color(col[0],col[1],col[2],0f) ;
				for (int i = 0; i < NB_SOMMETS_DESSIN; i++)
					gl.Vertex(o.x + _coordPoint[i].X, o.y + _coordPoint[i].Y, o.z);
				gl.Vertex(o.x + _coordPoint[0].X, o.y + _coordPoint[0].Y, o.z);
				gl.End();
			}
			
			#if TRACER
			RenderStop(CHRONO_TYPE.RENDER) ;
			#endif
		}
		

		
		/// <summary>
		/// Deplacement de tous les objets: flocons, camera...
		/// </summary>
		/// <param name="maintenant"></param>
		/// <param name="tailleEcran"></param>
		public override void Deplace( Temps maintenant, Rectangle tailleEcran )
		{
			#if TRACER
			RenderStart(CHRONO_TYPE.DEPLACE) ;
			#endif
			
			float depuisdebut = (float)(_debutAnimation.Subtract(_dernierDeplacement).TotalMilliseconds / 1000.0);
			float vitesseCamera = (float)Math.Sin(depuisdebut / _periodeRotation) * _vitesseRotation ;
			float deltaZ = _vitesse * maintenant._intervalle ;
			float deltaWind = (float)Math.Sin(depuisdebut / _periodeTranslation) * _vitesseTranslation * maintenant._intervalle ;
			// Deplace les flocons
			bool trier = false ;
			for (int i = 0; i < NB_ETOILES; i++)
			{
				if ( _etoiles[i].aSupprimer)
				{
					NouvelleEtoile( ref _etoiles[i] ) ;
					trier = true ;
				}
				else
				{
					if ( _etoiles[i].z >  _zCamera )
						_etoiles[i].aSupprimer = true ;
					else
					{
					_etoiles[i].z += deltaZ ;
					_etoiles[i].x += deltaWind ;
					}
				}
			}
			
			
			if ( trier)
			Array.Sort(_etoiles, delegate(Etoile O1, Etoile O2) {
			           	if  (O1.z > O2.z) return 1 ;
			           	if  (O1.z < O2.z) return -1 ;
			           	return 0 ;
			           });
			_dernierDeplacement = maintenant._temps ;
			
			#if TRACER
			RenderStop(CHRONO_TYPE.DEPLACE) ;
			#endif
			
		}
		
		public override void OpenGLInitialized( OpenGL gl )
		{
		
		}
	}
}
