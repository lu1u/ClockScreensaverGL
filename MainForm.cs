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
		public const string CAT = "Main" ;
		const string DELAI_CHANGE_FOND_MINUTES = "DelaiChangeFondMinutes" ;
		private  static Config conf = Config.getInstance() ;
		#endregion
		CouleurGlobale couleur = new CouleurGlobale() ;
		private List<DisplayedObject> listeObjets = new List<DisplayedObject>();
		private int JourActuel = -1 ; // Pour forcer un changement de date avant la premiere image
		private bool AfficherHelp = false ;
		
		const string PARAM_FONDDESAISON = "FondDeSaison" ;
		const string PARAM_TYPEFOND = "TypeFond" ;
		
		private bool _fondDeSaison  ;
		DateTime _derniereFrame = DateTime.Now ;
		DateTime _debut =  DateTime.Now ;
		Temps _temps ;
		const int NB_FONDS = 11 ;
		
		#if TRACER
		bool AfficheDebug = conf.getParametre(CAT, "Debug", true );
		DateTime lastFrame = DateTime.Now ;
		
		Process currentProc = Process.GetCurrentProcess();
		PerformanceCounter cpuCounter;
		PerformanceCounter ramCounter;
		#endif
		#region Preview API's

		[DllImport("user32.dll")]
		static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll",SetLastError = true)]
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
			InitializeComponent();
			
			_temps = new Temps( DateTime.Now, _derniereFrame ) ;
			_fondDeSaison = conf.getParametre( CAT, PARAM_FONDDESAISON, true ) ;
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
				MessageBox.Show(ex.Message + "\ndans : " + ex.Source, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error ) ;
				Application.Exit() ;
			}
		}

		#endregion

		/// <summary>
		/// Creer l'objet qui anime le fond d'ecran
		/// </summary>
		/// <returns></returns>
		private Fonds.Fond CreerObjetFond( int Type, bool initial )
		{
			OpenGL gl = openGLControl.OpenGL;
			if ( _fondDeSaison && initial)
			{
				return new Fonds.TroisD.Neige() ;
			}
			else
			{
				switch( Type )
				{
						case 1 : return new Metaballes.Neige(SystemInformation.VirtualScreen.Width,SystemInformation.VirtualScreen.Height) ;
						case 2 : return new Metaballes.Encre(SystemInformation.VirtualScreen.Width,SystemInformation.VirtualScreen.Height) ;
						case 3 : return new Metaballes.Bacteries(SystemInformation.VirtualScreen.Width,SystemInformation.VirtualScreen.Height) ;
						case 4 : return new Fonds.Life() ;
						case 5 : return new Fonds.Noir(SystemInformation.VirtualScreen.Width,SystemInformation.VirtualScreen.Height) ;
						case 6 : return new Fonds.Couleur(SystemInformation.VirtualScreen.Width,SystemInformation.VirtualScreen.Height) ;
						case 7 : return new Fonds.TroisD.Espace(gl) ;
						case 8 : return new Fonds.TroisD.Tunnel() ;
						case 9 : return new Fonds.TroisD.Nuages(gl) ;
						case 10: return new Fonds.TroisD.Neige() ;
						default :
							return new Metaballes.Metaballes(SystemInformation.VirtualScreen.Width,SystemInformation.VirtualScreen.Height) ;
				}
			}
			
			
		}
		void onLoad(object sender, System.EventArgs e)
		{
			try
			{
				UpdateStyles();
				#if TRACER
				cpuCounter = new PerformanceCounter();
				cpuCounter.CategoryName = "Processor";
				cpuCounter.CounterName = "% Processor Time";
				cpuCounter.InstanceName = "_Total";

				ramCounter = new PerformanceCounter("Memory","Available MBytes");
				#endif
			}
			catch (Exception ex)
			{
				//timerMain.Enabled = false ;
				MessageBox.Show(ex.Message + "\ndans : " + ex.Source, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error ) ;
				Application.Exit() ;
			}
		}
		void onGDIDraw(object sender, SharpGL.RenderEventArgs args)
		{
			Graphics g = args.Graphics ;
			DeplaceTous(g) ;
			RectangleF r = g.ClipBounds ;
			try
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				g.TextRenderingHint = TextRenderingHint.AntiAlias ;
				g.CompositingQuality = CompositingQuality.HighQuality ;
				
				Color Couleur = couleur.GetRGB() ;
				
				// Deplacer et Afficher tous les objets
				foreach( DisplayedObject b in listeObjets)
					b.AfficheGDI( g, _temps, Bounds, Couleur) ;
				

				#if TRACER
				// Afficher les informations de DEBUG
				if (AfficheDebug)
				{
					StringBuilder s = new StringBuilder() ;
					
					double NbMillisec = _temps._temps.Subtract( lastFrame ).TotalMilliseconds ;
					#if DEBUG
					s.Append("Version DEBUG " ) ;
					#else
					s.Append("Version RELEASE " ) ;
					#endif
					s.Append(Assembly.GetExecutingAssembly().GetName().Version).Append("\n\n");

					s.Append( (1000.0/NbMillisec).ToString( "0.0") + " FPS\n\n")
						.Append("Couleur: " + couleur.ToString() + "\n\n")
						.Append( "CPU " + cpuCounter.NextValue().ToString("00")+"%\n")
						.Append( "Free RAM " + (ramCounter.NextValue()/1024).ToString("0.00")+"GB\n")
						.Append( "Memory usage " + ((currentProc.PrivateMemorySize64/1024.0)/1024.0).ToString("0.0") + "MB\n\n") ;
					
					foreach( DisplayedObject b in listeObjets)
						s.Append( b.DumpRender()).Append("\n" ) ;
					
					g.DrawString( s.ToString(), SystemFonts.DefaultFont, Brushes.White, 0, 0 ) ;
					lastFrame = _temps._temps ;
				}
				#endif
				if (_fontHelp == null)
						_fontHelp = new Font(FontFamily.GenericSansSerif, 20);
					
				if  (AfficherHelp )
				{
					StringBuilder s = new StringBuilder(Resources.Aide) ;
					foreach( DisplayedObject b in listeObjets)
						b.AppendHelpText(s) ;
					
					g.DrawString( s.ToString(), _fontHelp, Brushes.White, 10, 10 ) ;
				}
				else
				if ( _temps._temps.Subtract(_debut).TotalMilliseconds < 100 )
				{
					g.DrawString( "Pressez H pour de l'aide", _fontHelp, Brushes.White, 10, 10 ) ;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error ) ;
				Application.Exit() ;
			}
		}
		
		/// <summary>
		/// Reception du timer: refaire l'affichage
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void DeplaceTous(Graphics g)
		{
			couleur.AvanceCouleur() ;
			
			_temps = new Temps( DateTime.Now, _derniereFrame ) ;
			
			foreach( DisplayedObject b in listeObjets)
				b.Deplace( _temps, Bounds) ;
			
			if ( JourActuel != _temps._JourDeLAnnee )
			{
				// Detection de changement de date, avertir les objets qui sont optimises pour ne changer
				// qu'une fois par jour
				foreach( DisplayedObject b in listeObjets)
					b.DateChangee( g, _temps ) ;
				
				JourActuel = _temps._JourDeLAnnee ;
			}
			
			_derniereFrame = _temps._temps ;
		}
		
		void OnOpenGLDraw(object sender, SharpGL.RenderEventArgs args)
		{
			// Get the OpenGL object, just to clean up the code.
			OpenGL gl = openGLControl.OpenGL;
			Color Couleur = couleur.GetRGB() ;
			
			// Deplacer et Afficher tous les objets
			foreach( DisplayedObject b in listeObjets)
				b.AfficheOpenGL( gl, _temps, Bounds, Couleur) ;
			

			gl.End(); // Done Drawing The Q
			gl.Flush();
			
		}
		
		void onOpenGLInitialized(object sender, System.EventArgs e)
		{
			CreerObjetsGraphiques() ;
			
			OpenGL gl = openGLControl.OpenGL;
			gl.Clear(0) ;
			// Deplacer et Afficher tous les objets
			foreach( DisplayedObject b in listeObjets)
				b.OpenGLInitialized( gl) ;
		}
		
		private void CreerObjetsGraphiques()
		{
			int CentreX = Bounds.Width/2 ;
			int CentreY = Bounds.Height/2 ;
			
			int TailleHorloge = conf.getParametre(CAT, "TailleCadran", 500 ) ;
			if (IsPreviewMode)
			{
				TailleHorloge = 100 ;
			}
			
			_fondDeSaison = conf.getParametre( CAT, PARAM_FONDDESAISON, true ) ;
			// Ajout de tous les objets graphiques, en finissant par celui qui sera affiche en dessus des autres
			listeObjets.Add( CreerObjetFond( conf.getParametre(CAT, PARAM_TYPEFOND, 0), true) ) ;
			
			
			// Copyright
			listeObjets.Add( new Textes.TexteCopyright(-4, 100) );
			
			
			// citations
			listeObjets.Add( new Textes.Citations( this, 200, 200 )) ;
			
			// Heure et date numeriques
			listeObjets.Add( new Textes.DateTexte(0, 0 )) ;
			listeObjets.Add( new Textes.HeureTexte(0, CentreY)) ;
			
			// Horloge ronde
			listeObjets.Add( new HorlogeRonde( TailleHorloge, CentreX-TailleHorloge/2, CentreY-TailleHorloge/2)) ;
			

		}
		
		void onTimerChangeFond(object sender, EventArgs e)
		{
			int Type = conf.getParametre(CAT, PARAM_TYPEFOND, 0) ;
			Type = (Type+1) % NB_FONDS ;
			conf.setParametre(CAT, PARAM_TYPEFOND, Type) ;
			
			// Remplacer le premier objet de la liste par le nouveau fond
			listeObjets[0] = CreerObjetFond(Type, false) ;
		}
		
		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (!IsPreviewMode) //disable exit functions for preview
			{
				switch ((Keys)e.KeyValue )
				{
						case Keys.Insert : couleur.ChangeHue(1) ;  break ;
						case Keys.Delete : couleur.ChangeHue(-1) ;  break ;
						case Keys.Home : couleur.ChangeSaturation(1) ;  break ;
						case Keys.End : couleur.ChangeSaturation(-1) ;  break ;
						case Keys.PageUp : couleur.ChangeValue(1) ;  break ;
						case Keys.PageDown : couleur.ChangeValue(-1) ;  break ;
						case Keys.H : AfficherHelp = ! AfficherHelp ; break ;
					case Keys.S :
						{
							// Changement de mode de fond
							_fondDeSaison = !_fondDeSaison ;
							conf.setParametre( CAT, PARAM_FONDDESAISON, _fondDeSaison ) ;
							
							listeObjets[0] = CreerObjetFond(conf.getParametre(CAT, PARAM_TYPEFOND, 0), false) ;
						}
						break ;
					case Keys.F :
						{
							// Passage en mode manuel
							timerChangeFond.Enabled = false ;
							int Type = conf.getParametre(CAT, PARAM_TYPEFOND, 0) ;
							Type = (Type+1) % NB_FONDS ;
							conf.setParametre(CAT, PARAM_TYPEFOND, Type ) ;
							
							// Remplacer le premier objet de la liste par le nouveau fond
							listeObjets[0] = CreerObjetFond(Type, false) ;
						}
						break ;
						#if TRACER
					case Keys.D :
						{
							AfficheDebug = ! AfficheDebug ;
							conf.setParametre(CAT, "Debug", AfficheDebug);
						}
						break ;
						#endif
					default:
						bool b = false ;
						foreach( DisplayedObject o in listeObjets )
							if (o.KeyDown(this, (Keys)e.KeyValue))
								b = true ;
						
						if ( ! b)
						{
							Cursor.Show();
							Application.Exit();
						}
						break ;
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
			
			timerChangeFond.Interval = conf.getParametre(CAT, DELAI_CHANGE_FOND_MINUTES, 3 ) * 60 * 1000 ;
			timerChangeFond.Enabled = true ;
		}
	}
}
