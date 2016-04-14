using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;
using System.Drawing;
using System.Text.RegularExpressions;
using SharpGL.SceneGraph.Assets;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Net;

namespace ClockScreenSaverGL.DisplayedObjects.PanneauActualites
{
    class LigneActu : IDisposable
    {
        public string source;
        public string titre;
        public string description;

        private Texture _texture;
        private OpenGL _gl;
        public float largeur { get; private set; }
        public float hauteur { get; private set; }

        
        public LigneActu( string s, string t, string d)
        {
            this.source = nettoieXML(s);
            this.titre = nettoieXML(t);
            this.description = nettoieXML(d);           
        }

        public void Dispose()
        {
            _texture?.Destroy(_gl);
        }

        internal void affiche(OpenGL gl, float x, float y, Font fSource, Font fTitre, Font fDescription, Color couleur, bool afficheDesc)
        {
            if (_texture == null)
                CreerTexture(gl, fSource, fTitre, fDescription, afficheDesc);

            gl.PushAttrib(OpenGL.GL_CURRENT_BIT);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE);
            gl.Color(couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1.0f);

            _texture.Bind(gl);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(x, y);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(x, y-hauteur);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(x+largeur, y-hauteur);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(x+largeur, y);
            gl.End();
            gl.PopAttrib();
        }

        private void CreerTexture(OpenGL gl, Font fSource, Font fTitre, Font fDescription, bool afficheDesc)
        {
            _gl = gl;

            float largeurTitre, largeurSource, largeurDesc = 0;
            float hauteurTitre, hauteurSource, hauteurDesc = 0;
            // Creer la texture representant le texte de cette information
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                SizeF sz = g.MeasureString(titre, fTitre);
                largeurTitre = sz.Width;
                hauteurTitre = sz.Height * 1.1f;
                
                sz = g.MeasureString(source, fSource);
                largeurSource = sz.Width;
                hauteurSource = sz.Height * 1.1f;

                if (afficheDesc)
                {
                    sz = g.MeasureString(description, fDescription);
                    largeurDesc = Math.Min(sz.Width, SystemInformation.VirtualScreen.Width * 0.75f);
                    hauteurDesc = sz.Height * 2.0f;
                }
                
                largeur = Math.Max(largeurSource, Math.Max(largeurTitre, largeurDesc));
                hauteur = (hauteurSource+hauteurTitre+hauteurDesc)*1.1f;
            }

            // Creation de la texture a partir d'une bitmap
            using (Bitmap bmp = new Bitmap((int)Math.Ceiling(largeur), (int)Math.Ceiling(hauteur), PixelFormat.Format32bppArgb))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                float x = 0;
                float y = 0;
                g.DrawString(source, fSource, Brushes.White, x, y);
                y += hauteurSource;

                g.DrawString(titre, fTitre, Brushes.White, x, y);
                y += hauteurTitre;

                if ( afficheDesc)
                TextRenderer.DrawText(g, description, fDescription, new Rectangle( (int)x,(int)y, (int)largeurDesc, (int)hauteurDesc*4), Color.White,
                    TextFormatFlags.Left |
                      TextFormatFlags.TextBoxControl |
                      TextFormatFlags.WordBreak |
                      TextFormatFlags.EndEllipsis);
                
                _texture = new Texture();
                _texture.Create(gl, bmp);
            }
        }

        private static string nettoieXML(string s)
        {
            if (s == null)
                return "null";

            s = Regex.Replace(s, @"<[^>]*>", String.Empty);
            s = WebUtility.HtmlDecode(s);
            return s;
        }
    }
}
