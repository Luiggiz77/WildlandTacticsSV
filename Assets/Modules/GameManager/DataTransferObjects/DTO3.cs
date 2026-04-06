using System;

[Serializable]
public class DTO3<T1, T2, T3>
{
    public T1 Item1;
    public T2 Item2;
    public T3 Item3;

    public DTO3(T1 loItem1, T2 loItem2, T3 loItem3)
    {
        Item1 = loItem1;
        Item2 = loItem2;
        Item3 = loItem3;
    }
}