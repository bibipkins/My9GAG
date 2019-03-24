﻿using My9GAG.Logic;
using My9GAG.Models;
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

        public PostsPageViewModel()
        {
            _my9GAGClient = new Client();
            Posts = new ObservableCollection<MediaPost>();
            NumberOfPosts = 15;
            InitCommands();
        }

        #endregion

        #region Events

        public delegate void ShowComments(CommentsPageViewModel viewModel);
        public event ShowComments OnShowComments;

        #endregion

        #region Methods

        public async void LoginAsync()
        {
            StartWorkIndication("Logging in");

            await Task.Run(async () =>
            {
                var requestStatus = await _my9GAGClient.LoginAsync(_userName, _password);

                StopWorkIndication();
                IsNotLoggedIn = !requestStatus.IsSuccessful;
                LogInErrorMessage = requestStatus.Message;

                if (requestStatus.IsSuccessful)
                {
                    await GetPostsAsync(PostCategory.Hot);
                }
            });

            StopWorkIndication();
        }
        public async Task GetPostsAsync(PostCategory postCategory, bool loadMore = false)
        {
            if (IsNotLoggedIn)
                return;
            
            if (!loadMore)
                StartWorkIndication("Loading " + postCategory);
            
            await Task.Run(async () =>
            {
                RequestStatus requestStatus = null;

                if (!loadMore)
                {
                    requestStatus = await _my9GAGClient.GetPostsAsync(postCategory, NumberOfPosts);
                }
                else if (Posts.Count > 0)
                {
                    requestStatus = await _my9GAGClient.GetPostsAsync(postCategory, NumberOfPosts, Posts[Posts.Count - 1].Id);
                }
                
                if (requestStatus != null && requestStatus.IsSuccessful)
                {
                    var loadedPosts = new ObservableCollection<MediaPost>(MediaPost.Convert(_my9GAGClient.Posts));
                    _currentCategory = postCategory;
                    
                    if (!loadMore)
                    {
                        Device.BeginInvokeOnMainThread(() => 
                        {
                            Position = 0;
                            Posts = new ObservableCollection<MediaPost>();
                            Posts = loadedPosts;
                        });

                        if (Posts.Count > 0 && Posts[0].Type == PostType.Animated)
                            Posts[0].Reload();
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            for (int i = 0; i < loadedPosts.Count; i++)
                            {
                                Debug.WriteLine(loadedPosts[i].Section);
                                Posts.Add(loadedPosts[i]);
                            }
                        });
                    }
                }
                else
                {
                    string message = requestStatus == null ? "Request failed" : requestStatus.Message;
                    ShowMessage(message, 2000);
                }
                
                StopWorkIndication();
            });
        }
        public async Task GetCommentsAsync()
        {
            if (IsNotLoggedIn)
                return;

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
                    OnShowComments(viewModel);
                }
                else
                {
                    ShowMessage(requestStatus.Message, 2000);
                }

                StopWorkIndication();
                Debug.WriteLine("Finished GetComments");
                return false;
            });
        }

        public void DownloadCurrentPost()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(10), () =>
            {
                if (Posts == null || Posts.Count <= Position)
                    return false;

                string url = Posts[Position].MediaUrl;
                string fileName = GetPostFileName(Posts[Position]);

                IDownloadManager downloadManager = DependencyService.Get<IDownloadManager>();

                try
                {
                    downloadManager.DownloadFile(url, fileName);
                }
                catch (Exception e)
                {
                    ShowMessage(e.Message, 2000);
                }

                return false;
            });
        }
        public string GetPostFileName(Post post)
        {
            if (post == null)
                return String.Empty;

            string[] splittedURl = post.MediaUrl.Split('/');
            var result = splittedURl[splittedURl.Length - 1];
            
            return result;
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

            if (!IsNotLoggedIn)
            {
                LoginAsync();
            }
        }

        #endregion

        #region Properties

        public ObservableCollection<MediaPost> Posts
        {
            get
            {
                return _posts;
            }
            private set
            {
                SetProperty<ObservableCollection<MediaPost>>(ref _posts, value);
            }
        }
        public bool IsNotLoggedIn
        {
            get { return _isNotLoggedIn; }
            set
            {
                if (SetProperty(ref _isNotLoggedIn, value))
                    UpdateCommands();
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
                    if (Posts != null && Posts.Count > _lastPosition && _lastPosition >= 0)
                        Posts[_lastPosition].Stop();

                    if (value >= 0 && value < Posts.Count && Posts[value].Type == PostType.Animated)
                        Device.StartTimer(TimeSpan.FromMilliseconds(10), () =>
                        {
                            Posts[value].Reload();
                            return false;
                        });

                    _lastPosition = value;

                    if (_position == Posts.Count - 1)
                        GetPostsAsync(_currentCategory, true);
                }
            }
        }
        public uint NumberOfPosts
        {
            get { return _numberOfPosts; }
            set { SetProperty(ref _numberOfPosts, value); }
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
        public ICommand ReLogInCommand
        {
            get;
            protected set;
        }

        #endregion

        #region Implementation

        protected void InitCommands()
        {
            LogInCommand = new Command(LoginAsync);
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
            ReLogInCommand = new Command(
                () => { IsNotLoggedIn = true; },
                () => { return !IsWorkIndicationVisible; });
        }
        protected override void UpdateCommands()
        {
            ((Command)GetHotPostsCommand).ChangeCanExecute();
            ((Command)GetTrendingPostsCommand).ChangeCanExecute();
            ((Command)GetFreshPostsCommand).ChangeCanExecute();
            ((Command)DownloadCommand).ChangeCanExecute();
            ((Command)CommentsCommand).ChangeCanExecute();
            ((Command)ReLogInCommand).ChangeCanExecute();
        }
        private T GetDictionaryEntry<T>(IDictionary<string, object> dictionary, string key, T defaultValue)
        {
            if (dictionary.ContainsKey(key))
                return (T)dictionary[key];

            return defaultValue;
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
        private uint _numberOfPosts;

        private PostCategory _currentCategory;
        private ObservableCollection<MediaPost> _posts;

        #endregion
    }
}
