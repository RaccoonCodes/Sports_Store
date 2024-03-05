namespace SportsStore.Models
{
    public class Cart
    {
        // a list of objects where each object represents a product in the shopping cart
        public List<CartLine> Lines { get; set; } = new List<CartLine>();

        // adds items to cart
        public virtual void AddItem(Product product, int quantity)
        {
            //if the item is already in the cart
            CartLine? line = Lines
            .Where(p => p.Product.ProductID == product.ProductID)
            .FirstOrDefault();

            //if it is not in the cart, add it 
            if (line == null)
            {
                Lines.Add(new CartLine
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        //removes product line from cart
        public virtual void RemoveLine(Product product) =>
        Lines.RemoveAll(l => l.Product.ProductID == product.ProductID);

        //calculates total sum in the cart
        public decimal ComputeTotalValue() =>
        Lines.Sum(e => e.Product.Price * e.Quantity);

        //clears all items from cart
        public virtual void Clear() => Lines.Clear();
    }
    public class CartLine
    {
        public int CartLineID { get; set; }
        public Product Product { get; set; } = new();
        public int Quantity { get; set; }
    }
}