/*
 * Un objet graphique Texte qui contient un texte fixe de copyright
 */
using System;
using System.Drawing ;

namespace ClockScreenSaverGL.Textes
{
	/// <summary>
	/// Description of TexteCopyright.
	/// </summary>
	public class TexteCopyright: Texte
	{
		const string CAT = "TexteCopyright" ;
		private const string texte = "Lucien Pilloni 2014" ;
		
		public TexteCopyright(int Px, int Py):
			base( Px, Py, conf.getParametre(CAT, "VX", 4), 
			     		conf.getParametre(CAT, "VY", 4), 
			     		conf.getParametre(CAT, "TailleFonte", 32),
			     		conf.getParametre(CAT, "Alpha", (byte)10))
		{
		}
		
		protected override SizeF getTailleTexte( Graphics g )
		{
			return _taille ;
		}
		
		protected override string getTexte(Temps maintenant)
		{
			return texte ;
		}

#if USE_GDI_PLUS_FOR_2D
        public override void DateChangee(Graphics g, Temps maintenant )
        {
            _taille = g.MeasureString( texte, _fonte ) ;
		}
#else
        public override void DateChangee(SharpGL.OpenGL gl, Temps maintenant)
        {
            Bitmap bmp = new Bitmap(10, 10);
            Graphics g = Graphics.FromImage(bmp);
            _taille = g.MeasureString(texte, _fonte);
        }
#endif

	}
}
