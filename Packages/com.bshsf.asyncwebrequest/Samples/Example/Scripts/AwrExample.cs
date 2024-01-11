using System;
using UnityEngine;
using UnityEngine.UI;
using zframework.web;
using static zframework.web.AsyncWebRequest;

namespace zframework.samples
{
    public class AwrExample : MonoBehaviour
    {
        string host;
        public Button login_bt;
        public Button getinfo_bt;
        public ScrollRect scrollRect;
        public Button clean;
        private Text text;
        private void Start()
        {
            var ws = FindObjectOfType<UniWebServer>();
            host = $"http://localhost:{ws.port}";
            login_bt.onClick.AddListener(OnLoginRequestAsync);
            getinfo_bt.onClick.AddListener(OnGetInfoRequestAsync);
            clean.onClick.AddListener(() => text.text = "");
            text = scrollRect.content.GetComponent<Text>();
        }

        private async void OnLoginRequestAsync()
        {
            var request = new LoginRequest()
            {
                id = "9527",
                password = "0123456"
            };
            Log($"开始登录: {request}");
            var response = await PostAsync<LoginRequest, LoginResponse>(host, request);
            if (response.code == "200")
            {
                Log($"登录成功: {response}");
            }
        }

        private async void OnGetInfoRequestAsync()
        {
            var url = $"{host}/api/userinfo?id=9527";
            Log($"开始获取用户信息: url = {url}");
            var response = await GetAsync<UserInfoResponse>(url);
            if (response.code == "200")
            {
                Log($"获取用户信息成功: info = {response}");
            }
        }
        private void Log(string message)
        {
            text.text += $"[{DateTime.Now:HH:mm:ss.fff}] {message} \n\n";
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
            scrollRect.normalizedPosition = new Vector2(0, 0);
        }
    }
}