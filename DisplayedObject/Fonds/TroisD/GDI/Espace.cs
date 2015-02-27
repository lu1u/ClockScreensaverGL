/*
 * Crée par SharpDevelop.
 * Utilisateur: lucien
 * Date: 30/12/2014
 * Heure: 23:08
 * 
 * Pour changer ce modèle utiliser Outils | Options | Codage | Editer les en-têtes standards.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
namespace ClockScreenSaverGL.Fonds.TroisD.GDI
{
    /// <summary>
    /// Description of Neige.
    /// </summary>
    public sealed class EspaceGDI : TroisDGDI
    {
        #region Parametres
        public const string CAT = "Espace.GDI";

        private readonly byte ALPHA = conf.getParametre(CAT, "Alpha", (byte)30);
        private readonly float TAILLE_ETOILE_MIN = conf.getParametre(CAT, "TailleMin", 256f);
        private readonly float TAILLE_ETOILE_MAX = conf.getParametre(CAT, "TailleMax", 512f);
        private readonly int NB_ETOILES = 2000; //conf.getParametre(CAT, "NbEtoiles", 2000 ) ;
        private readonly float PERIODE_TRANSLATION = conf.getParametre(CAT, "PeriodeTranslation", 13.0f);
        private readonly float PERIODE_ROTATION = conf.getParametre(CAT, "PeriodeRotation", 10.0f);
        private readonly float VITESSE_ROTATION = conf.getParametre(CAT, "VitesseRotation", 0.2f);
        private readonly float VITESSE_TRANSLATION = conf.getParametre(CAT, "VitesseTranslation", 1000f);
        private static readonly float _vitesse = -conf.getParametre(CAT, "Vitesse", 20000f);
        #endregion

        private readonly Objet3D[] _etoiles;
        static private DateTime _DernierDeplacement = DateTime.Now;
        private readonly Bitmap _bmp = Resources.particleTexture;

        static DateTime debut = DateTime.Now;
        public EspaceGDI(int Cx, int Cy)
        {
            _largeur = Cx;
            _hauteur = Cy;

            _centreX = _largeur / 2;
            _centreY = _hauteur / 2;

            _tailleCubeX = _largeur / 2;
            _tailleCubeY = _hauteur / 2;
            _tailleCubeZ = _largeur;

            _zEcran = -_tailleCubeZ;
            _zCamera = _zEcran * 1.75f;

            _etoiles = new Objet3D[NB_ETOILES];

            for (int i = 0; i < NB_ETOILES; i++)
            {
                NouvelleEtoile(ref _etoiles[i]);
                _etoiles[i].z = FloatRandom(_zCamera / 2, _tailleCubeZ * 100);
            }
        }

        private void NouvelleEtoile(ref Objet3D f)
        {
            if (f == null)
                f = new Objet3D();

            f.aSupprimer = false;
            f.x = FloatRandom(-_tailleCubeX * 100, _tailleCubeX * 100);
            f.z = _tailleCubeZ * 100;
            f.y = FloatRandom(-_tailleCubeY * 100, _tailleCubeY * 100);

            f.type = r.Next(0, 2);
            f.rayon = FloatRandom(256, 512);
            f.vx = 0;
            f.vy = 0;
            f.vz = _vitesse;
        }

        /// <summary>
        /// Affichage des flocons
        /// </summary>
        /// <param name="g"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleur"></param>
        public override void AfficheGDI(Graphics g, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if DEBUG
			RenderStart(CHRONO_TYPE.RENDER) ;
#endif
            g.Clear(Color.Black);
            TimeSpan diff = maintenant._temps.Subtract(_DernierDeplacement);
            float X, Y, X2, Y2;

            using (Bitmap bmp = BitmapNuance(g, _bmp, getCouleurAvecAlpha(couleur, ALPHA)))
                for (int i = 0; i < NB_ETOILES; i++)
                {
                    if (!_etoiles[i].aSupprimer)
                        if (_etoiles[i].z > _zCamera)
                        {
                            Coord2DFrom3D(_etoiles[i].x, _etoiles[i].y, _etoiles[i].z, out X, out Y);
                            Coord2DFrom3D(_etoiles[i].x + _etoiles[i].rayon, _etoiles[i].y + _etoiles[i].rayon, _etoiles[i].z, out X2, out Y2);
                            NormalizeCoord(ref X, ref X2, ref Y, ref Y2);

                            if ((X > _largeur) || (X2 < 0) || (Y > _hauteur) || (Y2 < 0))
                                NouvelleEtoile(ref _etoiles[i]);
                            else
                                try
                                {
                                    g.DrawImage(bmp, X, Y, X2 - X, Y2 - Y);
                                }
                                catch (Exception)
                                {
                                }
                        }
                }

#if DEBUG
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }



        public void NormalizeCoord(ref float X, ref float X2, ref float Y, ref float Y2)
        {
            if (Y2 < Y)
            {
                float t = Y2;
                Y2 = Y;
                Y = t;
            }

            if (X2 < X)
            {
                float t = X2;
                X2 = X;
                X = t;
            }
        }

        /// <summary>
        /// Deplacement de tous les objets: flocons, camera...
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
            float depuisdebut = (float)(debut.Subtract(_DernierDeplacement).TotalMilliseconds / 1000.0);
            float vitesseCamera = (float)Math.Sin(depuisdebut / PERIODE_ROTATION) * VITESSE_ROTATION;
            float xWind = (float)Math.Cos(depuisdebut / PERIODE_TRANSLATION) * VITESSE_TRANSLATION;
            float CosTheta = (float)Math.Cos(vitesseCamera * maintenant._intervalle);
            float SinTheta = (float)Math.Sin(vitesseCamera * maintenant._intervalle);
            float px, py;

            // Deplace les flocons
            for (int i = 0; i < NB_ETOILES; i++)
            {
                if (_etoiles[i].aSupprimer)
                {
                    NouvelleEtoile(ref _etoiles[i]);
                }
                else
                {
                    // Deplacement
                    _etoiles[i].x += ((_etoiles[i].vx + xWind) * maintenant._intervalle);
                    _etoiles[i].y += (_etoiles[i].vy * maintenant._intervalle);
                    _etoiles[i].z += (_etoiles[i].vz * maintenant._intervalle);

                    if (_etoiles[i].z < _zEcran)
                        _etoiles[i].aSupprimer = true;

                    // Rotation due a la position de la camera
                    px = (CosTheta * (_etoiles[i].x)) - (SinTheta * _etoiles[i].y);
                    py = (SinTheta * (_etoiles[i].x)) + (CosTheta * _etoiles[i].y);

                    _etoiles[i].x = px;
                    _etoiles[i].y = py;
                }
            }

            _DernierDeplacement = maintenant._temps;
        }

#if TRACER
        public override String DumpRender()
        {
            return base.DumpRender() + " Nb Etoiles:" + NB_ETOILES;
        }

#endif
    }
}
