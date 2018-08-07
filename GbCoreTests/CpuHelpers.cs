using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GbCore
{
    public enum ChangeType{
        B = 0b0000_0000,
        C = 0b0000_0001,
        D = 0b0000_0010,
        E = 0b0000_0011,
        H = 0b0000_0100,
        L = 0b0000_0101,
        aHL = 0b0000_0110,
        A = 0b0000_0111,
        F, AF, BC, DE, HL, StackPointer, Cycles, PC
    }

    public static class CpuHelpers
    {
        public static void ValidateState(Cpu cpuExpected, Cpu cpu)
        {
            //flags
            Assert.AreEqual(cpuExpected.ZeroFlag, cpu.ZeroFlag, "Unexpected ZeroFlag value!");
            Assert.AreEqual(cpuExpected.CarryFlag, cpu.CarryFlag, "Unexpected CarryFlag value!");
            Assert.AreEqual(cpuExpected.HalfCarryFlag, cpu.HalfCarryFlag, "Unexpected HalfCarryFlag value!");
            Assert.AreEqual(cpuExpected.SubFlag, cpu.SubFlag, "Unexpected SubFlag value!");

            //registers
            Assert.AreEqual(cpuExpected.AF, cpu.AF, "Unexpected AF value!");
            Assert.AreEqual(cpuExpected.BC, cpu.BC, "Unexpected BC value!");
            Assert.AreEqual(cpuExpected.DE, cpu.DE, "Unexpected DE value!");
            Assert.AreEqual(cpuExpected.HL, cpu.HL, "Unexpected HL value!");
            
            Assert.AreEqual(cpuExpected.A, cpu.A, "Unexpected A value!");
            Assert.AreEqual(cpuExpected.B, cpu.B, "Unexpected B value!");
            Assert.AreEqual(cpuExpected.C, cpu.C, "Unexpected C value!");
            Assert.AreEqual(cpuExpected.D, cpu.D, "Unexpected D value!");
            Assert.AreEqual(cpuExpected.E, cpu.E, "Unexpected E value!");
            Assert.AreEqual(cpuExpected.F, cpu.F, "Unexpected F value!");
            Assert.AreEqual(cpuExpected.H, cpu.H, "Unexpected H value!");
            Assert.AreEqual(cpuExpected.L, cpu.L, "Unexpected L value!");

            //CPU state
            Assert.AreEqual(cpuExpected.ProgramCounter, cpu.ProgramCounter, "Unexpected ProgramCounter value!");
            Assert.AreEqual(cpuExpected.StackPointer, cpu.StackPointer, "Unexpected StackPointer value!");
            Assert.AreEqual(cpuExpected.Cycles, cpu.Cycles, "Unexpected Cycles value!");

            //FIXME: memory
            Assert.AreEqual(cpuExpected.Mmu.DumpState(), cpu.Mmu.DumpState(), "Unexpected memory values");

        }

        /*private uint GetValue(Cpu state, ChangeType type){
            switch(type){
                case ChangeType.A:
                    return state.A;
                case ChangeType.AF:
                    return state.AF;
                case ChangeType.BC:
                    return state.BC;
                case ChangeType.DE:
                    return state.DE;
                case ChangeType.HL:
                    return state.HL;
                case ChangeType.StackPointer:
                    return state.StackPointer;
                case ChangeType.PC:
                    return state.ProgramCounter;
                case ChangeType.Cycles:
                    return state.Cycles;
            }
            throw new InvalidOperationException("Unsupported ChangeType in GetValue");
        }*/

        public  static void SetValue(Cpu state, ChangeType type, uint value){
            switch(type){
                case ChangeType.A:
                    state.A = (byte)value;
                    return;
                case ChangeType.B:
                    state.B = (byte)value;
                    return;
                case ChangeType.C:
                    state.C = (byte)value;
                    return;
                case ChangeType.D:
                    state.D = (byte)value;
                    return;
                case ChangeType.E:
                    state.E = (byte)value;
                    return;
                case ChangeType.H:
                    state.H = (byte)value;
                    return;
                case ChangeType.L:
                    state.L = (byte)value;
                    return;
                case ChangeType.AF:
                    state.AF = (ushort)value;
                    return;
                case ChangeType.HL:
                    state.HL = (ushort)value;
                    return;
                case ChangeType.BC:
                    state.BC = (ushort)value;
                    return;
                case ChangeType.DE:
                    state.DE = (ushort)value;
                    return;
                case ChangeType.StackPointer:
                    state.StackPointer = (ushort)value;
                    return;
            }
            throw new InvalidOperationException("Unsupported ChangeType in SetValue");
        }
    }
}