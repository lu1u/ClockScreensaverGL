/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 22/06/2014
 * Time: 16:45
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using SharpGL;
using System;

namespace ClockScreenSaverGL.DisplayedObjects.Bandes.BandeVerticale
{
    /// <summary>
    /// Description of BandeHeure.
    /// </summary>
    public sealed class BandeHeure : BandeVerticale
    {
        public BandeHeure(OpenGL gl, float LargeurSeconde, float OrigineX, float Py, int largeur)
            : base(gl, 24, 1, LargeurSeconde, OrigineX, Py, largeur, conf.getParametre(CAT, "AlphaHeure", (byte)40))
        {
        }

        protected override void getValue(Temps maintenant, out float value, out float decalage)
        {
            decalage = (maintenant._Minute + (maintenant._Seconde + (maintenant._Millieme / 1000.0f)) / 60.0f) / 60.0f;
            value = maintenant._Heure;
        }

    }
}
