public class CoroutineResultStruct<T> where T : struct
{
    public bool Completed = false;
    public T Object = default;

    public CoroutineResultStruct() { }
    public CoroutineResultStruct(T loDefault)
    {
        Object = loDefault;
    }

    public void OnCompleted(T loObject = default)
    {
        Completed = true;
        Object = loObject;
    }

    public void OnFailed(T loObject = default)
    {
        Completed = false;
        Object = loObject;
    }
}