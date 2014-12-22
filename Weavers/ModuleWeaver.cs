using System;
using System.Collections.Generic;
using System.Linq;
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

		public ExceptionHandler CreateTryCatchFinallyBlock(MethodDefinition method, Instruction tryEnd)
		{
			var il = method.Body.GetILProcessor();

			var ret = il.Create(OpCodes.Ret);
			var leave = il.Create(OpCodes.Leave, ret);

			il.InsertAfter(
				method.Body.Instructions.Last(), 
				tryEnd);

			il.InsertAfter(tryEnd, leave);
			il.InsertAfter(leave, ret);

			return new ExceptionHandler(ExceptionHandlerType.Catch) {
				TryStart = method.Body.Instructions.First(),
				TryEnd = tryEnd,
				HandlerStart = tryEnd,
				HandlerEnd = ret,
				CatchType = ModuleDefinition.Import(typeof(Exception)),
			};
		}
	}
}
