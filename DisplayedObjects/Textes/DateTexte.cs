/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 26/06/2014
 * Heure: 09:58
 * 
 * Affiche un objet texte contenant la date du jour
 * Derive de Texte, se contente de fournir la date sous forme de texte
 */
using ClockScreenSaverGL.Config;
using SharpGL;
using System;
using System.Drawing;
namespace ClockScreenSaverGL.DisplayedObjects.Textes
{
    /// <summary>
    /// Description of Date.
    /// </summary>
    public class DateTexte : Texte
    {
        const string CAT = "DateTexte";
        static protected CategorieConfiguration c = Config.Configuration.getCategorie(CAT);
        private string _date; // Sera initialise dans OnDateChange

        public DateTexte(OpenGL gl, int Px, int Py)
            : base(gl, Px, 0, c.getParametre("VX", -17), 0, c.getParametre("TailleFonte", 60), c.getParametre("Alpha", (byte)160))
        {
        }

        public DateTexte(OpenGL gl, int Px, int Py, int tailleFonte)
            : base(gl, Px, 0, 0, 0, tailleFonte, c.getParametre("Alpha", (byte)160))
        {
        }

        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
        protected override SizeF getTexte(Temps maintenant, out string texte)
        {
            _date = maintenant._temps.ToLongDateString();
            texte = _date;
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                return g.MeasureString(_date, _fonte);
        }


        public override void DateChangee(OpenGL gl, Temps maintenant)
        {
            _date = maintenant._temps.ToLongDateString();
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                _taille = g.MeasureString(_date, _fonte);
        }

    }
}
