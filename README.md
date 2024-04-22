# Sports Store Description
## Table of Contents
1. [Introduction](#introduction)
2. [Installation](#installation)
3. [Frontend and Backend](#front-and-back-end)
4. [Models](#models)
    - [StoreDbContext](#storedbcontext)
    - [SessionCart](#sessioncart)
    - [Order](#order)
    - [IOrderRepository](#iorderrepository)
    - [EFOrderRepository](#eforderrepository)
    - [IStoreRepository](#istorerepository)
    - [EFStoreRepository](#efstorerepository)
    - [AppIdentityDbContext](#appidentitydbcontext)
5. [Views](#views)
    - [Login.cshtml](#logincshtml)
    - [Index.cshtml](#indexcshtml)
    - [Checkout.cshtml](#checkoutcshtml)
    - [Default.cshtml (Partial View)](#defaultcshtml-partial-view)
6. [Controller](#controller)
    - [Account Controller](#account-controller)
    - [Home Controller](#home-controller)
    - [Order Controller](#order-controller)
7. [Infrastructure](#infrastructure)
    - [PageLinkHelper](#pagelinkhelper)
    - [SessionExtensions](#sessionextensions)
    - [UrlExtensions](#urlextensions)
8. [Testing Unit](#testing-unit)
    - [OrderControllerTest](#ordercontrollertest)
    - [NavigationMenuViewComponentTests](#navigationmenuviewcomponenttests)
9. [Summary](#summary)

## Introduction

This project is a Sport Store application built using ASP.NET Core. It allows users to browse, add, and remove products from their shopping cart, and place orders.

There is also an administartion page that includes create, read, update, and delete (or CRUD) for managements. A login is required to access this page. Along with development of this project, There are Unit testing using xunit and Moq packages. To access admin page, on the end of the url, add "/admin" when the project is running and its credentials are

name: admin

pass: Secret123$

## Installation

1) clone this repo
2) Open Visual Studio or preferred code editor.
3) Download and Update any NuGet Package Dependencies.
4) might need to run the database migration to create the needed tables. If so, run the command:
   1) "dotnet ef migrations add Initial"
   2) "dotnet ef database update"
6) Run and navigate Page, There is an admin page and to access it, add "/admin" at the end of the URL when first running the page.

#### **All work is Finalized and commented on each files where needed. Final updated work was on 3/4/2024**

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

## Views
Like in Models, I will showcase a few views and provide short description to what they do.

### Login.cshtml
```csharp
@model LoginModel
@{
    Layout = null;
}
<body>
    <!-- Navigation bar with Sports Store branding -->
    <div class="bg-dark text-white p-2">
        <span class="navbar-brand ml-2">SPORTS STORE</span>
    </div>

    <!-- Main content section -->
    <div class="m-1 p-1">
        <!-- Validation summary for displaying all validation errors -->
        <div class="text-danger" asp-validation-summary="All"></div>
        
        <!-- Login form with Name and Password fields -->
        <form asp-action="Login" asp-controller="Account" method="post">
            <input type="hidden" asp-for="ReturnUrl" />
            <!-- Form group for the Name field -->
            <div class="form-group">
                <label asp-for="Name"></label>
                <div asp-validation-for="Name" class="text-danger"></div>
                <input asp-for="Name" class="form-control" />
            </div>

            <!-- Form group for the Password field -->
            <div class="form-group">
                <label asp-for="Password"></label>
                <div asp-validation-for="Password" class="text-danger"></div>
                <input asp-for="Password" type="password" class="form-control" />
            </div>

            <!-- Submit button for logging in -->
            <button class="btn btn-primary mt-2" type="submit">Log In</button>
        </form>
    </div>
</body>
```
- The Login view presents a form with fields for entering a username and password. It is used for user authentication, allowing users to log in to the application securely.

### Index.cshtml
```cshtml
@*Using a partial view allows the same
markup to be inserted into any view that
needs to display a summary of a product. *@

@model ProductsListViewModel

<!-- Iterating through each product in the ProductsListViewModel -->
@foreach (var p in Model?.Products ?? Enumerable.Empty<Product>())
{
    <partial name="ProductSummary" model="p" />
}

<!-- Pagination controls for navigating through the product pages -->
<div page-model="@Model?.PagingInfo" page-action="Index" page-classes-enabled="true"
     page-class="btn" page-class-normal="btn-outline-dark"
     page-class-selected="btn-primary" page-url-category="@Model?.CurrentCategory!"
     class="btn-group pull-right m-1">
</div>
```
- The Index view displays a list of products available in the store. It includes a summary of each product, allowing users to view details and add products to their shopping cart.

### Checkout.cshtml
```cshtml
@model Order

<!-- Heading for the checkout page -->
<h2>Check Out Now</h2>
<p>Please enter your details, and we'll ship your goods right away!</p>

<!-- Displaying validation summary messages in case of errors -->
<div asp-validation-summary="All" class="text-danger"></div>

<!-- Form for entering shipping details and options -->
<form asp-action="Checkout" method="post">
    <h3>Ship to</h3>
    <div class="form-group">
        <label>Name:</label><input asp-for="Name" class="form-control" />
    </div>
    <h3>Address</h3>
    <div class="form-group">
        <label>Line 1:</label><input asp-for="Line1" class="form-control" />
    </div>
    <div class="form-group">
        <label>Line 2:</label><input asp-for="Line2" class="form-control" />
    </div>
    <div class="form-group">
        <label>Line 3:</label><input asp-for="Line3" class="form-control" />
    </div>
    <div class="form-group">
        <label>City:</label><input asp-for="City" class="form-control" />
    </div>
    <div class="form-group">
        <label>State:</label><input asp-for="State" class="form-control" />
    </div>
    <div class="form-group">
        <label>Zip:</label><input asp-for="Zip" class="form-control" />
    </div>
    <div class="form-group">
        <label>Country:</label><input asp-for="Country" class="form-control" />
    </div>

    <!-- Options section -->
    <h3>Options</h3>
    <div class="checkbox">
        <label>
            <input asp-for="GiftWrap" /> Gift wrap these items
        </label>
    </div>

    <!-- Submit button to complete the order -->
    <div class="text-center">
        <input class="btn btn-primary" type="submit" value="Complete Order" />
    </div>
</form>
```
- The Checkout view provides a form for users to enter their shipping details and options when completing their order. It includes fields for the user's name, address, city, state, zip code, and country, as well as options for gift wrapping

### Default.cshtml (Partial View)
```cshtml
@model IEnumerable<string>

<!-- Container for the navigation menu using Bootstrap's grid system -->
<div class="d-grid gap-2">
    <a class="btn btn-outline-secondary" asp-action="Index"
       asp-controller="Home" asp-route-category="">
        Home
    </a>
    <!-- Loop through each category in the model and create buttons -->
    @foreach (string category in Model ?? Enumerable.Empty<string>())
    {
        <a class="btn @(category == ViewBag.SelectedCategory ? "btn-primary": "btn-outline-secondary")"
           asp-action="Index" asp-controller="Home"
           asp-route-category="@category"
           asp-route-productPage="1">
            @category
        </a>
    }
</div>
```
- The Default partial view is used for displaying navigation buttons for navigating through the application. It includes buttons for returning to the home page and for filtering products by category.

## Controller
### Account Controller
```cshtml
 
namespace SportsStore.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userMgr,
        SignInManager<IdentityUser> signInMgr)
        {
            userManager = userMgr;
            signInManager = signInMgr;
        }

        public ViewResult Login(string returnUrl)
        {
            return View(new LoginModel
            {
                ReturnUrl = returnUrl
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user =
                await userManager.FindByNameAsync(loginModel.Name);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    if ((await signInManager.PasswordSignInAsync(user,
                    loginModel.Password, false, false)).Succeeded)
                    {
                        return Redirect(loginModel?.ReturnUrl ?? "/Admin");
                    }
                }
                ModelState.AddModelError("", "Invalid name or password");
            }
            return View(loginModel);
        }
        [Authorize]
        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
    }
}

```
-  handles user authentication and authorization for the application. It provides functionalities for user login and logout.
-  Login: Displays the login form and attempts to validate the provided login credentials. If successful, signs in the user and redirects them to the specified returnUrl.
-  Logout: Signs out the current user and redirects them to the specified returnUrl or a default URL.

### Home Controller
```cshtml
namespace SportsStore.Controllers
{
    public class HomeController : Controller{

        private IStoreRepository repository;

        public int PageSize = 4; //allow to display 4 items per page
        public HomeController(IStoreRepository repo) => repository = repo;

        public ViewResult Index(string? category,int productPage = 1)
            => View(new ProductsListViewModel{
                Products = repository.Products.Where(
                    p=> category == null || p.Category == category)
                .OrderBy(p => p.ProductID)
                .Skip((productPage - 1) * PageSize).Take(PageSize),

                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ? repository.Products.Count()
                    : repository.Products
                    .Where(e=> e.Category==category).Count()//else, count for specific category
                },
                CurrentCategory = category
            });
    }
}

```
- handles requests to the home page, retrieves and filters products from the repository based on category and pagination.

- Index: Retrieves a list of products based on the specified category, orders them, and applies pagination using the Skip and Take methods.

### Order Controller
```csharp
namespace SportsStore.Controllers
{
    public class OrderController : Controller
    {
        private IOrderRepository repository;
        private Cart cart;

        //dependency injection for IOrderRepository and Cart
        public OrderController(IOrderRepository repoService, Cart cartService)
        {
            repository = repoService;
            cart = cartService;
        }
        public ViewResult Checkout() => View(new Order());

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }
            if (ModelState.IsValid)
            {
                order.Lines = cart.Lines.ToArray();
                repository.SaveOrder(order);
                cart.Clear();
                return RedirectToPage("/Completed", new { orderId = order.OrderID });
            }
            else
            {
                return View();
            }
        }
    }
}
```
- allows users to add products to their cart, proceed to checkout, and place orders. It interacts with the repository to save order information and clears the cart after a successful order placement.
- Checkout: Renders the checkout view and allows users to enter their shipping details.
- Checkout (HttpPost): Checks if the shopping cart is empty. If not, populates the order information and saves it to the repository. Clears the cart after a successful order placement.

## Infrastructure
Infrastures are utilitties and helper classes that support the main application of this project. Below are the classes that help out the project.
### PageLinkHelper
```csharp
namespace SportsStore.Infrastructure
{
    //specifically for div element and attribute page-model
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }
        public PagingInfo? PageModel { get; set; } // paging info model
        public string? PageAction { get; set; } // action used to generate URL

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; }
            = new Dictionary<string, object>();

        //// allow certain CSS to be applied 
        public bool PageClassesEnabled { get; set; } = false; 
        public string PageClass { get; set; } = String.Empty;
        public string PageClassNormal { get; set; } = String.Empty;
        public string PageClassSelected { get; set; } = String.Empty;


        //applies custom css based on the current page
        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            if (ViewContext != null && PageModel != null)
            {
                IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
                TagBuilder result = new TagBuilder("div");
                for (int i = 1; i <= PageModel.TotalPages; i++)
                {
                    TagBuilder tag = new TagBuilder("a");
                    PageUrlValues["productPage"] = i;
                    tag.Attributes["href"] = urlHelper.Action(PageAction,
                    PageUrlValues);

                    if (PageClassesEnabled)
                    {
                        tag.AddCssClass(PageClass);
                        tag.AddCssClass(i == PageModel.CurrentPage
                         ? PageClassSelected : PageClassNormal);
                    }
                    tag.InnerHtml.Append(i.ToString());
                    result.InnerHtml.AppendHtml(tag);
                }
                output.Content.AppendHtml(result.InnerHtml);
            }
        }
    }
}

```
- custom Tag Helper designed to generate HTML for page navigation links based on the Pagination.
- process: Generates HTML for navigation links based on PageModel and Page Action. It also allows css to be applied based on the selected page.

### SessionExtensions
```csharp
namespace SportsStore.Infrastructure
{
    public static class SessionExtensions
    {
        public static void SetJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetJson<T>(this ISession session, string key)
        {
            var sessionData = session.GetString(key);
            return sessionData == null
            ? default(T) : JsonSerializer.Deserialize<T>(sessionData);
        }
    }
}

```
- class provides extension methods for Serializing and seserializing object into JSON format for Session storing
### UrlExtensions
```csharp
namespace SportsStore.Infrastructure
{
    public static class UrlExtensions{
        public static string PathAndQuery(this HttpRequest request) =>
            request.QueryString.HasValue ? $"{request.Path}{request.QueryString}"
            : request.Path.ToString();
    }
}
```
- The UrlExtensions class provides an extension method PathAndQuery for the HttpRequest object. This method concatenates the path and the query string if present. If there is no query string, it returns the path as a string.

# Testing Unit
The Unit Testing Used are Xunit and Moq. Below I Will showcase two Different unit testing.

### OrderControllerTest
This file contains comprehensive unit tests for the OrderController class, which manages the checkout process. It ensures that the checkout functionality behaves appropriately under different scenarios:

```csharp
/**Other Testing Methods***/
// Test to check if checkout is not possible with an empty cart
[Fact]
public void Cannot_Checkout_Empty_Cart()
{
    // Arrange - create a mock repository
    Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
    // Arrange - create an empty cart
    Cart cart = new Cart();
    // Arrange - create the order
    Order order = new Order();
    // Arrange - create an instance of the controller
    OrderController target = new OrderController(mock.Object, cart);
    // Act
    ViewResult? result = target.Checkout(order) as ViewResult;
    // Assert - check that the order hasn't been stored
    mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
    // Assert - check that the method is returning the default view
    Assert.True(string.IsNullOrEmpty(result?.ViewName));
    // Assert - check that I am passing an invalid model to the view
    Assert.False(result?.ViewData.ModelState.IsValid);
}
/**Other Testing Methods***/
```
- This test verifies that checkout is not possible when the cart is empty. It sets up a mock repository and an empty cart, then checks that the order is not stored and the view returned is the default view with invalid model state.

```csharp
/**Other Testing Methods***/
[Fact]
public void Can_Checkout_And_Submit_Order()
{
    // Arrange - create a mock order repository
    Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
    // Arrange - create a cart with one item
    Cart cart = new Cart();
    cart.AddItem(new Product(), 1);
    // Arrange - create an instance of the controller
    OrderController target = new OrderController(mock.Object, cart);
    // Act - try to checkout
    RedirectToPageResult? result =
    target.Checkout(new Order()) as RedirectToPageResult;
    // Assert - check that the order has been stored
    mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Once);
    // Assert - check that the method is redirecting to the Completed action
    Assert.Equal("/Completed", result?.PageName);
}
/**Other Testing Methods***/
```
- This test ensures that checkout is possible, and an order can be submitted successfully. It sets up a mock order repository, creates a cart with one item, and checks that the order is stored correctly and the method redirects to the completed action.

```csharp
/**Other Testing Methods***/
 [Fact]
 public void Cannot_Checkout_Invalid_ShippingDetails()
 {
     // Arrange - create a mock order repository
     Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
     // Arrange - create a cart with one item
     Cart cart = new Cart();
     cart.AddItem(new Product(), 1);
     // Arrange - create an instance of the controller
     OrderController target = new OrderController(mock.Object, cart);
     // Arrange - add an error to the model
     target.ModelState.AddModelError("error", "error");
     // Act - try to checkout
     ViewResult? result = target.Checkout(new Order()) as ViewResult;
     // Assert - check that the order hasn't been passed stored
     mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
     // Assert - check that the method is returning the default view
     Assert.True(string.IsNullOrEmpty(result?.ViewName));
     // Assert - check that I am passing an invalid model to the view
     Assert.False(result?.ViewData.ModelState.IsValid);
 }
/**Other Testing Methods***/
```
- This test confirms that checkout is not possible with invalid shipping details. It mocks the order repository and creates a cart with one item. It then adds an error to the model and checks that the order is not stored and the view returned is the default view with invalid model state.

### NavigationMenuViewComponentTests
This file contains meticulous unit tests for the NavigationMenuViewComponent class, responsible for rendering the navigation menu with product categories. These tests validate that the navigation menu behaves correctly:
```csharp
/**Other Testing Methods***/
[Fact]
public void Can_Select_Categories()
{
    // Arrange
    Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
    mock.Setup(m => m.Products).Returns((new Product[] {
     new Product {ProductID = 1, Name = "P1",Category = "Apples"},
     new Product {ProductID = 2, Name = "P2",Category = "Apples"},
     new Product {ProductID = 3, Name = "P3",Category = "Plums"},
     new Product {ProductID = 4, Name = "P4",Category = "Oranges"},
     }).AsQueryable<Product>());
    NavigationMenuViewComponent target =
    new NavigationMenuViewComponent(mock.Object);

    // Act = get the set of categories
    string[] results = ((IEnumerable<string>?)(target.Invoke()
    as ViewViewComponentResult)?.ViewData?.Model
    ?? Enumerable.Empty<string>()).ToArray();

    // Assert
    Assert.True(Enumerable.SequenceEqual(new string[] { "Apples",
        "Oranges", "Plums" }, results));
}
/**Other Testing Methods***/
```
- This test confirms that the view component can select categories accurately. It sets up a mock repository with sample products and asserts that the categories are selected correctly based on the product data.
```csharp
/**Other Testing Methods***/
[Fact]
public void Indicates_Selected_Category()
{
    // Arrange
    string categoryToSelect = "Apples";
    Mock<IStoreRepository> mock = new Mock<IStoreRepository>();
    mock.Setup(m => m.Products).Returns((new Product[] {
     new Product {ProductID = 1, Name = "P1", Category = "Apples"},
     new Product {ProductID = 4, Name = "P2", Category = "Oranges"},
     }).AsQueryable<Product>());
    NavigationMenuViewComponent target =
    new NavigationMenuViewComponent(mock.Object);
    
    target.ViewComponentContext = new ViewComponentContext
    {
        ViewContext = new ViewContext
        {
            RouteData = new Microsoft.AspNetCore.Routing.RouteData()
        }
    };
    target.RouteData.Values["category"] = categoryToSelect;
   
    // Action
    string? result = (string?)(target.Invoke()
    as ViewViewComponentResult)?.ViewData?["SelectedCategory"];

    // Assert
    Assert.Equal(categoryToSelect, result);
}
/**Other Testing Methods***/
```
- This test verifies that the selected category is indicated correctly in the navigation menu. It sets up a mock repository with sample products, specifies a selected category, and checks that the selected category is correctly indicated in the rendered navigation menu.

# Summary
The Sports Store application is a comprehensive e-commerce platform built using ASP.NET Core, offering a wide range of sports products for users to browse, add to cart, and purchase. The project utilizes various frontend and backend technologies, including ASP.NET Core MVC, Entity Framework Core, Identity Framework, Bootstrap 5, and Font Awesome for design and functionality.

## Key Features:
- User Authentication and Authorization: The application includes user authentication and authorization functionalities provided by Identity Framework, ensuring secure access to user-specific features and data.

- Product Management: Users can browse through a collection of sports products, view product details, and add products to their shopping cart.

- Shopping Cart: The application provides a seamless shopping experience with a persistent shopping cart, allowing users to add, remove, and update items in their cart before checkout.

- Order Placement: Users can proceed to checkout, enter their shipping details, and place orders. Orders are securely stored in the database for future reference.

- Admin Panel: The application includes an administration panel accessible to authorized users, allowing for CRUD (Create, Read, Update, Delete) operations on products and orders.

- Unit Testing: Unit tests are implemented using Xunit and Moq, ensuring the reliability and correctness of the application's functionalities.



  
