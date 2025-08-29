namespace HappyCode.NetCoreBoilerplate.Core.Repositories
{
    internal abstract class RepositoryBase<TEntity>
        where TEntity : class
    {
        protected WeatherContext DbContext { get; }

        protected RepositoryBase(WeatherContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}
