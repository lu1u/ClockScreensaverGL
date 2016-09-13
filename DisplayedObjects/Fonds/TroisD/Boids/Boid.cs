using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Boids
{
    class Boid
    {
        const float TWO_PI = (float)Math.PI * 2.0f;
        private static Random rand = new Random(DateTime.Now.Millisecond);
        public Vecteur3D _Position;
        public Vecteur3D _Vitesse;
        public Vecteur3D _Acceleration;
        public float _vitesseAnimation;
        public float image;
        public static readonly float TAILLE = Boids.TAILLE;
        static readonly float DISTANCE_VOISINS = TAILLE * 15.0f; // 50
        static readonly float SEPARATION = TAILLE * 4.0f; // 25
        static readonly float MAX_SPEED = 20f;    // Maximum speed 2.0
        static readonly float MAX_FORCE = MAX_SPEED * 0.002f;    // Maximum steering force 0.04

        public Boid(float x, float y, float z)
        {
            _Acceleration = new Vecteur3D(0, 0);
            image = DisplayedObject.FloatRandom(0, Boids.NB_IMAGES);
            _vitesseAnimation = DisplayedObject.FloatRandom(0.8f, 1.2f);
            float angle = DisplayedObject.FloatRandom(0, TWO_PI);
            _Vitesse = new Vecteur3D((float)Math.Cos(angle), (float)Math.Sin(angle));

            _Position = new Vecteur3D(x, y, z);
        }

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

        // Method to update location
        public void update(float Intervalle)
        {
            // Update velocity
            _Vitesse.additionner(_Acceleration);
            // Limiter speed
            _Vitesse.Limiter(MAX_SPEED);
            _Position.additionner(_Vitesse * Intervalle);
            // Reset accelertion to 0 each cycle
            _Acceleration.set(0, 0, 0);
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
}
