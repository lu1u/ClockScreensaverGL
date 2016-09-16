/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 16/01/2015
 * Heure: 11:37
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Reflection;

namespace ClockScreenSaverGL
{
    /// <summary>
    /// Description of Config.
    /// </summary>
    public class Config
    {
        public enum TYPE_PARAMETRE { T_INT, T_FLOAT, T_DOUBLE, T_BOOL, T_STRING, T_BYTE };
        private const string TYPE_INT = "int";
        private const string TYPE_FLOAT = "float";
        private const string TYPE_DOUBLE = "double";
        private const string TYPE_BOOL = "bool";
        private const string TYPE_STRING = "string";
        private const string TYPE_BYTE = "byte";
        private const char CAT_SEPARATOR = '|';
        private const char VALUE_SEPARATOR = '=';
        private const char KEY_SEPARATOR = '.';

        

        public class Categorie
        {
            public delegate void ParametreChange(String nom);

            private class Parameter
            {
                public Parameter(Object value, TYPE_PARAMETRE type, Object defaut)
                {
                    _value = value;
                    _type = type;
                    _default = defaut;
                }
                public TYPE_PARAMETRE _type;
                public Object _value;
                public Object _default;
            };

            private ParametreChange _parametreChange;
            public bool _propre = true;
            private Dictionary<string, Parameter> _valeurs = new Dictionary<string, Parameter>();
            private string _name;

            public Categorie(String name)
            {
                _name = name;

            }

            public void setListenerParametreChange( ParametreChange p)
            {
                _parametreChange = p;
            }

            public Object getParametre(String name, TYPE_PARAMETRE type,Object defaut)
            {
                Parameter p;
                _valeurs.TryGetValue(name, out p);
                if ((p != null) && (p is Parameter))
                {
                    if (p._type != type)
                        //throw new InvalidCastException("parametre " + name + ": type invalide");
                        return defaut;

                    return p._value;
                }

                _valeurs.Add(name, new Parameter(defaut, type, defaut));
                _propre = false;
                if ( _parametreChange !=null)
                    _parametreChange(name);
                return defaut;
            }

            public void setParametreFromFile(string key, Config.TYPE_PARAMETRE type, Object valeur, Object defaut)
            {
                if (_valeurs.ContainsKey(key))
                    _valeurs.Remove(key);

                Parameter par = new Parameter(valeur, type, defaut);
                _valeurs.Add(key, par);
            }


            public void setParametre(String valueName, TYPE_PARAMETRE type, Object defaut)
            {
                if (_valeurs.ContainsKey(valueName))
                    _valeurs.Remove(valueName);

                Parameter par = new Parameter(defaut, type, defaut);
                _valeurs.Add(valueName, par);

                _propre = false;
            }

            /// <summary>
            /// S'assurer que les modifications sur la categorie sont bien ecrites dans le fichier
            /// </summary>
            public void flush()
            {
                if (!_propre)
                {
                    string nomFichier = getNomFichier(_name);
                    using (TextWriter tw = new StreamWriter(nomFichier))
                    {
                        foreach (String key in _valeurs.Keys)
                        {
                            Parameter p = _valeurs[key];
                            tw.WriteLine(key + KEY_SEPARATOR + toLigneType(p));
                            tw.WriteLine(key + KEY_SEPARATOR + toLigneValue(p));
                            tw.WriteLine(key + KEY_SEPARATOR + toLigneDefaut(p));
                        }
                    }
                    _propre = true;
                }

            }

            static string toLigneType(Parameter p)
            {
                switch (p._type)
                {
                    case TYPE_PARAMETRE.T_INT:
                        return "type" + VALUE_SEPARATOR + TYPE_INT;
                    case TYPE_PARAMETRE.T_FLOAT:
                        return "type" + VALUE_SEPARATOR + TYPE_FLOAT;
                    case TYPE_PARAMETRE.T_DOUBLE:
                        return "type" + VALUE_SEPARATOR + TYPE_DOUBLE;
                    case TYPE_PARAMETRE.T_BOOL:
                        return "type" + VALUE_SEPARATOR + TYPE_BOOL;
                    case TYPE_PARAMETRE.T_STRING:
                        return "type" + VALUE_SEPARATOR + TYPE_STRING;
                    case TYPE_PARAMETRE.T_BYTE:
                        return "type" + VALUE_SEPARATOR + TYPE_BYTE;

                    default:
                        return null;
                }
            }

            static string toLigneValue(Parameter p)
            {
                switch (p._type)
                {
                    case TYPE_PARAMETRE.T_INT:
                        return "value" + VALUE_SEPARATOR + ((int)p._value);
                    case TYPE_PARAMETRE.T_FLOAT:
                        return "value" + VALUE_SEPARATOR + ((float)p._value);
                    case TYPE_PARAMETRE.T_DOUBLE:
                        return "value" + VALUE_SEPARATOR + ((double)p._value);
                    case TYPE_PARAMETRE.T_BOOL:
                        return "value" + VALUE_SEPARATOR + (stringFromBool((bool)p._value));
                    case TYPE_PARAMETRE.T_STRING:
                        return "value" + VALUE_SEPARATOR + ((string)p._value);
                    case TYPE_PARAMETRE.T_BYTE:
                        return "value" + VALUE_SEPARATOR + ((byte)p._value);

                    default:
                        return null;
                }
            }

            static string toLigneDefaut(Parameter p)
            {
                switch (p._type)
                {
                    case TYPE_PARAMETRE.T_INT:
                        return "default" + VALUE_SEPARATOR + ((int)p._default);
                    case TYPE_PARAMETRE.T_FLOAT:
                        return "default" + VALUE_SEPARATOR + ((float)p._default);
                    case TYPE_PARAMETRE.T_DOUBLE:
                        return "default" + VALUE_SEPARATOR + ((double)p._default);
                    case TYPE_PARAMETRE.T_BOOL:
                        return "default" + VALUE_SEPARATOR + (stringFromBool((bool)p._default));
                    case TYPE_PARAMETRE.T_STRING:
                        return "default" + VALUE_SEPARATOR + ((string)p._default);
                    case TYPE_PARAMETRE.T_BYTE:
                        return "default" + VALUE_SEPARATOR + ((byte)p._default);

                    default:
                        return null;
                }
            }

        }


        static Config _instance;
        private Dictionary<string, Categorie> _categories;

        public static Config getInstance()
        {
            if (_instance == null)
                _instance = new Config();

            return _instance;
        }

        private Config()
        {
            _categories = new Dictionary<string, Categorie>();
            LireFichierConf();
        }

        ~Config()
        {
            EcrireCategories();
        }

       
        public void flush( String name)
        {
            Categorie categorie;
            _categories.TryGetValue(name, out categorie);
            categorie?.flush();            
        }

        public static string getDataDirectory()
        {
            return Path.Combine(new FileInfo((Assembly.GetExecutingAssembly().Location)).Directory.FullName, "Donnees") ;
        }

        public static string getImagesDirectory()
        {
            return Path.Combine(getDataDirectory(), "Images");
        }

        public static string getImagePath(string imgName)
        {
            string res = Path.Combine(getImagesDirectory(), imgName);
            if (File.Exists(res))
              return res;

            return Path.Combine(getImagesDirectory(), "particule.png");
        }

        /// <summary>
        /// Ecrit le fichier de configuration
        /// </summary>
        void EcrireCategories()
        {
            foreach (String name in _categories.Keys)
                    flush(name);            
        }


        
        /// <summary>
        /// Lit le fichier de configuration et donne leur valeur aux parametres
        /// </summary>
        void LireFichierConf()
        {         
            // Parcours les fichiers conf dans le repertoire
            string rep = getRepertoire();
            try
            {
                string[] filePaths = Directory.GetFiles(rep, "*.conf");
                foreach (string filename in filePaths)
                {

                    // Creer la categorie
                    string categorieName = Path.GetFileNameWithoutExtension(filename);
                    Categorie categorie = new Categorie(categorieName);

                    // La remplir a partir du contenu du fichier
                    using (StreamReader file = new System.IO.StreamReader(filename))
                    {
                        string line, type;
                        while ((line = file.ReadLine()) != null)
                        {
                            int indexDp = line.IndexOf(KEY_SEPARATOR);
                            int indexEgal = line.IndexOf(VALUE_SEPARATOR);


                            string name = line.Substring(0, indexDp);
                            type = line.Substring(indexEgal + 1);

                            line = file.ReadLine();
                            string valeur = line.Substring(line.IndexOf(VALUE_SEPARATOR) + 1);
                            line = file.ReadLine();

                            string defaut;
                            if (line == null)
                                defaut = "";
                            else
                            {
                                int indice = line.IndexOf(VALUE_SEPARATOR);
                                try
                                {
                                    if (indice > 0)
                                        defaut = line.Substring(line.IndexOf(VALUE_SEPARATOR) + 1);
                                    else
                                        defaut = "";
                                }
                                catch (Exception)
                                {

                                    throw;
                                }
                            }

                            if (type.Equals(TYPE_BOOL))
                                categorie.setParametreFromFile( name, TYPE_PARAMETRE.T_BOOL, boolFromString(valeur), boolFromString(defaut));
                            else if (type.Equals(TYPE_DOUBLE))
                                categorie.setParametreFromFile( name, TYPE_PARAMETRE.T_DOUBLE, doubleFromString(valeur), doubleFromString(defaut));
                            else if (type.Equals(TYPE_FLOAT))
                                categorie.setParametreFromFile( name, TYPE_PARAMETRE.T_FLOAT, floatFromString(valeur), floatFromString(defaut));
                            else if (type.Equals(TYPE_INT))
                                categorie.setParametreFromFile( name, TYPE_PARAMETRE.T_INT, intFromSting(valeur), intFromSting(defaut));
                            else if (type.Equals(TYPE_STRING))
                                categorie.setParametreFromFile( name, TYPE_PARAMETRE.T_STRING, valeur, defaut);
                            else if (type.Equals(TYPE_BYTE))
                                categorie.setParametreFromFile( name, TYPE_PARAMETRE.T_BYTE, byteFromString(valeur), byteFromString(defaut));
                        }
                    }
                    _categories.Add(categorieName, categorie);
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
                throw e;
            }

        }

        static private byte byteFromString(string valeur)
        {
            return (byte)(intFromSting(valeur) % 256);
        }

        static private int intFromSting(string valeur)
        {
            try
            {
                return (int)Int64.Parse(valeur);
            }
            catch (Exception)
            {
                try
                {
                    return (int)Math.Round(Double.Parse(valeur));
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        static private float floatFromString(string valeur)
        {
            return (float)doubleFromString(valeur);
        }

        static private double doubleFromString(string valeur)
        {
            try
            {
                return Double.Parse(valeur);
            }
            catch (Exception)
            {
                try
                {
                    return Int64.Parse(valeur);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        static bool boolFromString(string s)
        {
            if (s == null)
                return false;

            string S = s.ToLower();
            if (S.Equals("true") || S.Equals("vrai"))
                return true;

            return false;
        }

        static string stringFromBool(bool b)
        {
            return b ? "true" : "false";
        }



        /// <summary>
        /// Calcule un repertoire pour stocker les fichiers de conf
        /// en utilisant le nom du programme et sa version
        /// </summary>
        /// <returns></returns>
        public static string getRepertoire()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString(),
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
            "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString());
        }

        /// <summary>
        /// Calcule le nom du fichier
        /// </summary>
        /// <returns></returns>
        static string getNomFichier(string categorie)
        {
            string path = getRepertoire();
            Directory.CreateDirectory(path);

            return Path.Combine(path, categorie + ".conf");
        }

        /// <summary>
        /// Retroube
        /// </summary>
        /// <param name="categorie"></param>
        /// <param name="name"></param>
        /// <param name="defaut"></param>
        /// <returns></returns>
        private Object getParametre(string cat, string name, TYPE_PARAMETRE type, Object defaut)
        {
            Categorie categorie;
            _categories.TryGetValue(cat, out categorie);
            if (categorie == null)
            {
                categorie = new Categorie(cat);
                categorie._propre = false;
                _categories.Add(cat, categorie);
            }

            return categorie.getParametre(name, type, defaut);
            
        }

        public int getParametre(string categorie, string name, int defaut)
        {
            return (int)(getParametre(categorie, name, TYPE_PARAMETRE.T_INT, defaut));
        }

        public bool getParametre(string categorie, string name, bool defaut)
        {
            return (bool)(getParametre(categorie, name, TYPE_PARAMETRE.T_BOOL, defaut));
        }

        public float getParametre(string categorie, string name, float defaut)
        {

            return (float)(getParametre(categorie, name, TYPE_PARAMETRE.T_FLOAT, defaut));
        }

        public double getParametre(string categorie, string name, double defaut)
        {

            return (double)(getParametre(categorie, name, TYPE_PARAMETRE.T_DOUBLE, defaut));
        }

        public string getParametre(string categorie, string name, string defaut)
        {

            return (string)(getParametre(categorie, name, TYPE_PARAMETRE.T_STRING, defaut));
        }

        public byte getParametre(string categorie, string name, byte defaut)
        {

            return (byte)(getParametre(categorie, name, TYPE_PARAMETRE.T_BYTE, defaut));
        }

        /// <summary>
        /// Ajoute un parametre ou change sa valeur s'il existe deja
        /// </summary>
        /// <param name="categorieName"></param>
        /// <param name="valueName"></param>
        /// <param name="type"></param>
        /// <param name="defaut"></param>
        private void setParametre(string categorieName, string valueName, TYPE_PARAMETRE type, Object defaut)
        {
            Categorie categorie;
            _categories.TryGetValue(categorieName, out categorie);

            if (categorie == null)
            {
                categorie = new Categorie(categorieName);
                _categories.Add(categorieName, categorie);
            }

            categorie.setParametre(valueName, type, defaut);
        }

        public void setParametre(string Cat, string nom, bool valeur)
        {
            setParametre(Cat, nom, TYPE_PARAMETRE.T_BOOL, valeur);
        }
        public void setParametre(string Cat, string nom, float valeur)
        {
            setParametre(Cat, nom, TYPE_PARAMETRE.T_FLOAT, valeur);
        }

        public void setParametre(string Cat, string nom, int valeur)
        {
            setParametre(Cat, nom, TYPE_PARAMETRE.T_INT, valeur);
        }

        public string[] getCategories()
        {
            string[] res = new string[_categories.Count];

            int i = 0;
            foreach (string key in _categories.Keys)
            {
                res[i] = key;
                i++;
            }

            return res;
        }

    }
}
