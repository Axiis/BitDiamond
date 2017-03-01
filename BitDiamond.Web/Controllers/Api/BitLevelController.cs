using Axis.Luna;
using Axis.Luna.Extensions;
using BitDiamond.Core.Models;
using BitDiamond.Core.Services;
using BitDiamond.Core.Utils;
using BitDiamond.Web.Controllers.Api.BitLevelControllerModels;
using BitDiamond.Web.Infrastructure.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Web.Controllers.Api
{
    public class BitLevelController : ApiController
    {
        private IBitLevelManager _bitlevel = null;

        public BitLevelController(IBitLevelManager bitlevel)
        {
            ThrowNullArguments(() => bitlevel);

            this._bitlevel = bitlevel;
        }

        [HttpGet, Route("api/bit-levels/bitcoin-addresses")]
        public IHttpActionResult GetAllBitcoinAddresses()
        => _bitlevel.GetAllBitcoinAddresses().OperationResult(Request);

        [HttpGet, Route("api/bit-levels/bitcoin-addresses/active")]
        public IHttpActionResult GetActiveBitcoinAddresses()
        => _bitlevel.GetActiveBitcoinAddress().OperationResult(Request);

        [HttpPost, Route("api/bit-levels/bitcoin-addresses")]
        public IHttpActionResult AddBitcoinAddress([FromBody] BitcoinAddress address)
        => _bitlevel.AddBitcoindAddress(address).OperationResult(Request);

        [HttpPut, Route("api/bit-levels/bitcoin-addresses/activate")]
        public IHttpActionResult ActivateAddress([FromBody] BitcoinAddressArgs args)
        => _bitlevel.ActivateAddress(args?.Id ?? 0).OperationResult(Request);

        [HttpPut, Route("api/bit-levels/bitcoin-addresses/deactivate")]
        public IHttpActionResult DeactivateAddress([FromBody] BitcoinAddressArgs args)
        => _bitlevel.DeactivateAddress(args?.Id ?? 0).OperationResult(Request);

        [HttpPut, Route("api/bit-levels/bitcoin-addresses/verify")]
        public IHttpActionResult VerifyAddress([FromBody] BitcoinAddressArgs args)
        => _bitlevel.VerifyAddress(args?.Id ?? 0).OperationResult(Request);



        [HttpPost, Route("api/bit-levels/cycles")]
        public IHttpActionResult Upgrade()
        => _bitlevel.Upgrade().OperationResult(Request);

        [HttpPut, Route("api/bit-levels/transactions/current")]
        public IHttpActionResult UpdateTransactionHash([FromBody] TransactionArgs arg)
        => _bitlevel.UpdateTransactionHash(arg.Hash).OperationResult(Request);

        //[HttpPut, Route("api/bit-levels/transactions/current/confirm")]
        //public IHttpActionResult ConfirmUpgradeDonation()
        //=> _bitlevel.ConfirmUpgradeDonnation().OperationResult(Request);

        [HttpGet, Route("api/bit-levels/cycles/current")]
        public IHttpActionResult CurrentUserLevel()
        => _bitlevel.CurrentUserLevel().OperationResult(Request);

        [HttpGet, Route("api/bit-levels/cycles")]
        public IHttpActionResult GetBitLevelById(string data)
        => Operation.Try(() => ThrowIfFail(() => Encoding.UTF8.GetString(Convert.FromBase64String(data)), ex => new MalformedApiArgumentsException()))
            .Then(_jopr => ThrowIfFail(() => JsonConvert.DeserializeObject<BitLevelArgs>(_jopr.Result, Constants.Misc_DefaultJsonSerializerSettings), ex => new MalformedApiArgumentsException()))
            .Then(argopr => _bitlevel.GetBitLevelById(argopr.Result.Id))
            .OperationResult(Request);


        [HttpGet, Route("api/bit-levels/cycles/history")]
        public IHttpActionResult UserUpgradeHistory()
        => _bitlevel.UserUpgradeHistory().OperationResult(Request);


        [HttpPut, Route("api/bit-levels/transactions/receiver-confirmation")]
        public IHttpActionResult ReceiverConfirmation(TransactionArgs arg)
        => _bitlevel.ReceiverConfirmation(arg?.Hash).OperationResult(Request);


        [HttpGet, Route("api/bit-levels/upgrade-fees/{level}")]
        public IHttpActionResult GetUpgradeFee(int level)
        => _bitlevel.GetUpgradeFee(level).OperationResult(Request);


        [HttpGet, Route("api/bit-levels/transactions/receivers")]
        public IHttpActionResult GetUpgradeTransactionReceiver(string data)
        => Operation.Try(() => ThrowIfFail(() => Encoding.UTF8.GetString(Convert.FromBase64String(data)), ex => new MalformedApiArgumentsException()))
            .Then(_jopr => ThrowIfFail(() => JsonConvert.DeserializeObject<BitcoinAddressArgs>(_jopr.Result, Constants.Misc_DefaultJsonSerializerSettings), ex => new MalformedApiArgumentsException()))
            .Then(argopr => _bitlevel.GetUpgradeTransactionReceiver(argopr.Result.Id))
            .OperationResult(Request);


        [HttpGet, Route("api/bit-levels/transactions/current")]
        public IHttpActionResult GetCurrentUpgradeTransaction()
        => _bitlevel.GetCurrentUpgradeTransaction().OperationResult(Request);
    }

    namespace BitLevelControllerModels
    {
        public class TransactionArgs
        {
            public string Hash { get; set; }
        }

        public class BitLevelArgs
        {
            public long Id { get; set; }
        }

        public class BitcoinAddressArgs
        {
            public long Id { get; set; }
        }
    }
}
