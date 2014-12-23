using System;
using Mono.Cecil.Cil;

namespace Weavers
{
    public static class ModuleExtensions
    {
        public static Instruction CreateLoadInstruction(this ILProcessor self, object obj)
        {
            if (obj is string)
                return self.Create(OpCodes.Ldstr, obj as string);
            else if (obj is int)
                return self.Create(OpCodes.Ldc_I4, (int)obj);

            throw new NotSupportedException();
        } 
    }
}