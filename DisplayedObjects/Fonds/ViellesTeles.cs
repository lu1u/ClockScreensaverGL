using ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity;
using ClockScreenSaverGL.DisplayedObjects.Fonds.Particules;
using ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD;
using ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD.Opengl;
using ClockScreenSaverGL.DisplayedObjects.Metaballes;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using SharpGL.SceneGraph.Assets;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds
{
    public class ViellesTeles : Fond
    {
        public const String CAT = "ToTexture";
        protected float[] COL_AMBIENT = { 0.21f, 0.12f, 0.05f, 1.0f };
        protected float[] COL_DIFFUSE = { 0.7f, 0.72f, 0.78f, 1.0f };
        protected float[] COL_SPECULAR = { 0.7f, 0.7f, 0.7f, 1.0f };
        protected float[] COL_COLOR = { 0.7f, 0.7f, 0.7f };
        protected float SHININESS = 18f;

        protected float[] LIGHTPOS = { -2, 1.5f, -2.5f, 1 };
        protected float[] LIG_SPECULAR = { 1.0f, 1.0f, 1.0f };
        protected float[] LIG_AMBIENT = { 0.5f, 0.5f, 0.5f };
        protected float[] LIG_DIFFUSE = { 1.0f, 1.0f, 1.0f };
        protected float RATIO_COULEUR = 1.0f / 256.0f;

        const int LARGEUR_TEXTURE = 128;
        const int HAUTEUR_TEXTURE = 128;
        protected uint texture = 0;
        protected DisplayedObject _objet;
        protected Texture tv;
        protected TimerIsole _timer = new TimerIsole(30000);
        private class Quad
        {
            public Vecteur3D position, angle, taille;
        }

        const int NB_QUADS = 10;
        List<Quad> quads = new List<Quad>();

        public ViellesTeles(OpenGL gl) : base(gl)
        {
            _objet = InitObjet(gl);
            texture = EmptyTexture(gl, LARGEUR_TEXTURE, HAUTEUR_TEXTURE);

            Quad quad = new Quad();
            quad.position = new Vecteur3D(5f, 0.8f, -3);
            quad.angle = new Vecteur3D(0, -45, 0);
            quad.taille = new Vecteur3D(2, 1.5f, 1);
            quads.Add(quad);

            quad = new Quad();
            quad.position = new Vecteur3D(-6f, -0.5f, -4);
            quad.angle = new Vecteur3D(0, 35, 0);
            quad.taille = new Vecteur3D(2.5f, 2.5f, 1);
            quads.Add(quad);

            quad = new Quad();
            quad.position = new Vecteur3D(0f, 0f, -6f);
            quad.angle = new Vecteur3D(0, 0, 0);
            quad.taille = new Vecteur3D(1f, 0.75f, 1);
            quads.Add(quad);

            tv = new Texture();
            tv.Create(gl, Config.getImagePath(r.Next(2) == 0? "tv1.png" : "tv2.png"));
        }

        protected DisplayedObject InitObjet(OpenGL gl)
        {
            switch (r.Next(19))
            {
                case 0: return new Neige(gl, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case 1: return new Encre(gl, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case 2: return new Bacteries(gl, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                case 3: return new Life(gl);
                case 4: return new Couronnes(gl);
                case 5: return new EspaceOpenGL(gl);
                case 6: return new Tunnel(gl);
                case 7: return new CarresEspace(gl);
                case 8: return new GravitationParticules(gl);
                case 9: return new NuagesOpenGL(gl);
                case 10: return new TerreOpenGL(gl);
                case 11: return new ParticulesGalaxie(gl);
                case 12: return new ParticulesFusees(gl);
                case 13: return new FeuDArtifice(gl);
                case 14: return new AttracteurParticules(gl);
                case 15: return new Gravitation(gl);
                case 16: return new Engrenages(gl);
                case 17: return new ADN(gl);
                default:
                    return new Metaballes.Metaballes(gl, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
            }
        }

        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
            base.Deplace(maintenant, tailleEcran);
            
            _objet.Deplace(maintenant, tailleEcran);

            //foreach (Quad q in quads)
             //q.position.RotateY(25 * maintenant._intervalle);
        }

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            if (_timer.Ecoule())
                _objet = InitObjet(gl);

            RenderToTexture(gl, maintenant, tailleEcran, couleur);
            //float[] bcol = { couleur.R / 1024.0f, couleur.G / 1024.0f, couleur.B / 1024.0f, 1 };
            // gl.ClearColor(bcol[0], bcol[1], bcol[2], bcol[3]);				// Set The Clear Color To Medium Blue
            gl.ClearColor(0, 0, 0, 0);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);      // Clear The Screen And Depth Buffer

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.ClearColor(0, 0, 0, 1);
            gl.LoadIdentity();
            gl.Enable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            gl.LookAt(0, 0f, 7, 0, 0, 0, 0, 1, 0);
            // Disable AutoTexture Coordinates
            // gl.Disable(OpenGL.GL_TEXTURE_GEN_S);
            //gl.Disable(OpenGL.GL_TEXTURE_GEN_T);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture);

            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1 };
            gl.Color(col);

            foreach (Quad q in quads)
                DessineRectangle(gl, q);

            
            tv.Bind(gl);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            foreach (Quad q in quads)
                DessineRectangleTV(gl, q);

            Console.getInstance(gl).AddLigne(Color.Green, _objet.GetType().Name);

#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

        private void DessineRectangle(OpenGL gl, Quad q)
        {
            gl.PushMatrix();
            gl.Translate(q.position.x, q.position.y, q.position.z);
            gl.Rotate(q.angle.x, q.angle.y, q.angle.z);
            gl.Scale(q.taille.x, q.taille.y, q.taille.z);
            gl.Begin(OpenGL.GL_QUADS);
            // Front Face
            gl.Normal(0.0f, 0.0f, 1.0f);                  // Normal Pointing Towards Viewer
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(-1.0f, -1.0f, 1.0f);  // Point 1 (Front)
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(1.0f, -1.0f, 1.0f);  // Point 2 (Front)
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(1.0f, 1.0f, 1.0f);  // Point 3 (Front)
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(-1.0f, 1.0f, 1.0f);  // Point 4 (Front)
            gl.End();
            gl.PopMatrix();
        }

        private void DessineRectangleTV(OpenGL gl, Quad q)
        {
            gl.PushMatrix();
            gl.Translate(q.position.x, q.position.y, q.position.z + 0.000001f);
            gl.Rotate(q.angle.x, q.angle.y, q.angle.z);
            gl.Scale(q.taille.x * 1.1f, q.taille.y * 1.1f, q.taille.z * 1.1f);
            gl.Begin(OpenGL.GL_QUADS);
            // Front Face
            gl.Normal(0.0f, 0.0f, 1.0f);                  // Normal Pointing Towards Viewer
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(-1.0f, -1.0f, 1.0f);  // Point 1 (Front)
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(1.0f, -1.0f, 1.0f);  // Point 2 (Front)
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(1.0f, 1.0f, 1.0f);  // Point 3 (Front)
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(-1.0f, 1.0f, 1.0f);  // Point 4 (Front)
            gl.End();
            gl.PopMatrix();
        }
        /*
            gl.Rotate(angle, angle, angle);
            gl.Begin(OpenGL.GL_QUADS);
            // Front Face
            gl.Normal(0.0f, 0.0f, 1.0f);                  // Normal Pointing Towards Viewer
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(-1.0f, -1.0f, 1.0f);  // Point 1 (Front)
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(1.0f, -1.0f, 1.0f);  // Point 2 (Front)
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(1.0f, 1.0f, 1.0f);  // Point 3 (Front)
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(-1.0f, 1.0f, 1.0f);  // Point 4 (Front)
                                                                    // Back Face
            gl.Normal(0.0f, 0.0f, -1.0f);                  // Normal Pointing Away From Viewer
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(-1.0f, -1.0f, -1.0f);  // Point 1 (Back)
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(-1.0f, 1.0f, -1.0f);  // Point 2 (Back)
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(1.0f, 1.0f, -1.0f);  // Point 3 (Back)
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(1.0f, -1.0f, -1.0f);  // Point 4 (Back)
                                                                     // Top Face
            gl.Normal(0.0f, 1.0f, 0.0f);                  // Normal Pointing Up
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(-1.0f, 1.0f, -1.0f);  // Point 1 (Top)
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(-1.0f, 1.0f, 1.0f);  // Point 2 (Top)
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(1.0f, 1.0f, 1.0f);  // Point 3 (Top)
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(1.0f, 1.0f, -1.0f);  // Point 4 (Top)
                                                                    // Bottom Face
            gl.Normal(0.0f, -1.0f, 0.0f);                  // Normal Pointing Down
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(-1.0f, -1.0f, -1.0f);  // Point 1 (Bottom)
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(1.0f, -1.0f, -1.0f);  // Point 2 (Bottom)
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(1.0f, -1.0f, 1.0f);  // Point 3 (Bottom)
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(-1.0f, -1.0f, 1.0f);  // Point 4 (Bottom)
                                                                     // Right face
            gl.Normal(1.0f, 0.0f, 0.0f);                  // Normal Pointing Right
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(1.0f, -1.0f, -1.0f);  // Point 1 (Right)
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(1.0f, 1.0f, -1.0f);  // Point 2 (Right)
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(1.0f, 1.0f, 1.0f);  // Point 3 (Right)
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(1.0f, -1.0f, 1.0f);  // Point 4 (Right)
                                                                    // Left Face
            gl.Normal(-1.0f, 0.0f, 0.0f);                  // Normal Pointing Left
            gl.TexCoord(0.0f, 0.0f); gl.Vertex(-1.0f, -1.0f, -1.0f);  // Point 1 (Left)
            gl.TexCoord(1.0f, 0.0f); gl.Vertex(-1.0f, -1.0f, 1.0f);  // Point 2 (Left)
            gl.TexCoord(1.0f, 1.0f); gl.Vertex(-1.0f, 1.0f, 1.0f);  // Point 3 (Left)
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(-1.0f, 1.0f, -1.0f);  // Point 4 (Left)
            gl.End();


        }*/


        public void RenderToTexture(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
            Rectangle r = new Rectangle(0, 0, LARGEUR_TEXTURE, HAUTEUR_TEXTURE);
            gl.Viewport(0, 0, r.Width, r.Height);                    // Set Our Viewport (Match Texture Size)
            gl.PushAttrib(OpenGL.GL_ENABLE_BIT);

            _objet.AfficheOpenGL(gl, maintenant, r, couleur);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture);          // Bind To The Blur Texture

            // Copy Our ViewPort To The Blur Texture (From 0,0 To 128,128... No Border)
            gl.CopyTexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB16, 0, 0, LARGEUR_TEXTURE, HAUTEUR_TEXTURE, 0);

            gl.PopAttrib();

            gl.Viewport(0, 0, tailleEcran.Width, tailleEcran.Height);
        }
    }
}
