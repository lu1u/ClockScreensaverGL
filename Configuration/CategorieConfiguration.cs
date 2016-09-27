///
/// Categorie de configuration ( = un fichier de conf, = un objet affiche)
///

using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ClockScreenSaverGL.Config
{
    public class CategorieConfiguration : IDisposable
    {
        private const string DEBUT_COMMENTAIRE = "#";
        public const string EXTENSION_CONF = ".conf";
        string _fileName, _nom;
        private SortedDictionary<string, Parametre> _valeurs = new SortedDictionary<string, Parametre>();
        private bool _propre = true;
        public delegate void ParametreChange(string nom);
        private ParametreChange _parametreChange;
        private string keyCourante;         // La valeur courante, modifiee interactivement

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="nom">Nom de la categorie</param>
        public CategorieConfiguration(string nom)
        {
            _nom = nom;
            _fileName = getFileName(nom);
            LireFichier(_fileName);
        }

        ~CategorieConfiguration()
        {
            flush();
        }

        /// <summary>
        /// Change le 'listener', delegate qui va etre appele en cas de changement
        /// interactif d'un parametre
        /// </summary>
        /// <param name="p"></param>
        public void setListenerParametreChange(ParametreChange p)
        {
            _parametreChange = p;
        }

        /// <summary>
        /// Obtient le nom de fichier pour cette categorie
        /// </summary>
        /// <param name="nom"></param>
        /// <returns></returns>
        public static string getFileName(string nom)
        {
            return Path.Combine(Configuration.getRepertoire(), nom + EXTENSION_CONF);
        }

        /// <summary>
        /// Lit les parametres dans le fichier de la categorie (un par ligne)
        /// </summary>
        /// <param name="filename"></param>
        private void LireFichier(string filename)
        {
            try
            {
                // La remplir a partir du contenu du fichier
                using (StreamReader file = new System.IO.StreamReader(filename))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                        if ((line.Length > 0) && (!line.StartsWith(DEBUT_COMMENTAIRE))) // Commentaire
                        {
                            Parametre parametre = new Parametre(line);
                            _valeurs.Add(parametre.nom, parametre);
                        }
                }
            }
            catch (FileNotFoundException)
            {
                // C'est normal: pas encore de fichier de configuration (premier lancement)
            }
            catch (DirectoryNotFoundException)
            {
                // C'est normal: par encore de fichier de configuration (premier lancement)
            }
            catch (Exception e)
            {
                // Erreur inconnue
                throw e;
            }

        }

        #region setParametre
        /// <summary>
        /// Changement d'un parametre
        /// </summary>
        /// <param name="valueName"></param>
        /// <param name="type"></param>
        /// <param name="defaut"></param>
        private void setParametre(string valueName, Parametre.TYPE_PARAMETRE type, Object defaut)
        {
            if (_valeurs.ContainsKey(valueName))
                _valeurs.Remove(valueName);

            Parametre par = new Parametre(valueName, type, defaut, true);
            _valeurs.Add(valueName, par);

            _propre = false;
            _parametreChange?.Invoke(valueName);
        }


        public void setParametre(string nom, bool valeur)
        {
            setParametre(nom, Parametre.TYPE_PARAMETRE.T_BOOL, valeur);
        }
        public void setParametre(string nom, float valeur)
        {
            setParametre(nom, Parametre.TYPE_PARAMETRE.T_FLOAT, valeur);
        }

        public void setParametre(string nom, int valeur)
        {
            setParametre(nom, Parametre.TYPE_PARAMETRE.T_INT, valeur);
        }
        #endregion

        #region getParametre
        /// <summary>
        /// Obtention d'un parametre, s'il n'existe pas dans la categorie, on l'ajoute
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="type"></param>
        /// <param name="defaut"></param>
        /// <param name="modifiable"></param>
        /// <returns></returns>
        private Object getParametre(String nom, Parametre.TYPE_PARAMETRE type, Object defaut, bool modifiable = false)
        {
            Parametre p;
            _valeurs.TryGetValue(nom, out p);
            if ((p != null) && (p is Parametre))
            {
                if (p._type != type)
                    return defaut;
                p._modifiable = modifiable;
                p._defaut = defaut;
                return p._value;
            }

            _valeurs.Add(nom, new Parametre(nom, type, defaut, modifiable));
            _propre = false;
            _parametreChange?.Invoke(nom);
            return defaut;
        }


        public int getParametre(string name, int defaut, bool modifiable = false)
        {
            return (int)(getParametre(name, Parametre.TYPE_PARAMETRE.T_INT, defaut, modifiable));
        }

        public bool getParametre(string name, bool defaut, bool modifiable = false)
        {
            return (bool)(getParametre(name, Parametre.TYPE_PARAMETRE.T_BOOL, defaut, modifiable));
        }

        public float getParametre(string name, float defaut, bool modifiable = false)
        {

            return (float)(getParametre(name, Parametre.TYPE_PARAMETRE.T_FLOAT, defaut, modifiable));
        }

        public double getParametre(string name, double defaut, bool modifiable = false)
        {

            return (double)(getParametre(name, Parametre.TYPE_PARAMETRE.T_DOUBLE, defaut, modifiable));
        }

        public string getParametre(string name, string defaut, bool modifiable = false)
        {

            return (string)(getParametre(name, Parametre.TYPE_PARAMETRE.T_STRING, defaut, modifiable));
        }

        public byte getParametre(string name, byte defaut, bool modifiable = false)
        {

            return (byte)(getParametre(name, Parametre.TYPE_PARAMETRE.T_BYTE, defaut, modifiable));
        }
        #endregion


        /// <summary>
        /// S'assurer que les modifications sur la categorie sont bien ecrites dans le fichier
        /// </summary>
        public void flush()
        {
            if (!_propre)
            {
                using (TextWriter tw = new StreamWriter(_fileName))
                {
                    tw.WriteLine("# ----------------------------------------");
                    tw.WriteLine("# " + _nom);
                    tw.WriteLine("# Fichier de configuration ");
                    tw.WriteLine("# (c) Lucien Pilloni 2014");
                    tw.WriteLine("# ----------------------------------------");
                    tw.WriteLine("");
                    foreach (Parametre p in _valeurs.Values.OrderBy(p => p.nom))
                        p.ecritDansFichier(tw);
                }
                _propre = true;
            }
        }

        /// <summary>
        /// Ajoute les parametre dans la console texte
        /// </summary>
        /// <param name="gl"></param>
        public void fillConsole(OpenGL gl)
        {
            DisplayedObjects.Console c = DisplayedObjects.Console.getInstance(gl);
            c.AddLigne(Color.LightGreen, "");
            c.AddLigne(Color.LightGreen, _nom);
            c.AddLigne(Color.LightGreen, "");
            c.AddLigne(Color.LightGreen, "8/2 : changer le parametre courant");
            c.AddLigne(Color.LightGreen, "4/6 : modifier la valeur du parametre courant");
            c.AddLigne(Color.LightGreen, "Les valeurs en gris nécessitent de redémarrer le fond (touche R)");
            c.AddLigne(Color.LightGreen, "");

            foreach (Parametre p in _valeurs.Values.OrderBy(p => p.nom))
                if (p._modifiable)
                    c.AddLigne(p.nom.Equals(keyCourante) ? Color.Yellow : Color.Green, p.nom + " = " + p.valueToString());
                else
                    c.AddLigne(p.nom.Equals(keyCourante) ? Color.White : Color.Gray, p.nom + " = " + p.valueToString());
        }

        /// <summary>
        /// Pression sur une touche, retourner true si l'objet a traite, false = fin de l'economiseur
        /// </summary>
        /// <param name="f"></param>
        /// <param name="k"></param>
        /// <returns>true si touche utilisee</returns>
        public virtual bool KeyDown(Keys k)
        {
            // Tableau des clefs
            List<string> clefs = new List<string>();
            foreach (Parametre p in _valeurs.Values.OrderBy(p => p.nom)/*.Where(p => p._modifiable)*/)
                clefs.Add(p.nom);

            if (clefs.Count == 0)
                return k == Keys.NumPad2 || k == Keys.NumPad8 || k == Keys.NumPad4 || k == Keys.NumPad6;

            if (keyCourante == null)
                keyCourante = clefs[0];

            switch (k)
            {
                case Keys.NumPad2:
                    {
                        int indice = clefs.IndexOf(keyCourante);
                        indice++;
                        if (indice >= clefs.Count)
                            indice = 0;

                        keyCourante = clefs[indice];
                        break;
                    }

                case Keys.NumPad8:
                    {
                        int indice = clefs.IndexOf(keyCourante);
                        indice--;
                        if (indice < 0)
                            indice = clefs.Count - 1;

                        keyCourante = clefs[indice];
                        break;
                    }

                case Keys.NumPad4:
                    {
                        Parametre p;
                        if (_valeurs.TryGetValue(keyCourante, out p))
                        {
                            p.Diminue();
                            _propre = false;
                            _parametreChange?.Invoke(keyCourante);
                        }
                        break;
                    }

                case Keys.NumPad6:
                    {
                        Parametre p;
                        if (_valeurs.TryGetValue(keyCourante, out p))
                        {
                            p.Augmente();
                            _propre = false;
                            _parametreChange?.Invoke(keyCourante);
                        }
                        break;
                    }

                case Keys.NumPad5:
                    {
                        Parametre p;
                        if (_valeurs.TryGetValue(keyCourante, out p))
                        {
                            p.Defaut();
                            _propre = false;
                            _parametreChange?.Invoke(keyCourante);
                        }
                        break;
                    }
                case Keys.NumPad0:
                    {
                        Parametre p;
                        if (_valeurs.TryGetValue(keyCourante, out p))
                        {
                            p.Nulle();
                            _propre = false;
                            _parametreChange?.Invoke(keyCourante);
                        }
                        break;
                    }

                case Keys.Subtract:
                    {
                        Parametre p;
                        if (_valeurs.TryGetValue(keyCourante, out p))
                        {
                            p.Negatif();
                            _propre = false;
                            _parametreChange?.Invoke(keyCourante);
                        }

                        break;
                    }
                default:
                    return false;
            }

            return true;
        }

        public void Dispose()
        {
            flush();
        }
    }
}
