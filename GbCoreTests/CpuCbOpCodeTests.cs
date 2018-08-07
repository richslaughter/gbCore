
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GbCore;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GbCoreTests
{
    [TestClass]
    public class CpuCbOpCodeTests
    {
        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestBitWhenBitSet(ChangeType register)
        {
            //foreach bit
            for(byte i = 0; i < 8; i++)
            {
                byte val = (byte)(1 << i);
                TestBit(register, val, i, true);
            }
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestBitWhenAllBitsSet(ChangeType register)
        {
            //foreach bit
            for(byte i = 0; i < 8; i++)
            {
                TestBit(register, 0xFF, i, true);
            }
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestBitWhenBitNotSet(ChangeType register)
        {
            //foreach bit
            for(byte i = 0; i < 8; i++)
            {
                byte val = (byte)~(1 << i);
                TestBit(register, val, i, false);
            }
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestBitWhenNoBitsSet(ChangeType register)
        {
            //foreach bit
            for(byte i = 0; i < 8; i++)
            {
                TestBit(register, 0, i, false);
            }
        }

        public void TestBit(ChangeType register, byte registerVal, byte bitToCheck, bool expectedZeroFlag)
        {
            //build opcode - 0b01<reg><bit>
            var opCode = 0b0100_0000 | (bitToCheck << 3) | (byte)register;
            //setup
            var mem = new byte[256];
            mem[0] = 0xCB;
            mem[1] = (byte)opCode;
            var aHL = (byte)0xF1;
            var mmu = new SimpleMmu(mem);
            var cpu = new Cpu(mmu);
            cpu.F = 0;
            if(register == ChangeType.aHL){
                cpu.HL = aHL;
                mmu.WriteByte(aHL, registerVal);
            } else {
                CpuHelpers.SetValue(cpu, register, registerVal);
            }

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = (uint)(register == ChangeType.aHL ? 12 : 8);
            cpuExpected.ProgramCounter = 2;
            cpuExpected.ZeroFlag = expectedZeroFlag;
            cpuExpected.HalfCarryFlag = true;
            cpuExpected.SubFlag = false;
            cpuExpected.CarryFlag = false;

            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestSetBitWhenRegisterEmpty(ChangeType register)
        {
            //foreach bit
            for(byte i = 0; i < 8; i++)
            {
                var expected = 1 << i;
                TestSetBit(register, 0, i, (byte)expected);
            }
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestSetBitWhenRegisterFull(ChangeType register)
        {
            //foreach bit
            for(byte i = 0; i < 8; i++)
            {
                TestSetBit(register, 0xFF, i, 0xFF);
            }
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestSetBitWhenBitAlreadySet(ChangeType register)
        {
            //foreach bit
            for(byte i = 0; i < 8; i++)
            {
                var val = (byte)(1 << i);
                TestSetBit(register, val, i, val);
            }
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestSetBitWhenOnlyBitNotSet(ChangeType register)
        {
            //foreach bit
            for(byte i = 0; i < 8; i++)
            {
                var val = (byte)~(1 << i);
                TestSetBit(register, val, i, 0xFF);
            }
        }

        public void TestSetBit(ChangeType register, byte registerInitialVal, byte bitToSet, byte expectedRegisterVal)
        {
            //build opcode - 0b11<reg><bit>
            var opCode = 0b1100_0000 | (bitToSet << 3) | (byte)register;
            //setup
            var mem = new byte[256];
            mem[0] = 0xCB;
            mem[1] = (byte)opCode;
            var aHL = (byte)0xF1;
            var mmu = new SimpleMmu(mem);
            var cpu = new Cpu(mmu);
            cpu.F = 0;
            if(register == ChangeType.aHL){
                cpu.HL = aHL;
                mmu.WriteByte(aHL, registerInitialVal);
            } else {
                CpuHelpers.SetValue(cpu, register, registerInitialVal);
            }

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = (uint)(register == ChangeType.aHL ? 12 : 8);
            cpuExpected.ProgramCounter = 2;
            cpuExpected.ZeroFlag = false;
            cpuExpected.HalfCarryFlag = false;
            cpuExpected.SubFlag = false;
            cpuExpected.CarryFlag = false;
            if(register == ChangeType.aHL){
                cpuExpected.Mmu.WriteByte(aHL, expectedRegisterVal);
            } else {
                CpuHelpers.SetValue(cpuExpected, register, expectedRegisterVal);
            }
            

            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestResetBitWhenRegisterEmpty(ChangeType register)
        {
            //foreach bit
            for(byte i = 0; i < 8; i++)
            {
                TestResetBit(register, 0, i, 0);
            }
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestResetBitWhenRegisterFull(ChangeType register)
        {
            //foreach bit
            for(byte i = 0; i < 8; i++)
            {
                var expected =(byte) ~(1 << i);
                TestResetBit(register, 0xFF, i, expected);
            }
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestResetBitWhenBitSet(ChangeType register)
        {
            //foreach bit
            for(byte i = 0; i < 8; i++)
            {
                var val =(byte)(1 << i);
                TestResetBit(register, val, i, 0);
            }
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestResetBitWhenBitNotSet(ChangeType register)
        {
            //foreach bit
            for(byte i = 0; i < 8; i++)
            {
                var val =(byte)~(1 << i);
                TestResetBit(register, val, i, val);
            }
        }

        public void TestResetBit(ChangeType register, byte registerInitialVal, byte bitToSet, byte expectedRegisterVal)
        {
            //build opcode - 0b11<reg><bit>
            var opCode = 0b1000_0000 | (bitToSet << 3) | (byte)register;
            //setup
            var mem = new byte[256];
            mem[0] = 0xCB;
            mem[1] = (byte)opCode;
            var aHL = (byte)0xF1;
            var mmu = new SimpleMmu(mem);
            var cpu = new Cpu(mmu);
            cpu.F = 0;
            if(register == ChangeType.aHL){
                cpu.HL = aHL;
                mmu.WriteByte(aHL, registerInitialVal);
            } else {
                CpuHelpers.SetValue(cpu, register, registerInitialVal);
            }            

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = (uint)(register == ChangeType.aHL ? 12 : 8);
            cpuExpected.ProgramCounter = 2;
            cpuExpected.ZeroFlag = false;
            cpuExpected.HalfCarryFlag = false;
            cpuExpected.SubFlag = false;
            cpuExpected.CarryFlag = false;
            if(register == ChangeType.aHL){
                cpuExpected.Mmu.WriteByte(aHL, expectedRegisterVal);
            } else {
                CpuHelpers.SetValue(cpuExpected, register, expectedRegisterVal);
            }
            
            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRlcWhenRegisterFull(ChangeType register)
        {
            var val = (byte)0xFF;
            var expectedVal = (byte)0xFF;
            TestRlc(register, val, expectedVal, true, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRlcWhenRegisterEmpty(ChangeType register)
        {
            var val = (byte)0x00;
            var expectedVal = (byte)0x00;
            TestRlc(register, val, expectedVal, false, true);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRlcWhenOnlyHighBitSet(ChangeType register)
        {
            var val = (byte)0x80;
            var expectedVal = (byte)0x01;
            TestRlc(register, val, expectedVal, true, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRlcWhenOnlyLowBitSet(ChangeType register)
        {
            var val = (byte)0x01;
            var expectedVal = (byte)0x02;
            TestRlc(register, val, expectedVal, false, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRlcWhenHighAndLowBitSet(ChangeType register)
        {
            var val = (byte)0x81;
            var expectedVal = (byte)0x03;
            TestRlc(register, val, expectedVal, true, false);
        }

        public void TestRlc(ChangeType register, byte registerInitialVal, byte expectedRegisterVal, bool expectedCarryFlag, bool expectedZeroFlag)
        {
            //build opcode - 0b0000_0rrr
            var opCode = 0b0000_0000 | (byte)register;
            //setup
            var mem = new byte[256];
            mem[0] = 0xCB;
            mem[1] = (byte)opCode;
            var aHL = (byte)0xF1;
            var mmu = new SimpleMmu(mem);
            var cpu = new Cpu(mmu);
            cpu.F = 0;
            if(register == ChangeType.aHL){
                cpu.HL = aHL;
                mmu.WriteByte(aHL, registerInitialVal);
            } else {
                CpuHelpers.SetValue(cpu, register, registerInitialVal);
            }            

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = (uint)(register == ChangeType.aHL ? 12 : 8);
            cpuExpected.ProgramCounter = 2;
            cpuExpected.ZeroFlag = expectedZeroFlag;
            cpuExpected.HalfCarryFlag = false;
            cpuExpected.SubFlag = false;
            cpuExpected.CarryFlag = expectedCarryFlag;
            if(register == ChangeType.aHL){
                cpuExpected.Mmu.WriteByte(aHL, expectedRegisterVal);
            } else {
                CpuHelpers.SetValue(cpuExpected, register, expectedRegisterVal);
            }
            
            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRrcWhenRegisterFull(ChangeType register)
        {
            var val = (byte)0xFF;
            var expectedVal = (byte)0xFF;
            TestRrc(register, val, expectedVal, true, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRrcWhenRegisterEmpty(ChangeType register)
        {
            var val = (byte)0x00;
            var expectedVal = (byte)0x00;
            TestRrc(register, val, expectedVal, false, true);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRrcWhenOnlyHighBitSet(ChangeType register)
        {
            var val = (byte)0x80;
            var expectedVal = (byte)0x40;
            TestRrc(register, val, expectedVal, false, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRrcWhenOnlyLowBitSet(ChangeType register)
        {
            var val = (byte)0x01;
            var expectedVal = (byte)0x80;
            TestRrc(register, val, expectedVal, true, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRrcWhenHighAndLowBitSet(ChangeType register)
        {
            var val = (byte)0x81;
            var expectedVal = (byte)0xC0;
            TestRrc(register, val, expectedVal, true, false);
        }

        public void TestRrc(ChangeType register, byte registerInitialVal, byte expectedRegisterVal, bool expectedCarryFlag, bool expectedZeroFlag)
        {
            //build opcode - 0b0000_1rrr
            var opCode = 0b0000_1000 | (byte)register;
            //setup
            var mem = new byte[256];
            mem[0] = 0xCB;
            mem[1] = (byte)opCode;
            var aHL = (byte)0xF1;
            var mmu = new SimpleMmu(mem);
            var cpu = new Cpu(mmu);
            cpu.F = 0;
            if(register == ChangeType.aHL){
                cpu.HL = aHL;
                mmu.WriteByte(aHL, registerInitialVal);
            } else {
                CpuHelpers.SetValue(cpu, register, registerInitialVal);
            }            

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = (uint)(register == ChangeType.aHL ? 12 : 8);
            cpuExpected.ProgramCounter = 2;
            cpuExpected.ZeroFlag = expectedZeroFlag;
            cpuExpected.HalfCarryFlag = false;
            cpuExpected.SubFlag = false;
            cpuExpected.CarryFlag = expectedCarryFlag;
            if(register == ChangeType.aHL){
                cpuExpected.Mmu.WriteByte(aHL, expectedRegisterVal);
            } else {
                CpuHelpers.SetValue(cpuExpected, register, expectedRegisterVal);
            }
            
            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }

        //-----------------
        // RL
        //-----------------
        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRlWhenHighBitSetNoCarryFlag(ChangeType register)
        {
            var val = (byte)0x80;
            var expectedVal = (byte)0x00;
            var opcode = (byte)0b0001_0000;
            TestRotate(opcode, register, val, false, expectedVal, true, true);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRlWhenHighBitSetWithCarryFlag(ChangeType register)
        {
            var val = (byte)0x80;
            var expectedVal = (byte)0x01;
            var opcode = (byte)0b0001_0000;
            TestRotate(opcode, register, val, true, expectedVal, true, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRlWhenLowBitSetNoCarryFlag(ChangeType register)
        {
            var val = (byte)0x01;
            var expectedVal = (byte)0x02;
            var opcode = (byte)0b0001_0000;
            TestRotate(opcode, register, val, false, expectedVal, false, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRlWhenLowBitSetWithCarryFlag(ChangeType register)
        {
            var val = (byte)0x01;
            var expectedVal = (byte)0x03;
            var opcode = (byte)0b0001_0000;
            TestRotate(opcode, register, val, true, expectedVal, false, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRlWhenHighAndLowBitSetNoCarryFlag(ChangeType register)
        {
            var val = (byte)0x81;
            var expectedVal = (byte)0x02;
            var opcode = (byte)0b0001_0000;
            TestRotate(opcode, register, val, false, expectedVal, true, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRlWhenHighAndLowBitSetWithCarryFlag(ChangeType register)
        {
            var val = (byte)0x81;
            var expectedVal = (byte)0x03;
            var opcode = (byte)0b0001_0000;
            TestRotate(opcode, register, val, true, expectedVal, true, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRlWhenNoBitsSetNoCarryFlag(ChangeType register)
        {
            var val = (byte)0x00;
            var expectedVal = (byte)0x00;
            var opcode = (byte)0b0001_0000;
            TestRotate(opcode, register, val, false, expectedVal, false, true);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRlWhenNoBitsSetWithCarryFlag(ChangeType register)
        {
            var val = (byte)0x00;
            var expectedVal = (byte)0x01;
            var opcode = (byte)0b0001_0000;
            TestRotate(opcode, register, val, true, expectedVal, false, false);
        }

        //-----------------
        // Rr
        //-----------------
        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRrWhenHighBitSetNoCarryFlag(ChangeType register)
        {
            var val = (byte)0x80;
            var expectedVal = (byte)0x40;
            var opcode = (byte)0b0001_1000;
            TestRotate(opcode, register, val, false, expectedVal, false, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRrWhenHighBitSetWithCarryFlag(ChangeType register)
        {
            var val = (byte)0x80;
            var expectedVal = (byte)0xC0;
            var opcode = (byte)0b0001_1000;
            TestRotate(opcode, register, val, true, expectedVal, false, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRrWhenLowBitSetNoCarryFlag(ChangeType register)
        {
            var val = (byte)0x01;
            var expectedVal = (byte)0x00;
            var opcode = (byte)0b0001_1000;
            TestRotate(opcode, register, val, false, expectedVal, true, true);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRrWhenLowBitSetWithCarryFlag(ChangeType register)
        {
            var val = (byte)0x01;
            var expectedVal = (byte)0x80;
            var opcode = (byte)0b0001_1000;
            TestRotate(opcode, register, val, true, expectedVal, true, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRrWhenHighAndLowBitSetNoCarryFlag(ChangeType register)
        {
            var val = (byte)0x81;
            var expectedVal = (byte)0x40;
            var opcode = (byte)0b0001_1000;
            TestRotate(opcode, register, val, false, expectedVal, true, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRrWhenHighAndLowBitSetWithCarryFlag(ChangeType register)
        {
            var val = (byte)0x81;
            var expectedVal = (byte)0xC0;
            var opcode = (byte)0b0001_1000;
            TestRotate(opcode, register, val, true, expectedVal, true, false);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRrWhenNoBitsSetNoCarryFlag(ChangeType register)
        {
            var val = (byte)0x00;
            var expectedVal = (byte)0x00;
            var opcode = (byte)0b0001_1000;
            TestRotate(opcode, register, val, false, expectedVal, false, true);
        }

        [DataTestMethod]
        [DataRow(ChangeType.B)]
        [DataRow(ChangeType.C)]
        [DataRow(ChangeType.D)]
        [DataRow(ChangeType.E)]
        [DataRow(ChangeType.H)]
        [DataRow(ChangeType.L)]
        [DataRow(ChangeType.aHL)]
        [DataRow(ChangeType.A)]
        public void TestRrWhenNoBitsSetWithCarryFlag(ChangeType register)
        {
            var val = (byte)0x00;
            var expectedVal = (byte)0x80;
            var opcode = (byte)0b0001_1000;
            TestRotate(opcode, register, val, true, expectedVal, false, false);
        }

        public void TestRotate(byte opCode, ChangeType register, byte registerInitialVal, bool carryFlagInitialVal, byte expectedRegisterVal, bool expectedCarryFlag, bool expectedZeroFlag)
        {
            //setup
            var mem = new byte[256];
            mem[0] = 0xCB;
            mem[1] = (byte)(opCode | (byte)register); //build opcode - 0bcccc_crrr
            var aHL = (byte)0xF1;
            var mmu = new SimpleMmu(mem);
            var cpu = new Cpu(mmu);
            cpu.F = 0;
            cpu.CarryFlag = carryFlagInitialVal;
            if(register == ChangeType.aHL){
                cpu.HL = aHL;
                mmu.WriteByte(aHL, registerInitialVal);
            } else {
                CpuHelpers.SetValue(cpu, register, registerInitialVal);
            }            

            //expected outcome
            var cpuExpected = cpu.CopyState();
            cpuExpected.Cycles = (uint)(register == ChangeType.aHL ? 12 : 8);
            cpuExpected.ProgramCounter = 2;
            cpuExpected.ZeroFlag = expectedZeroFlag;
            cpuExpected.HalfCarryFlag = false;
            cpuExpected.SubFlag = false;
            cpuExpected.CarryFlag = expectedCarryFlag;
            if(register == ChangeType.aHL){
                cpuExpected.Mmu.WriteByte(aHL, expectedRegisterVal);
            } else {
                CpuHelpers.SetValue(cpuExpected, register, expectedRegisterVal);
            }
            
            //execute & validate
            cpu.Step();
            CpuHelpers.ValidateState(cpuExpected, cpu);
        }
    }
}