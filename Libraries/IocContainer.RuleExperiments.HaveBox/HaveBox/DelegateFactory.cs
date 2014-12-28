using HaveBox.Collections;
using HaveBox.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using IocContainer.RuleExperiments.HaveBox;

namespace HaveBox
{
    public class DelegateFactory : IDelegateFactory
    {
        public CreateInstance GetCreateInstanceFromLambdaFunction(TypeDetails typeDetailsLocal)
        {
            return (TypeDetails typeDetails, out object output) => output = typeDetailsLocal.LamdaFunction.Invoke();
        }

        public CreateInstance GetCreateInstanceSingleton(TypeDetails typeDetails)
        {
#if SILVERLIGHT
            var method = new DynamicMethod("CreateInstance" + typeDetails.ImplementType.Name, typeof (void), new[] {typeof (TypeDetails), typeof (object).MakeByRefType()});
#else
            var method = new DynamicMethod("CreateSinlgetonInstance" + typeDetails.ImplementType.Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(void), new[] { typeof(TypeDetails), typeof(object).MakeByRefType() }, typeof(TypeDetails), true);
#endif
            var ilGenerator = method.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, typeof(TypeDetails).GetField("SingletonObject"));
            ilGenerator.Emit(OpCodes.Stind_Ref);
            ilGenerator.Emit(OpCodes.Ret);
            return (CreateInstance)method.CreateDelegate(typeof(CreateInstance));
        }

        public CreateInstance GetCreateInstanceLazySingleton(TypeDetails typeDetailsLocal, CreateInstance instansiationDelegate)
        {
            return (TypeDetails typeDetails, out object output) =>
                {
                    instansiationDelegate(typeDetails, out output);
                    typeDetails.IsSingleton = true;
                    typeDetails.SingletonObject = output;
                    typeDetails.CreateInstanceDelegate = GetCreateInstanceSingleton(typeDetails);
                };
        }

        public CreateInstance GetCreateInstanceWithInterception(TypeDetails typeDetailsLocal, CreateInstance delegateToBeIncepted)
        {
            var argTypes = typeDetailsLocal.DependenciesTypeDetails.Select(typeDetails => typeDetails.ImplementType);
            var createInterceptedInstance = GetCreateInterceptedInstance(typeDetailsLocal);

            return (TypeDetails typeDetails, out object output) =>
                {
                    var invocation = new Instantiation(typeDetails, delegateToBeIncepted, createInterceptedInstance)
                    {
                        argTypes = argTypes,
                        DependenciesTypeDetails = typeDetails.DependenciesTypeDetails,
                    };

                    typeDetailsLocal.Interceptor.Intercept(invocation);
                    output = invocation.instance;
                };
        }

        public CreateInterceptedInstance GetCreateInterceptedInstance(TypeDetails typeDetails)
        {
#if SILVERLIGHT
            var method = new DynamicMethod("CreateInterceptedInstance" + typeDetails.ImplementType.Name, typeof (void), new[] {typeof (object[]), typeof (object).MakeByRefType()});
#else
            var method = new DynamicMethod("CreateInterceptedInstance" + typeDetails.ImplementType.Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(void), new[] { typeof(object[]), typeof(object).MakeByRefType() }, typeof(TypeDetails), true);
#endif
            var ilGenerator = method.GetILGenerator();
            var parameters = typeDetails.ImplementType.GetConstructors().First().GetParameters();

            ilGenerator.Emit(OpCodes.Ldarg_1);

            var index = 0;
            parameters.Each(x =>
            {
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator = ResolveLdc_i4(ilGenerator, index);
                ilGenerator.Emit(OpCodes.Ldelem_Ref);
                ilGenerator.Emit(OpCodes.Castclass, x.ParameterType);
                index++;
            });

            ilGenerator.Emit(OpCodes.Newobj, typeDetails.ImplementType.GetConstructors().First());
            ilGenerator.Emit(OpCodes.Stind_Ref);
            ilGenerator.Emit(OpCodes.Ret);

            return (CreateInterceptedInstance)method.CreateDelegate(typeof(CreateInterceptedInstance));
        }

        public CreateInstance GetCreateInstanceIEnumerable(TypeDetails typeDetails)
        {
#if SILVERLIGHT
            var method = new DynamicMethod("CreateInstance" + typeDetails.ImplementType.Name, typeof (void), new[] {typeof (TypeDetails), typeof (object).MakeByRefType()});
#else
            var method = new DynamicMethod("CreateInstance" + typeDetails.ImplementType.Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(void), new[] { typeof(TypeDetails), typeof(object).MakeByRefType() }, typeof(TypeDetails), true);
#endif
            var ilGenerator = method.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, typeof(TypeDetails).GetField("TypeSiblings"));
            ilGenerator.Emit(OpCodes.Newobj, typeDetails.ImplementType.GetConstructors().First());
            ilGenerator.Emit(OpCodes.Stind_Ref);
            ilGenerator.Emit(OpCodes.Ret);
            return (CreateInstance)method.CreateDelegate(typeof(CreateInstance));
        }

        public CreateInstance GetCreateInstanceDelegate(TypeDetails typeDetails)
        {
#if SILVERLIGHT
            var method = new DynamicMethod("CreateInstance" + typeDetails.ImplementType.Name, typeof (void), new[] {typeof (TypeDetails), typeof (object).MakeByRefType()});
#else
            var method = new DynamicMethod("CreateInstance" + typeDetails.ImplementType.Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(void), new[] { typeof(TypeDetails), typeof(object).MakeByRefType() }, typeof(TypeDetails), true);
#endif
            var ilGenerator = method.GetILGenerator();
            var parameters = typeDetails.ImplementType.GetAllConstructors().First().GetParameters();

            var instances = new Queue<LocalBuilder>();
            var index = 0;

            LocalBuilder dependenciesDetails = null;
            if (parameters.Any())
            {
                dependenciesDetails = ilGenerator.DeclareLocal(typeof(TypeDetails[]));

                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, typeof(TypeDetails).GetField("DependenciesTypeDetails"));
                ilGenerator.Emit(OpCodes.Stloc, dependenciesDetails);
            }

            parameters.Each(x =>
            {
                var dependencyTypeDetails = typeDetails.DependenciesTypeDetails[index];
                if (dependencyTypeDetails.LamdaFunction != null || dependencyTypeDetails.DependenciesTypeDetails.Any() || dependencyTypeDetails.ImplementType.GetInterfaces().Contains(typeof(ITypeEnumerable)))
                {
                    var arrayElement = ilGenerator.DeclareLocal(typeof(object));
                    var instance = ilGenerator.DeclareLocal(typeof(object));
                    instances.Enqueue(instance);

                    ilGenerator.Emit(OpCodes.Ldloc, dependenciesDetails);
                    ilGenerator = ResolveLdc_i4(ilGenerator, index);
                    ilGenerator.Emit(OpCodes.Ldelem_Ref);
                    ilGenerator.Emit(OpCodes.Stloc, arrayElement);
                    ilGenerator.Emit(OpCodes.Ldloc, arrayElement);
                    ilGenerator.Emit(OpCodes.Ldfld, typeof(TypeDetails).GetField("CreateInstanceDelegate"));
                    ilGenerator.Emit(OpCodes.Ldloc, arrayElement);
                    ilGenerator.Emit(OpCodes.Ldloca_S, instance);
                    ilGenerator.Emit(OpCodes.Call, typeof(CreateInstance).GetMethod("Invoke", new[] { typeof(TypeDetails), typeof(object).MakeByRefType() }));
                }
                index++;
            });

            ilGenerator.Emit(OpCodes.Ldarg_1);

            index = 0;
            parameters.Each(x =>
            {
                var dependencyTypeDetails = typeDetails.DependenciesTypeDetails[index];

                if (dependencyTypeDetails.IsSingleton && !dependencyTypeDetails.IsLazySingleton)
                {
                    ilGenerator.Emit(OpCodes.Ldloc, dependenciesDetails);
                    ilGenerator = ResolveLdc_i4(ilGenerator, index);
                    ilGenerator.Emit(OpCodes.Ldelem_Ref);
                    ilGenerator.Emit(OpCodes.Ldfld, typeof(TypeDetails).GetField("SingletonObject"));
                }
                else if (dependencyTypeDetails.DependenciesTypeDetails != null && !dependencyTypeDetails.DependenciesTypeDetails.Any())
                {
                    ilGenerator.Emit(OpCodes.Newobj, typeDetails.DependenciesTypeDetails[index].ImplementType.GetConstructors().First());
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Ldloc, instances.Dequeue());
                }
                index++;
            });

            ilGenerator.Emit(OpCodes.Newobj, typeDetails.ImplementType.GetAllConstructors().First());
            ilGenerator.Emit(OpCodes.Stind_Ref);
            ilGenerator.Emit(OpCodes.Ret);
            return (CreateInstance) method.CreateDelegate(typeof (CreateInstance));
        }

        private ILGenerator ResolveLdc_i4(ILGenerator ilGenerator, int index)
        {
            switch (index)
            {
                case 0:
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    ilGenerator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    ilGenerator.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    ilGenerator.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    ilGenerator.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    ilGenerator.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    ilGenerator.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    ilGenerator.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    ilGenerator.Emit(OpCodes.Ldc_I4, index);
                    break;
            }
            return ilGenerator;
        }

        public delegate void CreateInstance(TypeDetails typeDetails, out object output);
        public delegate void CreateInterceptedInstance(object[] instances, out object output);
    }
}
