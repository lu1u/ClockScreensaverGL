using SharpGL;
/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 23/01/2015
 * Heure: 17:09
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ClockScreenSaverGL
{

    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Parametres
        public const string CAT = "Main";
        const string PARAM_DELAI_CHANGE_FOND = "DelaiChangeFondMinutes";
        const string PARAM_FONDDESAISON = "FondDeSaison";
        const string PARAM_TYPEFOND = "TypeFond";
        private static Config conf = Config.getInstance();
        static readonly int PRINTEMPS = conf.getParametre(CAT, "Printemps", 80);
        static readonly int ETE = conf.getParametre(CAT, "Ete", 172);
        static readonly int AUTOMNE = conf.getParametre(CAT, "Automne", 266);
        static readonly int HIVER = conf.getParametre(CAT, "Hiver", 356);
        #endregion

        CouleurGlobale _couleur = new CouleurGlobale();        // La couleur de base pour tous les affichages
        private List<DisplayedObject> _listeObjets = new List<DisplayedObject>();
        private int _jourActuel = -1;                          // Pour forcer un changement de date avant la premiere image
        private bool _afficherAide = false;                    // Vrai si on doit afficher le message d'aide

        private bool _fondDeSaison;                           // Vrai si on doit commencer par le fond 'de saison'
        DateTime _derniereFrame = DateTime.Now;                // Heure de la derniere frame affichee
        DateTime _debut = DateTime.Now;
        Temps _temps;

        #region Fonds
        const int TYPE_FOND_ESPACE = 0;
        const int TYPE_FOND_NOIR = 1;
        const int TYPE_FOND_METABALLES = 2;
        const int TYPE_FOND_NUAGES = 3;
        const int TYPE_FOND_COULEUR = 4;
        const int TYPE_FOND_BACTERIES = 5;
        const int TYPE_FOND_ENCRE = 6;
        const int TYPE_FOND_TUNNEL = 7;
        const int TYPE_FOND_NEIGE_META = 8;
        const int TYPE_FOND_LIFE = 9;
        const int TYPE_FOND_TERRE = 10;
        const int NB_FONDS = 11;
        #endregion

        #region Render Modes
        const int RENDERMODE_DIBSECTION = 0;
        const int RENDERMODE_FBO = 1;
        const int RENDERMODE_NATIVE = 2;
        #endregion

        enum SAISON { HIVER = 0, PRINTEMPS = 1, ETE = 2, AUTOMNE = 3 } ;
#if TRACER
        bool _afficheDebug = conf.getParametre(CAT, "Debug", true);
        DateTime lastFrame = DateTime.Now;

        Process currentProc = Process.GetCurrentProcess();
        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;
#endif
        #region Preview API's

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        #endregion

        #region Screensaver
        bool IsPreviewMode = false;
        public MainForm()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            try
            {
                InitializeComponent();

                _temps = new Temps(DateTime.Now, _derniereFrame);
                _fondDeSaison = conf.getParametre(CAT, PARAM_FONDDESAISON, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\ndans : " + ex.Source + "\n" + ex.StackTrace, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        //This constructor is the handle to the select screensaver dialog preview window
        //It is used when in preview mode (/p)
        public MainForm(IntPtr PreviewHandle)
        {
            try
            {
                InitializeComponent();

                //set the preview window as the parent of this window
                SetParent(this.Handle, PreviewHandle);

                //make this a child window, so when the select screensaver dialog closes, this will also close
                SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000));

                //set our window's size to the size of our window's new parent
                Rectangle ParentRect;
                GetClientRect(PreviewHandle, out ParentRect);
                this.Size = ParentRect.Size;

                //set our location at (0, 0)
                this.Location = new Point(0, 0);

                IsPreviewMode = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\ndans : " + ex.Source, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        #endregion

        /// <summary>
        /// Creer l'objet qui anime le fond d'ecran
        /// </summary>
        /// <returns></returns>
        private Fonds.Fond createBackgroundObject(int Type, bool initial)
        {
            //return new Fonds.Ete.Ete(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height); 

            OpenGL gl = openGLControl.OpenGL;
            if (_fondDeSaison && initial)
            {
                // Si l'option 'fond de saison' est selectionnee, l'economiseur commence par celui ci
                // Note: il n'apparaissent plus dans le cycle de changement du fond
                switch (getSaison())
                {
                    case SAISON.HIVER:
                        conf.setParametre(CAT, PARAM_TYPEFOND, TYPE_FOND_ESPACE);
                        if (conf.getParametre(CAT, "Neige.PrefereOpenGL", true))
                            return new Fonds.TroisD.Opengl.NeigeOpenGL(gl);
                        else
                            return new Fonds.TroisD.GDI.NeigeGDI(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);

                    case SAISON.PRINTEMPS:
                        return new Fonds.Printemps.Printemps(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                    case SAISON.ETE: break; // TODO
                    case SAISON.AUTOMNE: break; // TODO
                }
            }
            switch (Type)
            {
                case TYPE_FOND_METABALLES: return new Metaballes.Neige(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case TYPE_FOND_ENCRE: return new Metaballes.Encre(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case TYPE_FOND_BACTERIES: return new Metaballes.Bacteries(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case TYPE_FOND_LIFE: return new Fonds.Life();
                case TYPE_FOND_NOIR: return new Fonds.Noir(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case TYPE_FOND_COULEUR: return new Fonds.Couleur(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case TYPE_FOND_ESPACE:
                    if (conf.getParametre(CAT, "Espace.PrefereOpenGL", true))
                        return new Fonds.TroisD.Opengl.EspaceOpenGL(gl);
                    else
                        return new Fonds.TroisD.GDI.EspaceGDI(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case TYPE_FOND_TUNNEL:
                    if (conf.getParametre(CAT, "Tunnel.PrefereOpenGL", true))
                        return new Fonds.TroisD.Opengl.TunnelOpenGL(gl);
                    else
                        return new Fonds.TroisD.GDI.TunnelGDI(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case TYPE_FOND_NUAGES:
                    if (conf.getParametre(CAT, "Nuages.PrefereOpenGL", true))
                        return new Fonds.TroisD.Opengl.NuagesOpenGL(gl);
                    else
                        return new Fonds.TroisD.GDI.NuagesGDI(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case TYPE_FOND_TERRE:
                    return new Fonds.TroisD.Opengl.TerreOpenGL(gl);

                default:
                    return new Metaballes.Metaballes(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
            }
        }


        /// <summary>
        /// Retourne la saison, (calcul tres approximatif)
        /// </summary>
        /// <returns></returns>
        private SAISON getSaison()
        {
            int forceSaison = conf.getParametre(CAT, "Force saison", -1);
            if (forceSaison != -1)
                // Forcage de la saison
                return (SAISON)forceSaison;

            DateTime date = DateTime.Now;

            int quantieme = date.DayOfYear;
            // Hiver : jusqu'a l'equinoxe de printemps
            if (quantieme < PRINTEMPS)
                return SAISON.HIVER;

            // Printemps: jusqu'au solstice d'ete
            if (quantieme <= ETE)
                return SAISON.PRINTEMPS;

            // Ete: jusqu'a l'equinoxe d'automne
            if (quantieme < AUTOMNE)
                return SAISON.ETE;

            // Automne : jusqu'au solstice d'hiver
            if (quantieme < HIVER)
                return SAISON.AUTOMNE;

            return SAISON.HIVER;
        }

        /// <summary>
        /// Chargement de la fenetre et de ses composants
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onLoad(object sender, System.EventArgs e)
        {
            try
            {
                UpdateStyles();
                switch (conf.getParametre(CAT, "PreferedRenderMode. 0 DIBSECTION, 1 FBO, 2 NATIVE", RENDERMODE_FBO))
                {
                    case RENDERMODE_DIBSECTION: openGLControl.RenderContextType = RenderContextType.DIBSection; break;
                    case RENDERMODE_FBO: openGLControl.RenderContextType = RenderContextType.FBO; break;
                    case RENDERMODE_NATIVE: openGLControl.RenderContextType = RenderContextType.NativeWindow; break;
                }
                _fontHelp = new Font(FontFamily.GenericSansSerif, 20);

                timerChangeFond.Interval = conf.getParametre(CAT, PARAM_DELAI_CHANGE_FOND, 3) * 60 * 1000;
                timerChangeFond.Enabled = true;
#if TRACER
                cpuCounter = new PerformanceCounter();
                cpuCounter.CategoryName = "Processor";
                cpuCounter.CounterName = "% Processor Time";
                cpuCounter.InstanceName = "_Total";
                ramCounter = new PerformanceCounter("Memory", "Available MBytes");
#endif
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\ndans : " + ex.Source, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        /// <summary>
        /// Partie GDI (2D) de l'affichage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void onGDIDraw(object sender, SharpGL.RenderEventArgs args)
        {
            Graphics g = args.Graphics;
            moveAll(g);

            try
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;

                Color Couleur = _couleur.GetRGB();

                // Afficher tous les objets
                foreach (DisplayedObject b in _listeObjets)
                    b.AfficheGDI(g, _temps, Bounds, Couleur);

#if TRACER
                if (_afficheDebug)
                    afficheDebug(g);
#endif

                if (_afficherAide)
                {
                    StringBuilder s = new StringBuilder(Resources.Aide);
                    foreach (DisplayedObject b in _listeObjets)
                        b.AppendHelpText(s);

                    g.DrawString(s.ToString(), _fontHelp, Brushes.White, 10, 10);
                }
                else
                    if (_temps._temps.Subtract(_debut).TotalMilliseconds < 10000)
                    {
                        g.DrawString("Pressez H pour de l'aide", _fontHelp, Brushes.White, 10, 10);
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

#if TRACER
        /// <summary>
        /// Affichage des informations de debug et performance
        /// </summary>
        /// <param name="g"></param>
        private void afficheDebug(Graphics g)
        {
            {
                StringBuilder s = new StringBuilder();

                double NbMillisec = _temps._temps.Subtract(lastFrame).TotalMilliseconds;
#if DEBUG
                s.Append("Version DEBUG ");
#else
                s.Append("Version RELEASE ");
#endif
                s.Append(Assembly.GetExecutingAssembly().GetName().Version).Append("\n\n");

                s.Append((1000.0 / NbMillisec).ToString("0.0") + " FPS\n\n")
                    .Append("Couleur: " + _couleur.ToString() + "\n\n")
                    .Append("CPU " + cpuCounter.NextValue().ToString("00") + "%\n")
                    .Append("Free RAM " + (ramCounter.NextValue() / 1024).ToString("0.00") + "GB\n")
                    .Append("Memory usage " + ((currentProc.PrivateMemorySize64 / 1024.0) / 1024.0).ToString("0.0") + "MB\n\n");

                foreach (DisplayedObject b in _listeObjets)
                    s.Append(b.DumpRender()).Append("\n");

                g.DrawString(s.ToString(), SystemFonts.DefaultFont, Brushes.White, 0, 0);
                lastFrame = _temps._temps;
            }
        }
#endif
        /// <summary>
        /// Deplacer tous les objets
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void moveAll(Graphics g)
        {
            _couleur.AvanceCouleur();

            _temps = new Temps(DateTime.Now, _derniereFrame);

            Rectangle bnd = Bounds;
            foreach (DisplayedObject b in _listeObjets)
                b.Deplace(_temps, ref bnd);

            if (_jourActuel != _temps._JourDeLAnnee)
            {
                // Detection de changement de date, avertir les objets qui sont optimises pour ne changer
                // qu'une fois par jour
                foreach (DisplayedObject b in _listeObjets)
                    b.DateChangee(g, _temps);

                _jourActuel = _temps._JourDeLAnnee;
            }

            _derniereFrame = _temps._temps;
        }

        void onOpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
            // Get the OpenGL object, just to clean up the code.
            OpenGL gl = openGLControl.OpenGL;
            Color Couleur = _couleur.GetRGB();

            // Deplacer et Afficher tous les objets
            foreach (DisplayedObject b in _listeObjets)
            {
                gl.PushAttrib(OpenGL.GL_ENABLE_BIT);
                b.AfficheOpenGL(gl, _temps, Bounds, Couleur);
                gl.PopAttrib();
            }

            gl.End(); // Done Drawing The Q
            gl.Flush();

        }

        /// <summary>
        /// OpenGL est initialise
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onOpenGLInitialized(object sender, System.EventArgs e)
        {
            createAllObjects();

            OpenGL gl = openGLControl.OpenGL;
            gl.Clear(0);
            // Deplacer et Afficher tous les objets
            foreach (DisplayedObject b in _listeObjets)
                b.OpenGLInitialized(gl);
        }


        /// <summary>
        /// Creer tous les objets qui seront affiches
        /// </summary>
        private void createAllObjects()
        {
            int CentreX = Bounds.Width / 2;
            int CentreY = Bounds.Height / 2;

            int TailleHorloge = conf.getParametre(CAT, "TailleCadran", 400);
            if (IsPreviewMode)
            {
                TailleHorloge = 100;
            }

            _fondDeSaison = conf.getParametre(CAT, PARAM_FONDDESAISON, true);
            // Ajout de tous les objets graphiques, en finissant par celui qui sera affiche en dessus des autres
            _listeObjets.Add(createBackgroundObject(conf.getParametre(CAT, PARAM_TYPEFOND, 0), true));

            if (conf.getParametre(CAT, "Copyright", true))
                // Copyright
                _listeObjets.Add(new Textes.TexteCopyright(-4, 100));
            
            // Heure et date numeriques
            if (conf.getParametre(CAT, "Date", true))
                _listeObjets.Add(new Textes.DateTexte(0, 0));
            if (conf.getParametre(CAT, "Heure", true))
                _listeObjets.Add(new Textes.HeureTexte(100, CentreY));

            // Meteo
            if (conf.getParametre(CAT, "Meteo", true))
                _listeObjets.Add(new Meteo.Meteo());

            // citations
            if (conf.getParametre(CAT, "Citation", true))
                _listeObjets.Add(new Textes.Citations(this, 200, 200));

            // Horloge ronde
            if (conf.getParametre(CAT, "HorlogeRonde", true))
                _listeObjets.Add(new HorlogeRonde(TailleHorloge, CentreX - TailleHorloge / 2, CentreY - TailleHorloge / 2));

        }

        void onTimerChangeBackground(object sender, EventArgs e)
        {
            int Type = conf.getParametre(CAT, PARAM_TYPEFOND, 0);
            Type = (Type + 1) % NB_FONDS;
            conf.setParametre(CAT, PARAM_TYPEFOND, Type);

            // Remplacer le premier objet de la liste par le nouveau fond
            _listeObjets[0] = createBackgroundObject(Type, false);
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (!IsPreviewMode) //disable exit functions for preview
            {
                switch ((Keys)e.KeyValue)
                {
                    case Keys.Insert: _couleur.ChangeHue(1); break;
                    case Keys.Delete: _couleur.ChangeHue(-1); break;
                    case Keys.Home: _couleur.ChangeSaturation(1); break;
                    case Keys.End: _couleur.ChangeSaturation(-1); break;
                    case Keys.PageUp: _couleur.ChangeValue(1); break;
                    case Keys.PageDown: _couleur.ChangeValue(-1); break;
                    case Keys.H: _afficherAide = !_afficherAide; break;
                    case Keys.S:
                        {
                            // Changement de mode de fond
                            _fondDeSaison = !_fondDeSaison;
                            conf.setParametre(CAT, PARAM_FONDDESAISON, _fondDeSaison);
                            _listeObjets[0] = createBackgroundObject(conf.getParametre(CAT, PARAM_TYPEFOND, 0), _fondDeSaison);
                        }
                        break;
                    case Keys.F:
                        {
                            // Passage en mode manuel
                            timerChangeFond.Enabled = false;
                            int Type = conf.getParametre(CAT, PARAM_TYPEFOND, 0);
                            Type = (Type + 1) % NB_FONDS;
                            conf.setParametre(CAT, PARAM_TYPEFOND, Type);
                            // Remplacer le premier objet de la liste par le nouveau fond
                            _listeObjets[0] = createBackgroundObject(Type, false);
                        }
                        break;
#if TRACER
                    case Keys.D:
                        {
                            _afficheDebug = !_afficheDebug;
                            conf.setParametre(CAT, "Debug", _afficheDebug);
                        }
                        break;
#endif
                    default:
                        // Proposer la touche a chaque objet affiche
                        bool b = false;
                        foreach (DisplayedObject o in _listeObjets)
                            if (o.KeyDown(this, (Keys)e.KeyValue))
                                b = true;

                        if (!b)
                        {
                            // Touche non reconnue: terminer l'application
                            Cursor.Show();
                            Application.Exit();
                        }
                        break;
                }
            }
        }

        //start off OriginalLoction with an X and Y of int.MaxValue, because
        //it is impossible for the cursor to be at that position. That way, we
        //know if this variable has been set yet.
        Point OriginalLocation = new Point(int.MaxValue, int.MaxValue);
        private System.Drawing.Font _fontHelp;

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsPreviewMode) //disable exit functions for preview
            {
                //see if originallocat5ion has been set
                if (OriginalLocation.X == int.MaxValue & OriginalLocation.Y == int.MaxValue)
                {
                    OriginalLocation = e.Location;
                }
                //see if the mouse has moved more than 20 pixels in any direction. If it has, close the application.
                if (Math.Abs(e.X - OriginalLocation.X) > 20 | Math.Abs(e.Y - OriginalLocation.Y) > 20)
                {
                    Application.Exit();
                }
            }
        }
    }
}
