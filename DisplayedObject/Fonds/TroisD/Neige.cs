using SharpGL;
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
using GLfloat = System.Single;
namespace ClockScreenSaverGL.Fonds.TroisD
	
{
	/// <summary>
	/// Description of Neige.
	/// </summary>
	public class Neige : TroisD
	{
		public const string CAT = "Neige" ;
		
		protected static readonly float VITESSE_ROTATION = conf.getParametre(CAT, "VitesseRotation", 0.2f) ;
		protected static readonly float PERIODE_ROTATION = conf.getParametre(CAT, "PeriodeRotation", 20.0f) ;
		protected static readonly float VITESSE_Y = conf.getParametre(CAT, "VitesseChute", 5) ;
		protected static readonly float VITESSE_DELTA_VENT = conf.getParametre(CAT, "VitesseDeltaVent", 1f) ;
		protected static readonly float MAX_VENT = conf.getParametre(CAT, "MaxVent", 3f) ;
		protected static readonly float ALPHA_CENTRE = conf.getParametre(CAT, "AlphaCentre", 0.5f) ;
		protected static readonly float ALPHA_BORD = conf.getParametre(CAT, "AlphaBord", 0.05f) ;
		
		protected readonly int NB_FLOCONS = conf.getParametre(CAT, "NbFlocons", 5000 )  ;
		
		private class Flocon
		{
			public float x, y, z ;
			public float vx, vy, vz ;
			public float ax, ay, az ;
			public float r ;				// Ratio entre les branches et les creux des flocons, pour varier les formes
		}
		
		private readonly Flocon [] _flocons ;
		
		protected float _xWind = 0 ;
		
		static float	_xRotation ;
		static DateTime debut = DateTime.Now ;
		const float VIEWPORT_X = 2f ;
        const float VIEWPORT_Y = 2f;
        const float VIEWPORT_Z = 2f;
        const int NB_SOMMETS_DESSIN = 12;
		const float TAILLE_FLOCON = 0.8f ;


        public Neige()
            : base(VIEWPORT_X, VIEWPORT_Y, VIEWPORT_Z, 100, NB_SOMMETS_DESSIN, 1.0f)
		{
			_xRotation = _tailleCubeX * 0.75f;
			
			_flocons = new Flocon[NB_FLOCONS] ;
			for (int i = 0; i < NB_FLOCONS; i++)
			{
				_flocons[i] = new Flocon() ;
				
				_flocons[i].x = FloatRandom( - _tailleCubeX*50, _tailleCubeX*50) ;
				_flocons[i].z = FloatRandom( - _tailleCubeZ*5, _zCamera ) ;
				_flocons[i].y = FloatRandom( - _tailleCubeY*16, _tailleCubeY*16 ) ;
				
				_flocons[i].vx = FloatRandom( -0.1f, 0.1f ) ;
				_flocons[i].vy = FloatRandom( VITESSE_Y*0.75f, VITESSE_Y*1.5f) ;
				_flocons[i].vz = FloatRandom( -0.1f, 0.1f ) ;
				
				_flocons[i].ax = FloatRandom( 0, 360 ) ;
				_flocons[i].ay = FloatRandom( 0, 360 ) ;
				_flocons[i].az = FloatRandom( 0, 360 ) ;
				_flocons[i].r = FloatRandom( 0.2f, 0.95f ) ;
			}
		}
		
		private void NouveauFlocon( ref Flocon f )
		{
			if ( f == null )
				f = new Flocon() ;
			
			f.x = FloatRandom( - _tailleCubeX*50, _tailleCubeX*50) ;
			f.z = FloatRandom( - _tailleCubeZ*5, _zCamera ) ;
			f.y = VIEWPORT_Y * 16 ;
			
			f.vx = FloatRandom( -0.1f, 0.1f ) ;
			f.vy = FloatRandom( VITESSE_Y*0.75f, VITESSE_Y*1.5f) ;
			f.vz = FloatRandom( -0.1f, 0.1f ) ;
			
			f.ax = FloatRandom( 0, 360 ) ;
			f.ay = FloatRandom( 0, 360 ) ;
			f.az = FloatRandom( 0, 360 ) ;
			f.r = FloatRandom( 0.1f, 0.95f ) ;
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
			float [] col = { couleur.R/255.0f, couleur.G/255.0f,couleur.B/255.0f, ALPHA_CENTRE } ;
            GLfloat[] fogcolor = { couleur.R / 4096.0f, couleur.G / 4096.0f, couleur.B / 4096.0f, 0.5f};
           
			gl.ClearColor(fogcolor[0], fogcolor[1], fogcolor[2], fogcolor[3] ) ;
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT); 
			
			gl.Disable( OpenGL.GL_TEXTURE_2D ) ;
			gl.Disable(OpenGL.GL_LIGHTING) ;
			gl.Disable(OpenGL.GL_DEPTH ) ;
			
            gl.Enable(OpenGL.GL_FOG) ;
            gl.Fog (OpenGL.GL_FOG_MODE, OpenGL.GL_LINEAR) ;
            gl.Fog(OpenGL.GL_FOG_COLOR, fogcolor) ;
            gl.Fog(OpenGL.GL_FOG_DENSITY, 0.001f) ;
            gl.Fog(OpenGL.GL_FOG_START, _tailleCubeZ*3) ;
            gl.Fog(OpenGL.GL_FOG_END, _tailleCubeZ*50) ;

			gl.LoadIdentity();
			gl.Translate( 0, 0, - _zCamera) ;
			gl.Enable(OpenGL.GL_BLEND);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
			
			gl.Color( col ) ;
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
			foreach (Flocon o in _flocons)
			{
				gl.PushMatrix() ;
				gl.Translate( o.x, o.y, o.z ) ;
				gl.Rotate(o.ax, o.ay, o.az ) ;
				gl.Begin(OpenGL.GL_TRIANGLE_FAN);
				gl.Color( col ) ;
				gl.Vertex(0,0,TAILLE_FLOCON*0.1f);
				gl.Color(col[0],col[1],col[2],ALPHA_BORD) ;
				
				for (int i = 0; i < NB_SOMMETS_DESSIN; i++)
				{
					float taille = (i%2 == 0) ? TAILLE_FLOCON : TAILLE_FLOCON * o.r ;
					gl.Vertex(taille*_coordPoint[i].X, taille*_coordPoint[i].Y, 0);
				}
				gl.Vertex(TAILLE_FLOCON*_coordPoint[0].X,TAILLE_FLOCON* _coordPoint[0].Y, 0);
				
				gl.End();
				gl.PopMatrix() ;
				
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
			RenderStart( CHRONO_TYPE.DEPLACE ) ;
			#endif
			float depuisdebut = (float)(debut.Subtract(maintenant._temps).TotalMilliseconds / 1000.0);
			float vitesseCamera = (float)Math.Sin(depuisdebut / PERIODE_ROTATION) * VITESSE_ROTATION ;
			float vitesseRot = maintenant._intervalle * 100 ;
			
			float CosTheta = (float) Math.Cos(vitesseCamera * maintenant._intervalle) ;
			float SinTheta = (float) Math.Sin(vitesseCamera * maintenant._intervalle) ;
			float px, pz ;
			bool trier = false ;
			// Deplace les flocons
			for (int i = 0; i < NB_FLOCONS; i++)
			{
				if (_flocons[i].y < -VIEWPORT_Y * 16)
				{
					NouveauFlocon( ref _flocons[i] ) ;
					trier = true ;
				}
				else
				{
					// Deplacement
					_flocons[i].x += ((_flocons[i].vx+_xWind) * maintenant._intervalle) ;
					_flocons[i].y -= (_flocons[i].vy * maintenant._intervalle) ;
					_flocons[i].z += (_flocons[i].vz * maintenant._intervalle) ;
					
					// Variation de vitesse
					Varie( ref _flocons[i].vx, -1, 1, 10, maintenant._intervalle ) ;
					Varie( ref _flocons[i].vz, -1, 1, 10, maintenant._intervalle ) ;
					// Rotation due a la position de la camera
					px = (CosTheta * (_flocons[i].x-_xRotation)) - (SinTheta * _flocons[i].z) + _xRotation ;
					pz = (SinTheta * (_flocons[i].x-_xRotation)) + (CosTheta * _flocons[i].z) ;
					
					_flocons[i].x = px ;
					_flocons[i].z = pz ;
					_flocons[i].ax += vitesseRot ;
					_flocons[i].ay += vitesseRot ;
					_flocons[i].az += vitesseRot ;
				}
			}
			
			
			Varie( ref _xWind, -MAX_VENT, MAX_VENT, VITESSE_DELTA_VENT, maintenant._intervalle ) ;
			Varie( ref _xRotation, - _tailleCubeX/2,_tailleCubeX/2, 10, maintenant._intervalle) ;
			
			if (trier)
				Array.Sort(_flocons, delegate(Flocon O1, Flocon O2) {
				           	if  (O1.z > O2.z) return 1 ;
				           	if  (O1.z < O2.z) return -1 ;
				           	return 0 ;
				           });
			#if TRACER
			RenderStop(CHRONO_TYPE.DEPLACE ) ;
			#endif
			
		}
	}
}
