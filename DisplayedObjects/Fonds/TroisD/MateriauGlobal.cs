using ClockScreenSaverGL.Config;
using SharpGL;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD
{
    public abstract class  MateriauGlobal : TroisD
    {
        const String CONF_LIGHT_AMBIENT = "Material Light Ambient";
        const String CONF_LIGHT_DIFFUSE = "Material Light Diffuse";
        const String CONF_LIGHT_SPECULAR = "Material Light Specular";

        const String CONF_COL_AMBIENT = "Material Ambient";
        const String CONF_COL_DIFFUSE = "Material Diffuse";
        const String CONF_COL_SPECULAR = "Material Specular";
        const String CONF_COL_COLOR = "Material Color";
        const String CONF_SHININESS = "Material Shininess";
        
        protected enum VALEUR_MODIFIEE { LAMBIENT = 0, LDIFFUSE = 1, LSPECULAR = 2, AMBIENT = 3, DIFFUSE = 4, SPECULAR = 5, COLOR = 6, SHININESS = 7 };
        protected float[] COL_AMBIENT = { 0.21f, 0.12f, 0.05f, 1.0f };
        protected float[] COL_DIFFUSE = { 0.7f, 0.72f, 0.78f, 1.0f };
        protected float[] COL_SPECULAR = { 0.7f, 0.7f, 0.7f, 1.0f };
        protected float[] COL_COLOR = { 0.7f, 0.7f, 0.7f };
        protected float SHININESS = 18f;

        protected float[] LIGHTPOS = { -2, 1.5f, -2.5f, 1 };
        protected float[] LIG_SPECULAR = { 1.0f, 1.0f, 1.0f };
        protected float[] LIG_AMBIENT = { 0.5f, 0.5f, 0.5f };
        protected float[] LIG_DIFFUSE = { 1.0f, 1.0f, 1.0f };
        protected float RATIO_COULEUR = 1.0f / 256.0f;
        
        protected VALEUR_MODIFIEE valModifie = VALEUR_MODIFIEE.AMBIENT;
        public MateriauGlobal(OpenGL gl) : base(gl, 0, 0, 0, 0)
        {
            getConfiguration()?.setListenerParametreChange(onConfigurationChangee);
            onConfigurationChangee(null);            
        }

        /// <summary>
        /// Notification: valeurs de configuration changees
        /// </summary>
        /// <param name="valeur"></param>
        protected override void onConfigurationChangee(string valeur)
        {
            CategorieConfiguration c = getConfiguration();

            float val = c.getParametre(CONF_LIGHT_AMBIENT, 0.5f, true);
            LIG_AMBIENT[0] = val;
            LIG_AMBIENT[1] = val;
            LIG_AMBIENT[2] = val;

            val = c.getParametre(CONF_LIGHT_DIFFUSE, 1.0f, true);
            LIG_DIFFUSE[0] = val;
            LIG_DIFFUSE[1] = val;
            LIG_DIFFUSE[2] = val;

            val = c.getParametre(CONF_LIGHT_SPECULAR, 1.0f, true);
            LIG_SPECULAR[0] = val;
            LIG_SPECULAR[1] = val;
            LIG_SPECULAR[2] = val;

            val = c.getParametre(CONF_COL_AMBIENT, 0.2f, true);
            COL_AMBIENT[0] = val;
            COL_AMBIENT[1] = val;
            COL_AMBIENT[2] = val;

            val = c.getParametre(CONF_COL_DIFFUSE, 0.2f, true);
            COL_DIFFUSE[0] = val;
            COL_DIFFUSE[1] = val;
            COL_DIFFUSE[2] = val;

            val = c.getParametre(CONF_COL_SPECULAR, 0.7f, true);
            COL_SPECULAR[0] = val;
            COL_SPECULAR[1] = val;
            COL_SPECULAR[2] = val;

            val = c.getParametre(CONF_COL_COLOR, 0.7f, true);
            COL_COLOR[0] = val;
            COL_COLOR[1] = val;
            COL_COLOR[2] = val;

            SHININESS = c.getParametre(CONF_SHININESS, 45f, true);
        }

        protected void setGlobalMaterial(OpenGL gl, Color couleur)
        {
            // Lumiere
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, LIGHTPOS);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, LIG_SPECULAR);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, LIG_AMBIENT);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, LIG_DIFFUSE);

            // Aspect de la surface
            gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SPECULAR, COL_SPECULAR);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, COL_AMBIENT);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, COL_DIFFUSE);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SHININESS, SHININESS);

            gl.Color(COL_COLOR[0] * (float)couleur.R / 256.0f, COL_COLOR[1] * (float)couleur.G / 256.0f, COL_COLOR[2] * (float)couleur.B / 256.0f);
        }

        protected void setGlobalMaterial(OpenGL gl, float R, float G, float B)
        {
            // Lumiere
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, LIGHTPOS);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, LIG_SPECULAR);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, LIG_AMBIENT);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, LIG_DIFFUSE);

            // Aspect de la surface
            gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SPECULAR, COL_SPECULAR);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, COL_AMBIENT);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, COL_DIFFUSE);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SHININESS, SHININESS);

            gl.Color(COL_COLOR[0] * R, COL_COLOR[1] * G, COL_COLOR[2] * B);
        }
    }

}
