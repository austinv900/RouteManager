using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RouteManager.Managers
{
    public interface IModelManager<TID, TInstance, TCreateParameter, TUpdateParameter, TFindParameter> where TCreateParameter : class where TUpdateParameter : class where TFindParameter : class
    {
        /// <summary>
        /// Deletes a model from the data source
        /// </summary>
        /// <param name="instance">The instance to delete</param>
        /// <param name="cancellationToken">Cancels the deletion</param>
        /// <returns>True if successful</returns>
        Task<bool> Delete(TInstance instance, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes multiple models from the data source
        /// </summary>
        /// <param name="instances">The instances to delete</param>
        /// <param name="cancellationToken">Cancels the deletion</param>
        /// <returns>The number of database objects deleted</returns>
        Task<int> Delete(IEnumerable<TInstance> instances, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a model and saves it to the data source
        /// </summary>
        /// <param name="options">Options used to create the <see cref="TInstance"/></param>
        /// <param name="cancellationToken">Cancels the creation</param>
        /// <returns>The instance</returns>
        Task<TInstance> Create(TCreateParameter options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a existing model with new data in the data source
        /// </summary>
        /// <param name="id">The ID of the model</param>
        /// <param name="options">Options used to update the <see cref="TInstance"/></param>
        /// <param name="cancellationToken">Cancels the update</param>
        /// <returns>The instance</returns>
        Task<TInstance> Update(TID id, TUpdateParameter options, CancellationToken cancellationToken = default);

        /// <summary>
        /// Preforms a lookup on the data source
        /// </summary>
        /// <param name="options">Options used to narrow down the lookup</param>
        /// <param name="cancellationToken"></param>
        /// <returns>List of items relating to that search</returns>
        IAsyncEnumerable<TInstance> Find(TFindParameter? options, CancellationToken cancellationToken = default);
    }
}
