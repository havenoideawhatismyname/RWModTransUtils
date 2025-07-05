using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu;
using Menu.Remix;
using Menu.Remix.MixedUI;
using RWCustom;

namespace RWModTransUtils
{
    public class AddInGameTanslator
    {
        public static void Hook()
        {
            // 常见的remix menu套件
            On.Menu.Remix.MixedUI.OpTab.ctor += OpTab_ctor;
            On.Menu.Remix.MixedUI.OpLabel.ctor_float_float_string_bool += OpLabel_ctor_float_float_string_bool;
            On.Menu.Remix.MixedUI.OpLabel.ctor_Vector2_Vector2_string_FLabelAlignment_bool_FTextParams += OpLabel_ctor_Vector2_Vector2_string_FLabelAlignment_bool_FTextParams;
            On.ConfigurableBase.ctor += ConfigurableBase_ctor;
            On.ConfigurableInfo.ctor += ConfigurableInfo_ctor;
            // 迭代器对话
            On.Conversation.TextEvent.ctor += TextEvent_ctor;
        }

        public static string Translate(string text)
        {
            string text2 = Custom.rainWorld.inGameTranslator.Translate(text);
            if (string.IsNullOrEmpty(text2) || text2 == "!NO TRANSLATION!")
            {
                return text;
            }
            return text2;
        }

        private static void OpLabel_ctor_Vector2_Vector2_string_FLabelAlignment_bool_FTextParams(On.Menu.Remix.MixedUI.OpLabel.orig_ctor_Vector2_Vector2_string_FLabelAlignment_bool_FTextParams orig, OpLabel self, UnityEngine.Vector2 pos, UnityEngine.Vector2 size, string text, FLabelAlignment alignment, bool bigText, FTextParams textParams)
        {
            string newText = Translate(text);
            orig.Invoke(self,pos,size,newText,alignment,bigText,textParams);
        }

        private static void OpLabel_ctor_float_float_string_bool(On.Menu.Remix.MixedUI.OpLabel.orig_ctor_float_float_string_bool orig, OpLabel self, float posX, float posY, string text, bool bigText)
        {
            string newText = Translate(text);
            orig.Invoke(self, posX, posY, newText, bigText);
        }

        private static void OpTab_ctor(On.Menu.Remix.MixedUI.OpTab.orig_ctor orig, OpTab self, OptionInterface owner, string name)
        {
            string newName = Translate(name);
            orig.Invoke(self, owner, newName);
        }

        private static void ConfigurableInfo_ctor(On.ConfigurableInfo.orig_ctor orig, ConfigurableInfo self, string description, ConfigAcceptableBase acceptable, string autoTab, object[] tags)
        {
            string newDescription = Translate(description);
            orig.Invoke(self, newDescription, acceptable, autoTab, tags);
        }

        private static void ConfigurableBase_ctor(On.ConfigurableBase.orig_ctor orig, ConfigurableBase self, OptionInterface OI, string key, Type settingType, string defaultValue, ConfigurableInfo info)
        {
            string newKey = Translate(key);
            orig.Invoke(self, OI, newKey, settingType, defaultValue, info);
        }

        private static void TextEvent_ctor(On.Conversation.TextEvent.orig_ctor orig, Conversation.TextEvent self, Conversation owner, int initialWait, string text, int textLinger)
        {
            string newText = Translate(text);
            orig.Invoke(self, owner, initialWait, newText, textLinger);
        }
    }
}
