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

 Ld R d8 = Load immediate 1 byte to Register Rr
 Ld Rr d16 = Load immediate 2 bytes to Register Rr
 Ld BC d16 = Load immediate 2 bytes to Register BC
 Ld B C = Copy value of C into B
*/

namespace GbCoreTests
{
    [TestClass]
    public class CpuOpCodeTests
    {
        [DataTestMethod]
        [DataRow(ChangeType.B, OpCode.LD_B_n)]
        [DataRow(ChangeType.C, OpCode.LD_C_n)]
        [DataRow(ChangeType.D, OpCode.LD_D_n)]
        [DataRow(ChangeType.E, OpCode.LD_E_n)]
        [DataRow(ChangeType.H, OpCode.LD_H_n)]
        [DataRow(ChangeType.L, OpCode.LD_L_n)]
        [DataRow(ChangeType.A, OpCode.LD_A_n)]
        public void TestLd_R_D8(ChangeType register, OpCode opCode)
        {
            //setup
            var prog = new byte[]{(byte)opCode, 0xFE}; //LD r,n
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            
            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 8;
            cpuExpected.ProgramCounter = 2;
            CpuHelpers.SetValue(cpuExpected, register, 0xFE);

            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        [DataTestMethod]
        [DataRow(ChangeType.BC, (byte)0x01)]
        [DataRow(ChangeType.DE, (byte)0x11)]
        [DataRow(ChangeType.HL, (byte)0x21)]
        [DataRow(ChangeType.StackPointer, (byte)0x31)]
        public void TestLd_Rr_D16(ChangeType register, byte opCode)
        {
            //setup
            var prog = new byte[]{opCode, 0xFE, 0xEF}; //LD rr,nn, rr = 0xEFFE
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            
            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 12;
            cpuExpected.ProgramCounter = 3;
            CpuHelpers.SetValue(cpuExpected, register, 0xEFFE);

            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B, ChangeType.B, OpCode.LD_B_B)]
        [DataRow(ChangeType.B, ChangeType.C, OpCode.LD_B_C)]
        [DataRow(ChangeType.B, ChangeType.D, OpCode.LD_B_D)]
        [DataRow(ChangeType.B, ChangeType.E, OpCode.LD_B_E)]
        [DataRow(ChangeType.B, ChangeType.H, OpCode.LD_B_H)]
        [DataRow(ChangeType.B, ChangeType.L, OpCode.LD_B_L)]
        [DataRow(ChangeType.B, ChangeType.A, OpCode.LD_B_A)]

        [DataRow(ChangeType.C, ChangeType.B, OpCode.LD_C_B)]
        [DataRow(ChangeType.C, ChangeType.C, OpCode.LD_C_C)]
        [DataRow(ChangeType.C, ChangeType.D, OpCode.LD_C_D)]
        [DataRow(ChangeType.C, ChangeType.E, OpCode.LD_C_E)]
        [DataRow(ChangeType.C, ChangeType.H, OpCode.LD_C_H)]
        [DataRow(ChangeType.C, ChangeType.L, OpCode.LD_C_L)]
        [DataRow(ChangeType.C, ChangeType.A, OpCode.LD_C_A)]

        [DataRow(ChangeType.D, ChangeType.B, OpCode.LD_D_B)]
        [DataRow(ChangeType.D, ChangeType.C, OpCode.LD_D_C)]
        [DataRow(ChangeType.D, ChangeType.D, OpCode.LD_D_D)]
        [DataRow(ChangeType.D, ChangeType.E, OpCode.LD_D_E)]
        [DataRow(ChangeType.D, ChangeType.H, OpCode.LD_D_H)]
        [DataRow(ChangeType.D, ChangeType.L, OpCode.LD_D_L)]
        [DataRow(ChangeType.D, ChangeType.A, OpCode.LD_D_A)]

        [DataRow(ChangeType.E, ChangeType.B, OpCode.LD_E_B)]
        [DataRow(ChangeType.E, ChangeType.C, OpCode.LD_E_C)]
        [DataRow(ChangeType.E, ChangeType.D, OpCode.LD_E_D)]
        [DataRow(ChangeType.E, ChangeType.E, OpCode.LD_E_E)]
        [DataRow(ChangeType.E, ChangeType.H, OpCode.LD_E_H)]
        [DataRow(ChangeType.E, ChangeType.L, OpCode.LD_E_L)]
        [DataRow(ChangeType.E, ChangeType.A, OpCode.LD_E_A)]

        [DataRow(ChangeType.H, ChangeType.B, OpCode.LD_H_B)]
        [DataRow(ChangeType.H, ChangeType.C, OpCode.LD_H_C)]
        [DataRow(ChangeType.H, ChangeType.D, OpCode.LD_H_D)]
        [DataRow(ChangeType.H, ChangeType.E, OpCode.LD_H_E)]
        [DataRow(ChangeType.H, ChangeType.H, OpCode.LD_H_H)]
        [DataRow(ChangeType.H, ChangeType.L, OpCode.LD_H_L)]
        [DataRow(ChangeType.H, ChangeType.A, OpCode.LD_H_A)]

        [DataRow(ChangeType.L, ChangeType.B, OpCode.LD_L_B)]
        [DataRow(ChangeType.L, ChangeType.C, OpCode.LD_L_C)]
        [DataRow(ChangeType.L, ChangeType.D, OpCode.LD_L_D)]
        [DataRow(ChangeType.L, ChangeType.E, OpCode.LD_L_E)]
        [DataRow(ChangeType.L, ChangeType.H, OpCode.LD_L_H)]
        [DataRow(ChangeType.L, ChangeType.L, OpCode.LD_L_L)]
        [DataRow(ChangeType.L, ChangeType.A, OpCode.LD_L_A)]

        [DataRow(ChangeType.A, ChangeType.B, OpCode.LD_A_B)]
        [DataRow(ChangeType.A, ChangeType.C, OpCode.LD_A_C)]
        [DataRow(ChangeType.A, ChangeType.D, OpCode.LD_A_D)]
        [DataRow(ChangeType.A, ChangeType.E, OpCode.LD_A_E)]
        [DataRow(ChangeType.A, ChangeType.H, OpCode.LD_A_H)]
        [DataRow(ChangeType.A, ChangeType.L, OpCode.LD_A_L)]
        [DataRow(ChangeType.A, ChangeType.A, OpCode.LD_A_A)]
        public void TestLd_R_R(ChangeType targetRegister, ChangeType sourceRegister, OpCode opCode)
        {
            //setup
            byte testValue = 0xFE;
            var prog = new byte[]{(byte)opCode}; //LD r,r
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            CpuHelpers.SetValue(cpu, targetRegister, 0xFF);
            CpuHelpers.SetValue(cpu, sourceRegister, testValue);

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 4;
            cpuExpected.ProgramCounter = 1;
            CpuHelpers.SetValue(cpuExpected, targetRegister, testValue);

            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B, OpCode.LD_B_addrHL)]
        [DataRow(ChangeType.C, OpCode.LD_C_addrHL)]
        [DataRow(ChangeType.D, OpCode.LD_D_addrHL)]
        [DataRow(ChangeType.E, OpCode.LD_E_addrHL)]
        [DataRow(ChangeType.H, OpCode.LD_H_addrHL)]
        [DataRow(ChangeType.L, OpCode.LD_L_addrHL)]
        [DataRow(ChangeType.A, OpCode.LD_A_addrHL)]
        public void TestLd_R_addrHL(ChangeType targetRegister, OpCode opCode)
        {
            //setup
            byte testValue = 0x0E;
            ushort testAddress = 0xF1;
            var prog = new byte[256];
            prog[0] = (byte)opCode; //LD r,(HL)
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            //set value to be copied to target register in memory
            mmu.WriteByte(testAddress, testValue);
            //put address to be copied from into HL
            cpu.HL = testAddress;
            
            //put some nonsense in target register to make sure it changes
            //H/L are special cases as they affect the target address, so can't be altered
            if(targetRegister != ChangeType.H && targetRegister != ChangeType.L) {
                CpuHelpers.SetValue(cpu, targetRegister, 0xFF);
            }

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 8;
            cpuExpected.ProgramCounter = 1;
            CpuHelpers.SetValue(cpuExpected, targetRegister, testValue);

            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B, OpCode.LD_addrHL_B)]
        [DataRow(ChangeType.C, OpCode.LD_addrHL_C)]
        [DataRow(ChangeType.D, OpCode.LD_addrHL_D)]
        [DataRow(ChangeType.E, OpCode.LD_addrHL_E)]
        [DataRow(ChangeType.H, OpCode.LD_addrHL_H)]
        [DataRow(ChangeType.L, OpCode.LD_addrHL_L)]
        [DataRow(ChangeType.A, OpCode.LD_addrHL_A)]
        public void TestLd_addrHL_R(ChangeType sourceRegister, OpCode opCode)
        {
            //setup
            byte testValue = 0x0E;
            ushort testAddress = 0xF1;
            var prog = new byte[256];
            prog[0] = (byte)opCode; //LD r,(HL)
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            //put nonsense in target address
            mmu.WriteByte(testAddress, 0xFF);
            //set value in register to be copied
            CpuHelpers.SetValue(cpu, sourceRegister, testValue);
            //set HL to address to be copied to
            cpu.HL = testAddress;

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 8;
            cpuExpected.ProgramCounter = 1;
            cpuExpected.Mmu.WriteByte(testAddress, testValue);

            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        public void TestLdi_addrHL_A()
        {
            //setup
            byte testValue = 0x0E;
            ushort testAddress = 0xF1;
            var prog = new byte[256];
            prog[0] = (byte)OpCode.LDD_addrHL_A; //LDD (HL),A
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            //put nonsense in target address
            mmu.WriteByte(testAddress, 0xFF);
            //set value in register to be copied
            cpu.A = testValue;
            //set HL to address to be copied to
            cpu.HL = testAddress;

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 8;
            cpuExpected.ProgramCounter = 1;
            cpuExpected.Mmu.WriteByte(testAddress, testValue);
            cpuExpected.HL = (ushort)(testAddress+1);

            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        public void TestLdd_addrHL_A()
        {
            //setup
            byte testValue = 0x0E;
            ushort testAddress = 0xF1;
            var prog = new byte[256];
            prog[0] = (byte)OpCode.LDD_addrHL_A; //LDD (HL),A
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            //put nonsense in target address
            mmu.WriteByte(testAddress, 0xFF);
            //set value in register to be copied
            cpu.A = testValue;
            //set HL to address to be copied to
            cpu.HL = testAddress;

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 8;
            cpuExpected.ProgramCounter = 1;
            cpuExpected.Mmu.WriteByte(testAddress, testValue);
            cpuExpected.HL = (ushort)(testAddress-1);

            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        [TestMethod]
        public void TestLd_addrHL_n()
        {
            //setup
            byte testValue = 0x0E;
            ushort testAddress = 0xF1;
            var prog = new byte[256];
            prog[0] = (byte)OpCode.LD_addrHL_n;
            prog[1] = testValue;
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            //put nonsense in target address
            mmu.WriteByte(testAddress, 0xFF);
            //set HL to address to be copied to
            cpu.HL = testAddress;

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 12;
            cpuExpected.ProgramCounter = 2;
            cpuExpected.Mmu.WriteByte(testAddress, testValue);

            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        [TestMethod]
        public void TestXor_A()
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
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        [DataTestMethod]
        [DataRow((byte)0x0, (byte)0x0, (byte)0x0, true)]
        [DataRow((byte)0xF, (byte)0xF, (byte)0x0, true)]
        [DataRow((byte)0x0, (byte)0xF, (byte)0xF, false)]
        [DataRow((byte)0xF, (byte)0x0, (byte)0xF, false)]
        [DataRow((byte)0x8, (byte)0x1, (byte)0x9, false)]
        [DataRow((byte)0x9, (byte)0x1, (byte)0x8, false)]
        public void TestXor_R(byte aRegisterVal, byte xorRegisterVal, byte expectedResult, bool expectedZeroFlag)
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
            CpuHelpers.SetValue(cpu, xorRegister, xorRegisterVal);

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 4;
            cpuExpected.ProgramCounter = 1;
            cpuExpected.A = expectedResult;
            cpuExpected.F = 0;
            cpuExpected.ZeroFlag = expectedZeroFlag;

            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        [DataTestMethod]
        [DataRow((byte)0x00, true)]
        [DataRow((byte)0x0F, false)]
        public void TestOr_A(byte value, bool expectedZeroFlag)
        {
            //setup
            var prog = new byte[]{(byte)OpCode.Or_A};
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            cpu.AF = 0xFF;
            cpu.BC = 0x08;
            cpu.DE = 0x08;
            cpu.HL = 0x08;
            cpu.A = value;

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 4;
            cpuExpected.ProgramCounter = 1;
            cpuExpected.A = value;
            cpuExpected.F = 0;
            cpuExpected.ZeroFlag = expectedZeroFlag;

            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        [DataTestMethod]
        [DataRow((byte)0x0, (byte)0x0, (byte)0x0, true)]
        [DataRow((byte)0xF, (byte)0xF, (byte)0xF, false)]
        [DataRow((byte)0x0, (byte)0xF, (byte)0xF, false)]
        [DataRow((byte)0xF, (byte)0x0, (byte)0xF, false)]
        [DataRow((byte)0x8, (byte)0x1, (byte)0x9, false)]
        [DataRow((byte)0x9, (byte)0x1, (byte)0x9, false)]
        public void TestOr_R(byte aRegisterVal, byte orRegisterVal, byte expectedResult, bool expectedZeroFlag)
        {
            TestOr(OpCode.Or_B, ChangeType.B, aRegisterVal, orRegisterVal, expectedResult, expectedZeroFlag); //ORB
            TestOr(OpCode.Or_C, ChangeType.C, aRegisterVal, orRegisterVal, expectedResult, expectedZeroFlag); //ORC
            TestOr(OpCode.Or_D, ChangeType.D, aRegisterVal, orRegisterVal, expectedResult, expectedZeroFlag); //ORD
            TestOr(OpCode.Or_E, ChangeType.E, aRegisterVal, orRegisterVal, expectedResult, expectedZeroFlag); //ORE
            TestOr(OpCode.Or_H, ChangeType.H, aRegisterVal, orRegisterVal, expectedResult, expectedZeroFlag); //ORH
            TestOr(OpCode.Or_L, ChangeType.L, aRegisterVal, orRegisterVal, expectedResult, expectedZeroFlag); //ORL
        }

        public void TestOr(OpCode opCode, ChangeType orRegister, byte aRegisterVal, byte orRegisterVal, byte expectedResult, bool expectedZeroFlag)
        {
            //setup
            var prog = new byte[]{(byte)opCode};
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            cpu.AF = 0xFF;
            cpu.BC = 0xFF;
            cpu.DE = 0xFF;
            cpu.HL = 0xFF;
            cpu.A = aRegisterVal;
            CpuHelpers.SetValue(cpu, orRegister, orRegisterVal);

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = 4;
            cpuExpected.ProgramCounter = 1;
            cpuExpected.A = expectedResult;
            cpuExpected.F = 0;
            cpuExpected.ZeroFlag = expectedZeroFlag;

            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
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
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        [DataTestMethod]
        [DataRow(true, (ushort)0x00, (ushort)0x02, (byte)0xFB, (uint)8)] //no action
        [DataRow(false, (ushort)0x0A, (ushort)0x11, (byte)0x05, (uint)12)] //PC goes 10 -> 12 -> 17
        [DataRow(false, (ushort)0x0A, (ushort)0x07, (byte)0xFB, (uint)12)] //0xFB = -5. PC goes 10 -> 12 -> 7
        [DataRow(false, (ushort)0x00, (ushort)0xFFFD, (byte)0xFB, (uint)12)] //ushort wrap around
        public void TestJR_NZ_r8(bool zeroFlag, ushort programCounterBefore, ushort expectedProgramCounterAfter, byte r8, uint expectedCycles)
        {
            //setup
            var prog = new byte[256];
            prog[programCounterBefore] = (byte)OpCode.JR_NZ_r8;
            prog[programCounterBefore+1] = r8;
            var mmu = new SimpleMmu(prog);
            var cpu = new Cpu(mmu);
            cpu.ProgramCounter = programCounterBefore;
            cpu.ZeroFlag = zeroFlag;

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = expectedCycles;
            cpuExpected.ProgramCounter = expectedProgramCounterAfter;

            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }
    }
}
