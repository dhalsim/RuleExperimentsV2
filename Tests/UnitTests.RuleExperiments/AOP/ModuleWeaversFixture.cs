using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using NUnit.Framework;
using Weavers;

namespace UnitTests.RuleExperiments.AOP
{
    [TestFixture]
    public class ModuleWeaversFixture
    {
        private ModuleWeaver _moduleWeaver;

        [TestFixtureSetUp]
        public void Setup()
        {
            _moduleWeaver = new ModuleWeaver();
            DefaultAssemblyResolver assemblyResolver = new DefaultAssemblyResolver();

            const string directory = @"C:\Users\BAydek\Documents\Visual Studio 2013\Projects\RuleExperimentsV2\Implementations\Application.RuleExperiments\bin\Debug";
            assemblyResolver.AddSearchDirectory(directory);

            _moduleWeaver.ModuleDefinition = ModuleDefinition.ReadModule(Path.Combine(directory, "Application.RuleExperiments.dll"), new ReaderParameters
            {
                AssemblyResolver = assemblyResolver
            });

            _moduleWeaver.Execute();
        }

        [Test]
        public void Should_find_class_TestClass()
        {
            Assert.IsNotNull(_moduleWeaver);

            var testClass = _moduleWeaver.ModuleDefinition.Types.FirstOrDefault(t => t.Name == "TestClass");
            Assert.IsNotNull(testClass);
        }

        [Test]
        public void Should_find_class_with_method_decorator()
        {
            var types = _moduleWeaver.GetTypesWithDecoratorAttribute();

            Assert.IsNotNull(types);
            Assert.AreEqual(types[0].Name, "TestClass");
        }

        [Test]
        public void Should_find_methods_with_method_decorator()
        {
            var methods = _moduleWeaver.GetMethodsWithDecoratorAttributeOfType();

            Assert.IsNotNull(methods);
            Assert.AreEqual(methods[0].Name, "Test");
        }
    }
}