/*
 *  users can add products to their cart, proceed to checkout,
 *  and place orders. The order information is saved in the repository, 
 *  and the cart is cleared after a successful order placement.
 */

using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
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
        //render checkout view tied with Checkout.cshtml
        public ViewResult Checkout() => View(new Order());

        //checks if the shopping cart is empty.
        //If it is, an error message is added to the model state.
        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }
            //if valid, populate the information into cart and order saved into the repo.
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