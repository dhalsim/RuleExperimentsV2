using HaveBox.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace HaveBox.HaveBoxProxy
{
    internal class ClassProxyFactory
    {
        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly IDictionary<Type, Type> _assemblyMap;

        public ClassProxyFactory()
        {
            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("HaveBoxReducedProxies"), AssemblyBuilderAccess.Run);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule("HaveBoxReducedProxies");
            _assemblyMap = new HashTable<Type, Type>();
        }

        public void CreateProxyType(Type type, out Type proxy)
        {
            Type proxyType;
            if (_assemblyMap.TryGetValue(type, out proxyType))
            {
                proxy = proxyType;
            }
            else
            {
                proxy = CreateProxyType(type);
                _assemblyMap[type] = proxy;
            }                                            
        }

        private Type CreateProxyType(Type type)
        {
            var proxyBuilder = _moduleBuilder.DefineType(type.Name + "Proxy", TypeAttributes.Class | TypeAttributes.Public);
            proxyBuilder.SetParent(type);

            var parameterTypes = type.GetConstructors().First().GetParameters().Select(parameter => parameter.ParameterType);

            var interceptorField = proxyBuilder.DefineField("_interceptor", typeof(IInterceptor), FieldAttributes.Private);
            var proxyConstructor = proxyBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, (new Type[] { typeof(IInterceptor) }.Concat(parameterTypes)).ToArray());
            var proxyConstructorILGenerator = proxyConstructor.GetILGenerator();
            proxyConstructorILGenerator.Emit(OpCodes.Ldarg_0);

            var index = 2;
            parameterTypes.Each(parameterType =>
            {
                proxyConstructorILGenerator.Emit(OpCodes.Ldarg, index);
                index++;
            });

            proxyConstructorILGenerator.Emit(OpCodes.Call, type.GetConstructors().First());
            proxyConstructorILGenerator.Emit(OpCodes.Ldarg_0);
            proxyConstructorILGenerator.Emit(OpCodes.Ldarg_1);
            proxyConstructorILGenerator.Emit(OpCodes.Stfld, interceptorField);
            proxyConstructorILGenerator.Emit(OpCodes.Ret);

            var methods = type.GetMethods().Where(x => x.IsVirtual);
            methods.Each(method =>
            {
                CreateProxyMethod(type, proxyBuilder, interceptorField, method);
            });

            return proxyBuilder.CreateType();
        }

        private void CreateProxyMethod(Type type, TypeBuilder proxyBuilder, FieldBuilder interceptorField, MethodInfo method)
        {
            var invocationproxyConstructor = CreateInvocationTypeAndReturnConstructor(type, proxyBuilder, method);

            var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
            var overrideMethod = proxyBuilder.DefineMethod(method.Name, methodAttributes, method.ReturnType, method.GetParameters().Select(x => x.ParameterType).ToArray());

            var ilGenerator = overrideMethod.GetILGenerator();
            var invocation = ilGenerator.DeclareLocal(typeof(IInvocation));
            if (method.ReturnType == typeof(void))
            {
                CreateReturnVoidProxyMethod(interceptorField, method, invocationproxyConstructor, ilGenerator);
            }
            else
            {
                CreateReturnNonVoidProxeMethod(interceptorField, method, invocationproxyConstructor, ilGenerator, invocation);
            }
        }

        private void CreateReturnNonVoidProxeMethod(FieldBuilder interceptorField, MethodInfo method, ConstructorBuilder invocationproxyConstructor, ILGenerator ilGenerator, LocalBuilder invocation)
        {
            ilGenerator.Emit(OpCodes.Ldarg_0);

            CreateObjectArray(ilGenerator, method);

            ilGenerator.Emit(OpCodes.Newobj, invocationproxyConstructor);
            ilGenerator.Emit(OpCodes.Stloc, invocation);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, interceptorField);
            ilGenerator.Emit(OpCodes.Ldloc, invocation);
            ilGenerator.Emit(OpCodes.Call, typeof(IInterceptor).GetMethod("Intercept", new[] { typeof(IInvocation) }));
            ilGenerator.Emit(OpCodes.Ldloc, invocation);
            ilGenerator.Emit(OpCodes.Call, typeof(IInvocation).GetMethod("get_ReturnObject"));
            if (method.ReturnType.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Unbox_Any, method.ReturnType);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Castclass, method.ReturnType);
            }

            ilGenerator.Emit(OpCodes.Ret);
        }

        private void CreateReturnVoidProxyMethod(FieldBuilder interceptorField, MethodInfo method, ConstructorBuilder invocationproxyConstructor, ILGenerator ilGenerator)
        {
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, interceptorField);
            ilGenerator.Emit(OpCodes.Ldarg_0);

            CreateObjectArray(ilGenerator, method);

            ilGenerator.Emit(OpCodes.Newobj, invocationproxyConstructor);
            ilGenerator.Emit(OpCodes.Call, typeof(IInterceptor).GetMethod("Intercept", new[] { typeof(IInvocation) }));
            ilGenerator.Emit(OpCodes.Ret);
        }

        private void CreateObjectArray(ILGenerator ilGenerator, MethodInfo methodInfo)
        {
            var objectArray = ilGenerator.DeclareLocal(typeof(object[]));
            var parameters = methodInfo.GetParameters();
            var argCounts = parameters.Count();

            ilGenerator.Emit(OpCodes.Ldc_I4, argCounts);
            ilGenerator.Emit(OpCodes.Newarr, typeof(object));
            ilGenerator.Emit(OpCodes.Stloc, objectArray);

            for (int index = 0; index < argCounts; index++)
            {
                ilGenerator.Emit(OpCodes.Ldloc, objectArray);
                ilGenerator.Emit(OpCodes.Ldc_I4, index);
                ilGenerator.Emit(OpCodes.Ldarg, index + 1);

                if (parameters[index].ParameterType.IsValueType)
                {
                    ilGenerator.Emit(OpCodes.Box, parameters[index].ParameterType);
                }


                ilGenerator.Emit(OpCodes.Stelem_Ref);
            }

            ilGenerator.Emit(OpCodes.Ldloc, objectArray);
        }

        private ConstructorBuilder CreateInvocationTypeAndReturnConstructor(Type type, TypeBuilder proxyBuilder, MethodInfo method)
        {
            var invocationBuilder = proxyBuilder.DefineNestedType(method.Name + "ProxyInvocation" + method.GetHashCode(), TypeAttributes.Class | TypeAttributes.NestedPublic);
            invocationBuilder.AddInterfaceImplementation(typeof(IInvocation));
            var invocationProxeeField = invocationBuilder.DefineField("_proxee", type, FieldAttributes.Private);
            var invocationArgObjectsField = invocationBuilder.DefineField("Args", typeof(object[]), FieldAttributes.Public);
            var invocationReturnObjectField = invocationBuilder.DefineField("ReturnObject", typeof(object), FieldAttributes.Private);
            var invocationMethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final;

            var invocationproxyConstructor = CreateInvocationConstructor(type, invocationBuilder, invocationProxeeField, invocationArgObjectsField);

            var returnObjectproperty = invocationBuilder.DefineProperty(invocationReturnObjectField.Name, System.Reflection.PropertyAttributes.HasDefault, typeof(object[]), null);
            CreateGetter(invocationBuilder, invocationReturnObjectField, returnObjectproperty);
            CreateSetter(invocationBuilder, invocationReturnObjectField, returnObjectproperty);

            var property = invocationBuilder.DefineProperty(invocationArgObjectsField.Name, System.Reflection.PropertyAttributes.HasDefault, typeof(object[]), null);
            CreateGetter(invocationBuilder, invocationArgObjectsField, property);
            CreateSetter(invocationBuilder, invocationArgObjectsField, property);

            CreateMethodGetter(invocationBuilder, method, invocationProxeeField);

            CreateProceedMethod(method, invocationBuilder, invocationProxeeField, invocationReturnObjectField, invocationMethodAttributes, invocationArgObjectsField);

            invocationBuilder.CreateType();
            return invocationproxyConstructor;
        }

        private void CreateMethodGetter(TypeBuilder typeBuilder, MethodInfo methodInfo, FieldBuilder fieldBuilder)
        {
            var getSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual;
            var methodInfoField = typeBuilder.DefineField("method", typeof(MethodInfo), FieldAttributes.Private);
            var GetterMethod = typeBuilder.DefineMethod("get_Method", getSetAttr, typeof(MethodInfo), Type.EmptyTypes);
            var property = typeBuilder.DefineProperty(methodInfoField.Name, System.Reflection.PropertyAttributes.HasDefault, typeof(MethodInfo), null);

            var ilGenerator = GetterMethod.GetILGenerator();
            var systemArray = ilGenerator.DeclareLocal(typeof(Type[]));

            ilGenerator.Emit(OpCodes.Ldtoken, fieldBuilder.FieldType);
            ilGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", new[] { typeof(RuntimeTypeHandle) }));

            ilGenerator.Emit(OpCodes.Ldstr, methodInfo.Name);

            var args = methodInfo.GetParameters();
            ilGenerator.Emit(OpCodes.Ldc_I4, args.Count());
            ilGenerator.Emit(OpCodes.Newarr, typeof(Type));
            ilGenerator.Emit(OpCodes.Stloc, systemArray);

            var index = 0;
            args.Each(arg =>
            {
                ilGenerator.Emit(OpCodes.Ldloc, systemArray);
                ilGenerator.Emit(OpCodes.Ldc_I4, index);
                ilGenerator.Emit(OpCodes.Ldtoken, arg.ParameterType);
                ilGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", new[] { typeof(RuntimeTypeHandle) }));
                ilGenerator.Emit(OpCodes.Stelem_Ref);
                index++;
            });

            ilGenerator.Emit(OpCodes.Ldloc, systemArray);
            ilGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetMethod", new[] { typeof(string), typeof(Type[]) }));
            ilGenerator.Emit(OpCodes.Ret);

            property.SetGetMethod(GetterMethod);
        }

        private void CreateGetter(TypeBuilder typeBuilder, FieldBuilder fieldBuilder, PropertyBuilder propertyBuilder)
        {
            var getSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual;
            var GetterMethod = typeBuilder.DefineMethod("get_" + fieldBuilder.Name, getSetAttr, fieldBuilder.FieldType, Type.EmptyTypes);

            var ilGenerator = GetterMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            ilGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(GetterMethod);
        }

        private void CreateSetter(TypeBuilder typeBuilder, FieldBuilder fieldBuilder, PropertyBuilder propertyBuilder)
        {
            var getSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual;
            var GetterMethod = typeBuilder.DefineMethod("set_" + fieldBuilder.Name, getSetAttr, null, new Type[] { fieldBuilder.FieldType });

            var ilGenerator = GetterMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            ilGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetSetMethod(GetterMethod);
        }

        private void CreateProceedMethod(MethodInfo method, TypeBuilder invocationBuilder, FieldBuilder invocationProxeeField, FieldBuilder invocationReturnObjectField, MethodAttributes invocationMethodAttributes, FieldBuilder invocationArgObjectsField)
        {
            var proceedMethod = invocationBuilder.DefineMethod("Proceed", invocationMethodAttributes, typeof(void), new Type[] { });
            var proceedMehtodIl = proceedMethod.GetILGenerator();

            if (method.ReturnType == typeof(void))
            {
                CreateCallProxeeMethod(method, invocationProxeeField, invocationArgObjectsField, proceedMehtodIl);
            }
            else
            {
                proceedMehtodIl.Emit(OpCodes.Ldarg_0);
                CreateCallProxeeMethod(method, invocationProxeeField, invocationArgObjectsField, proceedMehtodIl);
                if (method.ReturnType.IsValueType)
                {
                    proceedMehtodIl.Emit(OpCodes.Box, method.ReturnType);
                }
                proceedMehtodIl.Emit(OpCodes.Stfld, invocationReturnObjectField);
            }

            proceedMehtodIl.Emit(OpCodes.Ret);
        }

        private void CreateCallProxeeMethod(MethodInfo method, FieldBuilder invocationProxeeField, FieldBuilder invocationArgObjectsField, ILGenerator proceedMehtodIl)
        {
            proceedMehtodIl.Emit(OpCodes.Ldarg_0);
            proceedMehtodIl.Emit(OpCodes.Ldfld, invocationProxeeField);
            CreatePassingLogicForProceedMethods(method, proceedMehtodIl, invocationArgObjectsField);
            proceedMehtodIl.Emit(OpCodes.Call, method);
        }

        private void CreatePassingLogicForProceedMethods(MethodInfo method, ILGenerator ilGenerator, FieldBuilder invocationArgObjectsField)
        {
            var medthodArgs = method.GetParameters();
            int index = 0;
            medthodArgs.Each(arg => {
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, invocationArgObjectsField);
                ilGenerator.Emit(OpCodes.Ldc_I4, index);
                ilGenerator.Emit(OpCodes.Ldelem_Ref);

                if (arg.ParameterType.IsValueType)
                {
                    ilGenerator.Emit(OpCodes.Unbox_Any, arg.ParameterType);
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Castclass, arg.ParameterType);
                }

                index++;
            });
        }

        private ConstructorBuilder CreateInvocationConstructor(Type type, TypeBuilder invocationBuilder, FieldBuilder invocationProxeeField, FieldBuilder invocationArgObjectsField)
        {
            var invocationproxyConstructor = invocationBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { type, typeof(object[]) });
            var invocationConstructorILGenerator = invocationproxyConstructor.GetILGenerator();
            invocationConstructorILGenerator.Emit(OpCodes.Ldarg_0);
            invocationConstructorILGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            invocationConstructorILGenerator.Emit(OpCodes.Ldarg_0);
            invocationConstructorILGenerator.Emit(OpCodes.Ldarg_1);
            invocationConstructorILGenerator.Emit(OpCodes.Stfld, invocationProxeeField);
            invocationConstructorILGenerator.Emit(OpCodes.Ldarg_0);
            invocationConstructorILGenerator.Emit(OpCodes.Ldarg_2);
            invocationConstructorILGenerator.Emit(OpCodes.Stfld, invocationArgObjectsField);
            invocationConstructorILGenerator.Emit(OpCodes.Ret);
            return invocationproxyConstructor;
        }
    }
}
