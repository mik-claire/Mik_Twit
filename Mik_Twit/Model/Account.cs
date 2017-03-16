using CoreTweet;

namespace Mik_Twit.Model
{
    public class Account
    {
        public long UserId { get; set; }
        public string ScreenName { get; set; }
        public string Name { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }

        public Tokens TokensData { get; set; }
        public User UserData { get; set; }

        public string IconUrl { get; set; }

        public Account(Tokens tokens, User user)
        {
            this.UserId = (long)user.Id;
            this.ScreenName = user.ScreenName;
            this.Name = user.Name;
            this.AccessToken = tokens.AccessToken;
            this.AccessTokenSecret = tokens.AccessTokenSecret;

            this.TokensData = tokens;
            this.UserData = user;

            this.IconUrl = this.UserData.ProfileImageUrl;
        }
    }
}
