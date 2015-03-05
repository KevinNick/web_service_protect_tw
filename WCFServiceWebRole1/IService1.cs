using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceWebRole1
{
    // 注意: 您可以使用 [重構] 功能表上的 [重新命名] 命令同時變更程式碼和組態檔中的介面名稱 "IService1"。
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]  //使用者註冊
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "UserRegister", BodyStyle = WebMessageBodyStyle.Bare)]
        string UserRegister(UserInformation UserData);

        [OperationContract]  //測試登入
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "Login_Post", BodyStyle = WebMessageBodyStyle.Bare)]
        string Login_Post(string x_string);

        [OperationContract] //使用者登入
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "Login/{Password}/{UserName}/{guid}")]
        string Login(string Password, string UserName, string guid);

        [OperationContract] //取得資料
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetOpenData/{type}/{guid}")]
        List<GetSitedata> GetOpenData(string type, string guid);

        [OperationContract] //取得群組資訊
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetGroupInfo/{UserID}/{guid}")]
        List<GroupInfo> GetGroupInfo(string UserID, string guid);

        [OperationContract] //加入群組
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "JoinGroup/{UserID}/{GroupID}/{guid}")]
        string JoinGroup(string UserID, string GroupID, string guid);

        [OperationContract] //創立群組
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "CreateGroup/{UserID}/{GroupName}/{guid}")]
        string CreateGroup(string UserID, string GroupName, string guid);

        [OperationContract] //刪除群組
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "DeleteGroup/{UserID}/{GroupID}/{guid}")]
        string DeleteGroup(string UserID, string GroupID, string guid);

        [OperationContract]  //訊息傳送
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "message_add/{userid}/{content}/{latitude}/{longitude}/{group}/{guid}")]
        string message_add(string userid, string content, string latitude, string longitude, string group, string guid);

        [OperationContract] //取得訊息
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetMessageInfo/{UserID}/{LastTime}/{guid}")]
        List<MessageInfo> GetMessageInfo(string UserID, string LastTime, string guid);

        [OperationContract]  //使用者設定位置
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "User_set_location/{userid}/{latitude}/{longitude}/{guid}")]
        string User_set_location(string userid, string latitude, string longitude, string guid);

        [OperationContract]  //取得公開資料訊息
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "Get_OpenData/{type}/{days}/{guid}")]
        List<OpenDataInfo> Get_OpenData(string type, string days, string guid);

        [OperationContract]  //新增求救
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "add_alarm_info/{userid}/{latitude}/{longitude}/{message}/{type}/{guid}")]
        string add_alarm_info(string userid, string latitude, string longitude, string message, string type, string guid);

        [OperationContract]  //求救訊息更新
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "UpdateAlarmMessage/{UserID}/{guid}")]
        List<UpdateAlarmInfo> UpdateAlarmMessage(string UserID, string guid);

        [OperationContract]  //註冊告警androidID
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "NotificationRegister/{UserID}/{PhoneType}/{Notification}")]
        string NotificationRegister(string UserID, string PhoneType, string Notification);

        [OperationContract]  //註冊告警IOSID
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "IOSNotificationTest/{UserID}/{message}")]
        void IOSNotificationTest(string UserID, string message);

        [OperationContract]  //設定機器位置
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "set_machine_location/{latitude}/{longitude}/{guid}")]
        string set_machine_location(string latitude, string longitude, string guid);

        [OperationContract]  //發送機器告警
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "machine_alarm/{latitude}/{longitude}/{l_gas}/{guid}")]
        void machine_alarm(string latitude, string longitude, string l_gas, string guid);

        [OperationContract]  //測試
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "test_function/{x}")]
        string test_function(string x);
    }
}
