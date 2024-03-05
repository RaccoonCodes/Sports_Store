using Microsoft.EntityFrameworkCore;
namespace SportsStore.Models
{
    public class EFOrderRepository : IOrderRepository
    {
        private StoreDbContext context; //access database
        public EFOrderRepository(StoreDbContext ctx)
        {
            context = ctx;
        }
        //represents all orders with associated lines and products.
        public IQueryable<Order> Orders => context.Orders
        .Include(o => o.Lines)
       .ThenInclude(l => l.Product);

        //// Attach products related to order lines to the context to avoid duplicate entries.
        public void SaveOrder(Order order)
        {
            // If the order is new (OrderID is 0), add it to the
            // Orders DbSet; otherwise, update the existing order.
            context.AttachRange(order.Lines.Select(l => l.Product));
            if (order.OrderID == 0)
            {
                context.Orders.Add(order);
            }
            
            context.SaveChanges();
        }
    }
}