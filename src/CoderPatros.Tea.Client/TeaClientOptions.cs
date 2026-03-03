// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

namespace CoderPatros.Tea.Client;

public sealed class TeaClientOptions
{
    public Uri? BaseAddress { get; set; }
    public string? BearerToken { get; set; }
}
