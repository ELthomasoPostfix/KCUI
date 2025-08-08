using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MelonLoader;
using MelonLoader.Assertions;
using Newtonsoft.Json;
using UnityEngine;

namespace KCUI
{
    public class KCUI : MelonMod
    {
        public override void OnApplicationStart()
        {
            LoggerInstance.Msg("Initialization started ...");

            // Validate dependencies manually as a form of documentation.
            LoggerInstance.Msg("Checking dependencies ...");
            this.AssertFileExists(this._DEP_NEWTON);
            this.AssertFileExists(this._DEP_SYSDAT);

            // Setup for dumping Kingdom runtime game data to disk.
            if (this._FLAG_DUMP_DATA)
            {
                LoggerInstance.Msg("Initializing dump directory ...");
                Directory.CreateDirectory(this._OUT_PATH);
            }

            // Initialization finished.
            this._initialized = true;
            LoggerInstance.Msg("Initialization finished!");
        }

        public override void OnUpdate()
        {
            // Silently do nothing if mod initialization failed.
            if (!this._initialized) return;

            /* Note: OnSceneWasLoaded is compatible with Unity v5.4+ but we target Unity v5.3.4!
                For MelonLoader OnLevelWasLoaded is deprecated and now subscribed to OnSceneWasLoaded
                instead! */
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (activeScene.name == this._SCENE_MAIN && this._FLAG_DUMP_DATA)
            {
                this.DumpDays();      
                this.DumpWaves();
                // CLeanup.
                this._FLAG_DUMP_DATA = false;
                
                Application.Quit(); // TODO: DELETE, TESTING CODE.
            }
        }

        /* Assert that the given file path dependency exists. */
        public void AssertFileExists(string path)
        {
            LemonAssert.IsTrue(File.Exists(path), $"Missing dependency: {path}");
        }
        
        /* Dump the regular, boss and recovery days lists of the Director in human-readable format. */
        public void DumpDays()
        {
            try
            {
                IEnumerable<Day> days = Managers.director.regularDays.
                    Concat(Managers.director.bossDays).
                    Concat(Managers.director.recoveryDays);
                foreach (Day day in days)
                {
                    string dumpPath = $"{this._OUT_PATH}/day-{day.name}.json";
                    using (StreamWriter writer = new StreamWriter(dumpPath))
                    {
                        writer.Write(JsonConvert.SerializeObject(new DumpDay(day), Formatting.Indented));
                        LoggerInstance.Msg($"Succeeded to dump Day data: {dumpPath}");
                    }
                }
            }
            catch (Exception e)
            {
                LoggerInstance.Msg($"Failed to dump Day data: {e.Message}");                
                throw e;
            }
        }

        /* Dump the different enemy Wave types in human-readable format. */
        public void DumpWaves()
        {
            try
            {
                string RES_PATH_WAVES = "Waves"; // The Waves resource path.
                foreach (Wave wave in Resources.LoadAll<Wave>(RES_PATH_WAVES))
                {
                    string dumpPath = $"{this._OUT_PATH}/wave-{wave.name}.json";
                    using (StreamWriter writer = new StreamWriter(dumpPath))
                    {
                        writer.Write(JsonConvert.SerializeObject(new DumpWave(wave), Formatting.Indented));
                        LoggerInstance.Msg($"Succeeded to dump Wave data: {dumpPath}");
                    }
                }
            }
            catch (Exception e)
            {
                LoggerInstance.Msg($"Failed to dump Wave data: {e.Message}");
                throw e;
            }
        }

        /* String constants. */
        private string _OUT_PATH { get => $"Mods/{nameof(KCUI)}"; } // The path to dump output files to.
        private string _SCENE_MAIN = "main"; // The Unity main scene name.
        private string _DEP_NEWTON = "UserLibs/Newtonsoft.Json.dll"; // Newtonsoft.Json dependency path.
        private string _DEP_SYSDAT = "UserLibs/System.Data.dll"; // System.Data dependency path.
        /* Boolean flags. */
        private bool _initialized = false;
        private bool _FLAG_DUMP_DATA = true;
    }
}