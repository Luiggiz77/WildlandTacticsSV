public class CoroutineResult<T> where T : class
{
    public bool Completed = false;
    public T Object = null;

    public void OnCompleted(T loObject = null)
    {
        Completed = true;
        Object = loObject;
    }

    public void OnFailed(T loObject = null)
    {
        Completed = false;
        Object = loObject;
    }
}