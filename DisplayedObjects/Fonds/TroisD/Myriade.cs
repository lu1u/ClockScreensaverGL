﻿using ClockScreenSaverGL.Config;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Drawing;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD
{
    class Myriade : TroisD, IDisposable
    {
        public const string CAT = "Myriade";
        static protected CategorieConfiguration c = Configuration.getCategorie(CAT);
        const float VIEWPORT = 20f;
        private static readonly float[] fogcolor = { 0, 0, 0, 1 };
        private static bool ADDITIVE = c.getParametre("Additive", true, (a) => { ADDITIVE = Convert.ToBoolean(a); } );
        private static byte ALPHA_ETOILE = c.getParametre("Alpha Etoile", (byte)255, (a) => { ALPHA_ETOILE = Convert.ToByte(a); });
        private static float DISTANCE  = c.getParametre("Ratio Distance", 1.0f, (a) => { DISTANCE = (float)Convert.ToDouble(a); } );
        private static float FOG_DENSITY  = c.getParametre("Fog density", 0.1f, (a) => { FOG_DENSITY = (float)Convert.ToDouble(a); } );
        private static int NB_POINTS = c.getParametre("Nb Etoiles", 20000);
        private static int FREQUENCE_TRI = c.getParametre("Frequence tri", 10, (a) => { FREQUENCE_TRI = Convert.ToInt32(a);    });
        private static float TAILLE = c.getParametre("Taille", 0.4f, (a) => { TAILLE = (float)Convert.ToDouble(a); });
        private static float VITESSE_ROTATION = c.getParametre("Vitesse rotation", 0.5f, (a) => { VITESSE_ROTATION = (float)Convert.ToDouble(a); });
        private readonly int NB_IMAGES ;
        private float _angle = 0;
        private Point[] _points;
        Texture _texture = new Texture();

        public Myriade( OpenGL gl ) : base( gl, VIEWPORT, VIEWPORT, VIEWPORT, 100 )
        {
            string nomImage = c.getParametre("Etoile", Configuration.getImagePath("etoile.png"));
            NB_IMAGES = getNbImagesTexture( nomImage );
            _texture.Create( gl, nomImage );

            _points = new Point[NB_POINTS];

            for ( int i = 0; i < NB_POINTS; i++ )
            {
                float x = FloatRandom(-VIEWPORT,VIEWPORT);
                float y = FloatRandom(-VIEWPORT,VIEWPORT);
                float z = FloatRandom(-VIEWPORT,VIEWPORT);

                _points[i] = new Point( new Vecteur3D( x, y, z ),
                    FloatRandom( 0.7f, 1.3f ), FloatRandom( 0.7f, 1.3f ), FloatRandom( 0.7f, 1.3f ), r.Next( 0, NB_IMAGES ) );
            }
        }

        public override void AfficheOpenGL( OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur )
        {
#if TRACER
            RenderStart( CHRONO_TYPE.RENDER );
#endif
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1f };
            gl.LoadIdentity();
            gl.Disable( OpenGL.GL_LIGHTING );
            gl.Disable( OpenGL.GL_DEPTH_TEST );
            gl.Enable( OpenGL.GL_BLEND );
            gl.Enable( OpenGL.GL_TEXTURE_2D );

            gl.BlendFunc( OpenGL.GL_SRC_ALPHA, ADDITIVE ? OpenGL.GL_ONE : OpenGL.GL_ONE_MINUS_SRC_ALPHA );

            changeZoom( gl, tailleEcran.Width, tailleEcran.Height, 0.001f, VIEWPORT * 2.0f );
            gl.LookAt( -VIEWPORT * DISTANCE, -VIEWPORT * DISTANCE, -VIEWPORT * DISTANCE, 0, 0, 0, 0, 1, 0 );
            gl.Rotate( _angle /** 0.5f*/, _angle /** 2.5f*/, _angle );
            gl.Enable( OpenGL.GL_FOG );
            gl.Fog( OpenGL.GL_FOG_MODE, OpenGL.GL_EXP );
            gl.Fog( OpenGL.GL_FOG_COLOR, fogcolor );
            gl.Fog( OpenGL.GL_FOG_DENSITY, FOG_DENSITY );
            gl.Fog( OpenGL.GL_FOG_START, 0 );
            gl.Fog( OpenGL.GL_FOG_END, VIEWPORT *0.2f );

            if ( UneFrameSur( FREQUENCE_TRI ) )
                trieEtoiles( gl );

            FrustumCulling fr = new FrustumCulling(gl);

            Vecteur3D vPoint0, vPoint1, vPoint2, vPoint3;
            float[] mat = new float[16];
            gl.GetFloat( OpenGL.GL_MODELVIEW_MATRIX, mat );
            Vecteur3D vRight = new Vecteur3D(mat[0], mat[4], mat[8]) * TAILLE;
            Vecteur3D vUp = new Vecteur3D(mat[1], mat[5], mat[9]) * TAILLE;

            _texture.Bind( gl );
            
            gl.Begin( OpenGL.GL_QUADS );
            foreach ( Point o in _points )
            {
                if ( fr.isVisible( o.position, TAILLE ) )
                {
                    vPoint0 = o.position + ((-vRight - vUp));
                    vPoint1 = o.position + ((vRight - vUp));
                    vPoint2 = o.position + ((vRight + vUp));
                    vPoint3 = o.position + ((-vRight + vUp));

                    float xG = 1.0f / NB_IMAGES * o.image;
                    float xD = 1.0f / NB_IMAGES * (o.image + 1);

                    gl.Color( couleur.R * o.rR / 128.0f, couleur.G * o.rG / 128.0f, couleur.B * o.rB / 128.0f, ALPHA_ETOILE / 256.0f );
                    gl.TexCoord( xG, 1.0f ); gl.Vertex( vPoint0.x, vPoint0.y, vPoint0.z );
                    gl.TexCoord( xG, 0.0f ); gl.Vertex( vPoint1.x, vPoint1.y, vPoint1.z );
                    gl.TexCoord( xD, 0.0f ); gl.Vertex( vPoint2.x, vPoint2.y, vPoint2.z );
                    gl.TexCoord( xD, 1.0f ); gl.Vertex( vPoint3.x, vPoint3.y, vPoint3.z );
                }
            }
            gl.End();

            //DessineCroix(gl, Vecteur3D.NULL, 10, Color.Red);
#if TRACER
            RenderStop( CHRONO_TYPE.RENDER );
#endif
        }

        public override void ClearBackGround( OpenGL gl, Color c )
        {
            gl.ClearColor( 0, 0, 0, 1 );
            gl.Clear( OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT );
        }

        public override void Deplace( Temps maintenant, Rectangle tailleEcran )
        {
#if TRACER
            RenderStart( CHRONO_TYPE.DEPLACE );
#endif
            base.Deplace( maintenant, tailleEcran );
            _angle += VITESSE_ROTATION * maintenant.intervalleDepuisDerniereFrame;
#if TRACER
            RenderStop( CHRONO_TYPE.DEPLACE );
#endif
        }

        public override void Dispose()
        {
            base.Dispose();
            _texture.Destroy( _gl );
        }

        public override CategorieConfiguration getConfiguration()
        {
            return c;
        }


        static Vecteur3D getCameraPos( OpenGL gl )
        {
            double[] mdl = new double[16];
            gl.GetDouble( OpenGL.GL_MODELVIEW_MATRIX, mdl );
            return new Vecteur3D( (float) -(mdl[0] * mdl[12] + mdl[1] * mdl[13] + mdl[2] * mdl[14]),
                (float) -(mdl[4] * mdl[12] + mdl[5] * mdl[13] + mdl[6] * mdl[14]),
                (float) -(mdl[8] * mdl[12] + mdl[9] * mdl[13] + mdl[10] * mdl[14]) );
        }

        private int getNbImagesTexture( string nomImage )
        {
            Image b = Image.FromFile(nomImage);
            return b.Width / b.Height;
        }

        private void trieEtoiles( OpenGL gl )
        {
            Vecteur3D camPos = getCameraPos(gl);
            foreach ( Point e in _points )
                e.distanceCam = e.position.Distance( camPos );

            Array.Sort( _points, delegate ( Point O1, Point O2 )
             {
                 if ( O1.distanceCam > O2.distanceCam ) return -1;
                 if ( O1.distanceCam < O2.distanceCam ) return 1;
                 return 0;
             } );
        }

        private class Point
        {
            public float distanceCam;
            public int image;
            public Vecteur3D position;
            public float rR, rG, rB;
            public Point(Vecteur3D pos, float r, float g, float b, int i)
            {
                position = pos;
                rR = r;
                rG = g;
                rB = b;
                image = i;
            }
        }
#if CROIX
        //////////////////////////////////////////////////////////////////////
        // Dessine une croix ( axes X, Y et Z ) autour d'une position donnee
        // ENTREES:	Position de la croix
        //			Taille de la croix
        ///////////////////////////////////////////////////////////////////////////////
        public static void DessineCroix(OpenGL gl, Vecteur3D Position, float Taille, Color Couleur)
        {
            gl.PushAttrib(OpenGL.GL_ENABLE_BIT);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Color(Couleur.R / 256.0f, Couleur.G / 256.0f, Couleur.B / 256.0f, 0.7f);
            gl.LineWidth(2);
            {
                gl.PushMatrix();
                gl.Translate(Position.x, Position.y, Position.z);
                {
                    gl.Begin(OpenGL.GL_LINES);
                    // X: vert
                    //gl.Color(0, 1.0f, 0);
                    gl.Vertex(-Taille, 0, 0);
                    gl.Vertex(Taille, 0, 0);


                    // Z: rouge
                    //gl.Color(0, 0, 1.0f);
                    gl.Vertex(0, 0, -Taille);
                    gl.Vertex(0, 0, Taille);

                    // Y
                    gl.Vertex(0, -Taille, 0);
                    gl.Vertex(0, Taille, 0);

                    gl.End();
                }
                gl.PopMatrix();
            }
            gl.PopAttrib();
        }
#endif
    }
}
