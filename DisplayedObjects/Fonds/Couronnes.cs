using System;
using System.Drawing;
using System.Windows.Forms;
using SharpGL;
using ClockScreenSaverGL.Config;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds
{
    /// <summary>
    /// Description of Couronnes.
    /// </summary>
    public class Couronnes : Fond
    {
        public const String CAT = "Couronnes";
        private const string PARAM_WIREFRAME = "WireFrame";
        private const string PARAM_ADDITIVE = "Additive";
        static protected CategorieConfiguration c = Configuration.getCategorie(CAT);
        private readonly int NB_COURONNES = c.getParametre("NbCouronnes", 5);
        private readonly int NB_SECTEURS = c.getParametre("Details Secteurs", 120);
        private readonly float RAYON_MAX = c.getParametre("Rayon Max", 1.5f);
        static private bool ADDITIVE = c.getParametre(PARAM_ADDITIVE, false, (a) => { ADDITIVE = Convert.ToBoolean(a); });
        static private bool WIRE_FRAME = c.getParametre(PARAM_WIREFRAME, false, (a) => { WIRE_FRAME = Convert.ToBoolean(a); });
        private readonly int NB_SEGMENTS_MAX = c.getParametre("Nb Segments Max", 40);
        private readonly int NB_SEGMENTS_MIN = c.getParametre("Nb Segments Min", 2);

        private DateTime _dernierDeplacement = DateTime.Now;
        private DateTime _debutAnimation = DateTime.Now;


        private class Couronne
        {
            public float rMin, rMax;
            public int nbSegments;
            public float ecartSegments;
            public float vitesseRotation;
            public float deltaRayon;
            public float angleRotationActuel;
            public float periodeRotation;
            public float periodeRayon;
            public float vitesseRayon;
        }

        Couronne[] _couronnes;
        private float _zCamera = 1.5f;

        public Couronnes(OpenGL gl) : base(gl)
        {
            _couronnes = new Couronne[NB_COURONNES];

            for (int i = 0; i < NB_COURONNES; i++)
            {
                _couronnes[i] = new Couronne();
                _couronnes[i].rMax = FloatRandom(RAYON_MAX / 10.0f, RAYON_MAX);
                _couronnes[i].rMin = _couronnes[i].rMax * FloatRandom(0.5f, 0.95f);
                _couronnes[i].vitesseRotation = FloatRandom(0.001f, 2) * SigneRandom();
                _couronnes[i].nbSegments = r.Next(NB_SEGMENTS_MIN, NB_SEGMENTS_MAX);
                _couronnes[i].ecartSegments = FloatRandom(0.5f, 0.95f);
                _couronnes[i].angleRotationActuel = r.Next(0, 360);
                _couronnes[i].periodeRotation = FloatRandom(0.001f, 10);
                _couronnes[i].deltaRayon = 1.0f;
                _couronnes[i].periodeRayon = FloatRandom(0.001f, 20);
                _couronnes[i].vitesseRayon = FloatRandom(0.010f, 0.1f) * SigneRandom();
            }
            
        }


        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }
        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 0.4f };

            gl.ClearColor(0, 0, 0, 1);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.Translate(0, 0, -_zCamera);

            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Disable(OpenGL.GL_FOG);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, ADDITIVE ? OpenGL.GL_ONE : OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            // Lumiere
            gl.Disable(OpenGL.GL_LIGHTING);

            if (WIRE_FRAME)
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
            gl.Color(col);

            // Tracer les anneaux
            foreach (Couronne c in _couronnes)
            {
                float tailleSegment = (((float)(Math.PI * 2) / c.nbSegments)) * c.ecartSegments;
                float rMin = c.rMin * c.deltaRayon;
                float rMax = c.rMax * c.deltaRayon;
                int nbSecteurs = (int)Math.Round((float)NB_SECTEURS / c.nbSegments * rMax);

                gl.PushMatrix();
                gl.Rotate(0, 0, c.angleRotationActuel);


                // Tracer chaque partie de la couronne
                for (int s = 0; s < c.nbSegments; s++)
                {
                    gl.Begin(OpenGL.GL_QUAD_STRIP);
                    for (int i = 0; i <= nbSecteurs; i++)
                    {
                        float angle = ((float)i / (float)nbSecteurs) * tailleSegment;

                        float sin = (float)Math.Sin(angle);
                        float cos = (float)Math.Cos(angle);
                        gl.Vertex(rMin * cos, rMin * sin, 0);
                        gl.Vertex(rMax * cos, rMax * sin, 0);
                    }
                    gl.End();
                    gl.Rotate(0, 0, 360.0f / c.nbSegments);
                }

                gl.PopMatrix();
            }


            if (WIRE_FRAME)
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif

        }
        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
            float depuisdebut = (float)(_debutAnimation.Subtract(_dernierDeplacement).TotalMilliseconds / 1000.0);

            foreach (Couronne c in _couronnes)
            {
                c.angleRotationActuel += (float)Math.Sin(depuisdebut / c.periodeRotation) * c.vitesseRotation;
                c.deltaRayon = 1.0f + (float)Math.Sin(depuisdebut / c.periodeRayon) * c.vitesseRayon;
            }

            _dernierDeplacement = maintenant.temps;
        }


        /// <summary>
        /// Pression sur une touche, retourner true si l'objet a traite, false = fin de l'economiseur
        /// </summary>
        /// <param name="f"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        /*
        public override bool KeyDown(Form f, Keys k)
        {
            switch (k)
            {
                case TOUCHE_INVERSER:
                    {
                        WIRE_FRAME = !WIRE_FRAME;
                        c.setParametre(PARAM_WIREFRAME, WIRE_FRAME);
                        return true;
                    }


                case TOUCHE_ADDITIVE:
                    {
                        ADDITIVE = !ADDITIVE;
                        c.setParametre(PARAM_ADDITIVE, ADDITIVE);
                        return true;
                    }

            }
            return base.KeyDown(f, k); ;
        }*/
    }
}
