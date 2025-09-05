namespace HappyCode.NetCoreBoilerplate.Core.Repositories
{
    internal abstract class RepositoryBase<TEntity, TContext>
        where TEntity : class
    {
        protected TContext DbContext { get; }

        protected RepositoryBase(TContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}
