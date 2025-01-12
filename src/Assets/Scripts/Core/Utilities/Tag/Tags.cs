using System.Linq;
using UnityEngine;

namespace Core.Utilities.Tag
{
    public static class TagUtility
    {
        public static bool HasTag<T>(this Component obj) where T : ITag
        {
            if (obj is null)
            {
                return false;
            }

            return obj.GetComponents<ITag>().Any(tag => tag is T t && tag.TagName == t.TagName);
        }

        public static bool HasTag(this Component obj, string tagName)
        {
            return obj.GetComponents<ITag>().Any(tag => tag.TagName == tagName);
        }

        // 複数のタグを取得する
        public static string[] GetTags(this Component obj)
        {
            return obj.GetComponents<ITag>().Select(tag => tag.TagName).ToArray();
        }
    }
}