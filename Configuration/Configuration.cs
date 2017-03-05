using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ClockScreenSaverGL.Config
{
    public class Configuration : IDisposable
    {
        private const string REPERTOIRE_DONNEES = "Donnees";
        private const string REPERTOIRE_IMAGES = "Images";
        private const string IMAGE_DEFAUT = "particule.png";

        // Singleton
        private static Configuration _instance;

        // Dictionnaire des categories de configuration
        private Dictionary<string, CategorieConfiguration> _categories;

        public static Configuration getInstance()
        {
            if ( _instance == null )
                return _instance = new Configuration();

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
            conf._categories.TryGetValue( nom, out categorie );
            if ( categorie != null )
                return categorie;

            // La categorie n'existe pas encore
            categorie = new CategorieConfiguration( nom );
            conf._categories.Add( nom, categorie );
            return categorie;
        }

        private Configuration()
        {
            _categories = new Dictionary<string, CategorieConfiguration>();
            LireCategories();
        }

        /// <summary>
        /// Lire les fichiers de configuration dans les objets CategorieConfiguration
        /// </summary>
        private void LireCategories()
        {
            // Parcours les fichiers conf dans le repertoire
            string rep = getRepertoire();
            try
            {
                string[] filePaths = Directory.GetFiles(rep, CategorieConfiguration.EXTENSION_CONF);
                foreach ( string filename in filePaths )
                {
                    string categorieName = Path.GetFileNameWithoutExtension(filename);
                    _categories.Add( categorieName, new CategorieConfiguration( categorieName ) );
                }
            }
            catch ( FileNotFoundException )
            {
                // C'est normal: pas encore de fichier de configuration (premier lancement)
            }
            catch ( DirectoryNotFoundException )
            {
                // C'est normal: par encore de fichier de configuration (premier lancement)
            }
            catch ( Exception e )
            {
                throw e;
            }
        }

        ~Configuration()
        {
            flush();
        }

        /// <summary>
        /// Reecriture de tous les fichiers de configuration qui en ont besoin
        /// </summary>
        public void flush()
        {
            foreach ( CategorieConfiguration cat in _categories.Values )
                cat.flush();
        }

        /// <summary>
        /// Calcule un repertoire pour stocker les fichiers de conf en utilisant le nom du programme et sa version
        /// </summary>
        /// <returns></returns>
        public static string getRepertoire()
        {
            string res = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString(),
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
            "Version 3");

            if ( !Directory.Exists( res ) )
                Directory.CreateDirectory( res );

            return res;
        }

        /// <summary>
        /// Retrouve le chemin du repertoire de stockage des donnees du programme,
        /// DIFFERENT du repertoire de la configuration
        /// </summary>
        /// <returns></returns>
        public static string getDataDirectory() => Path.Combine( new FileInfo( (Assembly.GetExecutingAssembly().Location) ).Directory.FullName, REPERTOIRE_DONNEES );
        
        public static string getImagesDirectory()=> Path.Combine( getDataDirectory(), REPERTOIRE_IMAGES );
        

        /// <summary>
        /// Retourne le chemin vers l'une des images dans le repertoire des images
        /// </summary>
        /// <param name="imgName">Nom de fichier de l'image</param>
        /// <param name="nullIfNotExist">Si true: on obtient un nom de fichier null si le chemin n'existe pas,
        /// si false: on obtient le chemin d'une image par defaut</param>
        /// <returns></returns>
        public static string getImagePath( string imgName, bool nullIfNotExist = false )
        {
            string res = Path.Combine(getImagesDirectory(), imgName);
            if ( File.Exists( res ) )
                return res;

            Log.getInstance().warning( "Image inexistante: " + imgName );

            if ( nullIfNotExist )
                return null;
            else
                return Path.Combine( getImagesDirectory(), IMAGE_DEFAUT );
        }

        public void Dispose()
        {
            flush();
        }
    }
}
