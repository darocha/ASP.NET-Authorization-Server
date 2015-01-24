using System;
using System.ComponentModel.DataAnnotations;

namespace Onyx.Authorization.Models
{
    public class Employee
    {
        /// <summary>
        /// Unique identifier for the employee (auto-generated)
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Required foreign key to the Region entity set
        /// </summary>
        public int RegionId { get; set; }
        /// <summary>
        /// First name of the employee
        /// </summary>
        [Required]
        public String FirstName { get; set; }
        /// <summary>
        /// Last name of the employee
        /// </summary>
        [Required]
        public String LastName { get; set; }
        /// <summary>
        /// The region for which the employee is responsible
        /// </summary>
        public virtual Region Region { get; set; }

        /// <summary>
        /// The full name of the employee
        /// </summary>
        public String FullName
        {
            get
            {
                return String.Format("{0} {1}", FirstName, LastName);
            }
        }
    }
}