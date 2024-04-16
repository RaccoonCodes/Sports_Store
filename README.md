# Sports Store Description

## Introduction

This project is a Sport Store application built using ASP.NET Core. It allows users to browse, add, and remove products from their shopping cart, and place orders.

There is also an administartion page that includes create, read, update, and delete (or CRUD) for managements. A login is required to access this page. Along with development of this project, There are Unit testing using xunit and Moq packages. To access it, on the end of the url, add "/admin" when the project is running and its credentials are

name: admin

pass: Secret123$


**All work is Finalized and commented on each files where needed. Final updated work was on 3/4/2024**

 ## Front and back End
- ASP.NET Core MVC: The web framework used for building the application.
- Entity Framework Core: Used for data access and database management.
- Identity Framework: Provides user authentication and authorization functionalities.
- JsonSerializer: For serializing and deserializing objects to and from JSON format.
- Bootstrap 5 and Font Awesome for Front-end Design

## Models
I will showcase a few Models and a quick explanation to behind the code.

### StoreDbContext
```csharp
 public class StoreDbContext: DbContext{
     public StoreDbContext(DbContextOptions<StoreDbContext>options) : base(options)
     { }

     // DbSet<Product> represents the collection of products in the database.
     public DbSet<Product> Products=> Set<Product>();

     // DbSet<Order> represents the collection of orders in the database.
     public DbSet<Order> Orders => Set<Order>();
 }
```
- Represents the database context for the Sport Store application.
- Contains DbSet<Product> and DbSet<Order> properties to represent the collections of products and orders in the database.

### SessionCart
```csharp
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
```
- Inherits from Cart class.
- Manages the user's shopping cart and stores it in the session.
- Overrides methods for adding, removing, and clearing items in the cart to update the session accordingly.

### Order

```csharp
public class Order
{
    [BindNever]
    public int OrderID { get; set; }
    [BindNever]
    public ICollection<CartLine> Lines { get; set; } = new List<CartLine>();
    [Required(ErrorMessage = "Please enter a name")]
    public string? Name { get; set; }
    [Required(ErrorMessage = "Please enter the first address line")]
    public string? Line1 { get; set; }
    public string? Line2 { get; set; }
    public string? Line3 { get; set; }
    [Required(ErrorMessage = "Please enter a city name")]
    public string? City { get; set; }
    [Required(ErrorMessage = "Please enter a state name")]
    public string? State { get; set; }
    public string? Zip { get; set; }
    [Required(ErrorMessage = "Please enter a country name")]
    public string? Country { get; set; }
    public bool GiftWrap { get; set; }
    [BindNever]
    public bool Shipped { get; set; }
}
```
- Represents an order in the Sport Store application.
- Contains properties for order details such as name, address, city, state, zip, country, and gift wrap option.
- Uses data annotations for validation.

### IOrderRepository

```csharp
public interface IOrderRepository
{
    //collection of orders
    IQueryable<Order> Orders { get; }

    //save order to repo.
    void SaveOrder(Order order);
}
```
- Interface for managing orders.
- Defines methods for accessing and saving orders.

### EFOrderRepository
```csharp
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
```
- Implementation of IOrderRepository interface.
- Uses Entity Framework Core to access and manage orders in the database.
- Includes methods for retrieving and saving orders.

### IStoreRepository

```csharp
public interface IStoreRepository{
    IQueryable<Product> Products { get; }
    void SaveProduct(Product p);
    void CreateProduct(Product p);
    void DeleteProduct(Product p);
}
```
- Interface for managing products.
- Defines methods for accessing, saving, creating, and deleting products.

### EFStoreRepository
```csharp
 public class EFStoreRepository : IStoreRepository
 {
     private StoreDbContext context;
     public EFStoreRepository(StoreDbContext ctx)
     {
         context = ctx;
     }
     //represents all products in the database.
     public IQueryable<Product> Products => context.Products;

     public void CreateProduct(Product p)
     {
         context.Add(p);
         context.SaveChanges();
     }
     public void DeleteProduct(Product p)
     {
         context.Remove(p);
         context.SaveChanges();
     }
     public void SaveProduct(Product p)
     {
         context.SaveChanges();
     }
 }
```
- Implementation of IStoreRepository interface.
- Uses Entity Framework Core to access and manage products in the database.
- Includes methods for retrieving, creating, updating, and deleting products.

### AppIdentityDbContext
```csharp
 public class AppIdentityDbContext : IdentityDbContext<IdentityUser>
 {
     public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
     : base(options) { }
 }
```
- Represents the database context for user authentication and authorization.
- Inherits from IdentityDbContext<IdentityUser> provided by Identity Framework.

**README FILE IN PROGRESS
