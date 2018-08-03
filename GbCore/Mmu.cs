using System;

namespace GbCore
{
    public interface IMmu
    {
        byte ReadByte(ushort address);
        ushort ReadWord(ushort address);
    }

    public class Mmu : IMmu
    {
        private byte _bootFlag;
        private byte[] _bios;

        public Mmu(byte[] bios)
        {
            if(bios.Length != 256){
                throw new InvalidOperationException("Bios must be 256 bytes!");
            }
            _bios = bios;
            _bootFlag = 0x00;
        }

        public byte ReadByte(ushort address)
        {
            if(_bootFlag == 0x00 && address <= 0x00FF){
                return _bios[address];
            }

            if(address == 0xFF50){
                return _bootFlag;
            }

            throw new InvalidOperationException("Unsupported MMU read address!");
        }

        public void WriteByte(ushort address, byte val)
        {
            if(address <= 0x00FF && _bootFlag == 0x00)
            {
                throw new InvalidOperationException("Cannot write to BIOS whilst booting!");
            }

            if(address == 0xFF50){
                _bootFlag = val;
            }
            else
            {
                throw new InvalidOperationException("Unsupported MMU write address!");    
            }
        }

        public ushort ReadWord(ushort address){
            return (ushort)((ReadByte((ushort)(address + 1)) << 8) | ReadByte(address));
        }
    }
}