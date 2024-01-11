using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Text;
using UnityEngine;

namespace zframework.web
{
    public static class AsyncWebRequest
    {
        static string Token; // 无需外部知道，登录完成后自动缓存
        /// <summary>
        /// 发送异步GET请求
        /// </summary>
        /// <typeparam name="T">返回数据类型为 <see cref="BaseResponse"/>子类</typeparam>
        /// <returns>返回数据</returns>
        public static async Task<T> GetAsync<T>(string url) where T : BaseResponse
        {
            using var www = new UnityWebRequest(url, "GET");
            www.downloadHandler = new DownloadHandlerBuffer();
            if (!string.IsNullOrEmpty(Token))
            {
                www.SetRequestHeader("Authorization", $"Bearer {Token}");
            }
            else
            {
                Debug.LogError($"请先完成登录！");
            }
            await www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log($"Get 请求成功！\nurl = {url}\nresponse = {json}");
                return JsonUtility.FromJson<T>(json);
            }
            else
            {
                Debug.LogError($"Get 请求失败！\nurl = {url}\nError ={www.responseCode} -  {www.error}");
                return null;
            }
        }

        /// <summary>
        /// 发送异步POST请求
        /// </summary>
        /// <typeparam name="T">返回数据类型为 <see cref="BaseInfoData"/>子类</typeparam>
        /// <param name="url">请求路径</param>
        /// <param name="data">请求数据</param>
        /// <returns>返回数据</returns>
        public static async Task<K> PostAsync<T, K>(string host, T data) where T : BaseRequest where K : BaseResponse
        {
            var url = $"{host}/{data.URI.TrimStart('/')}";
            using var uwr = new UnityWebRequest(url, "POST");
            if (null == data)
            {
                Debug.LogError($"Post 请求上传的数据不得为空！url = {url}");
                return null;
            }
            uwr.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            string json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
            // 如果不是登录请求，就需要带上令牌
            if (data is not LoginRequest)
            {
                if (string.IsNullOrEmpty(Token))
                {
                    Debug.LogError($"Token 为空值，请先登录以获取令牌！ ");
                }
                else
                {
                    uwr.SetRequestHeader("Authorization", Token);
                }
            }
            uwr.downloadHandler = new DownloadHandlerBuffer();
            await uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                json = uwr.downloadHandler.text;
                Debug.Log($"Post 请求成功！\nurl = {url}\nrequest = {data}\nresponse = {json}");
                var result = JsonUtility.FromJson<K>(json);
                if (result is LoginResponse loginResult)
                {
                    Token = loginResult.token;
                }
                return result;
            }
            else
            {
                Debug.LogError($"Post 请求失败！\nurl = {url}\nrequest = {data}\nError = {uwr.error}");
            }
            return null;
        }
    }
}