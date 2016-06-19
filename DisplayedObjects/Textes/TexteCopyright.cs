/*
 * Un objet graphique Texte qui contient un texte fixe de copyright
 */
using SharpGL;
using System;
using System.Drawing;

namespace ClockScreenSaverGL.DisplayedObjects.Textes
{
    /// <summary>
    /// Un objet graphique Texte qui contient un texte fixe de copyright
    /// </summary>
    public class TexteCopyright: Texte
	{
		const string CAT = "TexteCopyright" ;
		private const string _texte = "Lucien Pilloni 2014" ;
        private SizeF _Taille;
		public TexteCopyright(OpenGL gl, int Px, int Py):
			base( gl, Px, Py, conf.getParametre(CAT, "VX", 4), 
			     		conf.getParametre(CAT, "VY", 4), 
			     		conf.getParametre(CAT, "TailleFonte", 32),
			     		conf.getParametre(CAT, "Alpha", (byte)10))
		{
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                _Taille = g.MeasureString(_texte, _fonte);
        }
		

		
		protected override SizeF getTexte(Temps maintenant, out string texte )
		{
			texte = _texte ;
            return _Taille;
        }

        public override void DateChangee(OpenGL gl, Temps maintenant)
        {

        }
	}
}
