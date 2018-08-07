
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
            cpuExpected.Cycles = (uint)(register == ChangeType.aHL ? 16 : 8);
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
            cpuExpected.Cycles = (uint)(register == ChangeType.aHL ? 16 : 8);
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
            cpuExpected.Cycles = (uint)(register == ChangeType.aHL ? 16 : 8);
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
    }
}