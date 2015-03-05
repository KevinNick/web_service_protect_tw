using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceWebRole1
{
    // 注意: 您可以使用 [重構] 功能表上的 [重新命名] 命令同時變更程式碼、svc 和組態檔中的類別名稱 "Service1"。
    // 注意: 若要啟動 WCF 測試用戶端以便測試此服務，請在 [方案總管] 中選取 Service1.svc 或 Service1.svc.cs，然後開始偵錯。
    public class Service1 : IService1
    {
        public string UserRegister(UserInformation UserData)
        {
            string ret = "success";
            Protect_DBDataContext DB = new Protect_DBDataContext();

            // 檢查帳號密碼
            if (UserData.UserName.Length == 0
                || UserData.UserPassword.Length == 0)
            {
                ret = "註冊資料不完整";
            }
            else   // 檢查帳號是否重複
            {
                var CheckUser =
                    from User_info in DB.UserInfo
                    where User_info.Username == UserData.UserPassword
                    select User_info;

                if (CheckUser.Count() != 0)
                {
                    ret = "此帳號已被註冊";
                }
                else
                {
                    UserInfo user_info = new UserInfo();
                    user_info.Username = UserData.UserPassword;
                    user_info.Password = UserData.UserName;
                    user_info.EmailAddress = UserData.UserEmail;
                    DB.UserInfo.InsertOnSubmit(user_info);
                    try
                    {
                        DB.SubmitChanges();
                    }
                    catch
                    {
                        ret = "註冊失敗";
                    }
                }
            }
            return ret;
        }

        public string Login(string Password, string UserName, string guid)
        {
            string ret = "success";
            Protect_DBDataContext DB = new Protect_DBDataContext();

            var CheckUser =
                from User_info in DB.UserInfo
                where User_info.Username == UserName
                select User_info;

            if (CheckUser.Count() == 0)
            {
                ret = "查無此帳號";
            }
            else if (CheckUser.First().Password.Equals(Password) == false)
            {
                ret = "密碼錯誤";
            }
            else
            {
                ret = CheckUser.First().UserID.ToString();
            }

            return ret;
        }

        public string Login_Post(string x_string)
        {
            return x_string;
        }

        // 拿取公開資訊
        /*
         *  type說明：
         *      0: 全部SiteDB資料回傳
         *      1: 海巡
         *      2: 派出所
         *      3: 消防隊
         *      4: 警察總局
         *      5: 醫院
         *      6: 應變中心
         *      7: 清潔隊
         *      8: 災民收容所
         */
        public List<GetSitedata> GetOpenData(string type, string guid)
        {
            Protect_DBDataContext DB = new Protect_DBDataContext();
            List<GetSitedata> data_db = new List<GetSitedata>();
            List<SiteDB> site_db = new List<SiteDB>();
            string search_string = "";

            var GetAllSite =
                from AllSite in DB.SiteDB
                select AllSite;

            switch (type)
            {
                case "1":     // 海巡
                    search_string = "海巡";
                    break;
                case "2":     // 派出所
                    search_string = "派出所";
                    break;
                case "3":     // 消防隊
                    search_string = "消防隊";
                    break;
                case "4":     // 警察總局
                    search_string = "警察總局";
                    break;
                case "5":     // 醫院
                    search_string = "醫院";
                    break;
                case "6":     // 應變中心
                    search_string = "應變中心";
                    break;
                case "7":     // 清潔隊
                    search_string = "清潔隊";
                    break;
                case "8":     // 災民收容所
                    search_string = "災民收容場所";
                    break;
                default:
                    search_string = "nothing";
                    break;
            }

            if (search_string.Length != 0)
            {
                var search_db =
                    from site_info in GetAllSite
                    where site_info.SiteType == search_string
                    select site_info;
                site_db = search_db.ToList();
            }
            else
            {
                site_db = GetAllSite.ToList();
            }

            foreach (var item in site_db)
            {
                GetSitedata siteDB_item = new GetSitedata();
                siteDB_item.SiteID = item.SiteID.ToString();
                siteDB_item.SiteName = item.SiteName;
                siteDB_item.SiteLatitude = item.SiteLatitude.ToString();
                siteDB_item.SiteLongitude = item.SiteLongitude.ToString();
                siteDB_item.SiteAddress = item.SiteAddress;
                siteDB_item.SiteType = item.SiteType;
                siteDB_item.SitePhone = item.SitePhone;
                data_db.Add(siteDB_item);
            }
            return data_db;
        }

        // 取得群組資訊
        public List<GroupInfo> GetGroupInfo(string UserID, string guid)
        {
            List<GroupInfo> group_info = new List<GroupInfo>();
            Protect_DBDataContext DB = new Protect_DBDataContext();

            DateTime today = DateTime.Now.AddDays(-1); // 抓到的是格林威治標準時間
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time"); // 取得台北時區與格林威治標準時間差
            DateTime targetTime = TimeZoneInfo.ConvertTime(today, est); // 轉換為台北時間

            var search_db =
                from search_group in DB.GroupMapping
                where search_group.UserID == int.Parse(UserID)
                select search_group;

            foreach (var items in search_db)
            {
                GroupInfo group_info_obj = new GroupInfo();
                group_info_obj.GroupID = items.GroupID.ToString();
                group_info_obj.GroupName = items.UserGroup.UserGroupName.ToString();
                if (items.UserGroup.UserID == int.Parse(UserID))
                {
                    group_info_obj.master = "1";
                }
                else
                {
                    group_info_obj.master = "0";
                }
                foreach (var item in items.UserGroup.GroupMapping)
                {
                    GroupUserInfo user_obj = new GroupUserInfo();
                    string user_id = item.UserInfo.UserID.ToString();

                    // set user data
                    user_obj.UserName = item.UserInfo.Username;
                    user_obj.UserAddress = item.UserInfo.LocationAddress;
                    user_obj.UserID = user_id;
                    user_obj.UserEmail = item.UserInfo.EmailAddress;
                    user_obj.UserLatitude = item.UserInfo.UserLatitude.ToString();
                    user_obj.UserLongitude = item.UserInfo.UserLongitude.ToString();

                    // set user alarm info
                    var search_alarm_db =
                        from search_alarm in DB.AlarmInfo
                        where search_alarm.UserID == int.Parse(user_id)
                              && search_alarm.AlarmTime > targetTime
                        select search_alarm;

                    foreach (var alarm_items in search_alarm_db)
                    {
                        AlarmInfomation alarm_obj = new AlarmInfomation();
                        alarm_obj.AlarmMessage = alarm_items.Alarmmessage;
                        alarm_obj.Latitude = alarm_items.Latitude.ToString();
                        alarm_obj.Longitude = alarm_items.Longitude.ToString();
                        alarm_obj.AlarmType = alarm_items.AlarmType.ToString();
                        alarm_obj.AlarmTime = alarm_items.AlarmTime.ToString();
                        switch (alarm_items.AlarmType)
                        {
                            case 1:     // 回傳避難場所資訊
                            case 3:
                            case 5:
                                var search_sitehide_db =
                                    from search_site_hide in DB.SiteDB
                                    where search_site_hide.SiteType == "災民收容場所"
                                    select search_site_hide;

                                foreach (var Nearly_item in search_sitehide_db)
                                {
                                    if (distance(Nearly_item.SiteLatitude.ToString(), Nearly_item.SiteLongitude.ToString(), alarm_items.Latitude.ToString(), alarm_items.Longitude.ToString()) <= 10)
                                    {
                                        HideoutInfomation hideout_obj = new HideoutInfomation();
                                        hideout_obj.SiteAddress = Nearly_item.SiteAddress;
                                        hideout_obj.SiteLatitude = Nearly_item.SiteLatitude.ToString();
                                        hideout_obj.SiteLongitude = Nearly_item.SiteLongitude.ToString();
                                        hideout_obj.SiteName = Nearly_item.SiteName;
                                        hideout_obj.SitePhone = Nearly_item.SitePhone;
                                        alarm_obj.hideout.Add(hideout_obj);
                                    }
                                }
                                break;
                            case 2:     // 回傳醫院資訊                                
                            case 10:
                                var search_hospitle_db =
                                    from search_site_hide in DB.SiteDB
                                    where search_site_hide.SiteType == "醫院"
                                    select search_site_hide;

                                foreach (var Nearly_item in search_hospitle_db)
                                {
                                    if (distance(Nearly_item.SiteLatitude.ToString(), Nearly_item.SiteLongitude.ToString(), alarm_items.Latitude.ToString(), alarm_items.Longitude.ToString()) <= 10)
                                    {
                                        HideoutInfomation hideout_obj = new HideoutInfomation();
                                        hideout_obj.SiteAddress = Nearly_item.SiteAddress;
                                        hideout_obj.SiteLatitude = Nearly_item.SiteLatitude.ToString();
                                        hideout_obj.SiteLongitude = Nearly_item.SiteLongitude.ToString();
                                        hideout_obj.SiteName = Nearly_item.SiteName;
                                        hideout_obj.SitePhone = Nearly_item.SitePhone;
                                        alarm_obj.hideout.Add(hideout_obj);
                                    }
                                }
                                break;
                            case 4:     // 回傳警察局資訊                                
                            case 6:

                                var search_police_db =
                                    from search_site_hide in DB.SiteDB
                                    where search_site_hide.SiteType == "派出所"
                                    select search_site_hide;

                                foreach (var Nearly_item in search_police_db)
                                {
                                    if (distance(Nearly_item.SiteLatitude.ToString(), Nearly_item.SiteLongitude.ToString(), alarm_items.Latitude.ToString(), alarm_items.Longitude.ToString()) <= 10)
                                    {
                                        HideoutInfomation hideout_obj = new HideoutInfomation();
                                        hideout_obj.SiteAddress = Nearly_item.SiteAddress;
                                        hideout_obj.SiteLatitude = Nearly_item.SiteLatitude.ToString();
                                        hideout_obj.SiteLongitude = Nearly_item.SiteLongitude.ToString();
                                        hideout_obj.SiteName = Nearly_item.SiteName;
                                        hideout_obj.SitePhone = Nearly_item.SitePhone;
                                        alarm_obj.hideout.Add(hideout_obj);
                                    }
                                }
                                break;
                            case 7:     // 回傳消防局
                            case 8:
                            case 9:
                                var search_fire_db =
                                    from search_site_hide in DB.SiteDB
                                    where search_site_hide.SiteType == "消防隊"
                                    select search_site_hide;

                                foreach (var Nearly_item in search_fire_db)
                                {
                                    if (distance(Nearly_item.SiteLatitude.ToString(), Nearly_item.SiteLongitude.ToString(), alarm_items.Latitude.ToString(), alarm_items.Longitude.ToString()) <= 10)
                                    {
                                        HideoutInfomation hideout_obj = new HideoutInfomation();
                                        hideout_obj.SiteAddress = Nearly_item.SiteAddress;
                                        hideout_obj.SiteLatitude = Nearly_item.SiteLatitude.ToString();
                                        hideout_obj.SiteLongitude = Nearly_item.SiteLongitude.ToString();
                                        hideout_obj.SiteName = Nearly_item.SiteName;
                                        hideout_obj.SitePhone = Nearly_item.SitePhone;
                                        alarm_obj.hideout.Add(hideout_obj);
                                    }
                                }

                                break;
                        }
                        user_obj.alarm.Add(alarm_obj);
                    }

                    group_info_obj.GroupUser.Add(user_obj);
                }
                group_info.Add(group_info_obj);
            }

            return group_info;
        }

        public double distance(string lat1, string lon1, string lat2, string lon2)
        {
            if (lat1.Length > 0
                && lon1.Length > 0
                && lat2.Length > 0
                && lon2.Length > 0)
            {
                double theta = double.Parse(lon1) - double.Parse(lon2);
                double dist = Math.Sin(deg2rad(double.Parse(lat1))) * Math.Sin(deg2rad(double.Parse(lat2))) + Math.Cos(deg2rad(double.Parse(lat1))) * Math.Cos(deg2rad(double.Parse(lat2))) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                return Math.Abs(dist);
            }
            else
            {
                return 100000;
            }            
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        public List<UpdateAlarmInfo> UpdateAlarmMessage(string UserID, string guid)
        {
            List<UpdateAlarmInfo> alarm_info = new List<UpdateAlarmInfo>();
            Protect_DBDataContext DB = new Protect_DBDataContext();

            DateTime today = DateTime.Now.AddDays(-1); // 抓到的是格林威治標準時間
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time"); // 取得台北時區與格林威治標準時間差
            DateTime targetTime = TimeZoneInfo.ConvertTime(today, est); // 轉換為台北時間

            var search_db =
                from search_group in DB.GroupMapping
                where search_group.UserID == int.Parse(UserID)
                select search_group;

            foreach (var items in search_db)
            {
                foreach (var item in items.UserGroup.GroupMapping)
                {
                    
                    string user_id = item.UserInfo.UserID.ToString();

                    // set user alarm info
                    var search_alarm_db =
                        from search_alarm in DB.AlarmInfo
                        where search_alarm.UserID == int.Parse(user_id)
                              && search_alarm.AlarmTime > targetTime
                        select search_alarm;

                    if (search_alarm_db.Count() > 0)
                    {
                        foreach (var alarm_items in search_alarm_db)
                        {
                            UpdateAlarmInfo alarm_obj = new UpdateAlarmInfo();
                            alarm_obj.UserName = alarm_items.UserInfo.Username;
                            alarm_obj.AlarmMessage = alarm_items.Alarmmessage;
                            alarm_obj.Latitude = alarm_items.Latitude.ToString();
                            alarm_obj.Longitude = alarm_items.Longitude.ToString();
                            alarm_obj.AlarmTime = alarm_items.AlarmTime.ToString();
                            alarm_info.Add(alarm_obj);
                        }                        
                    }
                }                
            }

            return alarm_info;
        }

        // 加入群組
        public string JoinGroup(string UserID, string GroupID, string guid)
        {
            Protect_DBDataContext DB = new Protect_DBDataContext();
            string ret = "";
            int match = 0;

            // 檢查是否有group
            var group_db =
                from search_group in DB.UserGroup
                where search_group.UserGroupID == int.Parse(GroupID)
                select search_group;

            // 檢查user是否有重複加同一個群組
            if (group_db.Count() != 0)
            {
                foreach (var item in group_db.First().GroupMapping)
                {
                    if (item.UserID == int.Parse(UserID))
                    {
                        match = 1;
                        break;
                    }
                }
            }

            if (group_db.Count() == 0)
            {
                ret = "查無此群組";
            }
            else if (match == 1)
            {
                ret = "已加入此群組";
            }
            else
            {
                List<GroupMapping> group_list = new List<GroupMapping>();
                GroupMapping group_mapping = new GroupMapping();
                group_mapping.UserID = int.Parse(UserID);
                group_mapping.GroupID = int.Parse(GroupID);

                try
                {
                    // 加入群組，更新資料庫
                    group_list.Add(group_mapping);
                    DB.GroupMapping.InsertAllOnSubmit(group_list);
                    DB.SubmitChanges();
                    ret = "success";
                }
                catch
                {
                    ret = "加入程序錯誤";
                }
            }

            return ret;
        }

        // 建立群組
        public string CreateGroup(string UserID, string GroupName, string guid)
        {
            Protect_DBDataContext DB = new Protect_DBDataContext();
            string ret = "";

            // 檢查是否有user
            var user_db =
                from search_user in DB.UserInfo
                where search_user.UserID == int.Parse(UserID)
                select search_user;

            // 檢查是否有user是否曾經創立過群組
            var group_db =
                from search_group in DB.UserGroup
                where search_group.UserID == int.Parse(UserID)
                select search_group;

            // 檢查群組名稱是否重複
            var group_name_db =
                from search_group in DB.UserGroup
                where search_group.UserGroupName == GroupName
                select search_group;

            if (user_db.Count() == 0)
            {
                ret = "錯誤的使用者";
            }
            else if (group_db.Count() != 0)
            {
                ret = "曾創立過群組";
            }
            else if (group_name_db.Count() != 0)
            {
                ret = "群組名稱已重複";
            }
            else
            {
                // 建立群組
                List<UserGroup> create_group = new List<UserGroup>();
                UserGroup user_group = new UserGroup();
                user_group.UserGroupName = GroupName;
                user_group.UserID = int.Parse(UserID);
                create_group.Add(user_group);

                try
                {
                    DB.UserGroup.InsertAllOnSubmit(create_group);
                    DB.SubmitChanges();
                    // 加入群組
                    group_name_db =
                        from search_group in DB.UserGroup
                        where search_group.UserGroupName == GroupName
                        select search_group;
                    if (group_name_db.Count() != 0)
                    {
                        List<GroupMapping> group_list = new List<GroupMapping>();
                        GroupMapping group_mapping = new GroupMapping();
                        group_mapping.UserID = int.Parse(UserID);
                        group_mapping.GroupID = group_name_db.First().UserGroupID;
                        group_list.Add(group_mapping);

                        DB.GroupMapping.InsertAllOnSubmit(group_list);
                        DB.SubmitChanges();
                        ret = "success";
                    }
                }
                catch
                {
                    ret = "fail";
                }
            }
            return ret;
        }

        // 刪除群組
        public string DeleteGroup(string UserID, string GroupID, string guid)
        {
            Protect_DBDataContext DB = new Protect_DBDataContext();
            string ret = "";

            // 檢查是否有group
            var group_db =
                from search_group in DB.UserGroup
                where search_group.UserGroupID == int.Parse(GroupID)
                select search_group;

            if (group_db.Count() == 0)
            {
                ret = "群組不存在";
            }
            else if (group_db.First().UserID != int.Parse(UserID))
            {
                ret = "權限不足";
            }
            else
            {
                DB.GroupMapping.DeleteAllOnSubmit(group_db.First().GroupMapping);
                DB.UserGroup.DeleteAllOnSubmit(group_db);
                DB.SubmitChanges();
                ret = "success";
            }

            return ret;
        }

        // 取得訊息資訊
        public List<MessageInfo> GetMessageInfo(string UserID, string LastTime, string guid)
        {
            Protect_DBDataContext DB = new Protect_DBDataContext();
            List<MessageInfo> ret_messagegroup = new List<MessageInfo>();

            // 根據UserID挑出group
            var group_db =
                from search_group in DB.GroupMapping
                where search_group.UserID == int.Parse(UserID)
                select search_group;

            if (LastTime == null
                || LastTime == "0")
            {
                DateTime today = DateTime.Now; // 抓到的是格林威治標準時間
                TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time"); // 取得台北時區與格林威治標準時間差
                DateTime targetTime = TimeZoneInfo.ConvertTime(today, est); // 轉換為台北時間
                LastTime = targetTime.ToString();
            }

            var message_db =
                from search_message in DB.MessageManage
                where search_message.MessageTime < DateTime.Parse(LastTime)
                select search_message;

            foreach (var message_items in message_db)
            {
                // 找到使用者所加入的group
                var message_group_db =
                    from search_message in message_items.UserInfo.GroupMapping
                    where search_message.UserID == message_items.MessageUserID
                    select search_message;
                foreach (var group_items in group_db)
                {
                    var message_user_group_db =
                        from search_message in message_group_db
                        where search_message.GroupID == group_items.GroupID
                        select search_message;

                    if (message_user_group_db.Count() > 0)
                    {
                        MessageInfo ret_messagegroup_obj = new MessageInfo();
                        ret_messagegroup_obj.UserID = message_items.MessageUserID.ToString();
                        ret_messagegroup_obj.MessageID = message_items.MessageID.ToString();
                        ret_messagegroup_obj.Latitude = message_items.MessageLatitude.ToString();
                        ret_messagegroup_obj.Longitude = message_items.MessageLongitude.ToString();
                        ret_messagegroup_obj.Time = message_items.MessageTime.ToString();
                        ret_messagegroup_obj.Content = message_items.MessageContect;
                        ret_messagegroup_obj.UserName = message_items.UserInfo.Username;
                        if (message_items.MessageMark == 0)
                        {
                            foreach (var message_group_items in message_group_db)
                            {
                                MessageGroup obj = new MessageGroup();
                                obj.GroupID = message_group_items.GroupID.ToString();
                                obj.GroupName = message_group_items.UserGroup.UserGroupName;
                                ret_messagegroup_obj.MessageGroup.Add(obj);
                            }
                        }
                        else
                        {
                            foreach (var message_group_items in message_group_db)
                            {
                                if (message_items.MessageMark == message_group_items.UserGroup.UserGroupID)
                                {
                                    MessageGroup obj = new MessageGroup();
                                    obj.GroupID = message_group_items.UserGroup.UserGroupID.ToString();
                                    obj.GroupName = message_group_items.UserGroup.UserGroupName;
                                    ret_messagegroup_obj.MessageGroup.Add(obj);
                                    break;
                                }
                            }
                        }
                        ret_messagegroup.Add(ret_messagegroup_obj);
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return ret_messagegroup;
        }

        /*
         *   type:
         *      0: 全部
         *      1: 地震
         *      2: 海嘯
         *      3: 豪大雨
         *      4: 颱風
         *      5: 水庫洩洪
         *      6: 淹水警戒
         *      7: 土石流(目前還不支援)
         *      8: 河川水位
         *      9: 預警性道路封閉
         */
        public List<OpenDataInfo> Get_OpenData(string type, string days, string guid)
        {
            Protect_DBDataContext DB = new Protect_DBDataContext();
            List<OpenDataInfo> ret_opendata = new List<OpenDataInfo>();
            List<DisasterDB> open_data = new List<DisasterDB>();
            DateTime today = DateTime.Now.AddDays(-(int.Parse(days))); // 抓到的是格林威治標準時間
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time"); // 取得台北時區與格林威治標準時間差
            DateTime targetTime = TimeZoneInfo.ConvertTime(today, est); // 轉換為台北時間
            string search_string = "";

            var GetAllData =
                from AllOpenData in DB.DisasterDB
                select AllOpenData;

            switch (int.Parse(type))
            {
                case 0:
                    search_string = "all";
                    break;
                case 1:    // 地震
                    search_string = "地震";
                    break;
                case 2:    // 海嘯
                    search_string = "海嘯";
                    break;
                case 3:    // 豪大雨
                    search_string = "降雨";
                    break;
                case 4:    // 颱風
                    search_string = "颱風";
                    break;
                case 5:    // 水庫洩洪
                    search_string = "水庫洩洪";
                    break;
                case 6:    // 淹水警戒
                    search_string = "淹水";
                    break;
                case 8:    // 河川高水位
                    search_string = "河川高水位";
                    break;
                case 9:    // 預警性道路封閉
                    search_string = "道路封閉";
                    break;
                default:
                    search_string = "none";
                    break;
            }

            if (search_string == "all")
            {
                if (int.Parse(days) != 0)
                {
                    var opendata_db =
                        from search_data in GetAllData
                        where search_data.DisasterUpdate > targetTime
                        select search_data;
                    open_data = opendata_db.ToList();
                }
                else
                {
                    open_data = GetAllData.ToList();
                }
            }
            else
            {
                var opendata_db =
                    from search_data in GetAllData
                    where search_data.DisasterTitle == search_string
                    select search_data;

                if (int.Parse(days) != 0)
                {
                    var targetdata_db =
                        from search_data in opendata_db
                        where search_data.DisasterUpdate > targetTime
                        select search_data;

                    open_data = targetdata_db.ToList();
                }
                else
                {
                    open_data = opendata_db.ToList();
                }
            }

            foreach (var items in open_data)
            {
                OpenDataInfo info = new OpenDataInfo();

                info.OpenDataID = items.DisasterDataID;
                info.OpenDataTitle = items.DisasterTitle;
                info.OpenDataSummary = items.DisasterSummary;
                info.OpenDataUpdate = items.DisasterUpdate.ToString();

                ret_opendata.Add(info);
            }
            return ret_opendata;
        }

        public string message_add(string userid, string content, string latitude, string longitude, string group, string guid)
        {
            Protect_DBDataContext DB = new Protect_DBDataContext();
            string ret = "";

            MessageManage add_message = new MessageManage();
            add_message.MessageUserID = int.Parse(userid);
            DateTime today = DateTime.Now; // 抓到的是格林威治標準時間
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time"); // 取得台北時區與格林威治標準時間差
            DateTime targetTime = TimeZoneInfo.ConvertTime(today, est); // 轉換為台北時間
            add_message.MessageTime = targetTime;
            add_message.MessageContect = content;
            add_message.MessageMark = int.Parse(group);
            if (latitude != "0"
                && longitude != "0")
            {
                add_message.MessageLatitude = float.Parse(latitude);
                add_message.MessageLongitude = float.Parse(longitude);
            }

            try
            {
                DB.MessageManage.InsertOnSubmit(add_message);
                DB.SubmitChanges();
                ret = "success";
            }
            catch (Exception e)
            {
                ret = "fail";
            }

            return ret;
        }

        public string User_set_location(string userid, string latitude, string longitude, string guid)
        {
            Protect_DBDataContext DB = new Protect_DBDataContext();
            string ret = "";

            // 檢查UserID
            var user_db =
                from search_user in DB.UserInfo
                where search_user.UserID == int.Parse(userid)
                select search_user;

            UserInfo modify_user = user_db.First();
            modify_user.UserLatitude = float.Parse(latitude);
            modify_user.UserLongitude = float.Parse(longitude);
            try
            {
                DB.SubmitChanges();
                ret = "success";
            }
            catch (Exception e)
            {
                ret = "fail";
            }

            return ret;
        }

        public string add_alarm_info(string userid, string latitude, string longitude, string message, string type, string guid)
        {
            Protect_DBDataContext DB = new Protect_DBDataContext();
            string ret = "";

            AlarmInfo alarm_info = new AlarmInfo();
            alarm_info.UserID = int.Parse(userid);
            alarm_info.Latitude = float.Parse(latitude);
            alarm_info.Longitude = float.Parse(longitude);
            alarm_info.Alarmmessage = message;
            alarm_info.AlarmType = int.Parse(type);
            DateTime today = DateTime.Now; // 抓到的是格林威治標準時間
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time"); // 取得台北時區與格林威治標準時間差
            DateTime targetTime = TimeZoneInfo.ConvertTime(today, est); // 轉換為台北時間
            alarm_info.AlarmTime = targetTime;

            try
            {
                DB.AlarmInfo.InsertOnSubmit(alarm_info);
                DB.SubmitChanges();
                ret = "success";
            }
            catch (Exception e)
            {
                ret = "fail";
            }

            return ret;
        }

        // 發送Notification
        public void NotificationSend(int UserID, NotificationMessage messageString)
        {
            Protect_DBDataContext DB = new Protect_DBDataContext();
            var GetUserPhoneNotification =
                from UserPhoneNotification in DB.PhoneNotification
                where UserPhoneNotification.UserID == UserID
                select UserPhoneNotification;

            foreach (var item in GetUserPhoneNotification)
            {
                try
                {
                    if (item.PhoneType == "android")
                    {
                        AndroidModuleLib AndroidModule = new AndroidModuleLib();
                        AndroidModule.SendAndroidNotification(item.ServerKey.ServerKeyString, item.NotificationUrl, messageString);
                    }
                }
                catch { }
            }
        }

        // 註冊Notification        
        public string NotificationRegister(string UserID, string PhoneType, string Notification)
        {
            Protect_DBDataContext CloudDB = new Protect_DBDataContext();
            string result = "success";
            var CheckUserNotification =
                from UserNotification in CloudDB.PhoneNotification
                where UserNotification.UserID == int.Parse(UserID)
                select UserNotification;
            if (CheckUserNotification.Count() != 0)
            {
                var DeleteNotification =
                    from NotificationDelete in CloudDB.PhoneNotification
                    where NotificationDelete.UserID != int.Parse(UserID) && NotificationDelete.NotificationUrl == Notification
                    select NotificationDelete;
                if (DeleteNotification.Count() != 0)
                {
                    CloudDB.PhoneNotification.DeleteAllOnSubmit(DeleteNotification);
                }
                CheckUserNotification.First().PhoneType = PhoneType;
                CheckUserNotification.First().NotificationUrl = Notification;
                try
                {
                    CloudDB.SubmitChanges();
                }
                catch (Exception e)
                {
                    //FunctionLib CreateFunctionLib = new FunctionLib();
                    //CreateFunctionLib.SaveErrorLog("Cloud Service : NotificationRegister(Register error)", e);
                    result = "error";
                }
            }
            else
            {
                var DeleteNotification =
                    from NotificationDelete in CloudDB.PhoneNotification
                    where NotificationDelete.UserID != int.Parse(UserID) && NotificationDelete.NotificationUrl == Notification
                    select NotificationDelete;
                if (DeleteNotification.Count() != 0)
                {
                    CloudDB.PhoneNotification.DeleteAllOnSubmit(DeleteNotification);
                }
                PhoneNotification UserNotification = new PhoneNotification();
                UserNotification.PhoneType = PhoneType;
                UserNotification.NotificationUrl = Notification;
                UserNotification.UserID = int.Parse(UserID);
                if (PhoneType == "android")
                {
                    UserNotification.ServerKeyID = 1;
                }
                CloudDB.PhoneNotification.InsertOnSubmit(UserNotification);
                try
                {
                    CloudDB.SubmitChanges();
                }
                catch (Exception e)
                {
                    //FunctionLib CreateFunctionLib = new FunctionLib();
                    //CreateFunctionLib.SaveErrorLog("Cloud Service : NotificationRegister(Register error)", e);
                    result = "error";
                }
            }
            return result;
        }

        public void IOSNotificationTest(string UserID, string message)
        {
            Protect_DBDataContext DB = new Protect_DBDataContext();
            NotificationMessage messageString = new NotificationMessage();
            messageString.Message = message;
            messageString.Module = "1";
            messageString.Type = "0";
            var GetUserPhoneNotification =
                from UserPhoneNotification in DB.PhoneNotification
                where UserPhoneNotification.UserID == int.Parse(UserID)
                select UserPhoneNotification;

            foreach (var item in GetUserPhoneNotification)
            {
                try
                {
                    if (item.PhoneType == "android")
                    {
                        AndroidModuleLib AndroidModule = new AndroidModuleLib();
                        AndroidModule.SendAndroidNotification(item.ServerKey.ServerKeyString, item.NotificationUrl, messageString);
                    }
                }
                catch (Exception e)
                {
                    //FunctionLib CreateFunctionLib = new FunctionLib();
                    //CreateFunctionLib.SaveErrorLog("Cloud Service : IOSNotificationTest", e);
                }
            }
        }

        public string set_machine_location(string latitude, string longitude, string guid)
        {
            Protect_DBDataContext DB = new Protect_DBDataContext();
            string ret = "";

            MachineInfo machine_data = DB.MachineInfo.First();
            machine_data.Latitude = float.Parse(latitude);
            machine_data.Longitude = float.Parse(longitude);

            try
            {
                DB.SubmitChanges();
                ret = "success";
            }
            catch (Exception e)
            {
                ret = "fail";
            }

            return ret;
        }

        public void machine_alarm(string latitude, string longitude, string l_gas, string guid)
        {
            Protect_DBDataContext DB = new Protect_DBDataContext();
            List<UserInfo> user = new List<UserInfo>(DB.UserInfo);

            foreach (var item in user)
            {
                if (distance(latitude, longitude, item.UserLatitude.ToString(), item.UserLongitude.ToString()) <= 10)
                {
                    String message = "周圍危險氣體過高，請注意！天然氣數值：" + l_gas;
                    NotificationMessage messageString = new NotificationMessage();
                    messageString.Message = message;
                    messageString.Module = "1";
                    messageString.Type = "0";

                    var GetUserPhoneNotification =
                        from UserPhoneNotification in DB.PhoneNotification
                        where UserPhoneNotification.UserID == item.UserID
                        select UserPhoneNotification;

                    foreach (var items in GetUserPhoneNotification)
                    {
                        try
                        {
                            AndroidModuleLib AndroidModule = new AndroidModuleLib();
                            AndroidModule.SendAndroidNotification(items.ServerKey.ServerKeyString, items.NotificationUrl, messageString);
                        }
                        catch { }
                    }
                }
            }
        }

        /*
         *  公用funciton
         */
        public string test_function(string x)
        {
            string http = "http://protecttw.cloudapp.net/Service1.svc/Login_Post";

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(x);
            Stream response_stream = send_request_post_json(http, json);
            StreamReader streamReader = new StreamReader(response_stream);
            string text = streamReader.ReadToEnd();
            return text;
        }

        private Stream send_request_post_json(string url, string json)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            byte[] byteData = UTF8Encoding.UTF8.GetBytes(json);

            // Set the content length in the request headers  
            httpWebRequest.ContentLength = byteData.Length;

            using (Stream postStream = httpWebRequest.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }
            //StreamWriter writer = new StreamWriter(httpWebRequest.GetRequestStream());
            //writer.Write(json);
            //writer.Close();


            HttpWebResponse resp = (HttpWebResponse)httpWebRequest.GetResponse();
            return resp.GetResponseStream();
        }
    }
}
