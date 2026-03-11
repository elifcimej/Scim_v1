namespace Scim_v1.Models
{
    public class ScimUserRequest
    {
        public List<ScimUserName> userName { get; set; }

        public ScimName name { get; set; }

        public bool active { get; set; }
        public List<ScimEmail> emails { get; set; }
    }

    public class ScimUserName
    {
        public string value { get; set; }
    }

    public class ScimName
    {
        public string givenName { get; set; }

        public string familyName { get; set; }
    }
    public class ScimEmail  
    {
        public string value { get; set; }
        public bool primary { get; set; }
    }
}