using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCFServiceWebRole1
{
    public class UserInformation
    {
        public string PhoneType { get; set; }
        public string UserID { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserName { get; set; }
        public string UserAddress { get; set; }
        public string PhoneURL { get; set; }
        public string UserLongitude { get; set; }
        public string UserLatitude { get; set; }
        public string PhoneNumber { get; set; }
        public string result { get; set; }
    }

    public class GetSitedata
    {
        public string SiteType { get; set; }
        public string SiteID { get; set; }
        public string SiteName { get; set; }
        public string SiteAddress { get; set; }
        public string SiteLongitude { get; set; }
        public string SiteLatitude { get; set; }
        public string SitePhone { get; set; }
    }

    public class GroupInfo
    {
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public List<GroupUserInfo> GroupUser;
        public GroupInfo()
        {
            GroupUser = new List<GroupUserInfo>();
        }
        public string master { get; set; }
    }

    public class GroupUserInfo
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserAddress { get; set; }
        public string UserEmail { get; set; }
        public string UserLongitude { get; set; }
        public string UserLatitude { get; set; }
        public List<AlarmInfomation> alarm;
        public GroupUserInfo()
        {
            alarm = new List<AlarmInfomation>();
        }
    }

    public class AlarmInfomation
    {
        public string AlarmType { get; set; }
        public string AlarmMessage { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string AlarmTime { get; set; }
        public List<HideoutInfomation> hideout;
        public AlarmInfomation()
        {
            hideout = new List<HideoutInfomation>();
        }
    }

    public class HideoutInfomation
    {
        public string SiteAddress { get; set; }
        public string SiteLongitude { get; set; }
        public string SiteLatitude { get; set; }
        public string SiteName { get; set; }
        public string SitePhone { get; set; }
    }

    public class UpdateAlarmInfo
    {
        public string UserName { get; set; }
        public string AlarmMessage { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string AlarmTime { get; set; }
    }

    public class MessageManagment
    {
        public string MessageID { get; set; }
        public string Time { get; set; }
        public string Content { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string PriolotyID { get; set; }
        public string UserID { get; set; }
        public string type { get; set; }
        public string trigger { get; set; }
    }

    public class MessageInfo
    {
        public string MessageID { get; set; }
        public string Time { get; set; }
        public string Content { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public List<MessageGroup> MessageGroup;
        public MessageInfo()
        {
            MessageGroup = new List<MessageGroup>();
        }
    }

    public class MessageGroup
    {
        public string GroupID { get; set; }
        public string GroupName { get; set; }
    }

    public class OpenDataInfo
    {
        public string OpenDataID { get; set; }
        public string OpenDataTitle { get; set; }
        public string OpenDataUpdate { get; set; }
        public string OpenDataSummary { get; set; }
    }

    public class NotificationMessage
    {
        public string Type;
        public string Message;
        public string Module;
    }
}