/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 22/12/2014
 * Heure: 15:06
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using ClockScreenSaverGL.Config;
using SharpGL;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds
{
    /// <summary>
    /// Description of Class1.
    /// </summary>
    public abstract class Fond : DisplayedObject
    {

        public Fond(OpenGL gl) : base(gl)
        {

        }

        public override void ClearBackGround(OpenGL gl, Color c)
        {
            gl.ClearColor(0, 0, 0, 0);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        }

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
            base.AfficheOpenGL(gl, maintenant, tailleEcran, couleur);
        }

        public virtual void fillConsole(OpenGL gl)
        {
            getConfiguration()?.fillConsole(gl);
        }

        public override bool KeyDown(Form f, Keys k)
        {
            CategorieConfiguration c = getConfiguration();
            if (c?.KeyDown(k) == true)
                return true;

            return base.KeyDown(f, k);
        }
    }
}
