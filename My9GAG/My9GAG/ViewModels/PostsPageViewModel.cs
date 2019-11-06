﻿using My9GAG.Logic.Client;
using My9GAG.Logic.DownloadManager;
using My9GAG.Logic.PageNavigator;
using My9GAG.Models.Comment;
using My9GAG.Models.Post;
using My9GAG.Models.Request;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace My9GAG.ViewModels
{
    public class PostsPageViewModel : ViewModelBase
    {
        #region Constructors

        public PostsPageViewModel(IClientService clientService, IPageNavigator pageNavigator)
        {
            _clientService = clientService;
            _pageNavigator = pageNavigator;
            _currentCategory = PostCategory.Hot;

            Posts = new ObservableCollection<Post>();

            InitCommands();

            GetPostsAsync(_currentCategory);
        }

        #endregion

        #region Properties

        public ObservableCollection<Post> Posts
        {
            get { return _posts; }
            private set { SetProperty(ref _posts, value); Debug.WriteLine(""); Debug.WriteLine("NUMBER OF POSTS: " + _posts.Count); }
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
                    Debug.WriteLine($"Position: {value}, Count: {Posts.Count}");

                    if (Posts.Count > _lastPosition && _lastPosition >= 0)
                    {
                        Posts[_lastPosition].PostMedia.Pause();
                    }

                    _lastPosition = value;

                    if (Posts.Count > 0 && value > 0 && value >= Posts.Count - NUMBER_OF_POSTS_BEFORE_LOADING_MORE)
                    {
                        GetMorePostsAsync();
                    }
                }
            }
        }

        #endregion

        #region Methods

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
                RequestStatus requestStatus = await _clientService.GetPostsAsync(postCategory, NUMBER_OF_POSTS);

                if (requestStatus != null && requestStatus.IsSuccessful)
                {
                    var loadedPosts = new ObservableCollection<Post>(_clientService.Posts);
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
                    await ShowMessage(message, ViewModelConstants.MESSAGE_DELAY);
                }
            });

            StopWorkIndication();
        }
        public async Task GetMorePostsAsync()
        {
            if (Posts.Count < 1)
            {
                await ShowMessage(ViewModelConstants.EMPTY_POST_LIST_MESSAGE, ViewModelConstants.MESSAGE_DELAY);
                return;
            }

            await Task.Run(async () =>
            {
                RequestStatus requestStatus = await _clientService.GetPostsAsync(_currentCategory, NUMBER_OF_POSTS, Posts[Posts.Count - 1].Id);
                
                if (requestStatus != null && requestStatus.IsSuccessful)
                {
                    var loadedPosts = new ObservableCollection<Post>(_clientService.Posts);

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
                    await ShowMessage(message, ViewModelConstants.MESSAGE_DELAY);
                }
            });
        }
        public async Task GetCommentsAsync()
        {
            StartWorkIndication(ViewModelConstants.LOADING_COMMENTS);

            await Task.Run(async () =>
            {
                uint count = (uint)Posts[Position].CommentsCount;
                string postUrl = Posts[Position].Url;
                RequestStatus requestStatus = await this._clientService.GetCommentsAsync(postUrl, count);

                if (requestStatus != null && requestStatus.IsSuccessful)
                {
                    var viewModel = new CommentsPageViewModel()
                    {
                        Title = Posts[Position].Title,
                        Comments = new ObservableCollection<Comment>(_clientService.Comments)
                    };

                    _pageNavigator.GoToCommentsPage(viewModel);
                }
                else
                {
                    await ShowMessage(requestStatus.Message, ViewModelConstants.MESSAGE_DELAY);
                }
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
        public ICommand PositionChangedCommand
        {
            get;
            protected set;
        }

        #endregion

        #region Implementation

        protected void InitCommands()
        {
            LogInCommand = new Command(
                () => {  });
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

        private IClientService _clientService;
        private IPageNavigator _pageNavigator;
        private PostCategory _currentCategory;
        private ObservableCollection<Post> _posts;

        private bool _isNotLoggedIn;
        private string _logInErrorMessage;
        private string _userName;
        private string _password;
        private int _position = 0;
        private int _lastPosition = 0;
        private int _startPosition = 0;
        private int _endPosition = 0;
        private List<ICommand> _commands;

        #endregion

        #region Constants

        private const int NUMBER_OF_POSTS = 20;
        private const int NUMBER_OF_POSTS_BEFORE_LOADING_MORE = 10;

        #endregion
    }
}
