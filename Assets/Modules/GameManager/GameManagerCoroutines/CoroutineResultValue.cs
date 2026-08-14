public class CoroutineResultValue<T> : CoroutineResult
{
    public T Value = default;

    public void Succeed(T loValue)
    {
        Value = loValue;
        Succeed();
    }

    public void Failed(T loValue)
    {
        Value = loValue;
        Failed();
    }
}