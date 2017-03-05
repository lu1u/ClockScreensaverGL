using System;

using GLfloat = System.Single;
using SharpGL;
using System.Drawing;
using SharpGL.SceneGraph.Assets;
using System.Collections.Generic;
using ClockScreenSaverGL.Config;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Boids
{
    abstract class Boids : MateriauGlobal, IDisposable
    {
        #region Parametres

        public static readonly float MAX_X = 10;
        public static readonly float MAX_Y = 10;
        public static readonly float MAX_Z = 10;
        static float TAILLE, MAX_SPEED, MAX_FORCE, DISTANCE_VOISINS, SEPARATION, VITESSE_ANIMATION;
        protected readonly int NB_BOIDS;
        #endregion
        protected List<Boid> _boids;
        protected float _angleCamera = 0;
        uint _genLists = 0;
        public Boids(OpenGL gl, CategorieConfiguration cat, int Nb, float Taille, float MaxSpeed, float MaxForce, float DistanceVoisins, float Separation, float VitesseAnimation) :
            base(gl)
        {
            NB_BOIDS = Nb;
            TAILLE = Taille;
            MAX_SPEED = MaxSpeed;
            MAX_FORCE = MaxForce;
            DISTANCE_VOISINS = DistanceVoisins;
            SEPARATION = Separation;
            VITESSE_ANIMATION = VitesseAnimation;
            _boids = new List<Boid>();
            InitBoids(_boids);
        }

        protected abstract Boid newBoid();
        /// <summary>
        /// Initialisation du tableau de boids
        /// </summary>
        /// <param name="_boids"></param>
        protected virtual void InitBoids(List<Boid> _boids)
        {
            // for (int i = 0; i < NB_BOIDS; i++)
            //     _boids[i] = new Boid(r.Next(-MAX_X, MAX_X), r.Next(-MAX_Y, MAX_Y), r.Next(-MAX_Z, MAX_Z));
        }

        protected abstract void DessineBoid(OpenGL gl, float noImage);

        public override void ClearBackGround(OpenGL gl, Color c)
        {
            gl.ClearColor(0, 0, 0, 1);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        }

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif         
            if (_boids.Count < NB_BOIDS)
            {
                Boid b = newBoid();
                b._couleur = couleur;
                _boids.Add(b);
            }

            gl.LookAt(-0, -0, -MAX_Z * 1.5f, 0, 0, 0, 0, 1, 0);

            InitOpenGL(gl, maintenant, couleur);

            FrustumCulling frustum = new FrustumCulling(gl);
            foreach (Boid b in _boids)
                if (frustum.isVisible(b._Position, TAILLE))
                {
                    float theta = b._Vitesse.Heading2D();
                    theta = (float)(theta / Math.PI * 180.0);// - 90.0f;
                    gl.Color(b._couleur.R / 256.0f, b._couleur.G / 256.0f, b._couleur.B / 256.0f);
                    gl.PushMatrix();
                    {
                        gl.Translate(b._Position.x, b._Position.y, b._Position.z);
                        gl.Rotate(0, 0, theta);
                        b.dessine(gl);// gl.CallList(_genLists + (uint)b._image);
                    }
                    gl.PopMatrix();
                }



#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

        protected abstract void InitOpenGL(OpenGL gl, Temps maintenant, Color couleur);

        /// <summary>
        /// Deplacement des boids
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            _angleCamera += maintenant.intervalleDepuisDerniereFrame * 1.0f;

            foreach (Boid b in _boids)
                b.flock(_boids);

            float dImage = maintenant.intervalleDepuisDerniereFrame * VITESSE_ANIMATION;
            foreach (Boid b in _boids)
            {
                b.update(maintenant);
                b._image += dImage * b._vitesseAnimation;
            }
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }


        /// <summary>
        /// Classe des "boids": objets se deplacant en bancs
        /// </summary>
        protected abstract class Boid
        {
            const float TWO_PI = (float)Math.PI * 2.0f;
            public Vecteur3D _Position;
            public Vecteur3D _Vitesse;
            public Vecteur3D _Acceleration;
            public float _image;
            public float _vitesseAnimation;
            public Color _couleur;
            public Boid(float x, float y, float z)
            {
                _Acceleration = new Vecteur3D(0, 0);
                _image = FloatRandom(0, (float)(Math.PI * 2.0));
                _vitesseAnimation = FloatRandom(0.7f, 1.3f);
                float angle = FloatRandom(0, TWO_PI);
                float vitesse = FloatRandom(-MAX_SPEED, MAX_SPEED);
                _Vitesse = new Vecteur3D((float)Math.Cos(angle)*vitesse, (float)Math.Sin(angle)*vitesse);
                _Position = new Vecteur3D(x, y, z);
            }

            // We accumulate a new acceleration each time based on three rules
            public void flock(List<Boid> boids)
            {
                Vecteur3D sep, ali, coh;
                flocking(boids, out sep, out ali, out coh);

                // Arbitrarily weight these forces
                sep.multiplier_par(1.5f);
                ali.multiplier_par(1.0f);
                coh.multiplier_par(1.0f);
                // additionner the force vectors to acceleration
                _Acceleration.additionner(sep);
                _Acceleration.additionner(ali);
                _Acceleration.additionner(coh);
            }

            private void flocking(List<Boid> boids, out Vecteur3D sep, out Vecteur3D ali, out Vecteur3D coh)
            {
                sep = new Vecteur3D();
                ali = new Vecteur3D();
                coh = new Vecteur3D();
                int countSep = 0;
                int countAlign = 0;
                int countCoh = 0;

                foreach (Boid other in boids)
                    if (other != this)
                    {
                        float d = _Position.Distance(other._Position);

                        if (d < SEPARATION)
                        {
                            // Separation
                            Vecteur3D diff = _Position - other._Position;
                            diff.Normalize();
                            diff.diviser_par(d);        // Weight by distance
                            sep.additionner(diff);
                            countSep++;            // Keep track of how many
                        }

                        if (d < DISTANCE_VOISINS)
                        {
                            // Alignement
                            ali.additionner(other._Vitesse);
                            countAlign++;

                            // Cohesion
                            coh.additionner(other._Position); // additionner location
                            countCoh++;
                        }
                    }

                // Separation
                if (countSep > 0)
                {
                    sep.diviser_par((float)countSep);
                    // As long as the vector is greater than 0
                    if (sep.Longueur() > 0)
                    {
                        // Implement Reynolds: Steering = Desired - Velocity
                        sep.Normalize();
                        sep.multiplier_par(MAX_SPEED);
                        sep.soustraire(_Vitesse);
                        sep.Limiter(MAX_FORCE);
                    }
                }

                // Alignement
                if (countAlign > 0)
                {
                    ali.diviser_par((float)countAlign);
                    ali.Normalize();
                    ali.multiplier_par(MAX_SPEED);
                    ali.soustraire(_Vitesse);
                    ali.Limiter(MAX_FORCE);

                }

                // Cohesion
                if (countCoh > 0)
                {
                    coh.diviser_par(countCoh);
                    coh = seek(coh);
                }
            }

            // Method to update location
            public virtual void update(Temps maintenant)
            {
                // Update velocity
                _Vitesse.additionner(_Acceleration);
                // Limiter speed
                _Vitesse.Limiter(MAX_SPEED);
                _Position.additionner(_Vitesse * maintenant.intervalleDepuisDerniereFrame);
                // Reset accelertion to 0 each cycle
                _Acceleration.set(0, 0, 0);

                Restreint();
            }

            protected virtual void Restreint()
            {
                Restreint(ref _Position.x, MAX_X);
                Restreint(ref _Position.y, MAX_Y);
                Restreint(ref _Position.z, MAX_Z);
            }

            private void Restreint(ref float v, float max)
            {
                max = max * 1.5f;
                while (v > max)
                    v = -max;

                while (v < -max)
                    v = max;
            }

            // A method that calculates and applies a steering force towards a target
            // STEER = DESIRED MINUS VELOCITY
            Vecteur3D seek(Vecteur3D target)
            {
                Vecteur3D desired = target - _Position;  // A vector pointing from the location to the target
                                                         // Scale to maximum speed
                desired.Normalize();
                desired.multiplier_par(MAX_SPEED);

                // Steering = Desired minus Velocity
                Vecteur3D steer = desired - _Vitesse;
                steer.Limiter(MAX_FORCE);  // Limiter to maximum steering force
                return steer;
            }

            public abstract void dessine(OpenGL gl);

        }
    }
}
