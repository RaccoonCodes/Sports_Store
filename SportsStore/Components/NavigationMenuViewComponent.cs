/*
 * View Component for side navigation menu. Which is used on 
 * _Layout.cshtml
 * 
 */

using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
namespace SportsStore.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private IStoreRepository repository;

        //allows views component to access data without knowing which repository implementation will be used
        public NavigationMenuViewComponent(IStoreRepository repo)=> repository = repo;
        public IViewComponentResult Invoke()
        {
            ViewBag.Selectedcategory = RouteData?.Values["category"];
            return View(repository.Products
                .Select(x => x.Category)
                .Distinct().
                OrderBy(x => x));
        }
    }
}
