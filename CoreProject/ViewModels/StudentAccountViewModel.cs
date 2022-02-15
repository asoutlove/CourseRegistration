using System;

namespace CoreProject.ViewModels
{
    public class StudentAccountViewModel
    {
        public Guid Student_ID { get; set; }
        public string S_FirstName { get; set; }
        public string S_LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Idn { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string CellPhone { get; set; }
        public int Education { get; set; }
        public int? School_ID { get; set; }
        public bool Experience { get; set; }
        public int? ExYear { get; set; }
        public bool gender { get; set; }
        public string PassNumber { get; set; }
        public string PassPic { get; set; }
        public Guid b_empno { get; set; }
        public DateTime b_date { get; set; }
        public Guid? e_empno { get; set; }
        public DateTime? e_date { get; set; }
        public Guid? d_empno { get; set; }
        public DateTime? d_date { get; set; }

        public Guid Acc_sum_ID { get; set; }
        public string Account { get; set; }
        public string password { get; set; }
    }
}
