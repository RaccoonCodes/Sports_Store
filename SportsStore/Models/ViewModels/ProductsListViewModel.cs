/*
 *  combines product data, pagination information,
 *  and category details to provide a comprehensive 
 *  set of data required for rendering product listings
 */
namespace SportsStore.Models.ViewModels
{
    public class ProductsListViewModel
    {
        //Collection of products to be displayed
        public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
        
        //info on current page, total items, items per page,and total pages 
        public PagingInfo PagingInfo { get; set; } = new();

        public string? CurrentCategory { get; set; }
    }
}