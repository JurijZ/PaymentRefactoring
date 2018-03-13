using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using System;
using System.Configuration;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private IDataStore _datastore;

        public PaymentService(IDataStore datastore)
        {
            _datastore = datastore;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            Account account = GetAccount(request.DebitorAccountNumber) ?? null;

            var result = new MakePaymentResult();

            if (IsPaymentAllowed(request, account))
            {
                try
                {
                    account.Balance -= request.Amount;
                    // A call to do the actual transfer should be here
                    result.Success = true;
                    _datastore.UpdateAccount(account);                    
                }
                catch(Exception e)
                {
                    Console.WriteLine("An error occurred while making the payment: '{0}'", e);
                }                
            }
            else
            {
                result.Success = false;
            }

            return result;
        }
        
        private bool IsPaymentAllowed(MakePaymentRequest request, Account account)
        {            
            
            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                    if (account == null)
                    {
                        return false;
                    }
                    else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        return false;
                    }
                    else if (account.Status == AccountStatus.Live)
                    {
                        return true;
                    }
                    break;

                case PaymentScheme.FasterPayments:
                    if (account == null)
                    {
                        return false;
                    }
                    else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
                    {
                        return false;
                    }
                    else if (account.Balance < request.Amount  && account.Status == AccountStatus.Live)
                    {
                        return true;
                    }
                    break;

                case PaymentScheme.Chaps:
                    if (account == null)
                    {
                        return false;
                    }
                    else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
                    {
                        return false;
                    }
                    else if (account.Status == AccountStatus.Live)
                    {
                        return true;
                    }
                    break;

                default:
                    return false;                    
            }
            
            return false;
        }

        private Account GetAccount(string DebitorAccountNumber)
        {            
            Account account = _datastore.GetAccount(DebitorAccountNumber);

            return account;
        }
    }
}
