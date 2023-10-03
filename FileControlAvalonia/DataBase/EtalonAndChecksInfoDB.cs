namespace FileControlAvalonia.DataBase
{
    public class EtalonAndChecksInfoDB
    {
        public string Creator { get; set; }
        public string Date { get; set; }
        public string DateLastCheck { get; set; }
        public int TotalFiles { get; set; }
        public int Checked { get; set; }
        public int PartialChecked { get; set; }
        public int FailedChecked { get; set; }
        public int NoAccess { get; set; }
        public int NotFound { get; set; }
        public int NotChecked { get; set; }

        public EtalonAndChecksInfoDB()
        {

        }
    }
}
