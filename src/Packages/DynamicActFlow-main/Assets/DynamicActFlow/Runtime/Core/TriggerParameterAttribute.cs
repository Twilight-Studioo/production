namespace DynamicActFlow.Runtime.Core
{
    public class TriggerParameterAttribute : ParameterBaseAttribute
    {
        public TriggerParameterAttribute(string tag, object defaultValue = null)
        {
            Tag = tag;
            DefaultValue = defaultValue;
        }
    }
}