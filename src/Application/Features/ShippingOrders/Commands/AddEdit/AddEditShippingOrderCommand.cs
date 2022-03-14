// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.ShippingOrders.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.ShippingOrders.Commands.AddEdit;

public class AddEditShippingOrderCommand : ShippingOrderDto, IRequest<Result<int>>, IMapFrom<ShippingOrder>
{

}

public class AddEditShippingOrderCommandHandler : IRequestHandler<AddEditShippingOrderCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<AddEditShippingOrderCommandHandler> _localizer;
    public AddEditShippingOrderCommandHandler(
        IApplicationDbContext context,
        IStringLocalizer<AddEditShippingOrderCommandHandler> localizer,
        IMapper mapper
        )
    {
        _context = context;
        _localizer = localizer;
        _mapper = mapper;
    }
    public async Task<Result<int>> Handle(AddEditShippingOrderCommand request, CancellationToken cancellationToken)
    {

        if (request.Id > 0)
        {
            var item = await _context.ShippingOrders.FindAsync(new object[] { request.Id }, cancellationToken);
            _ = item ?? throw new NotFoundException("ShippingOrder {request.Id} Not Found.");
            item = _mapper.Map(request, item);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(item.Id);
        }
        else
        {
            var item = _mapper.Map<ShippingOrder>(request);
            foreach (var costdto in request.CostDetailDtos)
            {
                var cost = _mapper.Map<CostDetail>(costdto);
                item.CostDetails.Add(cost);
            }
            foreach(var goodsdto in request.GoodsDetailDtos)
            {
                var goods = _mapper.Map<GoodsDetail>(goodsdto);
                item.GoodsDetails.Add(goods);
            }
            _context.ShippingOrders.Add(item);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(item.Id);
        }

    }
}

