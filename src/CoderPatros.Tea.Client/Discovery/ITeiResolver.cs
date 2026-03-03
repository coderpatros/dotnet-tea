// Copyright (c) Patrick Dwyer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using CoderPatros.Tea.Client.Models.Discovery;

namespace CoderPatros.Tea.Client.Discovery;

public interface ITeiResolver
{
    Task<IReadOnlyList<DiscoveryInfo>> ResolveAsync(string tei, CancellationToken cancellationToken = default);
}
