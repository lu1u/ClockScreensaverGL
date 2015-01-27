/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 23/01/2015
 * Heure: 17:09
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
namespace ClockScreenSaverGL
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}

                if (_fontHelp != null)
                    _fontHelp.Dispose();
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.openGLControl = new SharpGL.OpenGLControl();
			this.timerChangeFond = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.openGLControl)).BeginInit();
			this.SuspendLayout();
			// 
			// openGLControl
			// 
			this.openGLControl.BackColor = System.Drawing.Color.Black;
			this.openGLControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.openGLControl.DrawFPS = true;
			this.openGLControl.FrameRate = 30;
			this.openGLControl.Location = new System.Drawing.Point(0, 0);
			this.openGLControl.Name = "openGLControl";
			this.openGLControl.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
			this.openGLControl.RenderContextType = SharpGL.RenderContextType.FBO;
			this.openGLControl.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
			this.openGLControl.Size = new System.Drawing.Size(951, 652);
			this.openGLControl.TabIndex = 0;
			this.openGLControl.OpenGLInitialized += new System.EventHandler(this.onOpenGLInitialized);
			this.openGLControl.OpenGLDraw += new SharpGL.RenderEventHandler(this.OnOpenGLDraw);
			this.openGLControl.GDIDraw += new SharpGL.RenderEventHandler(this.onGDIDraw);
			this.openGLControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
			this.openGLControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
			// 
			// timerChangeFond
			// 
			this.timerChangeFond.Enabled = true;
			this.timerChangeFond.Interval = 180000;
			this.timerChangeFond.Tick += new System.EventHandler(this.onTimerChangeFond);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(951, 652);
			this.ControlBox = false;
			this.Controls.Add(this.openGLControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ClockScreenSaverGL";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.onLoad);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
			((System.ComponentModel.ISupportInitialize)(this.openGLControl)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Timer timerChangeFond;
		private SharpGL.OpenGLControl openGLControl;

		

	}
}
