using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using stratfaucet.Lib;

namespace stratfaucet.Controllers

{
    [Route("api/[controller]")]
    public class FaucetController : Controller
    {

        private IWalletUtils _walletUtils;
        public FaucetController(IConfiguration config)
        {
            // TODO dependency injection

            _walletUtils = Startup.WalletUtils;
        }

        [Route("/Error")]
        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }

        [HttpGet("GetBalance")]
        public async Task<Balance> GetBalance()
        {
            return await _walletUtils.GetBalance();
        }

        [HttpPost("SendCoin")]
        public Recipient SendCoin([FromBody] Recipient recipient)
        {
            recipient.ip_address = HttpContext.Connection.RemoteIpAddress.MapToIPv4().GetAddressBytes().ToString();

            if (Startup.WalletQueue.Transactions.Count > 100)
            {
                // TODO figure out the "right" way to return an error object to an angular ui
                return null;
            }

            Startup.WalletQueue.Transactions.GetOrAdd(recipient.address, recipient);
            return WaitForTransaction(recipient.address);
        }


        private  Recipient WaitForTransaction(string address)
        {
            int timeout = 30;
            int waitCount = 1;
            while (waitCount < timeout)
            {
                Recipient recipient;
                if (Startup.WalletQueue.Transactions.TryGetValue(address, out recipient))
                {
                    if (recipient.is_sent)
                    {
                        return recipient;
                    }
                    else if(recipient.is_error) {
                        Startup.WalletQueue.Transactions.Remove(recipient.address, out Recipient rec );
                        return null;
                    }
                    else
                    {
                        Console.WriteLine("Waiting for transaction");
                        Thread.Sleep(1000);
                    }
                } else {
                  // TODO errors...
                  return null;
                }
                waitCount++;
            }

            return null;
        }
    }
}
