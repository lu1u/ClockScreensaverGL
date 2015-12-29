using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL
{
    class OpenGLFonte: IDisposable
    {
        readonly float RATIO_FONTE = 0.8f;// conf.getParametre(CAT, "RatioFonte", 0.70f);
        readonly OpenGL _gl;
        readonly string _caracteres;
        Texture[] _symboles ;
        readonly float[] _largeurChiffre ;
        readonly float _hauteurChiffre;

        public OpenGLFonte( OpenGL gl, string caracteres, int taille, FontFamily famille, FontStyle style)
        {
            _caracteres = caracteres;
            int nbSymboles = caracteres.Length;
            _gl = gl;
            _symboles = new Texture[nbSymboles];
            _largeurChiffre = new float[nbSymboles];

            using (Font f = new Font(famille, taille, style))
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                SizeF size = g.MeasureString("0", f);
                _hauteurChiffre = size.Height;

                for (int i = 0; i < nbSymboles; i++)
                    _symboles[i] = CreateSymbole(gl, f, _caracteres[i], out _largeurChiffre[i]);
            }
            
        }

        /// <summary>
        /// Creation de la texture correspondant à un symbole
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private Texture CreateSymbole(OpenGL gl, Font f, char c, out float largeur)
        {
            using (Graphics gr = Graphics.FromHwnd(IntPtr.Zero))
            {
                String s = c.ToString();
                SizeF taille = gr.MeasureString(s, f);
                largeur = (float)Math.Round(taille.Width * RATIO_FONTE);
                int hauteur = (int)Math.Ceiling(taille.Height);

                Texture t = new Texture();
                using (Bitmap bmp = new Bitmap((int)largeur, hauteur, PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                        g.DrawString(s, f, Brushes.White, 0, 0);

                    t.Create(gl, bmp);
                }
                return t;
            }
        }

        public void drawOpenGL(OpenGL gl, string texte, float X, float Y, Color couleur)
        {
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, couleur.A / 256.0f };
            gl.Color(col);
            gl.PushAttrib(OpenGL.GL_ENABLE_BIT);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            for (int i = 0; i < texte.Length; i++)
            {
                int Indice = getSymboleIndex(texte[i]);
                if (Indice != -1)
                {
                    _symboles[Indice].Bind(gl);
                    gl.Begin(OpenGL.GL_QUADS);
                    gl.TexCoord(0.0f, 0.0f); gl.Vertex(X, Y + _hauteurChiffre);
                    gl.TexCoord(0.0f, 1.0f); gl.Vertex(X, Y);
                    gl.TexCoord(1.0f, 1.0f); gl.Vertex(X + _largeurChiffre[Indice], Y);
                    gl.TexCoord(1.0f, 0.0f); gl.Vertex(X + _largeurChiffre[Indice], Y + _hauteurChiffre);
                    gl.End();
                    X += _largeurChiffre[Indice];
                }
            }
            gl.PopAttrib();
        }

        private int getSymboleIndex(char v)
        {
            for (int i = 0; i < _caracteres.Length; i++)
                if (v == _caracteres[i])
                    return i;

            return -1;
        }

        public void Dispose()
        {
            foreach (Texture t in _symboles)
                t?.Destroy(_gl);
        }
    }
}
