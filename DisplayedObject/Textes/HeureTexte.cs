/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 21/06/2014
 * Time: 23:03
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using System.Drawing;
using System.Windows.Forms;
namespace ClockScreenSaverGL.Textes
{
    /// <summary>
    /// Description of HeureTexte.
    /// </summary>
    public class HeureTexte : Texte
    {
        const string CAT = "HeureTexte";
        static string _texte;
        public HeureTexte(int Px, int Py)
            : base(Px, SystemInformation.VirtualScreen.Height - 100, -conf.getParametre(CAT, "VX", 15), 0, conf.getParametre(CAT, "TailleFonte", 80), conf.getParametre(CAT, "Alpha", (byte)180))
        {
        }


        protected override Font CreerFonte(int tailleFonte)
        {
            return new Font(FontFamily.GenericMonospace, tailleFonte, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
            base.Deplace(maintenant, ref tailleEcran);
            tailleEcran = new Rectangle(tailleEcran.Left, tailleEcran.Top, tailleEcran.Width, tailleEcran.Height - (int)_taille.Height);
        }

        protected override string getTexte(Temps maintenant)
        {
            _texte = maintenant._Heure + ":"
                + maintenant._Minute.ToString("D2") + ":"
                + maintenant._Seconde.ToString("D2") + ":"
                + maintenant._Millieme.ToString("D3");

            return _texte;
        }

        /// <summary>
        /// Retourne la taille du texte
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        protected override SizeF getTailleTexte(Graphics g)
        {
            if (_taille == null)
                _taille = g.MeasureString(_texte, _fonte);
            return _taille;
        }

        /// <summary>
        /// Appelee quand la date change: mettre la date a jour
        /// </summary>
        /// <param name="g"></param>
        /// <param name="maintenant"></param>
#if USE_GDI_PLUS_FOR_2D
        public override void DateChangee(Graphics g, Temps maintenant )
        {
            _texte = getTexte(maintenant);
            _taille = g.MeasureString(_texte, _fonte);
        }
#else
        public override void DateChangee(SharpGL.OpenGL gl, Temps maintenant)
        {
            Bitmap bmp = new Bitmap(10, 10);
            Graphics g = Graphics.FromImage(bmp);
            _taille = g.MeasureString(_texte, _fonte);
        }
#endif


    }
}
