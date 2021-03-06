﻿////
//// Gestion de la liste des actualites avec chargement des flux RSS en tache de fond
using ClockScreenSaverGL.Config;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;
using System.Xml;

namespace ClockScreenSaverGL.DisplayedObjects.PanneauActualites
{
    class ActuFactory : IDisposable
    {
        private List<string> _sourcesActualite;
        bool _continuerThread;
        Thread _thread;
        private static readonly string RFC822 = "ddd, dd MMM yyyy HH:mm:ss zzz";
        public List<LigneActu> _lignes = new List<LigneActu>();
        private static readonly char[] SEPARATEURS = { '|' };

        public ActuFactory()
        {
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
        /// Lecture de la liste des sources RSS d'actualite dans le fichier {install}/{donnees}/actualites.txt
        /// </summary>
        private void LireFichierSources()
        {
            _sourcesActualite = new List<string>();
            string fichierSources = Path.Combine(Configuration.getDataDirectory(), "actualites.txt");

            // Lire le fichier des sources d'actualite
            StreamReader file = new StreamReader(fichierSources);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                if (!line.StartsWith("#"))  // Ligne mise en commentaire
                    _sourcesActualite.Add(line);
            }

            file.Close();
            // Melanger les sources
            /*
            int DeuxiemeIndice;
            for (int i = 1; i < _sourcesActualite.Count; i++)
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
            */
        }

        /// <summary>
        /// Lecture des flux RSS en tache de fond
        /// </summary>
        private void LireSources()
        {
            // Lecture de la liste des url
            LireFichierSources();

            WebRequest.DefaultCachePolicy = new RequestCachePolicy(RequestCacheLevel.Revalidate);
            int sourceALire = Actualites.SourceCourante();

            // repete tant qu'on signale pas la fin de ce thread
            while (_continuerThread && _sourcesActualite.Count < Actualites.MAX_LIGNES)
            {
                sourceALire++;
                if (sourceALire >= _sourcesActualite.Count)
                    sourceALire = 0;

                // Lire une source
                LitRSS(_sourcesActualite[sourceALire]);

                // Attendre un petit peu
                for ( int i =0; i < 10 && _continuerThread; i++ )
                {
                    Thread.Sleep(100);
                }
            }
            Actualites.SourceCourante(sourceALire);

            // Ce thread est fini
            _thread = null;
        }

        /// <summary>
        /// Lit un flux RSS et ajoute les objets LigneActu
        /// </summary>
        /// <param name="source">URL du flux RSS</param>
        private void LitRSS(string source)
        {
            string[] tokens = source.Split(SEPARATEURS);
            if (tokens == null || tokens.Length < 2)
                return;

            String source_a_charger = tokens[0];
            String url_a_charger = tokens[1];
            if (source_a_charger == null || url_a_charger == null)
                return;

            try
            {
                XmlDocument RSSXml = new XmlDocument();
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
                    if (!_continuerThread)
                        return;

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
                            Image image = Actualites.AFFICHE_IMAGES ? chargeBitmap(RSSNode) : null;
                            ajoute(new LigneActu(source_a_charger, title, date, desc, image));

                            nbLignesPourCetteSource++;
                            if (nbLignesPourCetteSource > Actualites.MAX_LIGNES_PAR_SOURCE)
                                return;

                            if (_lignes.Count >= Actualites.MAX_LIGNES)
                                return;
                        }

                        if (!_continuerThread)
                            return;
                    }
                }
            }
            catch (Exception )
            {
                //_lignes.Add(new LigneActu(url_a_charger, "Impossible de charger les informations", DateTime.Now, e.Message, null));
            }
        }

        /// <summary>
        /// Ajoute une ligne d'actualite
        /// </summary>
        /// <param name="ligneActu"></param>
        private void ajoute(LigneActu ligneActu)
        {
            lock (_lignes)
            {
                // Peu d'actualites: on ajoute a la fin
                if (_lignes.Count < 3)
                    _lignes.Add(ligneActu);
                else
                {
                    // Ajoute au hasard pour melanger les actualites de differentes sources
                    int indice = DisplayedObject.r.Next(Actualites._derniereAffichee, _lignes.Count);
                    _lignes.Insert(indice, ligneActu);
                }
            }
        }

        /// <summary>
        /// Charge une bitmap a partir d'un element d'actualite RSS, si possible
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
                Image i = retailleImage(Bitmap.FromStream(stream));
                return DisplayedObject.BitmapDesaturee((Bitmap)i, Actualites.SATURATION_IMAGES);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Retaille l'image pour qu'elle tienne dans le bandeau d'affichage
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private Image retailleImage(Image image)
        {
            int nouvelleHauteur = (int)(Actualites.HAUTEUR_BANDEAU - (Actualites.TAILLE_SOURCE + Actualites.TAILLE_TITRE) * 0.9);
            int nouvelleLargeur = (int)(image.Width * ((float)nouvelleHauteur / (float)image.Height));

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

        /// <summary>
        /// Converti une date RSS en date C#
        /// </summary>
        /// <param name="RSSDate"></param>
        /// <returns></returns>
        private static DateTime RSSDateToDateTime(string RSSDate)
        {
            try
            {
                return DateTime.ParseExact(RSSDate, RFC822, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
            }
            catch (Exception)
            {

                return DateTime.MinValue;
            }
        }

        public void Dispose()
        {
            _continuerThread = false;
            //_thread?.Abort();

            lock (_lignes)
            {
                foreach (LigneActu l in _lignes)
                    l.Dispose();

                _lignes.Clear();
            }
        }
    }
}
