using System;
using System.Collections.Generic;
using System.Text;

namespace Cronitor
{
    public class ActivityClient
    {
        private readonly string _apiKey;
        private readonly bool _useHttps = true;

        public ActivityClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        public ActivityClient(string apiKey, bool useHttps)
        {
            _apiKey = apiKey;
            _useHttps = useHttps;
        }
    }
}
