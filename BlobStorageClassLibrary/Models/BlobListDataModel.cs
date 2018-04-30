using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageClassLibrary.Models
{
    public class BlobListDataModel: INotifyPropertyChanged
    {
        public string name { get; set; }
        public string blobType { get; set; }
        public string contentType { get; set; }
        public long? size { get; set; }
        public DateTime? lastModified { get; set; }

        public BlobListDataModel()
        {
            name = String.Empty;
            blobType = String.Empty;
            contentType = String.Empty;
            size = null;
            lastModified = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
