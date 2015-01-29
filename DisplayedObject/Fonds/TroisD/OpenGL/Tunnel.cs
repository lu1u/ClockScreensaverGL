/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 18/01/2015
 * Heure: 16:41
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using SharpGL ;
using SharpGL.SceneGraph ;
using GLbitfield = System.UInt32;
using GLboolean = System.Boolean;
using GLbyte = System.SByte;
using GLclampf = System.Single;
using GLdouble = System.Double;
using GLenum = System.UInt32;
using GLfloat = System.Single;
using GLint = System.Int32;
using GLshort = System.Int16;
using GLsizei = System.Int32;
using GLubyte = System.Byte;
using GLuint = System.UInt32;
using GLushort = System.UInt16;
using GLvoid = System.IntPtr;


namespace ClockScreenSaverGL.Fonds.TroisD
{
	/// <summary>
	/// Description of Tunnel.
	/// </summary>
	public class Tunnel : TroisD
	{
        protected const string CAT = "Tunnel.OpenGL";
		protected static readonly int TAILLE_ANNEAU = conf.getParametre(CAT, "NbFacettes", 16 ) ;
		protected static readonly int NB_ANNEAUX = conf.getParametre(CAT, "Nombre", 500 ) ;
		protected static readonly float VITESSE_ANNEAU = conf.getParametre(CAT, "Vitesse", 2f ) ;
		protected static readonly float DECALAGE_MAX = conf.getParametre(CAT, "DecalageMax", 5f ) ;
		protected static readonly float PERIODE_ROTATION = conf.getParametre(CAT, "PeriodeRotation", 10.0f ) ;
		protected static readonly float VITESSE_ROTATION = 0.2f;//conf.getParametre(CAT, "VitesseRotation", 100.0f ) ;
		protected static readonly float BANDES_PLEINES = conf.getParametre(CAT, "CouleursPleines", TAILLE_ANNEAU + 1 ) ;
		protected static readonly float RATIO_DEPLACEMENT = conf.getParametre(CAT, "DeplacementTunnel", 0.5f ) ;
		protected static readonly float RAYON_ANNEAU = RATIO_DEPLACEMENT * 10f ;
        protected static readonly GLfloat PERIODE_DEP_X = 5;//conf.getParametre(CAT, "PeriodeDEcalageX", 1.3f);
        protected static readonly GLfloat PERIODE_DEP_Y = 7f;//conf.getParametre(CAT, "PeriodeDEcalageY", 1.7f);
		
		float _CentreAnneauX ;
		float _CentreAnneauY ;
		
		static protected DateTime _DernierDeplacement = DateTime.Now ;
		static DateTime debut = DateTime.Now ;
		
		Vecteur3D[,] _anneaux ;
		readonly float _zMax ;
		const float VIEWPORT_X = 2f ;
        const float VIEWPORT_Y = 2f;
        const float VIEWPORT_Z = 2f;
        GLfloat[] LightPos = { 0, RAYON_ANNEAU * 0.75f, 0.5f, 2 };
		//GLfloat[] 	global_ambient = { 0.01f, 0.01f, 0.01f, 0.01f };
        public Tunnel()
            : base(VIEWPORT_X, VIEWPORT_Y, VIEWPORT_Z, VIEWPORT_Y / 2)
		{
			_anneaux = new Vecteur3D[NB_ANNEAUX,TAILLE_ANNEAU] ;
			
			_zMax = - _tailleCubeZ ;
			_CentreAnneauX = 0 ;
			_CentreAnneauY = 0 ;
			for ( int i = 0; i < NB_ANNEAUX; i++)
				PlaceAnneau( i ) ;
		}
		
		void PlaceAnneau(int i)
		{
			float profondeur = _tailleCubeZ * 50f ;
			float ecart = profondeur / NB_ANNEAUX ;
			float z = _tailleCubeZ - (i*ecart) ;
			
			for (int j = 0; j < TAILLE_ANNEAU; j++)
			{
				double angle = (Math.PI*2.0*j) / (double)TAILLE_ANNEAU ;
				_anneaux[i,j] = new Vecteur3D( 	_CentreAnneauX + (float)(RAYON_ANNEAU * Math.Cos(angle)),
				                              _CentreAnneauY + (float)(RAYON_ANNEAU * Math.Sin(angle)),
				                              z) ;
			}
			
			
		}
		
		public override void AfficheOpenGL( OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur )
		{
			#if TRACER
			RenderStart(CHRONO_TYPE.RENDER) ;
			#endif
			float depuisdebut = (float)(debut.Subtract(_DernierDeplacement).TotalMilliseconds / 1000.0);
			float rotation = (float)Math.Cos(depuisdebut / PERIODE_ROTATION) * VITESSE_ROTATION ;
			
			gl.ClearColor(0, 0, 0, 1) ;
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT); 
			gl.LoadIdentity();
			gl.Translate( 0,0, - _zCamera) ;
			
			gl.Enable(OpenGL.GL_LIGHTING); 	// Active l'éclairage
			gl.Enable(OpenGL.GL_LIGHT0); 	// Allume la lumière n°1
			gl.Light( OpenGL.GL_LIGHT0,OpenGL.GL_POSITION,LightPos);
			//gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
			gl.Enable( OpenGL.GL_COLOR_MATERIAL ) ;
			
			//gl.Rotate(0,0,rotation ) ;
			float [] col = { couleur.R/512.0f, couleur.G/512.0f,couleur.B/512.0f, 1.0f } ;
			gl.Color( col ) ;
			
			for ( int i = 0; i < NB_ANNEAUX-1; i++)
			{
				gl.Begin( OpenGL.GL_QUADS) ;
				{
					int iPlusUn = i < (NB_ANNEAUX-1) ? i+1 : 0 ;
					
					for (int j = 0; j < TAILLE_ANNEAU; j++)
					{
						if  ((j+1) % BANDES_PLEINES != 0)
						{
							int jPlusUn = j < (TAILLE_ANNEAU-1) ? j+1 : 0 ;
							
							Vecteur3D n = NormaleTriangle( _anneaux[iPlusUn,j], _anneaux[i,j], _anneaux[i,jPlusUn] ) ;
							gl.Normal( n.x, n.y, n.z ) ;
							gl.Vertex( _anneaux[i,j].x, _anneaux[i,j].y, _anneaux[i,j].z ) ;
							gl.Vertex( _anneaux[i,jPlusUn].x, _anneaux[i,jPlusUn].y, _anneaux[i,jPlusUn].z ) ;
							gl.Vertex( _anneaux[iPlusUn,jPlusUn].x, _anneaux[iPlusUn,jPlusUn].y, _anneaux[iPlusUn,jPlusUn].z ) ;
							gl.Vertex( _anneaux[iPlusUn,j].x, _anneaux[iPlusUn,j].y, _anneaux[iPlusUn,j].z ) ;
						}
						
					}
				}
				
				gl.End() ;
			}

			#if TRACER
			RenderStop(CHRONO_TYPE.RENDER) ;
			#endif
		}
		
	public override void Deplace( Temps maintenant, Rectangle tailleEcran )
		{
			#if TRACER
			RenderStart(CHRONO_TYPE.DEPLACE) ;
			#endif
            float depuisdebut = (float)(debut.Subtract(maintenant._temps).TotalMilliseconds / 1000.0);
            float vitesseCamera = (float)Math.Sin(depuisdebut / PERIODE_ROTATION) * VITESSE_ROTATION;
            float vitesseRot = maintenant._intervalle * 100;

            float CosTheta = (float)Math.Cos(vitesseCamera * maintenant._intervalle);
            float SinTheta = (float)Math.Sin(vitesseCamera * maintenant._intervalle);
            float px, py;
			
			float dZ = (VITESSE_ANNEAU * maintenant._intervalle) ;
			
			for ( int i = 0; i < NB_ANNEAUX; i++)
				for (int j = 0; j < TAILLE_ANNEAU; j++)
			{
				_anneaux[i,j].z += dZ ;

                    // Tourner autour de l'axe Z
                px = (CosTheta * (_anneaux[i, j].x)) - (SinTheta * _anneaux[i, j].y);
                py = (SinTheta * (_anneaux[i, j].x)) + (CosTheta * _anneaux[i, j].y);

                _anneaux[i, j].x = px;
                _anneaux[i, j].y = py;
			}

			if ( _anneaux[2,0].z > _tailleCubeZ)
			{
				for ( int i = 0; i < NB_ANNEAUX-1; i++)
					for (int j = 0; j < TAILLE_ANNEAU; j++)
						_anneaux[i,j] = _anneaux[i+1,j] ;

				_CentreAnneauX = (RAYON_ANNEAU * RATIO_DEPLACEMENT)  * (float)Math.Sin( depuisdebut / PERIODE_DEP_X) ;
				_CentreAnneauY = (RAYON_ANNEAU * RATIO_DEPLACEMENT)  * (float)Math.Cos( depuisdebut / PERIODE_DEP_Y );
				
				PlaceAnneau( NB_ANNEAUX-1 ) ;
			}
			
			_DernierDeplacement = maintenant._temps ;
			#if TRACER
			RenderStop(CHRONO_TYPE.DEPLACE) ;
			#endif
			
		}
	}
}
