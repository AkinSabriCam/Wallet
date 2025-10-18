using Application.Abstransaction;

namespace Infrastructure.Mapping;

public class MyMapper : IMapper
{
    private readonly MapsterMapper.Mapper _mapper;

    public MyMapper(MapsterMapper.Mapper mapper)
    {
        _mapper = mapper;
    }

    public TDestination Map<TDestination>(object source)
    {
        return _mapper.Map<TDestination>(source);
    }
}