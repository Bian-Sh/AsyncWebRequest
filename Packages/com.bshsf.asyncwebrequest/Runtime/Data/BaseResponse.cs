using System;
using UnityEngine;

namespace zframework.web
{
    [Serializable]
    public class BaseResponse
    {
        public string code;
        public string message;
        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}