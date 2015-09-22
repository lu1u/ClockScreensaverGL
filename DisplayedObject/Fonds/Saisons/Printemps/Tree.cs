using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ClockScreenSaverGL.Fonds.Printemps;

namespace ClockScreenSaverGL.Fonds.Printemps
{
    public class Tree
    {
        public bool DoneGrowing = false;

        Vector3 Position;

        readonly int NB_CIBLES;
        readonly int LARGEUR_ARBRE;
        readonly int HAUTEUR_ARBRE;
        readonly int HAUTEUR_TRONC;
        readonly int DISTANCE_MIN;
        readonly int DISTANCE_MAX;
        readonly int LONGUEUR_BRANCHE;
        readonly float LARGEUR_TRONC;

        readonly static float RATIO_TAILLE_PARENT = 1.01f;
        Branch _racineArbre;
        List<Cible> _ciblesBranches;
        Dictionary<Vector3, Branch> _branches;
        List<Feuille> _feuilles;
        Random r = new Random();
        float _oscillation;

        public Tree(float X, float Y, float Z, float LargeurTronc, int LargeurArbre, int HauteurArbre, int LongueurBranche, int DistanceMin, int DistanceMax, int NbCibles, int HauteurTronc)
        {
            Position = new Vector3(X, Y, Z);
            LARGEUR_TRONC = LargeurTronc;
            LARGEUR_ARBRE = LargeurArbre;
            HAUTEUR_ARBRE = HauteurArbre;
            LONGUEUR_BRANCHE = LongueurBranche;
            DISTANCE_MAX = DistanceMax;
            DISTANCE_MIN = DistanceMin;
            NB_CIBLES = NbCibles;
            HAUTEUR_TRONC = HauteurTronc;

            GenerateCrown();
            GenereTronc();

            _feuilles = new List<Feuille>();
        }

        private void GenerateCrown()
        {
            _ciblesBranches = new List<Cible>();
            GenereFeuilles(_ciblesBranches, Position.X - HAUTEUR_TRONC - LARGEUR_ARBRE / 2,
                                   Position.Y,
                                   LARGEUR_ARBRE / 2,
                                   HAUTEUR_ARBRE / 2, NB_CIBLES);
        }

        private void GenereFeuilles(List<Cible> Leaves, float X, float Y, int Largeur, int Hauteur, int NbFeuilles)
        {
            float angleX, dx, dy,angleZ, dz ;
            for (int i = 0; i < NbFeuilles; i++)
            {
                angleX = r.Next(0, (int)(Math.PI * 400.0)) / 100.0f;
                angleZ = r.Next(0, (int)(Math.PI * 400.0)) / 100.0f;
                dx = r.Next(0, Largeur * 10) / 10.0f;
                dy = r.Next(0, Hauteur * 10) / 10.0f;
                dz = r.Next(0, Hauteur * 10) / 10.0f;

                Leaves.Add(new Cible(new Vector3(X + (float)Math.Sin(angleX) * dx,
                                    Y + (float)Math.Cos(angleX) * dy,(float)Math.Sin(angleZ) * dz)));
            }
        }

        private void GenereTronc()
        {
            _branches = new Dictionary<Vector3, Branch>();

            _racineArbre = new Branch(null, Position, new Vector3(-1, 0, 0));
            _racineArbre.Size = LARGEUR_TRONC;
            _branches.Add(_racineArbre.Position, _racineArbre);

            Branch current = new Branch(_racineArbre, new Vector3(Position.X - LONGUEUR_BRANCHE, Position.Y, 0), new Vector3(-1, 0, 0));
            _branches.Add(current.Position, current);

            //Keep growing trunk upwards until we reach a leaf      
            while ((_racineArbre.Position - current.Position).Length() < HAUTEUR_TRONC)
            {
                Branch trunk = new Branch(current, new Vector3(current.Position.X - LONGUEUR_BRANCHE, current.Position.Y, 0), new Vector3(-1, 0, 0));
                _branches.Add(trunk.Position, trunk);
                current = trunk;
            }
        }

        public void Grow()
        {
            if (DoneGrowing) return;
            DoneGrowing = true;

            //If no leaves left, we are done
            if (_ciblesBranches.Count == 0) { DoneGrowing = true; return; }

            //process the leaves
            for (int i = 0; i < _ciblesBranches.Count; i++)
            {
                bool leafRemoved = false;

                _ciblesBranches[i].ClosestBranch = null;
                Vector3 direction = Vector3.Zero;

                //Find the nearest branch for this leaf
                foreach (Branch b in _branches.Values)
                {
                    direction = _ciblesBranches[i].Position - b.Position;                       //direction to branch from leaf
                    float distance = (float)Math.Round(direction.Length());            //distance to branch from leaf
                    direction.Normalize();

                    if (distance <= DISTANCE_MIN)            //Min leaf distance reached, we remove it
                    {
                        _feuilles.Add(new Feuille(b.Position));

                        _ciblesBranches.Remove(_ciblesBranches[i]);
                        i--;
                        leafRemoved = true;
                        break;
                    }
                    else if (distance <= DISTANCE_MAX)       //branch in range, determine if it is the nearest
                    {
                        if (_ciblesBranches[i].ClosestBranch == null)
                            _ciblesBranches[i].ClosestBranch = b;
                        else if ((_ciblesBranches[i].Position - _ciblesBranches[i].ClosestBranch.Position).Length() > distance)
                            _ciblesBranches[i].ClosestBranch = b;
                    }
                }

                if (!leafRemoved)
                {
                    //Set the grow parameters on all the closest branches that are in range
                    if (_ciblesBranches[i].ClosestBranch != null)
                    {
                        Vector3 dir = _ciblesBranches[i].Position - _ciblesBranches[i].ClosestBranch.Position;
                        dir.Normalize();
                        _ciblesBranches[i].ClosestBranch.GrowDirection += dir;       //add to grow direction of branch
                        _ciblesBranches[i].ClosestBranch.GrowCount++;
                    }

                    _ciblesBranches[i].Position.X += r.Next(-1, 2);
                    _ciblesBranches[i].Position.Y += r.Next(-1, 2);
                    _ciblesBranches[i].Position.Z += r.Next(-1, 2);
                    DoneGrowing = false;
                }


            }

            //Generate the new branches
            HashSet<Branch> newBranches = new HashSet<Branch>();
            foreach (Branch b in _branches.Values)
            {

                if (b.GrowCount > 0)    //if at least one leaf is affecting the branch
                {
                    Vector3 avgDirection = b.GrowDirection / b.GrowCount;
                    avgDirection.Normalize();

                    Branch newBranch = new Branch(b, b.Position + avgDirection * LONGUEUR_BRANCHE, avgDirection);

                    newBranches.Add(newBranch);
                    b.Reset();
                }
            }

            if (newBranches.Count == 0) { DoneGrowing = true; return; }

            //Add the new branches to the tree
            foreach (Branch b in newBranches)
            {
                //Check if branch already exists.  These cases seem to happen when leaf is in specific areas
                Branch existing;
                if (!_branches.TryGetValue(b.Position, out existing))
                {
                    _branches.Add(b.Position, b);
                    DoneGrowing = false;
                    //increment the size of the older branches, direct path to root
                    b.Size = Branch.LARGEUR_INITIALE;
                    Branch p = b.Parent;
                    while (p != null)
                    {
                        if (p.Parent != null)
                            p.Parent.Size = p.Size * RATIO_TAILLE_PARENT;

                        p = p.Parent;

                    }
                }
            }

            for (int i = 0; i < _ciblesBranches.Count; i++)
            {
                _ciblesBranches[i].Position.X += r.Next(-1, 2);
                _ciblesBranches[i].Position.Y += r.Next(-1, 2);
            }
        }


        public void Draw(Graphics g)
        {
            /*
            foreach (Cible b in _ciblesBranches )
                b.Draw(g);
             */

            foreach (Branch b in _branches.Values)
                b.Draw(g, 0, _oscillation * (b.Position.X - Position.X));

            foreach (Feuille f in _feuilles)
            {
                f.Grow(0.1f);
                f.Draw(g, 0, _oscillation * (f.Position.X - Position.X));
            }
        }

        internal void Oscillation(float p)
        {
            _oscillation = p;
        }
    }
}
