/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 20/11/2014
 * Heure: 23:09
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using SharpGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
namespace ClockScreenSaverGL.DisplayedObject.Textes
{

    /// <summary>
    /// Description of Citations.
    /// </summary>
    public sealed partial class Citations : Texte, IDisposable
    {
        #region Parametres
        const string CAT = "Citation";

        private static readonly int DELAI_CHANGEMENT = 1000 * 60 * conf.getParametre(CAT, "DelaiChange", 1);	// x minutes entre les changements de citation
        private static readonly int TailleMax = conf.getParametre(CAT, "TailleMax", 48);
        #endregion
        private String _citation;
        private String _auteur;
        private DateTime _changement;
        private int _derniereCitation;
        private RectangleF _rectCitation, _rectAuteur;
        private bool _citationChangee = false;
        private int _tailleFonte, _tailleFonteAuteur;
        public Citations(Form f, int Px, int Py)
            : base(Px, Py,
                   conf.getParametre(CAT, "VX", -15),
                   conf.getParametre(CAT, "VY", 12),
                   10,
                   conf.getParametre(CAT, "Alpha", (byte)150))
        {
            MelangerCitations();

            _derniereCitation = new Random().Next(0, _citations.Length - 1);
            ChoisitCitation(f);
        }


        protected override bool TexteChange() { return _citationChangee; }


        /// <summary>
        /// Choisit la prochaine citation
        /// </summary>
        /// <param name="f"></param>
        private void ChoisitCitation(Form f)
        {
            Graphics g = f.CreateGraphics();
            prochaineCitation();
            g.Dispose();
        }

        /// <summary>
        /// Melanger les citations aleatoirement
        /// </summary>
        private void MelangerCitations()
        {
            Random r = new Random();
            int DeuxiemeIndice;
            for (int i = 0; i < _citations.Length; i++)
            {
                do
                {
                    DeuxiemeIndice = r.Next(0, _citations.Length);
                }
                while (DeuxiemeIndice == i);

                string temp = _citations[i];
                _citations[i] = _citations[DeuxiemeIndice];
                _citations[DeuxiemeIndice] = temp;
            }
        }

        /// <summary>
        /// Choisit une citation dans la liste
        /// Calcule une taille de texte adequate pour l'afficher
        /// </summary>
        /// <param name="g"></param>
        private void prochaineCitation()
        {
            // Puisque les citations ont ete melangees, on prend la suivante dans la liste
            if (_derniereCitation < (_citations.Length - 1))
                _derniereCitation++;
            else
                _derniereCitation = 0;

            // Separer la citation et l'auteur
            if (_citations[_derniereCitation].StartsWith("*"))
            {
                // C'est un 'Le saviez-vous'
                _citation = Resources.LeSaviezVous;
                _auteur = _citations[_derniereCitation].Substring(1);
            }
            else
            {
                string[] words = _citations[_derniereCitation].Split('|');
                _citation = words[0];
                _auteur = "(" + words[1] + ")";// - {" + _derniereCitation + '/' + _citations.Length + '}';
            }

            _citation.Replace("\\n", "\n");
            // Choisir une taille de texte adequate


            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                _tailleFonte = Math.Min(calculeTailleTexte(g, _citation), calculeTailleTexte(g, _auteur));
                _tailleFonteAuteur = _tailleFonte;

                // Calculer la taille du texte affiche
                SizeF stringSize = new SizeF();
                using (Font fonte = new Font(FontFamily.GenericSansSerif, _tailleFonte, FontStyle.Italic, GraphicsUnit.Pixel))
                    stringSize = g.MeasureString(_citation, fonte, SystemInformation.VirtualScreen.Width);
                _rectCitation = new RectangleF(0, 0, stringSize.Width, stringSize.Height);

                using (Font fonte = new Font(FontFamily.GenericSansSerif, _tailleFonteAuteur, FontStyle.Italic, GraphicsUnit.Pixel))
                    stringSize = g.MeasureString(_auteur, fonte, SystemInformation.VirtualScreen.Width);
                _rectAuteur = new RectangleF(0, 0, stringSize.Width, stringSize.Height);

                _taille = new SizeF(Math.Max(_rectCitation.Width, _rectAuteur.Width), (_rectCitation.Height + _rectAuteur.Height));
                _changement = DateTime.Now;
            }
            _citationChangee = true;
        }

        /// <summary>
        /// Calcule une taille de texte adequate pour afficher la chaine donnee sur l'ecran sans que celle
        /// ci ne depasse un certain ratio
        /// </summary>
        /// <param name="g"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private int calculeTailleTexte(Graphics g, String s)
        {

            int LargeurEcran = (int)(SystemInformation.VirtualScreen.Width * conf.getParametre(CAT, "RatioTailleFonte", 0.4f));
            int TailleFonte = 30;

            Font f = new Font(FontFamily.GenericSansSerif, TailleFonte, FontStyle.Regular, GraphicsUnit.Pixel);

            // Choisir la plus grande taille de fonte possible
            while ((g.MeasureString(s, f).Width < LargeurEcran) && (TailleFonte < TailleMax))
            {
                f.Dispose();
                f = new Font(FontFamily.GenericSansSerif, TailleFonte, FontStyle.Regular, GraphicsUnit.Pixel);

                TailleFonte++;
            }

            f.Dispose();
            return TailleFonte;
        }

        /// <summary>
        ///  Pas utilisee
        /// </summary>
        /// <param name="maintenant"></param>
        /// <returns></returns>
        protected override SizeF getTexte(Temps maintenant, out string texte)
        {
            texte = _citation;
            return _taille;
        }

        /// <summary>
        /// Creer la bitmap contenant ce qu'on veut afficher
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="maintenant"></param>
        protected override void CreateBitmap(OpenGL gl, Temps maintenant)
        {
            _bitmap?.Dispose();

            string texte;
            _taille = getTexte(maintenant, out texte);

            _bitmap = new Bitmap((int)_taille.Width, (int)_taille.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(_bitmap))
            {
                RectangleF rect = new RectangleF(0, 0, _rectCitation.Width, _rectCitation.Height);
                using (Font fonte = new Font(FontFamily.GenericSansSerif, _tailleFonte, FontStyle.Italic, GraphicsUnit.Pixel))
                    g.DrawString(_citation, fonte, Brushes.White, rect);

                rect.Offset(0, _rectCitation.Height);
                using (Font fonteAuteur = new Font(FontFamily.GenericSansSerif, _tailleFonteAuteur, FontStyle.Italic, GraphicsUnit.Pixel))
                    g.DrawString(_auteur, fonteAuteur, Brushes.LightGray, rect.Left, rect.Top);
            }

            _texture.Create(gl, _bitmap);
            _citationChangee = false;
        }
        /*
        /// <summary>
        /// Affiche cet objet
        /// </summary>
        /// <param name="g"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleur"></param>
        public override void AfficheGDI(Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif

            RectangleF rect = new RectangleF(_trajectoire._Px, _trajectoire._Py, _rectCitation.Width, _rectCitation.Height);

            using (Brush brush = new SolidBrush(getCouleurAvecAlpha(couleur, _alpha)))
                g.DrawString(_citation, _fonte, brush, rect);

            rect.Offset(0, _rectCitation.Height);

            using (Brush brush = new SolidBrush(getCouleurAvecAlpha(couleur, (byte)(_alpha >> 1))))
                g.DrawString(_auteur, _fonteAuteur, brush, rect.Left, rect.Top);

            // Changer la citation toutes les 5 minutes
            if (DateTime.Now.Subtract(_changement).TotalMilliseconds > DELAI_CHANGEMENT)
                prochaineCitation(g);
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }
        */
        /// <summary>
        /// Pression sur une touche, si c'est 'C' : changer de citation et signaler qu'on a utilise la touche
        /// </summary>
        /// <param name="f"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public override bool KeyDown(Form f, Keys k)
        {
            if (Keys.C == k)
            {
                ChoisitCitation(f);
                return true;
            }

            return false;
        }

        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
            base.Deplace(maintenant, ref tailleEcran);
            // Changer la citation toutes les n minutes
            if (DateTime.Now.Subtract(_changement).TotalMilliseconds > DELAI_CHANGEMENT)
                prochaineCitation();
        }

        /// <summary>
        /// Ajoute le message d'aide correspondant a cet objet graphique
        /// </summary>
        /// <param name="s"></param>
        public override void AppendHelpText(StringBuilder s)
        {
            s.Append(Resources.AideCitation);
        }

        void IDisposable.Dispose()
        {
            _bitmap?.Dispose();
        }
    }
}
