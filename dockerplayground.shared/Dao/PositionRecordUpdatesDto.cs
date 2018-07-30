using DockerPlayground.Shared.Dao;
using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.EventStore
{
    public class PositionRecordUpdatesDto
    {
        public static PositionRecordUpdatesDto Empty = new PositionRecordUpdatesDto(new List<PositionRecordUpdateDto>(), false, false);
     
        public PositionRecordUpdatesDto(List<PositionRecordUpdateDto> updates, bool isState, bool isStale)
        {
            Updates = updates ?? new List<PositionRecordUpdateDto>();
            IsCacheState = isState;
            IsStale = isStale;
        }

        public List<PositionRecordUpdateDto> Updates { get; }
        public bool IsCacheState { get; }
        public bool IsStale { get; }
    }
}
