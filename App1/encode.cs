using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace App1
{
    [Activity(Label = "encode")]
    public class encode : Activity
    {
        //int count = 1;
        public static readonly int PickImageId = 1000;
        ImageView imageView;
        ImageView imageView1;
        string _binImg = "";
        public byte A;
        public byte G;
        public byte R;
        public byte B;
        public Color GetPixel(int x, int y);


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.encode);

            var btnCamera = FindViewById<Button>(Resource.Id.btnCamera);
            var btnGaleri = FindViewById<Button>(Resource.Id.btnGaleri);
            var btnProses = FindViewById<Button>(Resource.Id.btnProses);
            imageView = FindViewById<ImageView>(Resource.Id.imageView);
            imageView1 = FindViewById<ImageView>(Resource.Id.imageView1);
           
            btnCamera.Click += BtnCamera_click;
            
            btnGaleri.Click += BtnGaleri1_Click;
           
            btnProses.Click += BtnProses_Click;


        }

        private void BtnGaleri1_Click(object sender, EventArgs e)
        {
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);
        }

        private void BtnCamera1_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, 0);
        }

        private void BtnProses_Click(object sender, EventArgs e)
        {
            if (imageView.GetDrawableState() == null)
            {
               //Kosong

            }
            else
            {
                Bitmap b = BitmapFactory.DecodeResource(Resources, Resource.Id.imageView1);

                imageView1.SetImageBitmap(b);
                // Counter used to keep track of what bit we are at
                int dataWriteCtr = 0;
                // The data to be written
                char[] data = _binImg.ToCharArray();

                // Length of the data
                int msgLenWriteCtr = 0;

                // Convert length of the data into binary
                char[] dataLen = Convert.ToString(data.Length, 2).PadLeft(24, '0').ToCharArray();

                // We only write the length of the data in the last 6 pixels, max message is 16777215 bits.
                if (data.Length > 16777215) // Last 6 pixels
                    return;

                // Image to have data written too.
               // var imageView1 = (Bitmap)imageView.Resources;

                // If the data is too big to be stored inside the image, return.
                // We -6 because the last 6 pixels store the data length
                if ((data.Length / 4) > (this.imageView1.Width * this.imageView1.Height) - 6)
                    return;

                // Used in changing pixels in the image
                var newPixel = new Pixel();
                // Loop over every pixel but the last 2
                for (int x = 0; x < this.imageView1.Width; x++)
                {
                    for (int y = 0; y < this.imageView1.Height; y++)
                    {
                        Color pixel = new Color(b.GetPixel(x, y));

                        int A = Color.GetAlphaComponent(pixel);
                        int R = Color.GetRedComponent(pixel);
                        int G = Color.GetGreenComponent(pixel);
                        int B = Color.GetBlueComponent(pixel);

                        if (ProcessingImageLastSixPixels(b, x, y))
                        {
                            newPixel.A = SetPixelChannel(pixel.A, dataLen, ref msgLenWriteCtr);
                            newPixel.R = SetPixelChannel(pixel.R, dataLen, ref msgLenWriteCtr);
                            newPixel.G = SetPixelChannel(pixel.G, dataLen, ref msgLenWriteCtr);
                            newPixel.B = SetPixelChannel(pixel.B, dataLen, ref msgLenWriteCtr);
                        }
                        else if (dataWriteCtr < data.Length)
                        {
                            newPixel.A = SetPixelChannel(pixel.A, data, ref dataWriteCtr);
                            newPixel.R = SetPixelChannel(pixel.R, data, ref dataWriteCtr);
                            newPixel.G = SetPixelChannel(pixel.G, data, ref dataWriteCtr);
                            newPixel.B = SetPixelChannel(pixel.B, data, ref dataWriteCtr);
                        }
                        /*else // Uncomment if you want to show what pixels are being modified
                        {
                            newPixel.A = 255;
                            newPixel.R = 255;
                            newPixel.G = 255;
                            newPixel.B = 255;
                        }*/

                        b.SetPixel(x, y, Color.From(newPixel.A, newPixel.R, newPixel.G, newPixel.B));
                    }
                }
            }
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
                imageView1.SetImageURI(uri);
            }
            else
            {
                base.OnActivityResult(requestCode, resultCode, data);
                Bitmap bitmap = (Bitmap)data.Extras.Get("data");
                if (bitmap != null)
                {
                    imageView.SetImageBitmap(bitmap);
                }
            }
            
        }
        private static bool ProcessingImageLastSixPixels(Bitmap img, int x, int y)
        {
            // Images are 0-x indexed so to get to the last 6 pixels we take 
            // 6 + 1 from the width and 1 + 1 from the height to get to those pixels.
            return x > (img.Width - 7) && y > (img.Height - 2);
        }

        /// <summary>
        /// Store one bit in a channel of a pixel
        /// There is 4 channels, Alpha, Red, Green, Blue.
        /// </summary>
        /// <param name="currPixelChannel"></param>
        /// <param name="data"></param>
        /// <param name="msgWriteCtr"></param>
        /// <returns></returns>
        private static int SetPixelChannel(byte currPixelChannel, char[] data, ref int msgWriteCtr)
        {
            int newPixelChannel;

            // If the current pixel's channel value is odd
            // then we want to check the msg for the current bit
            if (currPixelChannel % 2 == 1)
            {
                // If the bit we want to write is 1
                if (data[msgWriteCtr++] == '1')
                {
                    // save the Alpha value for later
                    newPixelChannel = currPixelChannel;
                }
                else // its 0
                {
                    // change the Alpha value by 1 and save for later
                    newPixelChannel = currPixelChannel - 1;
                }
            }
            else // its even
            {
                // if the bit we want to write is 1
                if (data[msgWriteCtr++] == '1')
                {
                    // change the Alpha value by 1 and save for later
                    newPixelChannel = currPixelChannel + 1;
                }
                else // its 0
                {
                    // save the Alpha value for later
                    newPixelChannel = currPixelChannel;
                }
            }

            return newPixelChannel;
        }


        private void BtnCamera_click(object sender, EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, 0);
        }
    }
}