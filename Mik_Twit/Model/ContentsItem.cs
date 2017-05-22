namespace Mik_Twit.Model
{
    public class ContentsItem
    {
        public string ContentsUrl { get; set; }

        public ContentsItem(string contentsUrl)
        {
            if (!contentsUrl.EndWith(":orig"))
            {
                contentsUrl += ":orig";
            }
            
            this.ContentsUrl = contentsUrl;
        }
    }
}
