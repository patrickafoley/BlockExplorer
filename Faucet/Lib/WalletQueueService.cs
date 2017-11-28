using System.Collections.Concurrent;

namespace stratfaucet.Lib
{
    public class WalletQueueService : IWalletQueueService
    {

      private  ConcurrentDictionary<string, Recipient> _transactionQueue;
      public WalletQueueService() {
         _transactionQueue = new ConcurrentDictionary<string, Recipient>();
      }

       public ConcurrentDictionary<string, Recipient> Transactions{get{
         return _transactionQueue;
        }
       }

    }

}
