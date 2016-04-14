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
    class OpenGLFonte : IDisposable
    {
        readonly float RATIO_FONTE = 0.8f;// conf.getParametre(CAT, "RatioFonte", 0.70f);
        readonly OpenGL _gl;
        readonly string _caracteres;
        readonly float _hauteurSymbole;
        Texture _texture;
        private float largeurTexture;
        float[] _xCaractere;            // Coordonnees de chaque caractere dans la bitmap qui sert de texture (+1 element virtuel à droite du dernier caractere)

        internal float Largeur(string texte)
        {
            float larg = 0;
            for (int i = 0; i < texte.Length; i++)
            {
                int Indice = getSymboleIndex(texte[i]);
                if (Indice != -1)
                    larg += (_xCaractere[Indice + 1] - _xCaractere[Indice]);
            }

            return larg;
        }

        public OpenGLFonte(OpenGL gl, string caracteres, int taille, FontFamily famille, FontStyle style)
        {
            _caracteres = caracteres;
            int nbSymboles = caracteres.Length;
            _gl = gl;
            _xCaractere = new float[nbSymboles + 1];

            using (Font f = new Font(famille, taille, style))
            {
                float xCaractere = 0;
                float largeur;
                using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                {
                    for (int i = 0; i < nbSymboles; i++)
                    {
                        SizeF size = g.MeasureString(_caracteres[i].ToString(), f);
                        largeur = (float)Math.Round(size.Width * RATIO_FONTE);
                        _hauteurSymbole = Math.Max(_hauteurSymbole, size.Height);

                        _xCaractere[i] = xCaractere;
                        xCaractere += largeur;
                    }
                }
                _xCaractere[nbSymboles] = xCaractere;
                largeurTexture = xCaractere;
                using (Bitmap bmp = new Bitmap((int)Math.Ceiling(largeurTexture), (int)Math.Ceiling(_hauteurSymbole), PixelFormat.Format32bppArgb))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    _texture = new Texture();

                    for (int i = 0; i < nbSymboles; i++)
                    {
                        g.DrawString(_caracteres[i].ToString(), f, Brushes.White, _xCaractere[i], 0);
                    }
                    _texture.Create(gl, bmp);
                }
            }
        }

        internal float Hauteur()
        {
            return _hauteurSymbole;
        }

        public void drawOpenGL(OpenGL gl, string texte, float X, float Y, Color couleur)
        {
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, couleur.A / 256.0f };
            gl.Color(col);
            gl.PushAttrib(OpenGL.GL_ENABLE_BIT);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            _texture.Bind(gl);
            float XGauche = X;
            gl.Begin(OpenGL.GL_QUADS);
            for (int i = 0; i < texte.Length; i++)
            {
                if ( texte[i] == '\n')
                {
                    X = XGauche;
                    Y += Hauteur();
                }

                int Indice = getSymboleIndex(texte[i]);
                if (Indice != -1)
                {
                    float largeurChiffre = _xCaractere[Indice + 1] - _xCaractere[Indice];
                    float xTexture = _xCaractere[Indice] / largeurTexture;
                    float xSuivant = _xCaractere[Indice + 1] / largeurTexture;

                    gl.TexCoord(xTexture, 0.0f); gl.Vertex(X, Y + _hauteurSymbole);
                    gl.TexCoord(xTexture, 1.0f); gl.Vertex(X, Y);
                    gl.TexCoord(xSuivant, 1.0f); gl.Vertex(X + largeurChiffre, Y);
                    gl.TexCoord(xSuivant, 0.0f); gl.Vertex(X + largeurChiffre, Y + _hauteurSymbole);

                    X += largeurChiffre;
                }
            }
            gl.End();
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
            _texture?.Destroy(_gl);
        }
    }
}
