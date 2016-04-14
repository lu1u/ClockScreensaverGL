using SharpGL;
using SharpGL.SceneGraph.Quadrics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD
{
    class ADN : MateriauGlobal, IDisposable
    {

        const String CAT = "ADN";
        static readonly float RAYON_SPHERE = conf.getParametre(CAT, "Rayon Sphere", 1.5f);
        static readonly float LONGUEUR_RAYON = conf.getParametre(CAT, "Longueur Rayon", 10.0f);
        static readonly int NB_ETAGES = conf.getParametre(CAT, "Nb Etages", 20);
        static readonly float MIN_Y = conf.getParametre(CAT, "MinY", -18.0f);
        static readonly float MAX_Y = conf.getParametre(CAT, "Max", 18.0f);

        private class Etage
        {
            public float ratioRa, ratioGa, ratioBa;
            public float ratioRb, ratioGb, ratioBb;
            public float _angle;
            public float y;
        };

        List<Etage> _etages = new List<Etage>();
       /* static readonly float SHININESS = conf.getParametre(CAT, "Shininess", 30f);
        static readonly float[] COL_SPECULAR = { 0.5f, 0.5f, 0.5f, 1.0f };//{
        static readonly float[] COL_AMBIENT = { 0.0f, 0.0f, 0.0f, 1.0f };//{ 
        static readonly float[] COL_EMISSION = { 0.00f, 0.00f, 0.00f, 1.0f };
        static readonly float[] COL_DIFFUSE = { 0.4f, 0.4f, 0.4f, 1.0f };
        static readonly float[] COL_LIGHTPOS = { -3f, 2f, 3, 1 };
        static readonly float[] SPECULAR_LIGHT = { 0.8f, 0.8f, 0.8f }; //set the 
        static readonly float[] AMBIENT_LIGHT = { 0.01f, 0.01f, 0.01f }; //set the 
        static readonly float[] DIFFUSE_LIGHT = { 0.1f, 0.1f, 0.1f }; //set the 
        */
        Sphere _sphere = new Sphere();
        Cylinder _cylindre = new Cylinder();
        float _angle = 0;
        public ADN(OpenGL gl) : base(gl, CAT)
        {
            float y = MAX_Y;
            float angle = 0;
            for (int i = 0; i < NB_ETAGES; i++)
            {
                Etage e = new Etage();
                e.ratioRa = FloatRandom(0.8f, 1.2f);
                e.ratioGa = FloatRandom(0.8f, 1.2f);
                e.ratioBa = FloatRandom(0.8f, 1.2f);
                e.ratioRb = FloatRandom(0.8f, 1.2f);
                e.ratioGb = FloatRandom(0.8f, 1.2f);
                e.ratioBb = FloatRandom(0.8f, 1.2f);
                e.y = y;
                e._angle = angle;
                _etages.Add(e);

                y -= (MAX_Y - MIN_Y) / (float)NB_ETAGES;
                angle +=360.0f / (float)NB_ETAGES;
            }

            _sphere.CreateInContext(gl);
            _sphere.NormalGeneration = SharpGL.SceneGraph.Quadrics.Normals.Smooth;
            _sphere.NormalOrientation = SharpGL.SceneGraph.Quadrics.Orientation.Outside;
            _sphere.QuadricDrawStyle = SharpGL.SceneGraph.Quadrics.DrawStyle.Fill;
            _sphere.Slices = 40;
            _sphere.Stacks = 40 ;
            _sphere.Radius = RAYON_SPHERE;

            _cylindre.CreateInContext(gl);
            _cylindre.NormalGeneration = SharpGL.SceneGraph.Quadrics.Normals.Smooth;
            _cylindre.NormalOrientation = SharpGL.SceneGraph.Quadrics.Orientation.Outside;
            _cylindre.QuadricDrawStyle = SharpGL.SceneGraph.Quadrics.DrawStyle.Fill;
            _cylindre.Slices = 20;
            _cylindre.Stacks = 20;
            _cylindre.TopRadius =0.5f;
            _cylindre.BaseRadius = 0.5f;
            _cylindre.Height = LONGUEUR_RAYON ;
        }
        public override void ClearBackGround(OpenGL gl, Color c)
        {
            gl.ClearColor(0, 0, 0, 1);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        }
        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1f };
            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_ALPHA_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Disable(OpenGL.GL_BLEND);
            gl.Disable(OpenGL.GL_FOG);
            gl.DepthMask((byte)OpenGL.GL_TRUE);
            gl.Enable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.CullFace(OpenGL.GL_BACK);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            /*
                // Lumiere
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, COL_LIGHTPOS);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_COLOR, col);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, SPECULAR_LIGHT);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, AMBIENT_LIGHT);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, DIFFUSE_LIGHT);

            // Aspect de la surface
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SPECULAR, COL_SPECULAR);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, COL_AMBIENT);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, COL_DIFFUSE);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SHININESS, SHININESS);
            */
            setGlobalMaterial(gl, couleur);
            gl.LookAt(0, 0, -20, 0, 0, 0, 0, -1, 0);
            gl.Rotate(0, _angle, 30);
            foreach (Etage e in _etages)
            {
                gl.PushMatrix();
                gl.Rotate(0, e._angle, 0);
                {
                    gl.PushMatrix();
                    col[0] = couleur.R * e.ratioRa / 256.0f * COL_COLOR[0] ;
                    col[1] = couleur.G * e.ratioGa / 256.0f * COL_COLOR[1];
                    col[2] = couleur.B * e.ratioBa / 256.0f * COL_COLOR[2];
                    gl.Color(col);
                    gl.Translate(0, e.y, -LONGUEUR_RAYON / 2);
                    _sphere.PushObjectSpace(gl);
                    _sphere.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                    _sphere.PopObjectSpace(gl);
                    gl.PopMatrix();
                }

                
                {
                    gl.PushMatrix();
                    col[0] = couleur.R * e.ratioRb / 256.0f * COL_COLOR[0];
                    col[1] = couleur.G * e.ratioGb / 256.0f * COL_COLOR[1];
                    col[2] = couleur.B * e.ratioBb / 256.0f * COL_COLOR[2];
                    gl.Color(col);
                    gl.Translate(0, e.y, LONGUEUR_RAYON / 2);
                    _sphere.PushObjectSpace(gl);
                    _sphere.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                    _sphere.PopObjectSpace(gl);
                    gl.PopMatrix();
                }

                {
                    gl.PushMatrix();
                    col[0] = couleur.R  / 256.0f * COL_COLOR[0];
                    col[1] = couleur.G  / 256.0f * COL_COLOR[1];
                    col[2] = couleur.B  / 256.0f * COL_COLOR[2];
                    gl.Color(col);
                    gl.Translate(0, e.y, -LONGUEUR_RAYON / 2);
                    _cylindre.PushObjectSpace(gl);
                    _cylindre.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                    _cylindre.PopObjectSpace(gl);
                    gl.PopMatrix();
                }
                gl.PopMatrix();
            }

            fillConsole(gl);
        }

        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
            _angle += 2.5f * maintenant._intervalle;

            foreach( Etage e in _etages)
                e.y += maintenant._intervalle * 1.0f;

            Etage et = _etages.First();
            if (et.y > MAX_Y)
            {
                et.y = _etages.Last().y - (MAX_Y - MIN_Y) / (float)NB_ETAGES;
                _etages.RemoveAt(0);
                _etages.Add(et);
            }
        }
    }
}
