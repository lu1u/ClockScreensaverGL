﻿/*
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
using System.ComponentModel ;

namespace ClockScreenSaverGL
{
	/// <summary>
	/// Description of Config.
	/// </summary>
	public class Config
	{
		enum TYPE_PARAMETRE { T_INT, T_FLOAT, T_DOUBLE, T_BOOL, T_STRING, T_BYTE } ;
		private const string TYPE_INT = "int" ;
		private const string TYPE_FLOAT = "float" ;
		private const string TYPE_DOUBLE = "double" ;
		private const string TYPE_BOOL = "bool" ;
		private const string TYPE_STRING = "string" ;
		private const string TYPE_BYTE = "byte" ;
		private const char CAT_SEPARATOR = '|' ;
		private const char VALUE_SEPARATOR = '=' ;
		private const char KEY_SEPARATOR = ':' ;
		
		private class Parameter
		{
			public Parameter( Object value, TYPE_PARAMETRE type, Object defaut )
			{
				_value = value ;
				_type = type ;
				_default = defaut ;
			}
			public TYPE_PARAMETRE _type ;
			public Object _value ;
			public Object _default ;
		};
		
		
		
		
		
		bool _propre ; 	// AUcune valeur modifiee
		static Config _instance ;
		Dictionary<string, Dictionary<string, Parameter>> _categories ;
		
		public static Config getInstance()
		{
			if ( _instance == null)
				_instance = new Config() ;
			
			return _instance ;
		}
		
		private Config()
		{
			_categories = new Dictionary<string, Dictionary<string, Config.Parameter>>() ;
			LireFichierConf() ;
		}
		
		~Config()
		{
			if (! _propre)
				EcritFichier() ;
		}
		
		/// <summary>
		/// Ecrit le fichier de configuration
		/// </summary>
		void EcritFichier()
		{
			// Un fichier par categorie
			
			foreach( string categorie in _categories.Keys)
			{
				Dictionary<string, Parameter> valeurs ;
				_categories.TryGetValue( categorie, out valeurs ) ;
				if ( valeurs!= null)
				{
					string nomFichier = getNomFichier(categorie) ;
					TextWriter tw = new StreamWriter(nomFichier);
					foreach( String key in valeurs.Keys )
					{
						Parameter p = valeurs[key] ;
						tw.WriteLine( key + ':' + toLigneType(p)) ;
						tw.WriteLine( key + ':' + toLigneValue(p)) ;
						tw.WriteLine( key + ':' + toLigneDefaut(p)) ;
					}
					tw.Close();
				}
			}
			_propre = true ;
		}
		
		string toLigneType(Parameter p)
		{
			switch( p._type )
			{
				case TYPE_PARAMETRE.T_INT :
					return "type="  + TYPE_INT;
				case TYPE_PARAMETRE.T_FLOAT :
					return "type=" + TYPE_FLOAT ;
				case TYPE_PARAMETRE.T_DOUBLE :
					return "type=" + TYPE_DOUBLE  ;
				case TYPE_PARAMETRE.T_BOOL :
					return "type=" + TYPE_BOOL  ;
				case TYPE_PARAMETRE.T_STRING :
					return "type=" + TYPE_STRING ;
				case TYPE_PARAMETRE.T_BYTE :
					return "type=" + TYPE_BYTE ;
					
				default:
					return null ;
			}
		}
		
		string toLigneValue(Parameter p)
		{
			switch( p._type )
			{
				case TYPE_PARAMETRE.T_INT :
					return "value=" + ((int)p._value)   ;
				case TYPE_PARAMETRE.T_FLOAT :
					return "value=" + ((float)p._value)   ;
				case TYPE_PARAMETRE.T_DOUBLE :
					return "value=" + ((double)p._value)   ;
				case TYPE_PARAMETRE.T_BOOL :
					return "value=" + (stringFromBool((bool)p._value))   ;
				case TYPE_PARAMETRE.T_STRING :
					return "value=" + ((string)p._value)  ;
				case TYPE_PARAMETRE.T_BYTE :
					return "value=" + ((byte)p._value)  ;
					
				default:
					return null ;
			}
		}
		string toLigneDefaut(Parameter p)
		{
			switch( p._type )
			{
				case TYPE_PARAMETRE.T_INT :
					return "default=" + ((int)p._default)  ;
				case TYPE_PARAMETRE.T_FLOAT :
					return "default=" + ((float)p._default)  ;
				case TYPE_PARAMETRE.T_DOUBLE :
					return "default=" + ((double)p._default)  ;
				case TYPE_PARAMETRE.T_BOOL :
					return "default=" + (stringFromBool((bool)p._default))  ;
				case TYPE_PARAMETRE.T_STRING :
					return "default=" + ((string)p._default)  ;
				case TYPE_PARAMETRE.T_BYTE :
					return "default=" + ((byte)p._default)  ;
					
				default:
					return null ;
			}
		}
		
		/// <summary>
		/// Lit le fichier de configuration et donne leur valeur aux parametres
		/// </summary>
		void LireFichierConf()
		{
			// Parcours les fichiers conf dans le repertoire
			string rep = getRepertoire() ;
			try
			{string[] filePaths = Directory.GetFiles(rep, "*.conf");
				foreach( string filename in filePaths)
				{
					
					// Creer la categorie
					string categorieName = Path.GetFileNameWithoutExtension(filename) ;
					Dictionary<string, Parameter> categorie = new Dictionary<string, Config.Parameter>() ;
					
					// La remplir a partir du contenu du fichier
					StreamReader file = new System.IO.StreamReader(filename);
					string line, type ;
					while((line = file.ReadLine()) != null)
					{
						int indexDp = line.IndexOf(KEY_SEPARATOR) ;
						int indexEgal = line.IndexOf(VALUE_SEPARATOR) ;
						
						
						string name = line.Substring(0,indexDp) ;
						type = 	line.Substring(indexEgal+1) ;
						
						line = file.ReadLine() ;
						string valeur = line.Substring(line.IndexOf(VALUE_SEPARATOR)+1) ;
						line = file.ReadLine() ;
						string defaut = line.Substring(line.IndexOf(VALUE_SEPARATOR)+1) ;
						
						if ( type.Equals(TYPE_BOOL))
							setParametreFromFile( categorie, name, TYPE_PARAMETRE.T_BOOL, boolFromString(valeur), boolFromString( defaut) ) ;
						else if ( type.Equals(TYPE_DOUBLE))
							setParametreFromFile( categorie, name, TYPE_PARAMETRE.T_DOUBLE, Double.Parse(valeur), Double.Parse(defaut)) ;
						else if ( type.Equals(TYPE_FLOAT))
							setParametreFromFile( categorie, name, TYPE_PARAMETRE.T_FLOAT, (float)Double.Parse(valeur), (float)Double.Parse(defaut)) ;
						else if ( type.Equals(TYPE_INT))
							setParametreFromFile( categorie, name, TYPE_PARAMETRE.T_INT, (int)Int64.Parse(valeur), (int)Int64.Parse(defaut)) ;
						else if ( type.Equals(TYPE_STRING))
							setParametreFromFile( categorie, name, TYPE_PARAMETRE.T_INT, valeur, defaut) ;
						else if ( type.Equals(TYPE_BYTE))
							setParametreFromFile( categorie, name, TYPE_PARAMETRE.T_BYTE, (byte)Int64.Parse(valeur), (byte)Int64.Parse(defaut)) ;
					}
					
					file.Close() ;
					_categories.Add( categorieName, categorie ) ;
				}
			}
			catch( FileNotFoundException )
			{
				// C'est normal: par encore de fichier de configuration (premier lancement)
			}
			catch( DirectoryNotFoundException )
			{
				// C'est normal: par encore de fichier de configuration (premier lancement)
			}
			catch( Exception e )
			{
				throw e ;
			}
			
			/*
			string nomFichier = getNomFichier() ;
			_valeurs = new Dictionary<string, Parameter>() ;
			_propre = true ;
			string type ;
			try
			{
				string[] lines = System.IO.File.ReadAllLines(nomFichier);

				for (int i=0; i < lines.Length; i+=3)
				{
					type = lines[i] ;
					int indexDp = type.IndexOf(KEY_SEPARATOR) ;
					int indexEgal = type.IndexOf(VALUE_SEPARATOR) ;
					
					
					string categorie_name = type.Substring(0,indexDp) ;
					type = 	type.Substring(indexEgal+1) ;
					
					string valeur = lines[i+1].Substring(lines[i+1].IndexOf(VALUE_SEPARATOR)+1) ;
					string defaut = lines[i+2].Substring(lines[i+2].IndexOf(VALUE_SEPARATOR)+1) ;
					
					if ( type.Equals(TYPE_BOOL))
						setParametreFromFile( categorie_name, TYPE_PARAMETRE.T_BOOL, boolFromString(valeur), boolFromString( defaut) ) ;
					else if ( type.Equals(TYPE_DOUBLE))
						setParametreFromFile( categorie_name, TYPE_PARAMETRE.T_DOUBLE, Double.Parse(valeur), Double.Parse(defaut)) ;
					else if ( type.Equals(TYPE_FLOAT))
						setParametreFromFile( categorie_name, TYPE_PARAMETRE.T_FLOAT, (float)Double.Parse(valeur), (float)Double.Parse(defaut)) ;
					else if ( type.Equals(TYPE_INT))
						setParametreFromFile( categorie_name, TYPE_PARAMETRE.T_INT, (int)Int64.Parse(valeur), (int)Int64.Parse(defaut)) ;
					else if ( type.Equals(TYPE_STRING))
						setParametreFromFile( categorie_name, TYPE_PARAMETRE.T_INT, valeur, defaut) ;
					else if ( type.Equals(TYPE_BYTE))
						setParametreFromFile( categorie_name, TYPE_PARAMETRE.T_BYTE, (byte)Int64.Parse(valeur), (byte)Int64.Parse(defaut)) ;
					
				}
			}
			catch( FileNotFoundException )
			{
				// Fichier pas trouve, c'est normal si c'est le premier lancement
			}
			catch( Exception e )
			{
				throw e ;
			}
			 */
		}
		
		
		
		static bool boolFromString( string s )
		{
			if ( s == null)
				return false ;
			
			string S = s.ToLower() ;
			if (S.Equals("true") || S.Equals("vrai"))
				return true ;
			
			return false ;
		}
		
		static string stringFromBool( bool b )
		{
			return b ? "true" : "false" ;
		}
		
		void setParametreFromFile(Dictionary<string, Parameter> categorie, string key, Config.TYPE_PARAMETRE type, Object valeur, Object defaut)
		{
			if ( categorie.ContainsKey( key ))
				categorie.Remove(key) ;
			
			Parameter par = new Parameter( valeur, type, defaut ) ;
			categorie.Add( key, par ) ;
			
			_propre = false ;
		}
		
		public static string getRepertoire()
		{
			return  Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString(), "ClockScreenSaverGL" ) ;
		}
		
		/// <summary>
		/// Calcule le nom du fichier
		/// </summary>
		/// <returns></returns>
		string getNomFichier(string categorie)
		{
			string path = getRepertoire() ;
			Directory.CreateDirectory( path ) ;
			
			return Path.Combine( path, categorie + ".conf");
		}
		
		/// <summary>
		/// Retroube
		/// </summary>
		/// <param name="categorie"></param>
		/// <param name="name"></param>
		/// <param name="defaut"></param>
		/// <returns></returns>
		private Object getParametre( string cat, string name, TYPE_PARAMETRE type, Object defaut )
		{
			SortedList list = new SortedList() ;
			
			Dictionary<string, Parameter> categorie ;
			_categories.TryGetValue( cat, out categorie ) ;
			if ( categorie == null )
			{
				categorie = new Dictionary<string, Parameter>() ;
				_categories.Add(cat, categorie ) ;
			}
			
			Parameter p ;
			categorie.TryGetValue(name, out p ) ;
			if ( (p != null) &&  (p is Parameter))
			{
				if ( p._type != type )
					throw new InvalidCastException("parametre " + name + ": type invalide" ) ;
				
				return p._value ;
			}
			
			categorie.Add( name, new Parameter( defaut, type, defaut ) );
			return defaut ;
		}
		
		string constructKey(string categorie, string name)
		{
			return categorie + CAT_SEPARATOR + name ;
		}
		
		public int getParametre( string categorie, string name, int defaut )
		{
			return (int)(getParametre(categorie, name, TYPE_PARAMETRE.T_INT, defaut )) ;
		}
		
		public bool getParametre( string categorie, string name, bool defaut )
		{
			return (bool)(getParametre(categorie, name, TYPE_PARAMETRE.T_BOOL, defaut )) ;
		}
		
		public float getParametre( string categorie, string name, float defaut )
		{
			
			return(float)(getParametre(categorie, name, TYPE_PARAMETRE.T_FLOAT, defaut )) ;
		}
		
		public double getParametre( string categorie, string name, double defaut )
		{
			
			return(double)(getParametre(categorie, name, TYPE_PARAMETRE.T_DOUBLE, defaut )) ;
		}

		public string getParametre( string categorie, string name, string defaut )
		{
			
			return(string)(getParametre(categorie, name, TYPE_PARAMETRE.T_STRING, defaut )) ;
		}

		public byte getParametre( string categorie, string name, byte defaut )
		{
			
			return(byte)(getParametre(categorie, name, TYPE_PARAMETRE.T_BYTE, defaut )) ;
		}


		private void setParametre( string categorieName, string valueName, TYPE_PARAMETRE type, Object defaut )
		{
			Dictionary<string, Parameter> categorie ;
			_categories.TryGetValue( categorieName, out categorie ) ;
			
			if ( categorie == null)
			{
				categorie = new Dictionary<string, Config.Parameter>() ;
				_categories.Add( categorieName, categorie ) ;
			}
			
			if ( categorie.ContainsKey( valueName ))
				categorie.Remove(valueName) ;
			
			Parameter par = new Parameter( defaut, type, defaut ) ;
			categorie.Add( valueName, par ) ;
			
			_propre = false ;
		}
		
		public void setParametre(string Cat, string nom, bool valeur)
		{
			setParametre(  Cat, nom, TYPE_PARAMETRE.T_BOOL, valeur ) ;
		}
		public void setParametre(string Cat, string nom, float valeur)
		{
			setParametre(  Cat, nom, TYPE_PARAMETRE.T_FLOAT, valeur ) ;
		}
		
		public void setParametre(string Cat, string nom, int valeur)
		{
			setParametre(  Cat, nom, TYPE_PARAMETRE.T_INT, valeur ) ;
		}

		public string[] getCategories()
		{
			string[] res = new string[_categories.Count] ;
			
			int i = 0;
			foreach( string key in _categories.Keys)
			{
				res[i] = key ;
				i++ ;
			}
			
			return res ;
		}

	}
}