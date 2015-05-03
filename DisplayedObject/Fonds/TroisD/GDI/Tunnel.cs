/*
 * Tunnel infini en 3D
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
	/// Description of Tunnel.
	/// </summary>
    public sealed class TunnelGDI : TroisDGDI
    {
        #region Parametres
        public const string CAT = "Tunnel.GDI";
		public static readonly int TAILLE_ANNEAU = conf.getParametre( CAT, "TailleAnneau",  16) ;
		public static readonly int NB_ANNEAUX = conf.getParametre( CAT, "NbAnneau", 60 ) ;
		public static readonly int RAYON_ANNEAU = conf.getParametre( CAT, "Rayon", 10000 ) ;
		public static readonly float VITESSE_ANNEAU = - conf.getParametre( CAT, "Vitesse", 200000f ) ;
		public static readonly float DECALAGE_MAX = conf.getParametre(CAT, "DecalageMax", 50f ) ;
		private readonly float _periodeRotation = conf.getParametre(CAT, "PeriodeRotation", 10.0f ) ;
		private readonly float _vitesseRotation = conf.getParametre(CAT, "VitesseRotation", 0.2f ) ;
        #endregion
        float _CentreAnneauX ;
		float _CentreAnneauY ;
		
		static private DateTime _DernierDeplacement = DateTime.Now ;
		static DateTime debut = DateTime.Now ;
		Point[] points = new Point[3] ;
		Vecteur3D _directionLumiere = new Vecteur3D( 0, -1, 0 ) ;
		
		Vecteur3D[,] _anneaux ;
		readonly float _zMin, _zMax ; 
		
		public TunnelGDI( int Cx, int Cy )
		{
			points[0] = new Point() ;
			points[1] = new Point() ;
			points[2] = new Point() ;	
			
			_largeur = Cx ;
			_hauteur = Cy ;
			
			_centreX = _largeur/2 ;
			_centreY = _hauteur/2 ;
			
			_tailleCubeX = _largeur /2 ;
			_tailleCubeY = _hauteur /2 ;
			_tailleCubeZ = _largeur   ;
			
			_zEcran	 = -_tailleCubeZ ;
			_zCamera = _zEcran * 20;
			
			_anneaux = new Vecteur3D[NB_ANNEAUX,TAILLE_ANNEAU] ;
			
			_zMax = (-_zCamera) + _tailleCubeZ*5000 ;
			_zMin = _zCamera ;
			_CentreAnneauX = 0 ;
			_CentreAnneauY = 0 ;
			for ( int i = 0; i < NB_ANNEAUX; i++)
				PlaceAnneau( i ) ;
		}
		
		void PlaceAnneau(int i)
		{
			float profondeur = _zMax - _zMin ;
			float ecart = profondeur / NB_ANNEAUX ;
			float z = _zCamera + (i*ecart) ;
			
			for (int j = 0; j < TAILLE_ANNEAU; j++)
			{
				double angle = (Math.PI*2.0*j) / (double)TAILLE_ANNEAU ;
				_anneaux[i,j] = new Vecteur3D( _CentreAnneauX + (float)(RAYON_ANNEAU * Math.Cos(angle)), _CentreAnneauY + (float)(RAYON_ANNEAU * Math.Sin(angle)),z) ;
			}			
		}
		
		public override void AfficheGDI( Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur )
		{
			#if DEBUG
            RenderStart(CHRONO_TYPE.RENDER);
			#endif
			System.Drawing.Drawing2D.SmoothingMode s = g.SmoothingMode ;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            g.Clear(Color.Black);
			CompositingQuality q = g.CompositingQuality ;
			g.CompositingQuality = CompositingQuality.HighSpeed;
			
			for ( int i = (NB_ANNEAUX-2); i >= 0; i--)
			{
				if ( _anneaux[i,0].z > _zCamera)
				{
					int iPlusUn = i + 1 ;
					
					for (int j = 0; j < TAILLE_ANNEAU; j++)
					{
						int jPlusUn = j < (TAILLE_ANNEAU-1) ? j+1 : 0 ;
						Triangle( g, couleur, _anneaux[i,j], _anneaux[i,jPlusUn], _anneaux[iPlusUn,jPlusUn] ) ;
						Triangle( g, couleur, _anneaux[i,j], _anneaux[iPlusUn,jPlusUn], _anneaux[iPlusUn,j] );						
					}
				}
			}
			
			g.SmoothingMode = s ;
			g.CompositingQuality = q ;
			//dessine(g);
			#if DEBUG
            RenderStop(CHRONO_TYPE.RENDER);
			#endif
		}
		
		void Triangle(Graphics g, Color couleur, Vecteur3D v1, Vecteur3D v2, Vecteur3D v3)
		{
			Vecteur3D n = NormaleTriangle( v1, v2, v3 ) ;
			float angle = AngleEntre( n, _directionLumiere ) ;
			float X, Y ;
			
			try
			{
			Coord2DFrom3D( v1, out X, out Y ) ;
			points[0].X = (int)Math.Round(X) ;
			points[0].Y = (int)Math.Round(Y) ;
			
			Coord2DFrom3D( v2, out X, out Y ) ;
			points[1].X = (int)Math.Round(X) ;
			points[1].Y = (int)Math.Round(Y) ;
			
			Coord2DFrom3D( v3, out X, out Y ) ;
			points[2].X = (int)Math.Round(X) ;
			points[2].Y = (int)Math.Round(Y) ;
			
			using ( Brush b = calculeCouleur( couleur, angle, v1.z))
				g.FillPolygon( b, points ) ;
			}
			catch
			{}
		}
		
		int Integer(float x)
		{
			if ( x > int.MaxValue)
				return int.MaxValue ;
			if ( x < int.MinValue)
				return int.MinValue ;
			return (int)x ;
		}
		
		Brush calculeCouleur(Color couleur, float angle, float z)
		{
			double lumiere = ((angle/ (Math.PI*2.0)) * 255) ;
			lumiere *= (_zMax-z) / (_zMax-_zMin) ;
			            
			return new SolidBrush( getCouleurOpaqueAvecAlpha( couleur, (byte)lumiere)) ;
		}
		
		void dessine(Graphics g)
		{
			float Rx = (_largeur/_tailleCubeX) *0.08f ;
			
			float X = _centreX + (0*Rx) ;
			g.DrawLine( Pens.Azure, X, _centreY, X, _centreY+10 ) ;
			g.DrawString( "0", SystemFonts.DefaultFont,Brushes.Teal, X, _centreY + 20f ) ;
			
			
			X = _centreX + (- _tailleCubeZ*Rx) ;
			g.DrawLine( Pens.Azure, X, _centreY, X, _centreY+10 ) ;
			g.DrawString( "-_tailleCubeZ" + (-_tailleCubeZ), SystemFonts.DefaultFont,Brushes.Teal, X, _centreY + 20f ) ;
			
			X = _centreX + (_tailleCubeZ*Rx) ;
			g.DrawLine( Pens.Azure, X, _centreY, X, _centreY+10 ) ;
			g.DrawString( "_tailleCubeZ" + _tailleCubeZ, SystemFonts.DefaultFont,Brushes.Teal, X, _centreY + 20f ) ;
			
			X = _centreX + (_zEcran*Rx) ;
			g.DrawLine( Pens.Azure, X, _centreY, X, _centreY+10 ) ;
			g.DrawString( "_zEcran" + _zEcran, SystemFonts.DefaultFont,Brushes.Teal, X, _centreY + 30f ) ;
			
			X = _centreX + (_zCamera*Rx) ;
			g.DrawLine( Pens.Azure, X, _centreY, X, _centreY+10 ) ;
			g.DrawString( "_zCamera" + _zCamera, SystemFonts.DefaultFont,Brushes.Teal, X, _centreY + 20f ) ;
			
			for ( int i = 0; i < NB_ANNEAUX; i++)
			{
				X = _centreX + (_anneaux[i,0].z *Rx) ;
				g.DrawLine( Pens.BlueViolet, X, _centreY+ 40, X, _centreY+45 ) ;
				g.DrawString( ""+i, SystemFonts.DefaultFont,Brushes.Teal, X, _centreY + 50f ) ;
			}
		}

        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
		{
			float depuisdebut = (float)(debut.Subtract(_DernierDeplacement).TotalMilliseconds / 1000.0);
			float vitesseCamera = (float)Math.Sin(depuisdebut / _periodeRotation) * _vitesseRotation ;
			float CosTheta = (float) Math.Cos(vitesseCamera * maintenant._intervalle) ;
			float SinTheta = (float) Math.Sin(vitesseCamera * maintenant._intervalle) ;
			float px, py ;
			
			_CentreAnneauX = RAYON_ANNEAU * (float)Math.Sin( depuisdebut / 100 ) ;
			
			float dZ = (VITESSE_ANNEAU * maintenant._intervalle) ;
			for ( int i = 0; i < NB_ANNEAUX; i++)
				for (int j = 0; j < TAILLE_ANNEAU; j++)
			{
				_anneaux[i,j].z += dZ ;
				// Rotation due a la position de la camera
				px = (CosTheta * (_anneaux[i,j].x)) - (SinTheta * _anneaux[i,j].y)  ;
				py = (SinTheta * (_anneaux[i,j].x)) + (CosTheta * _anneaux[i,j].y) ;
				
				_anneaux[i,j].x = px ;
				_anneaux[i,j].y = py ;
			}

			if ( _anneaux[2,0].z < _zCamera)
			{
				for ( int i = 0; i < NB_ANNEAUX-1; i++)
					for (int j = 0; j < TAILLE_ANNEAU; j++)
						_anneaux[i,j] = _anneaux[i+1,j] ;

			_CentreAnneauX = RAYON_ANNEAU * (float)Math.Sin( depuisdebut / 3 ) * 0.55f ;
			_CentreAnneauY = RAYON_ANNEAU * (float)Math.Cos( depuisdebut / 2 ) * 0.55f ;
				
				float z = _tailleCubeZ + ((NB_ANNEAUX-1) * (2.0f*_tailleCubeZ/NB_ANNEAUX)) ;
				PlaceAnneau( NB_ANNEAUX-1 ) ;
			}
			
			_DernierDeplacement = maintenant._temps ;
		}
	}
}
