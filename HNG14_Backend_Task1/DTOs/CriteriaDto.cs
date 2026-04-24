namespace HNG14_Backend_Task2.DTOs
{
    public class CriteriaDto
    {
        public string? LeftExpression { get; set; } = null;
        public string? Comparator { get; set; } = null;
        public string? RightExpression { get; set; } = null;
        public string? LogicalOperator { get; set; } = null;
        public int CriteriaIndex { get; set; }
    }
}
