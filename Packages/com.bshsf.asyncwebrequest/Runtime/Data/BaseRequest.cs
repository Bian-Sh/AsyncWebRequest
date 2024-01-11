using System;
using UnityEngine;

namespace zframework.web
{
    [Serializable]
    public class BaseRequest
    {
        public virtual string URI { get;}
        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}