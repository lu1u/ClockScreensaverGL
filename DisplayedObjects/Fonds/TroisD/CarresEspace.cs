using ClockScreenSaverGL.Config;
using SharpGL;
using System;
using System.Drawing;
using GLfloat = System.Single;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD
{
    /// <summary>
    /// Description of Neige.
    /// </summary>
    public class CarresEspace : TroisD
    {
        #region Parametres

        public const string CAT = "CarresEspace";
        static private CategorieConfiguration c = Config.Configuration.getCategorie(CAT);
        private static readonly int NB_PAVES = c.getParametre("Nb", 200);
        private static byte ALPHA = c.getParametre("Alpha", (byte)250, (a) => { ALPHA = Convert.ToByte(a); } );
        private static float TAILLE_CARRE = c.getParametre("Taille", 5.0f, (a) => { TAILLE_CARRE = (float)Convert.ToDouble(a); } );
        private static float PERIODE_TRANSLATION = c.getParametre("PeriodeTranslation", 13.0f, (a) => { PERIODE_TRANSLATION = (float)Convert.ToDouble(a); } );
        private static float PERIODE_ROTATION = c.getParametre("PeriodeRotation", 10.0f, (a) => { PERIODE_ROTATION = (float)Convert.ToDouble(a); } );
        private static float VITESSE_ROTATION = c.getParametre("VitesseRotation", 50f, (a) => { VITESSE_ROTATION = (float)Convert.ToDouble(a); } );
        private static float VITESSE = c.getParametre("Vitesse", 8f, (a) => { VITESSE = (float)Convert.ToDouble(a); } );

        #endregion Parametres

        private const int VIEWPORT_X = 60;
        private const int VIEWPORT_Y = 60;
        private const float VIEWPORT_Z = 20.0f;
        private static readonly GLfloat[] fogcolor = { 0, 0, 0, 1 };

        private readonly Vecteur3D[] _Carres;                       // Coordonnees du centre des carres
        private DateTime _dernierDeplacement = DateTime.Now;
        private DateTime _debutAnimation = DateTime.Now;

        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="gl"></param>
        public CarresEspace( OpenGL gl )
            : base( gl, VIEWPORT_X, VIEWPORT_Y, VIEWPORT_Z, 100 )
        {
            _Carres = new Vecteur3D[NB_PAVES];

            // Initialiser les carres
            for ( int i = 0; i < NB_PAVES; i++ )
                NouveauCarre( ref _Carres[i] );

            TrierTableau();
        }

        /// <summary>
        /// Creation d'un carre tout au fond
        /// </summary>
        /// <param name="f"></param>
        private void NouveauCarre( ref Vecteur3D f )
        {
            if ( f == null )
            {
                f = new Vecteur3D();
                f.z = -VIEWPORT_Z + TAILLE_CARRE * r.Next( 0, (int) (_zCamera + VIEWPORT_Z) / (int) TAILLE_CARRE );
            }
            else
                while ( f.z > -VIEWPORT_Z )
                    f.z -= VIEWPORT_Z;

            //f.aSupprimer = false;
            f.x = GetXCoord();
            f.y = GetYCoord();
        }

        private int GetYCoord()
        {
            int c;
            do
            {
                c = (int) r.Next( -VIEWPORT_Y / 5, VIEWPORT_Y / 5 ) * 5;
            }
            while ( c == 0 );
            return c;
        }

        private int GetXCoord()
        {
            int c = (int)r.Next(-VIEWPORT_X, VIEWPORT_X);
            return (int) ((int) (c / TAILLE_CARRE) * TAILLE_CARRE);
        }

        /// <summary>
        /// Efface le fond d'ecran
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="c"></param>
        public override void ClearBackGround( OpenGL gl, Color c )
        {
            gl.ClearColor( 0, 0, 0, 1 );
            gl.Clear( OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT );
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
            RenderStart( CHRONO_TYPE.RENDER );
#endif
            float depuisdebut = (float)(_debutAnimation.Subtract(_dernierDeplacement).TotalMilliseconds / 1000.0);
            float vitesseCamera = (float)Math.Sin(depuisdebut / PERIODE_ROTATION) * VITESSE_ROTATION;

            gl.LoadIdentity();
            gl.Disable( OpenGL.GL_LIGHTING );
            gl.Enable( OpenGL.GL_DEPTH );
            gl.Disable( OpenGL.GL_BLEND );
            gl.Disable( OpenGL.GL_TEXTURE_2D );

            gl.Enable( OpenGL.GL_FOG );
            gl.Fog( OpenGL.GL_FOG_MODE, OpenGL.GL_LINEAR );
            gl.Fog( OpenGL.GL_FOG_COLOR, fogcolor );
            gl.Fog( OpenGL.GL_FOG_DENSITY, 0.2f );
            gl.Fog( OpenGL.GL_FOG_START, VIEWPORT_Z );
            gl.Fog( OpenGL.GL_FOG_END, _zCamera );

            changeZoom( gl, tailleEcran.Width, tailleEcran.Height, 0.001f, VIEWPORT_Z * 10 );

            gl.Translate( 0, 0, -_zCamera );
            gl.Rotate( 0, 0, vitesseCamera + 90 );
            couleur = getCouleurOpaqueAvecAlpha( couleur, ALPHA );
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1 };
            gl.Color( col );
            gl.Begin( OpenGL.GL_QUADS );
            foreach ( Vecteur3D o in _Carres )
            {
                gl.Vertex( o.x, o.y, o.z );
                gl.Vertex( o.x, o.y, o.z + TAILLE_CARRE );
                gl.Vertex( o.x + TAILLE_CARRE, o.y, o.z + TAILLE_CARRE );
                gl.Vertex( o.x + TAILLE_CARRE, o.y, o.z );
            }
            gl.End();
#if TRACER
            RenderStop( CHRONO_TYPE.RENDER );
#endif
        }

        /// <summary>
        /// Deplacement de tous les objets: carres, camera...
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace( Temps maintenant, Rectangle tailleEcran )
        {
#if TRACER
            RenderStart( CHRONO_TYPE.DEPLACE );
#endif
            float depuisdebut = (float)(_debutAnimation.Subtract(_dernierDeplacement).TotalMilliseconds / 1000.0);
            float deltaZ = VITESSE * maintenant.intervalleDepuisDerniereFrame;

            // Deplace les carres
            bool trier = false;
            for ( int i = 0; i < NB_PAVES; i++ )
            {
                if ( _Carres[i].z > (_zCamera + TAILLE_CARRE) )
                {
                    // Nouveau carre tout au fond
                    NouveauCarre( ref _Carres[i] );
                    trier = true;               // Il faudra trier le tableau
                }
                else
                {
                    _Carres[i].z += deltaZ;
                }
            }

            if ( trier )
                TrierTableau();

            _dernierDeplacement = maintenant.temps;
#if TRACER
            RenderStop( CHRONO_TYPE.DEPLACE );
#endif
        }

        private void TrierTableau()
        {
            Array.Sort( _Carres, delegate ( Vecteur3D O1, Vecteur3D O2 )
             {
                 if ( DistanceCarre( O1 ) > DistanceCarre( O2 ) ) return -1;
                 if ( DistanceCarre( O1 ) < DistanceCarre( O2 ) ) return 1;
                 return 0;
             } );
        }

        /// <summary>
        /// Calcule la distance au carre du point à la camera
        /// on n'a pas besoin de la racine carre, donc on ne perd pas de temps
        /// </summary>
        /// <param name="C"></param>
        /// <returns></returns>
        private double DistanceCarre( Vecteur3D C )
        {
            return (C.x * C.x) + (C.y * C.y) + ((C.z - _zCamera) * (C.z - _zCamera));
        }

#if TRACER

        public override String DumpRender()
        {
            return base.DumpRender() + " NbCarres:" + NB_PAVES;
        }

#endif
    }
}
