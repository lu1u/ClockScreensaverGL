/***
 * Affiche une mappemonde
 */

using System;
using System.Drawing;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using SharpGL.SceneGraph.Quadrics;
namespace ClockScreenSaverGL.Fonds.TroisD.Opengl
{
    class TerreOpenGL : TroisD
    {
        #region Parametres
        const String CAT = "TerreOpenGl";
        static readonly float SPEC = conf.getParametre(CAT, "Specular", 1f);
        static readonly float AMB = conf.getParametre(CAT, "Ambient", 0.0f);
        static readonly float DIF = conf.getParametre(CAT, "Diffuse", 0.1f);
        static readonly float SHININESS = conf.getParametre(CAT, "Shininess", 25);
        static readonly int NB_TRANCHES = conf.getParametre(CAT, "NbTranches", 64);
        static readonly int NB_MERIDIENS = conf.getParametre(CAT, "NbMeridiens", 64);
        static readonly float VITESSE = conf.getParametre(CAT, "Vitesse", 5f);
        static readonly float INITIAL_ROTATION = conf.getParametre(CAT, "Rotation initiale", 270); // Ajuster pour montrer le pays qu'on veut au depart
        static readonly float LONGITURE_DRAPEAU = 270 + conf.getParametre(CAT, "Longitude", 5.97f); // Longitude du drapeau + correction en fonction de la texture
        static readonly float LATITUDE_DRAPEAU = 0 + conf.getParametre(CAT, "Latitude", 45.28f); // Latitude du drapeau
        #endregion

        static readonly float[] colSpec = { SPEC, SPEC, SPEC, 1.0f };
        static readonly float[] colAmbient = { AMB, AMB, AMB, 1.0f };
        static readonly float[] colDiffuse = { DIF, DIF, DIF, 1.0f };

        Texture _textureTerre = new Texture();
        Sphere sphere = new Sphere();
        float[] LightPos = { -9f, 5f, -5f, 1 };
        float _rotation = 270;

        /// <summary>
        /// Constructeur: preparer les objets OpenGL
        /// </summary>
        /// <param name="gl"></param>
        public TerreOpenGL(OpenGL gl)
            : base(1.0f, 1.0f, 1.0f, 0, 0, 1.0f)
        {
            _textureTerre.Create(gl, Resources.terre);

            sphere.CreateInContext(gl);
            sphere.NormalGeneration = SharpGL.SceneGraph.Quadrics.Normals.Smooth ;
            sphere.NormalOrientation = SharpGL.SceneGraph.Quadrics.Orientation.Outside;
            sphere.QuadricDrawStyle = SharpGL.SceneGraph.Quadrics.DrawStyle.Fill;
            
            sphere.Slices = NB_MERIDIENS;
            sphere.Stacks = NB_TRANCHES ;
            sphere.TextureCoords = true;
            sphere.Transformation.RotateX = -90;
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
            float[] col = { couleur.R / 512.0f, couleur.G / 512.0f, couleur.B / 512.0f, 1 };
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_ALPHA_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Disable(OpenGL.GL_BLEND);
            gl.Disable(OpenGL.GL_FOG);
            gl.Disable(OpenGL.GL_BLEND);
            gl.DepthMask((byte)OpenGL.GL_TRUE);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.CullFace(OpenGL.GL_BACK);
            
            // Lumiere
            gl.Enable(OpenGL.GL_LIGHTING); 	
            gl.Enable(OpenGL.GL_LIGHT0); 	
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, LightPos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_COLOR, col);
            
            // Aspect de la surface
            gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SPECULAR, colSpec);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, colAmbient);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, colDiffuse);

            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SHININESS, SHININESS);
            gl.Color(col);

            // Rotations, translation
            gl.Translate(1, -0.5f, -2f);
            gl.Rotate(0, 0, 23.43f);         // Inclinaison reeel de l'axe de la terre
            gl.Rotate(0, _rotation, 0);

            // Dessine le globe
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            _textureTerre.Bind(gl);
            sphere.PushObjectSpace(gl);
            sphere.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            sphere.PopObjectSpace(gl);
            
            // Le petit drapeau
            {
                float[] colDrapeau = { couleur.R / 256, couleur.G / 256, couleur.B / 256, 1 };
                gl.Disable(OpenGL.GL_LIGHTING); 
                gl.Disable(OpenGL.GL_CULL_FACE);
                gl.Disable(OpenGL.GL_TEXTURE_2D);
                gl.Disable(OpenGL.GL_COLOR_MATERIAL);
                gl.Rotate(0, LONGITURE_DRAPEAU, LATITUDE_DRAPEAU);
                gl.Begin(OpenGL.GL_QUADS);
                gl.Vertex(1, 0, 0.005f);
                gl.Vertex(1, 0, -0.005f);
                gl.Vertex(1.1f, 0, -0.005f);
                gl.Vertex(1.1f, 0, 0.005f);

                gl.Vertex(1, 0.005f, 0);
                gl.Vertex(1, -0.005f, 0);
                gl.Vertex(1.1f, -0.005f, 0);
                gl.Vertex(1.1f, 0.005f, 0);

                gl.Vertex(1.05f, 0.01f, 0);
                gl.Vertex(1.05f, -0.05f, 0);
                gl.Vertex(1.09f, -0.05f, 0);
                gl.Vertex(1.09f, 0.01f, 0);
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
            _rotation += maintenant._intervalle * VITESSE ;

#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }
    }
}
