using System;
using System.Text;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Util;
using Nethereum.Contracts;
using Nethereum.Contracts.Extensions;
using Nethereum.Contracts.CQS;
using System.Numerics;
using System.Collections.Generic;
using Dividendos.Entidades;

using System.Linq;
using Nethereum.Hex.HexTypes;

namespace PruebasNethereum
{
    class Program
    {



        [Function("transfer", "bool")]
        public class TransferFunction : FunctionMessage
        {
            [Parameter("address", "_to", 1)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 2)]
            public BigInteger TokenAmount { get; set; }
        }


        [Function("balanceOf", "uint256")]
        public class BalanceOfFunction : FunctionMessage
        {
            [Parameter("address", "_account", 1)]
            public string Account { get; set; }
        }




        [Event("Transfer")]
        public class TransferEventDTO : IEventDTO
        {
            [Parameter("address", "_from", 1, true)]
            public string From { get; set; }

            [Parameter("address", "_to", 2, true)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 3, false)]
            public BigInteger Value { get; set; }
        }





        ///**** END CONTRACT DEFINITION
        public static async Task Main(string[] args)
        {


            
            var url = "@";
            var privateKey = "@";
            var account = new Account(privateKey);


            var web3 = new Web3(account, url);
            var decimales = 1000000000000000000;
            var total_supply = 33000000;
            var dividendos_entregar = 1000000;



            var contractAddress = "0x93c59ae42d899e42f330c57d765aaec525b82db2";
            var contractDividendos = "0x30711e05E13Cbf93659DF529767A321c3d34A7C6";





            var transferEventHandler = web3.Eth.GetEvent<TransferEventDTO>(contractAddress);//contractAddress


            var filterAllTransferEventsForContract = transferEventHandler.CreateFilterInput();

            var allTransferEventsForContract = await transferEventHandler.GetAllChanges(filterAllTransferEventsForContract);



            using (var DbContext = new pruebasContext())
            {
                long? cantida_actual;
                foreach (var item in allTransferEventsForContract)
                {



                    //No existe la ruta, registro nuevas direcciones y balance
                    if (!addressExists(item.Event.To))
                    {

                        var address = new TAddressContract
                        {
                            Account = item.Event.To,
                            Value = 0
                        };


                        DbContext.TAddressContracts.Add(address);
                        DbContext.SaveChanges();

                    }
                    //****************************************************************






                    // ******************      Actualizar Saldos      ***************** 
                    var balanceOfFunctionMessage = new BalanceOfFunction()
                    {
                        Account = item.Event.To
                    };

                    var queryHandler = web3.Eth.GetContractQueryHandler<BalanceOfFunction>();

                    var address_balance_contract = await queryHandler.QueryAsync<BigInteger>(contractAddress, balanceOfFunctionMessage);


                    var block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(item.Log.BlockNumber));

                    cantida_actual = (long?)(address_balance_contract / decimales);



                    // Comparar saldo en Blokchain con BD 
                    var saldoBd = DbContext.TAddressContracts.FirstOrDefault(x => x.Account == item.Event.To);

                    if (saldoBd != null)
                    {
                        if (saldoBd.Value != cantida_actual)
                        {
                            saldoBd.Value = cantida_actual;
                            DbContext.SaveChanges();
                        }
                    }
                    //*****************************************************************
                }




                var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();

                var holder = DbContext.TAddressContracts.Where(x => x.Account != "0x4431e577936abc3631139f22860ea6acc0f5dca7").ToList();

                double porcentaje = 0;
                BigInteger dividendo = 0;

                if (holder != null)
                {
                    foreach (var item in holder)
                    {
                        porcentaje = (double)(item.Value * 100) / total_supply;
                        dividendo = (BigInteger)((dividendos_entregar * porcentaje) * decimales);


                        var transfer = new TransferFunction
                        {
                            To = item.Account,
                            TokenAmount = dividendo
                        };

                        var transactionReceipt = await transferHandler.SendRequestAndWaitForReceiptAsync(contractDividendos, transfer);

                    }
                }

            }


            //¿existe registro de direccion?
            static bool addressExists(string _address)
            {
                try
                {
                    using (var DbContext = new pruebasContext())
                    {
                        var addrres = DbContext.TAddressContracts.FirstOrDefault(x => x.Account == _address);


                        if (addrres != null)
                        {
                            if (!String.IsNullOrEmpty(addrres.Account))
                            {
                                return true;
                            }

                        }

                    }
                }
                catch (Exception)
                {
                    return false;
                    throw;
                }
                return false;
            }


            //comparacion de saldos 
            static bool saldoDiferente(string _address, long? _value)
            {
                try
                {
                    using (var DbContext = new pruebasContext())
                    {
                        var saldoIgual = DbContext.TAddressContracts.FirstOrDefault(x => x.Account == _address && x.Value == _value);
                        if (saldoIgual != null)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                    throw;
                }
                return false;
            }


            static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
            {
                // Unix timestamp is seconds past epoch
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
                return dtDateTime;
            }

        }
    }
}
