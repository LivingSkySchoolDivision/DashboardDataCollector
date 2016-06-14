using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace LSKYDashboardDataCollector.FileSystem
{
    public class PhotoDirectoryRepository
    {
        private readonly List<string> _fileNames;

        public PhotoDirectoryRepository(string galleryName)
        {
            _fileNames = new List<string>();
            
            if (Directory.Exists(Settings.GetPhotoGalleryRootFileSystemDirectory() + @"\" + galleryName))
            {
                foreach (string fileName in Directory.GetFiles(Settings.GetPhotoGalleryRootFileSystemDirectory() + @"\" + galleryName))
                {
                    _fileNames.Add(Settings.GetPhotoGalleryRootWebDirectory() + @"/" + galleryName + @"/" + fileName.Substring(fileName.LastIndexOf(@"\") + 1));
                }
            }
        }

        public List<string> FileNames()
        {
            return _fileNames;
        } 
    }
}