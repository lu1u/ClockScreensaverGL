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
		public const string CAT = "Espace.OpenGL" ;

        protected static readonly byte ALPHA = conf.getParametre(CAT, "Alpha", (byte)10);
        protected static readonly float TAILLE_ETOILE = conf.getParametre(CAT, "Taille", 0.15f);
        protected static readonly int NB_ETOILES = conf.getParametre(CAT, "NbEtoiles", 10000);
        protected static readonly float PERIODE_TRANSLATION = conf.getParametre(CAT, "PeriodeTranslation", 13.0f);
        protected static readonly float PERIODE_ROTATION = conf.getParametre(CAT, "PeriodeRotation", 10.0f);
        protected static readonly float VITESSE_ROTATION = conf.getParametre(CAT, "VitesseRotation", 50f);
        protected static readonly float VITESSE_TRANSLATION = conf.getParametre(CAT, "VitesseTranslation", 0.2f);
		protected static readonly float VITESSE = conf.getParametre(CAT, "Vitesse", 1f ) ;
		protected static readonly int NB_SOMMETS_DESSIN = conf.getParametre(CAT, "NbSommets", 12 ) ;
		#endregion
		const float VIEWPORT_X = 5f ;
        const float VIEWPORT_Y = 5f;
        const float VIEWPORT_Z = 5f;
        
		private class Etoile
		{
			public float x, y, z ;
			public bool aSupprimer ;
		}
		private readonly Etoile [] _etoiles ;
		private DateTime _dernierDeplacement = DateTime.Now ;
		private DateTime _debutAnimation = DateTime.Now ;
		
		public Espace(OpenGL gl) :  base( VIEWPORT_X, VIEWPORT_Y, VIEWPORT_Z, 100, NB_SOMMETS_DESSIN, TAILLE_ETOILE )
		{
			_etoiles = new Etoile[NB_ETOILES] ;
			
			// Precalcul des sommets des particules
			
			// Initialiser les etoiles
			for (int i = 0; i < NB_ETOILES; i++)
			{
				NouvelleEtoile( ref _etoiles[i] ) ;
				_etoiles[i].z = FloatRandom( - _tailleCubeZ, _zCamera ) ;
			}
        }

        private void InitGL(OpenGL gl)
        {
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
			float vitesseCamera = (float)Math.Sin(depuisdebut / PERIODE_ROTATION) * VITESSE_ROTATION ;
			
			gl.ClearColor(0, 0, 0, 1) ;
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT); 
            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_COLOR_MATERIAL);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Disable(OpenGL.GL_FOG);
            gl.Disable(OpenGL.GL_DEPTH);

            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
        
			gl.Translate( 0, 0, - _zCamera) ;
            gl.Rotate(0, 0, vitesseCamera);
            
            float [] col = { couleur.R/512.0f, couleur.G/512.0f,couleur.B/512.0f, 1 } ;
			
			foreach (Etoile o in _etoiles)
			{
				gl.Begin(OpenGL.GL_TRIANGLE_FAN);
				gl.Color( col ) ;
				gl.Vertex(o.x, o.y, o.z);
				gl.Color(col[0],col[1],col[2],0.01f) ;
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
			float vitesseCamera = (float)Math.Sin(depuisdebut / PERIODE_ROTATION) * VITESSE_ROTATION ;
			float deltaZ = VITESSE * maintenant._intervalle ;
			float deltaWind = (float)Math.Sin(depuisdebut / PERIODE_TRANSLATION) * VITESSE_TRANSLATION * maintenant._intervalle ;
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
