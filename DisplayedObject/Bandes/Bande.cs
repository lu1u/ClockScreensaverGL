/*
 * Bande:
 * classe de base pour les objets qui affichent heure/minutes/secondes, verticalement ou horizontalement
 */
using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Drawing;
namespace ClockScreenSaverGL
{
    /// <summary>
    /// Description of Bande.
    /// </summary>
    public abstract class Bande : DisplayedObject, IDisposable
    {
        protected int _intervalleTexte;
        protected float _largeurCase;
        protected int _hauteurFonte;
        protected int _valeurMax;
        //protected Font _fonte;
        protected float _origine;
        protected Trajectoire _trajectoire;
        protected SizeF _taillebande;
        protected byte _alpha;

        protected Texture _texture ;
        protected int _largeurBande;
        protected int _hauteurBande;

        /// <summary>
        /// Retourne la valeur a afficher, avec un decalage partiel (ex: decalage partiel par seconde pour afficher
        /// les minutes
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="value"></param>
        /// <param name="decalage"></param>
        protected abstract void getValue(Temps maintenant, out float value, out float decalage);

        public Bande(OpenGL gl,  int valMax, int intervalle, float largeurcase, int hauteurfonte, float origineX, int largeur, byte alpha) :
            base()
        {
            _valeurMax = valMax;
            _largeurCase = largeurcase;
            _hauteurFonte = hauteurfonte;
            _origine = origineX;
            _intervalleTexte = intervalle;
            _alpha = alpha;
        }

        protected abstract void CreerTexture(OpenGL gl, int Min, int Max, int Pas );

        /// <summary>
        /// Implementation de la fonction virtuelle Deplace: deplacement de l'objet
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif

            _trajectoire.Avance(tailleEcran, _taillebande, maintenant);
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif

        }

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            if (_texture == null)
                return;

            gl.Clear(OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D(0.0, tailleEcran.Width, 0.0, tailleEcran.Height);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);


            float[] col = { couleur.R / 255.0f, couleur.G / 255.0f, couleur.B / 255.0f, _alpha / 255.0f };
            gl.Color(col);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            _texture.Bind(gl);

            gl.Translate(_trajectoire._Px, _trajectoire._Py, 0);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(0, _hauteurBande);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(0, 0);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(_largeurBande, 0);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(_largeurBande, _hauteurBande);
            gl.End();

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();

#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif

        }
        public void Dispose()
        {
            Dispose();
            //_fonte?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
