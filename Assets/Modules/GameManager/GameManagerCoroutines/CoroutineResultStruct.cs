public class CoroutineResultStruct<T> where T : struct
{
    public bool Completed { get; private set; } = false;
    public T Result { get; private set; } = default;

    public CoroutineResultStruct() { }
    public CoroutineResultStruct(T loDefault)
    {
        Result = loDefault;
    }

    public void Reset()
    {
        Completed = false;
    }

    public void SetResult(T loResult)
    {
        Completed = true;
        Result = loResult;
    }
}