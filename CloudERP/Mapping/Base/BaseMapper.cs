namespace CloudERP.Mapping.Base
{
    public interface IMapper<TDomain, TView>
    {
        TDomain MapToDomain(TView viewModel);
        TView MapToViewModel(TDomain domainModel);
    }

    public abstract class BaseMapper<TDomain, TView> : IMapper<TDomain, TView>
    {
        public abstract TDomain MapToDomain(TView viewModel);
        public abstract TView MapToViewModel(TDomain domainModel);
    }
}