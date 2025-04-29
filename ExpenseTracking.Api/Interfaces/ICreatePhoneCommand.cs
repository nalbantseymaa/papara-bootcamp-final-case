using ExpenseTracking.Api.Domain;
using ExpenseTracking.Schema;

namespace ExpenseTracking.Api.Interfaces
{
    public interface ICreatePhoneCommand
    {
        PhoneRequest Phone { get; }
        void ApplyOwner(Phone entity);
    }
}