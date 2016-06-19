using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SharpGL;
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D;
using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs;
using SharpGL.SceneGraph.Assets;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Particules
{
    class ParticulesGalaxie : SystemeParticules2D.SystemeParticules2D, IDisposable
    {
        const String CAT = "FontaineParticules";
        readonly int NB_EMETTEURS = conf.getParametre(CAT, "Nb Emetteurs", 2);
        static readonly int NB_PARTICULES = conf.getParametre(CAT, "Nb Particules", 10000);
        readonly float GRAVITE_X = conf.getParametre(CAT, "Gravite X", 0.05f);
        readonly float GRAVITE_Y = conf.getParametre(CAT, "Gravite Y", 0.2f);
        readonly float ALPHA_MODIFIEUR = conf.getParametre(CAT, "Modifieur Alpha", 0.1f);
        readonly float TAILLE_MODIFIEUR = conf.getParametre(CAT, "Modifieur Taille", 0.02f);
        readonly float TAILLE_PARTICULE = conf.getParametre(CAT, "TailleParticule", 0.01f);
        readonly float VITESSE_ANGLE = conf.getParametre(CAT, "VitesseAngle", 2.0f);
        readonly float VITESSE_PARTICULE = conf.getParametre(CAT, "VitesseParticule", 0.1f);
        Texture[] _texture = new Texture[3];
        
        public ParticulesGalaxie(OpenGL gl) : base(gl, NB_PARTICULES)
        {

            AjouteTexture(Config.getImagePath("nuages_petits.png"), 3);


            for (int i = 0; i < NB_EMETTEURS; i++)
                AjouteEmetteur(new EmetteurGalaxie(TAILLE_PARTICULE, VITESSE_ANGLE *FloatRandom(0.9f, 1.2f), VITESSE_PARTICULE * FloatRandom(0.9f, 1.2f), r.Next(2,8)));

            AttributBlend = SystemeParticules2D.SystemeParticules2D.PARTICULES_BLEND_ADDITIVE;

            AjouteModificateur(new ModificateurExclusion(MIN_X*1.1f, MIN_Y * 1.1f, MAX_X * 1.1f, MAX_Y * 1.1f, ModificateurExclusion.Exclusions.EXCLURE_TOUT));

            AjouteModificateur(new ModificateurLife());
            AjouteModificateur(new ModificateurVitesseLineaire());
            AjouteModificateur(new ModificateurAlpha(ALPHA_MODIFIEUR));
            AjouteModificateur(new ModificateurTaille(TAILLE_MODIFIEUR));
        }
    }
}
