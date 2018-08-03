using System.Collections.Generic;
using System;

namespace GbCore
{
    public class Cpu
    {
        ushort af;
        ushort bc;
        ushort de;
        ushort hl;

        public ushort ProgramCounter { get;set; }
        public ushort StackPointer { get; set; }
        public uint Cycles { get; set; }

        public ushort AF { get => af; set => af = value; }
        public byte A { get => (byte)(af >> 8); set => af = (ushort)((value << 8) | F); }
        public byte F { get => (byte)(af & 0xFF); set => af = (ushort)((A << 8) | value); }

        public ushort BC { get => bc; set => bc = value; }
        public byte B { get => (byte)(bc >> 8); set => bc = (ushort)((value << 8) | C); }
        public byte C { get => (byte)(bc & 0xFF); set => bc = (ushort)((B << 8) | value); }
        
        public ushort DE { get => de; set => de = value; }
        public byte D { get => (byte)(de >> 8); set => de = (ushort)((value << 8) | E); }
        public byte E { get => (byte)(de & 0xFF); set => de = (ushort)((D << 8) | value); }

        public ushort HL { get => hl; set => hl = value; }
        public byte H { get => (byte)(hl >> 8); set => hl = (ushort)((value << 8) | L); }
        public byte L { get => (byte)(hl & 0xFF); set => hl = (ushort)((H << 8) | value); }

        public bool ZeroFlag { get => (af & 0x80) == 0x80; set => af = (ushort)(value ? af | 0x80 : af & ~0x80); }
        public bool SubFlag { get => (af & 0x40) == 0x40; set => af = (ushort)(value ? af | 0x40 : af & ~0x40); }
        public bool HalfCarryFlag { get => (af & 0x20) == 0x20; set => af = (ushort)(value ? af | 0x20 : af & ~0x20); }
        public bool CarryFlag { get => (af & 0x10) == 0x10; set => af = (ushort)(value ? af | 0x10 : af & ~0x10); }

        IMmu mmu;
        Dictionary<OpCode, Action> ops;

        public Cpu(IMmu mmu){
            this.mmu = mmu;

            ops = new Dictionary<OpCode, Action>()
            {
                [OpCode.Noop] = () => { Cycles+=4; },
                [OpCode.LDBCnn] = () => { BC = mmu.ReadWord(ProgramCounter); ProgramCounter+=2; Cycles+=12; },
                [OpCode.LDDEnn] = () => { DE = mmu.ReadWord(ProgramCounter); ProgramCounter+=2; Cycles+=12; },
                [OpCode.LDHLnn] = () => { HL = mmu.ReadWord(ProgramCounter); ProgramCounter+=2; Cycles+=12; },
                [OpCode.LDSPnn] = () => { StackPointer = mmu.ReadWord(ProgramCounter); ProgramCounter+=2; Cycles+=12; },
                
                [OpCode.LD_B_B] = () => { B = B; Cycles+=4;},
                [OpCode.LD_B_C] = () => { B = C; Cycles+=4;},
                [OpCode.LD_B_D] = () => { B = D; Cycles+=4;},
                [OpCode.LD_B_E] = () => { B = E; Cycles+=4;},
                [OpCode.LD_B_H] = () => { B = H; Cycles+=4;},
                [OpCode.LD_B_L] = () => { B = L; Cycles+=4;},
                //[OpCode.LD_B_aHL] = () => { B = L; Cycles+=4;}, //FIXME
                [OpCode.LD_B_A] = () => { B = A; Cycles+=4;},
                
                [OpCode.XorA] = () => { A ^= A; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.XorB] = () => { A ^= B; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.XorC] = () => { A ^= C; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.XorD] = () => { A ^= D; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.XorE] = () => { A ^= E; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.XorH] = () => { A ^= H; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.XorL] = () => { A ^= L; Cycles+=4; F = 0; ZeroFlag = A == 0;},
            };

        }

        public void Step()
        {
            //fetch - read byte at ProgramCounter
            var op = mmu.ReadByte(ProgramCounter);
            ProgramCounter++;
            
            //decode
            var decodedOp = ops[(OpCode)op];

            //execute
            decodedOp();
        }

        public Cpu CopyState()
        {
            var cpu = new Cpu(null);
            cpu.StackPointer = StackPointer;
            cpu.ProgramCounter = ProgramCounter;
            cpu.Cycles = Cycles;
            cpu.AF = AF;
            cpu.BC = BC;
            cpu.DE = DE;
            cpu.HL = HL;

            return cpu;
        }
    } 

    public enum OpCode{
        Noop = 0x00,
        LDBCnn = 0x01,
        LDDEnn = 0x11,
        LDHLnn = 0x21,
        LDSPnn = 0x31,
        
        LD_B_B = 0x40,
        LD_B_C = 0x41,
        LD_B_D = 0x42,
        LD_B_E = 0x43,
        LD_B_H = 0x44,
        LD_B_L = 0x45,
        LD_B_aHL = 0x46,//FIXME:
        LD_B_A = 0x47, 

        XorB = 0xA8,
        XorC = 0xA9,
        XorD = 0xAA,
        XorE = 0xAB,
        XorH = 0xAC,
        XorL = 0xAD,
        XorA = 0xAF,

        
    }
}