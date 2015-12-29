/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 22/06/2014
 * Time: 20:15
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using System;
using System.Drawing;
using SharpGL;

namespace ClockScreenSaverGL.DisplayedObjects.Bandes.BandeVerticale
{
    /// <summary>
    /// Description of BandeVerticale.
    /// </summary>
    public abstract class BandeVerticale : Bande
    {
        public const string CAT = "BandeVerticale";
        public static int TailleFonte = conf.getParametre(CAT, "TailleFonte", 30);
        private OpenGLFonte _glFonte;
        

        public BandeVerticale(OpenGL gl, int valMax, int intervalle, float largeurcase, float origineY, float Px, int largeur, byte alpha)
            : base(gl, valMax, intervalle, largeurcase, TailleFonte, origineY, largeur, alpha)
        {
            _glFonte = new OpenGLFonte(gl, "0123456789", TailleFonte, FontFamily.GenericSansSerif, FontStyle.Regular);
            _trajectoire = new TrajectoireDiagonale(Px, _origine, conf.getParametre(CAT, "VY", 20f), 0);
            _taillebande = new SizeF(_hauteurFonte * 2, largeur);
        }



       

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
            float decalage, valeur;
            getValue(maintenant, out valeur, out decalage);

            
            //using (Brush brush = new SolidBrush(couleur))
           // using (Pen pen = new Pen(couleur, 4))
            {
                float Decalage = _origine - (decalage * _largeurCase);
                float Y = Decalage;
                float X = _trajectoire._Px;

                int val = (int)valeur;

                // Reculer jusqu'à la droite de l'écran
                while (Y > 0)
                {
                    Y -= (float)_largeurCase;
                    val--;
                }

                // Revenir jusqu'a la gauche de l'ecran
                while (val < 0)
                    val += (int)_valeurMax;
                
                // Trace les chiffres et marques
                while (Y < tailleEcran.Height)
                {
                    gl.Begin(OpenGL.GL_LINES);
                    gl.Vertex(X, Y);
                    gl.Vertex(val % _intervalleTexte == 0 ? X + _hauteurFonte : X + _hauteurFonte / 2.0f, Y);
                    gl.End();

                    if (val % _intervalleTexte == 0)
                        _glFonte.drawOpenGL( gl, val.ToString(), X, Y, couleur );
                    Y += _largeurCase;
                    val = (val + 1);
                    while (val >= _valeurMax)
                        val -= _valeurMax;
                }


                gl.Begin(OpenGL.GL_LINES);
                // Deux lignes verticales pour les bords de la bande
                gl.Vertex(X, 0);
                gl.Vertex(X, tailleEcran.Height);

                gl.Vertex(X + _hauteurFonte*2.0f, 0);
                gl.Vertex(X + _hauteurFonte*2.0f, tailleEcran.Height);


                // Repere pour la valeur
                gl.Vertex(X-4, _origine );
                gl.Vertex(X + 4 + _hauteurFonte * 2, _origine );

                gl.End();
            }
        }

    }
}