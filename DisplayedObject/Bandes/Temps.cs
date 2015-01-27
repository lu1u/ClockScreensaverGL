/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 14/12/2014
 * Heure: 23:18
 * 
 * Stockage optimise de l'heure pour calculer une seule fois
 */
using System;

namespace ClockScreenSaver
{
	/// <summary>
	/// Description of Temps.
	/// </summary>
	public class Temps
	{
		public DateTime _temps ;
		public int _Annee, _Mois, _JourDuMois, _JourDeLAnnee ;
		public int _Heure, _Minute, _Seconde, _Millieme ;
		public float _intervalle ;
		public DateTime _derniere ;
		
		public Temps(DateTime t, DateTime derniere )
		{
			_temps = t ;
			_Annee = t.Year ;
			_Mois = t.Month ;
			_JourDuMois = t.Day ;
			_JourDeLAnnee = t.DayOfYear ;
			_Heure = t.Hour ;
			_Minute = t.Minute ;
			_Seconde = t.Second ;
			_Millieme = t.Millisecond ;
			
			_derniere = derniere ;
			_intervalle = (float)(t.Subtract(derniere).TotalMilliseconds / 1000.0);
		}
	}
}
