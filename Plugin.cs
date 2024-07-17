using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using ModelReplacement;
using BepInEx.Configuration;
using System;
using System.Xml.Linq;

namespace ModelReplacement
{
    [BepInPlugin("com.naqibam.godzillamodel", "Godzilla Model", "0.1.0")]
    [BepInDependency("meow.ModelReplacementAPI", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigFile config;
		public static ConfigEntry<float> voiceSensitivity { get; private set; }

		private static void InitConfig()
		{
			voiceSensitivity = config.Bind<float>("Voice Sensitivity For Godzilla's Jaw", "Sensitivity Value", 10f, "Number from 5 to 20");
		}
		private void Awake()
        {
			config = ((BaseUnityPlugin)this).Config;
			InitConfig();
			Assets.PopulateAssets();

         ModelReplacementAPI.RegisterSuitModelReplacement("Godzilla", typeof(MRGOJIRA));
                

            Harmony harmony = new Harmony("com.naqibam.godzillamodel");
            harmony.PatchAll();
            Logger.LogInfo($"Plugin {"com.naqibam.godzillamodel"} is loaded!");
        }
    }
    public static class Assets
    {
        // Replace mbundle with the Asset Bundle Name from your unity project 
        public static string mainAssetBundleName = "godzillamodelsbundle";
        public static AssetBundle MainAssetBundle = null;

        private static string GetAssemblyName() => Assembly.GetExecutingAssembly().GetName().Name.Replace(" ","_");
        public static void PopulateAssets()
        {
            if (MainAssetBundle == null)
            {
                Console.WriteLine(GetAssemblyName() + "." + mainAssetBundleName);
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetAssemblyName() + "." + mainAssetBundleName))
                {
                    MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }

            }
        }
    }

}