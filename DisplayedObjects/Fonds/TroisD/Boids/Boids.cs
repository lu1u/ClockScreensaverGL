using System;

using GLfloat = System.Single;
using SharpGL;
using System.Drawing;
using SharpGL.SceneGraph.Assets;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Boids
{
    abstract class Boids : MateriauGlobal, IDisposable
    {
        #region Parametres
        const String CAT = "Boids";

        private static readonly int NB_IMAGES_QUEUE = conf.getParametre(CAT, "Nb images queue", 20);
        public static readonly int MAX_X = 200;// conf.getParametre(CAT, "Max X", 100);
        public static readonly int MAX_Y = 150;// conf.getParametre(CAT, "Max Y", 100);
        public static readonly int MAX_Z = 100; //conf.getParametre(CAT, "Max Z", 100);
        static float  TAILLE, MAX_SPEED, MAX_FORCE, DISTANCE_VOISINS, SEPARATION, VITESSE_ANIMATION;
        protected readonly int NB_BOIDS;
        #endregion




#if OISEAUX
        static readonly float HAUTEUR_CORPS = 0.25f * TAILLE;
        static readonly float LONGUEUR_TETE = 1.25f * TAILLE;
        static readonly float LONGUEUR_CORPS = -0.75f * TAILLE;
        static readonly float LONGUEUR_QUEUE = -0.55f * TAILLE;
        static readonly float LARGEUR_QUEUE = 0.75f * TAILLE;
        static readonly float LARGEUR_AILES = 5.0f * TAILLE;
        static readonly float COULEUR_FOND = 256.0f;
        static readonly float COULEUR_BOIDS = 1024.0f;
        static readonly float VITESSE_IMAGES = 4.0f;
#endif
        protected Boid[] _boids;
        protected float _angleCamera = 0;
        uint _genLists =0;
        public Boids(OpenGL gl, int Nb, float Taille, float MaxSpeed, float MaxForce, float DistanceVoisins, float Separation, float VitesseAnimation ) : base(gl, CAT)
        {
            NB_BOIDS = Nb;
            TAILLE = Taille;
            MAX_SPEED = MaxSpeed;
            MAX_FORCE = MaxForce;
            DISTANCE_VOISINS = DistanceVoisins;
            SEPARATION = Separation;
            VITESSE_ANIMATION = VitesseAnimation;
            _boids = new Boid[NB_BOIDS];
            InitBoids(_boids);
            InitRender(gl);
            
        }

        protected virtual void InitRender(OpenGL gl)
        {
            _genLists = gl.GenLists(NB_IMAGES_QUEUE);

            for (int i = 0; i < NB_IMAGES_QUEUE; i++)
            {
                gl.NewList(_genLists + (uint)i, OpenGL.GL_COMPILE);
                DessineBoid(gl, (float)i / NB_IMAGES_QUEUE);
                gl.EndList();
            }
        }

        /// <summary>
        /// Initialisation du tableau de boids
        /// </summary>
        /// <param name="_boids"></param>
        protected virtual void InitBoids(Boid[] _boids)
        {
            for (int i = 0; i < NB_BOIDS; i++)
                _boids[i] = new Boid(r.Next(-MAX_X, MAX_X), r.Next(-MAX_Y, MAX_Y), r.Next(-MAX_Z, MAX_Z));
        }

        protected abstract void DessineBoid(OpenGL gl, float noImage);

        public override void Dispose()
        {
            base.Dispose();
            if  (_genLists != 0)
                _gl.DeleteLists(_genLists, NB_IMAGES_QUEUE);
        }


        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif           
            InitOpenGL(gl, maintenant, couleur);


            FrustumCulling frustum = new FrustumCulling(gl);


            foreach (Boid b in _boids)
                if (frustum.isVisible(b._Position, TAILLE))
                {
                    float theta = b._Vitesse.Heading2D();
                    theta = (float)(theta / Math.PI * 180.0);// - 90.0f;
                    gl.PushMatrix();
                    gl.Translate(b._Position.x, b._Position.y, b._Position.z);
                    gl.Rotate(0, 0, theta);
                    gl.CallList(_genLists + (uint)b.image);

                    gl.End();
                    gl.PopMatrix();
                }

            fillConsole(gl);
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

        protected abstract void InitOpenGL(OpenGL gl, Temps maintenant, Color couleur);

        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            _angleCamera += maintenant._intervalle * 0.1f;

            foreach (Boid b in _boids)
                b.flock(_boids);

            float dImage = NB_IMAGES_QUEUE * maintenant._intervalle * VITESSE_ANIMATION ;
            foreach (Boid b in _boids)
            {
                b.update(maintenant);


                b.image += dImage * b._vitesseAnimation;
                if (b.image >= NB_IMAGES_QUEUE)
                    b.image = 0;
            }
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }


        /// <summary>
        /// Classe des "boids": objets se deplacant en bancs
        /// </summary>
        protected class Boid
        {
            const float TWO_PI = (float)Math.PI * 2.0f;
            private static Random rand = new Random(DateTime.Now.Millisecond);
            public Vecteur3D _Position;
            public Vecteur3D _Vitesse;
            public Vecteur3D _Acceleration;
            public float image;
            public float _vitesseAnimation;

            public Boid(float x, float y, float z)
            {
                _Acceleration = new Vecteur3D(0, 0);
                image = DisplayedObject.FloatRandom(0, Boids.NB_IMAGES_QUEUE);
                _vitesseAnimation = DisplayedObject.FloatRandom(0.8f, 1.2f);
                float angle = DisplayedObject.FloatRandom(0, TWO_PI);
                _Vitesse = new Vecteur3D((float)Math.Cos(angle), (float)Math.Sin(angle));

                _Position = new Vecteur3D(x, y, z);
            }

#if version1
            // We accumulate a new acceleration each time based on three rules
            public void flock(Boid[] boids)
            {
                Vecteur3D sep = Separation(boids);   // Separation
                Vecteur3D ali = Alignement(boids);      // Alignment
                Vecteur3D coh = Cohesion(boids);   // Cohesion
                 
                // Arbitrarily weight these forces
                sep.multiplier_par(1.5f);
                ali.multiplier_par(1.0f);
                coh.multiplier_par(1.0f);
                // additionner the force vectors to acceleration
                _Acceleration.additionner(sep);
                _Acceleration.additionner(ali);
                _Acceleration.additionner(coh);
            }
#endif

            // We accumulate a new acceleration each time based on three rules
            public void flock(Boid[] boids)
            {
                //Vecteur3D sep = Separation(boids);   // Separation
                //Vecteur3D ali = Alignement(boids);      // Alignment
                //Vecteur3D coh = Cohesion(boids);   // Cohesion

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

            private void flocking(Boid[] boids, out Vecteur3D sep, out Vecteur3D ali, out Vecteur3D coh)
            {
                sep = new Vecteur3D(0, 0, 0);
                ali = new Vecteur3D(0, 0, 0);
                coh = new Vecteur3D(0, 0, 0);
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

                if (countAlign > 0)
                {
                    ali.diviser_par((float)countAlign);
                    ali.Normalize();
                    ali.multiplier_par(MAX_SPEED);
                    ali.soustraire(_Vitesse);
                    ali.Limiter(MAX_FORCE);

                }

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
                _Position.additionner(_Vitesse * maintenant._intervalle);
                // Reset accelertion to 0 each cycle
                _Acceleration.set(0, 0, 0);

                Restreint();
            }

            protected virtual void Restreint()
            {
                if (_Position.x < -MAX_X)
                    _Position.x += MAX_X * 2;
                if (_Position.x > MAX_X)
                    _Position.x -= MAX_X * 2;
                if (_Position.y < -MAX_Y)
                    _Position.y += MAX_Y * 2;
                if (_Position.y > MAX_Y)
                    _Position.y -= MAX_Y * 2;
                /*if (_Position.z < -MAX_Z)
                    _Position.z += MAX_Z * 2;
                if (_Position.z > MAX_Z)
                    _Position.z -= MAX_Z * 2;*/
                /*
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
            */
                if (_Position.z < -MAX_Z)
                    if (_Vitesse.z < 0)
                        _Vitesse.z = -_Vitesse.z;

                if (_Position.z > MAX_Z)
                    if (_Vitesse.z > 0)
                        _Vitesse.z = -_Vitesse.z;
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


#if version1
            // Separation
            // Method checks for nearby boids and steers away
            Vecteur3D Separation(Boid[] boids)
            {
                Vecteur3D steer = new Vecteur3D(0, 0, 0);
                int count = 0;
                // For every boid in the system, check if it's too close
                foreach (Boid other in boids)
                    if (other != this)
                    {
                        float d = _Position.Distance(other._Position);
                        // If the distance is greater than 0 and less than an arbitrary amount
                        if (d < SEPARATION)
                        {
                            // Calculate vector pointing away from neighbor
                            Vecteur3D diff = _Position - other._Position;
                            diff.Normalize();
                            diff.diviser_par(d);        // Weight by distance
                            steer.additionner(diff);
                            count++;            // Keep track of how many
                        }
                    }
                // Average -- divide by how many
                if (count > 0)
                {
                    steer.diviser_par((float)count);

                    // As long as the vector is greater than 0
                    if (steer.Longueur() > 0)
                    {
                        // Implement Reynolds: Steering = Desired - Velocity
                        steer.Normalize();
                        steer.multiplier_par(MAX_SPEED);
                        steer.soustraire(_Vitesse);
                        steer.Limiter(MAX_FORCE);
                    }
                }
                return steer;
            }

            // Alignment
            // For every nearby boid in the system, calculate the average velocity
            Vecteur3D Alignement(Boid[] boids)
            {
                Vecteur3D sum = new Vecteur3D(0, 0);
                int count = 0;
                foreach (Boid other in boids)
                    if (other != this)
                    {
                        float d = _Position.Distance(other._Position);
                        if (d < DISTANCE_VOISINS)
                        {
                            sum.additionner(other._Vitesse);
                            count++;
                        }
                    }

                if (count > 0)
                {
                    sum.diviser_par((float)count);
                    // First two lines of code below could be condensed with new Vecteur3D setMag() method
                    // Not using this method until Processing.js catches up
                    // sum.setMag(maxspeed);

                    // Implement Reynolds: Steering = Desired - Velocity
                    sum.Normalize();
                    sum.multiplier_par(MAX_SPEED);
                    Vecteur3D steer = sum - _Vitesse;
                    steer.Limiter(MAX_FORCE);
                    return steer;
                }
                else
                    return sum;// new Vecteur3D(0, 0);
            }

            // Cohesion
            // For the average location (i.e. center) of all nearby boids, calculate steering vector towards that location
            Vecteur3D Cohesion(Boid[] boids)
            {
                Vecteur3D sum = new Vecteur3D(0, 0);   // Start with empty vector to accumulate all locations
                int count = 0;
                foreach (Boid other in boids)
                    if (other != this)
                    {
                        float d = _Position.Distance(other._Position);
                        if (d < DISTANCE_VOISINS)
                        {
                            sum.additionner(other._Position); // additionner location
                            count++;
                        }
                    }

                if (count > 0)
                {
                    sum.diviser_par(count);
                    return seek(sum);  // Steer towards the location
                }
                else
                    return sum;// Vecteur3D(0, 0);

            }
        }
#endif
        }
    }
}
