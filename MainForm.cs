
using ClockScreenSaverGL.DisplayedObjects;
using ClockScreenSaverGL.DisplayedObjects.Fonds;
using ClockScreenSaverGL.DisplayedObjects.Fonds.FontaineParticulesPluie;
using ClockScreenSaverGL.DisplayedObjects.Fonds.Particules;
using ClockScreenSaverGL.DisplayedObjects.Fonds.Printemps;
using ClockScreenSaverGL.DisplayedObjects.Fonds.Saisons.Ete;
using ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Opengl;
using ClockScreenSaverGL.DisplayedObjects.Metaballes;
using ClockScreenSaverGL.DisplayedObjects.Meteo;
using ClockScreenSaverGL.DisplayedObjects.Saisons;
using ClockScreenSaverGL.DisplayedObjects.Textes;
using ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD;
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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClockScreenSaverGL
{

    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form, IDisposable
    {

        #region Parametres
        public const string CAT = "Main";
        const string PARAM_DELAI_CHANGE_FOND = "DelaiChangeFondMinutes";
        const string PARAM_FONDDESAISON = "FondDeSaison";
        const string PARAM_TYPEFOND = "TypeFond";
        private static Config conf = Config.getInstance();
        static readonly int PRINTEMPS = conf.getParametre(CAT, "Printemps", 80);
        static readonly int ETE = conf.getParametre(CAT, "Ete", 172);
        static readonly int AUTOMNE = conf.getParametre(CAT, "Automne", 265);
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
        private bool wireframe = false;

        #region Fonds
        const int TYPE_FOND_ESPACE = 0;
        const int TYPE_FOND_COURONNES = 1;
        const int TYPE_FOND_METABALLES = 2;
        const int TYPE_FOND_NUAGES = 3;
        const int TYPE_FOND_PARTICULES_PLUIE = 4;
        const int TYPE_FOND_CARRES_ESPACE = 5;
        const int TYPE_FOND_ENCRE = 6;
        const int TYPE_FOND_TUNNEL = 7;
        const int TYPE_FOND_NEIGE_META = 8;
        const int TYPE_FOND_LIFE = 9;
        const int TYPE_FOND_TERRE = 10;
        const int TYPE_FOND_BACTERIES = 11;
        const int TYPE_FOND_PARTICULES1 = 12;
        const int TYPE_FOND_COULEUR = 13;
        const int TYPE_FOND_FUSEES = 14;
        const int TYPE_FOND_ARTIFICE = 15;
        const int TYPE_FOND_NOIR = 16;
        const int TYPE_FOND_ATTRACTEUR = 17;
        const int TYPE_FOND_GRAVITE = 18;
        const int TYPE_FOND_ENGRENAGES = 19;


        const int NB_FONDS = TYPE_FOND_ENGRENAGES + 1;
        #endregion

        #region Render Modes
        const int RENDERMODE_DIBSECTION = 0;
        const int RENDERMODE_FBO = 1;
        const int RENDERMODE_NATIVE = 2;
        #endregion

        enum SAISON { HIVER = 0, PRINTEMPS = 1, ETE = 2, AUTOMNE = 3 };
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
        private Fond createBackgroundObject(int Type, bool initial)
        {
               
            OpenGL gl = openGLControl.OpenGL;
            if (!initial)
                gl.PopAttrib();

            gl.PushAttrib(OpenGL.GL_ENABLE_BIT|OpenGL.GL_FOG_BIT|OpenGL.GL_LIGHTING_BIT);
            if (_fondDeSaison && initial)
            {
                // Si l'option 'fond de saison' est selectionnee, l'economiseur commence par celui ci
                // Note: il n'apparaissent plus dans le cycle de changement du fond
                switch (getSaison())
                {
                    case SAISON.HIVER:
                        conf.setParametre(CAT, PARAM_TYPEFOND, TYPE_FOND_ESPACE);
                        return new Hiver(gl);
                    case SAISON.PRINTEMPS:
                        return new Printemps(gl,SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                    case SAISON.ETE:
                        return new Ete(gl, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                    case SAISON.AUTOMNE:
                        return new Automne(gl);
                }
            }
            switch (Type)
            {
                case TYPE_FOND_METABALLES: return new Neige(gl, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case TYPE_FOND_ENCRE: return new Encre(gl, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case TYPE_FOND_BACTERIES: return new Bacteries(gl, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case TYPE_FOND_LIFE: return new Life(gl);
                case TYPE_FOND_NOIR: return new Noir(gl, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case TYPE_FOND_COURONNES: return new Couronnes(gl);
                case TYPE_FOND_COULEUR: return new Couleur(gl, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case TYPE_FOND_ESPACE: return new EspaceOpenGL(gl);
                case TYPE_FOND_TUNNEL: return new Tunnel(gl);
                case TYPE_FOND_CARRES_ESPACE: return new CarresEspace(gl);
                case TYPE_FOND_NUAGES: return new NuagesOpenGL(gl);
                case TYPE_FOND_TERRE: return new TerreOpenGL(gl);
                case TYPE_FOND_PARTICULES1: return new ParticulesGalaxie(gl);
                case TYPE_FOND_PARTICULES_PLUIE: return new FontaineParticulesPluie(gl);
                case TYPE_FOND_FUSEES: return new ParticulesFusees(gl);
                case TYPE_FOND_ARTIFICE: return new FeuDArtifice(gl);
                case TYPE_FOND_ATTRACTEUR: return new AttracteurParticules(gl);
                case TYPE_FOND_GRAVITE: return new Gravite(gl);
                case TYPE_FOND_ENGRENAGES: return new Engrenages(gl);
                default:
                    return new Metaballes(gl, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
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
                /* switch (conf.getParametre(CAT, "PreferedRenderMode. 0 DIBSECTION, 1 FBO, 2 NATIVE", RENDERMODE_FBO))
                 {
                     case RENDERMODE_DIBSECTION: openGLControl.RenderContextType = RenderContextType.DIBSection; break;
                     case RENDERMODE_FBO: openGLControl.RenderContextType = RenderContextType.FBO; break;
                     case RENDERMODE_NATIVE: openGLControl.RenderContextType = RenderContextType.NativeWindow; break;
                 }*/
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

        /*
        /// <summary>
        /// Partie GDI (2D) de l'affichage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void onGDIDraw(object sender, SharpGL.RenderEventArgs args)
        {
            if (openGLControl.RenderContextType == RenderContextType.NativeWindow)
                return;

            Graphics g = args.Graphics;

            try
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                g.TextRenderingHint = TextRenderingHint.SystemDefault;
                g.CompositingQuality = CompositingQuality.HighSpeed;

                Color Couleur = _couleur.GetRGB();

                // Afficher tous les objets
                foreach (DisplayedObject.DisplayedObject b in _listeObjets)
                    b.AfficheGDI(g, _temps, Bounds, Couleur);

#if TRACER
                if (_afficheDebug)
                    afficheDebug(g);
#endif

                if (_afficherAide)
                {
                    StringBuilder s = new StringBuilder(Resources.c);
                    foreach (DisplayedObject.DisplayedObject b in _listeObjets)
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
        */

#if TRACER
        /// <summary>
        /// Affichage des informations de debug et performance
        /// </summary>
        /// <param name="g"></param>
        private void afficheDebug(Graphics g)
        {
            {
                StringBuilder s = new StringBuilder();
                OpenGL gl = openGLControl.OpenGL;
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

                foreach (DisplayedObject.DisplayedObject b in _listeObjets)
                    s.Append(b.DumpRender()).Append("\n");

                g.DrawString(s.ToString(), SystemFonts.DefaultFont, Brushes.LightGray, 0, 10);
                lastFrame = _temps._temps;
            }
        }
#endif
        /// <summary>
        /// Deplacer tous les objets
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void moveAll()
        {
            _couleur.AvanceCouleur();

            _temps = new Temps(DateTime.Now, _derniereFrame);

            Rectangle bnd = Bounds;
            foreach (DisplayedObject b in _listeObjets)
                b.Deplace(_temps, bnd);

            if (_jourActuel != _temps._JourDeLAnnee)
            {
                // Detection de changement de date, avertir les objets qui sont optimises pour ne changer
                // qu'une fois par jour

                OpenGL gl = openGLControl.OpenGL;
                foreach (DisplayedObject b in _listeObjets)
                    b.DateChangee(gl, _temps);

                _jourActuel = _temps._JourDeLAnnee;
            }

            _derniereFrame = _temps._temps;
        }

        void onOpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
            moveAll();

            // Get the OpenGL object, just to clean up the code.
            OpenGL gl = openGLControl.OpenGL;
            Color Couleur = _couleur.GetRGB();
            //gl.Enable(OpenGL.GL_MULTISAMPLE);
            // Deplacer et Afficher tous les objets
            foreach (DisplayedObject b in _listeObjets)
                b.ClearBackGround(gl, Couleur);
            //gl.Hint(OpenGL.GL_LINE_SMOOTH_HINT, OpenGL.GL_NICEST);

            if (wireframe)
            {
                gl.LineWidth(1);
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
            }
            // Deplacer et Afficher tous les objets
            foreach (DisplayedObject b in _listeObjets)
            {
                gl.PushMatrix();
                gl.PushAttrib(OpenGL.GL_ENABLE_BIT);
                b.AfficheOpenGL(gl, _temps, Bounds, Couleur);
                gl.PopAttrib();
                gl.PopMatrix();
            }

            if (wireframe)
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
            //gl.End();
           gl.Finish();

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
        }


        /// <summary>
        /// Creer tous les objets qui seront affiches
        /// </summary>
        private void createAllObjects()
        {
            OpenGL gl = openGLControl.OpenGL;
            int CentreX = Bounds.Width / 2;
            int CentreY = Bounds.Height / 2;
            bool meteoADroite = new Random().Next(0, 2) > 0 ;
            int TailleHorloge = conf.getParametre(CAT, "TailleCadran", 400);
            if (IsPreviewMode)
            {
                TailleHorloge = 10;
                _listeObjets.Add(new HorlogeRonde(gl, true, TailleHorloge, CentreX - TailleHorloge / 2, CentreY - TailleHorloge / 2));
                return;
            }

            _fondDeSaison = conf.getParametre(CAT, PARAM_FONDDESAISON, true);
            // Ajout de tous les objets graphiques, en finissant par celui qui sera affiche en dessus des autres
            _listeObjets.Add(createBackgroundObject(conf.getParametre(CAT, PARAM_TYPEFOND, 0), true));

            if (conf.getParametre(CAT, "Copyright", true))
                // Copyright
                _listeObjets.Add(new TexteCopyright(gl, -4, 100));
            // citations
            if (conf.getParametre(CAT, "Citation", true))
                _listeObjets.Add(new Citations(gl, this, 200, 200));
            // Heure et date numeriques
         /*   if (conf.getParametre(CAT, "Date", true))
                _listeObjets.Add(new DateTexte(gl, 0, 0));
            if (conf.getParametre(CAT, "Heure", true))
                _listeObjets.Add(new HeureTexte(gl, 100, CentreY));*/
            /*
            if (conf.getParametre(CAT, "Deezer", true))
                _listeObjets.Add(new DeezerInfo(CentreX, CentreY));
            */
            // Meteo
            if (conf.getParametre(CAT, "Meteo", true))
                _listeObjets.Add(new PanneauInfos(gl, meteoADroite));



            // Horloge ronde
            //if (conf.getParametre(CAT, "HorlogeRonde", true))
            //    _listeObjets.Add(new HorlogeRonde(gl, ! meteoADroite, TailleHorloge, CentreX - TailleHorloge / 2, CentreY - TailleHorloge / 2));

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
                    case DisplayedObject.TOUCHE_REINIT:
                        _listeObjets[0] = createBackgroundObject(conf.getParametre(CAT, PARAM_TYPEFOND, 0), _fondDeSaison);
                        timerChangeFond.Stop();
                        timerChangeFond.Start();
                        break;

                    case DisplayedObject.TOUCHE_WIREFRAME:
                        wireframe = !wireframe;
                        break;

                    case DisplayedObject.TOUCHE_DE_SAISON:
                        {
                            // Changement de mode de fond
                            _fondDeSaison = !_fondDeSaison;
                            conf.setParametre(CAT, PARAM_FONDDESAISON, _fondDeSaison);
                            _listeObjets[0] = createBackgroundObject(conf.getParametre(CAT, PARAM_TYPEFOND, 0), _fondDeSaison);
                        }
                        break;
                    case DisplayedObject.TOUCHE_PROCHAIN_FOND:
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
