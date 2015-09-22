
using System.Drawing;
namespace ClockScreenSaverGL.Fonds.Printemps
{
    public class Branch
    {
        readonly public static float LARGEUR_INITIALE = 2 ;
        
        public Branch Parent { get; private set; }
        public Vector3 GrowDirection { get; set; }
        public Vector3 OriginalGrowDirection { get; set; }
        public int GrowCount { get; set; }
        public Vector3 Position { get; private set; }
        public float Size { get; set; }

        public Branch(Branch parent, Vector3 position, Vector3 growDirection)
        {
            Parent = parent;
            Position = position;
            GrowDirection = growDirection;
            OriginalGrowDirection = growDirection;
            Size =LARGEUR_INITIALE ;
        }

        public void Reset()
        {
            GrowCount = 0;
            GrowDirection = OriginalGrowDirection;
        }

        public void Draw(Graphics g, float dx, float dy )
        {
            if (Parent != null)
                using (Pen p = new Pen(Color.FromArgb (255,0,0,0), Size))
                   // g.DrawLine(p, Position.Point(), Parent.Position.Point());
            g.DrawLine(p, Position.X + dx, Position.Y + dy, Parent.Position.X + dx, Parent.Position.Y + dy);
        }
    }
}
