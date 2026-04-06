using System;

[Serializable]
public class DTO2<T1, T2>
{
    public T1 Item1;
    public T2 Item2;

    public DTO2(T1 loItem1, T2 loItem2)
    {
        Item1 = loItem1;
        Item2 = loItem2;
    }
}