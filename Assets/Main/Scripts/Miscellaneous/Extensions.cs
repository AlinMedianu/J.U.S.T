using System.Linq;
using UnityEngine;

public static class Extensions
{
    public static Vector3[] GetChildrenPositions(this Transform transform)
    {
        return (from child in transform.GetComponentsInChildren<Transform>()
                where child != transform
                select child.position).ToArray();
    }

    public static void SetChildrenPositions(this Transform transform, Vector3[] positions)
    {
        
        Transform[] children = (from child in transform.GetComponentsInChildren<Transform>()
                                where child != transform
                                select child).ToArray();
        if (children.Length == positions.Length)
            for (int i = 0; i < children.Length; ++i)
                children[i].position = positions[i];
    }

    public static Transform[] GetDirectChildren(this Transform transform)
    {
        return (from child in transform.GetComponentsInChildren<Transform>()
                where child.parent && child.parent == transform
                select child).ToArray();
    }

    public static bool HasChildren(this Transform transform)
    {
        return transform.GetComponentsInChildren<Transform>().Length > 1;
    }

    public static Vector3 XYToX0Z(this Vector2 vector2)
    {
        return new Vector3(vector2.x, 0, vector2.y);
    }

    public static int NumberOfOccurences(this int[] array, int number)
    {
        return (from element in array
                where element == number
                select element).ToArray().Length;
    }

    public static Vector3 RandomPosition(this Rect area)
    {
        return new Vector3(UnityEngine.Random.Range(area.xMin, area.xMax), 0, UnityEngine.Random.Range(area.yMin, area.yMax));
    }
}
