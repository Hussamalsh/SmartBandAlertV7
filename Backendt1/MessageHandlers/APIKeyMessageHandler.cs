using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Backendt1.MessageHandlers
{
    public class APIKeyMessageHandler : DelegatingHandler 
    {

        private const String apikeytocheck = "1234567890";

        protected override async Task<HttpResponseMessage> SendAsync(HttpResponseMessage httpR, CancellationToken ct )
        {
            bool ValidKey = false;
            IEnumerable<string> requestHeader;

            var checkAPIKeyExists = httpR.Headers.TryGetValues("APIKey", out re);
            if (checkAPIKeyExists)
            {
                if(requestHeader.FirstOrDefault().Equals(apikeytocheck))
                {
                    ValidKey = true;



                }

                if(!ValidKey)
                {

                    return httpR.CreateRespone(HttpValidationStatus);
                }