namespace CoAPnet.Protocol.BlockTransfer
{
    public class CoapBlockTransferOptionValue
    {
        public ushort Number { get; set; }

        public byte Size { get; set; }

        public bool HasFollowingBlocks { get; set; }
    }
}
