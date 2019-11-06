using System;
using System.Collections.Generic;
using System.Text;

namespace My9GAG.Models.Comment
{
    public class CommentMedia
    {
        public CommentMediaData ImageMetaByType
        {
            get;
            set;
        }
    }

    public class CommentMediaData
    {
        public CommentMediaElement Image
        {
            get;
            set;
        }
        public CommentMediaElement Animated
        {
            get;
            set;
        }
        public CommentMediaElement Video
        {
            get;
            set;
        }
    }

    public class CommentMediaElement
    {
        public double Width
        {
            get;
            set;
        }
        public double Height
        {
            get;
            set;
        }
        public string Url
        {
            get;
            set;
        }
    }
}
