using System;
using System.IO;
/// <summary>
/// Parametre dans un fichier de configuration, stocke dans une ligne au format:
/// {nom}:{type},{modifiable}={valeur}
/// ou
/// {type} = int, float,double, bool, string, byte
/// {modifiable} = modifiable, non_modifiable
/// </summary>

namespace ClockScreenSaverGL.Config
{
    public class Parametre
    {
        #region Public Fields

        public Action<object> _action;

        public object _defaut;

        public TYPE_PARAMETRE _type;

        public object _value;

        public string nom;

        #endregion Public Fields

        #region Private Fields

        private const string TYPE_BOOL = "bool";

        private const string TYPE_BYTE = "byte";

        private const string TYPE_DOUBLE = "double";

        private const string TYPE_FLOAT = "float";

        private const string TYPE_INT = "int";

        private const string TYPE_STRING = "string";

        #endregion Private Fields

        #region Public Constructors

        public Parametre( string ligneFichier )
        {
            string tmp = ligneFichier;

            // Extraire le nom
            int finMot = ligneFichier.IndexOf(':');
            if ( finMot == -1 )
                throw new Exception( "Impossible de trouver le nom du parametre " + ligneFichier );
            nom = ligneFichier.Substring( 0, finMot );
            ligneFichier = ligneFichier.Substring( finMot + 1 );

            // Type
            finMot = ligneFichier.IndexOf( ',' );
            if ( finMot == -1 )
                throw new Exception( "Impossible de trouver le type du parametre " + tmp );
            _type = stringToType( ligneFichier.Substring( 0, finMot ) );
            ligneFichier = ligneFichier.Substring( finMot + 1 );

            // Modifiable
            finMot = ligneFichier.IndexOf( '=' );
            if ( finMot == -1 )
                throw new Exception( "Impossible de trouver le code modifiable du parametre " + tmp );
            //modifiable = stringToModifiable( ligneFichier.Substring( 0, finMot ) );
            ligneFichier = ligneFichier.Substring( finMot + 1 );

            // Valeur
            _value = toObject( _type, ligneFichier );
            _defaut = toObject( _type, ligneFichier );
        }

        public Parametre( string nom, TYPE_PARAMETRE type, object valeur, Action<object> action = null)
        {
            this.nom = nom;
            _type = type;
            _value = valeur;
            _defaut = valeur;
            _action = action;
        }

        #endregion Public Constructors

        #region Public Enums

        public enum TYPE_PARAMETRE { T_INT, T_FLOAT, T_DOUBLE, T_BOOL, T_STRING, T_BYTE };

        #endregion Public Enums

        #region Public Properties

        public bool modifiable

        {
            get { return _action != null; }
        }

        #endregion Public Properties

        #region Public Methods

        static public String valueToString( TYPE_PARAMETRE type, Object value )
        {
            switch ( type )
            {
                case TYPE_PARAMETRE.T_INT:
                    return ((int) value).ToString();
                case TYPE_PARAMETRE.T_FLOAT:
                    return ((float) value).ToString();
                case TYPE_PARAMETRE.T_DOUBLE:
                    return ((double) value).ToString();
                case TYPE_PARAMETRE.T_BOOL:
                    return (stringFromBool( (bool) value )).ToString();
                case TYPE_PARAMETRE.T_STRING:
                    return ((string) value);
                case TYPE_PARAMETRE.T_BYTE:
                    return ((byte) value).ToString();

                default:
                    return null;
            }
        }

        public String valueToString()
        {
            return valueToString( _type, _value );
        }

        #endregion Public Methods

        #region Internal Methods

        internal void Augmente()
        {
            switch ( _type )
            {
                case TYPE_PARAMETRE.T_INT:
                    _value = ((int) _value) + 1;
                    break;

                case TYPE_PARAMETRE.T_FLOAT:
                    _value = ((float) _value) * 1.1f;
                    break;
                case TYPE_PARAMETRE.T_DOUBLE:
                    _value = ((double) _value) * 1.1;
                    break;

                case TYPE_PARAMETRE.T_BOOL:
                    _value = (bool) _value ? false : true;
                    break;

                case TYPE_PARAMETRE.T_STRING:
                    break;

                case TYPE_PARAMETRE.T_BYTE:
                    _value = (byte) (((byte) _value) + 1 % 256);
                    break;
            }

            _action?.Invoke( _value );
        }

        internal void Defaut()
        {
            _value = _defaut;
            _action?.Invoke( _value );
        }

        internal void Diminue()
        {
            switch ( _type )
            {
                case TYPE_PARAMETRE.T_INT:
                    _value = ((int) _value) - 1;
                    break;

                case TYPE_PARAMETRE.T_FLOAT:
                    _value = ((float) _value) / 1.1f;
                    break;
                case TYPE_PARAMETRE.T_DOUBLE:
                    _value = ((double) _value) / 1.1;
                    break;

                case TYPE_PARAMETRE.T_BOOL:
                    _value = (bool) _value ? false : true;
                    break;

                case TYPE_PARAMETRE.T_STRING:
                    break;

                case TYPE_PARAMETRE.T_BYTE:
                    _value = (byte) (((byte) _value) - 1 % 256);
                    break;
            }

            _action?.Invoke( _value );
        }

        /// <summary>
        /// Ligne de fichier de configuration:
        /// {nom}:{type},modifiable|non modifiable={valeur}|"{valeur}"
        /// </summary>
        /// <param name="tw"></param>
        internal void ecritDansFichier( TextWriter tw )
        {
            tw.WriteLine( nom + ":" + typeToString( _type ) + "," + modifiableToString( modifiable ) + "=" + valueToString( _type, _value ) );
        }

        internal void Negatif()
        {
            switch ( _type )
            {
                case TYPE_PARAMETRE.T_INT:
                    _value = -(int) _value;
                    break;

                case TYPE_PARAMETRE.T_FLOAT:
                    _value = -(float) _value;
                    break;
                case TYPE_PARAMETRE.T_DOUBLE:
                    _value = -(double) _value;
                    break;

                case TYPE_PARAMETRE.T_BOOL:
                    _value = false;
                    break;

                case TYPE_PARAMETRE.T_STRING:
                    _value = "";
                    break;

                case TYPE_PARAMETRE.T_BYTE:
                    _value = (byte) 0;
                    break;
            }

            _action?.Invoke( _value );
        }

        internal void Nulle()
        {
            switch ( _type )
            {
                case TYPE_PARAMETRE.T_INT:
                    _value = (int) 0;
                    break;

                case TYPE_PARAMETRE.T_FLOAT:
                    _value = ((float) 0.0f);
                    break;
                case TYPE_PARAMETRE.T_DOUBLE:
                    _value = ((double) 0.0);
                    break;

                case TYPE_PARAMETRE.T_BOOL:
                    _value = (bool) _value ? false : true;
                    break;

                case TYPE_PARAMETRE.T_STRING:
                    _value = "";
                    break;

                case TYPE_PARAMETRE.T_BYTE:
                    _value = (byte) (256 - (byte) _value);
                    break;
            }

            _action?.Invoke( _value );
        }

        #endregion Internal Methods

        #region Private Methods

        static bool boolFromString( string s )
        {
            if ( s == null )
                return false;

            string S = s.ToLower();
            if ( S.Equals( "true" ) || S.Equals( "vrai" ) )
                return true;

            return false;
        }

        static private byte byteFromString( string valeur )
        {
            return (byte) (intFromString( valeur ) % 256);
        }

        static private double doubleFromString( string valeur )
        {
            try
            {
                return Double.Parse( valeur );
            }
            catch ( Exception )
            {
                try
                {
                    return Int64.Parse( valeur );
                }
                catch ( Exception )
                {
                    return 0;
                }
            }
        }

        static private float floatFromString( string valeur )
        {
            return (float) doubleFromString( valeur );
        }

        static private int intFromString( string valeur )
        {
            try
            {
                return (int) Int64.Parse( valeur );
            }
            catch ( Exception )
            {
                try
                {
                    return (int) Math.Round( Double.Parse( valeur ) );
                }
                catch ( Exception )
                {
                    return 0;
                }
            }
        }

        static private String modifiableToString( bool modifiable )
        {
            return modifiable ? "modifiable" : "non_modifiable";
        }

        static string stringFromBool( bool b )
        {
            return b ? "vrai" : "false";
        }

        static private String stringFromString( string valeur )
        {
            if ( valeur.StartsWith( "\"" ) )
                valeur = valeur.Substring( 1 );

            if ( valeur.EndsWith( "\"" ) )
                valeur = valeur.Substring( 0, valeur.Length - 1 );

            return valeur;
        }

        /// <summary>
        /// Retourne un Object representant la valeur du parametre
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mot"></param>
        /// <returns></returns>
        private static Object toObject( TYPE_PARAMETRE type, String mot )
        {
            switch ( type )
            {
                case TYPE_PARAMETRE.T_INT: return intFromString( mot );
                case TYPE_PARAMETRE.T_FLOAT: return floatFromString( mot );
                case TYPE_PARAMETRE.T_DOUBLE: return doubleFromString( mot );
                case TYPE_PARAMETRE.T_BOOL: return boolFromString( mot );
                case TYPE_PARAMETRE.T_STRING: return stringFromString( mot );
                case TYPE_PARAMETRE.T_BYTE: return byteFromString( mot );
                default:
                    return stringFromString( mot );
            }
        }
        /// <summary>
        /// Retourne la chaine de caractere pour identifier un type dans le fichier de configuration
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static private String typeToString( TYPE_PARAMETRE type )
        {
            switch ( type )
            {
                case TYPE_PARAMETRE.T_INT:
                    return TYPE_INT;
                case TYPE_PARAMETRE.T_FLOAT:
                    return TYPE_FLOAT;
                case TYPE_PARAMETRE.T_DOUBLE:
                    return TYPE_DOUBLE;
                case TYPE_PARAMETRE.T_BOOL:
                    return TYPE_BOOL;
                case TYPE_PARAMETRE.T_STRING:
                    return TYPE_STRING;
                case TYPE_PARAMETRE.T_BYTE:
                    return TYPE_BYTE;

                default:
                    return null;
            }
        }

        private bool stringToModifiable( string modifiable )
        {
            if ( modifiable.Equals( "modifiable" ) )
                return true;

            return false;
        }

        private TYPE_PARAMETRE stringToType( string type )
        {
            if ( type.Equals( TYPE_BOOL ) )
                return TYPE_PARAMETRE.T_BOOL;
            else if ( type.Equals( TYPE_DOUBLE ) )
                return TYPE_PARAMETRE.T_DOUBLE;
            else if ( type.Equals( TYPE_FLOAT ) )
                return TYPE_PARAMETRE.T_FLOAT;
            else if ( type.Equals( TYPE_INT ) )
                return TYPE_PARAMETRE.T_INT;
            else if ( type.Equals( TYPE_STRING ) )
                return TYPE_PARAMETRE.T_STRING;
            else if ( type.Equals( TYPE_BYTE ) )
                return TYPE_PARAMETRE.T_BYTE;

            return TYPE_PARAMETRE.T_STRING;
        }

        #endregion Private Methods
    }
}
