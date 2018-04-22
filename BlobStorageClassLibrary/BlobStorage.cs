using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Storageをクラスライブラリ化しておく
/// </summary>
namespace AzureStorageClassLibrary.Blob
{
    /// <summary>
    /// Azure Blob Storage Library
    /// </summary>
    public class BlobStorage
    {
        #region
        // 変数
        private StorageCredentials credentials { get; set; }
        private CloudStorageAccount storageAccount { get; set; }
        private CloudBlobClient blobClient { get; set; }

        #endregion

        /// <summary>
        /// 初期化 
        /// </summary>
        /// <param name="accountName">接続名</param>
        /// <param name="accessKey">接続Key</param>
        public BlobStorage(string accountName, string accessKey)
        {
            // 接続設定を確保する
            credentials = new StorageCredentials(accountName, accessKey);
            storageAccount = new CloudStorageAccount(credentials, true);

            // Blobクライアントの作成
            blobClient = storageAccount.CreateCloudBlobClient();
        }

        /// <summary>
        /// container確認
        /// </summary>
        /// <param name="containerName">確認するコンテナ名</param>
        /// <returns></returns>
        public async Task<bool> BlobContainerExistsAsync(string containerName)
        {
            var container = blobClient.GetContainerReference(containerName);

            var ret = await container.ExistsAsync();

            return ret;
        }

        //container作成

        // Blob List

        // Blob binary upload

        // Blob binary download
    }
}
