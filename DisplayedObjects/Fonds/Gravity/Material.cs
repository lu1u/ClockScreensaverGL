using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity
{
    public class Material
    {
        public int shininess = 5 ;
        public float [] color = {1.0f, 1.0f, 1.0f, 1.0f };
        public float[] ambient = { 0.1f, 0.1f, 0.1f, 1.0f };
        public float[] diffuse = { 0.1f, 0.1f, 0.1f, 1.0f };
        public float[] specular = { 0.8f, 0.8f, 0.8f, 1.0f };

        public Material(float amb, float diff, float spec, float color, int sh)
        {
            setColor(color);
            setAmbient(amb);
            setDiffuse(diff);
            setSpecular(spec);
            shininess = sh;
        }
        public void setAmbient( float r, float g, float b )
        {
            ambient[0] = r;
            ambient[1] = g;
            ambient[2] = b;
        }
        public void setAmbient(float v)
        {
            ambient[0] = v;
            ambient[1] = v;
            ambient[2] = v;
        }
        public void setDiffuse(float r, float g, float b)
        {
            diffuse[0] = r;
            diffuse[1] = g;
            diffuse[2] = b;
        }
        public void setDiffuse(float v)
        {
            diffuse[0] = v;
            diffuse[1] = v;
            diffuse[2] = v;
        }
        public void setSpecular(float r, float g, float b)
        {
            specular[0] = r;
            specular[1] = g;
            specular[2] = b;
        }

        public void setSpecular(float v)
        {
            specular[0] = v;
            specular[1] = v;
            specular[2] = v;
        }
        public void setColor(float r, float g, float b)
        {
            color[0] = r;
            color[1] = g;
            color[2] = b;
        }

        public void setColor(float v)
        {
            color[0] = v;
            color[1] = v;
            color[2] = v;
        }

        public void set(float amb, float diff, float spec, float couleur, int sh)
        {
            setColor(couleur);
            setAmbient(amb);
            setDiffuse(diff);
            setSpecular(spec);
            shininess = sh;
        }
        public void apply(OpenGL gl)
        {
            gl.Color(color);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SPECULAR, specular);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, ambient);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, diffuse);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SHININESS, shininess);
        }
    }
}
