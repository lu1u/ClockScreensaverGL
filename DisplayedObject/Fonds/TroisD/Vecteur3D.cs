/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 17/01/2015
 * Heure: 13:04
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD
{
	/// <summary>
	/// Description of Vecteur3D.
	/// </summary>
	public class Vecteur3D
	{
		public Vecteur3D()
		{
		}
		public Vecteur3D(float X, float Y, float Z)
		{
			x = X ;
			y = Y ;
			z = Z ;
		}
		
		public float x, y, z;
		
		float norme()
		{
			return(float)Math.Sqrt(x*x+y*y+z*z);
		}
		
		public void Normalize()
		{
			float n=norme();
			x/=n;
			y/=n;
			z/=n;
		}
		
		static public Vecteur3D operator * (float f,Vecteur3D v)     //produit par un réel
		{Vecteur3D z=v;
			z.multiplier_par(f);
			return(z);
		}
		static public Vecteur3D operator * (Vecteur3D v,float f)     //le prod par un float est commutatif !!!
		{return(f*v);}    //je l'ai déjà défini dans l'autre sens, autant s'en servir !
		
		static public Vecteur3D operator / (Vecteur3D v,float f)
		{return(v*(1/f));}
		
		static public float operator * (Vecteur3D v,Vecteur3D w)     //produit scalaire
		{return v.prodscal(w);}
		
		static public Vecteur3D operator + (Vecteur3D v,Vecteur3D w)     //somme vectorielle
		{Vecteur3D z=v;
			v.additionner(w);
			return(z);
		}
		
		static public Vecteur3D operator - (Vecteur3D v,Vecteur3D w)     //différence vectorielle
		{return(v+((-1)*w));}
		
		static public Vecteur3D operator ^ (Vecteur3D v,Vecteur3D w)     //produit vectoriel
		{
			Vecteur3D z = new Vecteur3D(
				v.y*w.z-w.y*v.z ,
				v.z*w.x-w.z*v.x ,
				v.x*w.y-w.x*v.y
			);
			return z;
		}
		public Vecteur3D Cross (Vecteur3D w)     //produit vectoriel
		{
			Vecteur3D z = new Vecteur3D((this.y*w.z) - (w.y*this.z),
				(this.z*w.x) - (w.z*x) ,
				(x*w.y) - (w.x*y)
			);
			return z;
		}
		void multiplier_par(float a) {x=a*x;y=a*y;z=a*z;}
		float prodscal(Vecteur3D v) {return(x*v.x+y*v.y+z*v.z);}
		void additionner(float a)
		{x=a+x;y=a+y;z=a+z;}
		void additionner(float a,float b,float c)
		{x=a+x;y=b+y;z=c+z;}
		public void additionner(Vecteur3D a)
		{x=x+a.x;y=y+a.y;z=z+a.z;}
	}
}
