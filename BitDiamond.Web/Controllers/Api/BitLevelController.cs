﻿using Axis.Luna;
using Axis.Luna.Extensions;
using BitDiamond.Core.Models;
using BitDiamond.Core.Services;
using BitDiamond.Core.Utils;
using BitDiamond.Web.Controllers.Api.BitLevelControllerModels;
using BitDiamond.Web.Infrastructure.Exceptions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Web.Http;
using static Axis.Luna.Extensions.ExceptionExtensions;
using BitDiamond.Web.Infrastructure.Utils;

namespace BitDiamond.Web.Controllers.Api
{
    public class BitLevelController : ApiController
    {
        private IBitLevelManager _bitlevel = null;

        public BitLevelController(IBitLevelManager bitlevel)
        {
            ThrowNullArguments(() => bitlevel);

            _bitlevel = bitlevel;
        }

        [HttpGet, Route("api/bit-levels/bitcoin-addresses")]
        public IHttpActionResult GetAllBitcoinAddresses()
        => this.Log(() => _bitlevel.GetAllBitcoinAddresses().OperationResult(Request));

        [HttpGet, Route("api/bit-levels/bitcoin-addresses/referenced")]
        public IHttpActionResult GetReferencedAddresses()
        => this.Log(() => _bitlevel.GetReferencedAddresses().OperationResult(Request));

        [HttpDelete, Route("api/bit-levels/bitcoin-addresses/unreferenced")]
        public IHttpActionResult DeleteUnreferencedAddresses(string data)
        => this.Log(() => data.DecodeUrlData<BitcoinAddressArgs>(Encoding.UTF8)
               .Then(opr => _bitlevel.DeleteUnreferencedAddress(opr.Result.Id))
               .OperationResult(Request));

        [HttpGet, Route("api/bit-levels/bitcoin-addresses/active")]
        public IHttpActionResult GetActiveBitcoinAddresses()
        => this.Log(() => _bitlevel.GetActiveBitcoinAddress().OperationResult(Request));

        [HttpPost, Route("api/bit-levels/bitcoin-addresses")]
        public IHttpActionResult AddBitcoinAddress([FromBody] BitcoinAddress address)
        => this.Log(() => Operation.Try(() => address.ThrowIfNull(new MalformedApiArgumentsException()))
               .Then(opr => _bitlevel.AddBitcoindAddress(address))
               .OperationResult(Request));

        [HttpPut, Route("api/bit-levels/bitcoin-addresses/activate")]
        public IHttpActionResult ActivateAddress([FromBody] BitcoinAddressArgs args)
        => this.Log(() => Operation.Try(() => args.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _bitlevel.ActivateAddress(args.Id))
            .OperationResult(Request));

        [HttpPut, Route("api/bit-levels/bitcoin-addresses/deactivate")]
        public IHttpActionResult DeactivateAddress([FromBody] BitcoinAddressArgs args)
        => this.Log(() => Operation.Try(() => args.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _bitlevel.DeactivateAddress(args.Id))
            .OperationResult(Request));

        [HttpPut, Route("api/bit-levels/bitcoin-addresses/verify")]
        public IHttpActionResult VerifyAddress([FromBody] BitcoinAddressArgs args)
        => this.Log(() => Operation.Try(() => args.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _bitlevel.VerifyAddress(args.Id))
            .OperationResult(Request));



        [HttpPost, Route("api/bit-levels/cycles")]
        public IHttpActionResult Upgrade()
        => this.Log(() => _bitlevel.Upgrade().OperationResult(Request));

        [HttpPost, Route("api/bit-levels/cycles/promote")]
        public IHttpActionResult Promote([FromBody] PromotionArgs args)
        => this.Log(() => _bitlevel.Promote(args?.TargetUser, args?.Steps ?? 0, Request.Headers.GetValues("Haxh")?.FirstOrDefault())
               .OperationResult(Request));

        [HttpPost, Route("api/bit-levels/cycles/demote")]
        public IHttpActionResult Demote([FromBody] PromotionArgs args)
        => this.Log(() => _bitlevel.Promote(args?.TargetUser, args?.Steps ?? 0, Request.Headers.GetValues("Haxh")?.FirstOrDefault())
               .OperationResult(Request));

        [HttpPut, Route("api/bit-levels/transactions/current")]
        public IHttpActionResult UpdateTransactionHash([FromBody] TransactionArgs args)
        => this.Log(() => Operation.Try(() => args.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _bitlevel.VerifyAndSaveTransactionHash(args.Hash))
            .OperationResult(Request));

        [HttpPut, Route("api/bit-levels/transactions/current/confirm")]
        public IHttpActionResult ConfirmUpgradeDonation()
        => this.Log(() => _bitlevel.ConfirmUpgradeDonation().OperationResult(Request));

        [HttpGet, Route("api/bit-levels/cycles/current")]
        public IHttpActionResult CurrentUserLevel()
        => this.Log(() => _bitlevel.CurrentUserLevel().OperationResult(Request));

        [HttpGet, Route("api/bit-levels/cycles")]
        public IHttpActionResult GetBitLevelById(string data)
        => this.Log(() => data.DecodeUrlData<BitcoinAddressArgs>(Encoding.UTF8)
               .Then(opr => _bitlevel.GetBitLevelById(opr.Result.Id))
               .OperationResult(Request));


        [HttpGet, Route("api/bit-levels/cycles/history")]
        public IHttpActionResult UserUpgradeHistory()
        => this.Log(() => _bitlevel.UserUpgradeHistory().OperationResult(Request));


        [HttpGet, Route("api/bit-levels/cycles/history/pages")]
        public IHttpActionResult UserUpgradeHistory(string data)
        => this.Log(() => data.DecodeUrlData<SequencePageArgs>(Encoding.UTF8)
            .Then(argopr => _bitlevel.PagedUserUpgradeHistory(argopr.Result.PageSize, argopr.Result.PageIndex))
            .OperationResult(Request));


        [HttpGet, Route("api/bit-levels/upgrade-fees/{level}")]
        public IHttpActionResult GetUpgradeFee(int level)
        => this.Log(() => _bitlevel.GetUpgradeFee(level).OperationResult(Request));


        [HttpGet, Route("api/bit-levels/transactions/current")]
        public IHttpActionResult GetCurrentUpgradeTransaction()
        => this.Log(() => _bitlevel.GetCurrentUpgradeTransaction().OperationResult(Request));
    }

    namespace BitLevelControllerModels
    {
        public class TransactionArgs
        {
            public string Hash { get; set; }
            public long Id { get; set; }
        }

        public class BitLevelArgs
        {
            public long Id { get; set; }
        }

        public class BitcoinAddressArgs
        {
            public long Id { get; set; }
        }

        public class SequencePageArgs
        {
            public int PageSize { get; set; }
            public int PageIndex { get; set; }
        }

        public class PromotionArgs
        {
            public string TargetUser { get; set; }
            public int Steps { get; set; }
        }
    }
}
