using System.Threading;
using System.Threading.Tasks;

namespace RouteManager.Managers
{
    /// <summary>
    /// A interface used to update a existing model
    /// </summary>
    /// <typeparam name="TID">The identifier type used to find the model</typeparam>
    /// <typeparam name="TOptions">The options to update</typeparam>
    /// <typeparam name="TModel">The model type to update</typeparam>
    public interface IModelUpdate<TID, TOptions, TModel> where TOptions : class
    {
        /// <summary>
        /// Updates the model in the database
        /// </summary>
        /// <param name="modelId">The ID of the model</param>
        /// <param name="updateOptions">The options containing the parameters to update</param>
        /// <param name="cancellationToken">Token used to cancel the update operation</param>
        /// <returns>The updated model</returns>
        Task<TModel> UpdateModel(TID modelId, TOptions updateOptions, CancellationToken cancellationToken = default);
    }
}
