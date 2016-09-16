using ClockScreenSaverGL.Config;
using SharpGL;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Grilles
{
    class Cubes : GrilleBase
    {
        #region Parametres
        public const string CAT = "Cubes";
        static CategorieConfiguration c = Config.Configuration.getCategorie(CAT);
        private readonly int NB_CUBES_X = c.getParametre("Nb Cubes X", 30);
        private readonly int NB_CUBES_Y = c.getParametre("Nb Cubes Y", 30);
        private readonly int NB_CUBES_Z = c.getParametre("Nb Cubes Z", 30);
        private float TAILLE_CUBE = c.getParametre("Taille cubes", 0.06f);
        private float ECART_CUBE = c.getParametre("Ecart cubes", 0.4f);
        #endregion



        public Cubes(OpenGL gl) : base(gl, c)
        {
            VITESSE_ROTATION = c.getParametre("Vitesse Rotation", 0.5f);
            TRANSLATE_Z = ECART_CUBE * NB_CUBES_Z * -0.55f;
            LIGHTPOS[0] = ECART_CUBE * NB_CUBES_X;
            LIGHTPOS[1] = ECART_CUBE * NB_CUBES_Y;
            LIGHTPOS[2] = ECART_CUBE * NB_CUBES_Z;
            fogEnd = ECART_CUBE * NB_CUBES_X * 0.5f;
        }


        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
        protected override void GenererListe(OpenGL gl)
        {
            float ORIGINE_X = -(NB_CUBES_X * 0.5f) * ECART_CUBE;
            float ORIGINE_Y = -(NB_CUBES_Y * 0.5f) * ECART_CUBE;
            float ORIGINE_Z = -(NB_CUBES_Z * 0.5f) * ECART_CUBE;
            gl.Begin(OpenGL.GL_QUADS);

            for (int x = 0; x < NB_CUBES_X; x++)
                for (int y = 0; y < NB_CUBES_Y; y++)
                    for (int z = 0; z < NB_CUBES_Z; z++)
                        Brique(gl, ORIGINE_X + (float)x * ECART_CUBE, ORIGINE_Y + (float)y * ECART_CUBE, ORIGINE_Z + (float)z * ECART_CUBE,
                            TAILLE_CUBE, TAILLE_CUBE, TAILLE_CUBE);

            gl.End();
        }


    }
}
