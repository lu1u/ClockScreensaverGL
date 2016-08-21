
using ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity;
using ClockScreenSaverGL.DisplayedObjects.Fonds.TroisD;
using SharpGL;
using System;
using System.Drawing;

namespace ClockScreenSaverGL.DisplayedObjects.Fonds.Gravity
{
    class Camera
    {
        //////////////////////////////////////////////////////////////////////
        // Types
        //////////////////////////////////////////////////////////////////////
        public enum T_MODE_CAMERA { T_STATIC, T_AUTOMATIC, T_PLUS_GROS, T_FIXE };

        public static int CorpsSuiviFrom = 0;
        public static int CorpsSuiviTo = 1;
        public static T_MODE_CAMERA ModeCamera = T_MODE_CAMERA.T_STATIC;
        public static Vecteur3D CameraFrom, CameraTo;
        public static TimerIsole ChangementCamera = new TimerIsole(Gravitation.FreqChgtCamera * 1000);
        static Vecteur3D Tourne = new Vecteur3D (1000.0f, 100.0f, 0 )  ;

        //////////////////////////////////////////////////////////////////////
        // Initialisation de la camera
        //////////////////////////////////////////////////////////////////////
        public static void InitCamera()
        {
            switch (ModeCamera)
            {
                case T_MODE_CAMERA.T_STATIC:
                case T_MODE_CAMERA.T_AUTOMATIC:
                case T_MODE_CAMERA.T_FIXE:
                    {
                        CorpsSuiviFrom = 0;
                        ChangeCorpsSuivi(ref CorpsSuiviTo, CorpsSuiviFrom);
                    }
                    break;

                case T_MODE_CAMERA.T_PLUS_GROS:
                    {
                        Gravitation.IndicePlusGros = Gravitation.RecherchePlusGros();
                        CorpsSuiviTo = Gravitation.IndicePlusGros;
                        ChangeCorpsSuivi(ref CorpsSuiviFrom, CorpsSuiviTo);
                    }
                    break;
            }

            CameraFrom = Gravitation.Corps[CorpsSuiviFrom].Position;
            CameraTo = Gravitation.Corps[CorpsSuiviTo].Position;
        }


///////////////////////////////////////////////////////////////////////////////
// Gestion de la camera, avec des changements progressifs pour ne pas brusquer
// l'affichage
///////////////////////////////////////////////////////////////////////////////
public static void PlaceCamera(OpenGL gl, Rectangle tailleEcran, float intervalle)
        {
            // Emplacement de la camera
            // Changer le corps suivi, de temps en temps
            switch (ModeCamera)
            {
                case T_MODE_CAMERA.T_AUTOMATIC:
                    {
                        if (ChangementCamera.Ecoule())
                        {
                            Random r = new Random();
                            if (r.Next(1)!=0)
                            {
                                ChangeCorpsSuivi(ref CorpsSuiviFrom, CorpsSuiviTo);
                            }
                            else
                            {
                                ChangeCorpsSuivi(ref CorpsSuiviTo, CorpsSuiviFrom);
                            }                            
                        }
                        CameraSuitPlanetes(intervalle);
                    }
                    break;

                case T_MODE_CAMERA.T_STATIC:
                    {
                        CameraSuitPlanetes(intervalle);
                    }
                    break;

                case T_MODE_CAMERA.T_PLUS_GROS:
                    {
                        CorpsSuiviTo = Gravitation.IndicePlusGros;
                        if (ChangementCamera.Ecoule())
                        {
                            ChangeCorpsSuivi(ref CorpsSuiviFrom, CorpsSuiviTo);
                        }

                        CameraSuitPlanetes(intervalle);
                    }
                    break;



                case T_MODE_CAMERA.T_FIXE:
                    CameraFixe(intervalle);
                    break;
            }


            // Tous decaler, y compris la camera, vers l'origine
            for (int i = 0; i < Gravitation.NbCorps; i++)
            {
                Gravitation.Corps[i].Position.soustraire(CameraFrom);

                //for (int j = 0; j < Gravitation.Corps[i].NbTrajectoire; j++)
                 //   Gravitation.Corps[i].Trajectoire[j].Moins(CameraFrom);
            }

            /*for (i = 0; i < Gravitation.NbPoussieres; i++)
                Gravitation.Poussieres[i].Position.Moins(CameraFrom);

            for (i = 0; i < Gravitation.NbExplosions; i++)
                Gravitation.Explosions[i].Position.Moins(CameraFrom);*/

            CameraTo.soustraire(CameraFrom);
            CameraFrom = Vecteur3D.NULL;

            C3D.Camera(gl, tailleEcran, CameraFrom / Gravitation.DiviseurDistance, CameraTo / Gravitation.DiviseurDistance, Gravitation.AngleChampVision);
        }
        public static void SwitchCamera()
        {
            switch (ModeCamera)
            {
                case T_MODE_CAMERA.T_STATIC:
                    ModeCamera = T_MODE_CAMERA.T_AUTOMATIC;
                   break;

                case T_MODE_CAMERA.T_AUTOMATIC:
                    ModeCamera = T_MODE_CAMERA.T_PLUS_GROS;
                    break;

                case T_MODE_CAMERA.T_PLUS_GROS:
                    ModeCamera = T_MODE_CAMERA.T_FIXE;
                    break;

                case T_MODE_CAMERA.T_FIXE:
                    ModeCamera = T_MODE_CAMERA.T_STATIC;
                    break;

            }
        }

        public static void CameraFixe(float intervalle)
        {
           
            CameraTo = Gravitation.Corps[CorpsSuiviTo].Position;
            CameraFrom = CameraTo - Tourne;
             float Angle = CMath.VitesseConstante(5.0f, intervalle);
            Tourne.RotateX(Angle);
            Tourne.RotateY(Angle);
            Tourne.RotateZ(Angle);


            float Distance = (CameraFrom - CameraTo).Longueur();
            float NouveauDiviseur = Distance / Gravitation.DistanceCibleAffichage;

            /*Gravitation.DiviseurDistance += (NouveauDiviseur - Gravitation.DiviseurDistance) / 2.0f;
            Gravitation.DiviseurDistance = Math.Min(Gravitation.DiviseurDistance, Gravitation.RAYON_UNIVERS / 2.0f);*/
        }

        public static void CameraSuitPlanetes(float intervalle)
        {
            Vecteur3D To = Gravitation.Corps[CorpsSuiviTo].Position;
            Vecteur3D From = Gravitation.Corps[CorpsSuiviFrom].Position;

            float RayonImage = Planete.planetes[Gravitation.Corps[CorpsSuiviFrom].type].rayon * Gravitation.DistanceCibleAffichage;
            Vecteur3D Decalage = new Vecteur3D(RayonImage, RayonImage, RayonImage) ;

            // Deplacer progressivement le point de vue vers le corps suivi
            CameraFrom += (From - CameraFrom) * CMath.VitesseConstante(1.0f, intervalle );

            // Deplacer progressivement le point de vue vers le corps suivi
            CameraTo += (To - CameraTo) * CMath.VitesseConstante(1.0f, intervalle);

            float Distance = (CameraFrom - CameraTo).Longueur();

            // On s'arrange pour que l'objet vise soit a "distance visible" fixe
            float NouveauDiviseur = Distance / Gravitation.DistanceCibleAffichage;
            Gravitation.DiviseurDistance = (Gravitation.DiviseurDistance * (Gravitation.PonderationDiviseurDistance - 1.0f) + NouveauDiviseur)
                                    / Gravitation.PonderationDiviseurDistance;

            Gravitation.DiviseurDistance = Math.Min(Gravitation.DiviseurDistance, Gravitation.RAYON_UNIVERS);
        }


        // Choisi un corps, different de celui donne en 2eme parametre
        ///////////////////////////////////////////////////////////////////////////////
        public static void ChangeCorpsSuivi(ref int pCorpsAChanger, int Autre)
        {
            {
                if (Gravitation.NbCorps < 2)
                {
                    pCorpsAChanger = 0;
                    return;
                }

                Random r = new Random();
                do
                {
                    (pCorpsAChanger) = r.Next(Gravitation.NbCorps);
                }
                while ((pCorpsAChanger) == Autre);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Change l'emplacement ou se trouve la camera
        ///////////////////////////////////////////////////////////////////////////////
        static void ChangePosition( )
        {
            if (ModeCamera == T_MODE_CAMERA.T_STATIC ||
                    ModeCamera == T_MODE_CAMERA.T_AUTOMATIC ||
                    ModeCamera == T_MODE_CAMERA.T_PLUS_GROS)
            {
                do
                {
                    CorpsSuiviFrom = (CorpsSuiviFrom + 1) % Gravitation.NbCorps;
                }
                while (CorpsSuiviFrom == CorpsSuiviTo);
            }
        }

    }
}
