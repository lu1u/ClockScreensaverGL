/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 22/06/2014
 * Time: 16:11
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using System;
using System.Drawing;
using SharpGL;
using ClockScreenSaverGL.Config;

namespace ClockScreenSaverGL.DisplayedObjects.Bandes.BandeHorizontale
{
    /// <summary>
    /// Description of Bande.
    /// </summary>
    public abstract class BandeHorizontale : Bande
    {
        public const string CAT = "BandeHorizontale";
        static protected CategorieConfiguration c = Config.Configuration.getCategorie(CAT);
        public static readonly int TailleFonte = c.getParametre( "TailleFonte", 30);
        private OpenGLFonte _glFonte;

        protected BandeHorizontale(OpenGL gl, int valMax, int intervalle, float largeurcase, float origineX, float Py, int largeur, byte alpha)
            : base(gl, valMax, intervalle, largeurcase, TailleFonte, origineX, largeur, alpha)

        {
            _glFonte = new OpenGLFonte(gl, "0123456789", TailleFonte, FontFamily.GenericSansSerif, FontStyle.Regular);
            _trajectoire = new TrajectoireDiagonale(_origine, Py, 0.0f, c.getParametre( "VY", 20f ) );
            _taillebande = new SizeF(largeur, _hauteurFonte * 2);
        }

        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
            float decalage, valeur;
            getValue(maintenant, out valeur, out decalage);


            float Decalage = _origine - (decalage * _largeurCase);
            float X = Decalage;
            float Y = _trajectoire._Py;

            int val = (int)valeur;
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(0, Y);
            gl.Vertex(tailleEcran.Width, Y);
            gl.Vertex(0, Y + _hauteurFonte * 2);
            gl.Vertex(tailleEcran.Width, Y + _hauteurFonte * 2);
            gl.End();

            // Reculer jusqu'à la droite de l'écran
            int NbRecul = (int)(X / _largeurCase) + 1;
            X -= (NbRecul * _largeurCase);
            val -= NbRecul;
            while (val < 0)
                val += (int)_valeurMax;

            string texte;
            // Tracer les graduations
            while (X < (tailleEcran.Width))
            {
                if (val % _intervalleTexte == 0)
                {
                    texte = val.ToString();
                    _glFonte.drawOpenGL(gl, val.ToString(), X, Y, couleur);
                    gl.Begin(OpenGL.GL_LINES);
                    gl.Vertex(X, Y);
                    gl.Vertex(X, Y + _hauteurFonte);
                    gl.End();
                }
                else
                {
                    gl.Begin(OpenGL.GL_LINES);
                    gl.Vertex(X, Y);
                    gl.Vertex( X, Y + _hauteurFonte / 2);
                    gl.End();
                }

                X += _largeurCase;
                val = (val + 1);

                while (val > _valeurMax)
                    val -= _valeurMax;
            }

            // Repere de l'origine
            gl.Vertex(_origine , Y - 4);
            gl.Vertex(_origine , Y + _hauteurFonte * 2 + 4);
            gl.End();
#if TRACER
                RenderStop(CHRONO_TYPE.RENDER);
#endif
        }
    }
}
