using System.Linq;
using Domain.RuleExperiments.Attributes;
using Domain.RuleExperiments.Exceptions;
using NUnit.Framework;

namespace UnitTests.RuleExperiments.AOP
{
	public class TryCatchWeaverFixture : BaseAOPFixture
	{
		[Test]
		public void Should_find_class_TestClass()
		{
			Assert.IsNotNull(ModuleWeaver);

			var testClass = ModuleWeaver.ModuleDefinition.Types.FirstOrDefault(t => t.Name == "TestClass");
			Assert.IsNotNull(testClass);
		}

		[Test]
		public void Should_find_class_with_method_decorator()
		{
			var types = ModuleWeaver.GetTypesWithDecoratorAttribute<TryCatchDecoratorAttribute>();

			Assert.IsNotNull(types);
			Assert.AreEqual(types[0].Name, "TestClass");
		}

		[Test]
		public void Should_find_methods_with_method_decorator()
		{
            var methods = ModuleWeaver.GetMethodsWithDecoratorAttributeOfType<TryCatchDecoratorAttribute>();

			Assert.IsNotNull(methods);
			Assert.AreEqual(methods[0].Name, "Test");
		}

		[Test]
		public void Should_surround_method_with_try()
		{
            const string exceptionMessage = "System Level Exception Occureed.";

            foreach (var method in ModuleWeaver.GetMethodsWithDecoratorAttributeOfType<TryCatchDecoratorAttribute>())
            {
                string methodName = method.Name;
		        var exceptionHandler = ModuleWeaver.CreateTryCatchBlock<SystemLevelException>(method, exceptionMessage);
                method.Body.ExceptionHandlers.Add(exceptionHandler);
                ModuleWeaver.ModuleDefinition.Import(method);
                var instanceOfMyType = WriteAssemblyAndReadType(method);

                var exception = Assert.Throws<SystemLevelException>(() =>
                {
                    if (methodName == "Test")
                    {
                        instanceOfMyType.Test();
                    }
                    else
                    {
                        instanceOfMyType.Test2();
                    }
                });

		        Assert.AreEqual(exception.Message, exceptionMessage);
		    }
		}
	}
}