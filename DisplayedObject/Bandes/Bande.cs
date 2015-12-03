/*
 * Bande:
 * classe de base pour les objets qui affichent heure/minutes/secondes, verticalement ou horizontalement
 */
using System;
using System.Drawing;
namespace ClockScreenSaverGL.DisplayedObject.Bandes
{
    /// <summary>
    /// Description of Bande.
    /// </summary>
    public abstract class Bande : DisplayedObject, IDisposable
    {
        protected int _intervalleTexte;
        protected float _largeurCase;
        protected int _hauteurFonte;
        protected int _valeurMax;
        protected Font _fonte;
        protected float _origine;
        protected Trajectoire _trajectoire;
        protected SizeF _taillebande;
        protected byte _alpha;

        /// <summary>
        /// Retourne la valeur a afficher, avec un decalage partiel (ex: decalage partiel par seconde pour afficher
        /// les minutes
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="value"></param>
        /// <param name="decalage"></param>
        protected abstract void getValue(Temps maintenant, out float value, out float decalage);

        public Bande(int valMax, int intervalle, float largeurcase, int hauteurfonte, float origineX, int largeur, byte alpha) :
            base()
        {
            _valeurMax = valMax;
            _largeurCase = largeurcase;
            _hauteurFonte = hauteurfonte;
            _origine = origineX;
            _intervalleTexte = intervalle;
            _alpha = alpha;

            _fonte = new Font(FontFamily.GenericMonospace, _hauteurFonte, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        /// <summary>
        /// Implementation de la fonction virtuelle Deplace: deplacement de l'objet
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, ref Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif

            _trajectoire.Avance(tailleEcran, _taillebande, maintenant);
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif

        }

        public override void Dispose()
        {
            _fonte?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
