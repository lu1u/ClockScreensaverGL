using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Systeme_Particules.Modificateurs
{
    abstract class Modificateur
    {
        abstract public void Applique(SystemeParticules s, Temps maintenant);
    }
}
