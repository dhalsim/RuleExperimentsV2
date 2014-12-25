using System;
using System.IO;
using Mono.Cecil;
using NUnit.Framework;
using Weavers;

namespace UnitTests.RuleExperiments.AOP
{
	[TestFixture]
	public class BaseAOPFixture
	{
		protected ModuleWeaver ModuleWeaver;
		private string _directory;
		readonly string _upDirectory = Path.Combine("..", "..", "..", "..");
		private const string DllName = "Application.RuleExperiments.dll";
		private string _fullPath;
		private AppDomain _tempDomain;

		[TestFixtureSetUp]
		public void Setup()
		{
			ModuleWeaver = new ModuleWeaver();
			DefaultAssemblyResolver assemblyResolver = new DefaultAssemblyResolver();

			_directory = Path.Combine(Directory.GetCurrentDirectory(), _upDirectory, "Implementations",
				"Application.RuleExperiments", "bin", "Debug");
			assemblyResolver.AddSearchDirectory(_directory);
			_fullPath = Path.Combine(_directory, DllName);

			ModuleWeaver.ModuleDefinition = ModuleDefinition.ReadModule(_fullPath, new ReaderParameters {
				AssemblyResolver = assemblyResolver,
				ReadSymbols = true,
				ReadingMode = ReadingMode.Immediate
			});

			ModuleWeaver.Execute();
		}

		protected virtual dynamic WriteAssemblyAndReadType(MethodDefinition method)
		{
			// if appdomain exists, unload it
			if (_tempDomain != null)
				AppDomain.Unload(_tempDomain);

			string fullPath = Path.Combine(Directory.GetCurrentDirectory(), DllName);
			ModuleWeaver.ModuleDefinition.Assembly.Write(fullPath, new WriterParameters { WriteSymbols = true });

			// create a new appdomain because we need to read the assembly multiple times for tests
			_tempDomain = AppDomain.CreateDomain("myAppDomain", null, new AppDomainSetup { ApplicationBase = Directory.GetCurrentDirectory() });
			var instanceOfMyType = (dynamic)_tempDomain.CreateInstanceFromAndUnwrap(fullPath, method.DeclaringType.FullName);

			return instanceOfMyType;
		}
	}
}