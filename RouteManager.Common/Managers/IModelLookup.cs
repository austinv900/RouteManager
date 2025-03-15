using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RouteManager.Managers
{
    /// <summary>
    /// A interface used to lookup a models
    /// </summary>
    /// <typeparam name="TLookupOptions">The options used to search for models</typeparam>
    /// <typeparam name="TModel">The model type to return</typeparam>
    public interface IModelLookup<TLookupOptions, TModel> where TLookupOptions : class
    {
        /// <summary>
        /// Searches the database for models matching the query
        /// </summary>
        /// <param name="lookupOptions">The options used to locate models</param>
        /// <param name="cancellationToken">Cancels the lookup operations</param>
        /// <returns>A async enumerable containing the results of the query</returns>
        IAsyncEnumerable<TModel> LookupModels(TLookupOptions? lookupOptions, [EnumeratorCancellation] CancellationToken cancellationToken = default);
    }
}
