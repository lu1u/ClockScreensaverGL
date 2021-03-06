﻿/*
 * Un objet graphique Texte qui contient un texte fixe de copyright
 */
using ClockScreenSaverGL.Config;
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
        static protected CategorieConfiguration c = Config.Configuration.getCategorie(CAT);
        private const string _texte = "Lucien Pilloni 2014" ;
        private SizeF _Taille;
		public TexteCopyright(OpenGL gl, int Px, int Py):
			base( gl, Px, Py, c.getParametre( "VX", 4 ), 
			     		c.getParametre( "VY", 4 ), 
			     		c.getParametre( "TailleFonte", 32 ),
			     		c.getParametre( "Alpha", (byte) 10 ) )
		{
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                _Taille = g.MeasureString(_texte, _fonte);
        }


        public override CategorieConfiguration getConfiguration()
        {
            return c;
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
