using My9GAG.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public void LogIn()
        {
            Debug.WriteLine("START LogIn");
            ShowWorkIndication("Logging in");

            Device.StartTimer(TimeSpan.FromMilliseconds(1500), () =>
            {
                RequestStatus requestStatus = _my9GAGClient.Login(_userName, _password);
                if (!requestStatus.IsSuccessful)
                {
                    IsNotLoggedIn = true;
                    LogInErrorMessage = requestStatus.Message;
                }
                else
                {
                    IsNotLoggedIn = false;
                    GetPosts(PostCategory.Hot);
                }

                IsWorking = false;
                Debug.WriteLine("END LogIn");
                return false;
            });
        }
        public void GetPosts(PostCategory postCategory, bool loadMore = false)
        {
            Debug.WriteLine("START GetPosts");
            if (IsNotLoggedIn)
                return;

            if (!loadMore)
                ShowWorkIndication("Loading " + postCategory);

            Device.StartTimer(TimeSpan.FromMilliseconds(1500), () =>
            {
                Debug.WriteLine("1 GetPosts");
                RequestStatus requestStatus = null;

                if (!loadMore)
                {
                    Debug.WriteLine("11 GetPosts");
                    requestStatus = _my9GAGClient.GetPosts(postCategory, NumberOfPosts);
                    Debug.WriteLine("12 GetPosts");
                }
                else if (Posts.Count > 0)
                {
                    Debug.WriteLine("13 GetPosts");
                    requestStatus = _my9GAGClient.GetPosts(postCategory, NumberOfPosts, Posts[Posts.Count - 1].ID);
                    Debug.WriteLine("14 GetPosts");
                }
                    
                Debug.WriteLine("2 GetPosts " + requestStatus?.IsSuccessful);
                if (requestStatus != null && requestStatus.IsSuccessful)
                {
                    Debug.WriteLine("3 GetPosts");
                    var loadedPosts = new ObservableCollection<MediaPost>(MediaPost.Convert(_my9GAGClient.Posts));
                    _currentCategory = postCategory;
                    Debug.WriteLine("4 GetPosts");
                    if (!loadMore)
                    {
                        Debug.WriteLine("5 GetPosts");
                        Position = 0;
                        Posts = new ObservableCollection<MediaPost>();
                        Posts = loadedPosts;

                        if (Posts.Count > 0 && Posts[0].Type == PostType.Animated)
                            Posts[0].Reload();
                        Debug.WriteLine("6 GetPosts");
                    }
                    else
                    {
                        Debug.WriteLine("7 GetPosts");
                        for (int i = 0; i < loadedPosts.Count; i++)
                            Posts.Add(loadedPosts[i]);
                        Debug.WriteLine("8 GetPosts");
                    }                        
                }
                else
                {
                    Debug.WriteLine("9 GetPosts");
                    string message = requestStatus == null ? "Request failed" : requestStatus.Message;
                    DisplayMessage(message);
                    Debug.WriteLine("10 GetPosts");
                }
                    

                IsWorking = false;
                Debug.WriteLine("END GetPosts");
                return false;
            });
        }
        public void GetComments()
        {
            Debug.WriteLine("START GetComments");
            if (IsNotLoggedIn)
                return;

            ShowWorkIndication("Getting comments");

            Device.StartTimer(TimeSpan.FromMilliseconds(1500), () =>
            {
                RequestStatus requestStatus = _my9GAGClient.GetComments(Posts[Position].URL, (uint)Posts[Position].CommentsCount);

                if (requestStatus != null && requestStatus.IsSuccessful)
                {
                    CommentsPageViewModel viewModel = new CommentsPageViewModel(_my9GAGClient.Comments);
                    viewModel.Title = Posts[Position].Title;
                    OnShowComments(viewModel);
                }
                else
                {
                    DisplayMessage(requestStatus.Message);
                }

                IsWorking = false;
                Debug.WriteLine("END GetComments");
                return false;
            });
        }
        public void DownloadCurrentPost()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(10), () =>
            {
                if (Posts == null || Posts.Count <= Position)
                    return false;

                string url = Posts[Position].MediaURL;
                string fileName = GetPostFileName(Posts[Position]);

                IDownloadManager downloadManager = DependencyService.Get<IDownloadManager>();

                try
                {
                    downloadManager.DownloadFile(url, fileName);
                }
                catch (Exception e)
                {
                    DisplayMessage(e.Message);
                }

                return false;
            });
        }
        public string GetPostFileName(Post post)
        {
            Debug.WriteLine("START GetPostFileName");
            if (post == null)
                return String.Empty;

            string[] splittedURl = post.MediaURL.Split('/');
            var result = splittedURl[splittedURl.Length - 1];
            Debug.WriteLine("END GetPostFileName");
            return result;
        }
        public void SaveState(IDictionary<string, object> dictionary)
        {
            Debug.WriteLine("START SaveState");
            dictionary["isNotLoggedIn"] = IsNotLoggedIn;
            dictionary["userName"] = UserName;
            dictionary["password"] = Password;
            Debug.WriteLine("END SaveState");
        }
        public void RestoreState(IDictionary<string, object> dictionary)
        {
            Debug.WriteLine("START RestoreState");
            IsNotLoggedIn = GetDictionaryEntry(dictionary, "isNotLoggedIn", true);
            UserName = GetDictionaryEntry(dictionary, "userName", "");
            Password = GetDictionaryEntry(dictionary, "password", "");

            if (!IsNotLoggedIn)
                LogIn();
            Debug.WriteLine("END RestoreState");
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
        public bool IsWorking
        {
            get { return _isWorking; }
            set
            {
                if (SetProperty(ref _isWorking, value))
                    UpdateCommands();
            }
        }
        public string LoadingText
        {
            get { return _loadingText; }
            set { SetProperty(ref _loadingText, value); }
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
                        GetPosts(_currentCategory, true);
                }
            }
        }
        public uint NumberOfPosts
        {
            get { return _numberOfPosts; }
            set { SetProperty(ref _numberOfPosts, value); }
        }
        public string MessageText
        {
            get { return _messageText; }
            set { SetProperty(ref _messageText, value); }
        }
        public bool ShowMessage
        {
            get { return _showMessage; }
            set { SetProperty(ref _showMessage, value); }
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

        private void InitCommands()
        {
            LogInCommand = new Command(LogIn);
            GetHotPostsCommand = new Command(
                () => { GetPosts(PostCategory.Hot); }, 
                () => { return !IsNotLoggedIn && !IsWorking; });
            GetTrendingPostsCommand = new Command(
                () => { GetPosts(PostCategory.Trending); }, 
                () => { return !IsNotLoggedIn && !IsWorking; });
            GetFreshPostsCommand = new Command(
                () => { GetPosts(PostCategory.Fresh); }, 
                () => { return !IsNotLoggedIn && !IsWorking; });
            DownloadCommand = new Command(
                () => { DownloadCurrentPost(); },
                () => { return !IsNotLoggedIn && !IsWorking; });
            CommentsCommand = new Command(
                () => { GetComments(); },
                () => { return !IsNotLoggedIn && !IsWorking; });
            ReLogInCommand = new Command(
                () => { IsNotLoggedIn = true; },
                () => { return !IsWorking; });
        }
        private void UpdateCommands()
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
        private void ShowWorkIndication(string label)
        {
            IsWorking = true;
            int iteration = 0;
            string indicationText = label;
            LoadingText = indicationText;

            Device.StartTimer(TimeSpan.FromMilliseconds(400), () =>
            {
                indicationText = label;
                for (int i = 0; i < iteration; i++)
                    indicationText += ".";

                LoadingText = indicationText;
                iteration++;

                if (iteration > 2)
                    iteration = 0;

                if (!IsWorking)
                {
                    iteration = 0;
                    LoadingText = "";
                }

                return IsWorking;
            });
        }
        private void DisplayMessage(string label)
        {
            ShowMessage = true;
            MessageText = label;

            Device.StartTimer(TimeSpan.FromMilliseconds(1600), () =>
            {
                ShowMessage = false;
                MessageText = "";
                return ShowMessage;
            });
        }

        #endregion

        #region Fields

        private Client _my9GAGClient;
        private bool _isNotLoggedIn;
        private string _logInErrorMessage;
        private string _userName;
        private string _password;
        private bool _isWorking;
        private string _loadingText;
        private int _position = 0;
        private int _lastPosition = 0;
        private uint _numberOfPosts;
        private string _messageText;
        private bool _showMessage = false;

        private PostCategory _currentCategory;
        private ObservableCollection<MediaPost> _posts;

        #endregion
    }
}
