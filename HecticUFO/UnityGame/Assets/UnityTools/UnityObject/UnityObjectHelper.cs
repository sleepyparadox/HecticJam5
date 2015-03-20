using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityTools_4_6
{
    public static class UnityObjectHelper
    {
        public static GameObject Clone(this GameObject obj)
        {
            return (GameObject)GameObject.Instantiate(obj);
        }

        public static Material Clone(this Material obj)
        {
            return (Material)Material.Instantiate(obj);
        }
    }
}
