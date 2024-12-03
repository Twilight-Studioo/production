using DynamicActFlow.Runtime.Core.Action;

namespace DynamicActFlow.Test
{
    public static class CreateMock
    {
        public static T Action<T>() where T: ActionBase, new()
        {
            return new();
        }
    }
}