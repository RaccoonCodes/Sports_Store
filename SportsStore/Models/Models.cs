/*
 * defines properties for a product's ID, name, description, 
 * price, and category. It also includes data 
 * annotations for validation purposes, ensuring that 
 * required fields are provided and that the price falls 
 * within a specified range
 */
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace SportsStore.Models
{
    public class Product
    {
        public long? ProductID { get; set; }
        [Required(ErrorMessage = "Please enter a product name")]
        public string Name { get; set; } = String.Empty;
        [Required(ErrorMessage = "Please enter a description")]
        public string Description { get; set; } = String.Empty;
        [Required]
        [Range(0.01, double.MaxValue,
        ErrorMessage = "Please enter a positive price")]
        [Column(TypeName = "decimal(8, 2)")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Please specify a category")]
        public string Category { get; set; } = String.Empty;
    }
}