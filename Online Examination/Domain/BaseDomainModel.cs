namespace Online_Examination.Domain
{
    public abstract class BaseDomainModel
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}