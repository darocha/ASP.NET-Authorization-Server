using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Onyx.Authorization.Models
{
    /// <summary>
    /// Represents the sale of a product
    /// </summary>
    public class Sale
    {
        /// <summary>
        /// A unique identifier for the sale (auto-generated)
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Required foreign key to the Region entity set
        /// </summary>
        public int RegionId { get; set; }
        /// <summary>
        /// The cash value of the sale
        /// </summary>
        [Required]
        [Column(TypeName = "money")]
        public Decimal Amount { get; set; }

        /// <summary>
        /// The region which the sale is credited to
        /// </summary>
        public virtual Region Region { get; set; }
    }
}