using CoAPnet.Exceptions;
using System;

namespace CoAPnet.Protocol.BlockTransfer
{
    public class CoapBlockTransferOptionValueEncoder
    {
        public uint Encode(CoapBlockTransferOptionValue value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            if (value.Size > 1024)
            {
                throw new CoapProtocolViolationException("Block2 size max invalid (max 1024).");
            }

            var result = 0U;

            result |= (uint)value.Number << 4;

            if (value.HasFollowingBlocks)
            {
                result |= 0x8;
            }

            result |= (byte)(Math.Log(value.Size, 2) - 4);
            return result;
        }
    }
}
