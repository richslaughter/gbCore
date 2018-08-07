
using GbCore;

namespace GbCoreTests
{
    class SimpleMmu : IMmu
    {
        byte[] _data;

        public SimpleMmu(byte[] data)
        {
            _data = data;
        }

        public void WriteByte(ushort address, byte value)
        {
            _data[address] = value;
        }

        public byte ReadByte(ushort address)
        {
            return _data[address];
        }

        public ushort ReadWord(ushort address)
        {
            return (ushort)((_data[address+1] << 8) | _data[address]);
        }

        public IMmu CopyState()
        {
            return new SimpleMmu((byte[])_data.Clone());
        }

        public byte[] DumpState(){
            return _data;
        }
    }
}