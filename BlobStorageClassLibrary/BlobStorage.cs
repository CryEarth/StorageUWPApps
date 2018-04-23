/* 
 * ©2018 Tomoyuki Sasaki All rights reserved.
 * 本コンテンツの著作権、および本コンテンツ中に出てくる商標権、団体名、ロゴ、製品、サービスなどはそれぞれ、各権利保有者に帰属します。
 */

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
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
        private CloudStorageAccount storageAccount { get; set; }
        private CloudBlobClient blobClient { get; set; }

        #endregion

        /// <summary>
        /// 初期化 
        /// </summary>
        /// <param name="accountName">接続名</param>
        /// <param name="accessKey">接続Key</param>
        public BlobStorage(string connectionString)
        {
            // 接続設定を確保する
            storageAccount = CloudStorageAccount.Parse(connectionString);

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

        /// <summary>
        /// Blob Container 作成
        /// </summary>
        /// <param name="blobName"></param>
        /// <returns></returns>
        //container作成
        public async Task<bool> BlobContainerCreateAsync(string containerName)
        {
            var container = blobClient.GetContainerReference(containerName);

            var ret = await container.CreateIfNotExistsAsync();

            return ret;
        }

        /// <summary>
        /// Blob List
        /// </summary>
        /// <returns></returns>
        public async Task BlobGetListAsync()
        {
        }

        // Blob binary upload

        // Blob binary download
    }
}
