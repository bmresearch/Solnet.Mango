using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Historical.Models
{
    /// <summary>
    /// The status of the response.
    /// </summary>
    public enum TvBarStatus
    {
        /// <summary>
        /// The request was successful.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// There was an error in the request.
        /// </summary>
        Error = 1,

        /// <summary>
        /// There was no data for the request.
        /// </summary>
        NoData = 2
    }
}
