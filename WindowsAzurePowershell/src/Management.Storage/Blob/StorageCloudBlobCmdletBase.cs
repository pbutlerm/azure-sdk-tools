﻿﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Management.Storage.Common
{
    using Microsoft.WindowsAzure.Management.Storage.Model.Contract;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Text;

    /// <summary>
    /// Base cmdlet for storage blob/container cmdlet
    /// </summary>
    public class StorageCloudBlobCmdletBase : StorageCloudCmdletBase<IStorageBlobManagement>
    {
        /// <summary>
        /// Initializes a new instance of the StorageCloudBlobCmdletBase class.
        /// </summary>
        public StorageCloudBlobCmdletBase()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StorageCloudBlobCmdletBase class.
        /// </summary>
        /// <param name="channel">IStorageBlobManagement channel</param>
        public StorageCloudBlobCmdletBase(IStorageBlobManagement channel)
        {
            Channel = channel;
        }

        /// <summary>
        /// Make sure the pipeline blob is valid and already existing
        /// </summary>
        /// <param name="blob">ICloudBlob object</param>
        internal void ValidatePipelineICloudBlob(ICloudBlob blob)
        {
            if (null == blob)
            {
                throw new ArgumentException(String.Format(Resources.ObjectCannotBeNull, typeof(ICloudBlob).Name));
            }

            if (!NameUtil.IsValidBlobName(blob.Name))
            {
                throw new ArgumentException(String.Format(Resources.InvalidBlobName, blob.Name));
            }

            ValidatePipelineCloudBlobContainer(blob.Container);
            BlobRequestOptions requestOptions = null;

            if (!Channel.DoesBlobExist(blob, requestOptions, OperationContext))
            {
                throw new ResourceNotFoundException(String.Format(Resources.BlobNotFound, blob.Name, blob.Container.Name));
            }
        }

        /// <summary>
        /// Make sure the container is valid and already existing 
        /// </summary>
        /// <param name="container">A CloudBlobContainer object</param>
        internal void ValidatePipelineCloudBlobContainer(CloudBlobContainer container)
        {
            if (null == container)
            {
                throw new ArgumentException(String.Format(Resources.ObjectCannotBeNull, typeof(CloudBlobContainer).Name));
            }

            if (!NameUtil.IsValidContainerName(container.Name))
            {
                throw new ArgumentException(String.Format(Resources.InvalidContainerName, container.Name));
            }

            BlobRequestOptions requestOptions = null;

            if (!Channel.DoesContainerExist(container, requestOptions, OperationContext))
            {
                throw new ResourceNotFoundException(String.Format(Resources.ContainerNotFound, container.Name));
            }
        }

        /// <summary>
        /// Get blob client
        /// </summary>
        /// <returns>CloudBlobClient with default retry policy and settings</returns>
        internal CloudBlobClient GetCloudBlobClient()
        {
            //Use the default retry policy in storage client
            CloudStorageAccount account = GetCloudStorageAccount();
            return account.CreateCloudBlobClient();
        }

        /// <summary>
        /// Create blob client and storage service management channel if need to.
        /// </summary>
        /// <returns>IStorageManagement object</returns>
        protected override IStorageBlobManagement CreateChannel()
        {
            //Init storage blob managment channel
            if (Channel == null || !ShareChannel)
            {
                Channel = new StorageBlobManagement(GetCloudBlobClient());
            }

            return Channel;
        }
    }
}