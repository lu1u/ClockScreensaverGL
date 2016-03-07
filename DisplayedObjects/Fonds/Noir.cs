/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 22/12/2014
 * Heure: 15:36
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Drawing;
using System.Collections.Generic;
using SharpGL;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds
{
	/// <summary>
	/// Description of Noir.
	/// </summary>
	public class Noir: Couleur
	{
		public Noir(OpenGL gl, int Cx, int Cy): base( gl, Cx, Cy)
		{
		}

        protected override Color getCouleur(Color couleur)
        {
            return Color.Black ;
        }
    }
}
