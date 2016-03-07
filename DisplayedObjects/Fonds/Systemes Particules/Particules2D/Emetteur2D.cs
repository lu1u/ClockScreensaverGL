using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D
{
    public abstract class Emetteur2D
    {
        static protected Random r = new Random();
        public abstract void Deplace(SystemeParticules2D s, Temps maintenant, Color couleur);

    }
}
