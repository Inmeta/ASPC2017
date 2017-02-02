using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Microsoft.SharePoint.Client;

namespace GraphConsoleAppV3
{

    public static class UploadUserPhoto
    {
        //sizes for profile pictures
        const int SmallThumbWidth = 48;
        const int MediumThumbWidth = 72;
        const int LargeThumbWidth = 200;

        enum LogLevel { Information, Warning, Error };

        const string UserName = "kh@bahrda.onmicrosoft.com";

        private static SecureString _pwd;

        const string PwdS = "w8ing4dooM";

        const string MySiteUrl = "https://bahrda-my.sharepoint.com";
        const string PicLibUrl =  "/User%20Photos/profilbilder";
        const bool Upload3Thumbs = true;
        const bool CreateSMLThumbs = true;
        const bool EnableLogging = false;

        //static Configuration _appConfig;

        public static string UploadImageToSpo(string PictureName, Stream ProfilePicture)
        {
            _pwd = new SecureString();
            foreach (char c in PwdS) _pwd.AppendChar(c);
           
            try
            {
                string spPhotoPathTemplate = string.Concat(PicLibUrl.TrimEnd('/'), "/{0}_{1}Thumb.jpg"); //path template to photo lib in My Site Host
                string spImageUrl = string.Empty;

                //create SPO Client context to My Site Host
                ClientContext mySiteclientContext = new ClientContext(MySiteUrl);
                //provide auth crendentials using O365 auth
                mySiteclientContext.Credentials = new SharePointOnlineCredentials(UserName, _pwd);

                if (Upload3Thumbs) //just take single input image and upload to photo lib, no resizeing of image
                {
                    spImageUrl = string.Format(spPhotoPathTemplate, PictureName, "M");
                    LogMessage("Uploading single image, no resize, to " + spImageUrl, LogLevel.Information);
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(mySiteclientContext, spImageUrl, ProfilePicture, true);
                }
                else if (Upload3Thumbs && ! CreateSMLThumbs)//upload 3 of the same size image
                {
                    //not pretty code below, but works. Upload same source image 3 times, but with different name
                    // no resizing of any images
                    LogMessage("Uploading threes image to SPO, no resize", LogLevel.Information);

                    spImageUrl = string.Format(spPhotoPathTemplate, PictureName, "M");
                    LogMessage("Uploading medium image to " + spImageUrl, LogLevel.Information);
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(mySiteclientContext, spImageUrl, ProfilePicture, true);

                    ProfilePicture.Seek(0, SeekOrigin.Begin);
                    spImageUrl = string.Format(spPhotoPathTemplate, PictureName, "L");
                    LogMessage("Uploading large image to " + spImageUrl, LogLevel.Information);
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(mySiteclientContext, spImageUrl, ProfilePicture, true);

                    ProfilePicture.Seek(0, SeekOrigin.Begin);
                    spImageUrl = string.Format(spPhotoPathTemplate, PictureName, "S");
                    LogMessage("Uploading small image to " + spImageUrl, LogLevel.Information);
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(mySiteclientContext, spImageUrl, ProfilePicture, true);


                }
                else if (Upload3Thumbs && CreateSMLThumbs) //generate 3 different size thumbs
                {
                    LogMessage("Uploading threes image to SPO, with resizing", LogLevel.Information);
                    //create 3 images based on recommended sizes for SPO
                    //create small size,                   
                    using (Stream smallThumb = ResizeImageSmall(ProfilePicture, SmallThumbWidth))
                    {
                        if (smallThumb != null)
                        {
                            spImageUrl = string.Format(spPhotoPathTemplate, PictureName, "S");
                            LogMessage("Uploading small image to " + spImageUrl, LogLevel.Information);
                            Microsoft.SharePoint.Client.File.SaveBinaryDirect(mySiteclientContext, spImageUrl, smallThumb, true);
                        }
                    }

                    //create medium size
                    using (Stream mediumThumb = ResizeImageSmall(ProfilePicture, MediumThumbWidth))
                    {
                        if (mediumThumb != null)
                        {
                            spImageUrl = string.Format(spPhotoPathTemplate, PictureName, "M");
                            LogMessage("Uploading medium image to " + spImageUrl, LogLevel.Information);
                            Microsoft.SharePoint.Client.File.SaveBinaryDirect(mySiteclientContext, spImageUrl, mediumThumb, true);

                        }
                    }

                    //create large size image, shown on SkyDrive Pro main page for user
                    using (Stream largeThumb = ResizeImageLarge(ProfilePicture, LargeThumbWidth))
                    {
                        if (largeThumb != null)
                        {

                            spImageUrl = string.Format(spPhotoPathTemplate, PictureName, "L");
                            LogMessage("Uploading large image to " + spImageUrl, LogLevel.Information);
                            Microsoft.SharePoint.Client.File.SaveBinaryDirect(mySiteclientContext, spImageUrl, largeThumb, true);

                        }
                    }


                }
                //return medium sized URL, as this is the one that should be set in the user profile
                return MySiteUrl + string.Format(spPhotoPathTemplate, PictureName, "M");

            }
            catch (Exception ex)
            {
                LogMessage("User Error: Failed to upload thumbnail picture to SPO for " + PictureName + " " + ex.Message, LogLevel.Error);
                return string.Empty;
            }

        }
        /// <summary>
        /// Resize image stream to width passed into function. Will use source image dimension to scale image correctly
        /// </summary>
        /// <param name="OriginalImage"></param>
        /// <param name="NewWidth">New image size width in pixels</param>
        /// <returns></returns>
        private static Stream ResizeImageSmall(Stream OriginalImage, int NewWidth)
        {

            //when resizing large images i.e. bigger than 200px, we lose quality using the GetThumbnailImage method. There are better ways to do this, but will look to imporve in a future version
            // e.g. http://stackoverflow.com/questions/87753/resizing-an-image-without-losing-any-quality
            try
            {
                OriginalImage.Seek(0, SeekOrigin.Begin);
                Image originalImage = Image.FromStream(OriginalImage, true, true);
                if (originalImage.Width == NewWidth) //if sourceimage is same as destination, no point resizing, as it loses quality
                {
                    OriginalImage.Seek(0, SeekOrigin.Begin);
                    originalImage.Dispose();
                    return OriginalImage; //return same image that was passed in
                }
                else
                {
                    Image resizedImage = originalImage.GetThumbnailImage(NewWidth, (NewWidth * originalImage.Height) / originalImage.Width, null, IntPtr.Zero);
                    MemoryStream memStream = new MemoryStream();
                    resizedImage.Save(memStream, ImageFormat.Jpeg);
                    resizedImage.Dispose();
                    originalImage.Dispose();
                    memStream.Seek(0, SeekOrigin.Begin);
                    return memStream;
                }


            }
            catch (Exception ex)
            {
                LogMessage("User Error: cannot create resized image to new width of " + NewWidth.ToString() + ex.Message, LogLevel.Error);
                return null;
            }
        }


        /// <summary>
        /// Delivers better quality image for scaling large thumbs e.g. 200px in width
        /// </summary>
        /// <param name="OriginalImage"></param>
        /// <param name="NewWidth"></param>
        /// <returns></returns>
        private static Stream ResizeImageLarge(Stream OriginalImage, int NewWidth)
        {
            OriginalImage.Seek(0, SeekOrigin.Begin);
            Image originalImage = Image.FromStream(OriginalImage, true, true);
            int newHeight = (NewWidth * originalImage.Height) / originalImage.Width;

            Bitmap newImage = new Bitmap(NewWidth, newHeight);

            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(originalImage, new Rectangle(0, 0, NewWidth, newHeight)); //copy to new bitmap
            }


            MemoryStream memStream = new MemoryStream();
            newImage.Save(memStream, ImageFormat.Jpeg);
            originalImage.Dispose();
            memStream.Seek(0, SeekOrigin.Begin);
            return memStream;


        }

        /// <summary>
        /// Help funtion to log messages to the console window, and to a text file. Log level is currently not used other than for display colors in console
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Level"></param>
        private static void LogMessage(string Message, LogLevel Level)
        {
            //maybe write to log where image failed to upload or profile picture
            switch (Level)
            {
                case LogLevel.Error: Console.ForegroundColor = ConsoleColor.Red; break;
                case LogLevel.Warning: Console.ForegroundColor = ConsoleColor.Green; break;
                case LogLevel.Information: Console.ForegroundColor = ConsoleColor.White; break;

            }

            Console.WriteLine(Message);
            Console.ResetColor();

            try
            {
                
                if (EnableLogging) //check if logging is enabled in configuration file
                {
                    System.IO.File.AppendAllText(Environment.CurrentDirectory, Environment.NewLine + DateTime.Now + " : " + Message);
                }

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error writing to log file. " + ex.Message);
                Console.ResetColor();
            }
        }
    }
}
