using System.Threading;
using System.Threading.Tasks;

namespace RouteManager.Managers
{
    /// <summary>
    /// A interface used to delete existing models
    /// </summary>
    /// <typeparam name="TID">The identifier type used to identify the model</typeparam>
    /// <typeparam name="TModel">The model type to delete</typeparam>
    public interface IModelDelete<TID, TModel>
    {
        /// <summary>
        /// Deletes a model from the database
        /// </summary>
        /// <param name="modelId">Array of model IDs</param>
        /// <param name="cancellationToken">Cancels the delete operation</param>
        /// <returns>The amount of deleted models</returns>
        Task<int> DeleteModel(TID[] modelIds, CancellationToken cancellationToken = default);
    }
}
