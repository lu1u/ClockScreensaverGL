/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 21/06/2014
 * Time: 23:03
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
namespace ClockScreenSaverGL.DisplayedObject.Textes
{


    /// <summary>
    /// Description of HeureTexte.
    /// </summary>
    public class HeureTexte : Texte
    {
        const string CAT = "HeureTexte";
        readonly float RATIO_FONTE = 0.75f;// conf.getParametre(CAT, "RatioFonte", 0.70f);
        const int NB_SYMBOLES = 11;
        Texture[] _symboles = new Texture[NB_SYMBOLES];
        readonly int[] _largeurChiffre = new int[NB_SYMBOLES];
        readonly float _hauteurChiffre;

        public HeureTexte(OpenGL gl, int Px, int Py)
            : base(Px, SystemInformation.VirtualScreen.Height - 100, -conf.getParametre(CAT, "VX", 15), 0, conf.getParametre(CAT, "TailleFonte", 80), conf.getParametre(CAT, "Alpha", (byte)180))
        {
            using (Font fonte = CreerFonte(conf.getParametre(CAT, "TailleFonte", 80)))
            {
                using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                {
                    SizeF taille = g.MeasureString("0", fonte);
                    _hauteurChiffre = taille.Height;
                }

                _symboles[0] = CreateSymbole(gl, fonte, "0", out _largeurChiffre[0]);
                _symboles[1] = CreateSymbole(gl, fonte, "1", out _largeurChiffre[1]);
                _symboles[2] = CreateSymbole(gl, fonte, "2", out _largeurChiffre[2]);
                _symboles[3] = CreateSymbole(gl, fonte, "3", out _largeurChiffre[3]);
                _symboles[4] = CreateSymbole(gl, fonte, "4", out _largeurChiffre[4]);
                _symboles[5] = CreateSymbole(gl, fonte, "5", out _largeurChiffre[5]);
                _symboles[6] = CreateSymbole(gl, fonte, "6", out _largeurChiffre[6]);
                _symboles[7] = CreateSymbole(gl, fonte, "7", out _largeurChiffre[7]);
                _symboles[8] = CreateSymbole(gl, fonte, "8", out _largeurChiffre[8]);
                _symboles[9] = CreateSymbole(gl, fonte, "9", out _largeurChiffre[9]);
                _symboles[10] = CreateSymbole(gl, fonte, ":", out _largeurChiffre[10]);
            }
        }

        /// <summary>
        /// Creation de la texture correspondant à un symbole
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private Texture CreateSymbole(OpenGL gl, Font f, string v, out int largeur)
        {
            using (Graphics gr = Graphics.FromHwnd(IntPtr.Zero))
            {
                SizeF taille = gr.MeasureString(v, f);
                largeur = (int)(Math.Ceiling(taille.Width) * RATIO_FONTE);
                int hauteur = (int)Math.Ceiling(taille.Height);

                Texture t = new Texture();
                Bitmap bmp = new Bitmap(largeur, hauteur, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmp))
                    g.DrawString(v, f, Brushes.White, 0, 0);

                t.Create(gl, bmp);

                return t;
            }
        }

        protected override Font CreerFonte(int tailleFonte)
        {
            return new Font(FontFamily.GenericSansSerif, tailleFonte, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
            base.Deplace(maintenant, ref tailleEcran);
            tailleEcran = new Rectangle(tailleEcran.Left, tailleEcran.Top, tailleEcran.Width, tailleEcran.Height - (int)_taille.Height);
        }

        protected override SizeF getTexte(Temps maintenant, out string texte)
        {
            texte = maintenant._Heure + ":"
                + maintenant._Minute.ToString("D2") + ":"
                + maintenant._Seconde.ToString("D2") + ":"
                + maintenant._Millieme.ToString("D3");
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                return g.MeasureString(texte, _fonte);
        }

        protected override bool TexteChange() { return false; }

        /// <summary>
        /// Appelee quand la date change: mettre la date a jour
        /// </summary>
        /// <param name="g"></param>
        /// <param name="maintenant"></param>
        public override void DateChangee(OpenGL gl, Temps maintenant)
        {

        }

        protected override void drawOpenGL(OpenGL gl, Rectangle tailleEcran, Color couleur, Temps maintenant)
        {
            string texte = maintenant._Heure + ":"
                + maintenant._Minute.ToString("D2") + ":"
                + maintenant._Seconde.ToString("D2") + ":"
                + maintenant._Millieme.ToString("D3");
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, _alpha / 256.0f };
            gl.Color(col);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Translate(_trajectoire._Px, _trajectoire._Py, 0);

            for (int i = 0; i < texte.Length; i++)
            {
                char c = texte[i];
                int Indice = (int)c - (int)'0';
                if (Indice > NB_SYMBOLES - 1)
                    Indice = NB_SYMBOLES - 1;

                _symboles[Indice].Bind(gl);
                gl.Begin(OpenGL.GL_QUADS);
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(0, _hauteurChiffre);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(0, 0);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(_largeurChiffre[Indice], 0);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(_largeurChiffre[Indice], _hauteurChiffre);
                gl.End();
                gl.Translate(_largeurChiffre[Indice], 0, 0);
            }

        }
    }
}
