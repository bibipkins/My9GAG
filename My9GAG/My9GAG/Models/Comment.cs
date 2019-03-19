
namespace My9GAG.Models
{
    public class Comment
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public string Text { get; set; }
        public int LikesCount { get; set; }
    }
}
