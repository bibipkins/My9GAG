using My9GAG.Models.Request;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace My9GAG.Views.Behaviors
{
    public class WebViewNavigatingBehavior : Behavior<WebView>
    {
        #region BindableProperties

        public static readonly BindableProperty CommandProperty =
                BindableProperty.Create("Command", typeof(ICommand), typeof(WebViewNavigatingBehavior), null);

        #endregion

        #region Properties

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public WebView AssociatedObject { get; private set; }

        #endregion

        #region Implementation

        protected override void OnAttachedTo(WebView bindable)
        {
            base.OnAttachedTo(bindable);

            AssociatedObject = bindable;

            bindable.BindingContextChanged += Bindable_BindingContextChanged;
            bindable.Navigating += Bindable_Navigating;
        }
        protected override void OnDetachingFrom(WebView bindable)
        {
            base.OnDetachingFrom(bindable);

            bindable.BindingContextChanged -= Bindable_BindingContextChanged;
            bindable.Navigating -= Bindable_Navigating;

            AssociatedObject = null;
        }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            BindingContext = AssociatedObject.BindingContext;
        }

        private void Bindable_BindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }
        private void Bindable_Navigating(object sender, WebNavigatingEventArgs e)
        {
            Debug.WriteLine(e.Url);

            if (Command == null)
            {
                return;
            }

            if (Command.CanExecute(e.Url))
            {
                string code = RequestUtils.ExtractValueFromUrl(e.Url, CODE_ATTRIBUTE_KEY);

                if (!string.IsNullOrWhiteSpace(code))
                {
                    e.Cancel = true;
                    Command.Execute(code);
                }                
            }
        }

        #endregion

        #region Constants

        private const string CODE_ATTRIBUTE_KEY = "code";

        #endregion
    }
}
