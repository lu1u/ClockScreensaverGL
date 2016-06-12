///
/// Exploite l'API Yahoo Weather
/// https://developer.yahoo.com/weather/documentation.html  (annulee par yahoo depuis mars 2016)
/// http://www.meteofrance.com/previsions-meteo-france/crolles/38920
/// 
// 

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ClockScreenSaverGL.DisplayedObjects.Meteo
{
    class MeteoInfo : IDisposable
    {
        #region MEMBRES_PUBLICS
        const int NB_JOURS_PREVISIONS = 4;
        public bool _donneesPretes;

        public List<LignePrevisionMeteo> _lignes = new List<LignePrevisionMeteo>();
        public string _title;
        public bool _hasNewInfo;
        #endregion MEMBRES_PUBLICS

        private string _url;
        private DateTime _datePrevisions;
        private DateTime _finPrevisions;
        WebBrowser _wb;
        static Dictionary<string, string> _liensIcones = new Dictionary<string, string>();

        public MeteoInfo(string url)
        {
            _donneesPretes = false;
            _hasNewInfo = false;

            // MyMeteo.fr
            //_url = @"http://www.my-meteo.fr/previsions+meteo+france/crolles+12+jours.html";
            _url = @"http://www.meteofrance.com/previsions-meteo-france/crolles/38920";
            _title = "Crolles - http://www.meteofrance.com";

            LitCorrespondancesMeteo();
            ChargeDonnees();
        }

        /// <summary>
        /// Lecture de la table de correspondance qui fait le lien entre les nom utilises sur le site meteo
        /// et les icones utilisees par le programme
        /// </summary>
        private static void LitCorrespondancesMeteo()
        {
            _liensIcones.Clear();
            string fichierSources = Path.Combine(Config.getDataDirectory(), "icones meteo.txt");
            // Lire le fichier des sources d'actualite
            StreamReader file = new StreamReader(fichierSources);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                if (!line.StartsWith("#")) // Commentaire a ignorer ?
                {
                    string[] tokens = line.Split('>');
                    if (tokens?.Length == 2)
                        _liensIcones.Add(tokens[0], tokens[1]);
                }
            }

            file.Close();
        }

        /// <summary>
        /// Retrouve le nom de l'image a utiliser par ce programme en fonction de l'information
        /// trouvee sur la page du site meteo
        /// </summary>
        /// <param name="imageSurLeSite"></param>
        /// <returns></returns>
        public static string getIcone(string imageSurLeSite)
        {
            string valeur ;
            if (_liensIcones.TryGetValue(imageSurLeSite, out valeur))
                return valeur;

            return imageSurLeSite;
        }

        public void Dispose()
        {
            if (_lignes != null)
                foreach (LignePrevisionMeteo l in _lignes)
                    l.Dispose();
            _wb.Dispose();
        }

        /// <summary>
        /// Lecture des previsions meteo en multithreading
        /// </summary>
        public void ChargeDonnees()
        {
            try
            {
                if (_wb == null)
                {
                    _wb = new WebBrowser();
                    _wb.DocumentCompleted += onDocumentCompleted;

                    _wb.ScriptErrorsSuppressed = true;
                    _wb.Navigate(_url);
                }
            }
            catch (Exception)
            {
            }
            #region YAHOO_WEATHER // Obsolete
            /*
            // Create a new XmlDocument  
            try
            {
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
            _hasNewInfo = true;
            }
            catch (Exception)
            {
                donneesPretes = false;
                _hasNewInfo = false;             
            }
            */
            #endregion
        }

        /// <summary>
        /// Page meteo recue: l'interpreter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="wex"></param>
        private void onDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs wex)
        {
            if (_hasNewInfo)
                return;

            _datePrevisions = DateTime.Now;
            _finPrevisions = _datePrevisions.AddHours(2);
            _lignes.Clear();

            var doc = _wb.Document;
            if (doc == null)
                return;

            // meteofrance.com
            HtmlElement prev = doc.GetElementById("seven-days");
            if (prev == null)
                return;

            HtmlElementCollection div = prev.GetElementsByTagName("div");
            try
            {
                foreach (HtmlElement e in div)
                {
                    String classe = e.GetAttribute("className");
                    if ("group-days-summary".Equals(classe))
                    {
                        ParseGroupDays(e);

                    }

                }
            }
            catch (Exception)
            {

            }
            /* my-meteo.fr
            _title = "Crolles my-meteo.fr";
            _datePrevisions = DateTime.Now;
            _finPrevisions = _datePrevisions.AddDays(1);
            _lignes.Clear();

            HtmlElement colprev = doc.GetElementById("col_previsions");
            if (colprev == null)
                return;

            try
            {
                foreach (HtmlElement e in colprev.All)
                {
                    string classe = e.GetAttribute("className");
                    if (classe.StartsWith("item"))
                    {
                        HtmlElementCollection souselements = e.GetElementsByTagName("span");
                        DecodeLigne(souselements);

                        if (_lignes.Count > 4)
                            break;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            */
            _hasNewInfo = true;
            _donneesPretes = true;
            _wb = null;
        }

        private void ParseGroupDays(HtmlElement e)
        {
            HtmlElementCollection articles = e.GetElementsByTagName("article");
            foreach (HtmlElement article in articles)
            {
                String date = "?";
                String temperature = "?";
                String icone = "?";
                String texte = "?";
                String vent = "?";
                String pluie = "?";
                foreach (HtmlElement el in article.Children)
                {
                    String tagName = el.TagName?.ToLower();

                    if ("header".Equals(tagName))
                    {
                        // Header: date
                        date = el.InnerText;
                    }
                    else
                        if ("ul".Equals(tagName))
                    {
                        // Boucle dans les <LI>
                        foreach (HtmlElement li in el.Children)
                        {
                            String classe = li.GetAttribute("className");

                            // temperature
                            if ("day-summary-temperature".Equals(classe))
                            {
                                HtmlElementCollection spans = li.GetElementsByTagName("SPAN");
                                if (spans != null)
                                {
                                    temperature = filtreMin(spans[0]?.InnerText) + ", " + filtreMax(spans[1]?.InnerText);
                                }
                            }

                            // image et etxte de prevision
                            if ("day-summary-image".Equals(classe))
                            {
                                HtmlElementCollection spans = li.GetElementsByTagName("SPAN");
                                if (spans != null)
                                {
                                    icone = getIcone(spans[0]?.GetAttribute("className"));
                                    texte = spans[0]?.InnerText;
                                }
                            }

                            // Vent
                            if ("day-summary-wind".Equals(classe))
                            {
                                HtmlElementCollection P = li.GetElementsByTagName("P");
                                if (P != null)
                                {
                                    vent = P[0]?.InnerText + ' ' + filtreVent(P[1]?.InnerText);
                                }
                            }
                        }
                    }


                }
                _lignes.Add(new LignePrevisionMeteo(icone, date, temperature, texte, vent, pluie));
                if (_lignes.Count >= PanneauInfos.NB_LIGNES_INFO_MAX)
                    return;
            }
        }

        private string filtreMax(string texte)
        {
            return texte.Replace("Maximale", "Max");
        }

        private string filtreMin(string texte)
        {
            return texte.Replace("Minimale", "Min");
        }

        private string filtreVent(string texte)
        {
            return texte.Replace("Vent ", "");
        }
        /// <summary>
        /// Calcule le nom de l'image a afficher en fonction du nom donne par meteo france
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        /*private string decodeImage(string mf)
        {
            if ("picTemps J_W1_0-N_0".Equals(mf))
                return "ensoleille";
            if ("picTemps J_W1_0-N_1".Equals(mf))
                return "eclaircies";
            if ("picTemps J_W1_0-N_2".Equals(mf))
                return "eclaircies";
            if ("picTemps J_W1_0-N_3".Equals(mf))
                return "tres_nuageux";
            if ("picTemps J_W1_0-N_5".Equals(mf))
                return "ciel_voile";

            if ("picTemps J_W1_18-N_1".Equals(mf))
                return "rares_averses";
            if ("picTemps J_W1_18-N_2".Equals(mf))
                return "rares_averses";
            if ("picTemps J_W1_12-N_3".Equals(mf))
                return "orages";
            if ("picTemps J_W1_32-N_2".Equals(mf))
                return "averses_orageuses";
            if ("picTemps J_W1_25-N_3".Equals(mf))
                return "orages";
            if ("picTemps J_W1_32-N_4".Equals(mf))
                return "averses_orageuses";

            if ("picTemps J_W2_18".Equals(mf))
                return "risques_orages";
            if ("picTemps N_W2_8".Equals(mf))
                return "pluie";

            return mf;
        }*/

        /*
        /// <summary>
        /// Decode une ligne du site my meteo
        /// </summary>
        /// <param name="elems"></param>
        private void DecodeLigne(HtmlElementCollection elems)
        {
            try
            {
                string icone = "inconnu";
                string date = "?";
                string tmin = "?";
                string tmax = "?";
                string temps = "?";
                string vent = "?";
                string pluie = "?";
                foreach (HtmlElement el in elems)
                {
                    string cl = el.GetAttribute("className");
                    if ("icone-meteo grand p1".Equals(cl))
                        icone = el.Style;
                    else
                    if ("date".Equals(cl))
                        date = el.InnerText;
                    else
                        if ("t_min".Equals(cl))
                        tmin = el.InnerText;
                    else
                    if ("t_max".Equals(cl))
                        tmax = el.InnerText;
                    else
                    if ("temps".Equals(cl))
                        temps = el.InnerText;
                    else
                                if ("vent".Equals(cl))
                        vent = el.InnerText;
                    else
                                if ("pluie".Equals(cl))
                        pluie = el.InnerText;
                }

                _lignes.Add(new LignePrevisionMeteo(icone, date, ToTemp(tmin, tmax), temps, vent, pluie));
            }
            catch (Exception e)
            {
                _lignes.Add(new LignePrevisionMeteo("", e.Message, "", "", "", ""));
            }
        }

        private string ToTemp(string tmin, string tmax)
        {
            return tmin + "°/" + tmax + "°";
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
        }*/

        /// <summary>
        /// Retourne le pourcentage present de validite des previsions, en fonction de la longueur de
        /// validite donnee avec la reponse de yahoo
        /// </summary>
        /// <returns></returns>
        public float validitePassee()
        {
            DateTime now = DateTime.Now;
            double dureeTotale = _finPrevisions.Subtract(_datePrevisions).TotalSeconds;
            double dureeActuelle = _finPrevisions.Subtract(now).TotalSeconds;

            return (float)dureeActuelle / (float)dureeTotale;
        }

        public bool MustRefresh(Temps maintenant)
        {
            if (!_donneesPretes)
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
        /*
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
        */
        internal bool HasNewInfo()
        {
            if (_hasNewInfo)
            {
                _hasNewInfo = false;
                return true;
            }
            else
                return false;
        }
    }
}
