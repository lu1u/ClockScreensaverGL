/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 18/11/2014
 * Heure: 14:12
 * 
 * Pour changer ce modèle utiliser Outils  Options  Codage  Editer les en-têtes standards.
 */
using ClockScreenSaverGL.DisplayedObject;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ClockScreenSaverGL
{
	/// <summary>
	/// Description of Lune.
	/// </summary>
	public class Lune
	{
		//private Bitmap _bitmapLune = null ;
		private int _ageLune = -1 ;
		DateTime _maintenant ;

		
		private static int JulianDate(int d, int m, int y)
		{
			int mm, yy;
			int k1, k2, k3;
			int j;
			
			yy = y - (int)((12 - m) / 10);
			mm = m + 9;
			
			if (mm >= 12)
			{
				mm = mm - 12;
			}
			k1 = (int)(365.25 * (yy + 4712));
			k2 = (int)(30.6001 * mm + 0.5);
			k3 = (int)((int)((yy / 100) + 49) * 0.75) - 38;
			// 'j' for dates in Julian calendar:
			j = k1 + k2 + d + 59;
			if (j > 2299160)
			{
				// For Gregorian calendar:
				j = j - k3; // 'j' is the Julian date at 12h UT (Universal Time)
			}
			return j;
		}
		private static double MoonAge(int d, int m, int y)
		{
			int j = JulianDate(d, m, y);
			//Calculate the approximate phase of the moon
			double ip = (j + 4.867) / 29.53059;
			ip = ip - Math.Floor(ip);
			return ip ;
		}
		public static double CalcMoonAge(DateTime dDate)
		{
			double fJD, fIP, fAge;
			
			fJD = JulianDate(dDate.Day, dDate.Month, dDate.Year );
			fIP = Normalize((fJD - 2451550.1) / 29.530588853);
			fAge = fIP*29.530588853;
			return fAge;
		}
		
		private static double Normalize(double fN)
		{
			fN = fN - Math.Floor(fN);
			if (fN < 0)
			{
				fN = fN + 1;
			}
			return fN;
		}
		
		public String Dump()
		{
			return "Lune " + _ageLune + "/" + CalcMoonAge(_maintenant );
		}
		
		public Bitmap getImageLune( Graphics g, DateTime maintenant )
		{
			_maintenant = maintenant ;
			int lune = (int)Math.Round(CalcMoonAge(_maintenant) / 29.530588853 * 26) ;

            //if ((lune == _ageLune) && (_bitmapLune != null))
            //    return _bitmapLune;
            /*
            if  (lune != _ageLune)
			{
				_ageLune = lune ;
				if  (_bitmapLune != null )
				{
					// On va changer de lune
					_bitmapLune.Dispose() ;
					_bitmapLune = null ;
				}				
			}
			*/
			Bitmap bmp ;
			switch( lune )
			{
					case 0:  bmp = Resources.Lune00 ; break ;
					case 1:  bmp = Resources.Lune01 ; break ;
					case 2:  bmp = Resources.Lune02 ; break ;
					case 3:  bmp = Resources.Lune03 ; break ;
					case 4:  bmp = Resources.Lune04 ; break ;
					case 5:  bmp = Resources.Lune05 ; break ;
					case 6:  bmp = Resources.Lune06 ; break ;
					case 7:  bmp = Resources.Lune07 ; break ;
					case 8:  bmp = Resources.Lune08 ; break ;
					case 9:  bmp = Resources.Lune09 ; break ;
					case 10: bmp = Resources.Lune10 ; break ;
					case 11: bmp = Resources.Lune11 ; break ;
					case 12: bmp = Resources.Lune12 ; break ;
					case 13: bmp = Resources.Lune13 ; break ;
					case 14: bmp = Resources.Lune14 ; break ;
					case 15: bmp = Resources.Lune15 ; break ;
					case 16: bmp = Resources.Lune16 ; break ;
					case 17: bmp = Resources.Lune17 ; break ;
					case 18: bmp = Resources.Lune18 ; break ;
					case 19: bmp = Resources.Lune19 ; break ;
					case 20: bmp = Resources.Lune20 ; break ;
					case 21: bmp = Resources.Lune21 ; break ;
					case 22: bmp = Resources.Lune22 ; break ;
					case 23: bmp = Resources.Lune23 ; break ;
					case 24: bmp = Resources.Lune24 ; break ;
					case 25: bmp = Resources.Lune25 ; break ;
					default: bmp = Resources.Lune00 ; break ;
			}
            Bitmap bmpRes;

            // Rendre cette bitmap conforme à la transparence de l'horloge
            if (g != null)
                bmpRes = new Bitmap(bmp.Width, bmp.Height, g);
            else
                bmpRes = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppRgb);
			Graphics gMem = Graphics.FromImage(bmpRes);
			
			float[][] ptsArray =
			{
				new float[] {1, 0, 0, 0, 0}, 
				new float[] {0, 1, 0, 0, 0}, 
				new float[] {0, 0, 1, 0, 0}, 
				new float[] {0, 0, 0, HorlogeRonde.ALPHA_AIGUILLES/255.0f, 0}, 
				new float[] {0, 0, 0, 0, 1}
			};
			
			ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
			ImageAttributes imgAttribs = new ImageAttributes();
			imgAttribs.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
			
			gMem.DrawImage(bmp, 
			               new Rectangle(0, 0, bmp.Width, bmp.Height), 
			               0, 0, bmp.Width, bmp.Height, 
			               GraphicsUnit.Pixel, imgAttribs) ;
			gMem.Dispose() ;
			return bmpRes;
		}
	}
}
