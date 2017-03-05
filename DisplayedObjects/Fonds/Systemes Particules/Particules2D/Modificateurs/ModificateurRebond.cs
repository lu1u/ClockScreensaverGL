using System;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs
{
    class ModificateurRebond : Modificateur
    {
        private DateTime _derniere = DateTime.Now;
        float _minX, _maxX, _minY, _maxY;
        public ModificateurRebond( float minX, float maxX, float minY, float maxY)
        {
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
        }

        public override void Applique(SystemeParticules2D s, Temps maintenant)
        {
            for (int i = 0; i < s._nbParticules; i++)
                if (s._particules[i].active)
                {
                    s._particules[i].x += (s._particules[i].vx * maintenant.intervalleDepuisDerniereFrame);

                    if ((s._particules[i].x  - s._particules[i].taille < _minX) && (s._particules[i].vx < 0))
                        s._particules[i].vx = Math.Abs(s._particules[i].vx);
                    else
                        if (((s._particules[i].x + s._particules[i].taille) > _maxX) && (s._particules[i].vx > 0))
                        s._particules[i].vx = -Math.Abs(s._particules[i].vx);

                    s._particules[i].y += (s._particules[i].vy * maintenant.intervalleDepuisDerniereFrame);
                    if ((s._particules[i].y - s._particules[i].taille < _minY) && (s._particules[i].vy < 0))
                        s._particules[i].vy = Math.Abs(s._particules[i].vy);
                    else
                        if (((s._particules[i].y + s._particules[i].taille) > _maxY) && (s._particules[i].vy > 0))
                        s._particules[i].vy = -Math.Abs(s._particules[i].vy);
                }
        }
    }
}
