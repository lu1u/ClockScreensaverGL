using ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D.Modificateurs;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.SystemeParticules2D
{
    public class SystemeParticules2D : Fond, IDisposable
    {
        public readonly int NB_MAX_PARTICULES = 50000;
        readonly float SEUIL_ALPHA = 0.005f;
        public const uint PARTICULES_BLEND_NORMAL = OpenGL.GL_ONE_MINUS_SRC_ALPHA;
        public const uint PARTICULES_BLEND_ADDITIVE = OpenGL.GL_ONE;
        public const float MIN_X = -1.2f;
        public const float MAX_X = 1.2f;
        public const float MIN_Y = -1;
        public const float MAX_Y = 1;
        public const float LARGEUR = MAX_X - MIN_X;
        public const float HAUTEUR = MAX_Y - MIN_Y;
        public const float MILIEU_X = MIN_X + LARGEUR / 2.0f;
        public const float MILIEU_Y = MIN_Y + HAUTEUR / 2.0f;
        private static bool afficheDebug = false;

        public enum TYPE_FOND
        {
            FOND_NOIR, FOND_COULEUR
        }

        public enum COULEUR_PARTICULES
        {
            NOIR, BLANC
        }

        private List<Emetteur2D> _listeEmetteurs = new List<Emetteur2D>();
        private List<Modificateur> _listeModificateurs = new List<Modificateur>();
        public List<Texture> _listeTextures = new List<Texture>();
        public Particule2D[] _particules;
        public int _nbParticules;
        public bool _trier = false;
        private uint _attributBlend = PARTICULES_BLEND_NORMAL;
        public TYPE_FOND typeFond = TYPE_FOND.FOND_NOIR;
        public COULEUR_PARTICULES couleurParticules = COULEUR_PARTICULES.BLANC;
        
        public uint AttributBlend
        {
            get
            {
                return _attributBlend;
            }

            set
            {
                _attributBlend = value;
            }
        }

        public bool Trier
        {
            get
            {
                return _trier;
            }

            set
            {
                _trier = value;
            }
        }

        public SystemeParticules2D(OpenGL gl, int NbMaxParticules): base(gl)
        {
            NB_MAX_PARTICULES = NbMaxParticules;
            _particules = new Particule2D[NB_MAX_PARTICULES];
            for (int i = 0; i < NB_MAX_PARTICULES; i++)
                _particules[i] = new Particule2D();

            _nbParticules = 0;
        }

    

        public void AjouteTexture(Bitmap b)
        {
            Texture t = new Texture();
            t.Create(_gl, b);
            _listeTextures.Add(t);
        }

        public void AjouteEmetteur(Emetteur2D e)
        {
            _listeEmetteurs.Add(e);
        }

        public void AjouteModificateur(Modificateur m)
        {
            _listeModificateurs.Add(m);
        }

        /// <summary>
        /// Gestion de tous les deplacements
        /// </summary>
        /// <param name="maintenant"></param>
        /// <param name="tailleEcran"></param>
        public override void Deplace(Temps maintenant, Rectangle tailleEcran)
        {

        }

        /// <summary>
        /// Trouve une particule non utilisee
        /// </summary>
        /// <returns></returns>
        public int FindUnusedParticle()
        {
            if (_nbParticules < NB_MAX_PARTICULES)
            {
                return _nbParticules++;
            }

            for (int i = 0; i < _nbParticules; i++)
            {
                if (!_particules[i].active)
                    return i;
            }

            return 0;
        }
        public override void ClearBackGround(OpenGL gl, Color c)
        {
            if (typeFond == TYPE_FOND.FOND_NOIR)
                gl.ClearColor(0, 0, 0, 1);
            else
                gl.ClearColor(c.R / 512.0f, c.G / 512.0f, c.B / 512.0f, 1);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        }
        public override void AfficheOpenGL(OpenGL gl, Temps maintenant, Rectangle tailleEcran, Color couleur)
        {
#if TRACER
            RenderStart(CHRONO_TYPE.DEPLACE);
#endif
            DeplaceParticules(maintenant, couleur);
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif

#if TRACER
            RenderStart(CHRONO_TYPE.RENDER);
#endif
            
            
            float[] col = new float[4];
            if (couleurParticules == COULEUR_PARTICULES.NOIR)
            {
                col[0] = 0;
                col[1] = 0;
                col[2] = 0;
            }
            else
            {
                col[0] = couleur.R / 256.0f;
                col[1] = couleur.G / 256.0f;
                col[2] = couleur.B / 256.0f;
            }

            col[3] = 1.0f;

            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D(MIN_X, MAX_X, MIN_Y, MAX_Y);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_DEPTH);
            gl.Disable(OpenGL.GL_DEPTH_TEST);
            gl.Disable(OpenGL.GL_ALPHA_TEST);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, _attributBlend);

            if (_listeTextures.Count > 0)
            {
                gl.Enable(OpenGL.GL_TEXTURE_2D);
                int derniereTexture = -1;
                foreach (Particule2D p in _particules)
                {
                    // Particule active?
                    if (p.active && p.alpha > SEUIL_ALPHA)
                        // Particule sur l'ecran ?
                        if ((p.x+p.taille)>MIN_X && (p.x-p.taille)<MAX_X && (p.y+p.taille)>MIN_Y && (p.y-p.taille)<MAX_Y)
                    {
                        if (p._couleurIndividuelle)
                            gl.Color(p._couleur);
                        else
                        {
                            col[3] = p.alpha;
                            gl.Color(col);
                        }

                        if (p.textureIndex != derniereTexture)
                        {
                            _listeTextures[p.textureIndex].Bind(gl);
                            derniereTexture = p.textureIndex;
                        }

                        gl.Begin(OpenGL.GL_QUADS);
                        gl.TexCoord(0.0f, 0.0f); gl.Vertex(p.x - p.taille, p.y + p.taille);
                        gl.TexCoord(0.0f, 1.0f); gl.Vertex(p.x - p.taille, p.y - p.taille);
                        gl.TexCoord(1.0f, 1.0f); gl.Vertex(p.x + p.taille, p.y - p.taille);
                        gl.TexCoord(1.0f, 0.0f); gl.Vertex(p.x + p.taille, p.y + p.taille);
                        gl.End();
                    }
                }
            }
            else
            {
                gl.Disable(OpenGL.GL_TEXTURE_2D);
                foreach (Particule2D p in _particules)
                {
                    if (p.active && p.alpha > SEUIL_ALPHA)
                    {
                        if (p._couleurIndividuelle)
                            gl.Color(p._couleur);
                        else
                        {
                            col[3] = p.alpha;
                            gl.Color(col);
                        }

                        gl.Begin(OpenGL.GL_QUADS);
                        gl.Vertex(p.x - p.taille, p.y + p.taille);
                        gl.Vertex(p.x - p.taille, p.y - p.taille);
                        gl.Vertex(p.x + p.taille, p.y - p.taille);
                        gl.Vertex(p.x + p.taille, p.y + p.taille);
                        gl.End();

                    }
                }

            }

            Console.getInstance(gl).AddLigne(Color.Green, "Max particules " + NB_MAX_PARTICULES);

            if  (afficheDebug)
            {
                gl.Color(1.0f, 1.0f, 1.0f, 0.2f);
                gl.Disable(OpenGL.GL_TEXTURE_2D);

                gl.Rect(MIN_X, MIN_Y, MIN_X + LARGEUR * ((float)_nbParticules / (float)NB_MAX_PARTICULES), MIN_Y + HAUTEUR * 0.005f);

                gl.Rect(MILIEU_X - (LARGEUR / 1000.0f), MIN_Y + (HAUTEUR / 30.0f), MILIEU_X + (LARGEUR / 1000.0f), MIN_Y);
            }

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();
#if TRACER
            RenderStop(CHRONO_TYPE.DEPLACE);
#endif
        }

        private void DeplaceParticules(Temps maintenant, Color couleur)
        {
            _trier = false;
            foreach (Emetteur2D e in _listeEmetteurs)
                e.Deplace(this, maintenant, couleur);

            foreach (Modificateur m in _listeModificateurs)
                m.Applique(this, maintenant);

            if (_trier)
            {
                Array.Sort(_particules, 0, _nbParticules);

                while (_nbParticules > 0 && (_particules[_nbParticules - 1].active == false))
                    _nbParticules--;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (Texture t in _listeTextures)
                t.Destroy(_gl);
        }

        public override bool KeyDown(Form f, Keys k)
        {
            switch (k)
            {
                case TOUCHE_PARTICULES:
                    afficheDebug = !afficheDebug;
                    return true;

                default:
                    return base.KeyDown(f, k);
            }
        }
    }
}
