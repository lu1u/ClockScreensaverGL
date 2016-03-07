using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs
{
    public abstract class Modificateur
    {
        abstract public void Applique(SystemeParticules2D s, Temps maintenant);
    }
}
