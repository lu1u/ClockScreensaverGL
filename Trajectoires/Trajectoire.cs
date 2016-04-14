/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 21/06/2014
 * Time: 22:22
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using System.Drawing;
namespace ClockScreenSaverGL
{
    public abstract class Trajectoire
	{
		public float _Px, _Py, _Vx, _Vy;
		
		public abstract void Avance( Rectangle Bounds, SizeF Taille, Temps maintenant ) ;
        public abstract void Avance(RectangleF Bounds, SizeF Taille, Temps maintenant);
		
		public PointF Position
		{
			get
			{
			return new PointF( _Px, _Py) ;
			}
			
			set
			{
				_Px = value.X ;
				_Py = value.Y ;
			}
		}			
	}
}
