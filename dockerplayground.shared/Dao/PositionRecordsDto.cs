using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    public class PositionRecordsDto
    {
        public static readonly PositionRecordsDto Empty = new PositionRecordsDto(new PositionRecordDto[0], false, false);

        public PositionRecordsDto(IList<PositionRecordDto> positionRecords, bool isState, bool isStale)
        {
            PositionRecords = positionRecords;
            IsCacheState = isState;
            IsStale = isStale;
        }

        public IList<PositionRecordDto> PositionRecords { get; }
        public bool IsCacheState { get; }
        public bool IsStale { get; }
    }
}
