/*
 * this controller handles requests to the home page, 
 * retrieves and filters products from the repository 
 * based on category and pagination
 */

using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
    public class HomeController : Controller{

        private IStoreRepository repository;

        public int PageSize = 4; //allow to display 4 items per page
        public HomeController(IStoreRepository repo) => repository = repo;

        // it retrieves a list of products based on the specified category,
        // orders them, and applies pagination using the Skip and Take methods.

        public ViewResult Index(string? category,int productPage = 1)
            => View(new ProductsListViewModel{
                Products = repository.Products.Where(
                    p=> category == null || p.Category == category)
                .OrderBy(p => p.ProductID)
                //ex: if on page 1, the skip is 0 and take is 4. prints products (1-4)
                // if on page 2, the skip is 4, and take is the next 4 product (5-8)
                .Skip((productPage - 1) * PageSize).Take(PageSize),

                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ? repository.Products.Count() //if null, count all products in repo.
                    : repository.Products
                    .Where(e=> e.Category==category).Count()//else, count for specific category
                },
                CurrentCategory = category
            });

    }
}
