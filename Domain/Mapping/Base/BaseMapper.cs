namespace Domain.Mapping.Base
{
    public interface IMapper<TDomain, TDatabase>
    {
        TDomain MapToDomain(TDatabase dbModel);
        TDatabase MapToDatabase(TDomain domainModel);
    }

    public abstract class BaseMapper<TDomain, TDatabase> : IMapper<TDomain, TDatabase>
    {
        public abstract TDomain MapToDomain(TDatabase dbModel);
        public abstract TDatabase MapToDatabase(TDomain domainModel);
    }
}
