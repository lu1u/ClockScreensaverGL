﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObject.Textes
{
    public abstract class TexteBande : Texte
    {
        public TexteBande(float Px, float Py, float Vx, float Vy, int tailleFonte, byte alpha)
            :base( Px, Py, Vx, Vy, tailleFonte, alpha)
        {
         
        }
    }
}
