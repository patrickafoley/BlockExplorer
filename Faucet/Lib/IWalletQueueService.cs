using System.Collections.Concurrent;

namespace stratfaucet.Lib
{
     public interface IWalletQueueService
    {
     ConcurrentDictionary<string, Recipient> Transactions{get; }

    }
}
