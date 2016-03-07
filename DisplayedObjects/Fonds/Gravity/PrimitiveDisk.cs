using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;
using SharpGL.SceneGraph.Quadrics;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity
{
    public class PrimitiveDisk : Primitive3D
    {
        private float _taille;
        private Vecteur3Ddbl _rotation;
        static private Disk _disc;

        public PrimitiveDisk(OpenGL gl, Vecteur3Ddbl position, float rayon, Vecteur3Ddbl rotation) : base(position)
        {
            if (_disc == null)
            {
                _disc = new Disk();
                _disc.CreateInContext(gl);
                _disc.NormalGeneration = SharpGL.SceneGraph.Quadrics.Normals.Smooth;
                _disc.NormalOrientation = SharpGL.SceneGraph.Quadrics.Orientation.Outside;
                _disc.QuadricDrawStyle = SharpGL.SceneGraph.Quadrics.DrawStyle.Fill;
                _disc.Slices = 80;
                _disc.TextureCoords = true;
            }

            _taille = rayon;
            _rotation = rotation;
        }

        public override void dessine(OpenGL gl)
        {
            if (isVisible(_position, _taille))
            {
                double Details = CalculeNiveauDetail(_position, _taille) / 10.0;

                _disc.Transformation.TranslateX = (float)_position.x;
                _disc.Transformation.TranslateY = (float)_position.y;
                _disc.Transformation.TranslateZ = (float)_position.z;
                _disc.Transformation.ScaleX =_taille;
                _disc.Transformation.ScaleY = _taille;
                _disc.Transformation.ScaleZ = _taille;
                _disc.Transformation.RotateX = (float)_rotation.x - 90;
                _disc.Transformation.RotateY = (float)_rotation.y;
                _disc.Transformation.RotateZ = (float)_rotation.z;
                _disc.Slices = (int)(Details * Gravitation.DETAILS);
               
                setAttributes(gl);

                _disc.PushObjectSpace(gl);
                _disc.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                _disc.PopObjectSpace(gl);
            }
        }
    }
}
