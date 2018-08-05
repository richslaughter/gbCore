namespace GbCore
{
    public enum OpCode{
        Noop =          0x00,
        LD_BC_nn =      0x01,
        LD_addrBC_A =   0x02, //
        LD_B_n =        0x06,
        LD_A_addrBC =   0x0A, //
        LD_C_n =        0x0E,
        LD_DE_nn =      0x11,
        LD_addrDE_A =   0x12, //
        LD_D_n =        0x16,
        LD_A_addrDE =   0x1A, //
        LD_E_n =        0x1E,
        JR_NZ_r8 =      0x20,
        LD_HL_nn =      0x21,
        LDI_addrHL_A =  0x22,
        LD_H_n =        0x26,
        LD_L_n =        0x2E,
        LD_SP_nn =      0x31,
        LDD_addrHL_A =  0x32,
        LD_addrHL_n =   0x36,
        LD_A_n =        0x3E,

        LD_B_B =        0x40,
        LD_B_C =        0x41,
        LD_B_D =        0x42,
        LD_B_E =        0x43,
        LD_B_H =        0x44,
        LD_B_L =        0x45,
        LD_B_addrHL =   0x46,
        LD_B_A =        0x47, 

        LD_C_B =        0x48,
        LD_C_C =        0x49,
        LD_C_D =        0x4A,
        LD_C_E =        0x4B,
        LD_C_H =        0x4C,
        LD_C_L =        0x4D,
        LD_C_addrHL =   0x4E,
        LD_C_A =        0x4F, 

        LD_D_B =        0x50,
        LD_D_C =        0x51,
        LD_D_D =        0x52,
        LD_D_E =        0x53,
        LD_D_H =        0x54,
        LD_D_L =        0x55,
        LD_D_addrHL =   0x56,
        LD_D_A =        0x57, 

        LD_E_B =        0x58,
        LD_E_C =        0x59,
        LD_E_D =        0x5A,
        LD_E_E =        0x5B,
        LD_E_H =        0x5C,
        LD_E_L =        0x5D,
        LD_E_addrHL =   0x5E,
        LD_E_A =        0x5F, 

        LD_H_B =        0x60,
        LD_H_C =        0x61,
        LD_H_D =        0x62,
        LD_H_E =        0x63,
        LD_H_H =        0x64,
        LD_H_L =        0x65,
        LD_H_addrHL =   0x66,
        LD_H_A =        0x67, 

        LD_L_B =        0x68,
        LD_L_C =        0x69,
        LD_L_D =        0x6A,
        LD_L_E =        0x6B,
        LD_L_H =        0x6C,
        LD_L_L =        0x6D,
        LD_L_addrHL =   0x6E,
        LD_L_A =        0x6F, 

        LD_addrHL_B =   0x70,
        LD_addrHL_C =   0x71,
        LD_addrHL_D =   0x72,
        LD_addrHL_E =   0x73,
        LD_addrHL_H =   0x74,
        LD_addrHL_L =   0x75,
        Halt =          0x76,
        LD_addrHL_A =   0x77, 

        LD_A_B =        0x78,
        LD_A_C =        0x79,
        LD_A_D =        0x7A,
        LD_A_E =        0x7B,
        LD_A_H =        0x7C,
        LD_A_L =        0x7D,
        LD_A_addrHL =   0x7E,
        LD_A_A =        0x7F, 

        Xor_B =         0xA8,
        Xor_C =         0xA9,
        Xor_D =         0xAA,
        Xor_E =         0xAB,
        Xor_H =         0xAC,
        Xor_L =         0xAD,
        Xor_addrHL =    0xAE, //
        Xor_A =         0xAF, 

        Or_B =          0xB0,
        Or_C =          0xB1,
        Or_D =          0xB2,
        Or_E =          0xB3, 
        Or_H =          0xB4,
        Or_L =          0xB5,
        Or_addrHL =     0xB6, //
        Or_A =          0xB7,
    }
}