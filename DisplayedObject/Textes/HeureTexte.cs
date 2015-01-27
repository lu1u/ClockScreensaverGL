/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 21/06/2014
 * Time: 23:03
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using System;
using System.Drawing;

namespace ClockScreenSaverGL.Textes
{
	/// <summary>
	/// Description of HeureTexte.
	/// </summary>
	public class HeureTexte: Texte
	{
		const string CAT = "HeureTexte" ;
		static string _texte; 
       public HeureTexte(int Px, int Py)
       	: base( Px, Py, conf.getParametre(CAT, "VX", 15), conf.getParametre(CAT, "VY", -16), conf.getParametre(CAT, "TailleFonte", 80), conf.getParametre(CAT, "Alpha", (byte)180) )
		{
		}
		
				
		protected override Font CreerFonte( int tailleFonte )
		{
			return new Font( FontFamily.GenericMonospace, tailleFonte, FontStyle.Bold, GraphicsUnit.Pixel ) ;
		}
		
		protected override string getTexte(Temps maintenant)
		{
			_texte =  maintenant._Heure + ":" 
				+ maintenant._Minute.ToString("D2") + ":" 
				+ maintenant._Seconde.ToString("D2") + ":" 
				+ maintenant._Millieme.ToString("D3") ;
			
			return _texte ;
		}
		
		protected override SizeF getTailleTexte( Graphics g )
		{
			_taille = g.MeasureString( _texte, _fonte ) ;
			return _taille ;
		}
		
		public override void DateChangee(Graphics g, Temps maintenant )
		{
			_texte =  getTexte( maintenant ) ;
			_taille = g.MeasureString( _texte, _fonte ) ;
		}
		
	}
}
