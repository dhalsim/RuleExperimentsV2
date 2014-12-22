using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using NUnit.Framework;
using Weavers;
using Mono.Cecil.Cil;
using System;
using Application.RuleExperiments.MethodDecoratorWeaverTestClasses;

namespace UnitTests.RuleExperiments.AOP
{
	[TestFixture]
	public class ModuleWeaversFixture
	{
		private ModuleWeaver _moduleWeaver;
		private string _directory;
		string _upDirectory = Path.Combine("..", "..", "..", "..");
		private const string dllName = "Application.RuleExperiments.dll";
		private string _fullPath;

		[TestFixtureSetUp]
		public void Setup()
		{
			_moduleWeaver = new ModuleWeaver();
			DefaultAssemblyResolver assemblyResolver = new DefaultAssemblyResolver();

			_directory = Path.Combine(Directory.GetCurrentDirectory(), _upDirectory, "Implementations", 
				"Application.RuleExperiments", "bin", "Debug");
			assemblyResolver.AddSearchDirectory(_directory);
			_fullPath = Path.Combine(_directory, dllName);

			_moduleWeaver.ModuleDefinition = ModuleDefinition.ReadModule(_fullPath, new ReaderParameters {
				AssemblyResolver = assemblyResolver,
				ReadSymbols = true,
				ReadingMode = ReadingMode.Immediate
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

		[Test]
		public void Should_surround_method_with_try()
		{
			var method = _moduleWeaver.GetMethodsWithDecoratorAttributeOfType()[0];
			var il = method.Body.GetILProcessor();

			//Obtain the class type through reflection
			//Then import it to the target module
			var reflectionType = typeof(NotSupportedException);
			var exceptionCtor = reflectionType.GetConstructor(new Type[]{ });

			var constructorReference = _moduleWeaver.ModuleDefinition.Import(exceptionCtor);
			var exceptionInstance = il.Create(OpCodes.Newobj, constructorReference);

			var exceptionHandler = _moduleWeaver.CreateTryCatchFinallyBlock(method, exceptionInstance);

			Assert.IsNotNull(exceptionHandler);

			method.Body.ExceptionHandlers.Add(exceptionHandler);

			_moduleWeaver.ModuleDefinition.Import(method);
			string fullPath = Path.Combine(Directory.GetCurrentDirectory(), dllName);
			_moduleWeaver.ModuleDefinition.Assembly.Write(fullPath, new WriterParameters{ WriteSymbols = true });

			TestClass2 testClass = new TestClass2();
			Assert.Throws<NotSupportedException>(testClass.Test);
		}
	}
}