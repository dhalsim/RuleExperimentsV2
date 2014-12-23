using System;
using System.Collections.Generic;
using System.Linq;
using Domain.RuleExperiments.Exceptions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Weavers
{
	public class ModuleWeaver
	{
		// Will log an informational message to MSBuild
		public Action<string> LogInfo { get; set; }

		// An instance of Mono.Cecil.ModuleDefinition for processing
		public ModuleDefinition ModuleDefinition { get; set; }

		// Init logging delegates to make testing easier
		public ModuleWeaver ()
		{
			LogInfo = m => {
			};
		}

		public void Execute()
		{
            
		}

		public List<TypeDefinition> GetTypesWithDecoratorAttribute()
		{
			const string decoratorType = "MethodDecoratorAttribute";

			return ModuleDefinition.Types
                .Where(t => t.CustomAttributes.Any(ca => ca.AttributeType.Name == decoratorType))
                .ToList();
		}

		public List<MethodDefinition> GetMethodsWithDecoratorAttributeOfType()
		{
			const string decoratorType = "MethodDecoratorAttribute";

			return 
                (from definitionType in ModuleDefinition.Types
			              from method in definitionType.Methods
			              where method.CustomAttributes.Any(ca => ca.AttributeType.Name == decoratorType)
			              select method).ToList();
		}

		public ExceptionHandler CreateTryCatchBlock<T>(MethodDefinition method, string exceptionMessage)
		{
            method.Body.InitLocals = true;
            var il = method.Body.GetILProcessor();
            
            // Obtain the exception class type through reflection with appropriate constructor
            // Then import it to the target module
            var reflectionType = typeof(T);
            var exceptionCtor = reflectionType.GetConstructor(new Type[] { typeof(string), typeof(Exception) });
            var constructorReference = ModuleDefinition.Import(exceptionCtor);
            
            // create instance with new
            var exceptionInstance = il.Create(OpCodes.Newobj, constructorReference);

            // exception variable from catch statement
            var exceptionVariable = new VariableDefinition("e", method.Module.Import(typeof(Exception)));
            method.Body.Variables.Add(exceptionVariable);

		    var last = method.Body.Instructions.Last();
            Instruction tryEnd;
            Instruction ret = il.Create(OpCodes.Ret);

            // this pattern lets you construct IL instructions without thinking reverse in stack
            il.InsertAfter(last, tryEnd = last = il.Create(OpCodes.Stloc_S, exceptionVariable));    // store exception variable to local
            il.InsertAfter(last, last = il.CreateLoadInstruction(exceptionMessage));                // load exception message string to the stack
            il.InsertAfter(last, last = il.Create(OpCodes.Ldloc_0));                                // load exception variable from local
            il.InsertAfter(last, last = exceptionInstance);                                         // call constructor (uses these two variables from stack)
            il.InsertAfter(last, last = il.Create(OpCodes.Throw));                                  // throw that exception
            il.InsertAfter(last, last = il.Create(OpCodes.Leave, ret));                             // leave catch region and return void

			return new ExceptionHandler(ExceptionHandlerType.Catch) {
				TryStart = method.Body.Instructions.First(),
                TryEnd = tryEnd,
                HandlerStart = tryEnd,
                HandlerEnd = last,
				CatchType = ModuleDefinition.Import(typeof(Exception)),
			};
		}
	}
}
