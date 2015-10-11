using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;


namespace ClockScreenSaverGL.DisplayedObject.Fonds.Printemps
{
    public class Cible
    {
        public Vector3 Position;
        public Branch ClosestBranch;

        public Cible(Vector3 position)
        {
            Position = position;
        }

        public void Draw(Graphics g)
        {
            g.FillRectangle(Brushes.Green, Position.X, Position.Y, 2, 2);
        }

        public void Draw(OpenGL gl)
        {
            gl.Vertex(Position.X, Position.Y);
        }
    }
}
