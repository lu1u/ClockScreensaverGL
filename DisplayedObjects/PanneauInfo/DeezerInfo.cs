﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ClockScreenSaverGL.DisplayedObjects.Meteo
{
    class DeezerInfo : IDisposable
    {
        private bool _hasNewInfo;
        private DateTime _derniereMaj = DateTime.Now;
        private String _infos = "-";
        private Process _process;
        String _exeName;
        int _delai;

        public string Infos
        {
            get
            {
                return _infos;
            }

        }

        public DeezerInfo(String exeNaame, int delai)
        {
            _exeName = exeNaame;
            _hasNewInfo = false;
            _delai = delai;
            LitFichier();
        }

        internal bool MustRefresh(Temps maintenant)
        {
            return maintenant._temps.Subtract(_derniereMaj).TotalSeconds > _delai;
        }

        internal void Refresh()
        {
            LitFichier();
        }

        private void LitFichier()
        {
            new Thread(new ThreadStart(ChargeDonnees)).Start();
        }

        private void ChargeDonnees()
        {
            if (!isDeezerInfoRunning())
                if (!LanceDeezerInfo())
                    return;

            try
            {
                String folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                String filename = Path.Combine(folder, "deezerinfo.txt");

                String temp = File.ReadAllText(filename);
                temp = temp.Replace(" - ", "\n");

                if (!temp.Equals(_infos))
                {
                    _infos = temp;
                    _hasNewInfo = true;
                    _derniereMaj = DateTime.Now;
                }
            }
            catch (Exception e)
            {
                _infos = e.Message;
                _hasNewInfo = true;
                _derniereMaj = DateTime.Now;
            }

            _process = null;
        }

        private bool LanceDeezerInfo()
        {
            try
            {
                _process = Process.Start(_exeName);
                return true;
            }
            catch (Exception e)
            {
                _infos = e.Message;
                _hasNewInfo = true;
                _derniereMaj = DateTime.Now;
                return false;
            }
        }

        /// <summary>
        /// Retourne vrai si le processus GetDeezerInformation est lance
        /// </summary>
        /// <returns></returns>
        private bool isDeezerInfoRunning()
        {
            String pName = Path.GetFileNameWithoutExtension(_exeName);
            Process[] byName = Process.GetProcessesByName(pName);
            if (byName == null)
                return false;

            if (byName.Length == 0)
                return false;

            return true;
        }

        internal bool HasNewInfo()
        {
            if (_hasNewInfo)
            {
                _hasNewInfo = false;
                return true;
            }
            else
                return false;
        }


        /// <summary>
        /// Tuer le process qu'on a lance
        /// </summary>
        public void Dispose()
        {
            if (_process != null)
            {
                if (PanneauInfos.EXE_KILL)
                    if (!_process.HasExited)
                        _process.Kill();
            }
        }
    }
}
