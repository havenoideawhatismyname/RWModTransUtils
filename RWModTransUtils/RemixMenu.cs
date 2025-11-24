using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace RWModTransUtils
{
    public class RemixMenu : OptionInterface
    {
        public static Configurable<bool> disableFLabelIGT;
        public static Configurable<bool> disableAllCustomIGT;
        public static Configurable<bool> disableRegEx;

        public RemixMenu(Plugin rwModTransUtils)
        {
            disableFLabelIGT = config.Bind("RWModTransUtils_Bool_DisableFLabelIGT", true);
            disableAllCustomIGT = config.Bind("RWModTransUtils_Bool_DisableAllCustomIGT", false);
            disableRegEx = config.Bind("RWModTransUtils_Bool_DisableRegEx", false);
        }

        public override void Initialize()
        {
            var opTab = new OpTab(this, Custom.rainWorld.inGameTranslator.Translate("General"));
            Tabs = new[] { opTab };
            OpContainer opTabContainer = new OpContainer(new Vector2(0, 0));
            opTab.AddItems(opTabContainer);
            UIelement[] UIArrayElements = new UIelement[] 
            {
                new OpLabel(new Vector2(150f, 520f), new Vector2(300f, 30f), OptionInterface.Translate("模组翻译设定"), FLabelAlignment.Center, true, null),
                new OpLabel(new Vector2(150f, 460f), new Vector2(300f, 30f), Custom.ReplaceLineDelimeters(OptionInterface.Translate("本mod强制给常见的文本元素添加了inGameTranslator.Translate()方法（以及其它功能）。<LINE>这能解决一些模组的硬编码文本无法被翻译的问题，<LINE>但可能会导致翻译冲突以及让本不该被翻译的文本也被翻译。<LINE>如果遇到这种情况，下面的选项或许可以帮到您？")), FLabelAlignment.Center, false, null),
                new OpCheckBox(disableFLabelIGT, 100, 375f),
                new OpLabel(140, 377f, OptionInterface.Translate("禁用FLabel强制翻译挂钩")),
                new OpLabel(140, 340f, Custom.ReplaceLineDelimeters(OptionInterface.Translate("没有使用翻译方法的Flabel类UI文本将不再被翻译，<LINE>可解决Bingo mode等的菜单文本错位，但少量文本可能不被翻译。"))),
                new OpCheckBox(disableAllCustomIGT, 100, 275f),
                new OpLabel(140, 277f, OptionInterface.Translate("禁用所有强制翻译挂钩")),
                new OpLabel(140, 240f, Custom.ReplaceLineDelimeters(OptionInterface.Translate("一切没有使用翻译方法的文本将不再被翻译，<LINE>提供最大的兼容性，但会导致本模组提供的较多翻译无法显示。"))),
                new OpCheckBox(disableRegEx, 100, 175f),
                new OpLabel(140, 177f, OptionInterface.Translate("禁用正则表达式匹配")),
                new OpLabel(140, 140f, Custom.ReplaceLineDelimeters(OptionInterface.Translate("禁用后将使用基础的翻译匹配，<LINE>如遇到加载时间问题可尝试禁用，但少量文本可能不被翻译。"))),
            };
            opTab.AddItems(UIArrayElements);
        }
    }
}
