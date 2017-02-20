using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace uwppicturestrorage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void addPhoto()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile userPhoto = await picker.PickSingleFileAsync();
            if (userPhoto != null)
            {
                var filestream = await userPhoto.OpenAsync(FileAccessMode.Read);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(filestream);
                TakenPicture.Source = bitmapImage;
                saveUserPhoto(userPhoto);
            }
        }

        private CloudStorageAccount createStorageAccountFromConnectionString()
        {
            var localSettings = ApplicationData.Current.LocalSettings;

            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse("primary connection string");
            }
            catch (FormatException)
            {
                throw new FormatException("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
            }

            return storageAccount;
        }

        private async void saveUserPhoto(StorageFile photo)
        {
            CloudStorageAccount storageAccount = createStorageAccountFromConnectionString();
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("photoscontainer");
            await container.CreateIfNotExistsAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference(photo.Name);
            await blob.UploadFromFileAsync(photo);
            MessageDialog dialog = new MessageDialog("Photo sucessfully uploaded!");
            await dialog.ShowAsync();
        }

        private void PictureButton_Click(object sender, RoutedEventArgs e)
        {
            addPhoto();
        }
    }
}
