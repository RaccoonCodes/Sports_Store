namespace SportsStore.Models
{
    public interface IOrderRepository
    {
        //collection of orders
        IQueryable<Order> Orders { get; }

        //save order to repo.
        void SaveOrder(Order order);
    }
}