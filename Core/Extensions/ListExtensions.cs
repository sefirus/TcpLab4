namespace Core.Extensions;

public static class ListExtensions
{
    public static List<T> Shuffle<T>(this List<T> list)  
    {  
        var ran = new Random(DateTime.Now.Second);
        var n = list.Count;  
        while (n > 1) {  
            n--;  
            var k = ran.Next(n + 1);  
            (list[k], list[n]) = (list[n], list[k]);
        }

        return list;
    }
}