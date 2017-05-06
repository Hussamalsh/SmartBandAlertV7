using Backendt1.DataObjects;
using Microsoft.Azure.Mobile.Server.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Backendt1.Controllers
{
    [MobileAppController]
    public class LocationController : ApiController
    {
        // GET: api/Location
        public ArrayList Get()
        {
            LocationPersistence lp = new LocationPersistence();
            return lp.getallLocations();
        }

        // GET: api/Location/5
        public ArrayList Get(string latitude, string longitude)
        {
            LocationPersistence lp = new LocationPersistence();
            return lp.nearbyLocations(latitude, longitude);
        }

        // POST: api/Location
        public HttpResponseMessage Post([FromBody]Location value)
        {
            LocationPersistence lp = new LocationPersistence();
            String id;
            id = lp.saveUserLocation(value);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, String.Format("/user/{0}", id));

            return response;
        }

        // PUT: api/Location/
        public HttpResponseMessage Put([FromBody]Location value)
        {
            LocationPersistence lp = new LocationPersistence();
            String id;
            id = lp.editLocation(value);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, String.Format("/user/{0}", id));

            return response;
        }

        // DELETE: api/Location/5
        public void Delete(int id)
        {
        }
    }
}
