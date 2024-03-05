/*
 * View Component that passes Cart to view method to generate parts
 * of HTML that will be in the layout.
 * In other words it is designed to render a view that displays 
 * information related to a shopping cart 
 */

using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
namespace SportsStore.Components
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private Cart cart;
        public CartSummaryViewComponent(Cart cartService)
        {
            cart = cartService;
        }
        public IViewComponentResult Invoke()
        {
            return View(cart);
        }
    }
}
