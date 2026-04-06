using System;

[Serializable]
public class DTO4<T1, T2, T3, T4>
{
    public T1 Item1;
    public T2 Item2;
    public T3 Item3;
    public T4 Item4;

    public DTO4(T1 loItem1, T2 loItem2, T3 loItem3, T4 loItem4)
    {
        Item1 = loItem1;
        Item2 = loItem2;
        Item3 = loItem3;
        Item4 = loItem4;
    }
}