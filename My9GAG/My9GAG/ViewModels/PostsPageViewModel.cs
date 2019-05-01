using My9GAG.Logic;
using My9GAG.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace My9GAG.ViewModels
{
    public class PostsPageViewModel : ViewModelBase
    {
        #region Constructors

        public PostsPageViewModel()
        {
            _my9GAGClient = new Client();
            _currentCategory = PostCategory.Hot;
            Posts = new ObservableCollection<Post>();
            InitCommands();
        }

        #endregion

        #region Events

        public EventHandler<CommentsPageViewModel> OnOpenCommentsPage;

        #endregion

        #region Properties

        public ObservableCollection<Post> Posts
        {
            get { return _posts; }
            private set { SetProperty(ref _posts, value); }
        }
        public bool IsNotLoggedIn
        {
            get { return _isNotLoggedIn; }
            set
            {
                if (SetProperty(ref _isNotLoggedIn, value))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UpdateCommands();
                    });
                }
            }
        }
        public string LogInErrorMessage
        {
            get { return _logInErrorMessage; }
            set { SetProperty(ref _logInErrorMessage, value); }
        }
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }
        public int Position
        {
            get { return _position; }
            set
            {
                if (SetProperty(ref _position, value))
                {
                    if (Posts.Count > _lastPosition && _lastPosition >= 0)
                    {
                        Posts[_lastPosition].PostMedia.Stop();
                    }

                    _lastPosition = value;

                    //if (value - _startPosition >= NUMBER_OF_POSTS_BEFORE_UNLOADING_OLD)
                    //{
                    //    Debug.WriteLine($"{value}, from:{_startPosition}, to:{_startPosition + NUMBER_OF_POSTS_TO_UNLOAD}");
                    //    UnloadPosts(_startPosition, _startPosition + NUMBER_OF_POSTS_TO_UNLOAD);
                    //    _startPosition += NUMBER_OF_POSTS_TO_UNLOAD;
                    //}

                    if (Posts.Count > 0 && value > 0 && value >= Posts.Count - NUMBER_OF_POSTS_BEFORE_LOADING_MORE)
                    {
                        GetMorePostsAsync();
                    }
                }
            }
        }

        #endregion

        #region Methods

        public async Task LoginAsync()
        {
            StartWorkIndication(ViewModelConstants.LOGIN_MESSAGE);
            
            await Task.Run(async () =>
            {
                var requestStatus = await _my9GAGClient.LoginAsync(_userName, _password);

                StopWorkIndication();
                IsNotLoggedIn = !requestStatus.IsSuccessful;
                LogInErrorMessage = requestStatus.Message;

                await Task.Delay(ViewModelConstants.LOGIN_DELAY);

                if (requestStatus.IsSuccessful && Posts.Count == 0)
                {
                    StopWorkIndication();
                    await GetPostsAsync(_currentCategory);
                }
            });

            StopWorkIndication();
        }
        public async Task GetPostsAsync(PostCategory postCategory)
        {
            switch (postCategory)
            {
                case PostCategory.Hot:
                    StartWorkIndication(ViewModelConstants.LOADING_HOT_MESSAGE);
                    break;
                case PostCategory.Trending:
                    StartWorkIndication(ViewModelConstants.LOADING_TRENDING_MESSAGE);
                    break;
                case PostCategory.Vote:
                    StartWorkIndication(ViewModelConstants.LOADING_FRESH_MESSAGE);
                    break;
            }

            await Task.Run(async () =>
            {
                RequestStatus requestStatus = await _my9GAGClient.GetPostsAsync(postCategory, NUMBER_OF_POSTS);

                if (requestStatus != null && requestStatus.IsSuccessful)
                {
                    var loadedPosts = new ObservableCollection<Post>(_my9GAGClient.Posts);
                    _currentCategory = postCategory;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Position = 0;
                        Posts = loadedPosts;
                    });

                    await Task.Delay(ViewModelConstants.GET_POSTS_DELAY);
                }
                else
                {
                    StopWorkIndication();
                    string message = requestStatus == null ? ViewModelConstants.REQUEST_FAILED_MESSAGE : requestStatus.Message;
                    ShowMessage(message, ViewModelConstants.MESSAGE_DELAY);
                }
            });

            StopWorkIndication();
        }
        public async Task GetMorePostsAsync()
        {
            if (Posts.Count < 1)
            {
                ShowMessage(ViewModelConstants.EMPTY_POST_LIST_MESSAGE, ViewModelConstants.MESSAGE_DELAY);
                return;
            }

            await Task.Run(async () =>
            {
                RequestStatus requestStatus = await _my9GAGClient.GetPostsAsync(_currentCategory, NUMBER_OF_POSTS, Posts[Posts.Count - 1].Id);
                
                if (requestStatus != null && requestStatus.IsSuccessful)
                {
                    var loadedPosts = new ObservableCollection<Post>(_my9GAGClient.Posts);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        foreach (var post in loadedPosts)
                        {
                            Posts.Add(post);
                        }
                    });
                }
                else
                {
                    string message = requestStatus == null ? ViewModelConstants.REQUEST_FAILED_MESSAGE : requestStatus.Message;
                    ShowMessage(message, ViewModelConstants.MESSAGE_DELAY);
                }
            });
        }
        public async Task GetCommentsAsync()
        {
            StartWorkIndication("Getting comments");

            await Task.Run(async () =>
            {
                Debug.WriteLine("Started GetComments");
                uint count = (uint)Posts[Position].CommentCount;
                string postUrl = Posts[Position].Url;
                RequestStatus requestStatus = await _my9GAGClient.GetCommentsAsync(postUrl, count);

                if (requestStatus != null && requestStatus.IsSuccessful)
                {
                    var viewModel = new CommentsPageViewModel(_my9GAGClient.Comments)
                    {
                        Title = Posts[Position].Title
                    };
                    OnOpenCommentsPage(this, viewModel);
                }
                else
                {
                    ShowMessage(requestStatus.Message, ViewModelConstants.MESSAGE_DELAY);
                }
                
                Debug.WriteLine("Finished GetComments");
            });

            StopWorkIndication();
        }

        public void DownloadCurrentPost()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(10), () =>
            {
                if (Posts == null || Posts.Count <= Position)
                    return false;

                string url = Posts[Position].PostMedia.Url;
                string fileName = GetPostFileName(Posts[Position]);

                var downloadManager = DependencyService.Get<IDownloadManager>();

                try
                {
                    downloadManager.DownloadFile(url, fileName);
                }
                catch (Exception e)
                {
                    ShowMessage(e.Message, ViewModelConstants.MESSAGE_DELAY);
                }

                return false;
            });
        }
        public void SaveState(IDictionary<string, object> dictionary)
        {
            dictionary["isNotLoggedIn"] = IsNotLoggedIn;
            dictionary["userName"] = UserName;
            dictionary["password"] = Password;
        }
        public void RestoreState(IDictionary<string, object> dictionary)
        {
            IsNotLoggedIn = GetDictionaryEntry(dictionary, "isNotLoggedIn", true);
            UserName = GetDictionaryEntry(dictionary, "userName", "");
            Password = GetDictionaryEntry(dictionary, "password", "");
            
            LoginAsync();
        }

        #endregion

        #region Commands

        public ICommand LogInCommand
        {
            get;
            protected set;
        }
        public ICommand GetHotPostsCommand
        {
            get;
            protected set;
        }
        public ICommand GetTrendingPostsCommand
        {
            get;
            protected set;
        }
        public ICommand GetFreshPostsCommand
        {
            get;
            protected set;
        }
        public ICommand SearchCommand
        {
            get;
            protected set;
        }
        public ICommand DownloadCommand
        {
            get;
            protected set;
        }
        public ICommand CommentsCommand
        {
            get;
            protected set;
        }
        public ICommand RelogInCommand
        {
            get;
            protected set;
        }

        #endregion

        #region Implementation

        protected void UnloadPosts(int from, int to)
        {
            if (from >= to || from < 0 || to > Posts.Count)
            {
                return;
            }

            for (int i = from; i < to; i++)
            {
                Posts[i].PostMedia.Unload();
            }
        }

        protected void InitCommands()
        {
            LogInCommand = new Command(
                () => { LoginAsync(); });
            GetHotPostsCommand = new Command(
                () => { GetPostsAsync(PostCategory.Hot); }, 
                () => { return !IsNotLoggedIn && !IsWorkIndicationVisible; });
            GetTrendingPostsCommand = new Command(
                () => { GetPostsAsync(PostCategory.Trending); }, 
                () => { return !IsNotLoggedIn && !IsWorkIndicationVisible; });
            GetFreshPostsCommand = new Command(
                () => { GetPostsAsync(PostCategory.Vote); }, 
                () => { return !IsNotLoggedIn && !IsWorkIndicationVisible; });
            DownloadCommand = new Command(
                () => { DownloadCurrentPost(); },
                () => { return !IsNotLoggedIn && !IsWorkIndicationVisible; });
            CommentsCommand = new Command(
                () => { GetCommentsAsync(); },
                () => { return !IsNotLoggedIn && !IsWorkIndicationVisible; });
            RelogInCommand = new Command(
                () => { IsNotLoggedIn = true; },
                () => { return !IsWorkIndicationVisible; });

            _commands = new List<ICommand>()
            {
                LogInCommand,
                GetHotPostsCommand,
                GetTrendingPostsCommand,
                GetFreshPostsCommand,
                DownloadCommand,
                CommentsCommand,
                RelogInCommand
            };
        }
        protected override void UpdateCommands()
        {
            foreach (var c in _commands)
            {
                if (c is Command command)
                {
                    command.ChangeCanExecute();
                }
            }
        }
        protected T GetDictionaryEntry<T>(IDictionary<string, object> dictionary, string key, T defaultValue)
        {
            if (dictionary.ContainsKey(key))
                return (T)dictionary[key];

            return defaultValue;
        }
        protected string GetPostFileName(Post post)
        {
            if (post == null)
            {
                return String.Empty;
            }

            string[] splittedUrl = post.PostMedia.Url.Split('/');
            var result = splittedUrl[splittedUrl.Length - 1];

            return result;
        }

        #endregion

        #region Fields

        private Client _my9GAGClient;
        private bool _isNotLoggedIn;
        private string _logInErrorMessage;
        private string _userName;
        private string _password;
        private int _position = 0;
        private int _lastPosition = 0;
        private int _startPosition = 0;
        private int _endPosition = 0;
        private List<ICommand> _commands;

        private PostCategory _currentCategory;
        private ObservableCollection<Post> _posts;

        #endregion

        #region Constants

        private const int NUMBER_OF_POSTS = 10;
        private const int NUMBER_OF_POSTS_BEFORE_LOADING_MORE = 5;
        private const int NUMBER_OF_POSTS_BEFORE_UNLOADING_OLD = 30;
        private const int NUMBER_OF_POSTS_TO_UNLOAD = 10;

        #endregion
    }
}
