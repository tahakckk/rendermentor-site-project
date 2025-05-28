using Microsoft.AspNetCore.Http;
using System.IO;

namespace RenderMentor.Utility.Helper
{
    public class ImageUploader
    {
        public static void UploadImage(string webRoothPath, string folderName, IFormFile uploaded, string imageUrl, string fileName)
        {
            var uploads = Path.Combine(webRoothPath, @"images\" + folderName);
            var extension = Path.GetExtension(uploaded.FileName);
            if (imageUrl != null)
            {
                // this is an edit and we need to remove old image
                var imagePath = Path.Combine(webRoothPath, imageUrl.TrimStart('\\'));
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
            using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
            {
                uploaded.CopyTo(fileStreams);
            }
        }

        public static void UploadAudio(string webRoothPath, IFormFile uploaded, string audioUrl, string fileName)
        {
            var uploads = Path.Combine(webRoothPath, @"audio\");
            var extension = Path.GetExtension(uploaded.FileName);
            if (audioUrl != null)
            {
                // this is an edit and we need to remove old image
                var audioPath = Path.Combine(webRoothPath, audioUrl.TrimStart('\\'));
                if (File.Exists(audioPath))
                {
                    File.Delete(audioPath);
                }
            }
            using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
            {
                uploaded.CopyTo(fileStreams);
            }
        }
    }
}
