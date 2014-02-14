using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cus.WebApi
{
    class DocumentApi : ApiController
    {
        private readonly ApiDescriptor _apiDescriptor;
        public DocumentApi(ApiDescriptor apiDescriptor)
        {
            _apiDescriptor = apiDescriptor;
        }
        public ApiDescriptor GetApiDescriptor()
        {
            return _apiDescriptor;
        }
    }
}
