using ExpenseTracking.Api.Domain;
using ExpenseTracking.Schema;

namespace ExpenseTracking.Api.Interfaces
{
    public interface ICreateAddressCommand
    {
        AddressRequest Address { get; }
        void ApplyOwner(Address entity);
    }
}