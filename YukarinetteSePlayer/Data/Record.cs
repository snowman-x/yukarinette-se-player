namespace YukarinetteSePlayer.Data
{
    public class Record
    {
        public RunMode RunMode { get; set; } = RunMode.None;
        public string Keyword { get; set; } = "";
        public string FileName { get; set; } = "";
        public int Latency { get; set; } = 0;
        public ReplaceMode ReplaceMode { get; set; } = ReplaceMode.None;
        public string ReplaceWord { get; set; } = "";
    }
}