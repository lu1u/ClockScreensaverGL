using System.Drawing;

namespace ClockScreenSaverGL.DisplayedObjects.Meteo
{
    public class LignePrevisionMeteo
    {
        public Bitmap bmp;
        public string TMin;
        public string TMax;
        public string text;
        public string day;

        public LignePrevisionMeteo(Bitmap b, string min, string max, string t, string d)
        {
            bmp = b;
            TMin = min;
            TMax = max;
            text = t;
            if (d.Equals("Mon"))
                day = Resources.Lundi;
            else
                if (d.Equals("Tue"))
                    day = Resources.Mardi;
                else
                    if (d.Equals("Wed"))
                        day = Resources.Mercredi;
                    else
                        if (d.Equals("Thu"))
                            day = Resources.Jeudi;
                        else if (d.Equals("Fri"))
                            day = Resources.Vendredi;
                        else if (d.Equals("Sat"))
                            day = Resources.Samedi;
                        else if (d.Equals("Sun"))
                            day = Resources.Dimanche;
                        else
                            day = d;
        }
    }   ;
}
