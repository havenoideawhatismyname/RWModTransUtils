using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace RWModTransUtils
{
	[BepInPlugin("Dreamstars.ModTranslation", "RWModTransUtils", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		public bool inited;
        public static RemixMenu optionsMenuInstance;

        public void OnEnable()
		{
			On.RainWorld.OnModsInit += RainWorld_OnModsInit;
		}

		private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
		{
			orig.Invoke(self);
			if (inited) return;
            AddInGameTanslator.Hook();
			ApplyCRSPatch.Patch();
			RegExModule.Apply();
            optionsMenuInstance = new RemixMenu(this);
            MachineConnector.SetRegisteredOI("Dreamstars.ModTranslation", optionsMenuInstance);
            inited = true;
		}
	}
}