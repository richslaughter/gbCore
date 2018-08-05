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

        public IMmu Mmu { get => mmu; }

        public Cpu(IMmu mmu){
            this.mmu = mmu;

            ops = new Dictionary<OpCode, Action>()
            {
                [OpCode.Noop] = () => { Cycles+=4; },

                [OpCode.JR_NZ_r8] = () => {var r8 = (ushort)(sbyte)mmu.ReadByte(ProgramCounter); ProgramCounter++; if(ZeroFlag){Cycles+=8;} else {ProgramCounter += r8; Cycles+=12;}},

                [OpCode.LD_BC_nn] = () => { BC = mmu.ReadWord(ProgramCounter); ProgramCounter+=2; Cycles+=12; },
                [OpCode.LD_DE_nn] = () => { DE = mmu.ReadWord(ProgramCounter); ProgramCounter+=2; Cycles+=12; },
                [OpCode.LD_HL_nn] = () => { HL = mmu.ReadWord(ProgramCounter); ProgramCounter+=2; Cycles+=12; },
                [OpCode.LD_SP_nn] = () => { StackPointer = mmu.ReadWord(ProgramCounter); ProgramCounter+=2; Cycles+=12; },

                [OpCode.LDI_addrHL_A] = () => { mmu.WriteByte(HL, A); HL++; Cycles+=8; },
                [OpCode.LDD_addrHL_A] = () => { mmu.WriteByte(HL, A); HL--; Cycles+=8; },
                
                [OpCode.LD_B_n] = () => { B = mmu.ReadByte(ProgramCounter); ProgramCounter++; Cycles+=8;},
                [OpCode.LD_C_n] = () => { C = mmu.ReadByte(ProgramCounter); ProgramCounter++; Cycles+=8;},
                [OpCode.LD_D_n] = () => { D = mmu.ReadByte(ProgramCounter); ProgramCounter++; Cycles+=8;},
                [OpCode.LD_E_n] = () => { E = mmu.ReadByte(ProgramCounter); ProgramCounter++; Cycles+=8;},
                [OpCode.LD_H_n] = () => { H = mmu.ReadByte(ProgramCounter); ProgramCounter++; Cycles+=8;},
                [OpCode.LD_L_n] = () => { L = mmu.ReadByte(ProgramCounter); ProgramCounter++; Cycles+=8;},
                [OpCode.LD_addrHL_n] = () => { mmu.WriteByte(HL, mmu.ReadByte(ProgramCounter)); ProgramCounter++; Cycles+=12;},
                [OpCode.LD_A_n] = () => { A = mmu.ReadByte(ProgramCounter); ProgramCounter++; Cycles+=8;},

                [OpCode.LD_B_B] = () => { B = B; Cycles+=4;},
                [OpCode.LD_B_C] = () => { B = C; Cycles+=4;},
                [OpCode.LD_B_D] = () => { B = D; Cycles+=4;},
                [OpCode.LD_B_E] = () => { B = E; Cycles+=4;},
                [OpCode.LD_B_H] = () => { B = H; Cycles+=4;},
                [OpCode.LD_B_L] = () => { B = L; Cycles+=4;},
                [OpCode.LD_B_addrHL] = () => { B = mmu.ReadByte(HL); Cycles+=8;},
                [OpCode.LD_B_A] = () => { B = A; Cycles+=4;},

                [OpCode.LD_C_B] = () => { C = B; Cycles+=4;},
                [OpCode.LD_C_C] = () => { C = C; Cycles+=4;},
                [OpCode.LD_C_D] = () => { C = D; Cycles+=4;},
                [OpCode.LD_C_E] = () => { C = E; Cycles+=4;},
                [OpCode.LD_C_H] = () => { C = H; Cycles+=4;},
                [OpCode.LD_C_L] = () => { C = L; Cycles+=4;},
                [OpCode.LD_C_addrHL] = () => { C = mmu.ReadByte(HL); Cycles+=8;},
                [OpCode.LD_C_A] = () => { C = A; Cycles+=4;},

                [OpCode.LD_D_B] = () => { D = B; Cycles+=4;},
                [OpCode.LD_D_C] = () => { D = C; Cycles+=4;},
                [OpCode.LD_D_D] = () => { D = D; Cycles+=4;},
                [OpCode.LD_D_E] = () => { D = E; Cycles+=4;},
                [OpCode.LD_D_H] = () => { D = H; Cycles+=4;},
                [OpCode.LD_D_L] = () => { D = L; Cycles+=4;},
                [OpCode.LD_D_addrHL] = () => { D = mmu.ReadByte(HL); Cycles+=8;},
                [OpCode.LD_D_A] = () => { D = A; Cycles+=4;},

                [OpCode.LD_E_B] = () => { E = B; Cycles+=4;},
                [OpCode.LD_E_C] = () => { E = C; Cycles+=4;},
                [OpCode.LD_E_D] = () => { E = D; Cycles+=4;},
                [OpCode.LD_E_E] = () => { E = E; Cycles+=4;},
                [OpCode.LD_E_H] = () => { E = H; Cycles+=4;},
                [OpCode.LD_E_L] = () => { E = L; Cycles+=4;},
                [OpCode.LD_E_addrHL] = () => { E = mmu.ReadByte(HL); Cycles+=8;},
                [OpCode.LD_E_A] = () => { E = A; Cycles+=4;},

                [OpCode.LD_H_B] = () => { H = B; Cycles+=4;},
                [OpCode.LD_H_C] = () => { H = C; Cycles+=4;},
                [OpCode.LD_H_D] = () => { H = D; Cycles+=4;},
                [OpCode.LD_H_E] = () => { H = E; Cycles+=4;},
                [OpCode.LD_H_H] = () => { H = H; Cycles+=4;},
                [OpCode.LD_H_L] = () => { H = L; Cycles+=4;},
                [OpCode.LD_H_addrHL] = () => { H = mmu.ReadByte(HL); Cycles+=8;},
                [OpCode.LD_H_A] = () => { H = A; Cycles+=4;},

                [OpCode.LD_L_B] = () => { L = B; Cycles+=4;},
                [OpCode.LD_L_C] = () => { L = C; Cycles+=4;},
                [OpCode.LD_L_D] = () => { L = D; Cycles+=4;},
                [OpCode.LD_L_E] = () => { L = E; Cycles+=4;},
                [OpCode.LD_L_H] = () => { L = H; Cycles+=4;},
                [OpCode.LD_L_L] = () => { L = L; Cycles+=4;},
                [OpCode.LD_L_addrHL] = () => { L = mmu.ReadByte(HL); Cycles+=8;},
                [OpCode.LD_L_A] = () => { L = A; Cycles+=4;},

                [OpCode.LD_addrHL_B] = () => { mmu.WriteByte(HL, B); Cycles+=8;},
                [OpCode.LD_addrHL_C] = () => { mmu.WriteByte(HL, C); Cycles+=8;},
                [OpCode.LD_addrHL_D] = () => { mmu.WriteByte(HL, D); Cycles+=8;},
                [OpCode.LD_addrHL_E] = () => { mmu.WriteByte(HL, E); Cycles+=8;},
                [OpCode.LD_addrHL_H] = () => { mmu.WriteByte(HL, H); Cycles+=8;},
                [OpCode.LD_addrHL_L] = () => { mmu.WriteByte(HL, L); Cycles+=8;},
                [OpCode.LD_addrHL_A] = () => { mmu.WriteByte(HL, A); Cycles+=8;},

                //TODO: HALT

                [OpCode.LD_A_B] = () => { A = B; Cycles+=4;},
                [OpCode.LD_A_C] = () => { A = C; Cycles+=4;},
                [OpCode.LD_A_D] = () => { A = D; Cycles+=4;},
                [OpCode.LD_A_E] = () => { A = E; Cycles+=4;},
                [OpCode.LD_A_H] = () => { A = H; Cycles+=4;},
                [OpCode.LD_A_L] = () => { A = L; Cycles+=4;},
                [OpCode.LD_A_addrHL] = () => { A = mmu.ReadByte(HL); Cycles+=8;},
                [OpCode.LD_A_A] = () => { A = A; Cycles+=4;},
                
                [OpCode.Xor_A] = () => { A ^= A; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.Xor_B] = () => { A ^= B; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.Xor_C] = () => { A ^= C; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.Xor_D] = () => { A ^= D; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.Xor_E] = () => { A ^= E; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.Xor_H] = () => { A ^= H; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.Xor_L] = () => { A ^= L; Cycles+=4; F = 0; ZeroFlag = A == 0;},

                [OpCode.Or_A] = () => { A |= A; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.Or_B] = () => { A |= B; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.Or_C] = () => { A |= C; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.Or_D] = () => { A |= D; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.Or_E] = () => { A |= E; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.Or_H] = () => { A |= H; Cycles+=4; F = 0; ZeroFlag = A == 0;},
                [OpCode.Or_L] = () => { A |= L; Cycles+=4; F = 0; ZeroFlag = A == 0;},
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
            var cpu = new Cpu(mmu.CopyState());
            cpu.StackPointer = StackPointer;
            cpu.ProgramCounter = ProgramCounter;
            cpu.Cycles = Cycles;
            cpu.AF = AF;
            cpu.BC = BC;
            cpu.DE = DE;
            cpu.HL = HL;

            //TODO: memory?

            return cpu;
        }
    }
}