using System;

namespace Onyx.Authorization.Models
{
    /// <summary>
    /// A ViewModel that limits the view of a region's sales data for
    /// users
    /// </summary>
    public class RegionSalesSummaryViewModel
    {
        /// <summary>
        /// The unique identifier of the region
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the region
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// The full name of the director of sales for the region
        /// </summary>
        public String SalesDirector { get; set; }
        /// <summary>
        /// The actual gross sales for the region
        /// </summary>
        public Decimal GrossSales { get; set; }
        /// <summary>
        /// The gross sales target for the region
        /// </summary>
        public Decimal GrossSalesTarget { get; set; }
    }
}