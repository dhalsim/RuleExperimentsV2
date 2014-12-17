using HaveBox.Configuration;
using System;
using System.Configuration;
using System.Reflection;
using System.Reflection.Emit;

namespace HaveBox.SubConfigs
{
    public class ConfigInjection : Config
    {
        public ConfigInjection()
        {
            MergeConfig(new SimpleScanner(GetAssembly()));
        }

        private static Assembly GetAssembly()
        {
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("HaveBoxConfigInjection"), AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule("HaveBoxConfigInjection");

            ConfigurationManager.AppSettings.ToIEnumerable().Each(x => CreateType(module, x.Key, x.Value));

            return assembly;
        }

        private static void CreateType(ModuleBuilder module, string keyName, string value)
        {
            var configBuilder = module.DefineType("HaveBoxConfigInjection." + keyName, TypeAttributes.Class | TypeAttributes.Public);
            configBuilder.AddInterfaceImplementation(typeof(IKeyValueSet));
            ConstructorBuilder ctorBuilder = configBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            CreateProperty(configBuilder, "Key", keyName);
            CreateProperty(configBuilder, "Value", value);

            configBuilder.CreateType();
        }

        private static void CreateProperty(TypeBuilder configBuilder, string propertyName, string returnValue)
        {
            var keyProperty = configBuilder.DefineProperty(propertyName, System.Reflection.PropertyAttributes.HasDefault, typeof(string), null);

            ILGenerator ilgen;

            var getSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual;
            var keyPropertyGetMethod = configBuilder.DefineMethod("get_" + propertyName, getSetAttr, typeof(string), Type.EmptyTypes);

            ilgen = keyPropertyGetMethod.GetILGenerator();
            ilgen.Emit(OpCodes.Ldstr, returnValue);
            ilgen.Emit(OpCodes.Ret);

            keyProperty.SetGetMethod(keyPropertyGetMethod);
        }
    }
}
