using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Onyx.Authorization.Models;
using Thinktecture.IdentityModel.WebApi;

namespace Onyx.Authorization.Controllers
{
    /// <summary>
    /// Controller for the Regions API endpoint
    /// </summary>
    [RoutePrefix("regions")]
    public class RegionsController : ApiController
    {
        /// <summary>
        /// Datastore access context
        /// </summary>
        private AuthorizationDb db { get; set; }

        /// <summary>
        /// Handles initializing shared properties
        /// </summary>
        public RegionsController()
        {
            // initalize our access to th database
            db = new AuthorizationDb();
        }

        /// <summary>
        /// Provides authorized user with summary of sales by region
        /// </summary>
        /// <returns>
        /// An IHttpActionResult containing a collection of 
        /// view models representing sales by region
        /// </returns>
        [ResourceAuthorize("Read", "SalesSummary")]
        [Route("summary")]
        public IHttpActionResult GetSummary()
        {
            // Get all the regions
            List<Region> regions = db.Regions.ToList();
            // Create a list of view models which we will return
            var salesSummaryView = new List<RegionSalesSummaryViewModel>();
            // Change each region into a view model and insert it into the list 
            regions.ForEach(region => salesSummaryView.Add(
                new RegionSalesSummaryViewModel
                {
                    Id = region.Id,
                    Name = region.Name,
                    SalesDirector = region.SalesDirector.FullName,
                    GrossSales = region.GrossSales,
                    GrossSalesTarget = region.SalesTarget
                }
            ));
            // Return HTTP 200 with the view model list as body payload
            return Ok(salesSummaryView);
        }

        /// <summary>
        /// Releases managed and unmanaged resources based on parameters
        /// </summary>
        /// <param name="disposing">
        /// True: release managed and unamaged resources,
        /// False: release only unmanaged resources
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose of our datastore access context
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}