using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RenderMentor.DataAccess.Repository.IRepository;
using RenderMentor.Models;
using RenderMentor.Models.ViewModels;
using RenderMentor.Utility;
using RenderMentor.Utility.Helper;

namespace RenderMentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class BlogController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public BlogController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            BlogVM blogVM = new BlogVM()
            {
                Blog = new Blog() {
                    CreateDate = DateTime.Now
                },
                BlogAuthorList = _unitOfWork.BlogAuthor.GetAll().OrderBy(c => c.ListOrder).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if (id == null)
            {
                // Create
                return View(blogVM);
            }
            // Edit
            blogVM.Blog = _unitOfWork.Blog.Get(id.GetValueOrDefault());
            if (blogVM.Blog == null)
            {
                return NotFound();
            }
            return View(blogVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(BlogVM blogVM, IFormFile BlogImage)
        {
            var allObj = _unitOfWork.Blog.GetAll();
            var countObj = allObj.Count();
            var objFromDb = _unitOfWork.Blog.GetFirstOrDefault(i => i.Id == blogVM.Blog.Id, tracking: false);
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                string folderName = "blog";
                if (BlogImage != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    if (blogVM.Blog.Id != 0)
                    {
                        fileName = objFromDb.SEOUrl;
                        ImageUploader.UploadImage(webRootPath, folderName, BlogImage, blogVM.Blog.Image, fileName);
                    }
                    else
                    {
                        fileName = LinkConverter.CreateUrl(blogVM.Blog.Title);
                        ImageUploader.UploadImage(webRootPath, folderName, BlogImage, blogVM.Blog.Image, fileName);
                    }
                    var extension = Path.GetExtension(BlogImage.FileName);
                    string noCache = "?v=" + RandomGenerator.GenerateRandomNo(100, 999);

                    blogVM.Blog.Image = @"\images\" + folderName + @"\" + fileName + extension;
                    var imagePath = Path.Combine(webRootPath, blogVM.Blog.Image.TrimStart('\\'));
                    blogVM.Blog.Image = blogVM.Blog.Image + noCache;
                    var thumbnail = @"\images\" + folderName + @"\" + fileName + "-300x320" + extension;
                    var thumbPath = Path.Combine(webRootPath, thumbnail.TrimStart('\\'));
                    var thumbnailSm = @"\images\" + folderName + @"\" + fileName + "-203x100" + extension;
                    var thumbSmPath = Path.Combine(webRootPath, thumbnailSm.TrimStart('\\'));

                    if (System.IO.File.Exists(thumbPath))
                    {
                        System.IO.File.Delete(thumbPath);
                    }

                    ImageProcessing.CropImage(imagePath, thumbPath, 300, 320);

                    if (System.IO.File.Exists(thumbPath))
                    {
                        blogVM.Blog.Thumbnail = thumbnail + noCache;
                    }

                    if (System.IO.File.Exists(thumbSmPath))
                    {
                        System.IO.File.Delete(thumbSmPath);
                    }

                    ImageProcessing.CropImage(imagePath, thumbSmPath, 203, 100);

                    if (System.IO.File.Exists(thumbSmPath))
                    {
                        blogVM.Blog.ThumbnailSM = thumbnailSm + noCache;
                    }

                }
                else
                {
                    // update when they do not change the image
                    if (blogVM.Blog.Id != 0)
                    {
                        blogVM.Blog.Image = objFromDb.Image;
                    }
                }

                if (blogVM.Blog.Id == 0)
                {
                    blogVM.Blog.SEOUrl = LinkConverter.CreateUrl(blogVM.Blog.Title);
                }
                else if (objFromDb.SEOUrl == null || blogVM.Blog.Title != objFromDb.Title)
                {
                    blogVM.Blog.SEOUrl = LinkConverter.CreateUrl(blogVM.Blog.Title);
                }
                else
                {
                    blogVM.Blog.SEOUrl = objFromDb.SEOUrl;
                }

                if (blogVM.Blog.Id == 0)
                {
                    blogVM.Blog.Published = true;
                    // blog date should not be set automatically when it's changeable
                    //blogVM.Blog.CreateDate = DateTime.Now;
                    blogVM.Blog.ListOrder = 1;
                    foreach (var item in allObj)
                    {
                        item.ListOrder++;
                    }
                    _unitOfWork.Blog.Add(blogVM.Blog);
                }
                else
                {
                    _unitOfWork.Blog.Update(blogVM.Blog);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            blogVM.BlogAuthorList = _unitOfWork.BlogAuthor.GetAll().OrderBy(c => c.ListOrder).Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            if (blogVM.Blog.Id != 0)
            {
                blogVM.Blog = _unitOfWork.Blog.Get(blogVM.Blog.Id);
            }

            return View(blogVM.Blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAuthor(IFormFile avatarFile, string avatar, string name)
        {
            if (name == null || String.IsNullOrEmpty(name.Trim()))
            {
                ModelState.AddModelError("", "Yazar Adı gerekli alandır. Lütfen doldurunuz.");
                return Json(new { success = false, message = "Yazar isim alanı boş bırakılamaz." });
            }
            else
            {
                var allObj = _unitOfWork.BlogAuthor.GetAll();
                var countObj = allObj.Count();
                var author = new BlogAuthor()
                {
                    Name = name,
                    Avatar = avatar,
                    ListOrder = countObj + 1
                };
                if (avatarFile != null)
                {
                    string webRoothPath = _hostEnvironment.WebRootPath;
                    string folderName = "blog" + @"\" + "author";
                    string fileName = LinkConverter.CreateUrl(author.Name);
                    ImageUploader.UploadImage(webRoothPath, folderName, avatarFile, author.Avatar, fileName);
                    var extension = Path.GetExtension(avatarFile.FileName);
                    string noCache = "?v=" + RandomGenerator.GenerateRandomNo(100, 999);
                    author.Avatar = @"\images\" + folderName + @"\" + fileName + extension + noCache;
                }
                _unitOfWork.BlogAuthor.Add(author);
                _unitOfWork.Save();                
            }
            
            return Json(new { success = true, message = "Yazar eklendi." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAuthor(int authorId, IFormFile avatarFile, string avatar, string name)
        {
            if (name == null || String.IsNullOrEmpty(name.Trim()))
            {
                ModelState.AddModelError("", "Yazar Adı gerekli alandır. Lütfen doldurunuz.");
                return Json(new { success = false, message = "Yazar isim alanı boş bırakılamaz." });
            }
            else
            {
                var author = _unitOfWork.BlogAuthor.Get(authorId);
                var allObj = _unitOfWork.BlogAuthor.GetAll();
                var countObj = allObj.Count();
                author.Name = name;
                author.Avatar = avatar;
                string webRoothPath = _hostEnvironment.WebRootPath;
                string folderName = "blog" + @"\" + "author";
                if (avatarFile != null)
                {
                    string fileName = LinkConverter.CreateUrl(author.Name);
                    ImageUploader.UploadImage(webRoothPath, folderName, avatarFile, author.Avatar, fileName);
                    var extension = Path.GetExtension(avatarFile.FileName);
                    string noCache = "?v=" + RandomGenerator.GenerateRandomNo(100, 999);
                    author.Avatar = @"\images\" + folderName + @"\" + fileName + extension + noCache;
                }
                _unitOfWork.BlogAuthor.Update(author);
                _unitOfWork.Save();                
            }
            return Json(new { success = true, message = "Yazar düzenlendi." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAudio(IFormFile BlogAudioFile, string BlogAudio, string audioTitle, string audioDesc, int blogId)
        {
            var allObj = _unitOfWork.BlogAudio.GetAll().Where(m => m.BlogId == blogId);
            var countObj = allObj.Count();
            var blogAudio = new BlogAudio()
            {
                Title = audioTitle,
                Desc = audioDesc,
                Audio = BlogAudio,
                BlogId = blogId,
                ListOrder = countObj + 1
            };
            if (BlogAudioFile != null)
            {
                string webRoothPath = _hostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(BlogAudioFile.FileName);
                string[] extensions = { ".mp3", ".wav" };
                if (!extensions.Contains(extension))
                {
                    return Json(new { success = false, message = "Lütfen .mp3 veya .wav formatında bir ses dosyası ekleyiniz." });
                }
                ImageUploader.UploadAudio(webRoothPath, BlogAudioFile, blogAudio.Audio, fileName);
                blogAudio.Audio = @"\audio\" + fileName + extension;
            }
            _unitOfWork.BlogAudio.Add(blogAudio);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Podcast eklendi." });
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditAudio(int blogAudioId, IFormFile BlogAudioFile, string BlogAudio, string audioTitle, string audioDesc)
        {
            var blogAudio = _unitOfWork.BlogAudio.Get(blogAudioId);
            blogAudio.Title = audioTitle;
            blogAudio.Desc = audioDesc;
            blogAudio.Audio = BlogAudio;
            string webRoothPath = _hostEnvironment.WebRootPath;
            if (BlogAudioFile != null)
            {
                string fileName = Guid.NewGuid().ToString();                
                var extension = Path.GetExtension(BlogAudioFile.FileName);
                string[] extensions = { ".mp3", ".wav" };
                if (!extensions.Contains(extension))
                {
                    return Json(new { success = false, message = "Lütfen .mp3 formatında bir ses dosyası ekleyiniz." });
                }
                string webRootPath = _hostEnvironment.WebRootPath;
                string audioPath = "";
                if (blogAudio.Audio != null)
                {
                    audioPath = Path.Combine(webRootPath, blogAudio.Audio.TrimStart('\\'));
                }
                if (System.IO.File.Exists(audioPath))
                {
                    System.IO.File.Delete(audioPath);
                }
                ImageUploader.UploadAudio(webRoothPath, BlogAudioFile, blogAudio.Audio, fileName);
                blogAudio.Audio = @"\audio\" + fileName + extension;
            }
            _unitOfWork.BlogAudio.Update(blogAudio);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Podcast güncellendi." });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Blog.GetAll(includeProperties: "Author");
            return Json(new { data = allObj });
        }

        [HttpGet]
        public IActionResult GetAuthors()
        {
            var allObj = _unitOfWork.BlogAuthor.GetAll();
            return Json(new { data = allObj });
        }

        [HttpGet]
        public IActionResult GetBlogAudios(int id)
        {
            var allObj = _unitOfWork.BlogAudio.GetAll().Where(m => m.BlogId == id);
            return Json(new { data = allObj });
        }

        [HttpPost]
        public IActionResult PublishToggle([FromBody] int id)
        {
            var objFromDb = _unitOfWork.Blog.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Yayınlama durumunda hata oluştu." });
            }
            if (objFromDb.Published == true)
            {
                // user is currently locked, we will unlock them
                objFromDb.Published = false;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Blog yayından kaldırıldı." });
            }
            else
            {
                objFromDb.Published = true;
                _unitOfWork.Save();
                return Json(new { success = true, message = "Blog yayına alındı." });
            }
        }

        [HttpGet]
        public IActionResult FindPickedImages(int id)
        {
            var currentImages = _unitOfWork.BlogThumb.GetAll().Where(i => i.BlogId == id).ToList();
            return new JsonResult(currentImages);
        }

        [HttpPost("Admin/Blog/PickImages/{pickedImages}/{id}")]
        public IActionResult PickImages(string pickedImages, int id)
        {
            string[] values = JsonConvert.DeserializeObject<string[]>(pickedImages);
            int[] idsArray = Array.ConvertAll(values, i => int.Parse(i));
            var currentImages = _unitOfWork.BlogThumb.GetAll().Where(i => i.BlogId == id).ToList();
            if (currentImages.Count() > 0)
            {
                var toDelete = currentImages.Where(i => !idsArray.Contains(i.Id)).ToList();
                foreach (var item in toDelete)
                {
                    item.BlogId = null;
                }
            }

            var toPick = _unitOfWork.BlogThumb.GetAll().Where(i => idsArray.Contains(i.Id)).ToList();
            foreach (var item in toPick)
            {
                item.BlogId = id;
            }

            _unitOfWork.Save();
            return Json(new { success = true, message = "Toplu seçim işlemi başarılı." });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var allObj = _unitOfWork.Blog.GetAll();
            var objFromDb = _unitOfWork.Blog.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            string webRootPath = _hostEnvironment.WebRootPath;
            string imagePath = "";
            string thumbPath = "";
            string thumbSmPath = "";

            if (objFromDb.Image != null)
            {
                imagePath = Path.Combine(webRootPath, objFromDb.Image.TrimStart('\\'));

                // The code below removes noCache (?v=xxx). Because the suffix to prevent browser caching of the value in db won't match with actual image path.
                imagePath = imagePath.Remove(imagePath.Length - 6, 6);
            }
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            if (objFromDb.Thumbnail != null)
            {
                thumbPath = Path.Combine(webRootPath, objFromDb.Thumbnail.TrimStart('\\'));
                thumbPath = thumbPath.Remove(thumbPath.Length - 6, 6);
            }
            if (System.IO.File.Exists(thumbPath))
            {
                System.IO.File.Delete(thumbPath);
            }

            if (objFromDb.ThumbnailSM != null)
            {
                thumbSmPath = Path.Combine(webRootPath, objFromDb.ThumbnailSM.TrimStart('\\'));
                thumbSmPath = thumbSmPath.Remove(thumbSmPath.Length - 6, 6);
            }
            if (System.IO.File.Exists(thumbSmPath))
            {
                System.IO.File.Delete(thumbSmPath);
            }

            _unitOfWork.Blog.Remove(id);
            foreach (var item in allObj)
            {
                if (item.ListOrder > objFromDb.ListOrder)
                {
                    item.ListOrder --;
                }
            }
            _unitOfWork.Save();
            return Json(new { success = true, message = "Silme işlemi başarılı." });
        }

        [HttpGet]
        [Route("findAuthor/{id}")]
        public IActionResult FindAuthor(int id)
        {
            var author = _unitOfWork.BlogAuthor.Get(id);
            return new JsonResult(author);
        }

        [HttpDelete]
        public IActionResult DeleteAuthor(int id)
        {
            var allObj = _unitOfWork.BlogAuthor.GetAll();
            var objFromDb = _unitOfWork.BlogAuthor.Get(id);
            
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }

            var authorBlogs = _unitOfWork.Blog.GetAll().Where(m => m.AuthorId == id);
            if (authorBlogs.Count() != 0)
            {
                foreach (var blog in authorBlogs)
                {
                    blog.AuthorId = null;
                }
            }

            string webRoothPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRoothPath, objFromDb.Avatar.TrimStart('\\'));
            
            // The code below removes noCache (?v=xxx). Because the suffix to prevent browser caching of the value in db won't match with actual image path.
            imagePath = imagePath.Remove(imagePath.Length - 6, 6);

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitOfWork.BlogAuthor.Remove(objFromDb);
            foreach (var item in allObj)
            {
                if (item.ListOrder > objFromDb.ListOrder)
                {
                    item.ListOrder--;
                }
            }
            _unitOfWork.Save();
            return Json(new { success = true, message = "Silme işlemi başarılı." });
        }

        [HttpGet]
        [Route("findBlogAudio/{id}")]
        public IActionResult FindBlogAudio(int id)
        {
            var blogAudio = _unitOfWork.BlogAudio.Get(id);
            return new JsonResult(blogAudio);
        }

        [HttpDelete("Admin/Blog/DeleteBlogAudio/{blogId}/{id}")]
        public IActionResult DeleteBlogAudio(int blogId, int id)
        {
            var allObj = _unitOfWork.BlogAudio.GetAll().Where(m => m.BlogId == blogId);
            var objFromDb = _unitOfWork.BlogAudio.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Silme işleminde hata oluştu." });
            }
            string webRoothPath = _hostEnvironment.WebRootPath;
            var audioPath = Path.Combine(webRoothPath, objFromDb.Audio.TrimStart('\\'));
            if (System.IO.File.Exists(audioPath))
            {
                System.IO.File.Delete(audioPath);
            }
            _unitOfWork.BlogAudio.Remove(objFromDb);
            foreach (var item in allObj)
            {
                if (item.ListOrder > objFromDb.ListOrder)
                {
                    item.ListOrder--;
                }
            }
            _unitOfWork.Save();
            return Json(new { success = true, message = "Silme işlemi başarılı." });
        }

        public void ListOrder(int Id, int fromPosition, int toPosition)
        {
            var allObj = _unitOfWork.Blog.GetAll();
            var changedObj = allObj.First(c => c.ListOrder == Id);

            changedObj.ListOrder = toPosition;

            _unitOfWork.Save();
        }

        public void AuthorListOrder(int Id, int fromPosition, int toPosition)
        {
            var allObj = _unitOfWork.BlogAuthor.GetAll();
            var changedObj = allObj.First(c => c.ListOrder == Id);

            changedObj.ListOrder = toPosition;

            _unitOfWork.Save();
        }

        public void BlogAudioListOrder(int id, int AudioId, int fromPosition, int toPosition)
        {
            var allObj = _unitOfWork.BlogAudio.GetAll().Where(m => m.BlogId == id);
            BlogAudio changedObj = allObj.First(c => c.ListOrder == AudioId);

            changedObj.ListOrder = toPosition;
            _unitOfWork.Save();
        }

        #endregion
    }
}