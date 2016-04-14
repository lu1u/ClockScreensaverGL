using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace ClockScreenSaverGL.DisplayedObjects.PanneauActualites
{
    class Actualites : DisplayedObject, IDisposable
    {
        public const string CAT = "Actualites";

        private static readonly int HAUTEUR_BANDEAU = conf.getParametre(CAT, "Hauteur bandeau",150);
        private static readonly float VITESSE = conf.getParametre(CAT, "Vitesse", 75.0f);
        private static readonly int MIN_LIGNES = conf.getParametre(CAT, "Nb lignes min", 50);
        private static readonly int MAX_LIGNES = conf.getParametre(CAT, "Nb lignes max", 100);
        private static readonly int TAILLE_SOURCE = conf.getParametre(CAT, "Taille fonte source", 16);
        private static readonly int TAILLE_TITRE = conf.getParametre(CAT, "Taille fonte titre", 30);
        private static readonly int TAILLE_DESCRIPTION = conf.getParametre(CAT, "Taille fonte source", 14);
        private static bool AFFICHE_DESCRIPTION = conf.getParametre(CAT, "Affiche Description", true);
        private int derniereSource = conf.getParametre(CAT, "Derniere Source", 0);

        private float _decalageX = SystemInformation.VirtualScreen.Width;
        private int _derniereAffichee;
        public readonly Font _fonteSource = new Font(FontFamily.GenericSansSerif, TAILLE_SOURCE, FontStyle.Italic);
        public readonly Font _fonteTitre = new Font(FontFamily.GenericSansSerif, TAILLE_TITRE, FontStyle.Bold);
        public readonly Font _fonteDescription = new Font(FontFamily.GenericSansSerif, TAILLE_DESCRIPTION, FontStyle.Regular);
        List<LigneActu> _lignes = new List<LigneActu>();
        private string _source_a_charger;
        private string _url_a_charger;

        private List<string> _sourcesActualite = new List<string>();
        char[] SEPARATEURS = { '|' };

        public Actualites(OpenGL gl) : base(gl)
        {
            string fichierSources = Path.Combine(Config.getDataDirectory(), "actualites.txt");
            StreamReader file = new StreamReader(fichierSources);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                if (! line.StartsWith("#"))
                    _sourcesActualite.Add(line);
            }

            file.Close();
        }

        public override void Dispose()
        {
            base.Dispose();
            _fonteSource.Dispose();
            _fonteTitre.Dispose();
            _fonteDescription.Dispose();
        }

        /// <summary>
        /// Affichage deroulant des actualites
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleur"></param>
        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D(0, tailleEcran.Width, 0, tailleEcran.Height);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Color(0.1f, 0.1f, 0.1f, 0.55f);

            gl.Rect(tailleEcran.Left, tailleEcran.Top + HAUTEUR_BANDEAU, tailleEcran.Right, tailleEcran.Top);

            float x = tailleEcran.Left + _decalageX;
            _derniereAffichee = 1;
            lock (_lignes)
            foreach (LigneActu l in _lignes)
                {
                    l.affiche(gl, x, tailleEcran.Top + HAUTEUR_BANDEAU, _fonteSource, _fonteTitre, _fonteDescription, couleur, AFFICHE_DESCRIPTION);
                    x += l.largeur;
                    _derniereAffichee++;
                    if (x > tailleEcran.Right)
                        break;
                }

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();

            Console c = Console.getInstance(gl);
            c.AddLigne(Color.Green, "Decalage" + _decalageX.ToString("f2"));
            c.AddLigne(Color.Green, "Nb Lignes " + _lignes.Count);
            c.AddLigne(Color.Green, _sourcesActualite[derniereSource]);
        }

        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
            if (_lignes.Count < MIN_LIGNES)
                GetNextLigne();

            _decalageX -= VITESSE * maintenant._intervalle;

            lock (_lignes)
            {
                if (_lignes.Count > 1)
                    if (_decalageX + _lignes[0].largeur < 0)
                    {
                        // Suppression de la premiere annonce qui ne sera plus affichee
                        _decalageX += _lignes[0].largeur;
                        _lignes[0].Dispose();
                        _lignes.RemoveAt(0);
                    }
            }
        }

        /// <summary>
        /// Retrouve de nouvelles lignes d'information
        /// </summary>
        private void GetNextLigne()
        {
            derniereSource++;
            if (derniereSource >= _sourcesActualite.Count)
                derniereSource = 0;

            conf.setParametre(CAT, "Derniere Source", derniereSource);
            conf.flush(CAT);

            string[] tokens = _sourcesActualite[derniereSource].Split(SEPARATEURS);
            _source_a_charger = tokens[0];
            _url_a_charger = tokens[1];
            new Thread(new ThreadStart(GetInfos)).Start();
        }

        private void GetInfos()
        {
            XmlDocument RSSXml = new XmlDocument();
            try
            {
                RSSXml.Load(_url_a_charger);

                XmlNodeList RSSNodeList = RSSXml.SelectNodes("rss/channel/item");
                StringBuilder sb = new StringBuilder();
                foreach (XmlNode RSSNode in RSSNodeList)
                {
                    XmlNode RSSSubNode;
                    RSSSubNode = RSSNode.SelectSingleNode("title");
                    string title = RSSSubNode != null ? RSSSubNode.InnerText : "";
                    RSSSubNode = RSSNode.SelectSingleNode("description");
                    string desc = RSSSubNode != null ? RSSSubNode.InnerText : "";
                    lock (_lignes)
                    {
                        if (!Existe(title))
                        {
                            if (_lignes.Count < 2)
                                _lignes.Add(new LigneActu(_source_a_charger, title, desc));
                            else
                            {
                                int indice = r.Next(_derniereAffichee, _lignes.Count);
                                _lignes.Insert(indice, new LigneActu(_source_a_charger, title, desc));
                            }                            
                        }
                    }

                    if (_lignes.Count >= MAX_LIGNES)
                        break;
                }
            }
            catch (Exception )
            {
                //_lignes.Add(new LigneActu(_source_a_charger, "Impossible de charger les informations", e.Message));
            }
        }

        /// <summary>
        /// Determine si une actualite existe deja dans la liste
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private bool Existe(string title)
        {
            foreach (LigneActu la in _lignes)
                if (title.Equals(la.titre))
                    return true;
            return false;
        }

        public override bool KeyDown(Form f, Keys k)
        {
            if (Keys.J.Equals(k))
            {
                _lignes.RemoveAt(0);
                return true;
            }
            if ( Keys.E.Equals(k))
            {
                _lignes.Clear();
                AFFICHE_DESCRIPTION = !AFFICHE_DESCRIPTION;
                conf.setParametre(CAT, "Affiche Description", AFFICHE_DESCRIPTION);
                conf.flush(CAT);
                return true;
            }

            return base.KeyDown(f, k);
        }
    }
}
