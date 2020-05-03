using System;

namespace CoAPnet.Protocol.BlockTransfer
{
    public class CoapBlockTransferOptionValueEncoder
    {
        public uint Encode(CoapBlockTransferOptionValue value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            var result = 0U;

            result |= (uint)value.Number << 4;

            if (value.HasFollowingBlocks)
            {
                result |= 0x8;
            }


            result |= 3;

            //size = (byte)Math.Pow(2, size + 4);

            return result;
        }
    }
}
