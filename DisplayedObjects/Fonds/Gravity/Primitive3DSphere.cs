using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;
using ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD;
using SharpGL.SceneGraph.Quadrics;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity
{
    public class Primitive3DSphere : Primitive3D
    {
        private Vecteur3Ddbl _taille;
        private Vecteur3Ddbl _rotation;
        static private Sphere _sphere;

        public Primitive3DSphere(OpenGL gl, Vecteur3Ddbl position, Vecteur3Ddbl taille, Vecteur3Ddbl rotation) : base(position)
        {
            if (_sphere == null)
            {
                _sphere = new Sphere();
                _sphere.CreateInContext(gl);
                _sphere.NormalGeneration = SharpGL.SceneGraph.Quadrics.Normals.Smooth;
                _sphere.NormalOrientation = SharpGL.SceneGraph.Quadrics.Orientation.Outside;
                _sphere.QuadricDrawStyle = SharpGL.SceneGraph.Quadrics.DrawStyle.Fill;
                _sphere.Slices = 80;
                _sphere.Stacks = 80;
                _sphere.TextureCoords = true;
            }

            _taille = taille;
            _rotation = rotation;
        }

        public override void dessine(OpenGL gl)
        {
            if (isVisible(_position, _taille.x))
            {
                double Details = CalculeNiveauDetail(_position, _taille.x);

                _sphere.Transformation.TranslateX = (float)_position.x;
                _sphere.Transformation.TranslateY = (float)_position.y;
                _sphere.Transformation.TranslateZ = (float)_position.z;
                _sphere.Transformation.ScaleX = (float)_taille.x;
                _sphere.Transformation.ScaleY = (float)_taille.y;
                _sphere.Transformation.ScaleZ = (float)_taille.z;
                _sphere.Transformation.RotateX = (float)_rotation.x - 90;
                _sphere.Transformation.RotateY = (float)_rotation.y;
                _sphere.Transformation.RotateZ = (float)_rotation.z;
                _sphere.Slices = (int)(Details*Gravitation.DETAILS);
                _sphere.Stacks = (int)(Details* Gravitation.DETAILS);

                setAttributes(gl);

                _sphere.PushObjectSpace(gl);
                _sphere.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                _sphere.PopObjectSpace(gl);
            }
        }
    }
}
