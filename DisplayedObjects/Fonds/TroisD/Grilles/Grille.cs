using System;
using System.Drawing;
using System.Windows.Forms;
using SharpGL;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Grilles
{
    class Grille : GrilleBase
    {
        #region Parametres
        public const string CAT = "Grille";
        private readonly int NB_BARRES_X = conf.getParametre(CAT, "Nb Cubes X", 40);
        private readonly int NB_BARRES_Y = conf.getParametre(CAT, "Nb Cubes Y", 40);
        private readonly int NB_BARRES_Z = conf.getParametre(CAT, "Nb Cubes Z", 40);

        private readonly float EPAISSEUR_GRILLE = conf.getParametre(CAT, "Epaisseur grille", 0.025f);
        private float ECART_GRILLE = conf.getParametre(CAT, "Ecart grille", 0.5f);
        #endregion

        const int NB_FIGURES = 1;
        const uint FIGURE_X = 0;
        const uint FIGURE_Y = 1;
        const uint FIGURE_Z = 2;

       
        public Grille(OpenGL gl) : base(gl, CAT)
        {
            VITESSE_ROTATION = conf.getParametre(CAT, "Vitesse Rotation", 0.5f);
            TRANSLATE_Z = ECART_GRILLE* NB_BARRES_X *-0.25f;
            LIGHTPOS[0] = ECART_GRILLE * NB_BARRES_X;
            LIGHTPOS[1] = ECART_GRILLE * NB_BARRES_Y;
            LIGHTPOS[2] = ECART_GRILLE * NB_BARRES_Z;

            fogEnd = ECART_GRILLE * NB_BARRES_X * 0.75f;
        }



        protected override void GenererListe(OpenGL gl)
        {
            
            float ORIGINE_X = -(NB_BARRES_X * 0.5f) * ECART_GRILLE;
            float ORIGINE_Y = -(NB_BARRES_Y * 0.5f) * ECART_GRILLE;
            float ORIGINE_Z = -(NB_BARRES_Z * 0.5f) * ECART_GRILLE;

            // Barres de l'axe des X
            for (int y = 0; y < NB_BARRES_Y; y++)
                for (int z = 0; z < NB_BARRES_Z; z++)
                    Cube(gl, 0, ORIGINE_Y + (float)y * ECART_GRILLE, ORIGINE_Z + (float)z * ECART_GRILLE, NB_BARRES_X * ECART_GRILLE, EPAISSEUR_GRILLE, EPAISSEUR_GRILLE);

            // Barres de l'axe des Y
            for (int x = 0; x < NB_BARRES_X; x++)
                for (int z = 0; z < NB_BARRES_Z; z++)
                   Cube(gl, ORIGINE_X + (float)x * ECART_GRILLE, 0, ORIGINE_Z + (float)z * ECART_GRILLE, EPAISSEUR_GRILLE, NB_BARRES_Y * ECART_GRILLE, EPAISSEUR_GRILLE);
            
            for (int x = 0; x < NB_BARRES_X; x++)
                for (int y = 0; y < NB_BARRES_Y; y++)
                    Cube( gl, ORIGINE_X + (float)x * ECART_GRILLE, ORIGINE_Y + (float)y * ECART_GRILLE, 0, EPAISSEUR_GRILLE, EPAISSEUR_GRILLE, NB_BARRES_Z * ECART_GRILLE);
        }

    }
}
