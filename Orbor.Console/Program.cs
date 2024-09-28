using Orbor.Enums;
using Orbor.Imports;
using Orbor.Types;
using Orbor.Operands;
using Orbor.Sections;
using Orbor.Exports;

namespace Orbor.Console
{
    internal static class Program
    {
        private const string path = @"E:\Windows\Documents\Coding\UnityShenanigans\WASMTesting\test_game.wasm";
        private const string outPath = @"E:\Windows\Documents\Coding\UnityShenanigans\WASMTesting\test_game_out.wasm";

        static void Main()
        {
            WASMModule module = WASMModule.From(path);
            module.Write(outPath);

            var typeSection = module.Sections.Single(s => s.Type == SectionType.Type) as TypeSection;
            if (typeSection == null)
                return;

            var dataSection = module.Sections.Single(s => s.Type == SectionType.Data) as DataSection;
            if (dataSection == null)
                return;

            var importSection = module.Sections.Single(s => s.Type == SectionType.Import) as ImportSection;
            if (importSection == null)
                return;

            var functionSection = module.Sections.Single(s => s.Type == SectionType.Func) as FuncSection;
            if (functionSection == null)
                return;

            var exportSection = module.Sections.Single(s => s.Type == SectionType.Export) as ExportSection;
            if (exportSection == null)
                return;

            var codeSection = module.Sections.Single(s => s.Type == SectionType.Code) as CodeSection;
            if (codeSection == null)
                return;

            // Export all functions
            var functionsImported = importSection.Imports.Where(i => i.Type == ImportType.Type).Count();


            for (int i = 0; i < functionSection.Functions.Count; i++)
            {
                exportSection.Exports.Add(new Export($"func_{functionsImported+i}", (ulong)i, ExportType.Func));
            }

	 
	    // add new func signature
            typeSection.FunctionSignatures.Add(new FunctionSignature([], [NumberType.I32]));




            //List<Instruction> instructions = [
            //    
            //    Instruction.Create(OpCode.LocalGet, new IndexOperand(1)),
            //    Instruction.Create(OpCode.Call, new IndexOperand(100)),
            //;


	    // Expand memory
            ulong memorySize = 0;

            foreach (var import in importSection.Imports)
            {
                if (import is MemoryImport memoryImport)
                {
                    if (memoryImport.MemoryType.LimitType.HasMax)
                    {
                        // Not supported?
                        return;
                    }
                    memorySize = memoryImport.MemoryType.LimitType.Minimum;
                    memoryImport.MemoryType.LimitType.Minimum += 1;
                }
            }

	
	    // find offset of injected data
            var dataOffset = (int)(65536 * memorySize);

	    // A status byte for each function
            var initialData = new byte[functionSection.Functions.Count];
            for (int i = 0; i < functionSection.Functions.Count; i++)
            {
                initialData[i] = 0;
            }

            dataSection.Segments.Add(new DataSegment(DataSegmentType.Active, initialData, null, [Instruction.Create(OpCode.I32Const, new I32Operand(dataOffset)), Instruction.Create(OpCode.End)]));
        }

    }
}