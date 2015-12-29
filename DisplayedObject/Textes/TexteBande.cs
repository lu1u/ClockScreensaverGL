using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Textes
{
    public abstract class TexteBande : Texte
    {
        public TexteBande(OpenGL gl,  float Px, float Py, float Vx, float Vy, int tailleFonte, byte alpha)
            :base( gl, Px, Py, Vx, Vy, tailleFonte, alpha)
        {
         
        }
    }
}
