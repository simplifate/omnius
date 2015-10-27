using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FSPOC_WebProject.Controllers
{
    public class SourceBlockController : ApiController
    {
        [Route("api/sourceblock/{blockId}")]
        [HttpGet]
        public HttpResponseMessage GetSource(int blockId)
        {
            return Request.CreateResponse(HttpStatusCode.OK,new TestConnection {Name = "Test serialize"}) ;
        }
    }

    public class TestConnection
    {
        public string Name { get; set; }
    }
}