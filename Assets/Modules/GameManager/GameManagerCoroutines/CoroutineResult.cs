public class CoroutineResult
{
    public bool Completed { get; private set; } = false;
    public bool Succeeded { get; private set; } = false;

    public void Succeed()
    {
        Completed = true;
        Succeeded = true;
    }

    public void Failed()
    {
        Completed = true;
        Succeeded = false;
    }

    public void Reset()
    {
        Completed = false;
        Succeeded = false;
    }
}