/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 26/06/2014
 * Heure: 09:58
 * 
 * Affiche un objet texte contenant la date du jour
 * Derive de Texte, se contente de fournir la date sous forme de texte
 */
using System;
using System.Drawing ;
namespace ClockScreenSaverGL.Textes
{
	/// <summary>
	/// Description of Date.
	/// </summary>
	public class DateTexte: Texte
	{
		const string CAT = "DateTexte" ;
		static private string _date ; // Sera initialise dans OnDateChange

		public DateTexte(int Px, int Py)
			: base( Px, 0, conf.getParametre(CAT, "VX", -17), 0, conf.getParametre(CAT, "TailleFonte", 60), conf.getParametre(CAT, "Alpha", (byte)160) )
		{
		}

        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
            base.Deplace(maintenant, ref tailleEcran);
            tailleEcran = new Rectangle(tailleEcran.Left, tailleEcran.Top + (int)_taille.Height, tailleEcran.Width, tailleEcran.Height - (int)_taille.Height);
        }
		protected override string getTexte(Temps maintenant)
		{
			return _date ;
		}
		
		public override void DateChangee(Graphics g, Temps maintenant )
		{
			_date = maintenant._temps.ToLongDateString() ;
			_taille = g.MeasureString( _date, _fonte ) ;
		}
		
		protected override SizeF getTailleTexte( Graphics g )
		{
			return _taille ;
		}
	}
}
