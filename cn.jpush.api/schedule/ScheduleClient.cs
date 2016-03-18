﻿using cn.jpush.api.common;
using cn.jpush.api.push.notification;
using cn.jpush.api.push.mode;
using cn.jpush.api.util;
using cn.jpush.api.schedule;
using cn.jpush.api.push;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cn.jpush.api.schedule
{
    public class ScheduleClient : BaseHttpClient
    {
        private const String HOST_NAME_SSL = "https://api.jpush.cn";
        private const String PUSH_PATH = "/v3/schedules";
        private const String DELETE_PATH = "/";
        private const String PUT_PATH = "/";
        private const String GET_PATH = "?page=";
        private JsonSerializerSettings jSetting;
        private String appKey;
        private String masterSecret;

        public ScheduleClient(String appKey, String masterSecret)
        {
            this.appKey = appKey;
            this.masterSecret = masterSecret;
        }
        public ScheduleResult sendSchedule(SchedulePayload schedulepayload)
        {       
            Preconditions.checkArgument(schedulepayload != null, "schedulepayload should not be empty");
            schedulepayload.Check();
            String schedulepayloadJson = schedulepayload.ToJson();
            Console.WriteLine(schedulepayloadJson);
            return sendSchedule(schedulepayloadJson);
        }

        public ScheduleResult sendSchedule(string schedulepayload)
        {
            
            Preconditions.checkArgument(!string.IsNullOrEmpty(schedulepayload), "schedulepayload should not be empty");
            Console.WriteLine(schedulepayload);
            String url = HOST_NAME_SSL;
            url += PUSH_PATH;
            ResponseWrapper result = sendPost(url, Authorization(), schedulepayload);
            ScheduleResult messResult = new ScheduleResult();
            messResult.ResponseResult = result;

            ScheduleSuccess scheduleSuccess = JsonConvert.DeserializeObject<ScheduleSuccess>(result.responseContent);
            messResult.schedule_id = scheduleSuccess.schedule_id;
            messResult.name = scheduleSuccess.name;
            return messResult;
        }


        //GET /v3/schedules?page=
        public getScheduleResult getSchedule(string pageid)
        {
            jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;
            jSetting.DefaultValueHandling = DefaultValueHandling.Ignore;
            Console.WriteLine(pageid);
            String url = HOST_NAME_SSL;
            url += PUSH_PATH;
            url += GET_PATH;
            if (pageid != null)
            {
                url += pageid;
            }
            ResponseWrapper result = sendGet(url, Authorization(), pageid);
            getScheduleResult messResult = new getScheduleResult();
            messResult.ResponseResult = result;

            getScheduleSuccess getScheduleSuccess = JsonConvert.DeserializeObject<getScheduleSuccess>(result.responseContent, jSetting);
            
            messResult.page = getScheduleSuccess.page;
            messResult.total_pages = getScheduleSuccess.total_pages;
            messResult.total_count = getScheduleSuccess.total_count;
            messResult.schedules = getScheduleSuccess.schedules;
            return messResult;
        }


        //PUT  https://api.jpush.cn/v3/schedules/{schedule_id}

        public ScheduleResult putSchedule(SchedulePayload schedulepayload,String schedule_id)
        {
            Preconditions.checkArgument(schedulepayload != null, "schedulepayload should not be empty");
            //schedulepaload has to be checked,but has not to have all the arg
            //schedulepayload.Check();
            String schedulepayloadJson = schedulepayload.ToJson();
            Console.WriteLine(schedulepayloadJson);
            return putSchedule(schedulepayloadJson,schedule_id);
        }

        public ScheduleResult putSchedule(string schedulepayload, String schedule_id)
        {
            Preconditions.checkArgument(!string.IsNullOrEmpty(schedulepayload), "schedulepayload should not be empty");
            Console.WriteLine(schedulepayload);
            String url = HOST_NAME_SSL;
            url += PUSH_PATH;
            url += PUT_PATH;
            url += schedule_id;
            ResponseWrapper result = sendPut(url, Authorization(), schedulepayload);
            ScheduleResult messResult = new ScheduleResult();
            messResult.ResponseResult = result;

            ScheduleSuccess scheduleSuccess = JsonConvert.DeserializeObject<ScheduleSuccess>(result.responseContent);
            messResult.schedule_id = scheduleSuccess.schedule_id;
            messResult.name = scheduleSuccess.name;

            return messResult;
        }

        //  DELETE https://api.jpush.cn/v3/schedules/{schedule_id} 

        public ScheduleResult deleteSchedule(string schedule_id)
        {

            Console.WriteLine(schedule_id);
            String url = HOST_NAME_SSL;
            url += PUSH_PATH;
            url += DELETE_PATH;
            url += schedule_id;
            ResponseWrapper result = sendDelete(url, Authorization(), schedule_id);
            ScheduleResult messResult = new ScheduleResult();
            messResult.ResponseResult = result;

            ScheduleSuccess scheduleSuccess = JsonConvert.DeserializeObject<ScheduleSuccess>(result.responseContent);
            //messResult.schedule_id = scheduleSuccess.schedule_id;
            //messResult.name = scheduleSuccess.name;

            return messResult;
        }


        private String Authorization()
        {

            Debug.Assert(!string.IsNullOrEmpty(this.appKey));
            Debug.Assert(!string.IsNullOrEmpty(this.masterSecret));

            String origin = this.appKey + ":" + this.masterSecret;
            return Base64.getBase64Encode(origin);
        }


    }




}