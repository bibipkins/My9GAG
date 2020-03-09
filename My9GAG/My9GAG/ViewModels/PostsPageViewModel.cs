using My9GAG.Logic.Client;
using My9GAG.Logic.DownloadManager;
using My9GAG.Logic.PageNavigator;
using My9GAG.Models;
using NineGagApiClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
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

            Posts = new ObservableCollection<PostView>();

            InitCommands();

            GetPostsAsync(_currentCategory);
        }

        #endregion

        #region Properties

        public ObservableCollection<PostView> Posts
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
                    Device.BeginInvokeOnMainThread(() => UpdateCommands());
                }
            }
        }
        public bool ArePostsLoading
        {
            get;
            set;
        }
        public PostView LastPost
        {
            get { return _lastPost; }
            set { SetProperty(ref _lastPost, value); }
        }
        public PostView CurrentPost
        {
            get { return _currentPost; }
            set
            {
                var oldCurrentPost = _currentPost;

                if (SetProperty(ref _currentPost, value))
                {
                    LastPost = oldCurrentPost;
                    LastPost?.Pause();

                    int currentPosition = Posts.IndexOf(value);
                    int postsLeft = Posts.Count - currentPosition - 1;
                    bool needToLoadMore = postsLeft <= NUMBER_OF_POSTS_BEFORE_LOADING_MORE;

                    Debug.WriteLine($"Position:{currentPosition} Left:{postsLeft}/{Posts.Count} Id:{CurrentPost?.Id} Last:{LastPost?.Id} Name:{CurrentPost.Title}");

                    if (Posts.Count > 0 && needToLoadMore && !ArePostsLoading)
                    {
                        GetMorePostsAsync();
                    }
                }
            }
        }

        #endregion

        #region Methods

        public async void GetPostsAsync(PostCategory postCategory)
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

            ArePostsLoading = true;

            await Task.Run(async () =>
            {
                var requestStatus = await _clientService.GetPostsAsync(postCategory, NUMBER_OF_POSTS);

                if (requestStatus != null && requestStatus.IsSuccessful)
                {
                    _currentCategory = postCategory;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Posts.Clear();
                        Posts = new ObservableCollection<PostView>();

                        _clientService.Posts.ToList().ForEach(post =>
                            Posts.Add(PostViewFactory.CreatePostViewFromPost(post)));

                        CurrentPost = Posts.FirstOrDefault();
                    });

                    await Task.Delay(ViewModelConstants.GET_POSTS_DELAY);
                }
                else
                {
                    StopWorkIndication();
                    string message = requestStatus == null ? ViewModelConstants.REQUEST_FAILED_MESSAGE : requestStatus.Message;
                    await ShowMessage(message);
                }
            });

            ArePostsLoading = false;

            StopWorkIndication();
        }
        public async void GetMorePostsAsync()
        {
            Debug.WriteLine("Getting more posts");

            if (Posts.Count < 1)
            {
                await ShowMessage(ViewModelConstants.EMPTY_POST_LIST_MESSAGE);
                return;
            }

            ArePostsLoading = true;

            await Task.Run(async () =>
            {
                var requestStatus = await _clientService.GetPostsAsync(_currentCategory, NUMBER_OF_POSTS, Posts.Last().Id);
                
                if (requestStatus != null && requestStatus.IsSuccessful)
                {
                    Device.BeginInvokeOnMainThread(() =>
                        _clientService.Posts.ToList().ForEach(post => 
                            Posts.Add(PostViewFactory.CreatePostViewFromPost(post))));
                }
                else
                {
                    string message = requestStatus == null ? 
                        ViewModelConstants.REQUEST_FAILED_MESSAGE : 
                        requestStatus.Message;

                    await ShowMessage(message);
                }
            });

            ArePostsLoading = false;
        }
        public async void GetCommentsAsync()
        {
            StartWorkIndication(ViewModelConstants.LOADING_COMMENTS);

            await Task.Run(async () =>
            {
                var requestStatus = await _clientService.GetCommentsAsync(CurrentPost.Url, CurrentPost.CommentsCount);

                if (requestStatus != null && requestStatus.IsSuccessful)
                {
                    var viewModel = new CommentsPageViewModel()
                    {
                        Title = CurrentPost.Title,
                        Comments = new ObservableCollection<Comment>(_clientService.Comments)
                    };

                    _pageNavigator.GoToCommentsPage(viewModel);
                }
                else
                {
                    await ShowMessage(requestStatus.Message);
                }
            });

            StopWorkIndication();
        }
        public async void DownloadCurrentPostAsync()
        {
            await Task.Run(async () =>
            {
                string url = CurrentPost.PostMedia.Url;
                string fileName = GenerateFileName(CurrentPost);

                var downloadManager = DependencyService.Get<IDownloadManager>();

                try
                {
                    downloadManager.DownloadFile(url, fileName);
                }
                catch
                {
                    await ShowMessage(ViewModelConstants.DOWNLOAD_FAILED_MESSAGE);
                }
            });
        }
        public async void OpenInBrowserAsync()
        {
            var uri = new Uri(CurrentPost.Url);
            await Launcher.OpenAsync(uri);
        }

        public async void Logout()
        {
            IsNotLoggedIn = true;
            await _clientService.Logout();
            _pageNavigator.GoToLoginPage(null, true);
        }
        public void SaveState(IDictionary<string, object> dictionary)
        {
            dictionary["isNotLoggedIn"] = IsNotLoggedIn;
        }
        public void RestoreState(IDictionary<string, object> dictionary)
        {
            IsNotLoggedIn = GetDictionaryEntry(dictionary, "isNotLoggedIn", true);
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

        public ICommand OpenInBrowserCommand
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
        public ICommand LogoutCommand
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
                () => GetPostsAsync(PostCategory.Hot), 
                () => !IsNotLoggedIn && !IsWorkIndicationVisible);
            GetTrendingPostsCommand = new Command(
                () => GetPostsAsync(PostCategory.Trending), 
                () => !IsNotLoggedIn && !IsWorkIndicationVisible);
            GetFreshPostsCommand = new Command(
                () => GetPostsAsync(PostCategory.Vote), 
                () => !IsNotLoggedIn && !IsWorkIndicationVisible);
            OpenInBrowserCommand = new Command(
                () => OpenInBrowserAsync(),
                () => !IsNotLoggedIn && !IsWorkIndicationVisible && CurrentPost != null);
            DownloadCommand = new Command(
                () => DownloadCurrentPostAsync(),
                () => !IsNotLoggedIn && !IsWorkIndicationVisible && CurrentPost != null);
            CommentsCommand = new Command(
                () => GetCommentsAsync(),
                () => !IsNotLoggedIn && !IsWorkIndicationVisible && CurrentPost != null);
            LogoutCommand = new Command(
                () => Logout(),
                () => !IsWorkIndicationVisible);

            _commands = new List<ICommand>()
            {
                LogInCommand,
                GetHotPostsCommand,
                GetTrendingPostsCommand,
                GetFreshPostsCommand,
                OpenInBrowserCommand,
                DownloadCommand,
                CommentsCommand,
                LogoutCommand
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

        private string GenerateFileName(Post post)
        {
            string title = post.Title.Trim().Replace(' ', '-').ToLower();
            string name = title.Length > MAX_FILE_NAME_LENGTH ? title.Substring(0, MAX_FILE_NAME_LENGTH) : title;
            string extention = post.PostMedia.Url.Split('.').Last();
            return $"{name}.{extention}";
        }

        #endregion

        #region Fields

        private readonly IClientService _clientService;
        private readonly IPageNavigator _pageNavigator;

        private PostCategory _currentCategory;
        private ObservableCollection<PostView> _posts;
        private List<ICommand> _commands;

        private bool _isNotLoggedIn;
        private PostView _lastPost;
        private PostView _currentPost;

        #endregion

        #region Constants

        private const int NUMBER_OF_POSTS = 20;
        private const int NUMBER_OF_POSTS_BEFORE_LOADING_MORE = 15;
        private const int MAX_FILE_NAME_LENGTH = 30;

        #endregion
    }
}
