using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    public class PositionRecordUpdateDto
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public UpdateTypeDto UpdateType { get; set; }

        public PositionRecordDto PositionRecord { get; set; }

        public override string ToString()
        {
            return $"UpdateType: {UpdateType}, PositionRecord: {PositionRecord}";
        }
    }
}
