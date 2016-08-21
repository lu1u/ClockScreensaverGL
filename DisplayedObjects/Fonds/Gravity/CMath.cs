using ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity
{
    class CMath
    {
        //////////////////////////////////////////////////////////////////////
        // Evolution d'une valeur proportionnellement au temps entre deux
        // frames
        //////////////////////////////////////////////////////////////////////
        public static Vecteur3D VitesseConstante(Vecteur3D Vit, float intervalle)
        {
            return Vit * intervalle;
        }

        //////////////////////////////////////////////////////////////////////
        // Evolution d'une valeur proportionnellement au temps entre deux
        // frames
        //////////////////////////////////////////////////////////////////////
        public static float VitesseConstante(float Vitesse, float intervalle)
        {
            return Vitesse * intervalle;
        }

    }
}
