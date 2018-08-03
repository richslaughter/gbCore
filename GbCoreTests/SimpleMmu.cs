
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

        public byte ReadByte(ushort address)
        {
            return _data[address];
        }

        public ushort ReadWord(ushort address)
        {
            return (ushort)((_data[address+1] << 8) | _data[address]);
        }
    }
}