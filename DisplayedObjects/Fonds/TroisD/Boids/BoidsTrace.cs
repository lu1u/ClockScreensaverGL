using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Boids
{
    class BoidsTrace : Boids
    {
        const int NB = 100;
        const float MAX_SPEED = 40f;    // Maximum speed 2.0
        const float MAX_FORCE = MAX_SPEED * 0.005f;    // Maximum steering force 0.04
        const float TAILLE = 2.0f;
        const float DISTANCE_VOISINS = TAILLE * 40.0f;
        const float SEPARATION = TAILLE * 6.0f; // 25
        const float VITESSE_ANIMATION = 2.0f; // 25

        protected static double derniereTrace;
        protected const int MAX_TRACE = 100;
        public BoidsTrace(OpenGL gl) : base(gl, NB, TAILLE, MAX_SPEED, MAX_FORCE, DISTANCE_VOISINS, SEPARATION, VITESSE_ANIMATION)
        {
            derniereTrace = Temps.Maintenant();
        }

        protected override void InitRender(OpenGL gl)
        {

        }

        protected override void InitBoids(Boid[] _boids)
        {
            for (int i = 0; i < NB_BOIDS; i++)
                _boids[i] = new BoidTrace(r.Next(-MAX_X, MAX_X), r.Next(-MAX_Y, MAX_Y), r.Next(-MAX_Z, MAX_Z));
        }

        protected override void DessineBoid(OpenGL gl, float noImage)
        {
            throw new NotImplementedException();
        }


        public override void ClearBackGround(OpenGL gl, Color couleur)
        {
            couleur = getCouleurOpaqueAvecAlpha(couleur, 16);

            gl.ClearColor(couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1);

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        }

        protected override void InitOpenGL(OpenGL gl, Temps maintenant, Color couleur)
        {
            couleur = getCouleurOpaqueAvecAlpha(couleur, 64);
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1 };

            gl.Disable(OpenGL.GL_COLOR_MATERIAL);

            gl.Translate(0, 0, -MAX_Z * 2);
            gl.Rotate(_angleCamera*10.0f, _angleCamera * 10.0f, _angleCamera * 10.0f);


            gl.Disable(OpenGL.GL_ALPHA_TEST);
            gl.Disable(OpenGL.GL_CULL_FACE);
            gl.Enable(OpenGL.GL_BLEND);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            gl.Enable(OpenGL.GL_DEPTH);
            gl.Disable(OpenGL.GL_FOG);
            gl.Disable(OpenGL.GL_LIGHTING);
        }

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif           
            InitOpenGL(gl, maintenant, couleur);


            FrustumCulling frustum = new FrustumCulling(gl);

            gl.LineWidth(8);
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1 };
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE);

            foreach (BoidTrace b in _boids)
                if (frustum.isVisible(b._Position, TAILLE))
                    if (b._trace.Count > 0)
                    {
                        b._color[0] = col[0];
                        b._color[1] = col[1];
                        b._color[2] = col[2];
                        b._color[3] = col[3];

                        gl.Begin(OpenGL.GL_LINE_STRIP);
                        for (int i = 0; i < b._trace.Count; i++)
                        {
                            b._color[3] = 1.0f - (float)(b._trace.Count - i) / (float)b._trace.Count;
                            gl.Color(b._color);
                            b._trace[i].Vertex(gl);
                        }
                        gl.Color(b._color);
                        b._Position.Vertex(gl);

                        gl.End();
                    }

            fillConsole(gl);
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

        protected class BoidTrace : Boid
        {

            public List<Vecteur3D> _trace;
            public float[] _color;
            public BoidTrace(float x, float y, float z) : base(x, y, z)
            {
                _trace = new List<Vecteur3D>();
                _color = new float[4];
            }

            public override void update(Temps maintenant)
            {
                if ((maintenant._totalMillisecondes - derniereTrace) > 1500)
                {
                    // Ajouter une trace
                    if (_trace.Count > MAX_TRACE)
                        _trace.RemoveAt(0);

                    _trace.Add(new Vecteur3D(_Position));
                }

                base.update(maintenant);
            }

            protected override void Restreint()
            {
                if (_Position.x < -MAX_X)
                    if (_Vitesse.x < 0)
                        _Vitesse.x = -_Vitesse.x;

                if (_Position.x > MAX_X)
                    if (_Vitesse.x > 0)
                        _Vitesse.x = -_Vitesse.x;


                if (_Position.y < -MAX_Y)
                    if (_Vitesse.y < 0)
                        _Vitesse.y = -_Vitesse.y;

                if (_Position.y > MAX_Y)
                    if (_Vitesse.y > 0)
                        _Vitesse.y = -_Vitesse.y;
                if (_Position.z < -MAX_Z)
                    if (_Vitesse.z < 0)
                        _Vitesse.z = -_Vitesse.z;

                if (_Position.z > MAX_Z)
                    if (_Vitesse.z > 0)
                        _Vitesse.z = -_Vitesse.z;
            }
        }
    }
}
