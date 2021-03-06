﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App1
{
  
        [Activity(Label = "decode")]
        public class decode : Activity
        {
            //int count = 1;
            public static readonly int PickImageId = 1000;
            ImageView imageView;
            protected override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);

                // Create your application here
                SetContentView(Resource.Layout.decode);

              
                var btnGaleri = FindViewById<Button>(Resource.Id.btnGaleri);
                imageView = FindViewById<ImageView>(Resource.Id.imageView);

                
                btnGaleri.Click += BtnGaleri_Click;
            }

            private void BtnGaleri_Click(object sender, EventArgs e)
            {
                Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);
            }

            protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
            {
                if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
                {
                    Android.Net.Uri uri = data.Data;
                    imageView.SetImageURI(uri);
                }
             
            }

          
        }
    
}