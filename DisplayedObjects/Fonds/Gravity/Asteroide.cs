using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity
{
    public class Asteroide : Planete
    {
        private struct POINT
        {
            public Vecteur3Ddbl _vertex;
            public Vecteur3Ddbl _normale;
            public PointF _textCoord;
        }

        static int NbTranches = Gravitation.DETAILS_ASTEROIDS;
        public const double TWOPI = Math.PI * 2.0;
        public const double PID2 = Math.PI / 2.0;
        private float maxTaille;

        uint _iGenList;
        
        public Asteroide(OpenGL gl, float VA, float ax, int t, float rOrbite, float posOrbite, float vOrbite, float rX, float rY, float rZ, int centrale ) : base(gl, VA, ax, t, rOrbite, posOrbite, vOrbite, rX, rY, rZ, centrale )
        {
            rX *= (float)(modeles[_type].tailleMax()) ;
            rY *= (float)(modeles[_type].tailleMax());
            rZ *= (float)(modeles[_type].tailleMax());
            maxTaille = max(rX, max(rY, rZ));

            List<List<POINT>> vertexes = new List<List<POINT>>();

            #region genereCallList
            for (int j = 0; j <= (NbTranches / 2) + 1; j++)
            {
                double theta1 = (j - 1) * TWOPI / NbTranches - PID2;
                double theta2 = (j) * TWOPI / NbTranches - PID2;

                List<POINT> bande = new List<POINT>();

                for (int i = 0; i < NbTranches; i++)
                {
                    POINT pt = new POINT();

                    double theta3 = i * TWOPI / NbTranches;
                    pt._normale = new Vecteur3Ddbl(rX * Math.Cos(theta2) * Math.Cos(theta3),
                                                                rY * Math.Sin(theta2),
                                                                rZ * Math.Cos(theta2) * Math.Sin(theta3));

                    pt._vertex = new Vecteur3Ddbl(pt._normale);
                    pt._textCoord = new PointF((float)i / (float)NbTranches, 2.0f * (j + 1) / (float)NbTranches);

                    pt._normale.Normalize();
                    pt._normale.Normal(gl);
                    gl.TexCoord(pt._textCoord.X, pt._textCoord.Y);

                    float f = DisplayedObject.FloatRandom(0.9f, 1.1f);
                    pt._vertex.x *= f;
                    pt._vertex.y *= f;
                    pt._vertex.z *= f;

                    pt._vertex.Vertex(gl);
                    bande.Add(pt);
                }
                vertexes.Add(bande);
            }

            _iGenList = gl.GenLists(1);

            gl.NewList(_iGenList, OpenGL.GL_COMPILE);
            gl.Begin(OpenGL.GL_QUAD_STRIP);
            List<POINT> bandeMUn = vertexes[0];
            for (int i = 1; i < vertexes.Count; i++)
            {
                List<POINT> bande = vertexes[i];

                for (int j = 0; j < bande.Count; j++)
                {

                    bandeMUn[j]._normale.Normal(gl);
                    gl.TexCoord(bandeMUn[j]._textCoord.X, bandeMUn[j]._textCoord.Y);
                    bandeMUn[j]._vertex.Vertex(gl);
                    bande[j]._normale.Normal(gl);
                    gl.TexCoord(bande[j]._textCoord.X, bande[j]._textCoord.Y);
                    bande[j]._vertex.Vertex(gl);

                }

                bandeMUn = bande;
            }
            gl.End();
            gl.EndList();
            #endregion
        }

        static private float max(float rY, float rZ)
        {
            return rY > rZ ? rY : rZ;
        }

        public override void  addPrimitives(OpenGL gl, List<Primitive3D> primitives)
        {
            if (Primitive3D.isVisible(_position, maxTaille))
            {
                Primitive3D p = new PrimitiveCallList(_iGenList, _position, maxTaille, new Vecteur3Ddbl(_anglePoles, _angle, 0));
                p.ALPHABLEND = false;
                p.LIGHTING = true;
                p.TEXTURE = modeles[_type].texture;
                p.MATERIAL = modeles[_type]._material;
                primitives.Add(p);
            }
        }
    }
}
