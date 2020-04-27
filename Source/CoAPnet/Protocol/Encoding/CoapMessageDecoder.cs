using System;
using System.Collections.Generic;

namespace CoapTest.Protocol.Encoding
{
    public sealed class CoapMessageDecoder
    {
        // TODO: Consider creating "CoapMessageDecodeResult" which has Message (null or set) and "DecodeResult" as INTERFACE with all possible errors (VersionInvalidDecodeResult) etc.
        public CoapMessage Decode(ArraySegment<byte> buffer)
        {
            using (var reader = new CoapMessageReader(buffer))
            {
                var version = reader.ReadBits(2);
                if (version != 0x1)
                {
                    throw new CoAPProtocolViolationException("Version is not set to 1.");
                }

                var type = reader.ReadBits(2);

                var tokenLength = reader.ReadBits(4);

                //var codeDetails = (byte)reader.ReadBits(3);
                //var codeClass = (byte)reader.ReadBits(5);

                var code = (byte)reader.ReadBits(8);
                var codeClass = (byte)(code >> 5);
                var codeDetails = (byte)(0x1F & code);

                var id = reader.ReadByte() << 8;
                id |= reader.ReadByte();

                byte[] token = null;
                if (tokenLength > 0)
                {
                    token = reader.ReadBytes(tokenLength);
                }

                var options = DecodeOptions(reader);

                var payload = reader.ReadToEnd();
                if (payload.Count > 0)
                {
                    // Skip payload marker.
                    payload = new ArraySegment<byte>(payload.Array, payload.Offset + 1, payload.Count - 1);
                }

                var message = new CoapMessage
                {
                    Type = (CoapMessageType)type,
                    Code = new CoapMessageCode(codeClass, codeDetails),
                    Id = (ushort)id,
                    Token = token,
                    Options = options,
                    Payload = payload
                };

                return message;
            }

            List<CoapMessageOption> DecodeOptions(CoapMessageReader reader)
            {
                var options = new List<CoapMessageOption>();



                return options;
            }
        }
    }
}
