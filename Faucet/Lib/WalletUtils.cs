using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Refit;
using stratfaucet.request;

namespace stratfaucet.Lib
{
    public class WalletUtils : IWalletUtils
    {
        private IConfiguration _config;

        private String apiUrl;

        private string password;

        private string accountName;

        private string walletName;

        private IStratisWalletAPI stratApi;

        private decimal coinDivisor = 100000000M;
        public WalletUtils(IConfiguration config)
        {
            _config = config;
            apiUrl = (_config["Faucet:FullNodeApiUrl"] != null ? _config["Faucet:FullNodeApiUrl"] : "http://127.0.0.1:37220");
            password = _config["Faucet:FullNodePassword"];
            accountName = _config["Faucet:FullNodeAccountName"];
            walletName = _config["Faucet:FullNodeWalletName"];

            stratApi = RestService.For<IStratisWalletAPI>(apiUrl,
            new RefitSettings
            {
            }
          );
        }
        public async Task<Balance> GetBalance()
        {
            try
            {
                var bal = await stratApi.GetBalance(walletName);
                var address = await stratApi.GetUnusedAddress(walletName, accountName, 0);
                return new Balance
                {
                    balance = (bal.BalancesList.First().AmountConfirmed / coinDivisor),
                    returnAddress = address.Substring(address.IndexOf("\"") + 1, address.LastIndexOf("\"") - 1)
                };
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                return new Balance
                {
                    balance = 0
                };
            }
        }
        public async Task<Recipient> SendCoin(Recipient recipient)
        {

            if (newRecipient(recipient))
            {
                var amount = (await GetBalance()).balance / 100;

                BuildTransaction buildTransaction = new BuildTransaction
                {
                    WalletName = walletName,
                    AccountName = accountName,
                    CoinType = 105,
                    Password = password,
                    DestinationAddress = recipient.address,
                    Amount = amount.ToString(),
                    FeeType = "low",
                    AllowUnconfirmed = true
                };
                var transaction = await stratApi.BuildTransaction(buildTransaction);
                recipient.transaction_id = transaction.TransactionId;

                SendTransaction sendTransaction = new SendTransaction
                {
                    Hex = transaction.Hex
                };

                var resp = await stratApi.SendTransaction(sendTransaction);

                recipient.is_sent = true;

                Startup.WalletQueue.Transactions.Remove(recipient.address, out Recipient rec);
                Startup.WalletQueue.Transactions.GetOrAdd(recipient.address, recipient);

                Startup.AddressesSeen.Append(recipient.address);
                Startup.IPAddressesSeen.Append(recipient.ip_address);

                return recipient;

            } else  {
              return null;
            }
        }
        public bool newRecipient(Recipient recipient)
        {
            if(Startup.IPAddressesSeen.Contains(recipient.ip_address) || Startup.AddressesSeen.Contains(recipient.address)){
              return false;
            } else {
              return true;
            }
        }

    }
}
