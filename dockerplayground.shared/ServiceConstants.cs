using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    public class ServiceConstants
    {
        public const string PositionRecordCreatedEventType = "PositionRecordCreatedEventType";
        public const string PositionRecordUpdatedEventType = "PositionRecordUpdatedEventType";
       
        public static String EventStoreUrl = "tcp://admin:changeit@eventstore-node:1113";
        public static String MemberServiceUrl = "http://member";
        public static String LocationServiceUrl = "http://location";
        public static String ProximityServiceUrl = "http://proximity";
    }
}
