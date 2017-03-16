using CoreTweet;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Reactive.Linq;

namespace Mik_Twit.Util
{
    public static class TwitterUtil
    {
        public static Status GetTweetFromId(Tokens token, string tweetId)
        {
            var tweet = token.Statuses.Show(id => tweetId);
            return tweet;
        }

        public static async Task<Status> GetTweetFromIdAsync(Tokens token, string tweetId)
        {
            var tweet = await token.Statuses.ShowAsync(id => tweetId);
            return tweet;
        }

        public static List<Status> GetTalk(Tokens token, Status target)
        {
            List<Status> talk = new List<Status>();
            talk.Add(target);

            Status tweet = target;
            while (tweet.InReplyToStatusId != null)
            {
                tweet = TwitterUtil.GetTweetFromId(token, tweet.InReplyToStatusId.ToString());
                talk.Add(tweet);
            }

            return talk;
        }

        public static async Task<List<Status>> GetTalkAsync(Tokens token, Status target)
        {
            List<Status> talk = new List<Status>();
            talk.Add(target);

            Status tweet = target;
            while (tweet.InReplyToStatusId != null)
            {
                tweet = await TwitterUtil.GetTweetFromIdAsync(token, tweet.InReplyToStatusId.ToString());
                talk.Add(tweet);
            }

            return talk;
        }

        public static async Task<User> GetUserAsync(Tokens token, long userId)
        {
            var user = await token.Users.ShowAsync(id => userId);
            return user;
        }

        public static async Task<User> GetUserAsync(Tokens token, string screenName)
        {
            var user = await token.Users.ShowAsync(screen_name => screenName);
            return user;
        }

        public static async Task<List<Status>> GetUserTweetAsync(Tokens token, long userId, long maxId = 0)
        {
            var param = new Dictionary<string, object>();
            param["count"] = 50;
            param["user_id"] = userId;
            if (maxId != 0)
            {
                param["max_id"] = maxId;
            }

            var statuses = await token.Statuses.UserTimelineAsync(param);
            return statuses.ToList();
        }

        public static async Task<Cursored<User>> GetUserFollowsAsync(Tokens token, long userId, long currentCursor = -1)
        {
            var follow = await token.Friends.ListAsync(user_id => userId, cursor => currentCursor);

            return follow;
        }

        public static async Task<Cursored<User>> GetUserFollowersAsync(Tokens token, long userId, long currentCursor = -1)
        {
            var follower = await token.Followers.ListAsync(user_id => userId, cursor => currentCursor);

            return follower;
        }

        public static async Task<List<DirectMessage>> GetTalkDmAsync(Tokens token, string screenName, long maxId = 0)
        {
            var param = new Dictionary<string, object>();
            param["count"] = 200;
            param["full_text"] = true;
            if (maxId != 0)
            {
                param["max_id"] = maxId;
            }

            var dmRecieved = await token.DirectMessages.ReceivedAsync(param);
            var dmSent = await token.DirectMessages.SentAsync(param);

            List<DirectMessage> dmList = new List<DirectMessage>();
            dmList.AddRange(dmRecieved.Where(x => x.Sender.ScreenName == screenName));
            dmList.AddRange(dmSent.Where(x => x.Recipient.ScreenName == screenName));

            var query = dmList.OrderByDescending(x => x.CreatedAt);
            List<DirectMessage> dm = query.ToList<DirectMessage>();

            return dm;
        }
    }
}
