/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 21/06/2014
 * Time: 21:41
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using SharpGL;
using System;
namespace ClockScreenSaverGL.DisplayedObjects.Bandes.BandeHorizontale
{
    /// <summary>
    /// Description of BandeSeconde.
    /// </summary>
    public sealed class BandeSeconde : BandeHorizontale
    {
        public BandeSeconde(OpenGL gl, float LargeurSeconde, float OrigineX, float Py, int largeur)
            : base(gl, 60, 5, LargeurSeconde, OrigineX, Py, largeur, c.getParametre( "AlphaSeconde", (byte) 100 ) )
        {
        }

        protected override void getValue(Temps maintenant, out float value, out float decalage)
        {
            decalage = maintenant.milliemesDeSecondes / 1000.0f;
            value = maintenant.seconde + decalage;
        }
    }
}