﻿using Xamarin.Essentials;

namespace RealEstateApp.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new RealEstateApp.App());
        }
    }
}
