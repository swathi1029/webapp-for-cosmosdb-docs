using Newtonsoft.Json;

namespace SampleWebApi.Model
{
    public class TaskModel
    {
        [JsonProperty("id")]
        public string? Id {get; set;}
        [JsonProperty("title")]
        public string? Title { get; set; }
        [JsonProperty("description")]
        public string? Description { get; set; }
        [JsonProperty("duedate")]
        public string? DueDate { get; set; }
    }
}
