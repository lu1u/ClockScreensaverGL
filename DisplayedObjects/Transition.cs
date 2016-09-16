using ClockScreenSaverGL.Config;
using SharpGL;
using System;
using System.Drawing;

namespace ClockScreenSaverGL.DisplayedObjects
{
    public class Transition : DisplayedObject
    {
        public const string CAT = "Transition";
        static protected CategorieConfiguration c = Config.Configuration.getCategorie(CAT);
        public readonly float DureeTransition = c.getParametre("Duree transition", 2000) / 1000.0f; // En millisecondes
        public readonly int LARGEUR_TEXTURE = c.getParametre("Largeur texture", 512);
        public readonly int HAUTEUR_TEXTURE = c.getParametre("Hauteur texture", 512);
        private DisplayedObject _objetTransition;
        private float _etapeTransition = 0;
        private bool _transitionEnCours = false;
        protected uint _texture = 0;
        private enum TYPE_TRANSITION { ALPHA = 0, ROTATE = 1, SCALE = 2 };
        TYPE_TRANSITION _typeDeTransition;

        /// <summary>
        /// Constructeur, preparer la texture dans laquelle on va rendre le fond
        /// </summary>
        /// <param name="gl"></param>
        public Transition(OpenGL gl) : base(gl)
        {
            _texture = createEmptyTexture(LARGEUR_TEXTURE, HAUTEUR_TEXTURE);
        }

        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
        public override void Dispose()
        {
            base.Dispose();
            deleteEmptyTexture(_texture);
            _objetTransition?.Dispose();
            _objetTransition = null;
        }
        /// <summary>
        /// Initialisation d'une nouvelle transition, prendre une copie du fond d'ecran
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="objet"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleur"></param>
        public void InitTransition(OpenGL gl, DisplayedObject objet, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
            _objetTransition = objet;
            _etapeTransition = 0;
            RenderToTexture(gl, maintenant, tailleEcran, couleur);
            _transitionEnCours = true;
            _typeDeTransition = (TYPE_TRANSITION)r.Next(3);
        }

        /// <summary>
        /// Rendre le fond d'ecran dans une image memoire qu'on utilisera comme texture
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleur"></param>
        public void RenderToTexture(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
            Rectangle r = new Rectangle(0, 0, LARGEUR_TEXTURE, HAUTEUR_TEXTURE);
            gl.PushAttrib(OpenGL.GL_ENABLE_BIT);
            gl.PushMatrix();
            gl.Viewport(0, 0, r.Width, r.Height);                    // Set Our Viewport (Match Texture Size)

            _objetTransition.ClearBackGround(gl, couleur);
            _objetTransition.AfficheOpenGL(gl, maintenant, r, couleur);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, _texture);

            gl.CopyTexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB16, 0, 0, LARGEUR_TEXTURE, HAUTEUR_TEXTURE, 0);
            gl.PopMatrix();
            gl.PopAttrib();

            gl.Viewport(0, 0, tailleEcran.Width, tailleEcran.Height);
        }

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            if (_transitionEnCours)
            {
                gl.PushMatrix();
                gl.MatrixMode(OpenGL.GL_PROJECTION);
                gl.PushMatrix();
                gl.LoadIdentity();
                gl.Ortho2D(-1, 1, -1, 1);
                gl.MatrixMode(OpenGL.GL_MODELVIEW);

                gl.Disable(OpenGL.GL_LIGHTING);
                gl.Disable(OpenGL.GL_DEPTH);
                gl.Enable(OpenGL.GL_TEXTURE_2D);
                gl.Enable(OpenGL.GL_BLEND);
                gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, _texture);

                float echelle = 1.0f - (_etapeTransition / DureeTransition);

                switch (_typeDeTransition)
                {
                    case TYPE_TRANSITION.ALPHA:
                        gl.Color(1.0f, 1, 1, echelle);
                        break;

                    case TYPE_TRANSITION.ROTATE:
                        gl.Color(1.0f, 1, 1, echelle);
                        gl.Rotate(0, 0, 180 * (_etapeTransition / DureeTransition));
                        break;

                    case TYPE_TRANSITION.SCALE:
                        gl.Color(1.0f, 1, 1, echelle);
                        gl.Scale(echelle, echelle, 1);
                        gl.Translate((_etapeTransition / DureeTransition), (_etapeTransition / DureeTransition), 0);
                        break;
                }
                gl.Begin(OpenGL.GL_QUADS);
                // Front Face
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(-1.0f, 1.0f);
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(-1.0f, -1.0f);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(1.0f, -1.0f);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(1.0f, 1.0f);
                gl.End();

                gl.MatrixMode(OpenGL.GL_PROJECTION);
                gl.PopMatrix();
                gl.MatrixMode(OpenGL.GL_MODELVIEW);
                gl.PopMatrix();
                gl.Enable(OpenGL.GL_DEPTH);
                gl.Enable(OpenGL.GL_DEPTH_TEST);
                gl.Enable(OpenGL.GL_TEXTURE_2D);
            }
            //Console.getInstance(gl).AddLigne(Color.Yellow, "Transition " + _etapeTransition + "/" + DureeTransition + "," + echelle);
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
            _etapeTransition += maintenant._intervalle;
            if (_etapeTransition > DureeTransition)
            {
                _transitionEnCours = false;
                _objetTransition?.Dispose();
                _objetTransition = null;
            }
        }
    }
}
