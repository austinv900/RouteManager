using System.Threading;
using System.Threading.Tasks;

namespace RouteManager.Managers
{
    /// <summary>
    /// A interface used for creating database models
    /// </summary>
    /// <typeparam name="TOptions">The options used to create the model</typeparam>
    /// <typeparam name="TModel">The that is created</typeparam>
    public interface IModelCreate<TOptions, TModel> where TModel : class
    {
        /// <summary>
        /// Creates a model in the database
        /// </summary>
        /// <param name="createOptions">The options used to create the model</param>
        /// <param name="cancellationToken">Used to cancel the create operation</param>
        /// <returns>The created model</returns>
        Task<TModel> CreateModel(TOptions createOptions, CancellationToken cancellationToken = default);
    }
}
