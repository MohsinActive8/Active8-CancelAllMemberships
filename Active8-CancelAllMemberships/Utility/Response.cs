using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace Active8_CancelAllMemberships.Utility
{
    public class BaseResponse
    {
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; }
        public object Returned { get; set; }

        public List<object> Alerts { get; set; }
        public DatasetResponse DatasetContext { get; set; }

        public BaseResponse()
        {
            this.StatusCode = HttpStatusCode.OK;
            this.Message = "";
            this.Alerts = null;
            this.Returned = null;
        }

        /// <summary>
        /// Determines if the reponse has a status indicating a success.
        /// </summary>
        /// <returns></returns>
        public bool IsSuccess()
        {
            int code = (int)this.StatusCode;

            return code >= 200 && code <= 299;
        }
    }

    public class DatasetResponse
    {
        /// <summary>
        /// For pagination, Total Number of Pages dependedont on page size. // Math.Ceil(data.length / pageSize)
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// Total number of records returned from a dataset
        /// </summary>
        public int TotalRecordCount { get; set; }
    }
}
