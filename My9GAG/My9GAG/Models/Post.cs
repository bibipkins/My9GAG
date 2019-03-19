
namespace My9GAG.Models
{
    public class Post
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public int UpvoteCount { get; set; }
        public int CommentsCount { get; set; }
        public string MediaURL { get; set; }
        public PostType Type { get; set; }
    }
}
