namespace OnlineExamination.Domain

{
    public abstract class BaseDomainModel
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public String? CreatedBy { get; set; }
        public String? UpdatedBy { get; set; }
    }
}