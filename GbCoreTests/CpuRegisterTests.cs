using Microsoft.VisualStudio.TestTools.UnitTesting;
using GbCore;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GbCoreTests
{
    [TestClass]
    public class CpuRegisterTests
    {
        [TestMethod]
        public void TestFlags()
        {
            var state = new Cpu(Substitute.For<IMmu>());

            state.AF = 0xFFFF;            
            Assert.IsTrue(state.ZeroFlag);
            Assert.IsTrue(state.SubFlag);
            Assert.IsTrue(state.HalfCarryFlag);
            Assert.IsTrue(state.CarryFlag);

            state.AF = 0x0000;            
            Assert.IsFalse(state.ZeroFlag);
            Assert.IsFalse(state.SubFlag);
            Assert.IsFalse(state.HalfCarryFlag);
            Assert.IsFalse(state.CarryFlag);

            state.AF = 0xFFFF;
            state.ZeroFlag = false;
            Assert.IsFalse(state.ZeroFlag);
            Assert.IsTrue(state.SubFlag);
            Assert.IsTrue(state.HalfCarryFlag);
            Assert.IsTrue(state.CarryFlag);
            state.SubFlag = false;
            state.HalfCarryFlag = false;
            state.CarryFlag = false;
            Assert.IsFalse(state.ZeroFlag);
            Assert.IsFalse(state.SubFlag);
            Assert.IsFalse(state.HalfCarryFlag);
            Assert.IsFalse(state.CarryFlag);
            Assert.AreEqual(0xFF, state.A);

            state.AF = 0x0;
            state.ZeroFlag = true;
            Assert.IsTrue(state.ZeroFlag);
            Assert.IsFalse(state.SubFlag);
            Assert.IsFalse(state.HalfCarryFlag);
            Assert.IsFalse(state.CarryFlag);
            state.SubFlag = true;
            state.HalfCarryFlag = true;
            state.CarryFlag = true;
            Assert.IsTrue(state.ZeroFlag);
            Assert.IsTrue(state.SubFlag);
            Assert.IsTrue(state.HalfCarryFlag);
            Assert.IsTrue(state.CarryFlag);
            Assert.AreEqual(0x00, state.A);
        }

        [TestMethod]
        public void TestAf()
        {
            var state = new Cpu(Substitute.For<IMmu>());
            Assert.AreEqual(0, state.AF);

            state.AF = 0xF00D;
            Assert.AreEqual(0xF00D, state.AF);
            Assert.AreEqual(0xF0, state.A);
            Assert.AreEqual(0x0D, state.F);

            state.A = 0xC0;
            Assert.AreEqual(0xC00D, state.AF);
            Assert.AreEqual(0xC0, state.A);
            Assert.AreEqual(0x0D, state.F);

            state.F = 0xDE;
            Assert.AreEqual(0xC0DE, state.AF);
            Assert.AreEqual(0xC0, state.A);
            Assert.AreEqual(0xDE, state.F);
        }

        [TestMethod]
        public void TestBc()
        {
            var state = new Cpu(Substitute.For<IMmu>());
            Assert.AreEqual(0, state.BC);

            state.BC = 0xF00D;
            Assert.AreEqual(0xF00D, state.BC);
            Assert.AreEqual(0xF0, state.B);
            Assert.AreEqual(0x0D, state.C);

            state.B = 0xC0;
            Assert.AreEqual(0xC00D, state.BC);
            Assert.AreEqual(0xC0, state.B);
            Assert.AreEqual(0x0D, state.C);

            state.C = 0xDE;
            Assert.AreEqual(0xC0DE, state.BC);
            Assert.AreEqual(0xC0, state.B);
            Assert.AreEqual(0xDE, state.C);
        }

        [TestMethod]
        public void TestDe()
        {
            var state = new Cpu(Substitute.For<IMmu>());
            Assert.AreEqual(0, state.DE);

            state.DE = 0xF00D;
            Assert.AreEqual(0xF00D, state.DE);
            Assert.AreEqual(0xF0, state.D);
            Assert.AreEqual(0x0D, state.E);

            state.D = 0xC0;
            Assert.AreEqual(0xC00D, state.DE);
            Assert.AreEqual(0xC0, state.D);
            Assert.AreEqual(0x0D, state.E);

            state.E = 0xDE;
            Assert.AreEqual(0xC0DE, state.DE);
            Assert.AreEqual(0xC0, state.D);
            Assert.AreEqual(0xDE, state.E);
        }

        [TestMethod]
        public void TestHl()
        {
            var state = new Cpu(Substitute.For<IMmu>());
            Assert.AreEqual(0, state.HL);

            state.HL = 0xF00D;
            Assert.AreEqual(0xF00D, state.HL);
            Assert.AreEqual(0xF0, state.H);
            Assert.AreEqual(0x0D, state.L);

            state.H = 0xC0;
            Assert.AreEqual(0xC00D, state.HL);
            Assert.AreEqual(0xC0, state.H);
            Assert.AreEqual(0x0D, state.L);

            state.L = 0xDE;
            Assert.AreEqual(0xC0DE, state.HL);
            Assert.AreEqual(0xC0, state.H);
            Assert.AreEqual(0xDE, state.L);
        }
    }
}
