namespace RouteManager.Managers
{
    public interface IModelManager<TID, TInstance, TCreateParameter, TUpdateParameter, TFindParameter> : IModelCreate<TCreateParameter, TInstance>, IModelDelete<TID, TInstance>, IModelLookup<TFindParameter, TInstance>, IModelUpdate<TID, TUpdateParameter, TInstance> where TCreateParameter : class where TFindParameter : class where TUpdateParameter : class where TInstance : class
    {
    }
}
