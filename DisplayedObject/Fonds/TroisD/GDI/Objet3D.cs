/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 30/12/2014
 * Heure: 23:10
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;

namespace ClockScreenSaverGL.Fonds.TroisD.GDI
{
	/// <summary>
	/// Description of Flocon.
	/// </summary>
	public sealed class Objet3D
	{
		public float x, y, z;			// Position
		public int type ;				// Type (= bitmap)
		public float vx, vy, vz ;		// Vitesse
		public float rayon ;			// Taille
		public bool aSupprimer ;
	}
}
