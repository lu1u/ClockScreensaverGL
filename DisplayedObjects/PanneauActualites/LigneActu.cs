using ClockScreenSaverGL.Config;
using SharpGL;
using SharpGL.SceneGraph.Assets;
///
/// Une ligne d'actualite extraite d'un flux RSS
///
///
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ClockScreenSaverGL.DisplayedObjects.PanneauActualites
{
    class LigneActu : IDisposable
    {
        public static readonly char[] TRIM_CARACTERES = { ' ', '\n', '\r' };
        public string _source;
        public string _titre;
        public string _description;
        public string _date;
        public Image _bitmap;

        private Texture _texture;
        private OpenGL _gl;
        public float largeur { get; private set; }
        public float hauteur { get; private set; }


        public LigneActu(string s, string t, DateTime d, string desc, Image b)
        {
            this._source = nettoieXML( s ) ;
            this._titre = nettoieXML(t);
            this._date = d.ToShortDateString();
            this._description = nettoieXML(desc);
            this._bitmap = b;
        }

        public void Dispose()
        {
            _bitmap?.Dispose();
            _texture?.Destroy(_gl);
        }

        internal void affiche(OpenGL gl, float x, float y, bool afficheDesc)
        {
            if (_texture == null)
                CreerTexture(gl, afficheDesc);

            _texture.Bind(gl);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(x, y);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(x, y - hauteur);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(x + largeur, y - hauteur);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(x + largeur, y);
            gl.End();            
        }

        /// <summary>
        /// Creation de la texture OpenGL qui permet d'afficher cette actualite
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="afficheDesc"></param>
        private void CreerTexture(OpenGL gl, bool afficheDesc)
        {
            _gl = gl;

            float largeurTitre, largeurSource, largeurDesc = 0;
            float hauteurTitre, hauteurSource, hauteurDesc = 0;
            
            // Creer la texture representant le texte de cette information
            using (Font fTitre = new Font(FontFamily.GenericSansSerif, Actualites.TAILLE_TITRE, FontStyle.Bold))
            using (Font fDescription = new Font(FontFamily.GenericSansSerif, Actualites.TAILLE_DESCRIPTION, FontStyle.Regular))
            using ( Font fSource = new Font( FontFamily.GenericSansSerif, Actualites.TAILLE_SOURCE, FontStyle.Italic ) )
            {
                using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                {
                    SizeF sz = g.MeasureString(_titre, fTitre);
                    largeurTitre = sz.Width;
                    hauteurTitre = sz.Height * 1.1f;

                    sz = g.MeasureString(_source, fSource);
                    largeurSource = sz.Width;
                    hauteurSource = sz.Height * 1.05f;

                    if (afficheDesc)
                    {
                        sz = g.MeasureString(_description, fDescription);
                        largeurDesc = Math.Min(sz.Width, SystemInformation.VirtualScreen.Width * 0.75f);
                        hauteurDesc = sz.Height * 2.0f;
                    }

                    largeur = Math.Max(largeurSource, Math.Max(largeurTitre, largeurDesc)) + 50 ;
                    hauteur = (hauteurSource + hauteurTitre + hauteurDesc) * 1.05f;

                    if (_bitmap != null)
                        largeur += _bitmap.Width;
                }

                // Creation de la texture a partir d'une bitmap
                using (Bitmap bmp = new Bitmap((int)Math.Ceiling(largeur), (int)Math.Ceiling(hauteur), PixelFormat.Format32bppArgb))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    float x = 0;
                    float y = 0;
                    g.DrawString(_source + " - " + _date, fSource, Brushes.White, x, y);
                    y += hauteurSource;

                    if (_bitmap != null)
                    {
                        g.DrawImage(_bitmap, x, y);
                        x += _bitmap.Width;
                        largeurDesc -= _bitmap.Width;
                    }

                    g.DrawString(_titre, fTitre, Brushes.White, x, y);
                    y += hauteurTitre;

                    if (afficheDesc)
                        TextRenderer.DrawText(g, _description, fDescription, new Rectangle((int)x, (int)y, (int)largeurDesc, (int)hauteurDesc * 4), Color.White,
                            TextFormatFlags.Left |
                            TextFormatFlags.NoPrefix|
                              TextFormatFlags.TextBoxControl |
                              TextFormatFlags.WordBreak |
                              TextFormatFlags.EndEllipsis);

                    _texture = new Texture();
                    _texture.Create(gl, bmp);

                    _bitmap?.Dispose();
                    _bitmap = null;
                }
            }
        }

        /// <summary>
        /// Nettoie les codes XML et les caractères indesirables
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string nettoieXML(string s)
        {
            if (s == null)
                return "null";

            s = Regex.Replace(s, @"<[^>]*>", String.Empty);
            s = s.Replace("&nbsp;", " ");
            s = WebUtility.HtmlDecode(s);
            s = s.Trim(TRIM_CARACTERES);

            if (s.IndexOf("nbsp") != -1)
                Log.getInstance().warning("Reste un nbsp: " + s);
            return s;
        }
    }
}
