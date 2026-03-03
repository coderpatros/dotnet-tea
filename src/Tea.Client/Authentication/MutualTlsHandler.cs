// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System.Security.Cryptography.X509Certificates;

namespace Tea.Client.Authentication;

public static class MutualTlsHandler
{
    public static HttpClientHandler Create(X509Certificate2 clientCertificate)
    {
        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(clientCertificate);
        return handler;
    }

    public static HttpClientHandler Create(string certificatePath, string? password = null)
    {
        var cert = password is not null
            ? new X509Certificate2(certificatePath, password)
            : new X509Certificate2(certificatePath);

        return Create(cert);
    }
}
