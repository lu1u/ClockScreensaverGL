using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;
using ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity
{
    public class PrimitiveCallList: Primitive3D
    {
        uint _callList;
        Vecteur3Ddbl _rotation;
        float _rayonMax;
        public PrimitiveCallList(uint CallList, Vecteur3Ddbl position, float rayonMax, Vecteur3Ddbl rotation): base(position)
        {
            _callList = CallList;
            _rotation = rotation;
            _rayonMax = rayonMax;
        }

        public override void dessine(OpenGL gl)
        {
            if (isVisible(_position, _rayonMax))
            {
                setAttributes(gl);

                gl.PushMatrix();
                gl.Translate(_position.x, _position.y, _position.z);
                gl.Rotate((float)_rotation.x, (float)_rotation.y, (float)_rotation.z);
                gl.CallList(_callList);
                gl.PopMatrix();
            }
        }
    }
}
