﻿/*
 * Created by SharpDevelop.
 * User: lucien
 * Date: 21/06/2014
 * Time: 23:03
 * 
 * To change this template use Tools  Options  Coding  Edit Standard Headers.
 */
using ClockScreenSaverGL.Config;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
namespace ClockScreenSaverGL.DisplayedObjects.Textes
{


    /// <summary>
    /// Description of HeureTexte.
    /// </summary>
    public class HeureTexte : Texte
    {
        const string CAT = "HeureTexte";
        static protected CategorieConfiguration c = Config.Configuration.getCategorie(CAT);
        static int HAUTEUR_FONTE = c.getParametre("TailleFonte", 80);
        //readonly float RATIO_FONTE = 0.75f;// c.getParametre("RatioFonte", 0.70f);
        //const int NB_SYMBOLES = 11;
        //Texture[] _symboles = new Texture[NB_SYMBOLES];
        //readonly int[] _largeurChiffre = new int[NB_SYMBOLES];
        //readonly float _hauteurChiffre;
        readonly OpenGLFonte _glFonte;
        public HeureTexte(OpenGL gl, int Px, int Py)
            : base(gl, Px, SystemInformation.VirtualScreen.Height - HAUTEUR_FONTE * 2, -c.getParametre( "VX", 15 ), 0, c.getParametre( "TailleFonte", 80 ), c.getParametre( "Alpha", (byte) 180 ) )
        {
            _glFonte = new OpenGLFonte(gl, "?0123456789:", HAUTEUR_FONTE, FontFamily.GenericSansSerif, FontStyle.Bold);
        }
        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
        public HeureTexte(OpenGL gl, int Px, int Py, int tailleFonte)
           : base(gl, Px, SystemInformation.VirtualScreen.Height - HAUTEUR_FONTE * 2, 0, 0, tailleFonte, c.getParametre( "Alpha", (byte) 180 ) )
        {
            HAUTEUR_FONTE = tailleFonte;
            _glFonte = new OpenGLFonte(gl, "?0123456789:", HAUTEUR_FONTE, FontFamily.GenericSansSerif, FontStyle.Bold);
        }

        protected override Font CreerFonte(int tailleFonte)
        {
            return new Font(FontFamily.GenericSansSerif, tailleFonte, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
            base.Deplace(maintenant, tailleEcran);
            tailleEcran = new Rectangle(tailleEcran.Left, tailleEcran.Top, tailleEcran.Width, tailleEcran.Height - (int)_taille.Height);
        }

        protected override SizeF getTexte(Temps maintenant, out string texte)
        {
            texte = maintenant.heure + ":"
                + maintenant.minute.ToString("D2") + ":"
                + maintenant.seconde.ToString("D2") + ":"
                + maintenant.milliemesDeSecondes.ToString("D3");
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                return g.MeasureString(texte, _fonte);
        }

        protected override bool TexteChange() { return false; }

        /// <summary>
        /// Appelee quand la date change: mettre la date a jour
        /// </summary>
        /// <param name="g"></param>
        /// <param name="maintenant"></param>
        public override void DateChangee(OpenGL gl, Temps maintenant)
        {

        }

        protected override void drawOpenGL(OpenGL gl, Rectangle tailleEcran, Color couleur, Temps maintenant)
        {
            string texte = maintenant.heure + ":"
                + maintenant.minute.ToString("D2") + ":"
                + maintenant.seconde.ToString("D2") + ":"
                + maintenant.milliemesDeSecondes.ToString("D3");
           _glFonte.drawOpenGL(gl, texte, _trajectoire._Px, _trajectoire._Py, couleur);
        }
    }
}
