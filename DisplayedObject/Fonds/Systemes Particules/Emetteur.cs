using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Systeme_Particules
{
    abstract class Emetteur
    {
        static protected Random r = new Random();
        public abstract void Deplace(SystemeParticules s, Temps maintenant, Color couleur);

    }
}
