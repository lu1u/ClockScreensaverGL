/***
 * Un 'timer' indépendant de toute autres ressources
 */

using System;

namespace ClockScreenSaverGL
{
    public class TimerIsole
    {
        private double _intervalle;
        private DateTime _prochainTick;
        private bool _initial;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="intervalle">Intervalle en millisecondes</param>
        public TimerIsole( double intervalle, bool initial = false )
        {
            _intervalle = intervalle;
            _prochainTick = DateTime.Now.AddMilliseconds(intervalle);
            _initial = initial;
        }

        public double Intervalle
        {
            get
            {
                return _intervalle;
            }

            set
            {
                _intervalle = value;
            }
        }

        public bool Ecoule()
        {
            DateTime m = DateTime.Now;
            if (_initial || (m >= _prochainTick))
            {
                _prochainTick = m.AddMilliseconds(_intervalle);
                _initial = false;
                return true;
            }
            else
                return false;
        }
    }
}
