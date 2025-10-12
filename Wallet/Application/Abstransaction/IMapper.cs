namespace Application.Abstransaction;

public interface IMapper
{
    TDestination Map<TDestination>(object source);
}