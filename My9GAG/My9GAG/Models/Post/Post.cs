using My9GAG.Models.Post.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace My9GAG.Models.Post
{
    public class Post : SimplePost
    {
        public Post()
            : base()
        {
        }
        public IPostMedia PostMedia { get; set; }
    }
}
