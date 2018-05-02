/* 
 * ©2018 Tomoyuki Sasaki All rights reserved.
 * 本コンテンツの著作権、および本コンテンツ中に出てくる商標権、団体名、ロゴ、製品、サービスなどはそれぞれ、各権利保有者に帰属します。
 */

using AzureStorageClassLibrary.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AzureStorageClassLibrary.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.Streams;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace StorageUWPApps
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private BlobStorage blob;

        private BlobListDataModel selectBlobData = new BlobListDataModel();

        public MainPage()
        {
            this.InitializeComponent();

            // サンプルのためべた書きしていますが、このような書き方はセキュリティホールになるため、行わないでください。
            blob = new BlobStorage("DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;" +
            "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
            "BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;" +
            "TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;" +
            "QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;");

        }

        /// <summary>
        /// アイテム選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_ItemClick(object sender, ItemClickEventArgs e)
        {
            selectBlobData = (BlobListDataModel)e.ClickedItem;
        }

        /// <summary>
        /// Blob一覧取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetListButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(ContainerText.Text)) return;

            if (await blob.BlobContainerExistsAsync(ContainerText.Text))
            {
                var dataList = await blob.BlobGetListAsync(ContainerText.Text);

                if (dataList.Count > 0)
                {
                    dataListView.ItemsSource = new ObservableCollection<BlobListDataModel>(dataList);
                }
                else
                {
                    var msg = new ContentDialog();
                    msg.Title = "Container";
                    msg.Content = $"「{ContainerText.Text}」にアイテムはありませんでした。";
                    msg.PrimaryButtonText = "OK";
                    await msg.ShowAsync();
                }
            }
            else
            {
                var msg = new ContentDialog();
                msg.Title = "Container";
                msg.Content = $"「{ContainerText.Text}」は存在しません。";
                msg.PrimaryButtonText = "OK";
                await msg.ShowAsync();
            }
        }

        /// <summary>
        /// blob Container作成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ContainerCreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(ContainerText.Text)) return;

            if (await blob.BlobContainerExistsAsync(ContainerText.Text))
            {
                var msg = new ContentDialog();
                msg.Title = "Container";
                msg.Content = $"「{ContainerText.Text}」は既に存在しています。";
                msg.PrimaryButtonText = "OK";
                await msg.ShowAsync();
            }
            else
            {
                var msg = new ContentDialog();
                msg.Title = "Container";
                msg.Content = $"「{ContainerText.Text}」を作成してよろしいですか？";
                msg.PrimaryButtonText = "OK";
                msg.SecondaryButtonText = "NO";

                var res = await msg.ShowAsync();

                if (res == ContentDialogResult.Primary)
                {
                    await blob.BlobContainerCreateAsync(ContainerText.Text);
                }
            }
        }

        /// <summary>
        /// blob item　ダウンロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectBlobData == null || selectBlobData.blobType == "")
            {
                // アイテムを選択していない
                var msg = new ContentDialog();
                msg.Title = "Item";
                msg.Content = "一覧からダウンロードするアイテムを選択してください。";
                msg.PrimaryButtonText = "OK";
                await msg.ShowAsync();
            }
            else if (selectBlobData.contentType.ToString().ToLower().Equals(("Folder").ToLower()))
            {
                // フォルダを選択
                var msg = new ContentDialog();
                msg.Title = "Item";
                msg.Content = "フォルダはダウンロードできません。";
                msg.PrimaryButtonText = "OK";
                await msg.ShowAsync();
            }
            else
            {
                // アイテムが選択されている

                var extName = Path.GetExtension(selectBlobData.name);

                // ファイルを選択
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();

                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

                savePicker.FileTypeChoices.Add($"{selectBlobData.contentType}", new List<string>() { $"{extName}" });

                savePicker.SuggestedFileName = $"{selectBlobData.name}";

                Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    Windows.Storage.CachedFileManager.DeferUpdates(file);

                    var data = await blob.GetBlobBinaryAsync(ContainerText.Text, selectBlobData.name);

                    await Windows.Storage.FileIO.WriteBytesAsync(file, data);

                    Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                    if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        Debug.WriteLine($"File {file.Name} was saved.");
                    }
                    else
                    {
                        Debug.WriteLine($"File {file.Name} couldn't be saved.");
                    }
                }
            }
        }

        /// <summary>
        /// アップロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(ContainerText.Text)) return;

            if (await blob.BlobContainerExistsAsync(ContainerText.Text))
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");
                picker.FileTypeFilter.Add(".txt");
                picker.FileTypeFilter.Add(".");

                Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    var stream = await file.OpenAsync(FileAccessMode.Read);
                    var size = stream.Size;
                    byte[] bytes = new byte[size];
                    var reader = new DataReader(stream.GetInputStreamAt(0));
                    await reader.LoadAsync((uint)size);
                    reader.ReadBytes(bytes);
                    await blob.PutBlobBinaryAsync(ContainerText.Text, file.Name, bytes);
                }
                else
                {
                }
            }
            else
            {
                var msg = new ContentDialog();
                msg.Title = "Container";
                msg.Content = $"「{ContainerText.Text}」は存在しません。";
                msg.PrimaryButtonText = "OK";
                await msg.ShowAsync();
            }
        }
    }
}
