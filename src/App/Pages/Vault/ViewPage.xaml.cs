﻿using Bit.Core.Abstractions;
using Bit.Core.Utilities;
using System.Collections.Generic;

namespace Bit.App.Pages
{
    public partial class ViewPage : BaseContentPage
    {
        private readonly IBroadcasterService _broadcasterService;
        private ViewPageViewModel _vm;

        public ViewPage(string cipherId)
        {
            InitializeComponent();
            _broadcasterService = ServiceContainer.Resolve<IBroadcasterService>("broadcasterService");
            _vm = BindingContext as ViewPageViewModel;
            _vm.Page = this;
            _vm.CipherId = cipherId;
            SetActivityIndicator();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _broadcasterService.Subscribe(nameof(ViewPage), async (message) =>
            {
                if(message.Command == "syncCompleted")
                {
                    var data = message.Data as Dictionary<string, object>;
                    if(data.ContainsKey("successfully"))
                    {
                        var success = data["successfully"] as bool?;
                        if(success.HasValue && success.Value)
                        {
                            await _vm.LoadAsync();
                        }
                    }
                }
            });
            await LoadOnAppearedAsync(_scrollView, true, () => _vm.LoadAsync());
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _broadcasterService.Unsubscribe(nameof(ViewPage));
            _vm.CleanUp();
        }
    }
}