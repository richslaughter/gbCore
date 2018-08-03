using Microsoft.VisualStudio.TestTools.UnitTesting;
using GbCore;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System;

/*Notes
 R = 8 bit register (B, C etc)
 Rr = 16 bit register (BC, DE etc)
 Nn = immediate 2 bytes
 Ld = Load

 Ld Rr d16 = Load immedate 2 bytes to Register Rr
 Ld BC d16 = Load immedate 2 bytes to Register BC
 Ld B C = Copy value of C into B

 
*/

namespace GbCoreTests
{
    [TestClass]
    public class CpuOpCodeTests
    {
        [DataTestMethod]
        [DataRow(ChangeType.BC, (byte)0x01)]
        [DataRow(ChangeType.DE, (byte)0x11)]
        [DataRow(ChangeType.HL, (byte)0x21)]
        [DataRow(ChangeType.StackPointer, (byte)0x31)]
        public void TestLdRrD16(ChangeType register, byte opCode)
        {
            //setup
            var prog = new byte[]{opCode, 0xFE, 0xEF}; //LD rr,nn, rr = 0xEFFE
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            
            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 12;
            cpuExpected.ProgramCounter = 3;
            SetValue(cpuExpected, register, 0xEFFE);

            //execute & validate
            cpu.Step();
            ValidateState(cpuExpected, cpu);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B, ChangeType.B, OpCode.LD_B_B)]
        [DataRow(ChangeType.B, ChangeType.C, OpCode.LD_B_C)]
        [DataRow(ChangeType.B, ChangeType.D, OpCode.LD_B_D)]
        [DataRow(ChangeType.B, ChangeType.E, OpCode.LD_B_E)]
        [DataRow(ChangeType.B, ChangeType.H, OpCode.LD_B_H)]
        [DataRow(ChangeType.B, ChangeType.L, OpCode.LD_B_L)]
        [DataRow(ChangeType.B, ChangeType.A, OpCode.LD_B_A)]
        public void TestLdRR(ChangeType targetRegister, ChangeType sourceRegister, OpCode opCode)
        {
            //setup
            byte testValue = 0xFE;
            var prog = new byte[]{(byte)opCode}; //LD r,r
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            SetValue(cpu, targetRegister, 0xFF);
            SetValue(cpu, sourceRegister, testValue);

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 4;
            cpuExpected.ProgramCounter = 1;
            SetValue(cpuExpected, targetRegister, testValue);

            //execute & validate
            cpu.Step();
            ValidateState(cpuExpected, cpu);
        }

        [TestMethod]
        public void TestXorA()
        {
            //setup
            var prog = new byte[]{0xAF}; //XOR A
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            cpu.AF = 0xFF;
            cpu.BC = 0x08;
            cpu.DE = 0x08;
            cpu.HL = 0x08;

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 4;
            cpuExpected.ProgramCounter = 1;
            cpuExpected.A = 0;
            cpuExpected.F = 0;
            cpuExpected.ZeroFlag = true;

            //execute & validate
            cpu.Step();
            ValidateState(cpuExpected, cpu);
        }

        [DataTestMethod]
        [DataRow((byte)0x0, (byte)0x0, (byte)0x0, true)]
        [DataRow((byte)0xF, (byte)0xF, (byte)0x0, true)]
        [DataRow((byte)0x0, (byte)0xF, (byte)0xF, false)]
        [DataRow((byte)0xF, (byte)0x0, (byte)0xF, false)]
        [DataRow((byte)0x8, (byte)0x1, (byte)0x9, false)]
        [DataRow((byte)0x9, (byte)0x1, (byte)0x8, false)]
        public void TestXorR(byte aRegisterVal, byte xorRegisterVal, byte expectedResult, bool expectedZeroFlag)
        {
            TestXor(0xA8, ChangeType.B, aRegisterVal, xorRegisterVal, expectedResult, expectedZeroFlag); //XORB
            TestXor(0xA9, ChangeType.C, aRegisterVal, xorRegisterVal, expectedResult, expectedZeroFlag); //XORC
            TestXor(0xAA, ChangeType.D, aRegisterVal, xorRegisterVal, expectedResult, expectedZeroFlag); //XORD
            TestXor(0xAB, ChangeType.E, aRegisterVal, xorRegisterVal, expectedResult, expectedZeroFlag); //XORE
            TestXor(0xAC, ChangeType.H, aRegisterVal, xorRegisterVal, expectedResult, expectedZeroFlag); //XORH
            TestXor(0xAD, ChangeType.L, aRegisterVal, xorRegisterVal, expectedResult, expectedZeroFlag); //XORL
        }

        public void TestXor(byte opCode, ChangeType xorRegister, byte aRegisterVal, byte xorRegisterVal, byte expectedResult, bool expectedZeroFlag)
        {
            //setup
            var prog = new byte[]{opCode};
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            cpu.AF = 0xFF;
            cpu.BC = 0xFF;
            cpu.DE = 0xFF;
            cpu.HL = 0xFF;
            cpu.A = aRegisterVal;
            SetValue(cpu, xorRegister, xorRegisterVal);

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 4;
            cpuExpected.ProgramCounter = 1;
            cpuExpected.A = expectedResult;
            cpuExpected.F = 0;
            cpuExpected.ZeroFlag = expectedZeroFlag;

            //execute & validate
            cpu.Step();
            ValidateState(cpuExpected, cpu);
        }

        [TestMethod]
        public void TestNoop()
        {
            //setup
            var prog = new byte[]{0x00}; //Nop
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            cpu.AF = 0xFF;
            cpu.BC = 0xFF;
            cpu.DE = 0xFF;
            cpu.HL = 0xFF;

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 4;
            cpuExpected.ProgramCounter = 1;

            //execute & validate
            cpu.Step();
            ValidateState(cpuExpected, cpu);
        }

        private void ValidateState(Cpu cpuExpected, Cpu cpu)
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

            //memory

        }

        private uint GetValue(Cpu state, ChangeType type){
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
        }

        private void SetValue(Cpu state, ChangeType type, uint value){
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

        public enum ChangeType{
            A, B, C, D, E, F, H, L, AF, BC, DE, HL, StackPointer, Cycles, PC
        }
    }
}
