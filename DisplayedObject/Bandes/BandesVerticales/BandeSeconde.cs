/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 21/06/2014
 * Time: 21:41
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using System;
namespace ClockScreenSaverGL.Bandes.BandeVerticale
{
	/// <summary>
	/// Description of BandeSeconde.
	/// </summary>
	public sealed class BandeSeconde: BandeVerticale
	{

        
		public BandeSeconde( float LargeurSeconde, float OrigineX, float Py, int largeur )
			: base( 60, 5, LargeurSeconde, OrigineX, Py, largeur, conf.getParametre( CAT, "AlphaSeconde", (byte)100) )
		{
		}

		protected override void getValue( Temps maintenant, out float value, out float decalage  )
		{
			decalage = maintenant._Millieme/1000.0f ;
			value = maintenant._Seconde + (maintenant._Millieme/1000.0f) ;
		}
	}
}
