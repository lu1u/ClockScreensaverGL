/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 27/11/2014
 * Heure: 10:13
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Drawing;

namespace ClockScreenSaverGL
{
	/// <summary>
	/// Description of TrajectoireDiagonale.
	/// </summary>
	public class TrajectoireDiagonale : Trajectoire
	{
		public TrajectoireDiagonale( float Px, float Py, float Vx, float Vy )
		{
			_Px = Px ;
			_Py = Py ;
			_Vx = Vx ;
			_Vy = Vy ;
		}
		
		/// <summary>
		/// Deplace l'objet a vitesse constante, l'objet doit rester dans l'ecran
		/// </summary>
		/// <param name="Bounds"></param>
		/// <param name="Taille"></param>
		public override void Avance( Rectangle Bounds, SizeF Taille, Temps maintenant )
		{
			_Px += (_Vx * maintenant.intervalleDepuisDerniereFrame) ;
			
			if ((_Px < Bounds.Left) && (_Vx < 0))
				_Vx = Math.Abs(_Vx) ;
			else
				if ( ((_Px + Taille.Width) > Bounds.Right ) && (_Vx > 0))
					_Vx = - Math.Abs(_Vx ) ;

            _Py += (_Vy * maintenant.intervalleDepuisDerniereFrame);
			if ((_Py < Bounds.Top) && (_Vy < 0))
				_Vy = Math.Abs(_Vy) ;
			else
				if (( (_Py + Taille.Height) > Bounds.Bottom ) && (_Vy >0))
					_Vy = - Math.Abs(_Vy ) ;
		}
		/// <summary>
		/// Deplace l'objet a vitesse constante, l'objet doit rester dans l'ecran
		/// </summary>
		/// <param name="Bounds"></param>
		/// <param name="Taille"></param>
        public override void Avance(RectangleF Bounds, SizeF Taille, Temps maintenant)
		{
            _Px += (_Vx * maintenant.intervalleDepuisDerniereFrame);
			
			if ((_Px < Bounds.Left) && (_Vx < 0))
				_Vx = Math.Abs(_Vx) ;
			else
				if ( ((_Px + Taille.Width) > Bounds.Right ) && (_Vx > 0))
					_Vx = - Math.Abs(_Vx ) ;

            _Py += (_Vy * maintenant.intervalleDepuisDerniereFrame);
			if ((_Py < Bounds.Top) && (_Vy < 0))
				_Vy = Math.Abs(_Vy) ;
			else
				if (( (_Py + Taille.Height) > Bounds.Bottom ) && (_Vy >0))
					_Vy = - Math.Abs(_Vy ) ;
		}
		
		override public String ToString()
		{
			return "Vx:" + _Vx + " Vy:" + _Vy   + " Px:" + _Px + " Py:" + _Py;
		}
	}
}
