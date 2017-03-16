using CoreTweet;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Mik_Twit.Model
{
    public class UserInfoItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            handler(this, new PropertyChangedEventArgs(name));
        }

        private User user;
        private Tokens token;

        public long Id { get; set; }
        public string ScreenName { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        private bool isFollowing = false;
        private bool isFollower = false;
        private bool isBlocking = false;
        public string Profile { get; set; }
        public string Location { get; set; }
        public string Url { get; set; }
        public int TweetCount { get; set; }
        public int FavCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowerCount { get; set; }

        public UserInfoItem(Tokens token, User user)
        {
            this.token = token;
            this.user = user;

            this.Id = (long)user.Id;
            this.ScreenName = user.ScreenName;
            this.Name = user.Name;
            this.IconUrl = user.ProfileImageUrl;
            this.Profile = user.Description;
            this.Location = user.Location;
            this.Url = user.Url;
            this.TweetCount = user.StatusesCount;
            this.FavCount = user.FavouritesCount;
            this.FollowingCount = user.FriendsCount;
            this.FollowerCount = user.FollowersCount;

            setRelationAsync(token, user);
        }

        private async Task setRelationAsync(Tokens token, User user)
        {
            try
            {
                long cur = -1;
                var block = await token.Blocks.IdsAsync(cursor => cur, count => 200);
                while (0 < block.Count)
                {
                    if (block.Contains((long)user.Id))
                    {
                        this.isBlocking = true;
                        OnPropertyChanged("IsFollowingText");
                        OnPropertyChanged("IsFollowerText");
                        return;
                    }
                    cur = block.NextCursor;
                    block = await token.Blocks.IdsAsync(cursor => cur, count => 200);
                }
                OnPropertyChanged("IsEnabledMakeRelation");

                cur = -1;
                var follow = await token.Friends.IdsAsync(cursor => cur, count => 200);
                while (0 < follow.Count)
                {
                    if (follow.Contains((long)user.Id))
                    {
                        this.isFollowing = true;
                        break;
                    }
                    cur = follow.NextCursor;
                    follow = await token.Friends.IdsAsync(cursor => cur, count => 200);
                }
                OnPropertyChanged("IsFollowingText");

                cur = -1;
                var follower = await token.Followers.IdsAsync(cursor => cur, count => 200);
                while (0 < follower.Count)
                {
                    if (follower.Contains((long)user.Id))
                    {
                        this.isFollower = true;
                        break;
                    }
                    cur = follower.NextCursor;
                    follower = await token.Followers.IdsAsync(cursor => cur, count => 200);
                }
                OnPropertyChanged("IsFollowerText");

                OnPropertyChanged("MakeRelation");
            }
            catch (TwitterException tex)
            {
                MessageBox.Show(tex.Message,
                    "Twitter Exception.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        public async Task FollowAsync()
        {
            try
            {
                await this.token.Friendships.CreateAsync(id => this.user.Id);
                this.isFollowing = true;
                OnPropertyChanged("IsFollowingText");
                OnPropertyChanged("MakeRelation");
            }
            catch (TwitterException tex)
            {
                MessageBox.Show(tex.Message,
                    "Twitter Exception.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        public async Task RemoveAsync()
        {
            try
            {
                await this.token.Friendships.DestroyAsync(id => this.user.Id);
                this.isFollowing = false;
                OnPropertyChanged("IsFollowingText");
                OnPropertyChanged("MakeRelation");
            }
            catch (TwitterException tex)
            {
                MessageBox.Show(tex.Message,
                    "Twitter Exception.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        public async Task BlockAsync()
        {
            try
            {
                await this.token.Blocks.CreateAsync(id => this.user.Id);
                this.isFollowing = false;
                this.isFollower = false;
                OnPropertyChanged("IsFollowingText");
                OnPropertyChanged("IsFollowerText");
                this.isBlocking = true;
                OnPropertyChanged("IsEnabledMakeRelation");
            }
            catch (TwitterException tex)
            {
                MessageBox.Show(tex.Message,
                    "Twitter Exception.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        public string ScreenName2
        {
            get
            {
                return "@" + this.ScreenName;
            }
            set { }
        }

        public string Location2
        {
            get
            {
                return "  " + this.Location;
            }
            set { }
        }

        public string Url2
        {
            get
            {
                return "  " + this.Url;
            }
            set { }
        }

        public string TweetCount2
        {
            get
            {
                return "  " + this.TweetCount.ToString();
            }
            set { }
        }

        public string FavCount2
        {
            get
            {
                return "  " + this.FavCount.ToString();
            }
            set { }
        }

        public string FollowingCount2
        {
            get
            {
                return "  " + this.FollowingCount.ToString();
            }
            set { }
        }

        public string FollowerCount2
        {
            get
            {
                return "  " + this.FollowerCount.ToString();
            }
            set { }
        }

        public bool IsFollowing
        {
            get { return this.isFollowing; }
            set { this.isFollowing = value; }
        }

        public bool IsFollower
        {
            get { return this.isFollower; }
            set { this.isFollower = value; }
        }

        public string IsFollowingText
        {
            get
            {
                if (this.isFollowing)
                {
                    return "Following";
                }

                return string.Empty;
            }
            set { }
        }

        public string IsFollowerText
        {
            get
            {
                if (this.isFollower)
                {
                    return "Follower";
                }

                return string.Empty;
            }
            set { }
        }

        public string MakeRelation
        {
            get
            {
                if (this.isFollowing)
                {
                    return "Remove";
                }

                return "Follow";
            }
            set { }
        }

        public bool IsEnabledMakeRelation
        {
            get
            {
                if (this.isBlocking)
                {
                    return false;
                }

                return true;
            }
            set { }
        }

        public string MakeBlock
        {
            get
            {
                if (this.isBlocking)
                {
                    return "UnBlock";
                }

                return "Block";
            }
        }
    }
}
