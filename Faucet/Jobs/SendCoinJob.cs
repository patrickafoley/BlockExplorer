using System;
using stratfaucet.Lib;

namespace stratfaucet.Jobs {

  public class SendCoinJob {

      public static void Execute() {
          Console.WriteLine("SendCoinJob.Execute");
          // var walletQueue = Startup.WalletQueue; // TODO singleton
          var walletUtils = Startup.WalletUtils;
          foreach(Recipient recp in  Startup.WalletQueue.Transactions.Values)
          {
            Console.WriteLine("Sending Transaction {0}  ",  recp.address);
            if(!recp.is_sent && !recp.is_error)
            {
              walletUtils.SendCoin(recp);
            } else{

            }
          }
      }
  }

}
