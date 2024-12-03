namespace DynamicActFlow.Runtime.Core.Action
{
    public interface IFlowObject
    {
        /// <summary>
        ///     Called when the object is created and before set custom parameters.
        /// </summary>
        void OnCreated();
    }
}