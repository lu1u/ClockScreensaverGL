using System;
using System.Drawing;
using SharpGL;
using SharpGL.SceneGraph.Assets;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds
{
    public class LifeCube : Life
    {
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

        const int LARGEUR_TEXTURE = 1024;
        const int HAUTEUR_TEXTURE = 1024;
        uint texture = 0;
        float angle = 0;

        public LifeCube(OpenGL gl) : base(gl)
        {
            texture = EmptyTexture(gl);


        }
        /// <summary>
        /// Create an empty texture.
        /// </summary>
        /// <returns></returns>
        protected uint EmptyTexture(OpenGL gl)
        {
            uint[] txtnumber = new uint[1];                     // Texture ID

            // Create Storage Space For Texture Data (128x128x4)
            byte[] data = new byte[((LARGEUR_TEXTURE * HAUTEUR_TEXTURE) * 4 * sizeof(uint))];

            gl.GenTextures(1, txtnumber);					// Create 1 Texture
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, txtnumber[0]);			// Bind The Texture
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, 4, 128, 128, 0,
                OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, data);			// Build Texture Using Information In data
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);

            return txtnumber[0];						// Return The Texture ID
        }
        public void RenderToTexture(OpenGL gl, Temps maintenant, Rectangle tailleEcran)
        {
            Rectangle r = new Rectangle(0,0,LARGEUR_TEXTURE, HAUTEUR_TEXTURE );
            gl.Viewport(0, 0, r.Width, r.Height);                    // Set Our Viewport (Match Texture Size)

            renderText( gl,  maintenant, r);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture);          // Bind To The Blur Texture

            // Copy Our ViewPort To The Blur Texture (From 0,0 To 128,128... No Border)
            gl.CopyTexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB16, 0, 0, LARGEUR_TEXTURE, HAUTEUR_TEXTURE, 0);

            
            gl.Viewport(0, 0, tailleEcran.Width, tailleEcran.Height);
        }

        private void renderText(OpenGL gl, Temps maintenant, Rectangle rect)
        {
            //gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            //gl.ClearColor(1f, 0.5f, 0.5f, 1.0f);
            gl.LoadIdentity();
            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();

            gl.LoadIdentity();
            gl.Ortho2D(0, LARGEUR, 0, HAUTEUR);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Disable(OpenGL.GL_DEPTH_TEST);
            gl.Disable(OpenGL.GL_ALPHA_TEST);
            gl.Disable(OpenGL.GL_BLEND);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
           
            gl.Color(1.0f, 1.0f, 0);
             /*gl.TexCoord(0.0f, 0.0f); gl.Vertex(-1, 1);
            gl.TexCoord(0.0f, 1.0f); gl.Vertex(-1, -1);
            gl.TexCoord(1.0f, 1.0f); gl.Vertex( 1, -1);
            gl.TexCoord(1.0f, 0.0f); gl.Vertex( 1, 1);
            */
            Color Naissance = getCouleurOpaqueAvecAlpha(Color.White, 70);
            byte[] cNaissance = { Naissance.R, Naissance.G, Naissance.B };
            Color Normal = getCouleurOpaqueAvecAlpha(Color.White, 150);
            byte[] cNormal = { Normal.R, Normal.G, Normal.B };
           
            byte ancienType = MORT;

            textureCellule.Bind(gl);
            float largeurCellule = rect.Width / LARGEUR;
            float hauteurCellule = rect.Height / HAUTEUR;
            
            gl.Begin(OpenGL.GL_QUADS);
            for (int x = 0; x < LARGEUR; x++)
                for (int y = 0; y < HAUTEUR; y++)
                {
                    if (cellules[x, y] != MORT)
                    {
                        if (cellules[x, y] != ancienType)
                        {
                            if (cellules[x, y] == NAISSANCE)
                                gl.Color(cNaissance[0], cNaissance[1], cNaissance[2]);
                            else
                                gl.Color(cNormal[0], cNormal[1], cNormal[2]);
                            ancienType = cellules[x, y];
                        }

                        gl.TexCoord(0.0f, 0.0f); gl.Vertex(x , y + 1);
                        gl.TexCoord(0.0f, 1.0f); gl.Vertex(x, y);
                        gl.TexCoord(1.0f, 1.0f); gl.Vertex(x + 1, y);
                        gl.TexCoord(1.0f, 0.0f); gl.Vertex(x + 1, y + 1);
                    }

                }
            gl.End();
           
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();
        }
        protected void setGlobalMaterial(OpenGL gl, Color couleur)
        {
            // Lumiere
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, LIGHTPOS);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, LIG_SPECULAR);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, LIG_AMBIENT);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, LIG_DIFFUSE);

            // Aspect de la surface
            gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            //gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT | OpenGL.GL_DIFFUSE | OpenGL.GL_SPECULAR);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SPECULAR, COL_SPECULAR);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, COL_AMBIENT);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, COL_DIFFUSE);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SHININESS, SHININESS);

            gl.Color(COL_COLOR[0] * (float)couleur.R / 256.0f, COL_COLOR[1] * (float)couleur.G / 256.0f, COL_COLOR[2] * (float)couleur.B / 256.0f);
        }

        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            RenderToTexture(gl, maintenant, tailleEcran);
            float[] bcol = { couleur.R / 1024.0f, couleur.G / 1024.0f, couleur.B / 1024.0f, 1 };
            gl.ClearColor(bcol[0], bcol[1], bcol[2], bcol[3]);				// Set The Clear Color To Medium Blue
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);      // Clear The Screen And Depth Buffer

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.ClearColor(0, 0, 0, 1);
            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_DEPTH);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            setGlobalMaterial(gl, couleur);

            gl.LookAt(0, 2f, 2, 0, 0, 0, 0, 1, 0);
            // Disable AutoTexture Coordinates
           // gl.Disable(OpenGL.GL_TEXTURE_GEN_S);
            //gl.Disable(OpenGL.GL_TEXTURE_GEN_T);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture);

            float[] col = { couleur.R / 256.0f, couleur.G / 256.0f, couleur.B / 256.0f, 1 };
            gl.Color(col);
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

            Console.getInstance(gl).AddLigne(Color.Green, "Largeur " + LARGEUR + "x Hauteur " + HAUTEUR);
#if TRACER
            RenderStop(CHRONO_TYPE.RENDER);
#endif
        }

        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {
            base.Deplace(maintenant, tailleEcran);
            angle += 2.5f * maintenant._intervalle;
        }
    }
}
