using OwinAppDemo.Controller.ActionFilters;
using OwinAppDemo.Controller.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace OwinAppDemo.Controller.Controllers
{
    [RoutePrefix("api/Demo")]
    [APIKeyAuthorization]
    public class DemoController : ApiController
    {
        /// <summary>
        /// Get all values
        /// </summary>
        /// <returns>List of values</returns>
        [HttpGet]
        [Route("GetAll")]
        public async Task<IHttpActionResult> GetAll()
        {
            var list = new string[] { "value1", "value2" };

            return Ok(list);
        }

        /// <summary>
        /// Get value by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of values</returns>
        [HttpGet]
        [Route("Get")]
        public async Task<IHttpActionResult> Get(int id)
        {
            return Ok("value" + id);
        }

        /// <summary>
        /// Get all values without auth key
        /// </summary>
        /// <returns>List of values</returns>
        [HttpGet]
        [Route("GetAnonymous")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetAnonymous()
        {
            var list = new string[] { "Anonymous_value1", "Anonymous_value2" };

            return Ok(list);
        }

        /// <summary>
        /// Get all values from service
        /// </summary>
        /// <returns>List of values</returns>
        [HttpGet]
        [Route("GetAllFromService")]
        public async Task<IHttpActionResult> GetAllFromService()
        {
            DemoService demoService = new DemoService();
            var result = await Task.FromResult(demoService.GetAllValues());

            return Ok(result);
        }
    }
}
