using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects
{
    public class Console : IDisposable
    {
        OpenGLFonte _fonte;
        private class LIGNE
        {
            public LIGNE(Color c, String s)
            {
                couleur = c;
                texte = s;
            }
            public Color couleur;
            public String texte;
        };

        private List<LIGNE> _lignes;

        private static Console INSTANCE = null;

        static public Console getInstance(OpenGL gl)
        {
            if (INSTANCE == null)
                INSTANCE = new Console(gl);

            return INSTANCE;
        }

        private Console(OpenGL gl)
        {

            _fonte = new OpenGLFonte(gl, OpenGLFonte.CARACTERES, 12, FontFamily.GenericSansSerif, FontStyle.Regular);
            _lignes = new List<LIGNE>();
        }

        public void Dispose()
        {
            _fonte?.Dispose();
        }

        public void Clear()
        {
            _lignes.Clear();
        }

        public void AddLigne(Color couleur, String Texte)
        {
            _lignes.Add(new LIGNE(couleur, Texte));
        }

        public void trace( OpenGL gl, Rectangle tailleEcran)
        {
            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D(0, tailleEcran.Width-1, 0, tailleEcran.Height-1);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            float y = tailleEcran.Height - _fonte.Hauteur(); ;
            foreach( LIGNE ligne in _lignes)
            {
                _fonte.drawOpenGL(gl, ligne.texte, 0, y, ligne.couleur);
                y -= _fonte.Hauteur();
            }

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();
        }
    }
}
