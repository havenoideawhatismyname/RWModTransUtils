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
            //On.Menu.Remix.MixedUI.OpTab.ctor += OpTab_ctor;
            On.Menu.Remix.MixedUI.OpLabel.ctor_float_float_string_bool += OpLabel_ctor_float_float_string_bool;
            On.Menu.Remix.MixedUI.OpLabel.ctor_Vector2_Vector2_string_FLabelAlignment_bool_FTextParams += OpLabel_ctor_Vector2_Vector2_string_FLabelAlignment_bool_FTextParams;
            //On.ConfigurableBase.ctor += ConfigurableBase_ctor;
            //On.ConfigurableInfo.ctor += ConfigurableInfo_ctor;
            // 全是这些东西……
            On.Menu.Remix.MixedUI.UIelement.DisplayDescription += UIelement_DisplayDescription;
            On.Menu.Remix.MixedUI.OpCheckBox.DisplayDescription += OpCheckBox_DisplayDescription;
            On.Menu.Remix.MixedUI.OpTextBox.DisplayDescription += OpTextBox_DisplayDescription;
            On.Menu.Remix.MixedUI.OpKeyBinder.DisplayDescription += OpKeyBinder_DisplayDescription;
            On.Menu.Remix.MixedUI.OpFloatSlider.DisplayDescription += OpFloatSlider_DisplayDescription;
            On.Menu.Remix.MixedUI.OpColorPicker.DisplayDescription += OpColorPicker_DisplayDescription;
            On.Menu.Remix.MixedUI.OpComboBox.DisplayDescription += OpComboBox_DisplayDescription;
            On.Menu.Remix.MixedUI.OpRadioButton.DisplayDescription += OpRadioButton_DisplayDescription;
            On.Menu.Remix.MixedUI.OpSimpleButton.DisplayDescription += OpSimpleButton_DisplayDescription;
            On.Menu.Remix.MixedUI.OpSlider.DisplayDescription += OpSlider_DisplayDescription;
            On.Menu.Remix.MixedUI.OpDragger.DisplayDescription += OpDragger_DisplayDescription;
            On.Menu.Remix.MixedUI.OpHoldButton.DisplayDescription += OpHoldButton_DisplayDescription;
            On.Menu.Remix.MixedUI.OpUpdown.DisplayDescription += OpUpdown_DisplayDescription;
            On.Menu.Remix.MixedUI.OpListBox.DisplayDescription += OpListBox_DisplayDescription;
            // 迭代器对话
            On.Conversation.TextEvent.ctor += TextEvent_ctor;
            // HUD
            On.HUD.TextPrompt.AddMessage_string_int_int_bool_bool += TextPrompt_AddMessage_string_int_int_bool_bool;
            On.HUD.TextPrompt.AddMessage_string_int_int_bool_bool_float_List1 += TextPrompt_AddMessage_string_int_int_bool_bool_float_List1;
        }

        private static void TextPrompt_AddMessage_string_int_int_bool_bool_float_List1(On.HUD.TextPrompt.orig_AddMessage_string_int_int_bool_bool_float_List1 orig, HUD.TextPrompt self, string text, int wait, int time, bool darken, bool hideHud, float iconsX, List<MultiplayerUnlocks.SandboxUnlockID> iconIDs)
        {
            string newText = Translate(text);
            orig.Invoke(self, newText, wait, time, darken, hideHud, iconsX, iconIDs);
        }

        private static void TextPrompt_AddMessage_string_int_int_bool_bool(On.HUD.TextPrompt.orig_AddMessage_string_int_int_bool_bool orig, HUD.TextPrompt self, string text, int wait, int time, bool darken, bool hideHud)
        {
            string newText = Translate(text);
            orig.Invoke(self, newText, wait, time, darken, hideHud);
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

        #region 控件描述
        private static string UIelement_DisplayDescription(On.Menu.Remix.MixedUI.UIelement.orig_DisplayDescription orig, UIelement self)
        {
            if (!string.IsNullOrEmpty(self.description))
            {
                string newDescription = Translate(self.description);
                return newDescription;
            }
            else
            {
                return orig.Invoke(self);
            }
        }

        private static string OpCheckBox_DisplayDescription(On.Menu.Remix.MixedUI.OpCheckBox.orig_DisplayDescription orig, OpCheckBox self)
        {
            if (!string.IsNullOrEmpty(self.description))
            {
                string newDescription = Translate(self.description);
                return newDescription;
            }
            else
            {
                return orig.Invoke(self);
            }                        
        }

        private static string OpTextBox_DisplayDescription(On.Menu.Remix.MixedUI.OpTextBox.orig_DisplayDescription orig, OpTextBox self)
        {
            if (!string.IsNullOrEmpty(self.description))
            {
                string newDescription = Translate(self.description);
                return newDescription;
            }
            else
            {
                return orig.Invoke(self);
            }
        }

        private static string OpKeyBinder_DisplayDescription(On.Menu.Remix.MixedUI.OpKeyBinder.orig_DisplayDescription orig, OpKeyBinder self)
        {
            if (!string.IsNullOrEmpty(self.description))
            {
                string newDescription = Translate(self.description);
                return newDescription;
            }
            else
            {
                return orig.Invoke(self);
            }
        }

        private static string OpFloatSlider_DisplayDescription(On.Menu.Remix.MixedUI.OpFloatSlider.orig_DisplayDescription orig, OpFloatSlider self)
        {
            if (!string.IsNullOrEmpty(self.description))
            {
                string newDescription = Translate(self.description);
                return newDescription;
            }
            else
            {
                return orig.Invoke(self);
            }
        }


        private static string OpListBox_DisplayDescription(On.Menu.Remix.MixedUI.OpListBox.orig_DisplayDescription orig, OpListBox self)
        {
            if (!string.IsNullOrEmpty(self.description))
            {
                string newDescription = Translate(self.description);
                return newDescription;
            }
            else
            {
                return orig.Invoke(self);
            }
        }

        private static string OpUpdown_DisplayDescription(On.Menu.Remix.MixedUI.OpUpdown.orig_DisplayDescription orig, OpUpdown self)
        {
            if (!string.IsNullOrEmpty(self.description))
            {
                string newDescription = Translate(self.description);
                return newDescription;
            }
            else
            {
                return orig.Invoke(self);
            }
        }

        private static string OpHoldButton_DisplayDescription(On.Menu.Remix.MixedUI.OpHoldButton.orig_DisplayDescription orig, OpHoldButton self)
        {
            if (!string.IsNullOrEmpty(self.description))
            {
                string newDescription = Translate(self.description);
                return newDescription;
            }
            else
            {
                return orig.Invoke(self);
            }
        }

        private static string OpDragger_DisplayDescription(On.Menu.Remix.MixedUI.OpDragger.orig_DisplayDescription orig, OpDragger self)
        {
            if (!string.IsNullOrEmpty(self.description))
            {
                string newDescription = Translate(self.description);
                return newDescription;
            }
            else
            {
                return orig.Invoke(self);
            }
        }

        private static string OpSlider_DisplayDescription(On.Menu.Remix.MixedUI.OpSlider.orig_DisplayDescription orig, OpSlider self)
        {
            if (!string.IsNullOrEmpty(self.description))
            {
                string newDescription = Translate(self.description);
                return newDescription;
            }
            else
            {
                return orig.Invoke(self);
            }
        }

        private static string OpSimpleButton_DisplayDescription(On.Menu.Remix.MixedUI.OpSimpleButton.orig_DisplayDescription orig, OpSimpleButton self)
        {
            if (!string.IsNullOrEmpty(self.description))
            {
                string newDescription = Translate(self.description);
                return newDescription;
            }
            else
            {
                return orig.Invoke(self);
            }
        }

        private static string OpRadioButton_DisplayDescription(On.Menu.Remix.MixedUI.OpRadioButton.orig_DisplayDescription orig, OpRadioButton self)
        {
            if (!string.IsNullOrEmpty(self.description))
            {
                string newDescription = Translate(self.description);
                return newDescription;
            }
            else
            {
                return orig.Invoke(self);
            }
        }

        private static string OpComboBox_DisplayDescription(On.Menu.Remix.MixedUI.OpComboBox.orig_DisplayDescription orig, OpComboBox self)
        {
            if (!string.IsNullOrEmpty(self.description))
            {
                string newDescription = Translate(self.description);
                return newDescription;
            }
            else
            {
                return orig.Invoke(self);
            }
        }

        private static string OpColorPicker_DisplayDescription(On.Menu.Remix.MixedUI.OpColorPicker.orig_DisplayDescription orig, OpColorPicker self)
        {
            if (!string.IsNullOrEmpty(self.description))
            {
                string newDescription = Translate(self.description);
                return newDescription;
            }
            else
            {
                return orig.Invoke(self);
            }
        }
        #endregion
    }
}
