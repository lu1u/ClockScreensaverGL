/*
 * Un objet graphique Texte qui contient un texte fixe de copyright
 */
using SharpGL;
using System;
using System.Drawing;

namespace ClockScreenSaverGL.DisplayedObject.Textes
{
	/// <summary>
	/// Description of TexteCopyright.
	/// </summary>
	public class TexteCopyright: Texte
	{
		const string CAT = "TexteCopyright" ;
		private const string _texte = "Lucien Pilloni 2014" ;
		
		public TexteCopyright(int Px, int Py):
			base( Px, Py, conf.getParametre(CAT, "VX", 4), 
			     		conf.getParametre(CAT, "VY", 4), 
			     		conf.getParametre(CAT, "TailleFonte", 32),
			     		conf.getParametre(CAT, "Alpha", (byte)10))
		{

        }
		

		
		protected override SizeF getTexte(Temps maintenant, out string texte )
		{
			texte = _texte ;
            //using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                return g.MeasureString(texte, _fonte);
        }

        public override void DateChangee(OpenGL gl, Temps maintenant)
        {

        }
	}
}
