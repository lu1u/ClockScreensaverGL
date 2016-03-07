/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 26/06/2014
 * Heure: 09:58
 * 
 * Affiche un objet texte contenant la date du jour
 * Derive de Texte, se contente de fournir la date sous forme de texte
 */
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
        private string _date; // Sera initialise dans OnDateChange

        public DateTexte(OpenGL gl, int Px, int Py)
            : base(gl, Px, 0, conf.getParametre(CAT, "VX", -17), 0, conf.getParametre(CAT, "TailleFonte", 60), conf.getParametre(CAT, "Alpha", (byte)160))
        {
        }

        public DateTexte(OpenGL gl, int Px, int Py, int tailleFonte)
            : base(gl, Px, 0, 0, 0, tailleFonte, conf.getParametre(CAT, "Alpha", (byte)160))
        {
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
