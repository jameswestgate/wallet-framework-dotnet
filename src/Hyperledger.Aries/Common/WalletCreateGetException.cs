using System;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries
{
    /// <summary>
    /// Agent Framework exception
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class WalletCreateGetException : Exception
    {
        public WalletCreateGetException() { }

        public WalletCreateGetException(string aMessage) : base(aMessage) { }
    }
}
