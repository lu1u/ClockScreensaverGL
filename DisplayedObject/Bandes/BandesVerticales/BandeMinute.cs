/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 22/06/2014
 * Time: 16:28
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using SharpGL;
using System;
namespace ClockScreenSaverGL.Bandes.BandeVerticale
{
	/// <summary>
	/// Description of BandeMinute.
	/// </summary>
	public sealed class BandeMinute: BandeVerticale
	{
		public BandeMinute( OpenGL gl, float LargeurSeconde, float OrigineX, float Py, int largeur )
			: base( gl, 60, 5, LargeurSeconde, OrigineX, Py, largeur, conf.getParametre( CAT, "AlphaMinute", (byte)60) )
		{
            CreerTexture(gl, 0, 59, 5);
        }
		protected override void getValue( Temps maintenant, out float value, out float decalage  )
		{
			decalage = ( maintenant._Seconde + (maintenant._Millieme/1000.0f))/60.0f ;
			value = maintenant._Minute ;
		}

	}
}
