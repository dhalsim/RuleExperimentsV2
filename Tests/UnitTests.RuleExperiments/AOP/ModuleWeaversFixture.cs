using System.IO;
using System.Linq;
using System.Reflection;
using Domain.RuleExperiments.Exceptions;
using Mono.Cecil;
using NUnit.Framework;
using Weavers;
using System;

namespace UnitTests.RuleExperiments.AOP
{
	[TestFixture]
	public class ModuleWeaversFixture
	{
		private ModuleWeaver _moduleWeaver;
		private string _directory;
	    readonly string _upDirectory = Path.Combine("..", "..", "..", "..");
		private const string DllName = "Application.RuleExperiments.dll";
		private string _fullPath;
	    private AppDomain _tempDomain;

		[TestFixtureSetUp]
		public void Setup()
		{
			_moduleWeaver = new ModuleWeaver();
			DefaultAssemblyResolver assemblyResolver = new DefaultAssemblyResolver();

			_directory = Path.Combine(Directory.GetCurrentDirectory(), _upDirectory, "Implementations", 
				"Application.RuleExperiments", "bin", "Debug");
			assemblyResolver.AddSearchDirectory(_directory);
			_fullPath = Path.Combine(_directory, DllName);

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
            const string exceptionMessage = "System Level Exception Occureed.";

            foreach (var method in _moduleWeaver.GetMethodsWithDecoratorAttributeOfType())
            {
                string methodName = method.Name;
		        var exceptionHandler = _moduleWeaver.CreateTryCatchBlock<SystemLevelException>(method, exceptionMessage);
                method.Body.ExceptionHandlers.Add(exceptionHandler);
                _moduleWeaver.ModuleDefinition.Import(method);
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

	    private dynamic WriteAssemblyAndReadType(MethodDefinition method)
	    {
	        if (_tempDomain != null)
                AppDomain.Unload(_tempDomain);
            
            string newAssemblyPath = DllName.Replace(".dll", "2.dll");
	        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), newAssemblyPath);
	        _moduleWeaver.ModuleDefinition.Assembly.Write(fullPath, new WriterParameters {WriteSymbols = true});

            _tempDomain = AppDomain.CreateDomain("myAppDomain", null, new AppDomainSetup { ApplicationBase = Directory.GetCurrentDirectory() });
            var instanceOfMyType = (dynamic)_tempDomain.CreateInstanceFromAndUnwrap(fullPath, method.DeclaringType.FullName);
            
	        return instanceOfMyType;
	    }
	}
}