using zframework.web;
namespace zframework.samples
{
    // 只要不做其他 类型 的成员。无需使用 [Serializable] 标记,其他类同
    public class UserInfoResponse : BaseResponse
    {
        public string name;
        public int age;
        public string email;
    }
}
