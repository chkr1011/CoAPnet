using CoAPnet.Exceptions;
using System;

namespace CoAPnet.Protocol.BlockTransfer
{
    public class CoapBlockTransferOptionValueDecoder
    {
        public CoapBlockTransferOptionValue Decode(uint value)
        {
            var mask = 0x7;
            var size = (byte)(value & mask);

            if (size == 0x7)
            {
                throw new CoapProtocolViolationException("A SZX value of 7 is reserved.");
            }

            size = (byte)Math.Pow(2, size + 4);

            return new CoapBlockTransferOptionValue
            {
                Number = (ushort)(value >> 4),
                Size = size,
                HasFollowingBlocks = (value & 0x8) > 0
            };
        }
    }
}
