using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD
{
    public class MateriauGlobal : TroisD
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
        protected readonly String _CAT;

        protected VALEUR_MODIFIEE valModifie = VALEUR_MODIFIEE.AMBIENT;
        public MateriauGlobal(OpenGL gl, String cat) : base(gl, 0, 0, 0, 0)
        {
            _CAT = cat;

            float val = conf.getParametre(_CAT, CONF_LIGHT_AMBIENT, 0.5f);
            LIG_AMBIENT[0] = val;
            LIG_AMBIENT[1] = val;
            LIG_AMBIENT[2] = val;

            val = conf.getParametre(_CAT, CONF_LIGHT_DIFFUSE, 1.0f);
            LIG_DIFFUSE[0] = val;
            LIG_DIFFUSE[1] = val;
            LIG_DIFFUSE[2] = val;

            val = conf.getParametre(_CAT, CONF_LIGHT_SPECULAR, 1.0f);
            LIG_SPECULAR[0] = val;
            LIG_SPECULAR[1] = val;
            LIG_SPECULAR[2] = val;

            val = conf.getParametre(_CAT, CONF_COL_AMBIENT, 0.2f);
            COL_AMBIENT[0] = val;
            COL_AMBIENT[1] = val;
            COL_AMBIENT[2] = val;

            val = conf.getParametre(_CAT, CONF_COL_DIFFUSE, 0.2f);
            COL_DIFFUSE[0] = val;
            COL_DIFFUSE[1] = val;
            COL_DIFFUSE[2] = val;

            val = conf.getParametre(_CAT, CONF_COL_SPECULAR, 0.7f);
            COL_SPECULAR[0] = val;
            COL_SPECULAR[1] = val;
            COL_SPECULAR[2] = val;

            val = conf.getParametre(_CAT, CONF_COL_COLOR, 0.7f);
            COL_COLOR[0] = val;
            COL_COLOR[1] = val;
            COL_COLOR[2] = val;

            SHININESS = conf.getParametre(_CAT, CONF_SHININESS, 45f);
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
            //gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT | OpenGL.GL_DIFFUSE | OpenGL.GL_SPECULAR);
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

        protected void fillConsole(OpenGL gl)
        {
            Console c = Console.getInstance(gl);
            c.AddLigne(Color.Green, "TerreOpenGL");
            c.AddLigne(Color.Green, MarqueurIf(VALEUR_MODIFIEE.LAMBIENT)  + "L Ambient  " + Tableau( LIG_AMBIENT ) );
            c.AddLigne(Color.Green, MarqueurIf(VALEUR_MODIFIEE.LDIFFUSE)  + "L Diffuse    " + Tableau(LIG_DIFFUSE));
            c.AddLigne(Color.Green, MarqueurIf(VALEUR_MODIFIEE.LSPECULAR) + "L Specular " + Tableau(LIG_SPECULAR));

            c.AddLigne(Color.Green, MarqueurIf(VALEUR_MODIFIEE.AMBIENT)   + "Ambient      " + Tableau(COL_AMBIENT));
            c.AddLigne(Color.Green, MarqueurIf(VALEUR_MODIFIEE.DIFFUSE)   + "Diffuse        " + Tableau(COL_DIFFUSE));
            c.AddLigne(Color.Green, MarqueurIf(VALEUR_MODIFIEE.SPECULAR)  + "Specular     " + Tableau(COL_SPECULAR));
            c.AddLigne(Color.Green, MarqueurIf(VALEUR_MODIFIEE.COLOR)     + "Color            " + Tableau(COL_COLOR));
            c.AddLigne(Color.Green, MarqueurIf(VALEUR_MODIFIEE.SHININESS) + "Shininess    " + SHININESS);
        }

        private string Tableau(float[] tab)
        {
            return tab[0].ToString("0.000") + " : " + tab[1].ToString("0.000") + " : " + tab[2].ToString("0.000") ;
        }

        private string MarqueurIf(VALEUR_MODIFIEE val)
        {
            return (val == valModifie ? ">>" : "     ");
        }

        /// <summary>
        /// Pression sur une touche, retourner true si l'objet a traite, false = fin de l'economiseur
        /// </summary>
        /// <param name="f"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        ///
        public override bool KeyDown(Form f, Keys k)
        {
            switch (k)
            {
                case Keys.Insert:
                    {
                        int n = (int)valModifie - 1;
                        if (n >= 0)
                            valModifie = (VALEUR_MODIFIEE)n;
                        else
                            valModifie = VALEUR_MODIFIEE.SHININESS;
                        return true;
                    }

                case Keys.Delete:
                    {
                        int n = (int)valModifie + 1;
                        if (n <= (int)VALEUR_MODIFIEE.SHININESS)
                            valModifie = (VALEUR_MODIFIEE)n;
                        else
                            valModifie = (VALEUR_MODIFIEE)0;
                        return true;
                    }

                case Keys.Home:
                    {
                        switch (valModifie)
                        {
                            case VALEUR_MODIFIEE.LAMBIENT:
                                LIG_AMBIENT[0] *= 1.1f;
                                LIG_AMBIENT[1] *= 1.1f;
                                LIG_AMBIENT[2] *= 1.1f;
                                conf.setParametre(_CAT, CONF_LIGHT_AMBIENT, LIG_AMBIENT[0]);
                                break;
                            case VALEUR_MODIFIEE.LDIFFUSE:
                                LIG_DIFFUSE[0] *= 1.1f;
                                LIG_DIFFUSE[1] *= 1.1f;
                                LIG_DIFFUSE[2] *= 1.1f;
                                conf.setParametre(_CAT, CONF_LIGHT_DIFFUSE, LIG_DIFFUSE[0]);
                                break;
                            case VALEUR_MODIFIEE.LSPECULAR:
                                LIG_SPECULAR[0] *= 1.1f;
                                LIG_SPECULAR[1] *= 1.1f;
                                LIG_SPECULAR[2] *= 1.1f;
                                conf.setParametre(_CAT, CONF_LIGHT_SPECULAR, LIG_SPECULAR[0]);
                                break;
                            case VALEUR_MODIFIEE.AMBIENT:
                                COL_AMBIENT[0] *= 1.1f;
                                COL_AMBIENT[1] *= 1.1f;
                                COL_AMBIENT[2] *= 1.1f;
                                conf.setParametre(_CAT, CONF_COL_AMBIENT, COL_AMBIENT[0]);
                                break;
                            case VALEUR_MODIFIEE.DIFFUSE:
                                COL_DIFFUSE[0] *= 1.1f;
                                COL_DIFFUSE[1] *= 1.1f;
                                COL_DIFFUSE[2] *= 1.1f;
                                conf.setParametre(_CAT, CONF_COL_DIFFUSE, COL_DIFFUSE[0]);
                                break;
                            case VALEUR_MODIFIEE.SPECULAR:
                                COL_SPECULAR[0] *= 1.1f;
                                COL_SPECULAR[1] *= 1.1f;
                                COL_SPECULAR[2] *= 1.1f;
                                conf.setParametre(_CAT, CONF_COL_SPECULAR, COL_SPECULAR[0]);
                                break;
                            case VALEUR_MODIFIEE.COLOR:
                                COL_COLOR[0] *= 1.1f;
                                COL_COLOR[1] *= 1.1f;
                                COL_COLOR[2] *= 1.1f;
                                conf.setParametre(_CAT, CONF_COL_COLOR, COL_COLOR[0]);
                                break;
                            case VALEUR_MODIFIEE.SHININESS:
                                SHININESS++;
                                conf.setParametre(_CAT, CONF_SHININESS, SHININESS);
                                break;
                        }

                        conf.flush(_CAT);
                        return true;
                    }

                case Keys.End:
                    {
                        switch (valModifie)
                        {
                            case VALEUR_MODIFIEE.LAMBIENT:
                                LIG_AMBIENT[0] /= 1.1f;
                                LIG_AMBIENT[1] /= 1.1f;
                                LIG_AMBIENT[2] /= 1.1f;
                                conf.setParametre(_CAT, CONF_LIGHT_AMBIENT, LIG_AMBIENT[0]);
                                break;
                            case VALEUR_MODIFIEE.LDIFFUSE:
                                LIG_DIFFUSE[0] /= 1.1f;
                                LIG_DIFFUSE[1] /= 1.1f;
                                LIG_DIFFUSE[2] /= 1.1f;
                                conf.setParametre(_CAT, CONF_LIGHT_DIFFUSE, LIG_DIFFUSE[0]);
                                break;
                            case VALEUR_MODIFIEE.LSPECULAR:
                                LIG_SPECULAR[0] /= 1.1f;
                                LIG_SPECULAR[1] /= 1.1f;
                                LIG_SPECULAR[2] /= 1.1f;
                                conf.setParametre(_CAT, CONF_LIGHT_SPECULAR, LIG_SPECULAR[0]);
                                break;
                            case VALEUR_MODIFIEE.AMBIENT:
                                COL_AMBIENT[0] /= 1.1f;
                                COL_AMBIENT[1] /= 1.1f;
                                COL_AMBIENT[2] /= 1.1f;
                                conf.setParametre(_CAT, CONF_COL_AMBIENT, COL_AMBIENT[0]);
                                break;
                            case VALEUR_MODIFIEE.DIFFUSE:
                                COL_DIFFUSE[0] /= 1.1f;
                                COL_DIFFUSE[1] /= 1.1f;
                                COL_DIFFUSE[2] /= 1.1f;
                                conf.setParametre(_CAT, CONF_COL_DIFFUSE, COL_DIFFUSE[0]);
                                break;
                            case VALEUR_MODIFIEE.SPECULAR:
                                COL_SPECULAR[0] /= 1.1f;
                                COL_SPECULAR[1] /= 1.1f;
                                COL_SPECULAR[2] /= 1.1f;
                                conf.setParametre(_CAT, CONF_COL_SPECULAR, COL_SPECULAR[0]);
                                break;
                            case VALEUR_MODIFIEE.COLOR:
                                COL_COLOR[0] /= 1.1f;
                                COL_COLOR[1] /= 1.1f;
                                COL_COLOR[2] /= 1.1f;
                                conf.setParametre(_CAT, CONF_COL_COLOR, COL_COLOR[0]);
                                break;
                            case VALEUR_MODIFIEE.SHININESS:
                                SHININESS--;
                                conf.setParametre(_CAT, CONF_SHININESS, SHININESS);
                                break;
                        }
                        conf.flush(_CAT);
                        return true;
                    }
                /*
            case Keys.Insert:
                {
                    COL_AMBIENT[0] *= 1.1f;
                    COL_AMBIENT[1] *= 1.1f;
                    COL_AMBIENT[2] *= 1.1f;
                    conf.setParametre(CAT, "Material.Ambient", COL_AMBIENT[0]);
                    return true;
                }
            case Keys.Delete:
                {
                    COL_AMBIENT[0] *= 0.9f;
                    COL_AMBIENT[1] *= 0.9f;
                    COL_AMBIENT[2] *= 0.9f;
                    conf.setParametre(CAT, "Material.Ambient", COL_AMBIENT[0]);
                    return true;
                }

            case Keys.Home:
                {
                    COL_DIFFUSE[0] *= 1.1f;
                    COL_DIFFUSE[1] *= 1.1f;
                    COL_DIFFUSE[2] *= 1.1f;
                    conf.setParametre(CAT, "Material.Diffuse", COL_DIFFUSE[0]);
                    return true;
                }
            case Keys.End:
                {
                    COL_DIFFUSE[0] *= 0.9f;
                    COL_DIFFUSE[1] *= 0.9f;
                    COL_DIFFUSE[2] *= 0.9f;
                    conf.setParametre(CAT, "Material.Diffuse", COL_DIFFUSE[0]);
                    return true;
                }
            case Keys.PageUp:
                {
                    COL_SPECULAR[0] *= 1.1f;
                    COL_SPECULAR[1] *= 1.1f;
                    COL_SPECULAR[2] *= 1.1f;
                    conf.setParametre(CAT, "Material.Specular", COL_SPECULAR[0]);
                    return true;
                }
            case Keys.PageDown:
                {
                    COL_SPECULAR[0] *= 0.9f;
                    COL_SPECULAR[1] *= 0.9f;
                    COL_SPECULAR[2] *= 0.9f;
                    conf.setParametre(CAT, "Material.Specular", COL_SPECULAR[0]);
                    return true;
                }

            case Keys.Back:
                {
                    COL_COLOR[0] *= 1.1f;
                    COL_COLOR[1] *= 1.1f;
                    COL_COLOR[2] *= 1.1f;
                    conf.setParametre(CAT, "Material.Color", COL_COLOR[0]);
                    return true;
                }
            case Keys.Enter:
                {
                    COL_COLOR[0] *= 0.9f;
                    COL_COLOR[1] *= 0.9f;
                    COL_COLOR[2] *= 0.9f;
                    conf.setParametre(CAT, "Material.Color", COL_COLOR[0]);
                    return true;
                }



            case Keys.Subtract:
                SHININESS--;
                conf.setParametre(CAT, "Material.Shininess", SHININESS);
                return true;

            case Keys.Add:
                SHININESS++;
                conf.setParametre(CAT, "Material.Shininess", SHININESS);
                return true;*/
                default:
                    return false;
            }
        }
    }

}
