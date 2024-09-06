namespace Feature.Interface
{
    public interface IAnimator
    {
        void SetBool(int id, bool value);
        void SetFloat(int id, float value);
        void SetTrigger(int id);
        void SetInteger(int id, int value);
    }
}