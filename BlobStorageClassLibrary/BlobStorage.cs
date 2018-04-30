/* 
 * ©2018 Tomoyuki Sasaki All rights reserved.
 * 本コンテンツの著作権、および本コンテンツ中に出てくる商標権、団体名、ロゴ、製品、サービスなどはそれぞれ、各権利保有者に帰属します。
 */

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using AzureStorageClassLibrary.Models;

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
        /// <summary>
        /// Storageアカウント確保
        /// </summary>
        private CloudStorageAccount storageAccount { get; set; }
        /// <summary>
        /// blob クライアント確保
        /// </summary>
        private CloudBlobClient blobClient { get; set; }
        #endregion

        /// <summary>
        /// 初期化 
        /// </summary>
        /// <param name="connectionString">接続詞</param>
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
        /// ルートのみ
        /// </summary>
        /// <returns></returns>
        public async Task<List<BlobListDataModel>> BlobGetListAsync(string containerName)
        {
            // 一覧確保
             List<BlobListDataModel> models = new List<BlobListDataModel>();

            var container = blobClient.GetContainerReference(containerName);
            BlobContinuationToken blobtoken = null;

            do
            {
                var blobResult = await container.ListBlobsSegmentedAsync(blobtoken);

                // 継続トークンの取得
                blobtoken = blobResult.ContinuationToken;

                // 取得情報をList取得
                var blobList = blobResult.Results.ToList();

                foreach(var item in blobList)
                {
                    var blobListDataModel = new BlobListDataModel();

                    var blobTypeName = item.GetType().ToString().Split('.').Last();

                    if (item is CloudBlobDirectory)
                    {
                        // 取得はディレクトリ
                        blobListDataModel.name = ((CloudBlobDirectory)item).Prefix.Trim('/');
                        blobListDataModel.contentType = "Folder";
                        blobListDataModel.blobType = blobTypeName;
                        blobListDataModel.size = null;
                        blobListDataModel.lastModified = null;

                        models.Add(blobListDataModel);
                    }
                    else if (item is CloudPageBlob)
                    {
                        // 取得はPage
                        // 未実装
                    }
                    else if (item is CloudBlockBlob)
                    {
                        // 取得はBlock
                        blobListDataModel.name = ((CloudBlockBlob)item).Name;
                        blobListDataModel.contentType = ((CloudBlockBlob)item).Properties.ContentType;
                        blobListDataModel.blobType = blobTypeName;
                        blobListDataModel.size = ((CloudBlockBlob)item).Properties.Length;
                        blobListDataModel.lastModified = ((CloudBlockBlob)item).Properties.LastModified.Value.DateTime;

                        models.Add(blobListDataModel);
                    }
                    else
                    {
                        // 不明
                    }
                }
            } while (blobtoken != null);

            return models;
        }

        // Blob binary upload
        public async Task PutBlobBinaryAsync(string containerName, string putFileName, byte[] putData)
        {
            var container = blobClient.GetContainerReference(containerName);
            var cloudBlockBlob = container.GetBlockBlobReference(putFileName);

            using (var sourceData = new MemoryStream(putData))
            {
                await cloudBlockBlob.UploadFromStreamAsync(sourceData);
            }
        }

        // Blob binary download
        public async Task<byte[]> GetBlobBinaryAsync(string containerName, string getFileName)
        {
            byte[] retData = new byte[] { };

            var container = blobClient.GetContainerReference(containerName);
            var cloudBlockBlob = container.GetBlockBlobReference(getFileName);

            using (var destData = new MemoryStream())
            {
                await cloudBlockBlob.DownloadToStreamAsync(destData);

                retData = destData.ToArray();
            }

            return retData;
        }
    }
}
