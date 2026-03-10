namespace Scım_v1.Models
{
    public class ScimPatchRequest
    {
        public string[] schemas { get; set; }
        public List<PatchOperation> Operations { get; set; }
    }

    public class PatchOperation
    {
        public string op { get; set; }
        public string path { get; set; }
        public object value { get; set; }
    }
}
