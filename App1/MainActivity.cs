using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;

namespace App1
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var btnEncode = FindViewById<Button>(Resource.Id.btnEncode);
            var btnDecode = FindViewById<Button>(Resource.Id.btnDecode);
            var btnKeluar = FindViewById<Button>(Resource.Id.btnKeluar);
            btnEncode.Click += BtnEncode_Click;
            btnDecode.Click += BtnDecode_Click;
            btnKeluar.Click += BtnKeluar_Click;

        }

        private void BtnKeluar_Click(object sender, System.EventArgs e)
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }

        private void BtnDecode_Click(object sender, System.EventArgs e)
        {
            Intent nextActivity = new Intent(this, typeof(decode));
            StartActivity(nextActivity);
        }

        private void BtnEncode_Click(object sender, System.EventArgs e)
        {
            Intent nextActivity = new Intent(this, typeof(encode));
            StartActivity(nextActivity);
        }
    }
}