using System.Text.Json.Serialization;
using SportsStore.Infrastructure;
namespace SportsStore.Models
{
    public class SessionCart : Cart
    {
        //retrieves the cart from the session.
        public static Cart GetCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>()
            .HttpContext?.Session;
            SessionCart cart = session?.GetJson<SessionCart>("Cart")
            ?? new SessionCart();
            cart.Session = session;
            return cart;
        }
        //storing in session
        [JsonIgnore]
        public ISession? Session { get; set; }

        // Override AddItem method to update the session when items are added.
        public override void AddItem(Product product, int quantity)
        {
            base.AddItem(product, quantity);
            Session?.SetJson("Cart", this);
        }

        // Override RemoveLine method to update the session when lines are removed.
        public override void RemoveLine(Product product)
        {
            base.RemoveLine(product);
            Session?.SetJson("Cart", this);
        }

        // Override Clear method to update the session when the cart is cleared.
        public override void Clear()
        {
            base.Clear();
            Session?.Remove("Cart");
        }
    }
}
