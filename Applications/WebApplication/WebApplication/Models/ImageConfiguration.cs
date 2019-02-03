namespace WebApplication.Models
{
    public class ImageConfiguration
    {
        public string ImageUri { get; set; }

        public ImageConfigurationName Name { get; set; }

        public string ContainerName { get; set; }

        public bool Interactive { get; set; }

        public bool PseudoTerminal { get; set; }
    }
}