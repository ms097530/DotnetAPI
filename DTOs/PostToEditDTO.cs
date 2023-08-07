using System;
namespace DotnetAPI.DTOs
{
    public partial class PostToEditDTO
    {
        // * want to keep PostId in this DTO so we know which post to edit
        public int PostId { get; set; }

        public string PostTitle { get; set; } = "";
        public string PostContent { get; set; } = "";

        // private string _postTitle = "";
        // private string _postContent = "";
        // public string PostTitle
        // {
        //     get
        //     {
        //         return _postTitle;
        //     }
        //     set
        //     {
        //         _postTitle = value.Replace("'", "\'");
        //     }
        // }
        // public string PostContent
        // {
        //     get
        //     {
        //         return _postContent;
        //     }
        //     set
        //     {
        //         _postContent = value.Replace("'", "\'");
        //     }
        // }
    }
}