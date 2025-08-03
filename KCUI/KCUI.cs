using System;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KCUI
{
    public class KCUI : MelonMod
    {
        public override void OnApplicationStart()
        {
            if (this._FLAG_DUMP_DATA)
            {
                LoggerInstance.Msg("Initializing dump directory ...");
                Directory.CreateDirectory(this.dumpPath());
            }
        }

        public override void OnUpdate()
        {
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (activeScene.name == this._SCENE_MAIN && this._FLAG_DUMP_DATA)
            {
                /* Note: OnSceneWasLoaded is compatible with Unity v5.4+ but we target Unity v5.3.4!
                    For MelonLoader OnLevelWasLoaded is deprecated and now subscribed to OnSceneWasLoaded
                    instead! */
                this.dumpDays();      
                this.dumpWaves();
                // CLeanup.
                this._FLAG_DUMP_DATA = false;
                
                Application.Quit(); // TODO: DELETE, TESTING CODE.
            }
        }

        /* Get the path this mod dumps non-MelonLoader output files to. */
        public string dumpPath()
        {
            return $"Mods/{nameof(KCUI)}";
        }
        
        /* Dump the regular, boss and recovery days lists of the Director in human-readable format. */
        public void dumpDays()
        {
            try
            {
                string s = "{\n";
                s += JSON.json(Managers.director.regularDays)  + "\n";
                s += JSON.json(Managers.director.bossDays)     + "\n";
                s += JSON.json(Managers.director.recoveryDays) + "\n";
                s += "}";

                string dumpPath = $"{this.dumpPath()}/days.json";
                using (StreamWriter writer = new StreamWriter(dumpPath))
                {
                    writer.Write(s);
                    LoggerInstance.Msg($"Succeeded to dump Day data: {dumpPath}");
                }
            }
            catch (Exception e)
            {
                LoggerInstance.Msg($"Failed to dump Day data: {e.Message}");                
            }
        }

        /* Dump the different enemy Wave types in human-readable format. */
        public void dumpWaves()
        {
            try
            {
                string s = "{\n";
                string RES_PATH_WAVES = "Waves"; // The Waves resource path.
                foreach (Wave wave in Resources.LoadAll<Wave>(RES_PATH_WAVES))
                {
                    s += JSON.json(wave) + ",\n";
                }
                s += "}";

                string dumpPath = $"{this.dumpPath()}/waves.json";
                using (StreamWriter writer = new StreamWriter(dumpPath))
                {
                    writer.Write(s);
                    LoggerInstance.Msg($"Succeeded to dump Wave data: {dumpPath}");
                }
            }
            catch (Exception e)
            {
                LoggerInstance.Msg($"Failed to dump Wave data: {e.Message}");
            }
        }

        /* The name constants of Unity scenes. */
        private string _SCENE_MAIN = "main"; // The main scene name.
        /* Boolean flags of the mod. */
        private bool _FLAG_DUMP_DATA = true;
    }
}