using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Onyx.Authorization.Models
{
    /// <summary>
    /// Represents a geographical region for sales tracking purposes
    /// </summary>
    public class Region
    {
        /// <summary>
        /// A unique identifier of the region (auto-generated)
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the region as it appears on reports
        /// </summary>
        [Required]
        public String Name { get; set; }
        /// <summary>
        /// Optional foreign key to the Employee entity set
        /// </summary>
        public int? SalesDirectorId { get; set; }
        /// <summary>
        /// Current sales target for the region
        /// </summary>
        [Column(TypeName = "money")]
        public Decimal SalesTarget { get; set; }

        /// <summary>
        /// The director of sales for the region
        /// </summary>
        [ForeignKey("SalesDirectorId")]
        public virtual Employee SalesDirector { get; set; }
        /// <summary>
        /// A collection of all the sales for the region
        /// </summary>
        public virtual List<Sale> Sales { get; set; }

        /// <summary>
        /// The actual gross sales for the region
        /// </summary>
        public Decimal GrossSales
        {
            get
            {
                return Sales.Sum(s => s.Amount);
            }
        }
    }
}