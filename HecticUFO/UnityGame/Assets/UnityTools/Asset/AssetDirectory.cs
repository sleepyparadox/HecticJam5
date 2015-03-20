using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityTools_4_6
{
    public static class AssetDirectory
    {
        /// <summary>
        /// Returns assets of type in directory
        /// </summary>
        /// <returns></returns>
        public static T[] GetAssetsOfType<T>(this Asset[] src) where T : Asset
        {
            var type = typeof(T);
            return src.Where(a => a.GetType() == type).Cast<T>().ToArray(); 
        }
    }
}
