using System;
using System.Reflection;
using HarmonyLib;
using IPA;
using IPALogger = IPA.Logging.Logger;

namespace BetterPageControll
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        private const string HarmonyId = "com.denpadokei.BetterPageControll";

        private Harmony _harmony;

        internal static IPALogger Log { get; private set; }

        [Init]
        public Plugin(IPALogger logger)
        {
            Log = logger;
            Log?.Debug("Logger initialized.");
        }

        [OnEnable]
        public void OnEnable()
        {
            ApplyHarmonyPatches();
        }

        [OnDisable]
        public void OnDisable()
        {
            RemoveHarmonyPatches();
        }

        private void ApplyHarmonyPatches()
        {
            if (_harmony != null)
            {
                return;
            }

            try
            {
                Log?.Debug("Applying Harmony patches.");
                _harmony = new Harmony(HarmonyId);
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Log?.Error("Error applying Harmony patches: " + ex.Message);
                Log?.Debug(ex);
            }
        }

        private void RemoveHarmonyPatches()
        {
            if (_harmony == null)
            {
                return;
            }

            try
            {
                _harmony.UnpatchSelf();
            }
            catch (Exception ex)
            {
                Log?.Error("Error removing Harmony patches: " + ex.Message);
                Log?.Debug(ex);
            }
            finally
            {
                _harmony = null;
            }
        }
    }
}
