/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 16/12/2014
 * Heure: 21:13
 * 
 * Represente une metaballe, sa position, son intensite, la taille de son champs
 */
using System;
using System.Drawing;
namespace ClockScreenSaverGL.DisplayedObjects.Metaballes
{
	/// <summary>
	/// Description of MetaBalle.
	/// </summary>
	public sealed class MetaBalle
	{
		public double _Intensite, _Taille;
		public double _Px, _Py, _Vx, _Vy ;
		
		// Pour optimisation:
		public double _TailleCarre ;

		
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="intensite"></param>
		/// <param name="taille"></param>
		/// <param name="Px"></param>
		/// <param name="Py"></param>
		/// <param name="Vx"></param>
		/// <param name="Vy"></param>
		public MetaBalle(double intensite, double taille, double Px, double Py, double Vx, double Vy )
		{
			_Intensite = intensite ;
			_Taille = taille ;
			_TailleCarre = _Taille * _Taille ;
			_Px = Px ;
			_Py = Py ;
			_Vx = Vx ;
			_Vy = Vy ;
		}
		
		/// <summary>
		/// Idem constructeur, sert a refaire une metaballe sans allocation de memoire
		/// </summary>
		/// <param name="intensite"></param>
		/// <param name="taille"></param>
		/// <param name="Px"></param>
		/// <param name="Py"></param>
		/// <param name="Vx"></param>
		/// <param name="Vy"></param>
		public void Reset(double intensite, double taille, double Px, double Py, double Vx, double Vy )
		{
			_Intensite = intensite ;
			_Taille = taille ;
			_TailleCarre = _Taille * _Taille ;
			_Px = Px ;
			_Py = Py ;
			_Vx = Vx ;
			_Vy = Vy ;
		}
		
		/// <summary>
		/// Retourne la valeur de champs en fonction de la distance: 0..1
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>0..1</returns>
		public double Champ( double x, double y )
		{
			double DistanceCarre = ((x-_Px)*(x-_Px)) + ((y-_Py)*(y-_Py)) ;
			
			if ( DistanceCarre > _TailleCarre )
				// On est au dela du rayon d'influence
				return 0 ;
			// aspects legerement differents:
			return (_TailleCarre-DistanceCarre)/_TailleCarre * _Intensite ;
			//return (Taille-Math.Sqrt(DistanceCarre))/Taille * Intensite;
			//return Math.Sin((_TailleCarre-DistanceCarre)/_TailleCarre*Math.PI/2) * _Intensite ;
		}
	}
}
