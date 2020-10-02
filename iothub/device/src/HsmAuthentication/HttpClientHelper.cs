﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Net;
using System.Net.Http;

#if !NET451

using Microsoft.Azure.Devices.Client.HsmAuthentication.Transport;

#endif

namespace Microsoft.Azure.Devices.Client.HsmAuthentication
{
    internal static class HttpClientHelper
    {
        private const string HttpScheme = "http";
        private const string HttpsScheme = "https";
        private const string UnixScheme = "unix";

        public static HttpClient GetHttpClient(Uri providerUri)
        {
            return GetHttpClient(providerUri, null);
        }

        public static HttpClient GetHttpClient(Uri providerUri, IWebProxy proxy)
        {
            HttpClient client;

            if (providerUri.Scheme.Equals(HttpScheme, StringComparison.OrdinalIgnoreCase)
                | providerUri.Scheme.Equals(HttpsScheme, StringComparison.OrdinalIgnoreCase))
            {
                var httpClientHandler = new HttpClientHandler() { Proxy = proxy };
                client = new HttpClient(httpClientHandler);
                return client;
            }

#if !NET451
            if (providerUri.Scheme.Equals(UnixScheme, StringComparison.OrdinalIgnoreCase))
            {
                client = new HttpClient(new HttpUdsMessageHandler(providerUri));
                return client;
            }
#endif

            throw new InvalidOperationException("ProviderUri scheme is not supported");
        }

        public static string GetBaseUrl(Uri providerUri)
        {
#if !NET451
            if (providerUri.Scheme.Equals(UnixScheme, StringComparison.OrdinalIgnoreCase))
            {
                return $"{HttpScheme}://{providerUri.Segments.Last()}";
            }
#endif

            return providerUri.OriginalString;
        }
    }
}
