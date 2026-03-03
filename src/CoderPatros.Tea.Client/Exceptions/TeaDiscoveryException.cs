// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

namespace CoderPatros.Tea.Client.Exceptions;

public sealed class TeaDiscoveryException : Exception
{
    public TeaDiscoveryException(string message)
        : base(message)
    {
    }

    public TeaDiscoveryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
