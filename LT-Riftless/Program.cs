using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT_Riftless
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream assemblyStream = new FileStream("Assembly-CSharp.dll", FileMode.Open);
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyStream);

            MethodDefinition headsetConnectedMethod =
                assembly.MainModule.Types
                .First(x => x.Name.Equals("GameHook")).Methods
                .First(x => x.Name.Equals("IsHeadsetConnected"));
            headsetConnectedMethod.Body.Instructions[0] = Instruction.Create(OpCodes.Ldc_I4_1);

            MethodDefinition EnableCameraMethod =
                assembly.MainModule.Types
                .First(x => x.Name.Equals("CameraMotor")).Methods
                .First(x => x.Name.Equals("EnableCamera"));
            EnableCameraMethod.Body.Instructions[4] = Instruction.Create(OpCodes.Ldc_I4_0);

            MethodDefinition IsErrorCheckMethod =
                assembly.MainModule.Types
                .First(x => x.Name.Equals("UIEntitlementCheck")).Methods
                .First(x => x.Name.Equals("ProcessEntitlementCheck"));
            IsErrorCheckMethod.Body.Instructions.Where(x => x.Offset < 0x24).ToList().ForEach(x =>
                IsErrorCheckMethod.Body.Instructions.Remove(x)
            );

            assembly.Write(new FileStream("Assembly-CSharp-Patched.dll", FileMode.Create));
        }
    }
}
