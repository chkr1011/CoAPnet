using CoAPnet.Internal;
using CoAPnet.Logging;
using CoAPnet.Protocol;
using CoAPnet.Protocol.BlockTransfer;
using CoAPnet.Protocol.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPnet.Client
{
    public sealed class CoapClientBlockTransferReceiver
    {
        readonly CoapBlockTransferOptionValueEncoder _blockValueEncoder = new CoapBlockTransferOptionValueEncoder();
        readonly CoapBlockTransferOptionValueDecoder _blockValueDecoder = new CoapBlockTransferOptionValueDecoder();

        readonly CoapMessage _requestMessage;
        readonly CoapMessage _firstResponseMessage;
        readonly CoapClient _client;
        readonly CoapNetLogger _logger;

        public CoapClientBlockTransferReceiver(CoapMessage requestMessage, CoapMessage firstResponseMessage, CoapClient client, CoapNetLogger logger)
        {
            _requestMessage = requestMessage ?? throw new ArgumentNullException(nameof(requestMessage));
            _firstResponseMessage = firstResponseMessage ?? throw new ArgumentNullException(nameof(firstResponseMessage));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public static bool IsBlockTransfer(CoapMessage responseMessage)
        {
            if (responseMessage is null) throw new ArgumentNullException(nameof(responseMessage));

            return responseMessage.Options.Any(o => o.Number == (int)CoapMessageOptionNumber.Block2);
        }

        public async Task<ArraySegment<byte>> ReceiveFullPayload(CancellationToken cancellationToken)
        {
            var receivedBlock2Option = _firstResponseMessage.Options.First(o => o.Number == (int)CoapMessageOptionNumber.Block2);
            var receivedBlock2OptionValue = _blockValueDecoder.Decode(((CoapMessageOptionUintValue)receivedBlock2Option.Value).Value);
            _logger.Trace(nameof(CoapClientBlockTransferReceiver), "Received block {0}.", FormatBlock2OptionValue(receivedBlock2OptionValue));

            var requestMessage = new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = _requestMessage.Code,
                Token = _requestMessage.Token,
                Options = new List<CoapMessageOption>(_requestMessage.Options)
            };

            var requestBlock2OptionValue = new CoapMessageOptionUintValue(0);
            var requestBlock2Option = new CoapMessageOption((byte)CoapMessageOptionNumber.Block2, requestBlock2OptionValue);
            requestMessage.Options.Add(requestBlock2Option);

            // Crate a buffer which is pre sized to at least 4 blocks.
            using (var buffer = new MemoryBuffer(receivedBlock2OptionValue.Size * 4))
            {
                buffer.Write(_firstResponseMessage.Payload);

                while (receivedBlock2OptionValue.HasFollowingBlocks)
                {
                    // Patch Block1 so that we get the next chunk.
                    receivedBlock2OptionValue.Number++;

                    // TODO: Avoid setting value. Create new instead.
                    requestBlock2OptionValue.Value = _blockValueEncoder.Encode(receivedBlock2OptionValue);

                    var response = await _client.RequestAsync(requestMessage, cancellationToken).ConfigureAwait(false);
                    receivedBlock2Option = response.Options.First(o => o.Number == (int)CoapMessageOptionNumber.Block2);
                    receivedBlock2OptionValue = _blockValueDecoder.Decode(((CoapMessageOptionUintValue)receivedBlock2Option.Value).Value);

                    _logger.Trace(nameof(CoapClientBlockTransferReceiver), "Received block {0}.", FormatBlock2OptionValue(receivedBlock2OptionValue));

                    buffer.Write(response.Payload);
                }

                return buffer.GetBuffer();
            }
        }

        string FormatBlock2OptionValue(CoapBlockTransferOptionValue value)
        {
            return $"Block2:{value.Number}/{(value.HasFollowingBlocks ? 'M' : '_')}/{value.Size}";
        }
    }
}
