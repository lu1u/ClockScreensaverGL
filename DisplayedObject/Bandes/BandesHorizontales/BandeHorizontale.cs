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

namespace ClockScreenSaverGL.DisplayedObjects.Bandes.BandeHorizontale
{
    /// <summary>
    /// Description of Bande.
    /// </summary>
    public abstract class BandeHorizontale : Bande
    {
        public const string CAT = "BandeHorizontale";
        public static readonly int TailleFonte = conf.getParametre(CAT, "TailleFonte", 30);
        private OpenGLFonte _glFonte;

        public BandeHorizontale(OpenGL gl, int valMax, int intervalle, float largeurcase, float origineX, float Py, int largeur, byte alpha)
            : base(gl, valMax, intervalle, largeurcase, TailleFonte, origineX, largeur, alpha)

        {
            _glFonte = new OpenGLFonte(gl, "0123456789", TailleFonte, FontFamily.GenericSansSerif, FontStyle.Regular);
            _trajectoire = new TrajectoireDiagonale(_origine, Py, 0.0f, conf.getParametre(CAT, "VY", 20f));
            _taillebande = new SizeF(largeur, _hauteurFonte * 2);
        }
        /*
        public override void AfficheGDI(Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleurGlobale)
        {
            SizeF stringSize;
            String texte;
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif

            float decalage, valeur;
            getValue(maintenant, out valeur, out decalage);

            Color couleur = getCouleurAvecAlpha(couleurGlobale, _alpha);
            using (Brush brush = new SolidBrush(couleur))
            using (Pen pen = new Pen(couleur, 4))
            {
                float Decalage = _origine - (decalage * _largeurCase);
                float X = Decalage;
                float Y = _trajectoire._Py;

                int val = (int)valeur;

                g.DrawLine(pen, 0, Y, tailleEcran.Width, Y);
                g.DrawLine(pen, 0, Y + _hauteurFonte * 2, tailleEcran.Width, Y + _hauteurFonte * 2);

                // Reculer jusqu'à la droite de l'écran
                int NbRecul = (int)(X / _largeurCase) + 1;
                X -= (NbRecul * _largeurCase);
                val -= NbRecul;
                while (val < 0)
                    val += (int)_valeurMax;

                // Tracer les graduations
                while (X < (tailleEcran.Width - 1))
                {
                    if (val % _intervalleTexte == 0)
                    {
                        texte = val.ToString();
                        stringSize = g.MeasureString(texte, _fonte);
                        g.DrawString(texte, _fonte, brush, X, Y + _hauteurFonte);
                        g.DrawLine(pen, X, Y, X, Y + _hauteurFonte);
                    }
                    else
                        g.DrawLine(pen, X, Y, X, Y + _hauteurFonte / 2);

                    X += _largeurCase;
                    val = (val + 1);
                    while (val > _valeurMax)
                        val -= _valeurMax;
                }

                // Repere de l'origine
                g.DrawLine(pen, _origine - 4, Y - 4, _origine - 4, Y + _hauteurFonte * 2 + 4);
                g.DrawLine(pen, _origine + 4, Y - 4, _origine + 4, Y + _hauteurFonte * 2 + 4);

#if TRACER
                RenderStop(CHRONO_TYPE.RENDER);
#endif
            }
        }*/


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

            String texte;
            //SizeF stringSize;
            // Tracer les graduations
            while (X < (tailleEcran.Width - 1))
            {
                if (val % _intervalleTexte == 0)
                {
                    texte = val.ToString();
                    //stringSize = g.MeasureString(texte, _fonte);
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
