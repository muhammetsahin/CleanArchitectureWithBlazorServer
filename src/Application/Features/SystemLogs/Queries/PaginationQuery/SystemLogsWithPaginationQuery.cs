﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.SystemLogs.Caching;
using CleanArchitecture.Blazor.Application.Features.SystemLogs.DTOs;
using CleanArchitecture.Blazor.Application.Features.SystemLogs.Mappers;
using CleanArchitecture.Blazor.Application.Features.SystemLogs.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.SystemLogs.Queries.PaginationQuery;

public class SystemLogsWithPaginationQuery : SystemLogAdvancedFilter, ICacheableRequest<PaginatedData<SystemLogDto>>
{
    public SystemLogAdvancedSpecification Specification => new(this);

    public string CacheKey => SystemLogsCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => SystemLogsCacheKey.Tags;

    public override string ToString()
    {
        return
            $"Listview:{ListView}-{LocalTimeOffset.TotalHours},{Level},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
}

public class LogsQueryHandler : IRequestHandler<SystemLogsWithPaginationQuery, PaginatedData<SystemLogDto>>
{
    private readonly IApplicationDbContext _context;

    public LogsQueryHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<PaginatedData<SystemLogDto>> Handle(SystemLogsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _context.SystemLogs.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync(request.Specification, request.PageNumber, request.PageSize,
                SystemLogMapper.ToDto, cancellationToken);
        return data;
    }
}