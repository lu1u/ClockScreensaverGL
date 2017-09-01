using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Globalization;
using ClockScreenSaverGL.Config;

namespace ClockScreenSaverGL.DisplayedObjects.PanneauActualites
{
    class Actualites : DisplayedObject, IDisposable
    {
        #region Parametres
        public const string CAT = "Actualites";
        static protected CategorieConfiguration c = Config.Configuration.getCategorie(CAT);
        public static readonly int NB_JOURS_MAX_INFO = c.getParametre("Nb jours info max", 4);
        public static readonly int HAUTEUR_BANDEAU = c.getParametre("Hauteur bandeau", 150);
        public static readonly float VITESSE = c.getParametre("Vitesse", 75.0f);
        public static readonly int MIN_LIGNES = c.getParametre("Nb lignes min", 50);
        public static readonly int MAX_LIGNES = c.getParametre("Nb lignes max", 100);
        public static readonly int MAX_LIGNES_PAR_SOURCE = c.getParametre("Nb lignes max par source", 10);
        public static readonly int TAILLE_SOURCE = c.getParametre("Taille fonte source", 16);
        public static readonly int TAILLE_TITRE = c.getParametre("Taille fonte titre", 30);
        public static readonly int TAILLE_DESCRIPTION = c.getParametre("Taille fonte description", 14);
        public static bool AFFICHE_DESCRIPTION = c.getParametre("Affiche Description", true);
        public static bool AFFICHE_IMAGES = c.getParametre("Affiche Images", true);
        public static float SATURATION_IMAGES = c.getParametre("Saturation images", 0.5f);
        #endregion
        private int derniereSource = c.getParametre("Derniere Source", 0);

        private float _decalageX = SystemInformation.VirtualScreen.Width;
        public static int _derniereAffichee;

        private ActuFactory _actuFactory;

        public Actualites( OpenGL gl ) : base( gl )
        {
            _actuFactory = new ActuFactory();
        }

        public override void Dispose()
        {
            base.Dispose();
            _actuFactory.Dispose();
        }
        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
        /// <summary>
        /// Affichage deroulant des actualites
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleur"></param>
        public override void AfficheOpenGL( OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur )
        {
#if TRACER
            RenderStart( CHRONO_TYPE.RENDER );
#endif
            gl.PushMatrix();
            gl.MatrixMode( OpenGL.GL_PROJECTION );
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D( 0, tailleEcran.Width, 0, tailleEcran.Height );
            gl.MatrixMode( OpenGL.GL_MODELVIEW );

            gl.Disable( OpenGL.GL_LIGHTING );
            gl.Disable( OpenGL.GL_DEPTH );
            gl.Enable( OpenGL.GL_BLEND );
            gl.BlendFunc( OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA );

            gl.Disable( OpenGL.GL_TEXTURE_2D );
            gl.Color( 0.1f, 0.1f, 0.1f, 0.55f ); // Fond sombre
            gl.Rect( tailleEcran.Left, tailleEcran.Top + HAUTEUR_BANDEAU, tailleEcran.Right, tailleEcran.Top );

            float x = tailleEcran.Left + _decalageX;
            _derniereAffichee = 0;

            #region LignesActu
            gl.Enable( OpenGL.GL_TEXTURE_2D );
            gl.Color( couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1.0f );

            if ( _actuFactory._lignes != null )
                try
                {
                    lock ( _actuFactory._lignes ) foreach ( LigneActu l in _actuFactory._lignes )
                        {
                            l.affiche( gl, x, tailleEcran.Top + HAUTEUR_BANDEAU, AFFICHE_DESCRIPTION );
                            x += l.largeur;
                            _derniereAffichee++;
                            if ( x > tailleEcran.Right )
                                break;
                        }
                }
                catch ( Exception )
                {
                }


            #endregion

            gl.MatrixMode( OpenGL.GL_PROJECTION );
            gl.PopMatrix();
            gl.MatrixMode( OpenGL.GL_MODELVIEW );
            gl.PopMatrix();

#if TRACER
            RenderStop( CHRONO_TYPE.RENDER );
#endif
        }

        public override void Deplace( Temps maintenant, Rectangle tailleEcran )
        {
            _decalageX -= VITESSE * maintenant.intervalleDepuisDerniereFrame;

            if ( _actuFactory._lignes != null )
                lock ( _actuFactory._lignes )
                {
                    if ( _actuFactory._lignes.Count > 1 )
                        if ( _decalageX + _actuFactory._lignes[0].largeur < 0 )
                        {
                            // Suppression de la premiere annonce qui ne sera plus affichee
                            _decalageX += _actuFactory._lignes[0].largeur;
                            LigneActu l0 = _actuFactory._lignes[0];

                            //_actuFactory._lignes[0].Dispose();
                            _actuFactory._lignes.RemoveAt( 0 );
                            _actuFactory._lignes.Add( l0 );
                        }
                }
        }

        public static int SourceCourante()
        {
            return c.getParametre( "Derniere Source", 0 );
        }
        public static void SourceCourante( int source )
        {
            c.setParametre( "Derniere Source", source );
            c.flush();
        }




        public override bool KeyDown( Form f, Keys k )
        {
            if ( Keys.J.Equals( k ) )
            {
                if ( _actuFactory._lignes?.Count >= 1 )
                    lock ( _actuFactory._lignes )
                        _actuFactory._lignes.RemoveAt( 0 );
                return true;
            }
            if ( Keys.E.Equals( k ) )
            {
                lock ( _actuFactory._lignes )
                    _actuFactory._lignes?.Clear();
                AFFICHE_DESCRIPTION = !AFFICHE_DESCRIPTION;
                c.setParametre( "Affiche Description", AFFICHE_DESCRIPTION );
                c.flush();
                return true;
            }
            if ( Keys.I.Equals( k ) )
            {
                lock ( _actuFactory._lignes )
                    _actuFactory._lignes?.Clear();
                AFFICHE_IMAGES = !AFFICHE_IMAGES;
                c.setParametre( "Affiche Images", AFFICHE_DESCRIPTION );
                c.flush();
                return true;
            }

            return base.KeyDown( f, k );
        }
    }
}
