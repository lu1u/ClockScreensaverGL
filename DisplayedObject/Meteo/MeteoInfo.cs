///
/// Exploite l'API Yahoo Weather
/// https://developer.yahoo.com/weather/documentation.html

using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Xml.XPath;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
namespace ClockScreenSaverGL.DisplayedObject.Meteo
{
    class MeteoInfo
    {
        #region MEMBRES_PUBLICS
        public bool donneesPretes;
           
        public List<LignePrevisionMeteo> _lignes = new List<LignePrevisionMeteo>();
        public string _location;
        public string lever;
        public string coucher;
        public string _title;
        #endregion MEMBRES_PUBLICS

        private string _url;
        private DateTime _datePrevisions;
        private DateTime _finPrevisions;

        public MeteoInfo(string url)
        {
            donneesPretes = false;
            _url = url ;
            new Thread(new ThreadStart(ChargeDonnees)).Start();
        }

        /// <summary>
        /// Lecture des previsions meteo en multithreading
        /// </summary>
        public void ChargeDonnees()
        {
            // Create a new XmlDocument  
            XPathDocument doc = new XPathDocument(_url);

            // Create navigator  
            XPathNavigator navigator = doc.CreateNavigator();

            // Set up namespace manager for XPath  
            XmlNamespaceManager ns = new XmlNamespaceManager(navigator.NameTable);
            ns.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

            XPathNodeIterator nodes = navigator.Select("/rss/channel/title", ns);
            {   // Titre
                nodes.MoveNext();
                XPathNavigator l = nodes.Current;
                _title = l.InnerXml;
            }

            { // Duree de validite des previsions
                _datePrevisions = DateTime.Now;

                nodes = navigator.Select("/rss/channel/ttl", ns);
                nodes.MoveNext();
                XPathNavigator l = nodes.Current;
                int duree = Int32.Parse(l.InnerXml);
                _finPrevisions = _datePrevisions.AddMinutes(duree);
            }

            { // Localisation 
                nodes = navigator.Select("/rss/channel/yweather:location", ns);
                nodes.MoveNext();
                XPathNavigator l = nodes.Current;
                _location = l.GetAttribute("city", ns.DefaultNamespace);
            }

            { // Lever et coucher du soleil
                nodes = navigator.Select("/rss/channel/yweather:astronomy", ns);
                nodes.MoveNext();
                XPathNavigator l = nodes.Current;
                lever = TraduireHeure(l.GetAttribute("sunrise", ns.DefaultNamespace));
                coucher = TraduireHeure(l.GetAttribute("sunset", ns.DefaultNamespace));
            }

            { // Lignes de prevision
                nodes = navigator.Select("/rss/channel/item/yweather:forecast", ns);
                while (nodes.MoveNext())
                {
                    XPathNavigator node = nodes.Current;
                    LignePrevisionMeteo info = new LignePrevisionMeteo(getIcone(node.GetAttribute("code", ns.DefaultNamespace)),
                                            node.GetAttribute("low", ns.DefaultNamespace),
                                            node.GetAttribute("high", ns.DefaultNamespace),
                                            node.GetAttribute("text", ns.DefaultNamespace),
                                            node.GetAttribute("day", ns.DefaultNamespace)
                                        );
                    _lignes.Add(info);
                }
            }

            donneesPretes = true;
        }

        /// <summary>
        /// Traduire une heure HH.MM PM en heure 24
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string TraduireHeure(string p)
        {
            try
            {
                string[] morceaux = p.Split(new Char[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (morceaux.Length < 3)
                    return p;

                int heure = Int32.Parse(morceaux[0]);
                int minute = Int32.Parse(morceaux[1]);
                if (morceaux[2].ToLower().Equals("pm"))
                    heure += 12;

                return heure + "h" + minute;
            }
            catch (Exception)
            {
                return p;
            }
        }

        /// <summary>
        /// Retourne le pourcentage present de validite des previsions, en fonction de la longueur de
        /// validite donnee avec la reponse de yahoo
        /// </summary>
        /// <returns></returns>
        public float validitePassee()
        {
            DateTime now = DateTime.Now;
            double dureeTotale = _finPrevisions.Subtract(_datePrevisions).TotalSeconds ;
            double dureeActuelle = _finPrevisions.Subtract(now).TotalSeconds;

            return (float)dureeActuelle / (float)dureeTotale;
        }

        public bool MustRefresh(Temps maintenant)
        {
            if (!donneesPretes)
                return false;

            return maintenant._temps > _finPrevisions;
        }

        /***
         * 
         */
        /// <summary>
        /// Retrouve l'icone correspond au code de condition meteo
        /// </summary>
        /// <param name="p">Code Yahoo, voir </param>
        /// <returns></returns>
        private Bitmap getIcone(string p)
        {
            int Code;
            try
            {
                Code = Int32.Parse(p);
            }
            catch (Exception)
            {
                return Resources.Meteo_44; // Indeterminee
            }

            switch (Code)
            {
                case 0: //tornado
                    return Resources.Meteo_44; // Indeterminee
                case 1: // 	tropical storm
                    return Resources.Meteo_00;
                case 2: // 	hurricane
                    return Resources.Meteo_00;
                case 3: // 	severe thunderstorms
                    return Resources.Meteo_03;
                case 4: // 	thunderstorms
                    return Resources.Meteo_04;
                case 5: // 	mixed rain and snow
                    return Resources.Meteo_05;
                case 6: // 	mixed rain and sleet
                    return Resources.Meteo_06;
                case 7: // 	mixed snow and sleet
                    return Resources.Meteo_07;
                case 8: // 	freezing drizzle
                    return Resources.Meteo_08;
                case 9: // 	drizzle
                    return Resources.Meteo_09;

                case 10: // 	freezing rain
                    return Resources.Meteo_10;

                case 11: // 	showers
                    return Resources.Meteo_11;

                case 12: // 	showers
                    return Resources.Meteo_12;

                case 13: // 	snow flurries
                    return Resources.Meteo_13;

                case 14: // 	light snow showers
                    return Resources.Meteo_14;

                case 15: // 	blowing snow
                    return Resources.Meteo_15;

                case 16: // 	snow
                    return Resources.Meteo_16;

                case 17: // 	hail
                    return Resources.Meteo_17;

                case 18: // 	sleet
                    return Resources.Meteo_18;

                case 19: // 	dust
                    return Resources.Meteo_19;

                case 20: // 	foggy
                    return Resources.Meteo_20;

                case 21: // 	haze
                    return Resources.Meteo_20;

                case 22: // 	smoky
                    return Resources.Meteo_20;

                case 23: // 	blustery
                    return Resources.Meteo_23;

                case 24: // 	windy
                    return Resources.Meteo_24;

                case 25: // 	cold
                    return Resources.Meteo_25;
                case 26: // 	cloudy
                    return Resources.Meteo_26;
                case 27: // 	mostly cloudy (night)
                    return Resources.Meteo_27;
                case 28: // 	mostly cloudy (day)
                    return Resources.Meteo_28;
                case 29: // 	partly cloudy (night)
                    return Resources.Meteo_29;
                case 30: // 	partly cloudy (day)
                    return Resources.Meteo_30;
                case 31: // 	clear (night)
                    return Resources.Meteo_31;
                case 32: // 	sunny
                    return Resources.Meteo_32;
                case 33: // 	fair (night)
                    return Resources.Meteo_33;
                case 34: // 	fair (day)
                    return Resources.Meteo_34;
                case 35: // 	mixed rain and hail
                    return Resources.Meteo_35;
                case 36: // 	hot
                    return Resources.Meteo_36;
                case 37: // 	isolated thunderstorms
                    return Resources.Meteo_37;
                case 38: // 	scattered thunderstorms
                    return Resources.Meteo_38;
                case 39: // 	scattered thunderstorms
                    return Resources.Meteo_39;
                case 40: // 	scattered showers
                    return Resources.Meteo_40;
                case 41: // 	heavy snow
                    return Resources.Meteo_41;
                case 42: // 	scattered snow showers
                    return Resources.Meteo_42;
                case 43: // 	heavy snow
                    return Resources.Meteo_43;
                case 44: // 	partly cloudy
                    return Resources.Meteo_44;
                case 45: // 	thundershowers
                    return Resources.Meteo_45;
                case 46: // 	snow showers
                    return Resources.Meteo_46;
                case 47: // 	isolated thundershowers
                    return Resources.Meteo_47;
                default:
                    return Resources.Meteo_44; // Indeterminee
            }
        }
    }
}
