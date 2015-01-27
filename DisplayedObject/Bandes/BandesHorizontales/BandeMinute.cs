/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 22/06/2014
 * Time: 16:28
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using System;
using System.Text;
namespace ClockScreenSaverGL.Bandes.BandeHorizontale
{
	/// <summary>
	/// Description of BandeMinute.
	/// </summary>
	public sealed class BandeMinute: BandeHorizontale
	{
		public BandeMinute( float LargeurSeconde, float OrigineX, float Py, int largeur )
			: base( 60, 5, LargeurSeconde, OrigineX, Py, largeur, conf.getParametre( CAT, "AlphaMinute", (byte)60) )
		{
		}
		
		protected override void getValue( Temps maintenant, out float value, out float decalage  )
		{
			decalage = ( maintenant._Seconde + (maintenant._Millieme/1000.0f))/60.0f ;
			value = maintenant._Minute ;
		}

	}
}
