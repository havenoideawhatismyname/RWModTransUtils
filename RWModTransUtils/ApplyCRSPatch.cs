using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace RWModTransUtils
{
    public class ApplyCRSPatch
    {
        // 感谢实验喵老师贡献的代码AWA
        private static void Modify(MethodBase from, ILContext.Manipulator manipulator)
        {
            new ILHook(from, manipulator).Apply();
        }

        private static Type FindType(string fullName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetType(fullName)).FirstOrDefault(type => type != null);
        }

        public static void Patch()
        {
            var logger = BepInEx.Logging.Logger.CreateLogSource("RWModTransUtils");
            // 1. 定义目标方法的完整类型名和方法名
            const string targetTypeName = "CustomRegions.Collectables.Encryption";
            const string targetMethodName = "EncryptAllCustomPearls";

            // 2. 使用反射查找目标类型
            // 注意：Type.GetType 需要程序集名称。如果 CustomRegions 在一个名为 "CustomRegions.dll" 的文件中，
            // 那么程序集名称通常就是 "CustomRegions"。
            // 一个更健壮的方法是遍历所有已加载的程序集来查找类型。
            var targetType = FindType(targetTypeName);

            if (targetType == null)
            {
                // 如果类型未找到，说明 CustomRegions mod 不存在或未加载
                logger.LogInfo($"Info: Type '{targetTypeName}' not found. Skipping patch. This is normal if CustomRegions mod is not installed.");
                return;
            }

            // 3. 从找到的类型中获取目标方法
            var targetMethod = targetType.GetMethod(targetMethodName, BindingFlags.Public | BindingFlags.Static);

            if (targetMethod == null)
            {
                // 如果方法未找到，说明该方法可能被移除或改名了
                logger.LogInfo($"Info: Method '{targetMethodName}' in type '{targetTypeName}' not found. Skipping patch.");
                return;
            }

            // 4. 如果方法找到了，应用 Harmony 补丁
            try
            {
                Modify(targetMethod, il =>
                {
                    var c = new ILCursor(il)
                    {
                        Index = 0
                    };
                    c.Emit(OpCodes.Ret);
                }
                );
                logger.LogInfo($"Success: Successfully patched '{targetTypeName}.{targetMethodName}' to prevent execution.");
            }
            catch (Exception e)
            {
                logger.LogError($"Error: Failed to patch '{targetTypeName}.{targetMethodName}'. Exception: {e}");
            }
        }
    }
}
