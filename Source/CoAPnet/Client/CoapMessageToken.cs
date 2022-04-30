using System.Linq;

namespace CoAPnet.Client
{
    public sealed class CoapMessageToken
    {
        public CoapMessageToken(byte[] value)
        {
            Value = value ?? throw new System.ArgumentNullException(nameof(value));
        }

        public byte[] Value { get; }

        public override int GetHashCode()
        {
            var hash = 0;
            foreach (var @byte in Value)
            {
                hash ^= @byte;
            }

            return hash;
        }

        public override bool Equals(object obj)
        {
            var other = obj as CoapMessageToken;
            if (other == null)
            {
                return false;
            }

            return Value.SequenceEqual(other.Value);
        }
    }
}
