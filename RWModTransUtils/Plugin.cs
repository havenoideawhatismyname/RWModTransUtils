using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;

namespace RWModTransUtils
{
    [BepInPlugin("Dreamstars.ModTranslation", " RWModTransUtils", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public bool inited;
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig.Invoke(self);
            if (!inited)
            {
                AddInGameTanslator.Hook();
                inited = true;
            }
        }
    }
}
