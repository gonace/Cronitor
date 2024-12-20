﻿using Cronitor.Abstractions;
using Cronitor.Extensions;
using Cronitor.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Cronitor.Requests
{
    public class UpdateNotificationRequest : BaseRequest
    {
        public override string Endpoint { get; set; } = "templates/:key";
        public override HttpMethod Method => HttpMethod.Put;
        public string Key { get; set; }

        public UpdateNotificationRequest(string key, Models.Template template)
        {
            Key = key;
            Content = new StringContent(Serializer.Serialize(template), Encoding.UTF8, "application/json");
        }

        public override Uri ToUri()
        {
            var dictionary = new Dictionary<string, string>
            {
                { ":key", Key }
            };

            return base.ToUri().Build(dictionary);
        }
    }
}
