////
//// Gestion de la liste des actualites avec chargement des flux RSS en tache de fond
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;
using System.Xml;

namespace ClockScreenSaverGL.DisplayedObjects.PanneauActualites
{
    class ActuFactory : IDisposable
    {
        private List<string> _sourcesActualite = new List<string>();
        bool _continuerThread;
        Thread _thread;
        private static readonly string RFC822 = "ddd, dd MMM yyyy HH:mm:ss zzz";
        List<LigneActu> _lignes = new List<LigneActu>();
        private static readonly char[] SEPARATEURS = { '|' };

        public ActuFactory()
        {
            // Lecture de la liste des url
            LitSourcesActu();
            
            // Lancement du thread qui va lire les actualites en tache de fond
            LanceTacheDeFond();
        }

        /// <summary>
        /// Lancement du thread qui va lire les actualites en tache de fond
        /// </summary>
        private void LanceTacheDeFond()
        {
            _continuerThread = true;
            _thread = new Thread(new ThreadStart(LireSources));
            _thread.Name = "ActuFactory";
            _thread.Start();
        }

        /// <summary>
        /// Lecture de la liste des sources RSS d'actualite
        /// </summary>
        private void LitSourcesActu()
        {
            string fichierSources = Path.Combine(Config.getDataDirectory(), "actualites.txt");


            // Lire le fichier des sources d'actualite
            StreamReader file = new StreamReader(fichierSources);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                if (!line.StartsWith("#"))
                    _sourcesActualite.Add(line);
            }

            // Melanger
            int DeuxiemeIndice;
            for (int i = 0; i < _sourcesActualite.Count; i++)
            {
                do
                {
                    DeuxiemeIndice = DisplayedObject.r.Next(0, _sourcesActualite.Count);
                }
                while (DeuxiemeIndice == i);

                string temp = _sourcesActualite[i];
                _sourcesActualite[i] = _sourcesActualite[DeuxiemeIndice];
                _sourcesActualite[DeuxiemeIndice] = temp;
            }

            file.Close();
        }

        /// <summary>
        /// Lecture des flux RSS en tache de fond
        /// </summary>
        private void LireSources()
        {
            WebRequest.DefaultCachePolicy = new RequestCachePolicy(RequestCacheLevel.Revalidate);

            while (_continuerThread)
            {
                int sourceALire = Actualites.SourceCourante();
                sourceALire++;
                if (sourceALire >= _sourcesActualite.Count)
                    sourceALire = 0;
                Actualites.SourceCourante(sourceALire);

                // Lire une source
                LitRSS(_sourcesActualite[sourceALire]);

                // Attendre un petit peu
                do
                {
                    Thread.Sleep(500);
                }
                while (_continuerThread && _sourcesActualite.Count >= Actualites.MAX_LIGNES);
            }
        }

        /// <summary>
        /// Lit un flux RSS
        /// </summary>
        /// <param name="source">URL du flux RSS</param>
        private void LitRSS(string source)
        {
            try
            {
                string[] tokens = source.Split(SEPARATEURS);
                String source_a_charger = tokens[0];
                String url_a_charger = tokens[1];
                XmlDocument RSSXml = new XmlDocument();
                XmlDocument xmlDocument = new XmlDocument();
                RSSXml.Load(url_a_charger);
                if (!_continuerThread)
                    return;

                XmlNodeList RSSNodeList = RSSXml.SelectNodes("rss/channel/item");
                StringBuilder sb = new StringBuilder();
                int nbLignesPourCetteSource = 0;
                foreach (XmlNode RSSNode in RSSNodeList)
                {
                    XmlNode RSSSubNode;
                    RSSSubNode = RSSNode.SelectSingleNode("pubDate");

                    if (recent(RSSSubNode))
                    {
                        // L'article n'est pas trop vieux pour etre affiche
                        DateTime date = RSSDateToDateTime(RSSSubNode.InnerText);
                        RSSSubNode = RSSNode.SelectSingleNode("title");
                        string title = RSSSubNode != null ? RSSSubNode.InnerText : "";
                        RSSSubNode = RSSNode.SelectSingleNode("description");
                        string desc = RSSSubNode != null ? RSSSubNode.InnerText : "";

                        if (!existeDeja(title))
                        {
                            Image image = Actualites.AFFICHE_IMAGES ?  chargeBitmap(RSSNode) : null ; 
                            ajoute(new LigneActu(source_a_charger, title, date, desc, image));

                            nbLignesPourCetteSource++;
                            if (nbLignesPourCetteSource > Actualites.MAX_LIGNES_PAR_SOURCE)
                                return;

                            if (_lignes.Count >= Actualites.MAX_LIGNES)
                                return;
                        }
                    }
                    
                    if (!_continuerThread)
                        return;
                }
            }
            catch (Exception)
            {
                //_lignes.Add(new LigneActu(_source_a_charger, "Impossible de charger les informations", e.Message));
            }

            // Ce thread est fini
            _thread = null;
        }

        private void ajoute(LigneActu ligneActu)
        {
            lock (_lignes)
            {
                if (_lignes.Count < 2)
                    _lignes.Add(ligneActu);
                else
                {
                    int indice = DisplayedObject.r.Next(Actualites._derniereAffichee, _lignes.Count);
                    _lignes.Insert(indice, ligneActu);
                }
            }
        }

        /// <summary>
        /// Charge une bitmap a partir d'une URL
        /// </summary>
        /// <param name="innerText"></param>
        /// <returns></returns>
        private Image chargeBitmap(XmlNode Node)
        {
            try
            {
                XmlNode n = Node.SelectSingleNode("enclosure ");
                if (n == null)
                    return null;

                string URL = n.Attributes["url"].InnerText;
                if (URL == null)
                    return null;

                if (URL.Length == 0)
                    return null;

                var request = WebRequest.Create(URL);
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                Image i = retaille( Bitmap.FromStream(stream));
                return DisplayedObject.BitmapNiveauDeGris( (Bitmap)i );
            }
            catch( Exception )
            {
                return null;
            }
        }

        /// <summary>
        /// Retaille l'image pour qu'elle tienne dans le bandeau d'affichage
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private Image retaille(Image image)
        {
            int hauteur = image.Height;
            //if (hauteur < Actualites.HAUTEUR_BANDEAU - (Actualites.TAILLE_SOURCE + Actualites.TAILLE_TITRE) * 0.9f)
            //    return image;

            int nouvelleHauteur = (int)(Actualites.HAUTEUR_BANDEAU - (Actualites.TAILLE_SOURCE + Actualites.TAILLE_TITRE) * 0.9);
            int nouvelleLargeur = (int)(image.Width * ((float)nouvelleHauteur / (float)hauteur));

            var destRect = new Rectangle(0, 0, nouvelleLargeur, nouvelleHauteur);
            var destImage = new Bitmap(nouvelleLargeur, nouvelleHauteur);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
            }

            return destImage;
        }




        /// <summary>
        /// Determine si une actualite existe deja dans la liste
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private bool existeDeja(string title)
        {
            title = LigneActu.nettoieXML(title);
            foreach (LigneActu la in _lignes)
                if (title.Equals(la._titre))
                    return true;
            return false;
        }
        /// <summary>
        /// Retourne true si l'article est suffisament recent pour etre pris en compte
        /// </summary>
        /// <param name="RSSSubNode"></param>
        /// <returns></returns>
        private static bool recent(XmlNode RSSSubNode)
        {
            if (RSSSubNode == null)
                return true; // Pas de date

            DateTime rss = RSSDateToDateTime(RSSSubNode.InnerText);
            if (rss == null)
                return true; // Date non interpretable

            return DateTime.Now.Subtract(rss).Days < Actualites.NB_JOURS_MAX_INFO;
        }

        public List<LigneActu> getLignes()
        {
            return _lignes;
        }
        /// <summary>
        /// Converti une date RSS en date C#
        /// </summary>
        /// <param name="RSSDate"></param>
        /// <returns></returns>
        private static DateTime RSSDateToDateTime(string RSSDate)
        {
            try
            {
                return DateTime.ParseExact(RSSDate, RFC822,
                                                       DateTimeFormatInfo.InvariantInfo,
                                                       DateTimeStyles.None);
            }
            catch (Exception)
            {

                return DateTime.MinValue;
            }
        }
        public void Dispose()
        {
            _continuerThread = false;
            _thread.Abort();
            foreach (LigneActu l in _lignes)
                l.Dispose();

            _lignes.Clear();
        }
    }
}
