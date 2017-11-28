using System;
using stratfaucet.Lib;

namespace stratfaucet.Jobs {

  public class SendCoinJob {

      public static void Execute() {
          var walletQueue = Startup.WalletQueue; // TODO singleton
          var walletUtils = Startup.WalletUtils;
          foreach(Recipient recp in walletQueue.Transactions.Values)
          {
            if(!recp.is_sent){
              Console.WriteLine("Sending Transaction {0}  ",  recp.address);
              walletUtils.SendCoin(recp);
            }
          }
      }
  }

}
