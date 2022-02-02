using Recep.Models;

// namespace Recep.Wms.Services;

namespace Recep.Services;

public interface ICustomerService
{
    IList<ICustomer> GetCustomerList();

    void Add(ICustomer customer);

    void Update(ICustomer customer);

    //void Archive(ICustomer customer);

    //void Remove(ICustomer customer);

}

public class CustomerService : ICustomerService
{
    public IList<ICustomer> GetCustomerList()
    {
        throw new NotImplementedException();
    }

    public void Add(ICustomer customer)
    {
        throw new NotImplementedException();
    }

    public void Update(ICustomer customer)
    {
        throw new NotImplementedException();
    }
}
