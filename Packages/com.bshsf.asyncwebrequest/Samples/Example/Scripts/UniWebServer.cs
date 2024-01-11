using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using zframework.samples;
using zframework.web;

// 本地测试服务器，用于测试 Post 请求
// 请务必不要仿照此类写法用于正式项目中！！！
//  just for test post request
//  please don't use this class in your project
public class UniWebServer : MonoBehaviour
{
    public int port = 8080;
    private HttpListener listener;
    CancellationTokenSource cts;
    void Start()
    {
        cts = new CancellationTokenSource();
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:" + port + "/");
        listener.Start();
        Task.Run(BeginGetContextAsync);
    }

    private async Task BeginGetContextAsync()
    {
        while (!cts.IsCancellationRequested)
        {
            var context = await listener.GetContextAsync();
            if (cts.IsCancellationRequested)
            {
                break;
            }
            _ = ProcessRequestAsync(context);
        }
    }

    private void OnDestroy()
    {
        cts.Cancel();
        listener.Close();
    }

    // 测试用服务器， 1 个 Post Api ,1 个 Get API 
    // 1.   Post  登录 api/login
    // 2.   Get    获取用户信息 api/user
    private async Task ProcessRequestAsync(HttpListenerContext context)
    {
        if (context.Request.HttpMethod == "POST")
        {
            // 检测请求的 API 是否为 api/login 
            // check absolute path is api/login 
            if (context.Request.Url.AbsolutePath == "/api/login")
            {
                // 获取请求体，检测用户名和密码，返回 token 和状态
                // get request body ,check user name and password the response token and status
                var body = context.Request.InputStream;
                var encoding = context.Request.ContentEncoding;
                var reader = new System.IO.StreamReader(body, encoding);
                var json = reader.ReadToEnd();
                var request = JsonUtility.FromJson<LoginRequest>(json);
                var response = new LoginResponse();
                if (request.id == "9527" && request.password == "0123456")
                {
                    // 登录成功
                    // login success
                    response.code = "200";
                    response.message = "login success";
                    response.token = "1234567890";
                }
                else
                {
                    // 登录失败
                    // login failed
                    response.code = "400";
                    response.message = "login failed";
                }
                // 响应客户端
                // response to client
                var responseJson = JsonUtility.ToJson(response);
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseJson);
                context.Response.ContentLength64 = buffer.Length;
                var output = context.Response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);
                output.Close();
            }
        }
        // Get 
        if (context.Request.HttpMethod == "GET")
        {
            // 检测请求的 API 是否为   api/user
            // check absolute path is api/user
            if (context.Request.Url.AbsolutePath == "/api/userinfo")
            {
                // 获取请求头，检测 token
                // get request header , check token
                var token = context.Request.Headers["Authorization"];
                var id = context.Request.QueryString["id"];
                // log token and id
                Debug.Log($"token = {token} , id = {id}");

                var response = new UserInfoResponse();
                if (token == "Bearer 1234567890" && id == "9527")
                {
                    // 获取用户信息成功
                    // get user info success
                    response.code = "200";
                    response.message = "get user info success";
                    response.name = "张三";
                    response.email = "ZHANGSAN@email.com";
                    response.age = 18;
                }
                else
                {
                    // 获取用户信息失败
                    // get user info failed
                    response.code = "400";
                    response.message = "get user info failed";
                }
                // 响应客户端
                // response to client
                var responseJson = JsonUtility.ToJson(response);
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseJson);
                context.Response.ContentLength64 = buffer.Length;
                var output = context.Response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);
                output.Close();
            }
        }
    }
}
