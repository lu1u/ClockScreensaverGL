using System;

using System.Text;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Effects;

using System.Drawing;
namespace ClockScreenSaverGL.Fonds.TroisD.Opengl
{


    class TerreOpenGL : TroisD
    {
        #region Parametres
        const String CAT = "TerreOpenGl";
        static readonly float SPEC = conf.getParametre(CAT, "Specular", 0.6f);
        static readonly float AMB = conf.getParametre(CAT, "Ambient", 0.001f);
        static readonly float DIF = conf.getParametre(CAT, "Diffuse", 0.1f);
        static readonly float SHININESS = conf.getParametre(CAT, "Shininess", 100);
        static readonly int NB_TRANCHES = 64;//conf.getParametre(CAT, "NbTranches", 32);
        static readonly int NB_MERIDIENS = 64;//conf.getParametre(CAT, "NbMeridiens", 64);
        static readonly float LongitudeMarqueur = 270 + conf.getParametre(CAT, "Longitude", 5.97f);
        static readonly float LatitudeMarqueur = 0 +  conf.getParametre(CAT, "Latitude", 45.28f);
        #endregion
        float[] colSpec = { SPEC, SPEC, SPEC };
        float[] colAmbient = { AMB, AMB, AMB };
        float[] colDiffuse = { DIF, DIF, DIF };

        Texture _textureTerre = new Texture();
        Sphere sphere = new Sphere();
        float[] LightPos = { -9f, 3f, -8f, 1 };
        float _rotation = 270;
        public TerreOpenGL(OpenGL gl)
            : base(1.0f, 1.0f, 1.0f, 0, 0, 1.0f)
        {
            _textureTerre.Create(gl, Resources.terre);

            sphere.CreateInContext(gl);
            sphere.NormalGeneration = SharpGL.SceneGraph.Quadrics.Normals.Smooth;
            sphere.NormalOrientation = SharpGL.SceneGraph.Quadrics.Orientation.Outside;
            sphere.QuadricDrawStyle = SharpGL.SceneGraph.Quadrics.DrawStyle.Fill;

            sphere.Slices = NB_MERIDIENS;
            sphere.Stacks = NB_TRANCHES ;
            sphere.TextureCoords = true;
            sphere.Transformation.RotateX = -90;
        }

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            float[] col = { couleur.R / 512.0f, couleur.G / 512.0f, couleur.B / 512.0f, 1 };
            //  Load the identity matrix.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_ALPHA_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Disable(OpenGL.GL_BLEND);
            gl.Disable(OpenGL.GL_FOG);
            gl.Disable(OpenGL.GL_BLEND);
            gl.DepthMask((byte)OpenGL.GL_TRUE);

            gl.Enable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_LIGHTING); 	// Active l'éclairage
            gl.Enable(OpenGL.GL_LIGHT0); 	// Allume la lumière n°1

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, LightPos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_COLOR, col);
            gl.CullFace(OpenGL.GL_BACK);
            gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SPECULAR, colSpec);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, colAmbient);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, colDiffuse);

            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SHININESS, SHININESS);
            gl.Color(col);

            gl.Translate(1, -0.5, -2f);
            gl.Rotate(0, 0, 23);
            gl.Rotate(0, _rotation, 0);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            _textureTerre.Bind(gl);
            
            _textureTerre.Bind(gl);
            sphere.PushObjectSpace(gl);
            sphere.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            sphere.PopObjectSpace(gl);
            
            // Le petit drapeau
            {
                float[] colDrapeau = { couleur.R / 256, couleur.G / 256, couleur.B / 256, 1 };
                gl.Disable(OpenGL.GL_CULL_FACE);
                gl.Disable(OpenGL.GL_TEXTURE_2D);
                gl.Disable(OpenGL.GL_COLOR_MATERIAL);
                gl.Rotate(0, LongitudeMarqueur, LatitudeMarqueur);
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

        void drawSphere(OpenGL gl, double r, int lats, int longs)
        {
            double M_PI = Math.PI;
            int i, j;
            for (i = 0; i <= lats; i++)
            {
                double lat0 = M_PI * (-0.5 + (double)(i - 1) / lats);
                double z0 = Math.Sin(lat0);
                double zr0 = Math.Cos(lat0);

                double lat1 = M_PI * (-0.5 + (double)i / lats);
                double z1 = Math.Sin(lat1);
                double zr1 = Math.Cos(lat1);

                gl.Begin(OpenGL.GL_QUAD_STRIP);
                for (j = 0; j <= longs; j++)
                {
                    double lng = 2 * M_PI * (double)(j - 1) / longs;
                    double x = Math.Cos(lng);
                    double y = Math.Sin(lng);

                    gl.TexCoord(j / longs, i / lats);
                    gl.Normal(x * zr0, y * zr0, z0);
                    gl.Vertex(x * zr0, y * zr0, z0);
                    gl.TexCoord((j + 1) / longs, i / lats);
                    gl.Normal(x * zr1, y * zr1, z1);
                    gl.Vertex(x * zr1, y * zr1, z1);
                }
                gl.End();
            }
        }
        /// <summary>
        /// Deplacer les nuages
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            _rotation += maintenant._intervalle * 10f;

#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }


    }
}
