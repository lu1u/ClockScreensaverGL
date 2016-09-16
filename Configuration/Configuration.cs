using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ClockScreenSaverGL.Config
{
    public class Configuration : IDisposable
    {
        private const string REPERTOIRE_DONNEES = "Donnees";
        private const string REPERTOIRE_IMAGES = "Images";
        private const string IMAGE_DEFAUT = "particule.png";
        static Configuration _instance;
        private Dictionary<string, CategorieConfiguration> _categories;


        public static Configuration getInstance()
        {
            if (_instance == null)
                _instance = new Configuration();

            return _instance;
        }

        /// <summary>
        /// Retourne l'objet CategorieConfiguration dont on donne le nom
        /// </summary>
        /// <param name="nom"></param>
        /// <returns></returns>
        public static CategorieConfiguration getCategorie( String nom )
        {
            Configuration conf = getInstance();
            CategorieConfiguration categorie;
            conf._categories.TryGetValue(nom, out categorie);
            if (categorie == null)
            {
                categorie = new CategorieConfiguration(nom);
                conf._categories.Add(nom, categorie);
            }
            return categorie;
        }

        private Configuration()
        {
            _categories = new Dictionary<string, CategorieConfiguration>();
            LireCategories();
        }

        private void LireCategories()
        {
            // Parcours les fichiers conf dans le repertoire
            string rep = getRepertoire();
            try
            {
                string[] filePaths = Directory.GetFiles(rep, CategorieConfiguration.EXTENSION_CONF);
                foreach (string filename in filePaths)
                {
                    string categorieName = Path.GetFileNameWithoutExtension(filename);
                    _categories.Add(categorieName, new CategorieConfiguration(categorieName));
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

        ~Configuration()
        {
            flush();
        }

        private void flush()
        {
            foreach (CategorieConfiguration cat in _categories.Values)
                cat.flush();
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
            "Version 3");
        }

        public static string getDataDirectory()
        {
            return Path.Combine(new FileInfo((Assembly.GetExecutingAssembly().Location)).Directory.FullName, REPERTOIRE_DONNEES);
        }

        public static string getImagesDirectory()
        {
            return Path.Combine(getDataDirectory(), REPERTOIRE_IMAGES);
        }

        public static string getImagePath(string imgName)
        {
            string res = Path.Combine(getImagesDirectory(), imgName);
            if (File.Exists(res))
                return res;

            return Path.Combine(getImagesDirectory(), IMAGE_DEFAUT);
        }

        public void Dispose()
        {
            flush();
        }
    }
}
