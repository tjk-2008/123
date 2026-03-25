using DirectoryService.Domain.DepartmentsContext.ValueObjects;
using DirectoryService.Domain.PositionsContext.ValueObjects;

namespace DirectoryService.Domain.DepartmentsContext
{
    public class DepartmentPosition
    {
        public DepartmentId DepartmentId { get; }
        public PositionId PositionId { get; }
        public Rank PositionRank { get; private set; }

        public DepartmentPosition(DepartmentId departmentId, PositionId positionId, Rank rank)
        {
            DepartmentId = departmentId;
            PositionId = positionId;
            PositionRank = rank;
        }

        public void ChangeRank(Rank newRank)
        {
            PositionRank = newRank;
        }

        public void IncreaseRank()
        {
            PositionRank = PositionRank.Increase();
        }

        public void DecreaseRank()
        {
            PositionRank = PositionRank.Decrease();
        }
    }
}