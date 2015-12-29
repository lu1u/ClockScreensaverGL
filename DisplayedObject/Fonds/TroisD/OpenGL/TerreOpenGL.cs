/***
 * Affiche une mappemonde
 */

using System;
using System.Drawing;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Evaluators;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Opengl
{
    sealed class TerreOpenGL : Fond, IDisposable
    {
        #region Parametres
        const String CAT = "TerreOpenGl";
        static readonly float SPEC = 128.0f;// conf.getParametre(CAT, "Specular", 1f);
        static readonly float AMB = conf.getParametre(CAT, "Ambient", 0.0f);
        static readonly float DIF = conf.getParametre(CAT, "Diffuse", 0.1f);
        static readonly float SHININESS = 128;// conf.getParametre(CAT, "Shininess", 50);
        static readonly int NB_TRANCHES = conf.getParametre(CAT, "NbTranches", 64);
        static readonly int NB_MERIDIENS = conf.getParametre(CAT, "NbMeridiens", 64);
        static readonly float VITESSE = conf.getParametre(CAT, "Vitesse", 5f);
        static readonly float INITIAL_ROTATION = conf.getParametre(CAT, "Rotation initiale", 270); // Ajuster pour montrer le pays qu'on veut au depart
        static readonly float LONGITUDE_DRAPEAU = 270 + conf.getParametre(CAT, "Longitude", 5.97f); // Longitude du drapeau + correction en fonction de la texture
        static readonly float LATITUDE_DRAPEAU = 0 + conf.getParametre(CAT, "Latitude", 45.28f); // Latitude du drapeau
        static readonly int DETAILS_DRAPEAU = conf.getParametre(CAT, "Details drapeau", 10);
        #endregion

        static readonly float[] COL_SPECULAR = { SPEC, SPEC, SPEC, 1.0f };
        static readonly float[] COL_AMBIENT = { AMB, AMB, AMB, 1.0f };
        static readonly float[] COL_DIFFUSE = { DIF, DIF, DIF, 1.0f };
        static readonly float[] COL_LIGHTPOS = { -9f, 5f, -5f, 1 };

        Texture _textureTerre = new Texture();
        Sphere _sphere = new Sphere();
        
        float _rotation = 270;
        float[] _zDrapeau = new float[DETAILS_DRAPEAU];
        int _frame = 0;
        /// <summary>
        /// Constructeur: preparer les objets OpenGL
        /// </summary>
        /// <param name="gl"></param>
        public TerreOpenGL(OpenGL gl): base(gl)
            //: base(1.0f, 1.0f, 1.0f, 0)
        {
            _textureTerre.Create(gl, Resources.terre);

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

        public override void Dispose()
        {
            base.Dispose();
            _textureTerre?.Destroy(_gl);
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
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1 };
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_ALPHA_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Disable(OpenGL.GL_BLEND);
            gl.Disable(OpenGL.GL_FOG);
            gl.DepthMask((byte)OpenGL.GL_TRUE);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.CullFace(OpenGL.GL_BACK);

            // Lumiere
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, COL_LIGHTPOS);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_COLOR, col);

            // Aspect de la surface
            gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SPECULAR, COL_SPECULAR);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, COL_AMBIENT);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, COL_DIFFUSE);

            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SHININESS, SHININESS);
            gl.Color(col);

            // Rotations, translation
            gl.Translate(1, -0.5f, -2f);
            gl.Rotate(0, 0, 23.43f);         // Inclinaison reelle de l'axe de la terre
            gl.Rotate(0, _rotation, 0);

            // Dessine le globe
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
        public override void Deplace(Temps maintenant,  Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            _rotation += maintenant._intervalle * VITESSE;

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
