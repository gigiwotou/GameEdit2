using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace TCKill
{
    [Activity(Label = "TCKill", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        GLView1 view;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create our OpenGL view, and display it
            view = new GLView1(this);
            SetContentView(view);
        }

        protected override void OnPause()
        {
            base.OnPause();
            view.Pause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            view.Resume();
        }
    }
}

