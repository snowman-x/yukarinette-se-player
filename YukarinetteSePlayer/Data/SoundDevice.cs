namespace YukarinetteSePlayer.Data
{
    public class SoundDevice
    {
        public SoundDevice(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public string Id { get; set; }
        public string Description { get; set; }
    }
}