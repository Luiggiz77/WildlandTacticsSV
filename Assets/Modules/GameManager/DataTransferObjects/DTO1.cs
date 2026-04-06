using System;

[Serializable]
public class DTO1<T1>
{
    public T1 Item1;

    public DTO1(T1 loItem1)
    {
        Item1 = loItem1;
    }
}