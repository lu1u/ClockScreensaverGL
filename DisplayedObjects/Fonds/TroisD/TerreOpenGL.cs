/***
 * Affiche une mappemonde
 */

using System;
using System.Drawing;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Evaluators;
using System.Windows.Forms;
using ClockScreenSaverGL.Config;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD
{
    class TerreOpenGL : MateriauGlobal, IDisposable
    {
        #region Parametres
        const String CAT = "TerreOpenGl";
        static CategorieConfiguration c = Config.Configuration.getCategorie(CAT);
        static readonly int NB_TRANCHES = c.getParametre("NbTranches", 64);
        static readonly int NB_MERIDIENS = c.getParametre("NbMeridiens", 64);
        static float VITESSE = c.getParametre("Vitesse", 5f);
        static float LONGITUDE_DRAPEAU = 270 + c.getParametre("Longitude", 5.97f, (a) => { LONGITUDE_DRAPEAU = (float)Convert.ToDouble(a); } ); // Longitude du drapeau + correction en fonction de la texture
        static float LATITUDE_DRAPEAU = 0 + c.getParametre("Latitude", 45.28f, (a) => { LATITUDE_DRAPEAU = (float)Convert.ToDouble(a); } ); // Latitude du drapeau
        static int DETAILS_DRAPEAU = c.getParametre("Details drapeau", 10);
        #endregion

        Texture _textureTerre = new Texture();
        Sphere _sphere = new Sphere();

        float _rotation = 270;
        float[] _zDrapeau = new float[DETAILS_DRAPEAU];
        int _frame = 0;
        /// <summary>
        /// Constructeur: preparer les objets OpenGL
        /// </summary>
        /// <param name="gl"></param>
        public TerreOpenGL(OpenGL gl) : base(gl)
        //: base(1.0f, 1.0f, 1.0f, 0)
        {
            _textureTerre.Create(gl, c.getParametre( "Terre", Configuration.getImagePath( "terre.png" ) ) );

            _sphere.CreateInContext(gl);
            _sphere.NormalGeneration = SharpGL.SceneGraph.Quadrics.Normals.Smooth;
            _sphere.NormalOrientation = SharpGL.SceneGraph.Quadrics.Orientation.Outside;
            _sphere.QuadricDrawStyle = SharpGL.SceneGraph.Quadrics.DrawStyle.Fill;
            _sphere.Slices = NB_MERIDIENS;
            _sphere.Stacks = NB_TRANCHES;
            _sphere.TextureCoords = true;
            _sphere.Transformation.RotateX = -90;

            for (int i = 0; i < DETAILS_DRAPEAU; i++)
                _zDrapeau[i] = 0.002f * (float)Math.Sin(i * 4.0 * Math.PI / DETAILS_DRAPEAU);
            
        }

        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
        public override void Dispose()
        {
            base.Dispose();
            _textureTerre?.Destroy(_gl);
            _sphere.DestroyInContext(_gl);
        }

        /// <summary>
        /// Affichage
        /// </summary>
        /// <param name="gl"></param>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        /// <param name="couleur"></param>
        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            float[] lcol = { 1, 1, 1, 1 };
            gl.ClearColor(0, 0, 0, 0);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_ALPHA_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.CullFace(OpenGL.GL_BACK);
            gl.Disable(OpenGL.GL_BLEND);
            gl.Disable(OpenGL.GL_FOG);
            //gl.Enable(OpenGL.GL_DEPTH);
            //gl.Enable(OpenGL.GL_DEPTH_TEST);
            
            setGlobalMaterial(gl, couleur);

            // Rotations, translation
            gl.Translate(1, -0.5f, -2f);
            gl.Rotate(0, 0, 23.43f);         // Inclinaison reelle de l'axe de la terre
            gl.Rotate(0, _rotation, 0);

            // Dessine le globe
            //float[] col = { couleur.R / 512.0f, couleur.G / 512.0f, couleur.B / 512.0f, 1 };
            //gl.Color(col);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            _textureTerre.Bind(gl);
            _sphere.PushObjectSpace(gl);
            _sphere.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            _sphere.PopObjectSpace(gl);

            // Le petit drapeau
            {
                gl.Disable(OpenGL.GL_LIGHTING);
                gl.Disable(OpenGL.GL_CULL_FACE);
                gl.Disable(OpenGL.GL_TEXTURE_2D);
                gl.Rotate(0, LONGITUDE_DRAPEAU, LATITUDE_DRAPEAU);
                gl.Begin(OpenGL.GL_QUADS);
                gl.Vertex(1, 0, 0.002f);
                gl.Vertex(1, 0, -0.002f);
                gl.Vertex(1.1f, 0, -0.002f);
                gl.Vertex(1.1f, 0, 0.002f);

                gl.Vertex(1, 0.002f, 0);
                gl.Vertex(1, -0.002f, 0);
                gl.Vertex(1.2f, -0.002f, 0);
                gl.Vertex(1.2f, 0.002f, 0);
                gl.End();

                gl.Begin(OpenGL.GL_QUAD_STRIP);
                for (int i = 0; i < DETAILS_DRAPEAU; i++)
                {
                    gl.Vertex(1.15f, i * 0.05f / DETAILS_DRAPEAU, _zDrapeau[i]);
                    gl.Vertex(1.19f, i * 0.05f / DETAILS_DRAPEAU, _zDrapeau[i]);
                }
                gl.End();
            }
            

#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

        
        /// <summary>
        /// Faire tourner le globe
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            _rotation += maintenant.intervalleDepuisDerniereFrame * VITESSE;

            if ((_frame++) % 2 == 0)
            {
                float z = _zDrapeau[DETAILS_DRAPEAU - 1];
                for (int i = DETAILS_DRAPEAU - 1; i > 0; i--)
                    _zDrapeau[i] = _zDrapeau[i - 1];

                _zDrapeau[0] = z;
            }
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }
    }
}
